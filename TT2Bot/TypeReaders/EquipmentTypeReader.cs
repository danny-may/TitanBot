using System.Threading.Tasks;
using TitanBotBase.Commands;
using TitanBotBase.TypeReaders;
using TT2Bot.Helpers;
using TT2Bot.Models;

namespace TT2Bot.TypeReaders
{
    public class EquipmentTypeReader : TypeReader
    {
        public override async Task<TypeReaderResponse> Read(ICommandContext context, string value)
        {
            var equip = Equipment.Find(value);
            if (equip == null)
                return TypeReaderResponse.FromError($"`{value}` is not a valid equipment");

            var dataService = context.DependencyFactory.Get<TT2DataService>();

            var equipment = await dataService.GetEquipment(equip);
            if (equipment == null)
                return TypeReaderResponse.FromError($"Could not download data for equipment `{equip.Id}`");
            return TypeReaderResponse.FromSuccess(equipment);
        }
    }
}
