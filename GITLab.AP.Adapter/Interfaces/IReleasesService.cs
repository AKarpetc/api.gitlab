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
        /// <summary>
        /// Получение всех релизов с гит лаб
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Release>> GetAllReleases();

        /// <summary>
        /// Создания релиза
        /// </summary>
        /// <param name="model">Модель релиза</param>
        /// <returns></returns>
        Task<Release> Create(AddRelease model);

        /// <summary>
        /// Удаление Релиза
        /// </summary>
        /// <param name="tag_name">Тэг релиза по удалению</param>
        /// <returns>Возвращает удаленный релиз</returns>
        Task<Release> Delete(string tag_name);
    }
}
