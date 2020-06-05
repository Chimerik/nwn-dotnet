using System;

namespace NWN.Systems
{
    public static partial class Potager
    {
        public static class Models
        {
            public class PotagerSql
            {
                public int id { get; set; }
                public string type { get; set; }
                public int date { get; set; }
                public string tag { get; set; }
                public string uuid { get; set; }
                public DateTime datePlantage { get; set; }
            }
        }
    }
}
