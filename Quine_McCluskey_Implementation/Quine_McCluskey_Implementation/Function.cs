using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quine_McCluskey_Implementation
{
    public class Function
    {
        private readonly int VariablesNum;
        private List<PrimeImplicant> Minterms;
        private Dictionary<int, List<PrimeImplicant>> ImplicantGroups;
        private Dictionary<int, List<List<PrimeImplicant>>> CombinedImplicants;
        private List<List<PrimeImplicant>> PrimeImplicantsList;
        private List<List<PrimeImplicant>> EssentialPrimes;
        private List<List<PrimeImplicant>> MinimumPrimeImplicants;

        public Function(int variablesNum, string[] minterms)
        {
            VariablesNum = variablesNum;
            Minterms = new List<PrimeImplicant>();
            foreach (var i in minterms)
                Minterms.Add(new PrimeImplicant(int.Parse(i), VariablesNum));
            GetGroups();
            SetFirstColumn();
            for (int i = 1; i < variablesNum - 1; i++)
                SetNextColumns((int)Math.Pow(2, i));
            PrimeImplicantsList = new List<List<PrimeImplicant>>();
            foreach (var group in CombinedImplicants.Keys)
                foreach (var list in CombinedImplicants[group])
                    if (PrimeImplicantsList.
                        Count(x => x[0].BinaryRep == list[0].BinaryRep) == 0)
                        PrimeImplicantsList.Add(list);
            GetPrimeImplicants();
            GetEssentials();
            MinimumPrimeImplicants = new List<List<PrimeImplicant>>();
            MinimumPrimeImplicants.AddRange(EssentialPrimes);
            GetOtherPrimeImplicants();
            MinimumPrimeImplicants.RemoveAll(x => x.Count > 1 &&
            x.All(t => MinimumPrimeImplicants.Where(y => y != x ).ToList().Exists
            (z => z.Exists(u => u.Minterm == t.Minterm))));

            //print section
            var resultStringsList = new List<string>();
            foreach (var list in MinimumPrimeImplicants)
            {
                string mintermString = null;
                var binaryRep = list.First().BinaryRep;
                for (int i = 0; i < binaryRep.Length; i++)
                    if (binaryRep[i] == '0')
                    {
                        mintermString += (char)(i + 'a');
                        mintermString += "'";
                    }
                    else if (binaryRep[i] == '1')
                        mintermString += (char)(i + 'a');
                if (mintermString != null)
                    resultStringsList.Add(mintermString);
            }
            Console.WriteLine(String.Join(" + ", resultStringsList));
        }

        private void GetOtherPrimeImplicants()
        {
            var undefinedMinterms = Minterms.Where(x =>
            !MinimumPrimeImplicants.Exists(y =>
            y.Exists(z => z.Minterm == x.Minterm))).ToList();
            PrimeImplicantsList.RemoveAll(x => EssentialPrimes.Contains(x));
            while (undefinedMinterms.Count > 0)
            {
                if (PrimeImplicantsList.Count > 0 && PrimeImplicantsList.Exists(x =>
                x.Exists(y => undefinedMinterms.Exists(z => z.Minterm == y.Minterm))))
                {
                    var chosenGroup = PrimeImplicantsList.OrderByDescending(x =>
                    x.Count(y => undefinedMinterms.Exists(z =>
                    z.Minterm == y.Minterm))).First();
                    MinimumPrimeImplicants.Add(chosenGroup);
                    undefinedMinterms.RemoveAll(x => chosenGroup.Exists(y =>
                    y.Minterm == x.Minterm));
                    PrimeImplicantsList.Remove(chosenGroup);
                }
                else
                {
                    MinimumPrimeImplicants.Add(undefinedMinterms);
                    break;
                }
            }
        }

        private void GetEssentials()
        {
            EssentialPrimes = new List<List<PrimeImplicant>>();
            foreach (var list in PrimeImplicantsList)
            {
                var otherLists = PrimeImplicantsList.Where(x => x != list)
                    .ToList();
                if (list.All(x => otherLists.Exists(y =>
                y.Exists(z => z.Minterm == x.Minterm))))
                    continue;
                else
                    EssentialPrimes.Add(list);
            }
        }

        private void GetPrimeImplicants()
        {
            var primes = new List<List<PrimeImplicant>>();
            foreach (var list in PrimeImplicantsList)
            {
                var whereList = PrimeImplicantsList.Where
                    (x => x.Count(y => list.Exists(s => s.Minterm == y.Minterm)) == list.Count);
                if (whereList.Count() <= 1)
                    primes.Add(list);
            }
            PrimeImplicantsList = primes;
        }

        private void SetNextColumns(int combineNumber)
        {
            var combinedImplicants = new Dictionary<int,
               List<List<PrimeImplicant>>>();
            foreach (var groupNumber in CombinedImplicants.Keys)
            {
                if (CombinedImplicants.ContainsKey(groupNumber + 1))
                {
                    var implicantsList1 = CombinedImplicants[groupNumber];
                    var implicantsList2 = CombinedImplicants[groupNumber + 1];
                    foreach (var list1 in implicantsList1)
                    {
                        if (list1.Count == combineNumber)
                        {
                            foreach (var list2 in implicantsList2)
                            {
                                if (list2.Count == combineNumber)
                                {
                                    var combine = Combine(list1[0],
                                        list2.First(x => x != list1[0]));
                                    if (combine.Item1)
                                    {
                                        var pList1 = new List<PrimeImplicant>();
                                        var pList2 = new List<PrimeImplicant>();
                                        (pList1, pList2) = UpdateImplicants(list1, list2,
                                            combine.Item2);
                                        if (!combinedImplicants.ContainsKey(groupNumber))
                                            combinedImplicants.Add(groupNumber,
                                                new List<List<PrimeImplicant>>());
                                        combinedImplicants[groupNumber].
                                            Add(pList1.Concat(pList2).ToList());
                                    }
                                }
                            }
                        }
                    }
                }
            }
            foreach (var key in ImplicantGroups.Keys)
                if (combinedImplicants.ContainsKey(key))
                    foreach (var val in combinedImplicants[key])
                        CombinedImplicants[key].Add(val);
        }

        private (List<PrimeImplicant>, List<PrimeImplicant>) UpdateImplicants
            (List<PrimeImplicant> list1, List<PrimeImplicant> list2, string newBinaryRep)
        {
            var updatedList1 = new List<PrimeImplicant>();
            var updatedList2 = new List<PrimeImplicant>();
            foreach (var pImp in list1)
            {
                var imp1 = new PrimeImplicant(pImp.Minterm, VariablesNum);
                imp1.BinaryRep = newBinaryRep;
                updatedList1.Add(imp1);
            }   
            foreach (var pImp in list2)
            {
                var imp2 = new PrimeImplicant(pImp.Minterm, VariablesNum);
                imp2.BinaryRep = newBinaryRep;
                updatedList1.Add(imp2);
            }
            return (updatedList1, updatedList2);
        }

        private void SetFirstColumn()
        {
            var combinedImplicants = new Dictionary<int,
                List<List<PrimeImplicant>>>();
            foreach (var groupNumber in ImplicantGroups.Keys)
            {
                if (ImplicantGroups.ContainsKey(groupNumber + 1))
                {
                    var group1 = ImplicantGroups[groupNumber];
                    var group2 = ImplicantGroups[groupNumber + 1];
                    foreach (var implicant1 in group1)
                        foreach (var implicant2 in group2)
                        {
                            var combine = Combine(implicant1, implicant2);
                            if (combine.Item1)
                            {
                                if (!combinedImplicants.ContainsKey(groupNumber))
                                    combinedImplicants.Add(groupNumber,
                                        new List<List<PrimeImplicant>>());
                                var imp1 = new PrimeImplicant(implicant1.Minterm, VariablesNum);
                                var imp2 = new PrimeImplicant(implicant2.Minterm, VariablesNum);
                                imp1.BinaryRep = combine.Item2;
                                imp2.BinaryRep = combine.Item2;
                                combinedImplicants[groupNumber]
                                    .Add(new List<PrimeImplicant>
                                    { imp1, imp2 });
                            }
                        }
                    CombinedImplicants = combinedImplicants;
                }
            }
        }

        public static (bool, string) Combine(PrimeImplicant implicant1,
            PrimeImplicant implicant2)
        {
            string combinedString = null;
            for (int i = 0; i < implicant1.BinaryRep.Length; i++)
                if (implicant1.BinaryRep[i] != implicant2.BinaryRep[i])
                {
                    if (implicant1.BinaryRep[i] == '-')
                        return (false, null);
                    combinedString += '-';
                }
                else
                    combinedString += implicant1.BinaryRep[i];
            if (combinedString.Count(x => x == '-') >
                implicant1.BinaryRep.Count(x => x == '-') + 1)
                return (false, null);
            return (true, combinedString);
        }

        private void GetGroups()
        {
            ImplicantGroups = new Dictionary<int, List<PrimeImplicant>>();
            foreach (var minterm in Minterms)
            {
                if (!ImplicantGroups.ContainsKey(minterm.NumOf1s))
                    ImplicantGroups.Add(minterm.NumOf1s,
                        new List<PrimeImplicant>());
                ImplicantGroups[minterm.NumOf1s].Add(minterm);
            }
            ImplicantGroups.OrderBy(x => x.Key);
        }
    }
}