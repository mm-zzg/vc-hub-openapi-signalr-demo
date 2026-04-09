using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace OpenApiDemo
{
    public static class CertUtil
    {


      

        static X509Certificate2 certficate = new("openApiSign.crt");

        public static X509Certificate2 Certificate => certficate;


      


        public static bool Verify(byte[] data, byte[] signature)
        {
           
            byte[] hash = SHA1.HashData(data);
            var rsa = certficate.GetRSAPublicKey();
            return rsa!.VerifyHash(hash, signature, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
        }

    }
}
