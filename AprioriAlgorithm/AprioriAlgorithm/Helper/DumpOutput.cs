using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AprioriAlgorithm.Helper
{
    class DumpOutput
    {
        internal static void DumpFile(string key, Dictionary<string, int> itemSupport)
        {
            string filePath = ConfigurationManager.AppSettings[key];

            using (StreamWriter file = new StreamWriter(filePath,true))
            {
                foreach (var itemwithsupport in itemSupport)
                {
                    file.WriteLine(itemwithsupport.Value + ":" + itemwithsupport.Key);
                }
            }
        }
    }
}
