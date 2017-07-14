using Discord;
using System.Globalization;
using System.Threading.Tasks;
using TitanBot.Commands;

namespace TitanBot.TypeReaders
{
    class MessageTypeReader<T> : TypeReader
        where T : class, IMessage
    {

        public override async Task<TypeReaderResponse> Read(ICommandContext context, string value)
        {
            if (ulong.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out ulong id))
            {
                if (await context.Channel.GetMessageAsync(id, CacheMode.CacheOnly).ConfigureAwait(false) is T msg)
                    return TypeReaderResponse.FromSuccess(msg);
            }

            return TypeReaderResponse.FromError("TYPEREADER_ENTITY_NOTFOUND", value, typeof(T));
        }
    }
}
