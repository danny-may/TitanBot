using System.Threading.Tasks;
using TitanBot2.Models;
using TitanBot2.Services.CommandService;

namespace TitanBot2.TypeReaders.Readers
{
    public class ArtifactTypeReader : TypeReader
    {
        public override async Task<TypeReaderResult> Read(TitanbotCmdContext context, string value)
        {
            var art = Artifact.Find(value);
            if (art == null)
                return TypeReaderResult.FromError($"`{value}` is not a valid artifact");

            var artifact = await context.TT2DataService.GetArtifact(art);
            if (artifact == null)
                return TypeReaderResult.FromError($"Could not download data for artifact `#{art.Id}`");
            return TypeReaderResult.FromSuccess(artifact);
        }
    }
}
