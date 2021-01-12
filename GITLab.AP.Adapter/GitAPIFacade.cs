using GITLab.AP.Adapter.Interfaces;
using GITLab.AP.Adapter.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GITLab.AP.Adapter
{
    public class GitAPIFacade : IGitAPIFacade
    {
        public GitAPIFacade(string gitUrl, string projectUrl, string token)
        {
            MergeRequest = new MergeRequestService(gitUrl + projectUrl, token);
            Release = new ReleasesService(gitUrl + projectUrl, token);
        }

        public IMergeRequestService MergeRequest { get; set; }

        public IReleasesService Release { get; set; }

    }
}
