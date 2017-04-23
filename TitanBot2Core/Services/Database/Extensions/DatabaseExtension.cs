using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBot2.Services.Database.Extensions
{
    public abstract class DatabaseExtension
    {
        protected TitanbotDatabase Database { get; }
        public DatabaseExtension(TitanbotDatabase db)
        {
            Database = db;
        }
    }
}
