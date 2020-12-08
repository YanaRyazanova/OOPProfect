using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;

namespace Infrastructure
{
    class JsonParser
    {
        private readonly DBNameProvider dbNameProvider;

        public JsonParser(DBNameProvider dbNameProvider, string extension)
        {
            this.dbNameProvider = dbNameProvider;
        }

        public Lesson[] ParseJson(string groupName, DateTime day)
        {
            var key = $"{groupName}, {day}";
            throw new NotSupportedException();
        }

        public void LoadJson(string dbName)
        {
            using (StreamReader r = new StreamReader(dbName))
            {
                
                string json = r.ReadToEnd();
                List<Item> items = JsonConvert.DeserializeObject<List<Item>>(json);
                foreach (var el in items)
                    Console.WriteLine(el);
            }
        }
    }
}
