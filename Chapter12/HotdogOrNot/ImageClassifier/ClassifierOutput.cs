
namespace HotdogOrNot.ImageClassifier;

internal sealed class ClassifierOutput
{
    public string TopResultLabel { get; private set; }
    public float TopResultScore { get; private set; }
    public IDictionary<string, float> LabelScores { get; private set; }

    public byte[] Image { get; private set; }

    ClassifierOutput() { }

    public static ClassifierOutput Create(string topLabel, IDictionary<string, float> labelScores, byte[] image)
    {
        var topLabelValue = topLabel ?? throw new ArgumentException(nameof(topLabel));
        var labelScoresValue = labelScores ?? throw new ArgumentException(nameof(labelScores));

        return new ClassifierOutput
        {
            TopResultLabel = topLabelValue,
            TopResultScore = labelScoresValue.First(i => i.Key == topLabelValue).Value,
            LabelScores = labelScoresValue,
            Image = image,
        };
    }
}
