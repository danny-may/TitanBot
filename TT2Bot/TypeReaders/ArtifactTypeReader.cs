using System.Threading.Tasks;
using TitanBotBase.Commands;
using TitanBotBase.TypeReaders;
using TT2Bot.Helpers;
using TT2Bot.Models;

namespace TT2Bot.TypeReaders
{
    public class ArtifactTypeReader : TypeReader
    {
        public override async Task<TypeReaderResponse> Read(ICommandContext context, string value)
        {
            var art = Artifact.Find(value);
            if (art == null)
                return TypeReaderResponse.FromError($"`{value}` is not a valid artifact");

            var dataService = context.DependencyFactory.Get<TT2DataService>();

            var artifact = await dataService.GetArtifact(art);
            if (artifact == null)
                return TypeReaderResponse.FromError($"Could not download data for artifact `#{art.Id}`");
            return TypeReaderResponse.FromSuccess(artifact);
        }
    }
}
