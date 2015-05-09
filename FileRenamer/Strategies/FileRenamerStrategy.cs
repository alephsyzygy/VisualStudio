using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileRenamer.Strategies
{
    /// <summary>
    /// This interface encapsulates a strategy to rename a single file.
    /// </summary>
    public interface IFileRenamerStrategy
    {
        String RenameFile(FileMetaData FileName, int Position, NameSuffixHelper Helper);

        #region Events

        event EventHandler<EventArgs> StrategyChanged;

        #endregion
    }

    /// <summary>
    /// Just return the name unchanged.
    /// </summary>
    public class IdentityStrategy : IFileRenamerStrategy
    {

        string IFileRenamerStrategy.RenameFile(FileMetaData FileName, int Position, NameSuffixHelper Helper)
        {
            return FileName.Name;
        }

        #region Events

        public event EventHandler<EventArgs> StrategyChanged;

        #endregion
    }
}
