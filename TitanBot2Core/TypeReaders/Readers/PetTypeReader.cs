using System.Threading.Tasks;
using TitanBot2.Models;
using TitanBot2.Responses;
using TitanBot2.Services.CommandService;

namespace TitanBot2.TypeReaders.Readers
{
    public class PetTypeReader : TypeReader
    {
        public override async Task<TypeReaderResponse> Read(CmdContext context, string value)
        {
            var pet = Pet.Find(value);
            if (pet == null)
                return TypeReaderResponse.FromError($"`{value}` is not a valid pet");

            var artifact = await context.TT2DataService.GetPet(pet);
            if (artifact == null)
                return TypeReaderResponse.FromError($"Could not download data for pet `#{pet.Id}`");
            return TypeReaderResponse.FromSuccess(artifact);
        }
    }
}
