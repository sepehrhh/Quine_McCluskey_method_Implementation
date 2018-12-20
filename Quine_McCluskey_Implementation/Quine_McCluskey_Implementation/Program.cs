using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quine_McCluskey_Implementation
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Number of variables: ");
            var variablesNum = int.Parse(Console.ReadLine());

            //define maxterm or minterm option
            Console.WriteLine("Maxterms or Minterms?");
            var minterms = new List<string>();
            if (Console.ReadLine() == "minterm")
            {
                Console.WriteLine("Minterms: ");
                minterms = Console.ReadLine().Split(',', ' ').ToList();
            }
            else
            {
                Console.WriteLine("Maxterms: ");
                var maxterms = Console.ReadLine().Split(',', ' ');
                for (int i = 0; i < Math.Pow(2, variablesNum); i++)
                    if (!maxterms.Contains(i.ToString()))
                        minterms.Add(i.ToString());
            }
            
            var function = new Function(variablesNum, minterms.ToArray());
        }
    }
}
