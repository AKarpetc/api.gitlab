﻿using GITLab.AP.Adapter;
using GITLab.AP.Adapter.DTO;
using GITLab.AP.Adapter.Interfaces;
using GITLab.AP.Adapter.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.ReleaseCreator
{
    class Program
    {
        private const string RELEASE_NAME = "Релиз системы спринта за ";
        private static IGitAPIFacade client;
        private const int MR_COUNT_DAY = 8;
        private static string buildId;

        static Program()
        {
            var gitUrl = "http://git.kazzinc.kz/";

            var projectUrl = "api/v4/projects/18/";

            var token = "ZBmCg_M-ib9EzY8j2ZHg";

            client = new GitAPIFacade(gitUrl, projectUrl, token);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Начала выполнение!");

            if (args.Count() > 0)
            {
                Console.WriteLine($"Build Id Team City:{args.FirstOrDefault()}");
                buildId = args.FirstOrDefault();
            }

            var task = CreateRelease();

            task.Wait();

            task.ContinueWith(res =>
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

            Console.WriteLine($"За последнии {MR_COUNT_DAY} дней найдено {mrs.Count()} запросов на слияние");

            var lastMRReleases = mrs.OrderByDescending(x => x.merged_at).LastOrDefault(x => x.Labels.Contains("Release"));

            var firstMRReleases = mrs.OrderByDescending(x => x.merged_at).FirstOrDefault(x => x.Labels.Contains("Release"));

            var mrsToRelease = mrs.Where(x => x.merged_at > lastMRReleases.merged_at && x.merged_at < firstMRReleases.merged_at);

            if (lastMRReleases == firstMRReleases)
            {
                var message = $"За последнии {MR_COUNT_DAY} дней найден только 1 релиз";
                Console.WriteLine(message);
                return;
            }

            var lastMr = mrs.OrderByDescending(x => x.created_at).FirstOrDefault();

            if (!lastMr.Labels.Contains("Release"))
            {
                Console.WriteLine("Хот фикс не инициирует создания релиза!");
                return;
            }

            var releases = await client.Release.GetAllReleases();

            Console.WriteLine($"В системе всего {releases.Count()} релизов"); ;

            var dates = lastMRReleases.merged_at.ToString("dd.MM.yy") + "-" + firstMRReleases.merged_at.ToString("dd.MM.yy");

            var lastRelease = releases.OrderByDescending(x => x.created_at).FirstOrDefault();

            Console.WriteLine($"Последний релиз {lastRelease.tag_name} ");

            var version = lastRelease.tag_name.Split('.')[0] + "." + (Convert.ToInt32(lastRelease.tag_name.Split('.')[1]) + 1).ToString();

            Console.WriteLine($"Новая версия: {version}, название релиза: {RELEASE_NAME + dates}");

            await ToGit(mrsToRelease.ToList(), dates, version);
        }

        private static async Task ToGit(List<MergeRequestGet> mrs, string dates, string version)
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
                Ref = "Release",

            };

            if (buildId != null)
            {
                newRelease.assets = new Assets
                {
                    links = (new List<Link>() { new Link
                    {
                         url=$"http://devops.kazzinc.kz/viewLog.html?buildId={buildId}&tab=buildResultsDiv&buildTypeId=Kazzinc_Build",
                         name="Build",
                         external=true,
                         link_type="other"

                    }}).ToArray()

                };
            }

            await client.Release.Create(newRelease);
        }
    }

}
