using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDocSig.Model
{
    class Logger
    {
        string fileFullName;
        public Logger(string _path)
        {
            string pathToLog = Path.Combine(_path, "Log");
            if (!Directory.Exists(pathToLog))
            {
                Directory.CreateDirectory(pathToLog);
            }
            fileFullName = Path.Combine(pathToLog, string.Format("{0}_{1:dd.MM.yyy}.log"));
        }

        public void Write(string _log)
        {
            string l_fullText = string.Format("[{0:dd.MM.yyy HH:mm:ss.fff}] [{1}.{2}()] {3}\r\n", DateTime.Now, _log);
            File.AppendAllText(fileFullName, l_fullText);
        }

        public void Write(Exception _ex)
        {
            string l_fullText = string.Format("[{0:dd.MM.yyy HH:mm:ss.fff}] [{1}.{2}()] {3}\r\n", DateTime.Now, _ex.TargetSite.DeclaringType, _ex.TargetSite.Name, _ex.Message);
            File.AppendAllText(fileFullName, l_fullText);
        }
    }
}
