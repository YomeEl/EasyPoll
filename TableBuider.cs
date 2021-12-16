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
        private readonly List<string> columns = new() { "Номер ответа", "Ответ", "Подразделение" };

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
            for (int i = 0; i < questions.Count; i++)
            {
                rowIndex = 0;

                var qSheet = workbook.CreateSheet($"Вопрос {i + 1}");

                AddMegedRegion(qSheet, "Текст вопроса", questionTitleStyle);
                AddMegedRegion(qSheet, questions[i].Question, questionStyle);
                rowIndex++;
                AddTableHeader(qSheet);

                var question = questions[i];
                var answers = ExtractAnswers(question);
                for (int j = 0; j < answers.Count; j++)
                {
                    var answer = answers[j];
                    var row = qSheet.CreateRow(rowIndex + j + 1);
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

                SetColumnsAutoSize(qSheet);
            }

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

        private void AddTableHeader(ISheet sheet)
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
            for (int i = 0; i < columns.Count; i++)
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
