using Microsoft.ML.Data;

namespace UnoOnnx.Prediction.Onnx
{
    public class Inference
    {
        [VectorType(1, 256, 9)]
        [ColumnName("output_0")]
        public float[] Output { get; set; }
    }
}
