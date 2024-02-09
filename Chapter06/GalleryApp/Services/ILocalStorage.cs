namespace GalleryApp.Services;

public interface ILocalStorage
{
    void Store(string filename); 
    List<string> Get();
}
