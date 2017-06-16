using System.Threading.Tasks;
using TitanBotBase.Commands;
using TitanBotBase.TypeReaders;
using TT2Bot.Helpers;
using TT2Bot.Models;

namespace TT2Bot.TypeReaders
{
    public class HelperTypeReader : TypeReader
    {
        public override async Task<TypeReaderResponse> Read(ICommandContext context, string value)
        {
            var helper = Helper.Find(value);
            if (helper == null)
                return TypeReaderResponse.FromError($"`{value}` is not a valid hero");

            var dataService = context.DependencyFactory.Get<TT2DataService>();

            var hero = await dataService.GetHelper(helper);
            if (hero == null)
                return TypeReaderResponse.FromError($"Could not download data for hero `#{helper.Id}`");
            return TypeReaderResponse.FromSuccess(hero);
        }
    }
}
