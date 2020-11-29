using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Infrastructure
{
    public class DBNameProvider
    {
        public string GetDBName(string fileName)
        {
            var path = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            path = path.Remove(path.Length - 28);
            var dbName = $"{path}\\{fileName}.db";
            return dbName;
        }
    }
}
