using Titansmasher.Services.Display;
using Titansmasher.Services.Display.Interfaces;

namespace Titanbot.Commands
{
    public static class ReplyMessage
    {
        #region Statics

        public static readonly string SuccessKey = "replytype.success";
        public static readonly string InfoKey = "replytype.info";
        public static readonly string ErrorKey = "replytype.error";

        public static IDisplayable<string> Success(IDisplayable<string> message)
            => Create(SuccessKey, message);

        public static IDisplayable<string> Error(IDisplayable<string> message)
            => Create(InfoKey, message);

        public static IDisplayable<string> Info(IDisplayable<string> message)
            => Create(ErrorKey, message);

        public static IDisplayable<string> Create(string replyTypeKay, IDisplayable<string> message)
            => new Translation(replyTypeKay, message);

        #endregion Statics
    }
}