using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Tutorials.AzureFunctions.DependencyInjection.Extensions
{
    public static class ConfigurationExtensions
    {
        private const string _clientThumbprint = "clientThumbprint";

        public static X509Certificate2 GetClientCert(this IConfiguration configuration)
        {
            using (X509Store str = new X509Store())
            {
                str.Open(OpenFlags.ReadOnly);
                X509Certificate2 result = str.Certificates
                    .OfType<X509Certificate2>()
                    .FirstOrDefault(x => x.Thumbprint.ToUpper().Trim() == configuration[_clientThumbprint]?.ToUpper()?.Trim());
                str.Close();
                return result;
            }
        }
    }
}
