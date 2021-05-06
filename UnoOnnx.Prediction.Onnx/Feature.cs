using Microsoft.ML.Data;

namespace UnoOnnx.Prediction.Onnx
{
    public class Feature
    {
        [VectorType(1, 256)]
        [ColumnName("input_ids")]
        public long[] Tokens { get; set; }

        [VectorType(1, 256)]
        [ColumnName("attention_mask")]
        public long[] Attention { get; set; }
    }
}
