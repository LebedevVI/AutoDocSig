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
            RSAKey = (RSACryptoServiceProvider)certificate.GetRSAPublicKey();
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

            signedXml.KeyInfo = GetCertInfo();
            signedXml.ComputeSignature();
            XmlElement xmlDigitalSignature = signedXml.GetXml();
            _xmlDoc.DocumentElement.AppendChild(_xmlDoc.ImportNode(xmlDigitalSignature, true));
        }

        public void SaveSignedXml(XmlDocument _xmlDoc, string _path)
        {
            var l_name = _xmlDoc.BaseURI.Split('/').Last();
            _xmlDoc.Save(_path + "//" + l_name);
        }

        KeyInfo GetCertInfo()
        {
            KeyInfo l_keyInfo = new KeyInfo();
            KeyInfoX509Data l_keydata = new KeyInfoX509Data(certificate);
            X509IssuerSerial l_serial = new X509IssuerSerial();
            l_serial.IssuerName = certificate.IssuerName.ToString();
            l_serial.SerialNumber = certificate.SerialNumber;
            l_keydata.AddIssuerSerial(l_serial.IssuerName, l_serial.SerialNumber);
            l_keyInfo.AddClause(l_keydata);
            return l_keyInfo;
        }

    }
}
