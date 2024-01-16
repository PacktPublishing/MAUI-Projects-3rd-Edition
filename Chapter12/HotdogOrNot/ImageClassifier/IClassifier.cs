
namespace HotdogOrNot.ImageClassifier;

internal interface IClassifier
{
    ClassifierOutput Classify(byte[] bytes);
}
