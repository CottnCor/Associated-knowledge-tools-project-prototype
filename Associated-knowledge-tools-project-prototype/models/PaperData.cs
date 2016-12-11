using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Associated_knowledge_tools_project_prototype.service
{
    public class PaperData
    {
        public Dictionary<int, PaperBasicData> PaperBasicCollection { get; set; }
        public Dictionary<int, PaperConcatData> PaperConcatCollection { get; set; }
    }
}