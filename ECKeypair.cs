using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Security.Cryptography;
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
    internal class ECKeypair
    {
        private BigInteger privateKey = BigInteger.Zero;
        private ECPoint publicKey = ECPoint.INF;
        private IEllipticCurve curve;

        /// <summary>
        /// Get the private key.
        /// </summary>
        public BigInteger PrivateKey
        {
            get { return privateKey; }
        }

        /// <summary>
        /// Get the public key.
        /// </summary>
        public ECPoint PublicKey
        {
            get { return publicKey; }
        }

        /// <summary>
        /// Determine if this keypair has a public key.
        /// </summary>
        public bool HasPublicKey
        {
            get { return publicKey != ECPoint.INF; }
        }

        /// <summary>
        /// Determine if this keypair has a private key.
        /// </summary>
        public bool HasPrivateKey
        {
            get { return privateKey != BigInteger.Zero; }
        }

        /// <summary>
        /// Create an EC keypair from an existing keypair.
        /// </summary>
        /// <param name="publicKey">The public key or zero.</param>
        /// <param name="privateKey">The private key or zero.</param>
        /// <param name="curve">The curve used to generate the keypair.</param>
        public ECKeypair(ECPoint publicKey, BigInteger privateKey, IEllipticCurve curve)
        {
            if (curve == null)
                throw new ArgumentNullException("No curve specified.");
            if (publicKey != ECPoint.INF)
            {
                if (publicKey.X > BigInteger.Pow(2, curve.BitSize) - 1)
                {
                    throw new ArgumentException("Bad size on public key (x-coordinate).");
                }
                if (publicKey.Y > BigInteger.Pow(2, curve.BitSize) - 1)
                {
                    throw new ArgumentException("Bad size on public key (y-coordinate).");
                }
                if (!curve.OnCurve(publicKey))
                {
                    throw new ArgumentException("The public key is not a valid curve point.");
                }
            }
            if (privateKey != BigInteger.Zero && privateKey > BigInteger.Pow(2, curve.BitSize) - 1)
                throw new ArgumentException("Bad size on private key.");
            this.publicKey = publicKey;
            this.privateKey = privateKey;
            this.curve = curve;
        }

        /// <summary>
        /// Load a keypair from a PEM file.
        /// </summary>
        /// <param name="file">The path to the PEM file where the keys are stored.</param>
        /// <returns>A keypair or null if an error occurred.</returns>
        public static ECKeypair Load(string file)
        {
            try
            {
                string line;
                BigInteger privateKey = BigInteger.Zero;
                ECPoint publicKey = ECPoint.INF;
                IEllipticCurve curve = null;

                using (var reader = new StreamReader(file))
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line == "-----BEGIN EC PARAMETERS----")
                            curve = ReadEllipticCurve(reader);
                        else if (line == "-----BEGIN EC PUBLIC KEY----")
                            publicKey = ReadPublicKey(reader);
                        else if (line == "-----BEGIN EC PUBLIC KEY----")
                            privateKey = ReadPrivateKey(reader);
                    }
                }
                if (privateKey == null && publicKey == null)
                {
                    throw new FormatException("File does not contain any valid keys.");
                }
                return new ECKeypair(publicKey, privateKey, curve);
            }
            catch
            {
                return null;
            }
        }

        private static IEllipticCurve ReadEllipticCurve(StreamReader reader)
        {
            string line = reader.ReadLine();
            var parameters = new List<string>();
            while (line != "-----END EC PRIVATE KEY-----")
            {
                if (line == null)
                {
                    throw new FormatException("Unexpected end of file.");
                }
                parameters.Add(line);
                line = reader.ReadLine();
            }
            if (parameters.Count == 1)
            {
                if (parameters[0] == new Secp128r1().ToString())
                    return new Secp128r1();
            }
            // Possibly support a curve specified by (P, A, B, N)?
            return null;
        }

        private static BigInteger ReadPrivateKey(StreamReader reader)
        {
            string line = reader.ReadLine();
            var sb = new StringBuilder();
            while (line != "-----END EC PRIVATE KEY-----")
            {
                if (line == null)
                {
                    throw new FormatException("Unexpected end of file.");
                }
                sb.Append(line);
                line = reader.ReadLine();
            }
            return new BigInteger(Convert.FromBase64String(sb.ToString()));
        }

        private static ECPoint ReadPublicKey(StreamReader reader)
        {
            string line = reader.ReadLine();
            var sb = new StringBuilder();
            while (line != "-----END EC PUBLIC KEY-----")
            {
                if (line == null)
                {
                    throw new FormatException("Unexpected end of file.");
                }
                sb.Append(line);
                line = reader.ReadLine();
            }
            BigInteger x = new BigInteger(Convert.FromBase64String(sb.ToString().Split(new char[] { '#' })[0]));
            BigInteger y = new BigInteger(Convert.FromBase64String(sb.ToString().Split(new char[] { '#' })[1]));
            return new ECPoint(x, y);
        }

        /// <summary>
        /// Store the keypair in a PEM file.
        /// Description of the format used: https://goo.gl/zAtXSj
        /// </summary>
        /// <param name="file">The name of the keyfile, will be created or overwritten.</param>
        /// <param name="savePrivateKey">True if the private key should be written, defaults to false</param>
        /// <returns>True if the keypair was saved successfully.</returns>
        public bool Store(string file, bool savePrivateKey = false)
        {
            try
            {
                using (var writer = new StreamWriter(file))
                {
                    writer.WriteLine("-----BEGIN EC PARAMETERS-----");
                    writer.WriteLine(curve.ToString());
                    writer.WriteLine("-----END EC PARAMETERS-----");
                    if (HasPrivateKey && savePrivateKey)
                    {
                        writer.WriteLine("-----BEGIN EC PRIVATE KEY-----");
                        string privateKeyBase64 = Convert.ToBase64String(privateKey.ToByteArray());
                        foreach (string chunk in privateKeyBase64.Chunks(64))
                        {
                            writer.WriteLine(chunk);
                        }
                        writer.WriteLine("-----END EC PRIVATE KEY-----");
                    }
                    if (HasPublicKey)
                    {
                        writer.WriteLine("-----BEGIN EC PUBLIC KEY-----");
                        string publicKeyBase64 = Convert.ToBase64String(publicKey.X.ToByteArray()) + 
                            "#" + Convert.ToBase64String(publicKey.Y.ToByteArray());
                        foreach (string chunk in publicKeyBase64.Chunks(64))
                        {
                            writer.WriteLine(chunk);
                        }
                        writer.WriteLine("-----END EC PUBLIC KEY-----");
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
