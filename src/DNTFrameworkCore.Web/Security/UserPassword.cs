using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using DNTFrameworkCore.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace DNTFrameworkCore.Web.Security
{
    public sealed class UserPassword : IUserPassword
    {
        /* =======================
         * HASHED PASSWORD FORMATS
         * =======================
         *
         * Version 3:
         * PBKDF2 with HMAC-SHA256, 128-bit salt, 256-bit subKey, 10000 iterations.
         * Format: { 0x01, prf (UInt32), iteration count (UInt32), salt length (UInt32), salt, subKey }
         * (All UInt32s are stored big-endian.)
         */
        private const int IterationCount = 10000;
        private const int SubKeyLength = 256 / 8; // 256 bits
        private const int SaltSize = 128 / 8; // 128 bits

        private readonly RandomNumberGenerator _rand = RandomNumberGenerator.Create();

        public string HashPassword(string password)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            return Convert.ToBase64String(HashPasswordInternal(password));
        }

        private byte[] HashPasswordInternal(string password, KeyDerivationPrf prf = KeyDerivationPrf.HMACSHA256,
            int iterationCount = IterationCount, int saltSize = SaltSize, int numBytesRequested = SubKeyLength)
        {
            var salt = new byte[saltSize];
            _rand.GetBytes(salt);
            var subKey = KeyDerivation.Pbkdf2(password, salt, prf, iterationCount, numBytesRequested);

            var outputBytes = new byte[13 + salt.Length + subKey.Length];
            outputBytes[0] = 0x01; // format marker
            WriteNetworkByteOrder(outputBytes, 1, (uint) prf);
            WriteNetworkByteOrder(outputBytes, 5, (uint) iterationCount);
            WriteNetworkByteOrder(outputBytes, 9, (uint) saltSize);
            Buffer.BlockCopy(salt, 0, outputBytes, 13, salt.Length);
            Buffer.BlockCopy(subKey, 0, outputBytes, 13 + saltSize, subKey.Length);
            return outputBytes;
        }

        private static uint ReadNetworkByteOrder(byte[] buffer, int offset)
        {
            return ((uint) (buffer[offset + 0]) << 24)
                   | ((uint) (buffer[offset + 1]) << 16)
                   | ((uint) (buffer[offset + 2]) << 8)
                   | buffer[offset + 3];
        }

        public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            if (hashedPassword == null)
            {
                throw new ArgumentNullException(nameof(hashedPassword));
            }

            if (providedPassword == null)
            {
                throw new ArgumentNullException(nameof(providedPassword));
            }

            var decodedHashedPassword = Convert.FromBase64String(hashedPassword);

            // read the format marker from the hashed password
            if (decodedHashedPassword.Length == 0)
            {
                return PasswordVerificationResult.Failed;
            }

            // Verify hashing format.
            if (decodedHashedPassword[0] != 0x01)
            {
                // Unknown format header.
                return PasswordVerificationResult.Failed;
            }

            if (VerifyHashedPasswordInternal(decodedHashedPassword, providedPassword, out var embeddedIterationCount))
            {
                // If this hasher was configured with a higher iteration count, change the entry now.
                return embeddedIterationCount < IterationCount
                    ? PasswordVerificationResult.SuccessRehashNeeded
                    : PasswordVerificationResult.Success;
            }

            return PasswordVerificationResult.Failed;
        }

        private static bool VerifyHashedPasswordInternal(byte[] hashedPassword, string providedPassword, out int iterationCount)
        {
            iterationCount = default;

            try
            {
                // Read header information
                var prf = (KeyDerivationPrf) ReadNetworkByteOrder(hashedPassword, 1);
                iterationCount = (int) ReadNetworkByteOrder(hashedPassword, 5);
                var saltLength = (int) ReadNetworkByteOrder(hashedPassword, 9);

                // Read the salt: must be >= 128 bits
                if (saltLength < 128 / 8)
                {
                    return false;
                }

                var salt = new byte[saltLength];
                Buffer.BlockCopy(hashedPassword, 13, salt, 0, salt.Length);

                // Read the subKey (the rest of the payload): must be >= 128 bits
                var subKeyLength = hashedPassword.Length - 13 - salt.Length;
                if (subKeyLength < 128 / 8)
                {
                    return false;
                }

                var expectedSubKey = new byte[subKeyLength];
                Buffer.BlockCopy(hashedPassword, 13 + salt.Length, expectedSubKey, 0, expectedSubKey.Length);

                // Hash the incoming password and verify it
                var actualSubKey = KeyDerivation.Pbkdf2(providedPassword, salt, prf, iterationCount, subKeyLength);
                return ByteArraysEqual(actualSubKey, expectedSubKey);
            }
            catch
            {
                // This should never occur except in the case of a malformed payload, where
                // we might go off the end of the array. Regardless, a malformed payload
                // implies verification failed.
                return false;
            }
        }

        private static void WriteNetworkByteOrder(byte[] buffer, int offset, uint value)
        {
            buffer[offset + 0] = (byte) (value >> 24);
            buffer[offset + 1] = (byte) (value >> 16);
            buffer[offset + 2] = (byte) (value >> 8);
            buffer[offset + 3] = (byte) (value >> 0);
        }

        // Compares two byte arrays for equality. The method is specifically written so that the loop is not optimized.
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static bool ByteArraysEqual(byte[] a, byte[] b)
        {
            if (a == null && b == null)
            {
                return true;
            }

            if (a == null || b == null || a.Length != b.Length)
            {
                return false;
            }

            var areSame = true;
            for (var i = 0; i < a.Length; i++)
            {
                areSame &= (a[i] == b[i]);
            }

            return areSame;
        }
    }
}