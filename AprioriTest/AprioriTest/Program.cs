using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AprioriTest
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime startTime = DateTime.Now;//现在时间  
            string dateDiff = null;
            TimeSpan ts1 = new TimeSpan(startTime.Ticks);  
  
            Apriori apr = new Apriori();
            apr.Start();

            DateTime endTime = DateTime.Now;//现在时间  
            TimeSpan ts2 = new TimeSpan(endTime.Ticks);
            TimeSpan ts = ts1.Subtract(ts2).Duration();
            //显示时间  
            dateDiff = ts.Hours.ToString() + "小时"
                    + ts.Minutes.ToString() + "分钟"
                    + ts.Seconds.ToString() + "秒";
            Console.WriteLine(dateDiff);
            Console.Read();
        }
    }
}
