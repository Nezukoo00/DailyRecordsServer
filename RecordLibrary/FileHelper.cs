using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace RecordLibrary
{
    public static class FileHelper
    {
        public static void SaveRecordToFile(string filePath, string content)
        {
            File.AppendAllText(filePath, content);
        }
    }
}