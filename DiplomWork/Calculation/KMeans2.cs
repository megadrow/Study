using System;

namespace Calculation
{
    public class Alglibexception : Exception
    {
        public string Msg;
        public Alglibexception(string s)
        {
            Msg = s;
        }

    }

    public class KMeans2
    {
        public static void Shuffle<T>(T[] array)
        {
            var r = new Random();
            for (int i = array.Length; i > 1; i--)
            {
                int j = r.Next(i);
                T tmp = array[j];
                array[j] = array[i - 1];
                array[i - 1] = tmp;
            }
        }

        public static void K_MeansGenerate(double[,] xy,//[npoints, nvars]
            int npoints,
            int nvars,
            int k,
            int[] cond, //[k]
            int restarts,
            out int info,
            out double[,] c,    //[k, nvars]
            out int[] xyc,
            out double ebest,
            out double ebest2)      //[npoints]
        {
            int i = 0;
            int j = 0;
            double[,] ct = new double[0, 0];
            double[,] ctbest = new double[0, 0];
            int[] xycbest = new int[0];
            double e = 0;
            double e2 = 0;
            double[] x = new double[0];
            double[] tmp = new double[0];
            double[] d2 = new double[0];
            double[] p = new double[0];
            int[] csizes = new int[0];
            bool[] cbusy = new bool[0];
            double v = 0;
            int cclosest = 0;
            double dclosest = 0;
            double[] work = new double[0];
            bool waschanges = new bool();
            bool condcomplete = false;
            bool zerosizeclusters = new bool();
            int pass = 0;
            int i_ = 0;
            ebest = Maxrealnumber;
            ebest2 = Maxrealnumber;

            c = new double[0, 0];
            xyc = new int[0];


            //
            // Test parameters
            //
            if (((npoints < k | nvars < 1) | k < 1) | restarts < 1)
            {
                info = -1;
                return;
            }

            //
            // TODO: special case K=1
            // TODO: special case K=NPoints
            //
            info = 1;

            //
            // Multiple passes of k-means++ algorithm
            //
            ct = new double[k, nvars];
            ctbest = new double[k, nvars];
            xyc = new int[npoints];
            xycbest = new int[npoints];
            d2 = new double[npoints];
            p = new double[npoints];
            tmp = new double[nvars];
            csizes = new int[k];
            cbusy = new bool[k];
            
            for (pass = 1; pass <= restarts; pass++)
            {

                //
                // Select initial centers  using k-means++ algorithm
                // 1. Choose first center at random
                // 2. Choose next centers using their distance from centers already chosen
                //
                // Note that for performance reasons centers are stored in ROWS of CT, not
                // in columns. We'll transpose CT in the end and store it in the C.
                //
                i = RandomInteger(npoints);
                for (i_ = 0; i_ <= nvars - 1; i_++)
                {
                    ct[0, i_] = xy[i, i_];
                }
                cbusy[0] = true;
                for (i = 1; i <= k - 1; i++)
                {
                    cbusy[i] = false;
                }
                if (!SelectCenter(xy, npoints, nvars, ref ct, cbusy, k, ref d2, ref p, ref tmp))
                {
                    info = -3;
                    return;
                }

                //
                // Update centers:
                // 2. update center positions
                //
                for (i = 0; i <= npoints - 1; i++)
                {
                    xyc[i] = -1;
                }

                var newCond = new int[k];
                cond.CopyTo(newCond, 0);
                Shuffle(newCond);
                long cnt = 0;
                while (true)
                {

                    //
                    // fill XYC with center numbers
                    //
                    waschanges = false;
                    condcomplete = false;

                    for (j = 0; j < k; j++)
                    {
                        csizes[j] = 0;
                    }

                    for (i = 0; i < npoints; i++)
                    {
                        cclosest = -1;
                        dclosest = Maxrealnumber;
                        for (j = 0; j <= k - 1; j++)
                        {
                            if (csizes[j] < newCond[j])
                            {
                                for (i_ = 0; i_ < nvars; i_++)
                                {
                                    tmp[i_] = xy[i, i_];
                                }
                                for (i_ = 0; i_ < nvars; i_++)
                                {
                                    tmp[i_] = tmp[i_] - ct[j, i_];
                                }
                                v = 0.0;
                                for (i_ = 0; i_ < nvars; i_++)
                                {
                                    v += tmp[i_] * tmp[i_];
                                }
                                if ((double)(v) < (double)(dclosest))
                                {
                                    cclosest = j;
                                    dclosest = v;
                                }
                            }
                        }
                        if (xyc[i] != cclosest)
                        {
                            waschanges = true;
                        }
                        xyc[i] = cclosest;
                        csizes[cclosest]++;
                    }

                    //
                    // Update centers
                    //

                    for (i = 0; i < k; i++)
                    {
                        for (j = 0; j < nvars; j++)
                        {
                            ct[i, j] = 0;
                        }
                    }
                    for (i = 0; i < npoints; i++)
                    {
                        //csizes[xyc[i]] = csizes[xyc[i]] + 1;
                        for (i_ = 0; i_ < nvars; i_++)
                        {
                            ct[xyc[i], i_] = ct[xyc[i], i_] + xy[i, i_];
                        }
                    }
                    zerosizeclusters = false;
                    for (i = 0; i < k; i++)
                    {
                        cbusy[i] = csizes[i] != 0;
                        zerosizeclusters = zerosizeclusters | csizes[i] == 0;
                    }
                    if (zerosizeclusters)
                    {

                        //
                        // Some clusters have zero size - rare, but possible.
                        // We'll choose new centers for such clusters using k-means++ rule
                        // and restart algorithm
                        //
                        if (!SelectCenter(xy, npoints, nvars, ref ct, cbusy, k, ref d2, ref p, ref tmp))
                        {
                            info = -3;
                            return;
                        }
                        continue;
                    }
                    for (j = 0; j < k; j++)
                    {
                        v = (double)1 / (double)csizes[j];
                        for (i_ = 0; i_ < nvars; i_++)
                        {
                            ct[j, i_] = v * ct[j, i_];
                        }
                    }

                    //
                    // if nothing has changed during iteration
                    //
                    int[] csize = new int[csizes.Length];
                    csizes.CopyTo(csize, 0);

                    for (int l = 0; l < csize.Length - 1; l++)
                    {
                        for (int m = l + 1; m < csize.Length; m++)
                        {
                            if (csize[l] > csize[m])
                            {
                                int tmpSize = csize[l];
                                csize[l] = csize[m];
                                csize[m] = tmpSize;
                            }
                        }
                    }

                    int[] csize2 = new int[cond.Length];
                    cond.CopyTo(csize2, 0);

                    for (int l = 0; l < csize2.Length - 1; l++)
                    {
                        for (int m = l + 1; m < csize2.Length; m++)
                        {
                            if (csize2[l] > csize2[m])
                            {
                                int tmpSize = csize2[l];
                                csize2[l] = csize2[m];
                                csize2[m] = tmpSize;
                            }
                        }
                    }

                    for (int l = 0; l < csize.Length; l++)
                    {
                        if (csize[l] > csize2[l])
                        {
                            condcomplete = true;
                        }
                    }

                    if (((!waschanges) && (!condcomplete)) || (cnt == 5000))
                    {
                        break;
                    }
                    cnt++;
                }

                //
                // 3. Calculate E, compare with best centers found so far
                //
                e = 0;
                e2 = 0;
                for (i = 0; i < npoints; i++)
                {
                    for (i_ = 0; i_ < nvars; i_++)
                    {
                        tmp[i_] = xy[i, i_];
                    }
                    for (i_ = 0; i_ < nvars; i_++)
                    {
                        tmp[i_] = tmp[i_] - ct[xyc[i], i_];
                    }
                    v = 0.0;
                    for (i_ = 0; i_ < nvars; i_++)
                    {
                        v += tmp[i_] * tmp[i_];
                    }
                    e = e + v;
                    e2 = e2 + Math.Sqrt(v);

                }
                if ((double)(e) < (double)(ebest))
                {

                    //
                    // store partition.
                    //
                    ebest = e;
                    ebest2 = e2;
                    CopyMatrix(ct, 0, k - 1, 0, nvars - 1, ref ctbest, 0, k - 1, 0, nvars - 1);
                    for (i = 0; i <= npoints - 1; i++)
                    {
                        xycbest[i] = xyc[i];
                    }
                }
            }

            //
            // Copy and transpose
            //
            c = new double[nvars - 1 + 1, k - 1 + 1];
            CopyAndTranspose(ctbest, 0, k - 1, 0, nvars - 1, ref c, 0, nvars - 1, 0, k - 1);
            for (i = 0; i <= npoints - 1; i++)
            {
                xyc[i] = xycbest[i];
            }
        }


