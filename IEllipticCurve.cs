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
    internal interface IEllipticCurve
    {
        /// <summary>
        /// Add two points on the curve.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>The sum of a and b</returns>
        ECPoint Add(ECPoint a, ECPoint b);

        /// <summary>
        /// Perform quick repeated addition.
        /// </summary>
        /// <param name="g">A base point for the curve.</param>
        /// <param name="e">The number of additions.</param>
        /// <returns>g + g + g... repeated e times</returns>
        ECPoint Multiply(ECPoint g, BigInteger e);

        /// <summary>
        /// Get a base point (generator) for the curve.
        /// </summary>
        /// <returns>A base point for the group</returns>
        ECPoint BasePoint { get; }

        /// <summary>
        /// Get the order of the group.
        /// </summary>
        /// <returns>The number of points on the curve.</returns>
        BigInteger Order { get; }

        /// <summary>
        /// Determine if a point is on the curve, e.g if p
        /// is a valid group element.
        /// </summary>
        /// <param name="p">The point.</param>
        /// <returns>True if the point is a valid group element.</returns>
        bool OnCurve(ECPoint p);

        /// <summary>
        /// Get the number of bits in the modulus.
        /// </summary>
        int BitSize { get; }
    }
}
