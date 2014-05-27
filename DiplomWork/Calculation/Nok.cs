using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calculation
{
    public class Nok
    {
        public static int GetNok(int a, int b)
        {
            return (a*b)/GetNod(a, b);
        }

        public static int GetNod(int a, int b)
        {
            return (b > 0) ? GetNod(b, a%b) : a;
        }
    }
}
