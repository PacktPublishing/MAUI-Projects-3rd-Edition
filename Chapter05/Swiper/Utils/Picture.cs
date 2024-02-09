namespace Swiper.Utils;

internal class Picture
{
    public Uri Uri { get; init; }
    public string Description { get; init; }

    public Picture()
    {
        Uri = new Uri($"https://picsum.photos/400/400/?random&ts={ DateTime.Now.Ticks }");    
        var generator = new DescriptionGenerator(); 
        Description = generator.Generate();
    }
}
