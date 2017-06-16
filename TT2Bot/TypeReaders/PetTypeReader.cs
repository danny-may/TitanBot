using System;
using System.Threading.Tasks;
using TitanBotBase.Commands;
using TitanBotBase.TypeReaders;
using TT2Bot.Helpers;
using TT2Bot.Models;

namespace TT2Bot.TypeReaders
{
    public class PetTypeReader : TypeReader
    {
        public override async Task<TypeReaderResponse> Read(ICommandContext context, string value)
        {
            var pet = Pet.Find(value);
            if (pet == null)
                return TypeReaderResponse.FromError($"`{value}` is not a valid pet");

            var dataService = context.DependencyFactory.Get<TT2DataService>();

            var artifact = await dataService.GetPet(pet);
            if (artifact == null)
                return TypeReaderResponse.FromError($"Could not download data for pet `#{pet.Id}`");
            return TypeReaderResponse.FromSuccess(artifact);
        }
    }
}
