using System.Threading.Tasks;
using TitanBot2.Models;
using TitanBot2.Responses;
using TitanBot2.Services.CommandService;

namespace TitanBot2.TypeReaders.Readers
{
    public class EquipmentTypeReader : TypeReader
    {
        public override async Task<TypeReaderResponse> Read(CmdContext context, string value)
        {
            var equip = Equipment.Find(value);
            if (equip == null)
                return TypeReaderResponse.FromError($"`{value}` is not a valid equipment");

            var equipment = await context.TT2DataService.GetEquipment(equip);
            if (equipment == null)
                return TypeReaderResponse.FromError($"Could not download data for equipment `{equip.Id}`");
            return TypeReaderResponse.FromSuccess(equipment);
        }
    }
}
