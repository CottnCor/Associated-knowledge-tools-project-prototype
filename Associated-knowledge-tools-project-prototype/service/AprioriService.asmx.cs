using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json;
using System.Data;
using DbOperation;

namespace Associated_knowledge_tools_project_prototype.service
{
    /// <summary>
    /// AprioriService1 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]

    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    [System.Web.Script.Services.ScriptService]
    public class AprioriService1 : System.Web.Services.WebService
    {

        [WebMethod]
        public string SearchPaperData(int Data)
        {
            PaperData PaperData = new PaperData();
            PaperBasicData PaperBasicData = new PaperBasicData();
            PaperConcatData PaperConcatData = new PaperConcatData();

            PaperData.PaperBasicCollection = new Dictionary<int, PaperBasicData>();
            PaperData.PaperConcatCollection = new Dictionary<int, PaperConcatData>();

            Coords Coords = new Coords();
            ConcatPaper ConcatPaper = new ConcatPaper();

            string conStr = string.Format("{0}{1}{2}", "provider=microsoft.jet.oledb.4.0; Data Source=", System.Environment.CurrentDirectory, @"\Resource\PaperData.mdb");
            DbHelper dbHelper = new DbHelper(conStr);
            dbHelper.OpenDb();

            string sqlConcat = "SELECT * FROM BasicData";
            DataSet dataSetConcat = dbHelper.ExecuteQuery(sqlConcat);
            DataTable dataTableConcat = dataSetConcat.Tables[0];
            DataRow dataRowConcat;

            Random random = new Random();

            //int ConcatCount = dataTableConcat.Rows.Count;
            //int ConcatCount = 10;
            int ConcatCount = Data;

            for (int i = 0; i < ConcatCount; i++)
            {
                dataRowConcat = dataTableConcat.Rows[i];

                PaperConcatData = new PaperConcatData();
                PaperConcatData.ConcatPaper = new List<ConcatPaper>();

                PaperConcatData.ID = Convert.ToInt32(dataRowConcat["ID"]);

                int index = random.Next(0, ConcatCount);
                
                for (int j = 0; j < index; j++)
                {
                    int randomID = Convert.ToInt32(random.Next(0, ConcatCount + 1));

                    if (randomID == i + 1 || randomID == 0 || IsRandomInConcatData(PaperConcatData.ConcatPaper, randomID)) continue;

                    ConcatPaper = new ConcatPaper();

                    ConcatPaper.ID = randomID;

                    ConcatPaper.Rate = Convert.ToDouble(random.Next(65, 95) / 100.000);

                    if (ConcatPaper != null)
                    {
                        PaperConcatData.ConcatPaper.Add(ConcatPaper);
                    }
                }
                PaperData.PaperConcatCollection.Add(PaperConcatData.ID, PaperConcatData);
            }


            string sqlBasic = "SELECT * FROM BasicData";
            DataSet dataSetBasic = dbHelper.ExecuteQuery(sqlBasic);
            DataTable dataTableBasic = dataSetBasic.Tables[0];
            DataRow dataRowBasic;

            string sqlBasicTemp = "";
            DataSet dataSetBasicTemp;
            DataTable dataTableBasicTemp;

            //int BasicCount = dataTableBasic.Rows.Count;
            //int BasicCount = 10;
            int BasicCount = Data;

            for (int i = 0; i < BasicCount; i++)
            {
                dataRowBasic = dataTableBasic.Rows[i];

                PaperBasicData = new PaperBasicData();
                PaperBasicData.Coords = new Coords();

                Coords = new Coords();

                Coords.longitude = Convert.ToDouble((Convert.ToString(dataRowBasic["Coords"]).Split(','))[0]);
                Coords.latitude = Convert.ToDouble((Convert.ToString(dataRowBasic["Coords"]).Split(','))[1]);

                PaperBasicData.Coords = Coords;
                PaperBasicData.ID = Convert.ToInt32(dataRowBasic["ID"]);
                PaperBasicData.Subject = Convert.ToString(dataRowBasic["Subject"]);
                PaperBasicData.Author = Convert.ToString(dataRowBasic["Author"]);
                PaperBasicData.Location = Convert.ToString(dataRowBasic["Location"]);
                PaperBasicData.Organization = Convert.ToString(dataRowBasic["Organization"]);

                sqlBasicTemp = "SELECT * FROM KeyWorlds WHERE ID = " + i + 1;
                dataSetBasicTemp = dbHelper.ExecuteQuery(sqlBasicTemp);
                dataTableBasicTemp = dataSetBasicTemp.Tables[0];

                foreach (DataRow dataRowBasicTemp in dataTableBasicTemp.Rows)
                {
                    PaperBasicData.KeyWords = Convert.ToString(dataRowBasicTemp["KeyWorlds"]);
                }

                PaperData.PaperBasicCollection.Add(Convert.ToInt32(dataRowBasic["ID"]), PaperBasicData);
            }

            return JsonConvert.SerializeObject(PaperData);
        }

        private bool IsRandomInConcatData(List<ConcatPaper> concatDataCurrent, int random) {
            for (int i = 0; i < concatDataCurrent.Count; i++)
            {
                if (concatDataCurrent[i].ID == random) return true;
            }
            return false;
        }
    }
}
