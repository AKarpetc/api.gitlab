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
        static void ListToExcel(List<MRViewModel> mrs)
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
                    excelWorksheet.Cells[i, 12].Value = mr.assignee?.name;

                    i++;

                }

               

                FileInfo excelFile = new FileInfo(@"D:\test.xlsx");
                excel.SaveAs(excelFile);
            }

        }
        static void Main(string[] args)
        {
            var mrCollection = new List<MRViewModel>();


            for (int i = 1; i <= 36; i++)
            {
                WebRequest request = WebRequest.Create("http://git/api/v4/projects/18/merge_requests?state=merged&scope=all&page="+i);
                // If required by the server, set the credentials.
                request.Headers.Add("Private-Token:DT9gHYJxNWHc7df68rSU");
                // Get the response.
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                // Display the status.
                Console.WriteLine(response.StatusDescription);
                // Get the stream containing content returned by the server.
                Stream dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                string responseFromServer = reader.ReadToEnd();
                // Display the content.

                var collection = JsonConvert.DeserializeObject<List<MRViewModel>>(responseFromServer);

                mrCollection.AddRange(collection);

                Console.WriteLine(responseFromServer);


                // Cleanup the streams and the response.

                reader.Close();
                dataStream.Close();
                response.Close();
            }

            ListToExcel(mrCollection);

            Console.ReadLine();

        }
    }
}
