namespace UnoOnnx.Prediction
{
    public interface ISettings
    {
        string ModelPath { get; }

        string ConfigPath { get; }

        string VocabPath { get; }

        string[] ModelInput { get; }

        string[] ModelOutput { get; }

        int SequenceLength { get; }
    }
}
