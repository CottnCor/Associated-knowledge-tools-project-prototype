using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Associated_knowledge_tools_project_prototype.service
{
    public class PaperConcatData
    {
        public int ID { get; set; }
        public List<ConcatPaper> ConcatPaper { get; set; }
    }

    public class ConcatPaper
    {
        public int ID { get; set; }
        public double Rate { get; set; }
    }
}