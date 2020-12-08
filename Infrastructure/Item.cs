using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure
{
    class Item
    {
        [JsonProperty("dayOfWeekAndGroup")]
        public string dayOfWeekAndGroup { get; set; }
    }
}
