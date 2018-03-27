//Copyright 2017 Open Science, Engineering, Research and Development Information Systems Open, LLC. (OSRS Open)
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//       http://www.apache.org/licenses/LICENSE-2.0
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

/* COPYRIGHT INFO FROM ORIGINAL SOURCE
 * 
Triangle
A Two-Dimensional Quality Mesh Generator and Delaunay Triangulator.
Version 1.6

Show Me
A Display Program for Meshes and More.
Version 1.6

Copyright 1993, 1995, 1997, 1998, 2002, 2005 Jonathan Richard Shewchuk
2360 Woolsey #H
Berkeley, California  94705-1927
Please send bugs and comments to jrs@cs.berkeley.edu

Created as part of the Quake project (tools for earthquake simulation).
Supported in part by NSF Grant CMS-9318163 and an NSERC 1967 Scholarship.
There is no warranty whatsoever.  Use at your own risk.


Triangle generates exact Delaunay triangulations, constrained Delaunay
triangulations, conforming Delaunay triangulations, Voronoi diagrams, and
high-quality triangular meshes.  The latter can be generated with no small
or large angles, and are thus suitable for finite element analysis.
Show Me graphically displays the contents of the geometric files used by
Triangle.  Show Me can also write images in PostScript form.

Information on the algorithms used by Triangle, including complete
references, can be found in the comments at the beginning of the triangle.c
source file.  Another listing of these references, with PostScript copies
of some of the papers, is available from the Web page

    http://www.cs.cmu.edu/~quake/triangle.research.html

------------------------------------------------------------------------------

These programs may be freely redistributed under the condition that the
copyright notices (including the copy of this notice in the code comments
and the copyright notice printed when the `-h' switch is selected) are
not removed, and no compensation is received.  Private, research, and
institutional use is free.  You may distribute modified versions of this
code UNDER THE CONDITION THAT THIS CODE AND ANY MODIFICATIONS MADE TO IT
IN THE SAME FILE REMAIN UNDER COPYRIGHT OF THE ORIGINAL AUTHOR, BOTH
SOURCE AND OBJECT CODE ARE MADE FREELY AVAILABLE WITHOUT CHARGE, AND
CLEAR NOTICE IS GIVEN OF THE MODIFICATIONS.  Distribution of this code as
part of a commercial system is permissible ONLY BY DIRECT ARRANGEMENT
WITH THE AUTHOR.  (If you are not directly supplying this code to a
customer, and you are instead telling them how they can obtain it for
free, then you are not required to make any arrangement with me.)

------------------------------------------------------------------------------
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osrs.Numerics
{
    public static class ExactMathUtils
    {
        private static double splitter = 1.0d;

        public static void Fast_Two_Sum_Tail(double a, double b, ref double x, ref double y)
        {
            y = b - (x - a);
        }

        public static void Fast_Two_Sum(double a, double b, ref double x, ref double y)
        {
            x = a + b;
            Fast_Two_Sum_Tail(a, b, ref x, ref y);
        }

        public static void Two_Sum_Tail(double a, double b, double x, ref double y)
        {
            double bvirt = x - a;
            double avirt = x - bvirt;
            double bround = b - bvirt;
            double around = a - avirt;
            y = around + bround;
        }

        public static void Two_Sum(double a, double b, ref double x, ref double y)
        {
            x = a + b;
            Two_Sum_Tail(a, b, x, ref y);
        }

        public static void Two_Diff_Tail(double a, double b, double x, ref double y)
        {
            double bvirt = a - x;
            double avirt = x + bvirt;
            double bround = bvirt - b;
            double around = a - avirt;
            y = around + bround;
        }

        public static void Two_Diff(double a, double b, ref double x, ref double y)
        {
            x = a - b;
            Two_Diff_Tail(a, b, x, ref y);
        }

        public static void Split(double a, ref double ahi, ref double alo)
        {
            double c = splitter * a;
            double abig = c - a;
            ahi = c - abig;
            alo = a - ahi;
        }

        public static void Two_Product_Tail(double a, double b, double x, ref double y)
        {
            double ahi = 0.0;
            double alo = 0.0;
            double bhi = 0.0;
            double blo = 0.0;
            Split(a, ref ahi, ref alo);
            Split(b, ref bhi, ref blo);
            double err1 = x - (ahi * bhi);
            double err2 = err1 - (alo * bhi);
            double err3 = err2 - (ahi * blo);
            y = (alo * blo) - err3;
        }

        public static void Two_Product(double a, double b, ref double x, ref double y)
        {
            x = a * b;
            Two_Product_Tail(a, b, x, ref y);
        }

        /* Two_Product_Presplit() is Two_Product() where one of the inputs has       */
        /*   already been split.  Avoids redundant splitting.                        */
        public static void Two_Product_Presplit(double a, double b, double bhi, double blo, ref double x, ref double y)
        {
            double ahi = 0.0;
            double alo = 0.0;
            x = a * b;
            Split(a, ref ahi, ref alo);
            double err1 = x - (ahi * bhi);
            double err2 = err1 - (alo * bhi);
            double err3 = err2 - (ahi * blo);
            y = (alo * blo) - err3;
        }

        /* Square() can be done more quickly than Two_Product().                     */
        public static void Square_Tail(double a, double x, ref double y)
        {
            double ahi = 0.0;
            double alo = 0.0;
            Split(a, ref ahi, ref alo);
            double err1 = x - (ahi * ahi);
            double err3 = err1 - ((ahi + ahi) * alo);
            y = (alo * alo) - err3;
        }

        public static void Square(double a, ref double x, ref double y)
        {
            x = a * a;
            Square_Tail(a, x, ref y);
        }

        /* Macros for summing expansions of various fixed lengths.  These are all    */
        /*   unrolled versions of Expansion_Sum().                                   */
        public static void Two_One_Sum(double a1, double a0, double b, ref double x2, ref double x1, ref double x0)
        {
            double _i = 0.0;
            Two_Sum(a0, b, ref _i, ref x0);
            Two_Sum(a1, _i, ref x2, ref x1);
        }

        public static void Two_One_Diff(double a1, double a0, double b, ref double x2, ref double x1, ref double x0)
        {
            double _i = 0.0;
            Two_Diff(a0, b, ref _i, ref x0);
            Two_Sum(a1, _i, ref x2, ref x1);
        }

        public static void Two_Two_Sum(double a1, double a0, double b1, double b0, ref double x3, ref double x2, ref double x1, ref double x0)
        {
            double _j = 0.0;
            double _0 = 0.0;
            Two_One_Sum(a1, a0, b0, ref _j, ref _0, ref x0);
            Two_One_Sum(_j, _0, b1, ref x3, ref x2, ref x1);
        }

        public static void Two_Two_Diff(double a1, double a0, double b1, double b0, ref double x3, ref double x2, ref double x1, ref double x0)
        {
            double _j = 0.0;
            double _0 = 0.0;
            Two_One_Diff(a1, a0, b0, ref _j, ref _0, ref x0);
            Two_One_Diff(_j, _0, b1, ref x3, ref x2, ref x1);
        }

        /* Macro for multiplying a two-component expansion by a single component.    */
        public static void Two_One_Product(double a1, double a0, double b, ref double x3, ref double x2, ref double x1, ref double x0)
        {
            double bhi = 0.0;
            double blo = 0.0;
            double _i = 0.0;
            double _j = 0.0;
            double _0 = 0.0;
            double _k = 0.0;
            Split(b, ref bhi, ref blo);
            Two_Product_Presplit(a0, b, bhi, blo, ref _i, ref x0);
            Two_Product_Presplit(a1, b, bhi, blo, ref _j, ref _0);
            Two_Sum(_i, _0, ref _k, ref x1);
            Fast_Two_Sum(_j, _k, ref x3, ref x2);
        }

        /*****************************************************************************/
        /*                                                                           */
        /*  fast_expansion_sum_zeroelim()   Sum two expansions, eliminating zero     */
        /*                                  components from the output expansion.    */
        /*                                                                           */
        /*  Sets h = e + f.  See my Robust Predicates paper for details.             */
        /*                                                                           */
        /*  If round-to-even is used (as with IEEE 754), maintains the strongly      */
        /*  nonoverlapping property.  (That is, if e is strongly nonoverlapping, h   */
        /*  will be also.)  Does NOT maintain the nonoverlapping or nonadjacent      */
        /*  properties.                                                              */
        /*                                                                           */
        /*****************************************************************************/
        public static int fast_expansion_sum_zeroelim(int elen, double[] e, int flen, double[] f, double[] h)
        {
            double Q;
            double Qnew = 0.0;
            double hh = 0.0;
            int eindex, findex, hindex;
            double enow, fnow;

            enow = e[0];
            fnow = f[0];
            eindex = findex = 0;
            if ((fnow > enow) == (fnow > -enow))
            {
                Q = enow;
                enow = e[++eindex];
            }
            else
            {
                Q = fnow;
                fnow = f[++findex];
            }
            hindex = 0;
            if ((eindex < elen) && (findex < flen))
            {
                if ((fnow > enow) == (fnow > -enow))
                {
                    Fast_Two_Sum(enow, Q, ref Qnew, ref hh);
                    eindex++;
                    if (eindex < e.Length)
                        enow = e[eindex];
                    else
                        enow = 0;
                }
                else
                {
                    Fast_Two_Sum(fnow, Q, ref Qnew, ref hh);
                    findex++;
                    if (findex < f.Length)
                        fnow = f[findex];
                    else
                        fnow = 0;
                }
                Q = Qnew;
                if (hh != 0.0)
                {
                    h[hindex++] = hh;
                }
                while ((eindex < elen) && (findex < flen))
                {
                    if ((fnow > enow) == (fnow > -enow))
                    {
                        Two_Sum(Q, enow, ref Qnew, ref hh);
                        eindex++;
                        if (eindex < e.Length)
                            enow = e[eindex];
                        else
                            enow = 0;
                    }
                    else
                    {
                        Two_Sum(Q, fnow, ref Qnew, ref hh);
                        findex++;
                        if (findex < f.Length)
                            fnow = f[findex];
                        else
                            fnow = 0;
                    }
                    Q = Qnew;
                    if (hh != 0.0)
                    {
                        h[hindex++] = hh;
                    }
                }
            }
            while (eindex < elen)
            {
                Two_Sum(Q, enow, ref Qnew, ref hh);
                eindex++;
                if (eindex < e.Length)
                    enow = e[eindex];
                else
                    enow = 0;
                Q = Qnew;
                if (hh != 0.0)
                {
                    h[hindex++] = hh;
                }
            }
            while (findex < flen)
            {
                Two_Sum(Q, fnow, ref Qnew, ref hh);
                findex++;
                if (findex < f.Length)
                    fnow = f[findex];
                else
                    fnow = 0;
                Q = Qnew;
                if (hh != 0.0)
                {
                    h[hindex++] = hh;
                }
            }
            if ((Q != 0.0) || (hindex == 0))
            {
                h[hindex++] = Q;
            }
            return hindex;
        }

        /*****************************************************************************/
        /*                                                                           */
        /*  scale_expansion_zeroelim()   Multiply an expansion by a scalar,          */
        /*                               eliminating zero components from the        */
        /*                               output expansion.                           */
        /*                                                                           */
        /*  Sets h = be.  See my Robust Predicates paper for details.                */
        /*                                                                           */
        /*  Maintains the nonoverlapping property.  If round-to-even is used (as     */
        /*  with IEEE 754), maintains the strongly nonoverlapping and nonadjacent    */
        /*  properties as well.  (That is, if e has one of these properties, so      */
        /*  will h.)                                                                 */
        /*                                                                           */
        /*****************************************************************************/
        public static int scale_expansion_zeroelim(int elen, double[] e, double b, double[] h)
        {
            double Q = 0.0;
            double sum = 0.0;
            double hh = 0.0;
            double product1 = 0.0;
            double product0 = 0.0;
            int eindex, hindex;
            double enow;
            //double bvirt;
            //double avirt, bround, around;
            //double c;
            //double abig;
            //double ahi, alo, bhi, blo;
            double bhi = 0.0;
            double blo = 0.0;
            //double err1, err2, err3;

            Split(b, ref bhi, ref blo);
            Two_Product_Presplit(e[0], b, bhi, blo, ref Q, ref hh);
            hindex = 0;
            if (hh != 0)
            {
                h[hindex++] = hh;
            }
            for (eindex = 1; eindex < elen; eindex++)
            {
                enow = e[eindex];
                Two_Product_Presplit(enow, b, bhi, blo, ref product1, ref product0);
                Two_Sum(Q, product0, ref sum, ref hh);
                if (hh != 0)
                {
                    h[hindex++] = hh;
                }
                Fast_Two_Sum(product1, sum, ref Q, ref hh);
                if (hh != 0)
                {
                    h[hindex++] = hh;
                }
            }
            if ((Q != 0.0) || (hindex == 0))
            {
                h[hindex++] = Q;
            }
            return hindex;
        }

        /*****************************************************************************/
        /*                                                                           */
        /*  estimate()   Produce a one-word estimate of an expansion's value.        */
        /*                                                                           */
        /*  See my Robust Predicates paper for details.                              */
        /*                                                                           */
        /*****************************************************************************/
        public static double estimate(int eLen, double[] e)
        {
            double Q = e[0];

            for (int eindex = 1; eindex < eLen; eindex++)
            {
                Q += e[eindex];
            }

            return Q;
        }

        public static double estimate(double[] e)
        {
            return estimate(e.Length, e);
        }

        static ExactMathUtils()
        {
            double lastcheck;
            bool every_other = true;
            double half = 0.5;
            double epsilon = 1.0;
            splitter = 1.0;
            double check = 1.0;
            /* Repeatedly divide `epsilon' by two until it is too small to add to      */
            /*   one without causing roundoff.  (Also check if the sum is equal to     */
            /*   the previous sum, for machines that round up instead of using exact   */
            /*   rounding.  Not that these routines will work on such machines.)       */
            do
            {
                lastcheck = check;
                epsilon *= half;
                if (every_other)
                {
                    splitter *= 2.0;
                }
                every_other = !every_other;
                check = 1.0 + epsilon;
            } while ((check != 1.0) && (check != lastcheck));
            splitter += 1.0;
        }
    }
}
