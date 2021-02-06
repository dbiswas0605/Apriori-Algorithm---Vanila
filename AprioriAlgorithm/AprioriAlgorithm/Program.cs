using AprioriAlgorithm.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AprioriAlgorithm
{
    class Program
    {
        static void Main(string[] args)
        {
            TransactionDB transactionDB = new TransactionDB(0.01M,false);
            transactionDB.LoadTransactionDB();

            transactionDB.GenerateApriori();
        }
    }
}
