using System.Collections.Generic;
using System.Linq;

namespace Calculation
{
    static class IntLinearEquationSolve
    {
        private static int pointCount;

        private static int xCount;

        private static List<int[]> A;

        private static int[] B;

        private static int StationMin;

        public static int GetMin()
        {
            return StationMin;
        }

        public static void SetMin(int stMin)
        {
            StationMin = stMin;
        }

        public static void SetParam(List<int[]> a, int[] b)
        {
            A = a;
            B = b;
            pointCount = 0;
            foreach (var i in B)
            {
                pointCount += i;
            }

            StationMin = 0;

            xCount = A[0].Length;

            foreach (int[] t in A)
            {
                int tmp = t.Count(intse => intse != 0);
                if (StationMin < tmp)
                {
                    StationMin = tmp;
                }
            }

            StationMin = (pointCount / StationMin) + 1;
        }

        private static void Next(int[] mat)
        {
            if (mat[mat.Length - 1] != 0)
            {
                if (mat[mat.Length - 2] != 0)
                {
                    mat[mat.Length - 2]--;
                    mat[mat.Length - 1]++;
                    return;
                }
                for (var i = mat.Length - 2; i >= 0; i--)
                {
                    if (mat[i] != 0)
                    {
                        mat[i]--;
                        mat[i + 1] = mat[mat.Length - 1] + 1;
                        mat[mat.Length - 1] = 0;
                        return;
                    }
                }
            }
            else
            {
                for (var i = mat.Length - 2; i >= 0; i--)
                {
                    if (mat[i] != 0)
                    {
                        mat[i]--;
                        mat[i + 1]++;
                        return;
                    }
                }
            }
        }

        public static List<int[]> Solve()
        {
            var res = new int[xCount];
            var tmpRes = new int[B.Length];
            var result = new List<int[]>();
            res[0] = StationMin;

            while (true)
            {
                var isBreaked = false;

                for (var i = 0; i < A.Count; i++)
                {
                    tmpRes[i] = 0;

                    for (var j = 0; j < A[i].Length; j++)
                    {
                        tmpRes[i] += res[j] * A[i][j];
                    }

                    if (tmpRes[i] < B[i])
                    {
                        isBreaked = true;
                        break;
                    }
                }

                if (!isBreaked)
                {
                    result.Add(new int[xCount]);
                    res.CopyTo(result.Last(), 0);
                }

                if (res[xCount - 1] == StationMin)
                {
                    break;
                }
                Next(res);
            }

            return result;
        }
    }
}
