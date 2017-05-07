using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Models;
using TitanBot2.Services.CommandService;

namespace TitanBot2.TypeReaders
{
    public class ArtifactTypeReader : TypeReader<Artifact>
    {
        public ArtifactTypeReader()
        {
            Target = typeof(Artifact);
        }
        public override async Task<TypeReaderResponse<Artifact>> Read(TitanbotCmdContext context, string value)
        {
            var artId = Artifact.FindArtifact(value);
            if (artId == null)
                return TypeReaderResponse.FromError<Artifact>($"`{value}` is not a valud artifact");

            var artifact = await context.TT2DataService.GetArtifact(artId.Value);
            if (artifact == null)
                return TypeReaderResponse.FromError<Artifact>($"Could not download data for artifact #{artId.Value}");
            return TypeReaderResponse.FromSuccess(artifact);
        }
    }
}
