using UnoOnnx.Prediction;

namespace UnoOnnx.Platform.Prediction.Engine
{
    public static class Factory
    {
        public static IEngine Create()
        {
            return UnoOnnx.Prediction.Onnx.Engine.Create(Settings.Default, global::Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory.CreateLogger("Prediction.Engine"));
        }
    }
}
