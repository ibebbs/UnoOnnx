using System;
using System.Collections.Generic;

namespace UnoOnnx.Prediction
{
    public interface IEngine : IDisposable
    {
        IReadOnlyCollection<Result> Predict(string text);
    }
}
