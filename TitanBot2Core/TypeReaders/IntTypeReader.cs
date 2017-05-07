using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Services.CommandService;

namespace TitanBot2.TypeReaders
{
    public class IntTypeReader : TypeReader<int>
    {
        public IntTypeReader()
        {
            Target = typeof(int);
        }
        public override async Task<TypeReaderResponse<int>> Read(TitanbotCmdContext context, string value)
        {
            int res;
            if (int.TryParse(value, out res))
                return TypeReaderResponse.FromSuccess(res);
            return TypeReaderResponse.FromError<int>($"`{value}` is not of type `int`");
        }
    }
}
