using System.Threading.Tasks;
using TitanBot2.Models;
using TitanBot2.Services.CommandService;

namespace TitanBot2.TypeReaders.Readers
{
    public class PetTypeReader : TypeReader
    {
        public override async Task<TypeReaderResult> Read(CmdContext context, string value)
        {
            var pet = Pet.Find(value);
            if (pet == null)
                return TypeReaderResult.FromError($"`{value}` is not a valid pet");

            var artifact = await context.TT2DataService.GetPet(pet);
            if (artifact == null)
                return TypeReaderResult.FromError($"Could not download data for pet `#{pet.Id}`");
            return TypeReaderResult.FromSuccess(artifact);
        }
    }
}
