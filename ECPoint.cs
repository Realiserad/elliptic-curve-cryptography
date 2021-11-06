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
    /// Contains a point (x, y) on an elliptic curve.
    /// </summary>
    struct ECPoint
    {
        private BigInteger x, y;

        /// <summary>
        /// Get the point at infinity.
        /// </summary>
        public static ECPoint INF
        {
            get { return new ECPoint(BigInteger.Zero, BigInteger.Zero); }
        }

        public ECPoint(BigInteger x, BigInteger y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Get the carthesian x-coordinate of this point.
        /// </summary>
        public BigInteger X
        {
            get { return x; }
        }

        /// <summary>
        /// Get the carthesian y-coordinate of this point.
        /// </summary>
        public BigInteger Y
        {
            get { return y; }
        }

        /// <summary>
        /// Compares this point with another object.
        /// <para/>
        /// a.Equals(b) if:
        /// <para/>b is an instance of the class ECPoint
        /// <para/>a.X == b.X
        /// <para/>a.Y == b.Y
        /// </summary>
        /// <param name="obj">The object to compare with.</param>
        /// <returns>True if the object is equal to this object.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj is ECPoint)
            {
                return (ECPoint)obj == this;
            }
            return false;
        }
        
        public override int GetHashCode()
        {
            unchecked
            {
                return x.GetHashCode() + y.GetHashCode();
            }
        }

        public static bool operator ==(ECPoint p0, ECPoint p1)
        {
            return p0.X == p1.X && p1.Y == p1.Y;
        }

        public static bool operator !=(ECPoint p0, ECPoint p1)
        {
            return !(p0 == p1);
        }
    }
}
