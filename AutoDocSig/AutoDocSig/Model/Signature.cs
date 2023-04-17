﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography.Pkcs;
using System.Xml;
namespace AutoDocSig.Model
{
    class Signature
    {
        Logger logger = new Logger(AppDomain.CurrentDomain.BaseDirectory);
        X509Certificate2 certificate;
        bool detached;
        //RSACryptoServiceProvider RSAKey;
        public Signature(string _signaturePath, string _password, bool _detached)
        {
            try
            {
                detached = _detached;
                certificate = new X509Certificate2(_signaturePath, _password);
                logger.Write("Сертификат " + certificate.FriendlyName + " загружен");
                /*RSAKey = (RSACryptoServiceProvider)certificate.GetRSAPrivateKey();
                if (RSAKey != null)
                {
                    logger.Write("Ключ инициализирован");
                }
                else
                {
                    CspParameters cspParams = new()
                    {
                        KeyContainerName = "XML_DSIG_RSA_KEY"
                    };
                    RSAKey = new(cspParams);
                }*/
            }
            catch (Exception e)
            {
                logger.Write(e);
            }
        }

        public void SignFiles(string [] _filePathList, string _outputDirectory)
        {
            try
            {
                foreach (var l_filePath in _filePathList)
                {
                    /*XmlDocument xmlDoc = new()
                    {
                        PreserveWhitespace = true
                    };
                    xmlDoc.Load(l_filePath);
                    SignXml(xmlDoc);
                    logger.Write("Файл " + l_filePath + " подписан");
                    SaveSignedXml(xmlDoc, _outputDirectory);
                    logger.Write("Подписанный файл " + l_filePath + " сохранен в директорию " + _outputDirectory);
                    xmlDoc.Save(l_filePath);
                    File.Delete(l_filePath);*/
                    var l_fileAsByteArray = ReadFileAsByteArray(l_filePath);
                    logger.Write("Файл " + l_filePath + " загружен");
                    l_fileAsByteArray = SignFile(certificate, l_fileAsByteArray, detached);
                    logger.Write("Файл " + l_filePath + " подписан");
                    SaveFile(l_filePath, l_fileAsByteArray);
                    logger.Write("Файл " + l_filePath + " сохранен");
                }
            }
            catch (Exception e)
            {
                logger.Write(e);
            }
        }

        public byte[] ReadFileAsByteArray(string _path)
        {
            byte[] content;
            content = File.ReadAllBytes(_path);
            return content;
        }

        public byte[] SignFile(X509Certificate2 certificate, byte[] data, bool detached)
        {
            var contentInfo = new ContentInfo(data);
            var signedCms = new SignedCms(contentInfo, detached);
            var cmsSigner = new CmsSigner(SubjectIdentifierType.IssuerAndSerialNumber, certificate);
            signedCms.ComputeSignature(cmsSigner, true);
            return signedCms.Encode();
        }
        public void SaveFile(string _path, byte[] fileContent)
        {
            var l_name = _path.Split('/').Last();
            File.WriteAllBytes(_path + ".sig", fileContent);
        }

        /*public void SignXml(XmlDocument _xmlDoc)
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
        }*/
    }
}
