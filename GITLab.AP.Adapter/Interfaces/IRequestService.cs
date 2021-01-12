using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GITLab.AP.Adapter.Interfaces
{
    internal interface IRequestService
    {
        Task<IEnumerable<T>> GetListAsync<T>(string url);
    }
}
