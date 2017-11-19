namespace Titansmasher.Services.Display.Interfaces
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