using GITLab.AP.Adapter.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GITLab.AP.Adapter.Interfaces
{
    public interface IReleasesService
    {
        Task<IEnumerable<Release>> GetAllReleases();

        Task<Release> Create(AddRelease model);


        Task<Release> Delete(string tag_name);
    }
}
