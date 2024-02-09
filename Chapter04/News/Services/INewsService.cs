using News.Models;

namespace News.Services;

public interface INewsService
{
    public Task<NewsResult> GetNews(NewsScope scope);
}
