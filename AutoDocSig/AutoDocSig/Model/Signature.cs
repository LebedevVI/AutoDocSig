using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Pkcs;
using LibCore;
using LibCore.Security.Cryptography.X509Certificates;

namespace AutoDocSig.Model
{
    class Signature
    {
        /// <summary>
        /// Логгер
        /// </summary>
        Logger logger = new Logger(AppDomain.CurrentDomain.BaseDirectory);
        /// <summary>
        /// Сертификат, используемый для подписи файлов
        /// </summary>
        X509Certificate2 certificate;
        /// <summary>
        /// Отдельная подпись - true; подпись в файле - false
        /// </summary>
        bool detached;
        /// <summary>
        /// Конструктор класса подписи
        /// </summary>
        /// <param name="_signaturePath">Путь к файлу с сертификатом подписи</param>
        /// <param name="_password">Пароль для сертификата</param>
        /// <param name="_detached">Отдельная подпись - true; подпись в файле - false</param>
        public Signature(string _signaturePath, string _password, bool _detached)
        {
            try
            {
                LibCore.Initializer.Initialize();
                detached = _detached;
                certificate = X509CertificateExtensions.Create(_signaturePath, _password, CpX509KeyStorageFlags.DefaultKeySet);
                logger.Write("Сертификат " + certificate.FriendlyName + " загружен");
            }
            catch (Exception e)
            {
                logger.Write(e);
            }
        }
        /// <summary>
        /// Подпись списка файлов с выводом в выходную директорию
        /// </summary>
        /// <param name="_filePathList">Список файлов из входной директории</param>
        /// <param name="_outputDirectory">Директория для вывода файлов</param>
        public void SignFiles(string [] _filePathList, string _outputDirectory)
        {
            try
            {
                foreach (var l_filePath in _filePathList)
                {
                    var l_fileAsByteArray = ReadFileAsByteArray(l_filePath);
                    logger.Write("Файл " + l_filePath + " загружен");
                    l_fileAsByteArray = SignFile(certificate, l_fileAsByteArray, detached);
                    logger.Write("Файл " + l_filePath + " подписан");
                    SaveFile(l_filePath, _outputDirectory, l_fileAsByteArray);
                    logger.Write("Файл " + l_filePath + " сохранен");
                }
            }
            catch (Exception e)
            {
                logger.Write(e);
            }
        }
        /// <summary>
        /// Считывание файла
        /// </summary>
        /// <param name="_path">Путь к файлу</param>
        /// <returns>Считанный файл в виде байтового массива</returns>
        public byte[] ReadFileAsByteArray(string _path)
        {
            byte[] l_content;
            l_content = File.ReadAllBytes(_path);
            return l_content;
        }
        /// <summary>
        /// Подпись файла
        /// </summary>
        /// <param name="certificate">Подписывающий сертификат безопасности</param>
        /// <param name="data">Подписываемый файл в виде массива байтов</param>
        /// <param name="detached">Отдельная подпись - true; подпись в файле - false</param>
        /// <returns>Подписанный файл</returns>
        public byte[] SignFile(X509Certificate2 _certificate, byte[] _signedFile, bool _detached)
        {
            var l_contentInfo = new ContentInfo(_signedFile);
            var l_signedCms = new SignedCms(l_contentInfo, detached);
            var l_cmsSigner = new CmsSigner(SubjectIdentifierType.IssuerAndSerialNumber, _certificate);
            l_signedCms.ComputeSignature(l_cmsSigner, true);
            return l_signedCms.Encode();
        }
        /// <summary>
        /// Сохранение файла
        /// </summary>
        /// <param name="_path">Путь к файлу</param>
        /// <param name="_outputDirectory">Выходная директория</param>
        /// <param name="_fileContent">Файл в виде массива байтов</param>
        public void SaveFile(string _path, string _outputDirectory, byte[] _fileContent)
        {
            var l_name = _path.Split('/').Last();
            File.WriteAllBytes(_outputDirectory + _path + ".sig", _fileContent);
        }
    }
}
