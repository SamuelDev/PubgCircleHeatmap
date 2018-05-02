using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircleHeatmapAnalysis
{

    public class Rootobject
    {
        public Data data { get; set; }
    }

    public class Data
    {
        public string type { get; set; }
        public string id { get; set; }
        public Attributes attributes { get; set; }
        public Relationships relationships { get; set; }
    }

    public class Attributes
    {
        public DateTime createdAt { get; set; }
        public string titleId { get; set; }
        public string shardId { get; set; }
    }

    public class Relationships
    {
        public Matches matches { get; set; }
    }

    public class Matches
    {
        public Match[] data { get; set; }
    }

    public class Match
    {
        public string type { get; set; }
        public string id { get; set; }
    }

}
