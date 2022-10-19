using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace DistSysAcw
{
    public static class StoreKey
    {
        private static string ContainerName;

        public static void GenerateKeys(string containerName)
        {
            ContainerName = containerName;
            var parameters = new CspParameters
            {
                KeyContainerName = containerName
            };
            using var rsa = new RSACryptoServiceProvider(parameters);
        }

        public static string GetPublicKey()
        {
            var parameters = new CspParameters
            {
                KeyContainerName = ContainerName
            };

            using var rsa = new RSACryptoServiceProvider(parameters);

            return rsa.ToXmlString(false);
        }

        public static string Sign(string message)
        {
            var parameters = new CspParameters
            {
                KeyContainerName = ContainerName
            };

            using var rsa = new RSACryptoServiceProvider(parameters);

            byte[] asciiByteMessage = System.Text.Encoding.ASCII.GetBytes(message);
            byte[] signedMessage = rsa.SignData(asciiByteMessage, SHA1.Create());
            return BitConverter.ToString(signedMessage);
        }

        public static byte[] decryptBytes(string input)
        {
            var parameters = new CspParameters
            {
                KeyContainerName = ContainerName
            };

            using var rsa = new RSACryptoServiceProvider(parameters);

            byte[] byteData = HexConvert(input);
            return rsa.Decrypt(byteData, false);
        }

        private static byte[] HexConvert(string data)
        {
            string[] hexData = data.Split('-');
            byte[] convertedData = new byte[hexData.Length];
            for (int i = 0; i < hexData.Length; i++)
            {
                byte converted = Convert.ToByte(hexData[i], 16);
                convertedData[i] = converted;
            }
            return convertedData;
        }
    }
}
