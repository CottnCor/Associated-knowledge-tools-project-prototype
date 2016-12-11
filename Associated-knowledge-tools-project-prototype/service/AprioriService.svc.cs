using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using DbOperation;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;

namespace Associated_knowledge_tools_project_prototype.service
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class AprioriService
    {

        [WebInvoke]
        public string GetData()
        {
            //Apriori apr = new Apriori(value);
            //apr.Start();

            string ConcatData = "{\"";

            string BasicData = "{\"";

            string conStr = string.Format("{0}{1}{2}", "provider=microsoft.jet.oledb.4.0; Data Source=", System.Environment.CurrentDirectory, @"\Resource\PaperData.mdb");
            DbHelper dbHelper = new DbHelper(conStr);
            dbHelper.OpenDb();


            string sqlConcat = "SELECT * FROM BasicData";
            DataSet dataSetConcat = dbHelper.ExecuteQuery(sqlConcat);
            DataTable dataTableConcat = dataSetConcat.Tables[0];
            DataRow dataRowConcat;

            Random random = new Random();

            //int ConcatCount = dataTableConcat.Rows.Count;
            int ConcatCount = 10;

            for (int i = 0; i < ConcatCount; i++)
            {
                dataRowConcat = dataTableConcat.Rows[i];

                ConcatData += Convert.ToString(dataRowConcat["Subject"]) + "\":{"
                           + "\"ID\":" + Convert.ToInt32(dataRowConcat["ID"])
                           + ",\"ConcatPaper\":[";

                int index = random.Next(1, 10);

                for (int j = 0; j < index; j++)
                {
                    ConcatData += "{\"ID\":\"" + Convert.ToString(random.Next(1, ConcatCount));
                    ConcatData += "\",\"Rate\":" + random.Next(65, 95) / 10.000;

                    if (j != index - 1) ConcatData += "},";
                    else ConcatData += "}]";
                }

                if (i != ConcatCount - 1) ConcatData += "},\"";
                else ConcatData += "}}";
            }


            string sqlBasic = "SELECT * FROM BasicData";
            DataSet dataSetBasic = dbHelper.ExecuteQuery(sqlBasic);
            DataTable dataTableBasic = dataSetBasic.Tables[0];
            DataRow dataRowBasic;

            string sqlBasicTemp = "";
            DataSet dataSetBasicTemp;
            DataTable dataTableBasicTemp;

            //int BasicCount = dataTableBasic.Rows.Count;
            int BasicCount = 10;

            for (int i = 0; i < BasicCount; i++)
            {
                dataRowBasic = dataTableBasic.Rows[i];

                BasicData += Convert.ToString(dataRowBasic["ID"])
                           + "\":{" + "\"Coords\":[" + Convert.ToString(dataRowBasic["Coords"])
                           + "],\"Subject\":\"" + Convert.ToString(dataRowBasic["Subject"])
                           + "\",\"Author\":\"" + Convert.ToString(dataRowBasic["Author"])
                           + "\",\"Location\":\"" + Convert.ToString(dataRowBasic["Location"])
                           + "\",\"Organization\":\"" + Convert.ToString(dataRowBasic["Organization"])
                           + "\",\"KeyWords\":\"";

                sqlBasicTemp = "SELECT * FROM KeyWorlds WHERE ID = " + i + 1;
                dataSetBasicTemp = dbHelper.ExecuteQuery(sqlBasicTemp);
                dataTableBasicTemp = dataSetBasicTemp.Tables[0];

                foreach (DataRow dataRowBasicTemp in dataTableBasicTemp.Rows)
                {
                    BasicData += dataRowBasicTemp["KeyWorlds"];
                    BasicData += "\"";
                }

                if (i != BasicCount - 1) BasicData += "},\"";
                else BasicData += "}}";
            }

            string Data = "{\"BasicData\":" + BasicData + ",\"ConcatData\":" + ConcatData + "}";

            return Data;
        }
    }
}
