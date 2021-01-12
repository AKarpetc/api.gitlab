using GITLab.AP.Adapter.DTO;
using GITLab.AP.Adapter.DTO.Enums;
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
    internal class MergeRequestService : IMergeRequestService
    {
        private readonly string _url;
        private readonly string _privateToken;
        private readonly IRequestService _request;
        public MergeRequestService(string url, string privateToken)
        {
            _url = url;
            _privateToken = privateToken;
            _request = new RequestService(_privateToken);
        }

        public async Task<IEnumerable<MergeRequestGet>> GetAll(DateTime dateStart, MergeStates state = MergeStates.merged)
        {
            var mrCollection = new List<MergeRequestGet>();

            int i = 1;
            int count = 1;
            while (count > 0)
            {
                var collection = await _request.GetListAsync<MergeRequestGet>($"{_url}merge_requests?&sort=desc&state={state}&scope=all&page=" + i);

                count = collection.Count();
                mrCollection.AddRange(collection.Where(x => x.merged_at > dateStart));

                if (!collection.Any() || collection.Min(x => x.merged_at) < dateStart)
                {
                    break;
                }

                i++;
            }

            return mrCollection;
        }
    }
}
