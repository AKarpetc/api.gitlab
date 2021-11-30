using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GITLab.AP.Adapter.Interfaces
{
    public interface IGitAPIFacade
    {
        /// <summary>
        /// Сервис работы с MR с гит лаб
        /// </summary>
        IMergeRequestService MergeRequest { get; set; }

        /// <summary>
        /// Сервис работы с Релизами в гит лаб
        /// </summary>
        IReleasesService Release { get; set; }
    }
}
