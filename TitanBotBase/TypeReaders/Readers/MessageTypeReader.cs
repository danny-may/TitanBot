using Discord;
using System.Globalization;
using System.Threading.Tasks;
using TitanBotBase.Commands;

namespace TitanBotBase.TypeReaders
{
    class MessageTypeReader<T> : TypeReader
        where T : class, IMessage
    {

        internal override async Task<TypeReaderResponse> Read(ICommandContext context, string value)
        {
            ulong id;

            //By Id (1.0)
            if (ulong.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out id))
            {
                var msg = await context.Channel.GetMessageAsync(id, CacheMode.CacheOnly).ConfigureAwait(false) as T;
                if (msg != null)
                    return TypeReaderResponse.FromSuccess(msg);
            }

            return TypeReaderResponse.FromError("Message not found.");
        }
    }
}
