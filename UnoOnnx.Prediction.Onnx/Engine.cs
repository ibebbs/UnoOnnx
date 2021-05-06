using Microsoft.Extensions.Logging;
using Microsoft.ML;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace UnoOnnx.Prediction.Onnx
{
    public class Engine : IEngine
    {
        private static PredictionEngine<Feature, Inference> CreateModel(ISettings settings)
        {
            var context = new MLContext();

            var dataView = context.Data
                 .LoadFromEnumerable(new List<Feature>());

            var pipeline = context.Transforms
                .ApplyOnnxModel(
                    modelFile: settings.ModelPath,
                    shapeDictionary: new Dictionary<string, int[]>
                    {
                        { "input_ids", new [] { 1, settings.SequenceLength } },
                        { "attention_mask", new [] { 1, settings.SequenceLength } }
                    },
                    inputColumnNames: settings.ModelInput,
                    outputColumnNames: settings.ModelOutput,
                    gpuDeviceId: (int?)null);

            var transformer = pipeline.Fit(dataView);

            var model = context.Model.CreatePredictionEngine<Feature, Inference>(transformer);
            return model;
        }

        private static Tokenizers.WordPieceTokenizer CreateTokenizer(ISettings settings)
        {
            var vocabulary = File.ReadAllLines(settings.VocabPath);

            return new Tokenizers.WordPieceTokenizer(vocabulary);
        }

        private static HuggingFace.Config CreateConfiguration(ISettings settings)
        {
            var config = HuggingFace.Config.FromFile(settings.ConfigPath);

            return config;
        }

        public static IEngine Create(ISettings settings, ILogger logger)
        {
            var model = CreateModel(settings);
            var tokenizer = CreateTokenizer(settings);
            var config = CreateConfiguration(settings);

            return new Engine(model, tokenizer, config, settings, logger);
        }

        private readonly PredictionEngine<Feature, Inference> _model;
        private readonly Tokenizers.WordPieceTokenizer _tokenizer;
        private readonly HuggingFace.Config _configuation;
        private readonly ISettings _settings;
        private readonly ILogger _logger;

        public Engine(PredictionEngine<Feature, Inference> model, Tokenizers.WordPieceTokenizer tokenizer, HuggingFace.Config configuation, ISettings settings, ILogger logger)
        {
            _model = model;
            _tokenizer = tokenizer;
            _configuation = configuation;
            _settings = settings;
            _logger = logger;
        }

        public void Dispose()
        {
            _model.Dispose();
        }

        private static (int WordIndex, string Word, int Category, string Label, float Score) GetWordCategory(HuggingFace.Config config, int wordIndex, string word, IEnumerable<float> values)
        {
            return values
                .Select((v, i) => (Value: v, Index: i))
                .GroupBy(group => group.Index % 9)
                .Select((group, index) => (Category: index, Value: group.Average(g => g.Value)))
                .Where(tuple => tuple.Value > 0.1)
                .OrderByDescending(tuple => tuple.Value)
                .Select(tuple => (wordIndex, word, tuple.Category, config.id2label[tuple.Category.ToString()], tuple.Value))
                .FirstOrDefault();
        }

        public IReadOnlyCollection<Result> Predict(string text)
        {
            _logger.LogInformation("Inferring entities from text: '{0}'", text);

            var tokens = _tokenizer.Tokenize(text).ToArray();

            var padded = tokens.Select(t => (long)t.VocabularyIndex).Concat(Enumerable.Repeat(0L, _settings.SequenceLength - tokens.Length)).ToArray();

            var attentionMask = Enumerable.Repeat(1L, padded.Length).ToArray();

            var feature = new Feature { Tokens = padded, Attention = attentionMask };

            var inference = _model.Predict(feature);

            var results = tokens
                .Zip(inference.Output.Batch(9).ToArray(), (token, values) => (Token: token, Values: values))
                .GroupBy(tuple => (WordIndex: tuple.Token.WordIndex, Word: tuple.Token.Word))
                .Select(group => GetWordCategory(_configuation, group.Key.WordIndex, group.Key.Word, group.SelectMany(g => g.Values)))
                .Where(tuple => tuple.Category > 0)
                .Select(tuple => new Result(tuple.Word, tuple.WordIndex, tuple.Label, tuple.Score))
                .ToArray();

            return results;
        }
    }
}
