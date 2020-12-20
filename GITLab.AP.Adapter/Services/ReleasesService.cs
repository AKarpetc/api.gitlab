using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GITLab.AP.Adapter.Services
{
    public class ReleasesService
    {
        private readonly string _url;
        private readonly string _privateToken;

        public ReleasesService(string url, string privateToken)
        {
            _url = url;
            _privateToken = privateToken;
        }

    }
}
