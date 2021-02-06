using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using AprioriAlgorithm.Helper;

namespace AprioriAlgorithm.Core
{
    class Transaction
    {
        public int TID { get; set; }
        public List<string> ItemSets { get; set; }
    }
    class TransactionDB
    {
        internal List<Transaction> _Transactions = new List<Transaction>();
        private decimal _support = 0;
        internal decimal _supportCount = 0;
        bool _isAbsoluteSupport = false;

        public TransactionDB(decimal support, bool absolute)
        {
            _support = support;
            _isAbsoluteSupport = absolute;
        }

        internal void LoadTransactionDB()
        {
            string filePath = ConfigurationManager.AppSettings["inputfile"];

            var fileLInes = File.ReadLines(filePath);

            int tid = 0;
            foreach(var line in fileLInes)
            {
                var items = line.Split(';').ToList();
                items.Sort();

                Transaction t = new Transaction();
                t.TID = ++tid;
                t.ItemSets = items;

                _Transactions.Add(t);
            }

            if (!_isAbsoluteSupport)
                _supportCount = Math.Floor(_support * fileLInes.Count());
            else
                _supportCount = _support;
        }

        internal void GenerateApriori()
        {
            var C1 = CandidateSetGenerator.GenerateKCandidates(1, null, this);
            var L1 = FinalSetGenerator.GenerateKFinal(1, C1, this);

            var C2 = CandidateSetGenerator.GenerateKCandidates(2, L1, this);
            var Lk = FinalSetGenerator.GenerateKFinal(2, C2, this);

            int k = 3;
            List<string> tempL = Lk;
            while (tempL.Count > 0)
            {
                var CK = CandidateSetGenerator.GenerateKCandidates(k, tempL, this);
                if (CK.Count > 0)
                    tempL = FinalSetGenerator.GenerateKFinal(k, CK, this);
                else
                    break;

                k++;
            }
        }



        internal void DumpFile(int k, Dictionary<string, int> itemSupport)
        {
            string filePath = string.Empty;
            if(k==1)
                filePath = ConfigurationManager.AppSettings["outputfile_1"];
            else
                filePath = ConfigurationManager.AppSettings["outputfile_K"];

            using (StreamWriter file = new StreamWriter(filePath,true))
            {
                foreach (var itemwithsupport in itemSupport)
                {
                    file.WriteLine(itemwithsupport.Value + ":" + itemwithsupport.Key);
                }
            }
        }

        internal List<string> GetFinal1Itemsets()
        {
            Dictionary<string, int> itemSupport = new Dictionary<string, int>();
            var distinctItems = _Transactions.SelectMany(x => x.ItemSets).Distinct();

            foreach(string item in distinctItems)
            {
                var count = _Transactions.SelectMany(x => x.ItemSets).Count(x => x == item);

                if(count>_supportCount)
                {
                    itemSupport.Add(item, count);
                }
            }

            DumpFile(1, itemSupport);

            var final1temset = itemSupport.Keys.ToList();
            final1temset.Sort();

            return final1temset;
        }

        internal List<string> GetFinal2temSets(int K, List<string> FinalK_1ItemSet)
        {
            //Create the candidate set
            List<string> candidate2set = new List<string>();

            for(int i=0; i < FinalK_1ItemSet.Count - 2;i++)
            {
                for(int j=i+1; j < FinalK_1ItemSet.Count - 1; j++ )
                {
                    candidate2set.Add(FinalK_1ItemSet[i] + ";" + FinalK_1ItemSet[j]);
                }
            }


            //generate frequentItemsSet
            Dictionary<string, int> candidate2itemsetWithCount = new Dictionary<string, int>();

            foreach (string candidateitem in candidate2set)
            {
                foreach(var transaction in _Transactions)
                {
                    int _tid_ = transaction.TID;
                    List<string> _itemset = transaction.ItemSets;

                    int foundCount = 0;
                    foreach (var candidate in candidateitem.Split(';'))
                    {
                        if(!_itemset.Contains(candidate))
                        {
                            break;
                        }
                        foundCount++;
                    }

                    if(foundCount == K)
                    {
                        if(candidate2itemsetWithCount.ContainsKey(candidateitem))
                        {
                            candidate2itemsetWithCount[candidateitem] = candidate2itemsetWithCount[candidateitem] + 1;
                        }
                        else
                        {
                            candidate2itemsetWithCount.Add(candidateitem, 1);
                        }
                    }

                }
            }

            return candidate2itemsetWithCount.Where(x => x.Value > _supportCount).Select(x => x.Key).ToList();
        }


    }
}
