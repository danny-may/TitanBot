namespace Titansmasher.Services.Displaying.Interfaces
{
    public interface IDisplayable
    {
        object Display(IDisplayService service, DisplayOptions options = default);
    }

    public interface IDisplayable<TOut> : IDisplayable
    {
        new TOut Display(IDisplayService service, DisplayOptions options = default);
    }
}