        /*************************************************************************
        Select center for a new cluster using k-means++ rule
        *************************************************************************/
        private static bool SelectCenter(double[,] xy,
            int npoints,
            int nvars,
            ref double[,] centers,
            bool[] busycenters,
            int ccnt,
            ref double[] d2,
            ref double[] p,
            ref double[] tmp)
        {
            bool result = new bool();
            int i = 0;
            int j = 0;
            int cc = 0;
            double v = 0;
            double s = 0;
            int i_ = 0;

            busycenters = (bool[])busycenters.Clone();

            result = true;
            for (cc = 0; cc <= ccnt - 1; cc++)
            {
                if (!busycenters[cc])
                {

                    //
                    // fill D2
                    //
                    for (i = 0; i <= npoints - 1; i++)
                    {
                        d2[i] = Maxrealnumber;
                        for (j = 0; j <= ccnt - 1; j++)
                        {
                            if (busycenters[j])
                            {
                                for (i_ = 0; i_ <= nvars - 1; i_++)
                                {
                                    tmp[i_] = xy[i, i_];
                                }
                                for (i_ = 0; i_ <= nvars - 1; i_++)
                                {
                                    tmp[i_] = tmp[i_] - centers[j, i_];
                                }
                                v = 0.0;
                                for (i_ = 0; i_ <= nvars - 1; i_++)
                                {
                                    v += tmp[i_] * tmp[i_];
                                }
                                if ((double)(v) < (double)(d2[i]))
                                {
                                    d2[i] = v;
                                }
                            }
                        }
                    }

                    //
                    // calculate P (non-cumulative)
                    //
                    s = 0;
                    for (i = 0; i <= npoints - 1; i++)
                    {
                        s = s + d2[i];
                    }
                    if ((double)(s) == (double)(0))
                    {
                        result = false;
                        return result;
                    }
                    s = 1 / s;
                    for (i_ = 0; i_ <= npoints - 1; i_++)
                    {
                        p[i_] = s * d2[i_];
                    }

                    //
                    // choose one of points with probability P
                    // random number within (0,1) is generated and
                    // inverse empirical CDF is used to randomly choose a point.
                    //
                    s = 0;
                    v = RandomReal();
                    for (i = 0; i <= npoints - 1; i++)
                    {
                        s = s + p[i];
                        if ((double)(v) <= (double)(s) | i == npoints - 1)
                        {
                            for (i_ = 0; i_ <= nvars - 1; i_++)
                            {
                                centers[cc, i_] = xy[i, i_];
                            }
                            busycenters[cc] = true;
                            break;
                        }
                    }
                }
            }
            return result;
        }

