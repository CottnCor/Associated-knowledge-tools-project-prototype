using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Associated_knowledge_tools_project_prototype.service
{
    public class PaperBasicData
    {
        public int ID { get; set; }
        public Coords Coords { get; set; }
        public string Subject { get; set; }
        public string Author { get; set; }
        public string Location { get; set; }
        public string Organization { get; set; }
        public string KeyWords { get; set; }
    }

    public class Coords
    {
        public double longitude { get; set; }
        public double latitude { get; set; }
    }
}