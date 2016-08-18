using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ExcelLibrary;
using ExcelLibrary.SpreadSheet;
using OfficeOpenXml;

namespace SampleProcess
{
    class Program
    {
        static string path = "C:/Users/t-yaxie/Desktop/Fun/single_note_result/";
        static string[] filename = { "1.txt", "2.txt", "3.txt", "4.txt", "5.txt", "6.txt", "7.txt" };
        static string outfile = path + "t.xls";
        static void Main(string[] args)
        {
            var file = path + @"t.xlsx";
            if (File.Exists(file)) File.Delete(file);
            using (var excel = new ExcelPackage(new FileInfo(file)))
            {
                var ws = excel.Workbook.Worksheets.Add("Sheet1");
                ws.Cells[1, 1].Value = "freq1";
                ws.Cells[1, 2].Value = "freq2";
                ws.Cells[1, 3].Value = "freq3";
                ws.Cells[1, 4].Value = "note";
                int line = 2;
                for(int i = 0; i < 7; i++)
                {
                    FileStream sFile = new FileStream(path + filename[i], FileMode.Open);
                    
                }

                excel.Save();
            }
        }
    }
}
