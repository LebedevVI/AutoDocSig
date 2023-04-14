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
            SignedXml l_signedXml = new(_xmlDoc)
            {
                SigningKey = RSAKey
            };
            Reference l_reference = new()
            {
                Uri = ""
            };
            XmlDsigEnvelopedSignatureTransform l_envelopedSignatureTransform = new();
            l_reference.AddTransform(l_envelopedSignatureTransform);
            l_signedXml.AddReference(l_reference);

            l_signedXml.KeyInfo = GetCertInfo();
            l_signedXml.ComputeSignature();
            XmlElement l_xmlDigitalSignature = l_signedXml.GetXml();
            _xmlDoc.DocumentElement.AppendChild(_xmlDoc.ImportNode(l_xmlDigitalSignature, true));
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
