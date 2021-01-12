using GITLab.AP.Adapter.DTO;
using GITLab.AP.Adapter.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GITLab.AP.Adapter.Services
{
    internal class ReleasesService : IReleasesService
    {
        private readonly string _url;
        private readonly string _privateToken;
        private readonly IRequestService _request;

        public ReleasesService(string url, string privateToken)
        {
            _url = url;
            _privateToken = privateToken;
            _request = new RequestService(_privateToken);
        }

        public async Task<IEnumerable<Release>> GetAllReleases() =>
               await _request.GetListAsync<Release>($"{_url}releases");


        public async Task<Release> Create(AddRelease model) =>
               await _request.PostAsync<AddRelease, Release>(model, $"{_url}releases");


        public async Task<Release> Delete(string tag_name) =>
              await _request.DeleteAsync<Release>($"{_url}releases/{tag_name}");
    }
}
