using System.Threading.Tasks;
using TitanBot2.Models;
using TitanBot2.Responses;
using TitanBot2.Services.CommandService;

namespace TitanBot2.TypeReaders.Readers
{
    public class HelperTypeReader : TypeReader
    {
        public override async Task<TypeReaderResponse> Read(CmdContext context, string value)
        {
            var helper = Helper.Find(value);
            if (helper == null)
                return TypeReaderResponse.FromError($"`{value}` is not a valid hero");

            var hero = await context.TT2DataService.GetHelper(helper);
            if (hero == null)
                return TypeReaderResponse.FromError($"Could not download data for hero `#{helper.Id}`");
            return TypeReaderResponse.FromSuccess(hero);
        }
    }
}