        public static int RandomInteger(int n)
        {
            int r;
            lock (RndObject) { r = RndObject.Next(n); }
            return r;
        }

        public static double RandomReal()
        {
            double r = 0;
            lock (RndObject) { r = RndObject.NextDouble(); }
            return r;
        }

        public static void CopyAndTranspose(double[,] a,
           int is1,
           int is2,
           int js1,
           int js2,
           ref double[,] b,
           int id1,
           int id2,
           int jd1,
           int jd2)
        {
            int isrc = 0;
            int jdst = 0;
            int i_ = 0;
            int i1_ = 0;

            if (is1 > is2 | js1 > js2)
            {
                return;
            }
            Assert(is2 - is1 == jd2 - jd1, "CopyAndTranspose: different sizes!");
            Assert(js2 - js1 == id2 - id1, "CopyAndTranspose: different sizes!");
            for (isrc = is1; isrc <= is2; isrc++)
            {
                jdst = isrc - is1 + jd1;
                i1_ = (js1) - (id1);
                for (i_ = id1; i_ <= id2; i_++)
                {
                    b[i_, jdst] = a[isrc, i_ + i1_];
                }
            }
        }

        public static void CopyMatrix(double[,] a,
            int is1,
            int is2,
            int js1,
            int js2,
            ref double[,] b,
            int id1,
            int id2,
            int jd1,
            int jd2)
        {
            int isrc = 0;
            int idst = 0;
            int i_ = 0;
            int i1_ = 0;

            if (is1 > is2 | js1 > js2)
            {
                return;
            }
            Assert(is2 - is1 == id2 - id1, "CopyMatrix: different sizes!");
            Assert(js2 - js1 == jd2 - jd1, "CopyMatrix: different sizes!");
            for (isrc = is1; isrc <= is2; isrc++)
            {
                idst = isrc - is1 + id1;
                i1_ = (js1) - (jd1);
                for (i_ = jd1; i_ <= jd2; i_++)
                {
                    b[idst, i_] = a[isrc, i_ + i1_];
                }
            }
        }

        public static void Assert(bool cond, string s)
        {
            if (!cond)
                throw new Alglibexception(s);
        }

        public static System.Random RndObject = new System.Random(System.DateTime.Now.Millisecond + 1000 * System.DateTime.Now.Second + 60 * 1000 * System.DateTime.Now.Minute);

        public const double Machineepsilon = 5E-16;
        public const double Maxrealnumber = 1E300;
        public const double Minrealnumber = 1E-300;
    }
}
