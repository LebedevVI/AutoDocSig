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
            {
                return inputDirectory;
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
        string signaturePassword;
        public String SignaturePassword
        {
            get
            {
                return signaturePassword;
            }
            set
            {
                signaturePassword = value;
                OnPropertyChanged();
            }
        }
        bool detached;
        public bool Detached
        {
            get
            {
                return detached;
            }
            set
            {
                detached = value;
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
        bool isWorked;
        public bool IsWorked
        {
            get
            {
                return isWorked;
            }
            set
            {
                isWorked = value;
                OnPropertyChanged();
            }
        }
        public RelayCommand SelectInputDirectoryButtonClick { get; set; }
        public RelayCommand SelectOutputDirectoryButtonClick { get; set; }
        public RelayCommand SelectSignatureButtonClick { get; set; }
        public RelayCommand DoWorkButtonClick { get; set; }
        public RelayCommand StopWorkButtonCLick { get; set; }

        public MainWindowVM()
        {
            SelectInputDirectoryButtonClick = new RelayCommand(o => SelectInputDirectory());
            SelectOutputDirectoryButtonClick = new RelayCommand(o => SelectOutputDirectory());
            SelectSignatureButtonClick = new RelayCommand(o => SelectSignature());
            DoWorkButtonClick = new RelayCommand(o => Work());
            StopWorkButtonCLick = new RelayCommand(o => StopWork());
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
        AutoDocSig.Model.Signature signature;
        async void Work()
        {
            IsWorked = true;
            IsReady = false;
            signature = new AutoDocSig.Model.Signature(SignaturePath, SignaturePassword, Detached);
            while (IsWorked)
            {
                await DoWorkAsync();
            }
        }
        Task DoWorkAsync()
        {
            return Task.Run(() => DoWork());
        }
        void DoWork()
        {
            var l_filePathList = Directory.GetFiles(InputDirectory, "*.xml");
            signature.SignFiles(l_filePathList, OutputDirectory);
        }

        void StopWork()
        {
            IsWorked = false;
            IsReady = true;
        }
    }
}
