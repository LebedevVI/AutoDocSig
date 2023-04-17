using System;
using System.Threading.Tasks;
using System.IO;
using AutoDocSig.Core;
using Microsoft.Win32;

namespace AutoDocSig
{
    class MainWindowVM : ObservableObject
    {
        /// <summary>
        /// Директория с файлами на подпись
        /// </summary>
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
        /// <summary>
        /// Директория для вывода подписанных файлов
        /// </summary>
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
        /// <summary>
        /// Расположение файла с сертификатом подписи
        /// </summary>
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
        /// <summary>
        /// Пароль для сертификата подписи
        /// </summary>
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
        /// <summary>
        /// Отдельная подпись - true; подпись в файле - false
        /// </summary>
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
        /// <summary>
        /// Все данные для запуска в работу программы готовы - true; чего-то не хватает - false
        /// </summary>
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
        /// <summary>
        /// Программа в работе (мониторит входную папку) - true; ожидает запуска - false
        /// </summary>
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
        /// <summary>
        /// Проверка заполненности всех параметров: входная директория, выходная директория, сертификат
        /// </summary>
        /// <returns>Все данные для запуска в работу программы готовы - true; чего-то не хватает - false</returns>
        bool CheckParams()
        {
            if (String.IsNullOrEmpty(InputDirectory) || String.IsNullOrEmpty(OutputDirectory) || String.IsNullOrEmpty(SignaturePath))
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// Пользователь выбрал входную директорию
        /// </summary>
        void SelectInputDirectory()
        {
            InputDirectory = WpfFolderDialog.Win32API.SelectDirectory();
            IsReady = CheckParams();
        }
        /// <summary>
        ///  Пользователь выбрал выходную директорию
        /// </summary>
        void SelectOutputDirectory()
        {
            OutputDirectory = WpfFolderDialog.Win32API.SelectDirectory();
            IsReady = CheckParams();
        }
        /// <summary>
        /// Пользователь выбрал файл с сертификатом подписи
        /// </summary>
        void SelectSignature()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                SignaturePath = dialog.FileName;
                IsReady = CheckParams();
            }
        }
        /// <summary>
        /// Используемая подпись
        /// </summary>
        AutoDocSig.Model.Signature signature;
        /// <summary>
        /// Пользователь нажал кнопку 'В работу'
        /// </summary>
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
        /// <summary>
        /// Асинхронный запуск просмотра файлов во входной директории и их подписи
        /// </summary>
        /// <returns></returns>
        Task DoWorkAsync()
        {
            return Task.Run(() => DoWork());
        }
        /// <summary>
        /// Просмотр файлов во входной директории и их подпись
        /// </summary>
        void DoWork()
        {
            var l_filePathList = Directory.GetFiles(InputDirectory, "*.xml");
            signature.SignFiles(l_filePathList, OutputDirectory);
        }
        /// <summary>
        /// Пользователь нажал кнопку 'Остановить'
        /// </summary>
        void StopWork()
        {
            IsWorked = false;
            IsReady = true;
        }
    }
}
