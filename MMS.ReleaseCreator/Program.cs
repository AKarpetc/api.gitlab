using GITLab.AP.Adapter;
using GITLab.AP.Adapter.DTO;
using GITLab.AP.Adapter.Interfaces;
using GITLab.AP.Adapter.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.ReleaseCreator
{
    class Program
    {
        private const string RELEASE_NAME = "Релиз системы спринта за ";
        private static IGitAPIFacade client;
        private const int MR_COUNT_DAY = 12;

        static Program()
        {
            var gitUrl = "http://git.kazzinc.kz/";

            var projectUrl = "api/v4/projects/18/";

            var token = "ZBmCg_M-ib9EzY8j2ZHg";

            client = new GitAPIFacade(gitUrl, projectUrl, token);
        }

        static void Main(string[] args)
        {
            CreateRelease().ContinueWith(res =>
            {
                if (res.IsFaulted)
                {
                    throw res.Exception;
                }

                Console.WriteLine("Выполнено успешно");
            });

        }

        static async Task CreateRelease()
        {
            var mrs = (await client.MergeRequest.GetAll(DateTime.Now.AddDays(MR_COUNT_DAY * -1)))
                      .Where(x => x.target_branch == "Develop" || x.target_branch == "Release");

            var lastMRReleases = mrs.OrderByDescending(x => x.merged_at).LastOrDefault(x => x.Labels.Contains("Release"));

            var firstMRReleases = mrs.OrderByDescending(x => x.merged_at).FirstOrDefault(x => x.Labels.Contains("Release"));

            var mrsToRelease = mrs.Where(x => x.merged_at > lastMRReleases.merged_at && x.merged_at < firstMRReleases.merged_at);

            if (lastMRReleases == firstMRReleases)
            {
                Console.WriteLine("За последнии {0} дней найден только 1 релиз", MR_COUNT_DAY);
                return;
            }

            var releases = await client.Release.GetAllReleases();

            var dates = lastMRReleases.merged_at.ToString("dd.MM.yy") + "-" + firstMRReleases.merged_at.ToString("dd.MM.yy");

            var lastRelease = releases.OrderByDescending(x => x.created_at).FirstOrDefault();

            var version = lastRelease.tag_name.Split('.')[0] + "." + (Convert.ToInt32(lastRelease.tag_name.Split('.')[1]) + 1).ToString();

            ToGit(mrsToRelease.ToList(), dates, version);
        }

        private static void ToGit(List<MergeRequestGet> mrs, string dates, string version)
        {
            var descriptoin = "";

            int number = 1;
            foreach (var mr in mrs)
            {
                descriptoin += $"{(number)}. {mr.title} {mr.description}\n\n";
                number++;
            }

            var newRelease = new AddRelease()
            {
                tag_name = version,
                description = descriptoin,
                name = RELEASE_NAME + dates,
                assets = new Assets(),
                Ref = "Release"
            };

            client.Release.Create(newRelease);
        }
    }

}
