namespace UnoOnnx.Platform.Prediction.Engine
{
    public class Settings : UnoOnnx.Prediction.ISettings
    {
        public static readonly UnoOnnx.Prediction.ISettings Default = new Settings();

        public string ModelPath => "./Assets/model-optimized-quantized.onnx"; // "./Assets/distilbert-base-cased-finetuned-conll03-english.onnx";

        public string ConfigPath => "./Assets/config.json";

        public string VocabPath => "./Assets/vocab.txt";

        public string[] ModelInput => new[] { "input_ids", "attention_mask" };

        public string[] ModelOutput => new[] { "output_0" };

        public int SequenceLength => 256;
    }
}
