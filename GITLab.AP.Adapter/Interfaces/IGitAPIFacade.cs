using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GITLab.AP.Adapter.Interfaces
{
    public interface IGitAPIFacade
    {
        IMergeRequestService MergeRequest { get; set; }

        IReleasesService Release { get; set; }
    }
}
