using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using SixLabors.ImageSharp.Formats.Png;
using Image = SixLabors.ImageSharp.Image;

namespace HotdogOrNot.ImageClassifier;

internal class MLNetClassifier : IClassifier
{
    readonly InferenceSession session;
    readonly bool isBgr;
    readonly bool isRange255;
    readonly string inputName;
    readonly int inputSize;

    public MLNetClassifier(byte[] model) 
    {
        session = new InferenceSession(model);
        isBgr = session.ModelMetadata.CustomMetadataMap["Image.BitmapPixelFormat"] == "Bgr8";
        isRange255 = session.ModelMetadata.CustomMetadataMap["Image.NominalPixelRange"] == "NominalRange_0_255";
        inputName = session.InputMetadata.Keys.First();
        inputSize = session.InputMetadata[inputName].Dimensions[2];
    }

    public ClassifierOutput Classify(byte[] imageBytes)
    {
        (Tensor<float> tensor, byte[] resizedImage) = LoadInputTensor(imageBytes, inputSize, isBgr, isRange255);

        var resultsCollection = session.Run(new List<NamedOnnxValue>
                {
                    NamedOnnxValue.CreateFromTensor<float>(inputName, tensor)
                });

        var topLabel = resultsCollection
            ?.FirstOrDefault(i => i.Name == "classLabel")
            ?.AsTensor<string>()
            ?.First();

        var labelScores = resultsCollection
            ?.FirstOrDefault(i => i.Name == "loss")
            ?.AsEnumerable<NamedOnnxValue>()
            ?.First()
            ?.AsDictionary<string, float>();

        return ClassifierOutput.Create(topLabel, labelScores, resizedImage);
    }

    static (Tensor<float>, byte[] resizedImage) LoadInputTensor(byte[] imageBytes, int imageSize, bool isBgr, bool isRange255)
    {
        var input = new DenseTensor<float>(new[] { 1, 3, imageSize, imageSize });
        byte[] pixelBytes;

        using (var image = Image.Load<Rgba32>(imageBytes))
        {
            image.Mutate(x => x.Resize(imageSize, imageSize));

            image.ProcessPixelRows(source =>
            {
                for (int y = 0; y < image.Height; y++)
                {
                    Span<Rgba32> pixelSpan = source.GetRowSpan(y);
                    for (int x = 0; x < image.Width; x++)
                    {
                        if (isBgr)
                        {
                            input[0, 0, y, x] = pixelSpan[x].B;
                            input[0, 1, y, x] = pixelSpan[x].G;
                            input[0, 2, y, x] = pixelSpan[x].R;
                        }
                        else
                        {
                            input[0, 0, y, x] = pixelSpan[x].R;
                            input[0, 1, y, x] = pixelSpan[x].G;
                            input[0, 2, y, x] = pixelSpan[x].B;
                        }

                        if (!isRange255)
                        {
                            input[0, 0, y, x] = input[0, 0, y, x] / 255;
                            input[0, 1, y, x] = input[0, 1, y, x] / 255;
                            input[0, 2, y, x] = input[0, 2, y, x] / 255;
                        }
                    }
                }
            });

            var outStream = new MemoryStream();
            image.Save(outStream, new PngEncoder());
            pixelBytes = outStream.ToArray();
        }
        return (input, pixelBytes);
    }
}
