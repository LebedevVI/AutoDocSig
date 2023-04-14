using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace AutoDocSig.Model
{
    class Signature
    {
        X509Certificate2 certificate;
        RSACryptoServiceProvider RSAKey;
        public Signature(string _signaturePath)
        {
            certificate = new X509Certificate2(_signaturePath, "123");
            RSAKey = (RSACryptoServiceProvider)certificate.GetRSAPrivateKey();
           /* CspParameters l_cspParams = new()
            {
                KeyContainerName = "XML_DSIG_RSA_KEY"
            };
            RSAKey = new(l_cspParams);*/
        }

        public void SignXml(XmlDocument _xmlDoc)
        {
            if (_xmlDoc == null)
            {
                throw new ArgumentException(null, nameof(_xmlDoc));
            }
            if (RSAKey == null)
            {
                throw new ArgumentException(null, nameof(RSAKey));

            }
            SignedXml signedXml = new(_xmlDoc)
            {
                SigningKey = RSAKey
            };
            Reference reference = new()
            {
                Uri = ""
            };
            XmlDsigEnvelopedSignatureTransform env = new();
            reference.AddTransform(env);
            signedXml.AddReference(reference);
            signedXml.ComputeSignature();
            XmlElement xmlDigitalSignature = signedXml.GetXml();
            _xmlDoc.DocumentElement.AppendChild(_xmlDoc.ImportNode(xmlDigitalSignature, true));
        }

         public void SaveSignedXml(XmlDocument _xmlDoc, string _path)
        {
            var l_name = _xmlDoc.BaseURI.Split('/').Last();
            _xmlDoc.Save(_path + "//" + l_name);
        }
    }
}
