using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Scallywags
{
    class Log
    {
        public enum LogErrorLevel { ERROR_NONE, ERROR_MINOR, ERROR_MAJOR, ERROR_CRITICAL, ERROR_WARNING, ERROR_INFO };
        private const string fileName = "ErrorLog.txt";

        /// <summary>
        /// Write to a log file.
        /// </summary>
        /// <param name="errorLevel">This is the error level of the log being written</param>
        /// <param name="message">This is the message to output.</param>
        public static void WriteToLog(LogErrorLevel errorLevel, string message)
        {

            StreamWriter errorFile = new StreamWriter(fileName);

            errorFile.Write(DateTime.Now.ToString() + "\t");

            switch(errorLevel){
                case LogErrorLevel.ERROR_NONE:
                    errorFile.Write("ERROR_NONE\t");
                    break;
                case LogErrorLevel.ERROR_MINOR:
                    errorFile.Write("ERROR_MINOR\t");
                    break;
                case LogErrorLevel.ERROR_MAJOR:
                    errorFile.Write("ERROR_MOJOR\t");
                    break;
                case LogErrorLevel.ERROR_CRITICAL:
                    errorFile.Write("ERROR_CRITICAL\t");
                    break;
                case LogErrorLevel.ERROR_WARNING:
                    errorFile.Write("ERROR_WARNING\t");
                    break;
                case LogErrorLevel.ERROR_INFO:
                    errorFile.Write("ERROR_INFO\t");
                    break;
            }
            errorFile.WriteLine(message);

            Console.WriteLine(message);
        }

    }
}
