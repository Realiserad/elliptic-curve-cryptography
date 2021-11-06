using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

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
    public static class BigIntegerExtensions
    {
        /// <summary>
        /// Converts a <see cref="BigInteger"/> to a binary string.
        /// Implemented by Kevin P. Rice at https://stackoverflow.com/users/733805
        /// </summary>
        /// <param name="bigint">A <see cref="BigInteger"/>.</param>
        /// <returns>
        /// A <see cref="string"/> containing a binary representation of the supplied <see cref="BigInteger"/>.
        /// </returns>
        public static string ToBinaryString(this BigInteger bigint)
        {
            var bytes = bigint.ToByteArray();
            var idx = bytes.Length - 1;

            // Create a StringBuilder having appropriate capacity.
            var base2 = new StringBuilder(bytes.Length * 8);

            // Convert first byte to binary.
            var binary = Convert.ToString(bytes[idx], 2);

            // Ensure leading zero exists if value is positive.
            if (binary[0] != '0' && bigint.Sign == 1)
            {
                base2.Append('0');
            }

            // Append binary string to StringBuilder.
            base2.Append(binary);

            // Convert remaining bytes adding leading zeros.
            for (idx--; idx >= 0; idx--)
            {
                base2.Append(Convert.ToString(bytes[idx], 2).PadLeft(8, '0'));
            }

            return base2.ToString();
        }

        /// <summary>
        /// Calculate the multiplicative inverse of a mod n, e.g
        /// find a number k such that k * a = 1 mod n.
        /// </summary>
        /// <param name="a">The number whose multiplicative inverse should be found.</param>
        /// <param name="n">The modulus.</param>
        /// <returns>A number k such that k * a = 1 mod n</returns>
        public static BigInteger ModInv(this BigInteger a, BigInteger n)
        {
            BigInteger i = a, v = 0, d = 1;
            while (a > 0)
            {
                BigInteger t = i / a, x = a;
                a = i % x;
                i = x;
                x = d;
                d = v - t * x;
                v = x;
            }
            v %= a;
            if (v < 0) v = (v + a) % a;
            return v;
        }
    }

    public static class StringExtensions
    {
        /// <summary>
        /// Splits a <see cref="string"/> into chunks of specified size.
        /// </summary>
        /// <param name="str">The <see cref="string"/> to split.</param>
        /// <param name="chunkSize">The maximum size of a chunk.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> for the chunks produced</returns>
        public static IEnumerable<string> Chunks(this string str, int chunkSize)
        {
            for (int i = 0; i < str.Length; i += chunkSize)
            {
                yield return str.Substring(i, Math.Min(chunkSize, str.Length - i));
            }
        }
    }
}
