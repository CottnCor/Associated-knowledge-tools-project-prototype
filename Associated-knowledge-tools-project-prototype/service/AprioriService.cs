using DbOperation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Associated_Service
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的类名“AprioriService”。
    public class AprioriService : IAprioriService
    {
        public string GetData(string value)
        {
            //Apriori apr = new Apriori(value);
            //apr.Start();

            string ConcatData = "{\"PaperName1\":{\"ID\":\"123\",\"ConcatPaper\":[[{\"ID\":\"abc\",\"Rate\":123},{\"ID\":\"abc\",\"Rate\":123}]]},\"PaperName2\":{\"ID\":\"123\",\"ConcatPaper\":[[{\"ID\":\"abc\",\"Rate\":123},{\"ID\":\"abc\",\"Rate\":123}]]}}";

            string BasicData = "{\"";

            string conStr = string.Format("{0}{1}{2}", "provider=microsoft.jet.oledb.4.0; Data Source=", System.Environment.CurrentDirectory, @"\Resource\PaperData.mdb");
            DbHelper dbHelper = new DbHelper(conStr);
            dbHelper.OpenDb();

            string sql = "SELECT * FROM BasicData";
            DataSet dataSet = dbHelper.ExecuteQuery(sql);
            DataTable dataTable = dataSet.Tables[0];
            DataRow dataRow;

            string sqlTemp = "";
            DataSet dataSetTemp;
            DataTable dataTableTemp;

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                dataRow = dataTable.Rows[i];

                BasicData += Convert.ToString(dataRow["ID"])
                           + "\":{\"Coords\":[" + Convert.ToString(dataRow["Coords"])
                           + "],\"Subject\":\"" + Convert.ToString(dataRow["Subject"])
                           + "\",\"Author\":\"" + Convert.ToString(dataRow["Author"])
                           + "\",\"Location\":\"" + Convert.ToString(dataRow["Location"])
                           + "\",\"Organization\":\"" + Convert.ToString(dataRow["Organization"])
                           + "\",\"KeyWords\":\"";

                sqlTemp = "SELECT * FROM KeyWorlds WHERE ID = " + i;
                dataSetTemp = dbHelper.ExecuteQuery(sqlTemp);
                dataTableTemp = dataSetTemp.Tables[0];

                foreach (DataRow dataRowTemp in dataTableTemp.Rows)
                {
                    BasicData += dataRowTemp["KeyWords"];
                }

                if (i != dataTable.Rows.Count - 1) BasicData += "\"}, ";
                else BasicData += "\"}";
            }

            BasicData += "}";

            string Data = "{\"BasicData\":" + BasicData + ",\"ConcatData\":\"" + ConcatData + "}";

            return Data;
        }



        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }
    }
}
