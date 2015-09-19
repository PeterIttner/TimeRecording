using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeRecording.Model;
using TimeRecording.TimeCalculation;

namespace TimeRecording.IO.Reporting.Text
{
    public class TextReport {
     #region Member

        private WorkingTimeCalculator mCalculator = new WorkingTimeCalculator();

        #endregion

        private Project mProject;

        public TextReport(Project project)
        {
            mProject = project;
        }

        public void ExportReport(string filename)
        {
            var document = CreateDocumentFromProject(mProject);
            SaveDocument(filename, document);
         }

        #region Private Helper

        private string CreateDocumentFromProject(Project project)
        {
            var workTimes = mCalculator.GetWorkingTimesOfProjectPerDay(project);
            StringBuilder sb = new StringBuilder();;

            sb.AppendLine("Arbeitszeiten für Projekt: " + project.Name);
            sb.AppendLine();

            var table = CreateTable(workTimes);
            var format = GetRowFormatString(table);

            var tableRow = table.ElementAt(0);
            var headline = string.Format(format, tableRow.Item1, tableRow.Item2, tableRow.Item3);
            
            sb.AppendLine(headline);
            sb.AppendLine(new String('=', headline.Length));

            for (int rowIndex = 1; rowIndex < table.Count; rowIndex++)
            {
                tableRow = table.ElementAt(rowIndex);
                sb.AppendLine(string.Format(format, tableRow.Item1, tableRow.Item2, tableRow.Item3));
            }

            return sb.ToString();
        }

        private static string GetRowFormatString(List<Tuple<string, string, string>> table)
        {
            var maxDateLen = table.Max(row => row.Item1.Length) + 2;
            var maxDurationLen = table.Max(row => row.Item2.Length) + 2;

            var format = "{0,-" + maxDateLen + "} {1,-" + maxDurationLen + "} {2}";
            return format;
        }

        private static List<Tuple<string, string, string>> CreateTable(List<ViewModel.WorkTime> workTimes)
        {
            var table = new List<Tuple<string, string, string>>();
            table.Add(new Tuple<string, string, string>("Datum", "Arbeitszeit", "Tätigkeit"));
            table.Add(new Tuple<string, string, string>("", "", ""));

            for (int workTimeIndex = 0; workTimeIndex < workTimes.Count; workTimeIndex++)
            {
                var workTime = workTimes.ElementAt(workTimeIndex);
                var date = workTime.Date.ToString("dd.MM.yyyy");
                var duration = workTime.WorkingTime.ToString("hh\\:mm");
                var activities = workTime.Activities;
                table.Add(new Tuple<string, string, string>(date, duration, activities));
            }
            return table;
        }

        private void SaveDocument(string filename, string document)
        {
            File.WriteAllText(filename, document);
        }

        #endregion
    }
}