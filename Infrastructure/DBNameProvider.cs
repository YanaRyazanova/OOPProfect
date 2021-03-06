﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Infrastructure
{
    public class DBNameProvider
    {
        public string GetDBName(string fileName, string extension = "db")
        {
            var path = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var catalogs = path.Split('\\');
            var dbName = new StringBuilder();
            foreach (var catalog in catalogs)
            {
                dbName.Append($"/{catalog}");
                if (catalog == "OOPProfect")
                {
                    dbName.Append("/Infrastructure/DataBases");
                    break;
                }
            }
            dbName = dbName.Remove(0, 1);
            return dbName.Append($"/{fileName}.{extension}").ToString();
        }
    }
}