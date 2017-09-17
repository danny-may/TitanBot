namespace TitanBot.Core.Models.Contexts
{
    public enum MessageCommandState
    {
        Invalid = 0,
        HasPrefix = 1,
        HasCommand = 2,
        ValidCommand = 4,
    }
}