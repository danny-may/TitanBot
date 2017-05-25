using Discord;
using System.Globalization;
using System.Threading.Tasks;
using TitanBot2.Services.CommandService;

namespace TitanBot2.TypeReaders.Readers
{
    public class MessageTypeReader<T> : TypeReader
        where T : class, IMessage
    { 

        public override async Task<TypeReaderResult> Read(CmdContext context, string value)
        {
            ulong id;

            //By Id (1.0)
            if (ulong.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out id))
            {
                var msg = await context.Channel.GetMessageAsync(id, CacheMode.CacheOnly).ConfigureAwait(false) as T;
                if (msg != null)
                    return TypeReaderResult.FromSuccess(msg);
            }

            return TypeReaderResult.FromError("Message not found.");
        }
    }
}
