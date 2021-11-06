using System;
using System.Globalization;
using System.Numerics;

namespace EPK.Crypto
{
    // Copyright (c) 2015 Bastian Fredriksson <bastianf@kth.se>
    //
    // Permission is hereby granted, free of charge, to any person obtaining a copy 
    // of this software and associated documentation files (the "Software"), to deal 
    // in the Software without restriction, including without limitation the rights 
    // to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
    // copies of the Software, and to permit persons to whom the Software is furnished 
    // to do so, subject to the following conditions:
    // 
    // The above copyright notice and this permission notice shall be included in all 
    // copies or substantial portions of the Software.
    //
    // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    // IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    // FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    // AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    // LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    // OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    // THE SOFTWARE.

    /// <summary>
    /// An implementation of the elliptic curve secp128r1. Parameters for this
    /// curve can be found in "SEC 2: Recommended Elliptic Curve Domain Parameters"
    /// available at http://www.secg.org/SEC2-Ver-1.0.pdf
    /// </summary>
    internal class Secp128r1 : IEllipticCurve
    {
        // secp112r1 parameters
        private static BigInteger P = BigInteger.Parse("0fffffffdffffffffffffffffffffffff", NumberStyles.HexNumber);
        private static BigInteger A = BigInteger.Parse("0fffffffdfffffffffffffffffffffffc", NumberStyles.HexNumber);
        private static BigInteger B = BigInteger.Parse("0e87579c11079f43dd824993c2cee5ed3", NumberStyles.HexNumber);
        private static BigInteger GX = BigInteger.Parse("0161ff7528b899b2d0c28607ca52c5b86", NumberStyles.HexNumber);
        private static BigInteger GY = BigInteger.Parse("0cf5ac8395bafeb13c02da292dded7a83", NumberStyles.HexNumber);
        private static BigInteger N = BigInteger.Parse("0fffffffe0000000075a30d1b9038a115", NumberStyles.HexNumber);
        
        public ECPoint Add(ECPoint a, ECPoint b)
        {
            if (a == ECPoint.INF)
            {
                return b;
            }
            if (b == ECPoint.INF)
            {
                return a;
            }

            // Calculate the slope k of the line crossing p0 and p1
            BigInteger k;
            if (a == b)
            {
                var dy = 3 * BigInteger.ModPow(a.X, 2, P) + A;
                var dx = 2 * a.Y;
                k = dy * dx.ModInv(P);
            }
            else
            {
                var dy = b.Y - a.Y;
                var dx = b.X - a.X;
                k = dy * dx.ModInv(P);
            }

            // Calculate the third intersection point c = (x, y) and reflect on x-axis
            var x = BigInteger.Pow(k, 2) - a.X - b.X;
            var y = k * a.X - x - a.Y;

            return new ECPoint(x % P, y % P);
        }

        public ECPoint Multiply(ECPoint g, BigInteger e)
        {
            if (e <= 0) throw new ArgumentException("Exponent must be greater than zero");

            var n  = ECPoint.INF;
            var bin = e.ToBinaryString();

            for (int i = bin.Length - 1; i >= 0; i--)
            {
                if (bin[i] == '1')
                {
                    n = Add(n, g);
                }
                n = Add(n, n);
            }
            return n;
        }

        public ECPoint BasePoint
        {
            get { return new ECPoint(GX, GY); }
        }

        public BigInteger Order
        {
            get { return N; }
        }

        public bool OnCurve(ECPoint p)
        {
            if (p == ECPoint.INF)
                return true;
            return BigInteger.Pow(p.Y, 2) % P == (BigInteger.Pow(p.X, 3) + A * p.X + B) % P;
        }

        public int BitSize
        {
            get { return 112; }
        }

        /// <summary>
        /// Returns the name of this elliptic curve.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "secp128r1";
        }
    }
}
