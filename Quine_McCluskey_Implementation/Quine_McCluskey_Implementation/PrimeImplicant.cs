using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quine_McCluskey_Implementation
{
    public class PrimeImplicant
    {
        public int Minterm;
        public string BinaryRep;
        public int NumOf1s;

        public PrimeImplicant(int minterm, int variablesNum)
        {
            Minterm = minterm;
            var s = new string[variablesNum];
            for (int i = 0; i < variablesNum; i++)
            {
                s[i] = (minterm % 2).ToString();
                minterm /= 2;
            }
            BinaryRep = String.Join("", s.Reverse());
            NumOf1s = BinaryRep.Count(x => x == '1');
        }
    }
}