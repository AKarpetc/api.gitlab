using GITLab.AP.Adapter.DTO;
using GITLab.AP.Adapter.Services;
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
        static MergeRequestService mergeRequestService;
        static ReleasesService releasesService;
        static Program()
        {
            var url = "http://git/api/v4/projects/18/";
            var token = "GGy1nCUCtzmaYfeZz8s_";

            mergeRequestService = new MergeRequestService(url, token);

            releasesService = new ReleasesService(url, token);
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

            GetFromReleaseToRelease();

            Console.ReadLine();
        }

        static void CreateRelease(List<MergeRequestGet> mrs, string dates, string version)
        {
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Worksheets.Add("Worksheet1");
                var excelWorksheet = excel.Workbook.Worksheets["Worksheet1"];

                int i = 1;

                excelWorksheet.Cells[i, 1].Value = "Релиз системы спринта за " + dates;

                i++;
                i++;
                foreach (var mr in mrs)
                {
                    excelWorksheet.Cells[i, 1].Value = (i - 2).ToString();
                    excelWorksheet.Cells[i, 2].Value = mr.title;
                    excelWorksheet.Cells[i, 3].Value = mr.description;
                    i++;
                }
                i++;
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

        static void GetFromReleaseToRelease()
        {
            var mrs = mergeRequestService.GetAll(DateTime.Now.AddDays(-18));

            var lastReleases = mrs.OrderByDescending(x => x.merged_at).LastOrDefault(x => x.Labels.Contains("Release"));

            var firstReleases = mrs.OrderByDescending(x => x.merged_at).FirstOrDefault(x => x.Labels.Contains("Release"));

            var mrsToRelease = mrs.Where(x => x.merged_at > lastReleases.merged_at && x.merged_at < firstReleases.merged_at);

            if (lastReleases == firstReleases)
            {
                Console.WriteLine("За последнии 10 дней найден только 1 релиз");
                return;
            }

            var releases = releasesService.GetAllReleases();

            var dates = lastReleases.merged_at.ToString("dd.MM.yy") + "-" + firstReleases.merged_at.ToString("dd.MM.yy");

            var lasrRelease = releases.OrderByDescending(x => x.created_at).FirstOrDefault();

            var version = lasrRelease.tag_name.Split('.')[0] + "." + (Convert.ToInt32(lasrRelease.tag_name.Split('.')[1]) + 1).ToString();

            CreateRelease(mrsToRelease.ToList(), dates, version);
        }
    }
}
