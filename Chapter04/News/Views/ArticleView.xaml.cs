namespace News.Views;

using System.Web;

[QueryProperty("Url", "url")]
public partial class ArticleView : ContentPage
{
    public string Url
    {
        set
        {
            BindingContext = new UrlWebViewSource
            {
                Url = HttpUtility.UrlDecode(value)
            };
        }
    }

    public ArticleView()
	{
		InitializeComponent();
	}
}