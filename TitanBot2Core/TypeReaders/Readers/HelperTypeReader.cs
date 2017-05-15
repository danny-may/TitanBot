using System.Threading.Tasks;
using TitanBot2.Models;
using TitanBot2.Services.CommandService;

namespace TitanBot2.TypeReaders.Readers
{
    public class HelperTypeReader : TypeReader
    {
        public override async Task<TypeReaderResult> Read(TitanbotCmdContext context, string value)
        {
            var helper = Helper.Find(value);
            if (helper == null)
                return TypeReaderResult.FromError($"`{value}` is not a valid hero");

            var hero = await context.TT2DataService.GetHelper(helper);
            if (hero == null)
                return TypeReaderResult.FromError($"Could not download data for hero `#{helper.Id}`");
            return TypeReaderResult.FromSuccess(hero);
        }
    }
}
