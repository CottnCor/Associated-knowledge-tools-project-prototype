using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Data;
using System.Data.OleDb;

namespace Associated_Service
{
    class C_Node
    {
        public char[] item = new char[23];  //数据项
        //public List<char> item = new List<char>(5);
        public int min_supp_count;//最小支持度数

        public C_Node()
        {

        }
    }; //候选集

    class L_Node
    {
        public char[] item = new char[23];  //数据项
        //public List<char> item = new List<char>(5);
        public int min_supp_count;//最小支持度数

    };//频繁集

    class R_Node
    {
        public string lItem = "";  //规则左侧数据项
        public string rItem = "";  //规则右侧数据项
        public int lValue = 0; //规则左侧数据关键字数字化值
        public int rValue = 0;//规则右侧数据关键字数字化值
        public double rate = 0;
        public int lNum = 0;
        public int rNum = 0;
    }

    class RelatedNode
    {
        public int id;  //数据ID
        public double rate; //关联度
    }

    class RelatedSet
    {
        public int mainID; //主数据ID
        public int mainValue; //主数据关键字数字化值
        public R_Node[] R = new R_Node[9999]; //主数据满足的关联规则集
        public int rSize = 0;//主数据满足的关联规则个数
        public RelatedNode[] node = new RelatedNode[9999];//满足条件的关联数据集
        public int nSize = 0;//满足条件的关联数据个数
    }

    class Keyword
    {
        public string value = "";
        public int num = 0;
    }
    class Apriori
    {
        int nDBFieldCount;
        int nDbItemCount;//统计数据库中事务个数
        int nLargeCount;//统计频繁集的总个数
        int min_supp_count;//最小支持度数 min_supp_count=nDbItemCount*min_supp
        double min_supp;//最小支持度min_supp
        double min_con;
        int[] DB;//用来生成保存数据库散列后的数据
        string[] s_DB;
        int keywordNum;
        char firstChar;
        char lastChar;
        int lnNum = 0;
        int ruleNum = 0;
        string searchWord;
        Dictionary<char, string> D = new Dictionary<char, string>();
        C_Node[] C = new C_Node[9999];
        L_Node[] L = new L_Node[9999];
        R_Node[] R = new R_Node[9999];
        RelatedSet rSet = new RelatedSet();

        public Apriori(string value)
        {
            searchWord = value;
            nDbItemCount = 10;
            keywordNum = 26;
            firstChar = '0';
            lastChar = (char)(firstChar + keywordNum - 1);
        }


        public void Start()
        {
            min_supp = 0.2f;
            min_con = 0.5f;

            HashDB();
            //C1();//初始化,生成1项候选集C1
            //L1();//得到1项频繁集L1
            GetLn(1);
            int n = 1;
            while (L[0].min_supp_count != 0 && n <= nDBFieldCount)
            {
                n += 1;
                GetLn(n);
            }
            GetRule();

            GetRelatedNodes(0);
        }


        public void HashDB()//通过扫描数据库, 得到数据库的散列数值
        {
            string strtemp = "";

            OleDbConnection _oleDbConn;
            string _strdata =
               string.Format("{0}{1}{2}", "provider=microsoft.jet.oledb.4.0; Data Source=",
               System.Environment.CurrentDirectory, @"\Resource\DataBase.mdb");
            _oleDbConn = new OleDbConnection(_strdata);
            string strsql = "";
            //strsql = "SELECT * FROM mushroom1";
            strsql = "SELECT * FROM mushroom2";
            _oleDbConn.Open();//打开连接;

            OleDbCommand command = new OleDbCommand(strsql, _oleDbConn);
            OleDbDataReader dr = command.ExecuteReader();
            nDbItemCount = 0;
            DB = new int[9999];
            s_DB = new string[9999];
            char ctemp = 'a';
            nDBFieldCount = dr.FieldCount - 1;

            DataTable dt = new DataTable();
            DataColumn dtcol;
            DataRow dtrow;

            for (int i = 0; i < dr.FieldCount; i++)
            {
                dtcol = new DataColumn();
                dtcol.ColumnName = dr.GetName(i);
                dtcol.DataType = dr.GetFieldType(i);
                dt.Columns.Add(dtcol);
            }

            while (dr.Read())
            {
                bool flag = false;
                for (int i = 1; i < dr.FieldCount; i++)//判断是否包含搜索词
                {
                    if (!dr.IsDBNull(i))
                    {
                        strtemp = dr.GetString(i);
                        if (strtemp == searchWord)
                        {
                            flag = true;
                        }
                    }
                }
                if (flag == false)
                {
                    continue;
                }
                dtrow = dt.NewRow();
                for (int i = 1; i < dr.FieldCount; i++)//建立关键词字典
                {
                    if (!dr.IsDBNull(i))
                    {
                        dtrow[i] = dr[i];
                        strtemp = dr.GetString(i);
                        if (D.Count == 0)
                        {
                            ctemp = firstChar;
                            D.Add(ctemp, strtemp);
                        }//end if
                        else if (!D.ContainsValue(strtemp))
                        {
                            ctemp = (char)(D.Last().Key + 1);
                            D.Add(ctemp, strtemp);
                        }//end else if
                        else
                        {
                            foreach (KeyValuePair<char, string> kvp in D)
                            {
                                if (kvp.Value.Equals(strtemp))
                                {
                                    ctemp = kvp.Key;
                                }
                            }
                        }//end else
                    }//end if
                }//end for 建立关键词字典
                dt.Rows.Add(dtrow);
            }
            nDbItemCount = dt.Rows.Count;
            min_supp_count = (int)(min_supp * nDbItemCount + 0.5);//得到最小支持度数
            keywordNum = D.Count;
            lastChar = (char)(firstChar + keywordNum - 1);

            for (int row = 0; row < nDbItemCount; row++)
            {
                DB[nDbItemCount] = 0;
                s_DB[nDbItemCount] = "";
                for (int i = 1; i < nDBFieldCount + 1; i++)
                {
                    if (!dt.Rows[row].IsNull(i))
                    {
                        strtemp = dt.Rows[row][i].ToString();
                        foreach (KeyValuePair<char, string> kvp in D)
                        {
                            if (kvp.Value.Equals(strtemp))
                            {
                                ctemp = kvp.Key;
                            }
                        }
                        DB[row] += 1 << (int)(lastChar - ctemp);
                        s_DB[row] += ctemp;
                    }
                }
            }

        }


