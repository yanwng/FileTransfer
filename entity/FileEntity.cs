using System;
using System.Collections.Generic;
using System.Text;

namespace FileTransfer.entity
{
    public class FileEntity
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public bool  IsSend { get; set; }
        public bool SendSucess { get; set; }
        public bool IsSucess { get; set; }

        public long FileSize { get; set; }

        public long SendSize { get; set; }
        public long ReceiveSize { get; set; }


        public FileEntity(string filePath, string fileName, bool isSend, bool isSucess)
        {
            FilePath = filePath;
            FileName = fileName;
            IsSend = isSend;
            IsSucess = isSucess;
        }

        public FileEntity(string filePath, string fileName, bool isSend, bool isSucess,long fileSize)
            :this(filePath, fileName, isSend, isSucess) 
        {
            FileSize = fileSize;
        }




    }
}
