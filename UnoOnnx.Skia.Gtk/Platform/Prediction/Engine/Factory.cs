using Microsoft.Extensions.Logging;
using System;
using UnoOnnx.Prediction;

namespace UnoOnnx.Platform.Prediction.Engine
{
    public static class Factory
    {
        private static readonly Lazy<ILogger> Logger = new Lazy<ILogger>(() => global::Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory.CreateLogger("Prediction.Engine"));

        public static IEngine Create()
        {
            Logger.Value.LogInformation("Creating Prediction Engine: '{0}'", typeof(UnoOnnx.Prediction.Onnx.Engine).FullName);

            return UnoOnnx.Prediction.Onnx.Engine.Create(Settings.Default, Logger.Value);
        }
    }
}
