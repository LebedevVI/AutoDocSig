using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Xml;
using System.IO;
using AutoDocSig.Core;
using Microsoft.Win32;

namespace AutoDocSig
{
    class MainWindowVM : ObservableObject
    {
        RSACryptoServiceProvider RSAKey;
        string inputDirectory;
        public String InputDirectory
        {
            get 
            { return inputDirectory; 
            }
            set
            {
                inputDirectory = value;
                OnPropertyChanged();
            }
        }
        string otputDirectory;
        public String OutputDirectory
        {
            get
            {
                return otputDirectory;
            }
            set
            {
                otputDirectory = value;
                OnPropertyChanged();
            }
        }
        string signaturePath;
        public String SignaturePath
        {
            get
            {
                return signaturePath;
            }
            set
            {
                signaturePath = value;
                OnPropertyChanged();
            }
        }
        bool isReady;
        public bool IsReady
        {
            get
            {
                return isReady;
            }
            set
            {
                isReady = value;
                OnPropertyChanged();
            }
        }
        public RelayCommand SelectInputDirectoryButtonClick { get; set; }
        public RelayCommand SelectOutputDirectoryButtonClick { get; set; }
        public RelayCommand SelectSignatureButtonClick { get; set; }
        public RelayCommand DoWorkButtonClick { get; set; }

        public MainWindowVM()
        {
            SelectInputDirectoryButtonClick = new RelayCommand(o => SelectInputDirectory());
            SelectOutputDirectoryButtonClick = new RelayCommand(o => SelectOutputDirectory());
            SelectSignatureButtonClick = new RelayCommand(o => SelectSignature());
            DoWorkButtonClick = new RelayCommand(o => Work());
        }

        bool CheckParams()
        {
            if (String.IsNullOrEmpty(InputDirectory) || String.IsNullOrEmpty(OutputDirectory) || String.IsNullOrEmpty(SignaturePath))
            {
                return false;
            }
            return true;
        }

        void SelectInputDirectory()
        {           
            InputDirectory = WpfFolderDialog.Win32API.SelectDirectory();
            IsReady = CheckParams();
        }

        void SelectOutputDirectory()
        {
            OutputDirectory = WpfFolderDialog.Win32API.SelectDirectory();
            IsReady = CheckParams();
        }

        void SelectSignature()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                SignaturePath = dialog.FileName;
                SignatureInizialization(SignaturePath);
                IsReady = CheckParams();
            }
        }

        void Work()
        {
            var l_filePathList = Directory.GetFiles(InputDirectory).ToList();
            foreach (var l_filePath in l_filePathList)
            {
                XmlDocument xmlDoc = new ()
                {
                    PreserveWhitespace = true
                };
                xmlDoc.Load(l_filePath);
                SignXml(xmlDoc);
                SaveSignedXml(xmlDoc);
                xmlDoc.Save(l_filePath);
                File.Delete(l_filePath);
            }
        }

        void SignatureInizialization(string _signaturePath)
        {
            CspParameters l_cspParams = new()
            {
                KeyContainerName = "XML_DSIG_RSA_KEY"
            };
            RSAKey = new(l_cspParams);            
        }

        void SignXml(XmlDocument _xmlDoc)
        {
            if (_xmlDoc == null)
            {
                throw new ArgumentException(null, nameof(_xmlDoc));
            }
            if (RSAKey == null)
            {
                throw new ArgumentException(null, nameof(RSAKey));

            }
            SignedXml signedXml = new (_xmlDoc)
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

        void SaveSignedXml(XmlDocument _xmlDoc)
        {             
            var l_name = _xmlDoc.BaseURI.Split('/').Last();
            _xmlDoc.Save(OutputDirectory + "//" + l_name);
        }
    }
}
