using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiplomWork
{
    public class Class1
    {
        public int[] S { get; set; }

        public Class1(int n, int inf)
        {
            S = new int[n];
            for (int i = 0; i < n; i++)
            {
                S[i] = inf;
            }
        }
    }
}