        public void check_supp(int num, int no)//检查候选集支持度
        {
            string strtemp;
            int k, m;
            int i, j;
            int check;
            m = lastChar;

            for (i = 1; i <= num; i++)//for1
            {
                check = 0;
                C[i].min_supp_count = 0;
                for (j = 0; j < no; j++)//for2
                {
                    k = 1;
                    check += (int)(k << (m - C[i].item[j]));

                }//end for2


                for (j = 0; j < nDbItemCount; j++)//for3
                {
                    if (check == (check & DB[j]))
                    {
                        C[i].min_supp_count += 1;//子集存在，支持度数加1
                    }
                }//end for3	
            }//end for1
        }


        public void GetCn(int n)//生成n项候选集
        {

            if (n == 1)
            {
                n = keywordNum;
                C[0] = new C_Node();
                C[0].min_supp_count = n;//1 项候选集的个数，在本算法中，用C[0].min_supp_count来保存候选集Cn的个数   

                for (int i = 0; i < n; i++)
                {
                    C[i + 1] = new C_Node();
                    C[i + 1].item[0] = firstChar;
                    C[i + 1].item[0] += (char)i;
                }

                check_supp(n, 1);
            }
            else if (n > 1)
            {
                int i, j, k, num;
                int no = 0, temp = 0;

                C[0].min_supp_count = 0;  //初始化


                num = L[0].min_supp_count;  //num是Ln-1项集的数据个数

                for (i = 1; i <= num; i++)

                    for (j = i + 1; j <= num; j++)   //for2
                    {

                        temp = 1;  //测试是否满足联结条件
                        if (n > 2)//if 1
                        {
                            for (k = 0; k < n - 2; k++)    	//for3	  
                            {
                                if (L[i].item[k] != L[j].item[k])
                                {
                                    temp = 0;
                                    break;
                                }//if 1

                            }//end for3

                        }//end if1

                        if (temp == 1)//满足联结条件
                        {
                            // printf("in if 2  no=%d\n",no);
                            no++;
                            C[no] = new C_Node();
                            //C[no].item. = L[i].item;	//在这里，用字符串拷贝函数直接把L[i].item赋值给C[no].item		
                            L[i].item.CopyTo(C[no].item, 0);
                            C[no].item[n - 1] = L[j].item[n - 2];	//L[j].item[n-2]赋值给C[no].item[n-1]，
                            //C[no].item[n] = '\0';				//设置末尾标志位
                            C[no].min_supp_count = 0;
                            C[0].min_supp_count += 1;
                        }//end if2
                    }//end for2

                num = C[0].min_supp_count;
                check_supp(num, n);//测试支持度 
            }
        }


        public void GetLn(int n)//生成n项频繁集
        {
            string strValue, strtemp;
            int i, j, k;
            GetCn(n);
            j = 0;
            L[0] = new L_Node();
            L[0].min_supp_count = 0;


            for (i = 1; i <= C[0].min_supp_count; i++)  //for 1
            {
                if (C[i].min_supp_count >= min_supp_count)
                {
                    j += 1;
                    L[j] = new L_Node();
                    L[j].item = C[i].item;
                    L[j].min_supp_count = C[i].min_supp_count;
                }  //end if
            }//end for1


            L[0].min_supp_count = j; //保存数据的个数
            k = L[0].min_supp_count;


            //nLargeCount++;
            //Console.WriteLine("---------------");

            //if (k != 0)
            //{
            //    lnNum = k;
            //    for (i = 1; i <= lnNum; i++)
            //    {

            //        nLargeCount++;

            //        strtemp = new string(L[i].item);
            //        Console.WriteLine("1: " + strtemp + L[i].min_supp_count.ToString());

            //    }//for


            //}//if
        }


