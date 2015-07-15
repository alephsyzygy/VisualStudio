using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileRenamer.Strategies;

namespace FileRenamer.ViewModel
{
    abstract class StrategyViewModel : ViewModelBase
    {
        public abstract IFileRenamerStrategy Strategy {get;}

    }
}
