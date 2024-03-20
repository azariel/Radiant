﻿using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Radiant.Common.OSDependent;

namespace Radiant.Common.Utils
{
    public static class HttpsCertificateHelper
    {
        public static X509Certificate2 GetSelfSignedCertificate(string aFriendlyCertificateName)
        {
            var password = Guid.NewGuid().ToString();
            var commonName = aFriendlyCertificateName;
            var rsaKeySize = 2048;
            var years = 5;
            var hashAlgorithm = HashAlgorithmName.SHA256;

            using (var rsa = RSA.Create(rsaKeySize))
            {
                var request = new CertificateRequest($"cn={commonName}", rsa, hashAlgorithm, RSASignaturePadding.Pkcs1);

                request.CertificateExtensions.Add(
                    new X509KeyUsageExtension(X509KeyUsageFlags.DataEncipherment | X509KeyUsageFlags.KeyEncipherment | X509KeyUsageFlags.DigitalSignature, false)
                );
                request.CertificateExtensions.Add(
                    new X509EnhancedKeyUsageExtension(
                        new OidCollection { new Oid("1.3.6.1.5.5.7.3.1") }, false)
                );

                var certificate = request.CreateSelfSigned(DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddYears(years));

                if (OperatingSystemHelper.GetCurrentOperatingSystem() == SupportedOperatingSystem.Windows)
                    certificate.FriendlyName = commonName;

                // Return the PFX exported version that contains the key
                return new X509Certificate2(certificate.Export(X509ContentType.Pfx, password), password, X509KeyStorageFlags.MachineKeySet);
            }
        }
    }
}
