using System;
using System.IO;

namespace AutoDocSig.Model
{
    class Logger
    {
        /// <summary>
        /// Путь к фаулу лога
        /// </summary>
        string fileFullName;
        /// <summary>
        /// Конструктор логгера
        /// </summary>
        /// <param name="_path">Директория хранения лога</param>
        public Logger(string _path)
        {
            string pathToLog = Path.Combine(_path, "Log");
            if (!Directory.Exists(pathToLog))
            {
                Directory.CreateDirectory(pathToLog);
            }
            fileFullName = Path.Combine(pathToLog, string.Format("{0}_{1:dd.MM.yyy}.log", AppDomain.CurrentDomain.FriendlyName, DateTime.Now));
        }
        /// <summary>
        /// Запись обычного действия
        /// </summary>
        /// <param name="_log">Текст записи в лог</param>
        public void Write(string _log)
        {
            string l_fullText = string.Format("[{0:dd.MM.yyy HH:mm:ss.fff}] [{1}]\r\n", DateTime.Now, _log);
            File.AppendAllText(fileFullName, l_fullText);
        }
        /// <summary>
        /// Запись ошибки
        /// </summary>
        /// <param name="_ex">Ошибка</param>
        public void Write(Exception _ex)
        {
            string l_fullText = string.Format("[{0:dd.MM.yyy HH:mm:ss.fff}] [{1}.{2}()] {3}\r\n", DateTime.Now, _ex.TargetSite.DeclaringType, _ex.TargetSite.Name, _ex.Message);
            File.AppendAllText(fileFullName, l_fullText);
        }
    }
}
