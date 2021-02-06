using AprioriAlgorithm.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AprioriAlgorithm.Helper
{
    class FinalSetGenerator
    {
        //This is where DB will be accessed for Support Count
        public static List<string> GenerateKFinal(int k, List<string> CandidateItemSets, TransactionDB transactions)
        {
            decimal support = transactions._supportCount;
            if (k == 1)
            {
                Dictionary<string, int> itemSupport = new Dictionary<string, int>();

                foreach (string item in CandidateItemSets)
                {
                    int count = 0;
                    foreach (var itemset in transactions._Transactions)
                    {
                        if (itemset.ItemSets.Contains(item))
                            count++;
                    }

                    if (count > support)
                    {
                        itemSupport.Add(item, count);
                    }
                }

                DumpOutput.DumpFile("outputfile_1", itemSupport);
                DumpOutput.DumpFile("outputfile_k", itemSupport);

                var final1temset = itemSupport.Keys.ToList();
                final1temset.Sort();

                return final1temset;
            }
            else
            {
                Dictionary<string, int> itemSupport = new Dictionary<string, int>();

                foreach (string itemset in CandidateItemSets)
                {
                    var t2 = itemset.Split(';').ToList();

                    int count = 0;
                    foreach(var transaction in transactions._Transactions)
                    {
                        bool isSubset = !t2.Except(transaction.ItemSets).Any();
                        if(isSubset)
                        {
                            count++;
                        }
                    }
                    if(count>transactions._supportCount)
                    {
                        itemSupport.Add(itemset, count);
                    }
                }

                DumpOutput.DumpFile("outputfile_k", itemSupport);

                var finalKtemset = itemSupport.Keys.ToList();
                finalKtemset.Sort();

                return finalKtemset;
            }
        }
    }
}
