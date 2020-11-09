using System;
using System.Collections.Generic;

namespace RBTree
{
    class Program
    {
        static void Main(string[] args)
        {
            var tree = new RedBlackTree<int>();
            var hashSet = new HashSet<int>();
            var random = new Random();
            while (hashSet.Count <= 10)
            {
                var next = random.Next(1,101);
                if(hashSet.Contains(next)) continue;
                tree.Add(next);
                hashSet.Add(next);
            }
            tree.Print();
            ReadAndDoIt(tree,hashSet);
        }

        static void ReadAndDoIt(RedBlackTree<int> tree, HashSet<int> hashSet )
        {
            while (true)
            {
                var line = Console.ReadLine().Split(' ');
                if(line.Length==0 || line.Length >2)
                {
                    Console.WriteLine("Wrong input line!");
                    continue;
                }
                var code = int.Parse(line[0]);
                var argument = line.Length == 2 ? int.Parse(line[1]) : 0;
                RedBlackNode<int> node = null;
                switch (code)
                {
                    case 0:
                        return;
                    case 1:
                        if(!hashSet.Contains(argument))
                            tree.Add(argument);
                        break;
                    case 2 :
                        tree.Delete(argument);
                        break;
                    case 3:
                        node = tree.Find(argument);
                        break;
                    case 4:
                        node = tree.Min();
                        break;
                    case 5:
                        node = tree.Max();
                        break;
                    case 6 :
                        node = tree.FindNext(argument);
                        break;
                    case 7:
                        node = tree.FindPrev(argument);
                        break;
                }
                if(code > 2)
                {
                    if (node != null) Console.WriteLine(node.ToString());
                }
                else
                    tree.Print();
            }
        }
    }
}