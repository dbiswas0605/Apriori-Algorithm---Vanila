using AprioriAlgorithm.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AprioriAlgorithm.Helper
{
    class CandidateSetGenerator
    {
        public static List<string> GenerateKCandidates(int k, List<string> itemsets, TransactionDB transactions)
        {
            List<string> candidateKset = new List<string>();

            if (k==1)
            {
                candidateKset = transactions._Transactions.SelectMany(x => x.ItemSets).Distinct().ToList();
                return candidateKset;
            }
            else if(k==2)
            {
                for (int i = 0; i < itemsets.Count - 1; i++)
                {
                    for (int j = i + 1; j < itemsets.Count; j++)
                    {
                        candidateKset.Add(itemsets[i] + ";" + itemsets[j]);
                    }
                }
                return candidateKset;
            }
            else//K3 and Above
            {
                itemsets.Sort();
                //Find all the elements which has same K-2 value
                int takeCount = k - 2;

                //list of all the elemets starting with Kth Item
                List<string> klist = new List<string>();

                foreach(string s in itemsets)
                {
                    var arr = s.Split(';');
                    string kthItem = string.Join(";", arr.Take(takeCount));

                    if (!klist.Contains(kthItem))
                        klist.Add(kthItem);
                }

                foreach (string kitem in klist)
                {
                    var itemsStartsWithK = itemsets.Where(x => x.StartsWith(kitem + ";")).ToArray();

                    if(itemsStartsWithK.Length > 1)
                    {
                        //Take the 1st Elememt
                        string t1 = itemsStartsWithK[0];

                        for(int i=1; i<itemsStartsWithK.Length; i++)
                        {
                            string lastElement = itemsStartsWithK[i].Split(';')[k - 2];

                            string kset = t1 + ";" + lastElement;

                            candidateKset.Add(kset);
                        }
                    }
                }
                candidateKset = PruneKCandidate(k, candidateKset, itemsets);
                return candidateKset;
            }
        }

        private static IEnumerable<IEnumerable<T>> SubSetsOf<T>(IEnumerable<T> source)
        {
            if (!source.Any())
                return Enumerable.Repeat(Enumerable.Empty<T>(), 1);

            var element = source.Take(1);

            var haveNots = SubSetsOf(source.Skip(1));
            var haves = haveNots.Select(set => element.Concat(set));

            return haves.Concat(haveNots);
        }

        private static List<string> GenerateKsubSets(int k, List<string> CurrentCandidateItemsets)
        {
            var s = SubSetsOf(CurrentCandidateItemsets).Where(x => x.Count() == k-1).Select(x => x);

            return s.Select(x => string.Join(";", x)).ToList();
        }

        private static List<string> PruneKCandidate(int k, List<string> CurrentCandidateItemsets, List<string> L_1ItemSets)
        {
            List<string> returnList = new List<string>();
            if(k>2)
            {
                foreach(var candidateitemset in CurrentCandidateItemsets)
                {
                    var allSubSets = GenerateKsubSets(k, candidateitemset.Split(';').ToList());
                    int subsetCount = allSubSets.Count;

                    //if all the subsets are part of L_1ItemSets
                    int subsetfound = 0;
                    foreach (string subset in allSubSets)
                    {
                        var t2 = subset.Split(';').ToList();
                        bool isSubset = false;
                        foreach (var itemset in L_1ItemSets)
                        {
                            var transaction = itemset.Split(';').ToList();
                            isSubset = !t2.Except(transaction).Any();

                            if (isSubset)
                            {
                                subsetfound++;
                                break;
                            }
                        }

                        if(subsetfound == subsetCount)
                        {
                            returnList.Add(candidateitemset);
                        }
                    }
                }
            }
            return returnList;
        }
    }
}
