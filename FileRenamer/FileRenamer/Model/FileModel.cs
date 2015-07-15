using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileRenamer.Model
{
    /// <summary>
    /// A FileModel object represents a file in the process of being renamed.
    /// </summary>
    class FileModel
    {
        /// <summary>
        /// The original filename
        /// </summary>
        public string OriginalFileName { get; private set; }

        /// <summary>
        /// The new filename
        /// </summary>
        public string NewFileName { get; set; }

        /// <summary>
        /// Does the new filename clash with an existing file?
        /// This is to be set by the user of this object
        /// </summary>
        public bool Clashes { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="OriginalFileName">The original filename</param>
        public FileModel(string OriginalFileName)
        {
            this.OriginalFileName = OriginalFileName;
            this.NewFileName = OriginalFileName;
            this.Clashes = false;
        }
    }
}
