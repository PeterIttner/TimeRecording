using NPOI.HSSF.UserModel;
using NPOI.OpenXmlFormats.Spreadsheet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeRecording.Model;
using TimeRecording.TimeCalculation;

namespace TimeRecording.IO.Reporting.Excel
{
    public class ExcelReport
    {
        #region Member

        private WorkingTimeCalculator mCalculator = new WorkingTimeCalculator();

        #endregion

        private Project mProject;

        public ExcelReport(Project project)
        {
            mProject = project;
        }

        public void ExportReport(string filename)
        {
            var workbook = CreateWorkbookFromProject(mProject);
            var tempfile = SaveWorkbookToTempFile(workbook);
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
            File.Move(tempfile, filename);
         }

        #region Private Helper

        private HSSFWorkbook CreateWorkbookFromProject(Project project)
        {
            var workTimes = mCalculator.GetWorkingTimesOfProjectPerDay(project);
            var book = new HSSFWorkbook();
            var sheet = book.CreateSheet(project.Name);

            var row = sheet.CreateRow(0);
            row.CreateCell(0).SetCellValue("Arbeitszeiten für Projekt: ");
            row.CreateCell(1).SetCellValue(project.Name);

            for (int workTimeIndex = 0; workTimeIndex < workTimes.Count; workTimeIndex++)
            {
                var workTime = workTimes.ElementAt(workTimeIndex);
                row = sheet.CreateRow(workTimeIndex + 1);
                row.CreateCell(0).SetCellValue(workTime.Date.ToString("dd.MM.yyyy"));
                row.CreateCell(1).SetCellValue(workTime.WorkingTime.ToString("hh\\:mm"));
                row.CreateCell(2).SetCellValue(workTime.Activities);
            }
            sheet.AutoSizeColumn(0);
            sheet.AutoSizeColumn(1);
            sheet.AutoSizeColumn(2);
            return book;
        }

        private string SaveWorkbookToTempFile(HSSFWorkbook workbook)
        {
            var tempFilename = Path.GetTempFileName();
            SaveWorkbook(workbook, tempFilename);
            return tempFilename;
        }

        private void SaveWorkbook(HSSFWorkbook workbook, string filename)
        {
            FileStream file = new FileStream(filename, FileMode.Create);
            workbook.Write(file);
            file.Close();
        }

        #endregion
    }
}
