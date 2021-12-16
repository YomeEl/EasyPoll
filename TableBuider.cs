using System.Collections.Generic;
using System.Linq;

using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

using EasyPoll.Models;

namespace EasyPoll
{
    public class TableBuider
    {
        private readonly List<string> questionColumns = new() { "Номер ответа", "Ответ", "Подразделение" };

        private readonly List<QuestionModel> questions;

        private readonly IWorkbook workbook;

        private ICellStyle borderFull;
        private ICellStyle borderSide;
        private ICellStyle borderNoTop;
        private ICellStyle questionTitleStyle;
        private ICellStyle questionStyle;

        private int rowIndex = 0;

        public TableBuider(List<QuestionModel> questions)
        {
            this.questions = questions;
            workbook = new XSSFWorkbook();
            CreateStyles(workbook);
        }

        public XSSFWorkbook Build()
        {
            AddParticipantsSheet();
            AddQuestionSheets();

            return workbook as XSSFWorkbook;
        }

        private void CreateStyles(IWorkbook workbook)
        {
            borderFull = workbook.CreateCellStyle();
            borderSide = workbook.CreateCellStyle();

            borderFull.BorderLeft = BorderStyle.Thin;
            borderFull.BorderRight = BorderStyle.Thin;
            borderSide.CloneStyleFrom(borderFull);
            borderFull.BorderTop = BorderStyle.Thin;
            borderFull.BorderBottom = BorderStyle.Thin;

            borderNoTop = workbook.CreateCellStyle();
            borderNoTop.CloneStyleFrom(borderFull);
            borderNoTop.BorderTop = BorderStyle.None;

            questionTitleStyle = workbook.CreateCellStyle();
            questionTitleStyle.CloneStyleFrom(borderFull);
            questionTitleStyle.Alignment = HorizontalAlignment.Center;

            questionStyle = workbook.CreateCellStyle();
            questionStyle.CloneStyleFrom(questionTitleStyle);
            questionStyle.WrapText = true;
        }

        private void AddQuestionSheets()
        {
            for (int i = 0; i < questions.Count; i++)
            {
                rowIndex = 0;

                var sheet = workbook.CreateSheet($"Вопрос {i + 1}");

                AddMegedRegion(sheet, "Текст вопроса", questionTitleStyle);
                AddMegedRegion(sheet, questions[i].Question, questionStyle);
                rowIndex++;
                AddTableHeader(sheet, questionColumns);

                var question = questions[i];
                var answers = ExtractAnswers(question);
                for (int j = 0; j < answers.Count; j++)
                {
                    var answer = answers[j];
                    var row = sheet.CreateRow(rowIndex + j + 1);
                    row.CreateCell(0).SetCellValue(answer.Answer);
                    row.CreateCell(1).SetCellValue(question.Options[answer.Answer - 1].Text);
                    row.CreateCell(2).SetCellValue(answer.User.Department.Name);
                    if (j == answers.Count - 1 || answers[j + 1].Answer != answer.Answer)
                    {
                        row.Cells.ForEach(c => c.CellStyle = borderNoTop);
                    }
                    else
                    {
                        row.Cells.ForEach(c => c.CellStyle = borderSide);
                    }
                }

                SetColumnsAutoSize(sheet);
            }
        }

        private void AddParticipantsSheet()
        {
            var sheet = workbook.CreateSheet("Участники");

            var answersCnt = new Dictionary<DepartmentModel, int>();
            questions.ForEach(q => q.Answers.ForEach(a => {
                int count = q.Answers.Count(ans => ans.User.Department.Id == a.User.Department.Id);
                answersCnt[a.User.Department] = count;
            }));

            rowIndex = 0;

            var rowSummary = sheet.CreateRow(rowIndex++);
            rowSummary.CreateCell(0).SetCellValue("Всего участников");
            var cellSummary = rowSummary.CreateCell(1);
            cellSummary.SetCellValue(answersCnt.Values.Sum());
            cellSummary.SetCellType(CellType.Numeric);
            rowSummary.Cells.ForEach(c => c.CellStyle = borderFull);

            rowIndex++;

            var rowInfo = sheet.CreateRow(rowIndex);
            rowInfo.CreateCell(0).SetCellValue("По подразделениям");
            rowInfo.CreateCell(1);
            rowInfo.Cells.ForEach(c => c.CellStyle = questionTitleStyle);
            sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 1));
            rowIndex++;

            foreach (var dept in answersCnt.Keys.OrderBy(d => d.Id))
            {
                var row = sheet.CreateRow(rowIndex++);
                row.CreateCell(0).SetCellValue(dept.Name);
                var cell = row.CreateCell(1);
                cell.SetCellValue(answersCnt[dept]);
                cell.SetCellType(CellType.Numeric);
                row.Cells.ForEach(c => c.CellStyle = borderFull);
            }

            sheet.AutoSizeColumn(0);
            sheet.AutoSizeColumn(0);
        }

        private void AddMegedRegion(ISheet sheet, string value, ICellStyle style)
        {
            var qRow = sheet.CreateRow(rowIndex);
            var qCell = qRow.CreateCell(0);
            qCell.SetCellValue(value);
            qRow.CreateCell(1); 
            qRow.CreateCell(2);
            qRow.Cells.ForEach(c => c.CellStyle = style);
            sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 2));

            rowIndex++;
        }

        private void AddTableHeader(ISheet sheet, List<string> columns)
        {
            var row = sheet.CreateRow(rowIndex);
            for (int i = 0; i < columns.Count; i++)
            {
                row.CreateCell(i).SetCellValue(columns[i]);
            }
            row.Cells.ForEach(c => c.CellStyle = borderFull);
        }

        private void SetColumnsAutoSize(ISheet sheet)
        {
            for (int i = 0; i < questionColumns.Count; i++)
            {
                sheet.AutoSizeColumn(i);
            }
        }

        private static List<AnswerModel> ExtractAnswers(QuestionModel question)
        {
            var answers = question.Answers
                .OrderBy(a => a.Answer)
                .ThenBy(a => a.User.DepartmentId)
                .ToList();
            return answers;
        }
    }
}
