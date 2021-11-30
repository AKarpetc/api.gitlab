using GITLab.AP.Adapter.DTO;
using GITLab.AP.Adapter.DTO.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GITLab.AP.Adapter.Interfaces
{
    public interface IMergeRequestService
    {
        /// <summary>
        /// получение всех MRs за период времени с определенном статусе
        /// </summary>
        /// <param name="dateStart">Дата с</param>
        /// <param name="state">Дата по</param>
        /// <returns>Список MR</returns>
        Task<IEnumerable<MergeRequestGet>> GetAll(DateTime dateStart, MergeStates state = MergeStates.merged);
    }
}
