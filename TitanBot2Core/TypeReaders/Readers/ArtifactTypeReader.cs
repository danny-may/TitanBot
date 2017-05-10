using System.Threading.Tasks;
using TitanBot2.Models;
using TitanBot2.Services.CommandService;

namespace TitanBot2.TypeReaders.Readers
{
    public class ArtifactTypeReader : TypeReader
    {
        public override async Task<TypeReaderResult> Read(TitanbotCmdContext context, string value)
        {
            var artId = Artifact.FindArtifact(value);
            if (artId == null)
                return TypeReaderResult.FromError($"`{value}` is not a valid artifact");

            var artifact = await context.TT2DataService.GetArtifact(artId.Value);
            if (artifact == null)
                return TypeReaderResult.FromError($"Could not download data for artifact #{artId.Value}");
            return TypeReaderResult.FromSuccess(artifact);
        }
    }
}
