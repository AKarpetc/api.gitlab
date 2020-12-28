using GITLab.AP.Adapter.DTO;
using GITLab.AP.Adapter.Services;
using GitLabApiClient;
using GitLabApiClient.Internal.Paths;
using GitLabApiClient.Models.Releases.Requests;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GetMRFromGitLab
{
    class Program
    {
        private const string RELEASE_NAME = "Релиз системы спринта за ";
        static MergeRequestService mergeRequestService;
        static ReleasesService releasesService;
        static GitLabClient client;
        static Program()
        {
            var url = "http://git.kazzinc.kz/api/v4/projects/18/";
            var token = "ZBmCg_M-ib9EzY8j2ZHg";

            mergeRequestService = new MergeRequestService(url, token);

            releasesService = new ReleasesService(url, token);

            client = new GitLabClient("http://git.kazzinc.kz", token);
        }

        static void ListToExcel(List<MergeRequestGet> mrs)
        {
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Worksheets.Add("Worksheet1");
                var excelWorksheet = excel.Workbook.Worksheets["Worksheet1"];

                int i = 1;

                excelWorksheet.Cells[i, 1].Value = "Id MR";
                excelWorksheet.Cells[i, 2].Value = "Отображаемый Id";
                excelWorksheet.Cells[i, 3].Value = "Id проекта";
                excelWorksheet.Cells[i, 4].Value = "Название";
                excelWorksheet.Cells[i, 5].Value = "Описание";
                excelWorksheet.Cells[i, 6].Value = "C Ветки";
                excelWorksheet.Cells[i, 7].Value = "В Ветку";
                excelWorksheet.Cells[i, 8].Value = "Создано";
                excelWorksheet.Cells[i, 9].Value = "Обновлено";
                excelWorksheet.Cells[i, 10].Value = "Ссылка";
                excelWorksheet.Cells[i, 11].Value = "Создал";
                excelWorksheet.Cells[i, 12].Value = "Принял";
                excelWorksheet.Cells[i, 13].Value = "Метки";

                i++;
                foreach (var mr in mrs)
                {

                    excelWorksheet.Cells[i, 1].Value = mr.id;
                    excelWorksheet.Cells[i, 2].Value = mr.iid;
                    excelWorksheet.Cells[i, 3].Value = mr.project_id;
                    excelWorksheet.Cells[i, 4].Value = mr.title;
                    excelWorksheet.Cells[i, 5].Value = mr.description;
                    excelWorksheet.Cells[i, 6].Value = mr.source_branch;
                    excelWorksheet.Cells[i, 7].Value = mr.target_branch;
                    excelWorksheet.Cells[i, 8].Value = mr.created_at.ToShortDateString();
                    excelWorksheet.Cells[i, 9].Value = mr.updated_at.ToShortDateString();
                    excelWorksheet.Cells[i, 10].Value = mr.web_url;
                    excelWorksheet.Cells[i, 11].Value = mr.author?.name;
                    excelWorksheet.Cells[i, 12].Value = (mr.assignee?.name) ?? mr.author?.name;
                    excelWorksheet.Cells[i, 13].Value = string.Join(", ", mr.Labels);
                    i++;

                }

                DriveInfo myDrive = DriveInfo.GetDrives().FirstOrDefault(x => x.DriveType == DriveType.Fixed);

                var fileName = $@"{myDrive.Name}Temp\ImportFromGit-{DateTime.Now.ToShortDateString()}.xlsx";

                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                };

                FileInfo excelFile = new FileInfo(fileName);



                excel.SaveAs(excelFile);

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Файл сохранен по пути " + fileName);
            }

        }
        static void Main(string[] args)
        {
            var mrs = mergeRequestService.GetAll(DateTime.Now.AddDays(10));

            //ListToExcel(mrs.ToList());

            CreateRelease();

            //releasesService.Delete("v1.10005");


            Console.ReadLine();
        }

        static void CreateRelease(List<MergeRequestGet> mrs, string dates, string version)
        {
            ToExcell(mrs, dates, version);

            ToGit(mrs, dates, version);



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
                tag_name = "v1.10005",
                description = descriptoin,
                name = RELEASE_NAME + dates,
                assets = new Assets(),
                Ref = "Release"
            };

            releasesService.Create(newRelease);
        }

        private static void ToExcell(List<MergeRequestGet> mrs, string dates, string version)
        {
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Worksheets.Add("Worksheet1");
                var excelWorksheet = excel.Workbook.Worksheets["Worksheet1"];

                int i = 1;

                excelWorksheet.Cells[i, 1].Value = RELEASE_NAME + dates;

                i = i + 2;
                int number = 1;
                foreach (var mr in mrs)
                {
                    excelWorksheet.Cells[i, 1].Value = (number).ToString();
                    excelWorksheet.Cells[i, 2].Value = mr.title;
                    excelWorksheet.Cells[i, 3].Value = mr.description;
                    i = i + 2;
                    number++;
                }
                i = i + 2;
                excelWorksheet.Cells[i, 1].Value = version;

                DriveInfo myDrive = DriveInfo.GetDrives().FirstOrDefault(x => x.DriveType == DriveType.Fixed);

                var fileName = $@"{myDrive.Name}Temp\Releases-{DateTime.Now.ToShortDateString()}.xlsx";

                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                };

                FileInfo excelFile = new FileInfo(fileName);

                excel.SaveAs(excelFile);

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Файл сохранен по пути " + fileName);
            }
        }

        static void CreateRelease()
        {
            var mrs = mergeRequestService.GetAll(DateTime.Now.AddDays(-12)).Where(x => x.target_branch == "Develop" || x.target_branch == "Release");

            var lastMRReleases = mrs.OrderByDescending(x => x.merged_at).LastOrDefault(x => x.Labels.Contains("Release"));

            var firstMRReleases = mrs.OrderByDescending(x => x.merged_at).FirstOrDefault(x => x.Labels.Contains("Release"));

            var mrsToRelease = mrs.Where(x => x.merged_at > lastMRReleases.merged_at && x.merged_at < firstMRReleases.merged_at);

            if (lastMRReleases == firstMRReleases)
            {
                Console.WriteLine("За последнии 10 дней найден только 1 релиз");
                return;
            }

            var releases = releasesService.GetAllReleases();

            var dates = lastMRReleases.merged_at.ToString("dd.MM.yy") + "-" + firstMRReleases.merged_at.ToString("dd.MM.yy");

            var lastRelease = releases.OrderByDescending(x => x.created_at).FirstOrDefault();

            var version = lastRelease.tag_name.Split('.')[0] + "." + (Convert.ToInt32(lastRelease.tag_name.Split('.')[1]) + 1).ToString();

            CreateRelease(mrsToRelease.ToList(), dates, version);

        }
    }
}
