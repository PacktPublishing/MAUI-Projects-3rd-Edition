namespace News.Services;

using News.Models;
using System.Net.Http.Json;

public class NewsService : INewsService, IDisposable
{
    private bool disposedValue;

    const string UriBase = "https://newsapi.org/v2";
    readonly HttpClient httpClient = new() { 
        BaseAddress = new(UriBase), 
        DefaultRequestHeaders = { { "user-agent", "maui-projects-news/1.0" } }
    };

    public async Task<NewsResult> GetNews(NewsScope scope)
    {
        NewsResult result;
        string url = GetUrl(scope);
        try
        {
            result = await httpClient.GetFromJsonAsync<NewsResult>(url);
        }
        catch (Exception ex) {
            result = new() { Articles = new() { new() { Title = $"HTTP Get failed: {ex.Message}", PublishedAt = DateTime.Now} } };
        }
        return result;
    }

    private string GetUrl(NewsScope scope) => scope switch
    {
        NewsScope.Headlines => Headlines,
        NewsScope.Global => Global,
        NewsScope.Local => Local,
        _ => throw new Exception("Undefined scope")
    };

    private static string Headlines => $"{UriBase}/top-headlines?country=us&apiKey={Settings.NewsApiKey}";

    private static string Local => $"{UriBase}/everything?q=local&apiKey={Settings.NewsApiKey}";

    private static string Global => $"{UriBase}/everything?q=global&apiKey={Settings.NewsApiKey}";

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                httpClient.Dispose();
            }
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
