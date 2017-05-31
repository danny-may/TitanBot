using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Services.Database.Tables;

namespace TitanBot2.Services.Database.Extensions
{
    public class TT2SubmissionExtensions : DatabaseExtension<TT2Submission>
    {
        public TT2SubmissionExtensions(BotDatabase db) : base(db)
        {
        }
    }
}
