using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileRenamer.Model
{
    class FileModel
    {
        public string OriginalFileName { get; private set; }
        public string NewFileName { get; set; }
        public bool Clashes { get; set; }

        public FileModel(string OriginalFileName)
        {
            this.OriginalFileName = OriginalFileName;
            this.NewFileName = OriginalFileName;
            this.Clashes = false;
        }
    }
}
