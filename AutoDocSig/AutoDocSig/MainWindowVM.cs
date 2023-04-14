using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                IsReady = CheckParams();
            }
        }

        void Work()
        {
            var l_filePathList = Directory.GetFiles(InputDirectory, "*.xml");
            AutoDocSig.Model.Signature l_signature = new AutoDocSig.Model.Signature(SignaturePath);
            foreach (var l_filePath in l_filePathList)
            {
                XmlDocument xmlDoc = new ()
                {
                    PreserveWhitespace = true
                };
                xmlDoc.Load(l_filePath);
                l_signature.SignXml(xmlDoc);
                l_signature.SaveSignedXml(xmlDoc, OutputDirectory);
                xmlDoc.Save(l_filePath);
                File.Delete(l_filePath);
            }
        }
    }
}
