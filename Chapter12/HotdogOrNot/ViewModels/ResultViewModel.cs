using CommunityToolkit.Mvvm.ComponentModel;
using HotdogOrNot.Models;

namespace HotdogOrNot.ViewModels;

public partial class ResultViewModel : ObservableObject, IQueryAttributable
{
	[ObservableProperty]
	private string title;

	[ObservableProperty]
	private string description;

	[ObservableProperty]
	byte[] photoBytes;

	public ResultViewModel()
	{
	}

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        Initialize(query["result"] as Result);
    }

    public void Initialize(Result result)
    {
        PhotoBytes = result.PhotoBytes;

        if (result.IsHotdog && result.Confidence > 0.9)
        {
            Title = "Hot dog";
            Description = "This is for sure a hot dog";
        }
        else if (result.IsHotdog)
        {
            Title = "Maybe";
            Description = "This is maybe a hot dog";
        }
        else
        {
            Title = "Not a hot dog";
            Description = "This is not a hot dog";
        }
    }
}

