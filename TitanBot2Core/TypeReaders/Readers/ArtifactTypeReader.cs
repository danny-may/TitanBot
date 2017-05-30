using System.Threading.Tasks;
using TitanBot2.Models;
using TitanBot2.Responses;
using TitanBot2.Services.CommandService;

namespace TitanBot2.TypeReaders.Readers
{
    public class ArtifactTypeReader : TypeReader
    {
        public override async Task<TypeReaderResponse> Read(CmdContext context, string value)
        {
            var art = Artifact.Find(value);
            if (art == null)
                return TypeReaderResponse.FromError($"`{value}` is not a valid artifact");

            var artifact = await context.TT2DataService.GetArtifact(art);
            if (artifact == null)
                return TypeReaderResponse.FromError($"Could not download data for artifact `#{art.Id}`");
            return TypeReaderResponse.FromSuccess(artifact);
        }
    }
}