        public void GetRule()
        {
            if (lnNum != 0)
            {
                string strItem;
                ruleNum = 0;
                for (int i = 1; i <= lnNum; i++)//for1
                {
                    strItem = new string(L[i].item);
                    strItem = strItem.Replace("\0", "");
                    int mask = 0;

                    mask = (int)Math.Pow(2, strItem.Length) - 1;//创建集合抽取掩膜

                    int pos = 0;
                    //找出每一个子集掩膜
                    for (pos = (pos - mask) & mask; ; pos = (pos - mask) & mask)//for2
                    {
                        if (pos == mask)
                        {
                            break;
                        }
                        int t = 0;
                        R[ruleNum] = new R_Node();
                        //通过掩膜提取子集元素
                        for (int k = 0; k < strItem.Length; k++)//for3
                        {

                            t = pos & (1 << k);
                            if (t != 0)
                            {
                                R[ruleNum].lValue += (int)(1 << (lastChar - strItem.ElementAt(k)));
                                R[ruleNum].lItem += strItem.ElementAt(k);//生成子集字符串
                            }
                            else
                            {
                                R[ruleNum].rValue += (int)(1 << (lastChar - strItem.ElementAt(k)));
                                R[ruleNum].rItem += strItem.ElementAt(k);
                            }
                        }//end for3


                        for (int j = 0; j < nDbItemCount; j++)//for4
                        {
                            if (R[ruleNum].lValue == (R[ruleNum].lValue & DB[j]))
                            {
                                R[ruleNum].lNum += 1;//子集存在，支持度数加1
                                if (R[ruleNum].rValue == (R[ruleNum].rValue & DB[j]))
                                {
                                    R[ruleNum].rNum += 1;//子集存在，支持度数加1
                                }
                            }
                        }//end for4
                        R[ruleNum].rate = (double)R[ruleNum].rNum / R[ruleNum].lNum;
                        if (R[ruleNum].rate > min_con)
                        {
                            ruleNum++;
                        }


                    }//end for2

                }//end for1

                //for (int i = 0; i < ruleNum; i++)
                //{
                //    R[i].rate = Math.Round(R[i].rate, 2);
                //    Console.Write("2: " + R[i].lItem + "->" + R[i].rItem + " " +
                //                R[i].lNum.ToString() + " " +
                //                R[i].rNum.ToString() + " " +
                //                R[i].rate.ToString() + "\n");
                //    Console.Write("---------------\n");
                //}
            }//if
        }


        public void GetRelatedNodes(int id)
        {
            rSet.mainID = id;
            rSet.mainValue = DB[id + 1];
            int ruleN = 0;
            double maxRate = 0;
            double minRate = 0.8;
            double totalMaxRate = 0;
            int nodeN = 0;
            int tempNum;
            double tempRate = 0;
            string tempStr = "";
            RelatedNode[] rate = new RelatedNode[9999];

            for (int i = 0; i < ruleNum; i++)
            {
                if (R[i].lValue == (R[i].lValue & DB[id]))
                {
                    rSet.R[ruleN] = R[i];
                    ruleN++;
                }
            }

            for (int i = 0; i < nDbItemCount; i++)
            {
                maxRate = 0;
                for (int j = 0; j < ruleN; j++)
                {
                    if (rSet.R[j].rValue == (rSet.R[j].rValue & DB[i]))
                    {
                        //tempStr = StringUnion(s_DB[id], s_DB[i]);
                        tempRate = rSet.R[j].rate;
                        //tempRate *= ((double)rSet.R[j].lItem.Length / ((double)s_DB[id].Length)) *
                        //        ((double)rSet.R[j].rItem.Length / ((double)s_DB[i].Length));
                        tempRate *= ((double)rSet.R[j].lItem.Length + (double)rSet.R[j].rItem.Length) /
                                (double)s_DB[i].Length;
                        //lift *= ((double)rSet.R[j].lItem.Length / ((double)tempStr.Length)) *
                        //    ((double)rSet.R[j].rItem.Length / ((double)tempStr.Length));
                        //maxRate = Math.Max(maxRate, lift);
                        maxRate += tempRate;
                        tempRate = 0;
                    }
                }
                rate[i] = new RelatedNode();
                rate[i].id = i;
                rate[i].rate = maxRate;
                totalMaxRate = Math.Max(maxRate, totalMaxRate);
            }

            for (int i = 0; i < nDbItemCount - 1; i++)
            {
                tempRate = rate[i].rate / totalMaxRate;
                if (tempRate >= minRate && rate[i].id != rSet.mainID)
                {
                    rSet.node[nodeN] = new RelatedNode();
                    rSet.node[nodeN].id = i;
                    rSet.node[nodeN].rate = tempRate;
                    rSet.node[nodeN].rate = Math.Round(rSet.node[nodeN].rate, 2);
                    nodeN++;
                }
            }
            rSet.nSize = nodeN;

            //for (int i = 0; i < nodeN; i++)
            //{
            //    Console.Write("3: " + rSet.node[i].id.ToString() + " " + rSet.node[i].rate.ToString() + "\n");
            //}

            //Console.Write("共" + nodeN.ToString() + "个\n");
        }
    }
}