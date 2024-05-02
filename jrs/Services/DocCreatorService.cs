using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using HtmlToOpenXml;
using jrs.Models;
using Newtonsoft.Json.Linq;

namespace jrs.Services {
    public interface IDocCreatorService {
        MemoryStream CreateDocxFromHtml (string htmlContent);
    }
    public class DocCreatorService : IDocCreatorService {
        public class ExcelCell {
            public string columnName { get; set; }
            public dynamic cellValue { get; set; }
            public string cellType { get; set; }
        }
        public class ExcelRow {
            public ExcelCell[] cells { get; set; }
        }

        ///<summary>
        /// Creates an OpenXml Word document from the provided HTML string.
        ///</summary>
        ///<returns>MemoryStream representing the generate OpenXml Word document.</returns>
        /// <param name="htmlContent">String of HTML content to be used in the geenrated document.</param>
        public MemoryStream CreateDocxFromHtml (string htmlContent) {
            MemoryStream generatedDocument = new MemoryStream ();
            using (WordprocessingDocument package = WordprocessingDocument.Create (generatedDocument, WordprocessingDocumentType.Document)) {
                MainDocumentPart mainPart = package.MainDocumentPart;
                if (mainPart == null) {
                    mainPart = package.AddMainDocumentPart ();
                    new Document (new Body ()).Save (mainPart);
                }

                HtmlConverter converter = new HtmlConverter (mainPart);
                converter.ParseHtml (htmlContent);
                mainPart.Document.Save ();
            }
            return generatedDocument;
        }

        public MemoryStream CreateExcel (string[] colNames, string jsonDataRows = null) {
            MemoryStream generatedDocument = new MemoryStream ();

            using (SpreadsheetDocument document = SpreadsheetDocument.Create (generatedDocument, SpreadsheetDocumentType.Workbook)) {
                WorkbookPart workbookPart = document.AddWorkbookPart ();
                workbookPart.Workbook = new Workbook ();

                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart> ();
                var sheetData = new SheetData ();
                worksheetPart.Worksheet = new Worksheet (sheetData);

                Sheets sheets = workbookPart.Workbook.AppendChild (new Sheets ());
                Sheet sheet = new Sheet () { Id = workbookPart.GetIdOfPart (worksheetPart), SheetId = 1, Name = "Sheet1" };

                sheets.Append (sheet);

                Row headerRow = new Row ();

                foreach (string colName in colNames) {
                    Cell cell = new Cell ();
                    cell.DataType = CellValues.String; // TODO: Setup cell type calc.
                    cell.CellValue = new CellValue (colName);
                    headerRow.AppendChild (cell);
                }

                sheetData.AppendChild (headerRow);

                // Add data rows to document
                if (!String.IsNullOrEmpty (jsonDataRows)) {
                    using (JsonDocument jd = JsonDocument.Parse (jsonDataRows)) {
                        JsonElement root = jd.RootElement;
                        foreach (JsonElement tableRow in root.EnumerateArray ()) {
                            Row newRow = new Row ();
                            foreach (string colName in colNames) {
                                Cell cell = new Cell ();
                                cell.DataType = CellValues.String;
                                if (tableRow.TryGetProperty (colName, out JsonElement cejsonCellValue)) {
                                    cell.CellValue = new CellValue (cejsonCellValue.GetString ());
                                } else {
                                    cell.CellValue = new CellValue ("NULL");
                                }
                                newRow.AppendChild (cell);
                            }

                            sheetData.AppendChild (newRow);
                        }
                    }
                }
                workbookPart.Workbook.Save ();
            }
            return generatedDocument;
        }

        public MemoryStream CreateExcelForOverviewOutputIndicators (GenericExcelColumnLabel[] cols, OutpInd[] rows) {
            MemoryStream generatedDocument = new MemoryStream ();

            using (SpreadsheetDocument document = SpreadsheetDocument.Create (generatedDocument, SpreadsheetDocumentType.Workbook)) {
                WorkbookPart workbookPart = document.AddWorkbookPart ();
                workbookPart.Workbook = new Workbook ();

                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart> ();
                var sheetData = new SheetData ();
                worksheetPart.Worksheet = new Worksheet (sheetData);

                Sheets sheets = workbookPart.Workbook.AppendChild (new Sheets ());
                Sheet sheet = new Sheet () { Id = workbookPart.GetIdOfPart (worksheetPart), SheetId = 1, Name = "Sheet1" };

                sheets.Append (sheet);

                Row headerRow = new Row ();

                foreach (GenericExcelColumnLabel col in cols) {
                    string colName = col.columnLabel;
                    Cell cell = new Cell ();
                    cell.DataType = CellValues.String; // TODO: Setup cell type calc.
                    cell.CellValue = new CellValue (filterUndefined (colName));
                    headerRow.AppendChild (cell);
                }

                sheetData.AppendChild (headerRow);

                // Add data rows to document
                if (rows != null && rows.Length > 0) {
                    foreach (OutpInd outpInd in rows) {

                        if (outpInd.subCatOutcome == null || outpInd.subCatOutcome.Length == 0) {
                            Row newRow = new Row ();
                            Cell cell = new Cell ();
                            cell.DataType = CellValues.String;
                            cell.CellValue = new CellValue (filterUndefined (outpInd.OUTCOME));
                            newRow.AppendChild (cell);
                            sheetData.AppendChild (newRow);
                        } else {

                            foreach (subCatOutcome _subCatOutcome in outpInd.subCatOutcome) {
                                if (_subCatOutcome.subCatPmsCoiCode == null || _subCatOutcome.subCatPmsCoiCode.Length == 0) {
                                    Row newRow = new Row ();
                                    Cell cell = new Cell ();
                                    cell.DataType = CellValues.String;
                                    cell.CellValue = new CellValue (filterUndefined (outpInd.OUTCOME));
                                    newRow.AppendChild (cell);

                                    Cell cell1 = new Cell ();
                                    cell1.DataType = CellValues.String;
                                    cell1.CellValue = new CellValue (filterUndefined (_subCatOutcome.PMS_COI_CODE));
                                    newRow.AppendChild (cell1);
                                    sheetData.AppendChild (newRow);
                                } else {

                                    foreach (subCatPmsCoiCode _subCatPmsCoiCode in _subCatOutcome.subCatPmsCoiCode) {

                                        if (_subCatPmsCoiCode.subCatPmsTosDescription == null || _subCatPmsCoiCode.subCatPmsTosDescription.Length == 0) {
                                            Row newRow = new Row ();
                                            Cell cell = new Cell ();
                                            cell.DataType = CellValues.String;
                                            cell.CellValue = new CellValue (filterUndefined (outpInd.OUTCOME));
                                            newRow.AppendChild (cell);
                                            Cell cell1 = new Cell ();
                                            cell1.DataType = CellValues.String;
                                            cell1.CellValue = new CellValue (filterUndefined (_subCatOutcome.PMS_COI_CODE));
                                            newRow.AppendChild (cell1);

                                            Cell cell2 = new Cell ();
                                            cell2.DataType = CellValues.String;
                                            cell2.CellValue = new CellValue (filterUndefined (_subCatPmsCoiCode.PMS_TOS_DESCRIPTION));
                                            newRow.AppendChild (cell2);

                                            sheetData.AppendChild (newRow);
                                        } else {
                                            foreach (subCatPmsTosDescription _subCatPmsTosDescription in _subCatPmsCoiCode.subCatPmsTosDescription) {

                                                if (_subCatPmsTosDescription.subCatPmsProcess == null || _subCatPmsTosDescription.subCatPmsProcess.Length == 0) {

                                                    Row newRow = new Row ();
                                                    Cell cell = new Cell ();
                                                    cell.DataType = CellValues.String;
                                                    cell.CellValue = new CellValue (filterUndefined (outpInd.OUTCOME));
                                                    newRow.AppendChild (cell);
                                                    Cell cell1 = new Cell ();
                                                    cell1.DataType = CellValues.String;
                                                    cell1.CellValue = new CellValue (filterUndefined (_subCatOutcome.PMS_COI_CODE));
                                                    newRow.AppendChild (cell1);

                                                    Cell cell2 = new Cell ();
                                                    cell2.DataType = CellValues.String;
                                                    cell2.CellValue = new CellValue (filterUndefined (_subCatPmsCoiCode.PMS_TOS_DESCRIPTION));
                                                    newRow.AppendChild (cell2);
                                                    Cell cell3 = new Cell ();
                                                    cell3.DataType = CellValues.String;
                                                    cell3.CellValue = new CellValue (filterUndefined (_subCatPmsTosDescription.PMS_PROCESS));
                                                    newRow.AppendChild (cell3);
                                                    // Cell cell4 = new Cell ();
                                                    // cell4.DataType = CellValues.String;
                                                    // cell4.CellValue = new CellValue (filterUndefined(_subCatPmsProcess.PMS_OUTPUT);
                                                    // newRow.AppendChild (cell4);
                                                    sheetData.AppendChild (newRow);

                                                } else {
                                                    foreach (subCatPmsProcess _subCatPmsProcess in _subCatPmsTosDescription.subCatPmsProcess) {
                                                        if (_subCatPmsProcess.subCatPmsOutput == null || _subCatPmsProcess.subCatPmsOutput.Length == 0) {

                                                            Row newRow = new Row ();
                                                            Cell cell = new Cell ();
                                                            cell.DataType = CellValues.String;
                                                            cell.CellValue = new CellValue (filterUndefined (outpInd.OUTCOME));
                                                            newRow.AppendChild (cell);
                                                            Cell cell1 = new Cell ();
                                                            cell1.DataType = CellValues.String;
                                                            cell1.CellValue = new CellValue (filterUndefined (_subCatOutcome.PMS_COI_CODE));
                                                            newRow.AppendChild (cell1);

                                                            Cell cell2 = new Cell ();
                                                            cell2.DataType = CellValues.String;
                                                            cell2.CellValue = new CellValue (filterUndefined (_subCatPmsCoiCode.PMS_TOS_DESCRIPTION));
                                                            newRow.AppendChild (cell2);
                                                            Cell cell3 = new Cell ();
                                                            cell3.DataType = CellValues.String;
                                                            cell3.CellValue = new CellValue (filterUndefined (_subCatPmsTosDescription.PMS_PROCESS));
                                                            newRow.AppendChild (cell3);
                                                            Cell cell4 = new Cell ();
                                                            cell4.DataType = CellValues.String;
                                                            cell4.CellValue = new CellValue (filterUndefined (_subCatPmsProcess.PMS_OUTPUT));
                                                            newRow.AppendChild (cell4);
                                                            sheetData.AppendChild (newRow);

                                                        } else {

                                                            foreach (subCatPmsOutput _subCatPmsOutput in _subCatPmsProcess.subCatPmsOutput) {
                                                                Row newRow = new Row ();
                                                                Cell cell = new Cell ();
                                                                cell.DataType = CellValues.String;
                                                                cell.CellValue = new CellValue (filterUndefined (outpInd.OUTCOME));
                                                                newRow.AppendChild (cell);

                                                                Cell cell1 = new Cell ();
                                                                cell1.DataType = CellValues.String;
                                                                cell1.CellValue = new CellValue (filterUndefined (_subCatOutcome.PMS_COI_CODE));
                                                                newRow.AppendChild (cell1);

                                                                Cell cell2 = new Cell ();
                                                                cell2.DataType = CellValues.String;
                                                                cell2.CellValue = new CellValue (filterUndefined (_subCatPmsCoiCode.PMS_TOS_DESCRIPTION));
                                                                newRow.AppendChild (cell2);
                                                                Cell cell3 = new Cell ();
                                                                cell3.DataType = CellValues.String;
                                                                cell3.CellValue = new CellValue (filterUndefined (_subCatPmsTosDescription.PMS_PROCESS));
                                                                newRow.AppendChild (cell3);
                                                                Cell cell4 = new Cell ();
                                                                cell4.DataType = CellValues.String;
                                                                cell4.CellValue = new CellValue (filterUndefined (_subCatPmsProcess.PMS_OUTPUT));
                                                                newRow.AppendChild (cell4);
                                                                Cell cell5 = new Cell ();
                                                                cell5.DataType = CellValues.String;
                                                                cell5.CellValue = new CellValue (filterUndefined (filterHTMLTags (_subCatPmsOutput.INDICATOR)));
                                                                newRow.AppendChild (cell5);
                                                                Cell cell6 = new Cell ();
                                                                cell6.DataType = CellValues.String;
                                                                cell6.CellValue = new CellValue (filterUndefined (_subCatPmsOutput.TECHNIQUE));
                                                                newRow.AppendChild (cell6);
                                                                sheetData.AppendChild (newRow);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // using (JsonDocument jd = JsonDocument.Parse(jsonDataRows))
                    // {
                    //     JsonElement root = jd.RootElement;
                    //     foreach (JsonElement tableRow in root.EnumerateArray())
                    //     {
                    //         Row newRow = new Row();
                    //         foreach (string colName in colNames)
                    //         {
                    //             Cell cell = new Cell();
                    //             cell.DataType = CellValues.String;
                    //             if (tableRow.TryGetProperty(colName, out JsonElement cejsonCellValue))
                    //             {
                    //                 cell.CellValue = new CellValue(cejsonCellValue.GetString());
                    //             }
                    //             else
                    //             {
                    //                 cell.CellValue = new CellValue("NULL");
                    //             }
                    //             newRow.AppendChild(cell);
                    //         }

                    //         sheetData.AppendChild(newRow);
                    //     }
                    // }
                }
                workbookPart.Workbook.Save ();
            }
            return generatedDocument;
        }
        public string filterHTMLTags (string inputHTML) {
            if (inputHTML == null) return "";
            inputHTML = inputHTML.Replace ("<b>", "").Replace ("</b>", "").Replace ("<br>", "\r\n");
            return inputHTML;
        }

        public string filterUndefined (string inputHTML) {
            if (inputHTML == null) return "";
            inputHTML = inputHTML.Replace ("undefined", "");
            return inputHTML;
        }

        /// <summary>Generate Excell file based on a JRsTable configuration.</summary>
        /// <param  name="jsonTableInfo">
        /// Json string strguctured as JrsTable condifgurations. I.e: {
        ///     "columns": [{"column_name":*String*, js_type":*String*}],
        ///     "table_data": [{*Object*}]
        ///     "item_count": *Number*
        /// }
        /// </param>
        /// <param  name="columnLabels">Password</param>
        /// <returns>MemoryStream representation of the created file</returns>
        public MemoryStream CreateExcel (string jsonTableInfo, GenericExcelColumnLabel[] columnLabels = null) {
            MemoryStream generatedDocument = new MemoryStream ();
            try {
                using (SpreadsheetDocument document = SpreadsheetDocument.Create (generatedDocument, SpreadsheetDocumentType.Workbook)) {
                    WorkbookPart workbookPart = document.AddWorkbookPart ();
                    workbookPart.Workbook = new Workbook ();

                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart> ();
                    var sheetData = new SheetData ();
                    worksheetPart.Worksheet = new Worksheet (sheetData);

                    Sheets sheets = workbookPart.Workbook.AppendChild (new Sheets ());
                    Sheet sheet = new Sheet () { Id = workbookPart.GetIdOfPart (worksheetPart), SheetId = 1, Name = "Sheet1" };

                    sheets.Append (sheet);

                    using (JsonDocument jd = JsonDocument.Parse (jsonTableInfo)) {
                        JsonElement root = jd.RootElement;

                        // Get column list
                        JsonElement[] columns = root.GetProperty ("columns").EnumerateArray ().ToArray ();

                        // Get data rows
                        int countRows = root.TryGetProperty ("item_count", out JsonElement itemCount) ? itemCount.GetInt32 () : 0;
                        JsonElement[] tableRows = new JsonElement[countRows];
                        tableRows = root.TryGetProperty ("table_data", out JsonElement tableData) ? tableData.EnumerateArray ().ToArray () : tableRows;

                        // Create label dictionary
                        Dictionary<string, string> labelDict = new Dictionary<string, string> ();
                        if (columnLabels != null) {
                            foreach (GenericExcelColumnLabel labelItem in columnLabels) {
                                labelDict.Add (labelItem.columnName, labelItem.columnLabel);
                            }
                        }

                        // Create Header Row
                        Row headerRow = new Row ();
                        foreach (JsonElement col in columns) {
                            string colName = col.GetProperty ("column_name").GetString ();
                            Cell cell = new Cell ();
                            try {
                                cell.CellValue = new CellValue (labelDict[colName]);
                            } catch (KeyNotFoundException) {
                                cell.CellValue = new CellValue (colName);
                            }
                            cell.DataType = CellValues.String;
                            headerRow.AppendChild (cell);
                        }

                        sheetData.AppendChild (headerRow);

                        // Create Data rows
                        foreach (JsonElement tableRow in tableRows) {
                            Row newRow = new Row ();
                            foreach (JsonElement col in columns) {
                                string colName = col.GetProperty ("column_name").GetString ();
                                string colType = col.GetProperty ("js_type").GetString ().ToLower ();
                                string type_field = col.GetProperty ("type").GetString ().ToLower ();

                                Cell cell = new Cell ();
                                JsonElement cellContent = tableRow.TryGetProperty (colName, out JsonElement cellCont) ? cellCont : new JsonElement ();

                                // string tmpCellValue;
                                double tmpDouble;
                                DateTime tmpDate;

                                // Assign default  cell values
                                cell.DataType = CellValues.Error;
                                cell.CellValue = new CellValue ();

                                // Manage missing data
                                if (!string.Equals (cellContent.ToString (), "")) {
                                    switch (colType) {
                                        case "string":
                                            cell.DataType = CellValues.String;
                                            string cellValuString = cellContent.ToString ();
                                            // Strip the text of any html tags
                                            cellValuString = Regex.Replace (cellValuString, @"<[^>]+>|&nbsp;", "").Trim ();
                                            cell.CellValue = new CellValue (cellValuString);
                                            break;
                                        case "numeric":
                                            if (cellContent.TryGetDouble (out tmpDouble)) {
                                                cell.DataType = CellValues.Number;
                                                cell.CellValue = new CellValue (tmpDouble);
                                            }
                                            break;
                                        case "date":
                                            if (cellContent.TryGetDateTime (out tmpDate)) {
                                                cell.DataType = CellValues.String;
                                                cell.CellValue = new CellValue (cellContent.GetDateTime ().ToString ());
                                            }
                                            break;
                                        case "boolean":
                                            cell.DataType = CellValues.String;
                                            cell.CellValue = new CellValue (cellContent.GetBoolean ().ToString ());
                                            break;
                                        default:
                                            break;
                                    }
                                    if(type_field == "search_table_multi"){
                                        List<string> Lista = new List<string>();
                                        foreach (JsonElement item in cellContent.EnumerateArray ().ToArray()) {
                                            string itemField = col.GetProperty("itemText").GetString().ToString();
                                            string item_str = item.ToString();
                                            JObject fieldvalue = JObject.Parse(item_str);
                                            string itemText =  fieldvalue[itemField] != null ? fieldvalue[itemField].ToString(): "";
                                            Lista.Add(itemText);
                                        }                                         
                                        cell.CellValue = new CellValue(String.Join(',', Lista));
                                    }
                                }
                                newRow.AppendChild (cell);
                            }
                            sheetData.AppendChild (newRow);
                        }
                    }
                    workbookPart.Workbook.Save ();
                }
            } catch (Exception e) {
                throw e;
            }
            return generatedDocument;
        }

        public MemoryStream CreateExcelOvwInd (ExcelStructure[] payLoads) {
            MemoryStream generatedDocument = new MemoryStream ();
            try {
                using (SpreadsheetDocument document = SpreadsheetDocument.Create (generatedDocument, SpreadsheetDocumentType.Workbook)) {

                    UInt32 j = 1;
                    WorkbookPart workbookPart = document.AddWorkbookPart ();
                    workbookPart.Workbook = new Workbook ();

                    Sheets sheets = document.WorkbookPart.Workbook.AppendChild (new Sheets ());

                    for (int i = 0; i < payLoads.Length; i++) {
                        WorksheetPart worksheetPart = document.WorkbookPart.AddNewPart<WorksheetPart> ();

                        sheets = document.WorkbookPart.Workbook.GetFirstChild<Sheets> ();
                        //if (i > 0) sheets = document.WorkbookPart.Workbook.GetFirstChild<Sheets> ();

                        var payLoad = payLoads[i];

                        var sheetData = new SheetData ();
                        worksheetPart.Worksheet = new Worksheet (sheetData);

                        Sheet sheet = new Sheet () { Id = workbookPart.GetIdOfPart (worksheetPart), SheetId = j++, Name = payLoad.sheetName };

                       

                        using (JsonDocument jd = JsonDocument.Parse (payLoad.jsonTableInfo)) {
                            JsonElement root = jd.RootElement;

                            // Get column list
                            JsonElement[] columns = root.GetProperty ("columns").EnumerateArray ().ToArray ();

                            // Get data rows
                            int countRows = root.TryGetProperty ("item_count", out JsonElement itemCount) ? itemCount.GetInt32 () : 0;
                            JsonElement[] tableRows = new JsonElement[countRows];
                            tableRows = root.TryGetProperty ("table_data", out JsonElement tableData) ? tableData.EnumerateArray ().ToArray () : tableRows;

                            // Create label dictionary
                            Dictionary<string, string> labelDict = new Dictionary<string, string> ();
                            if (payLoad.columnLabels != null) {
                                foreach (GenericExcelColumnLabel labelItem in payLoad.columnLabels) {
                                    labelDict.Add (labelItem.columnName, labelItem.columnLabel);
                                }
                            }

                            // Create Header Row
                            Row headerRow = new Row ();
                            foreach (JsonElement col in columns) {
                                string colName = col.GetProperty ("column_name").GetString ();
                                Cell cell = new Cell ();
                                try {
                                    cell.CellValue = new CellValue (labelDict[colName]);
                                } catch (KeyNotFoundException) {
                                    cell.CellValue = new CellValue (colName);
                                }
                                cell.DataType = CellValues.String;
                                headerRow.AppendChild (cell);
                            }

                            sheetData.AppendChild (headerRow);

                            // Create Data rows
                            foreach (JsonElement tableRow in tableRows) {
                                Row newRow = new Row ();
                                foreach (JsonElement col in columns) {
                                    string colName = col.GetProperty ("column_name").GetString ();
                                    string colType = col.GetProperty ("js_type").GetString ().ToLower ();

                                    Cell cell = new Cell ();
                                    JsonElement cellContent = tableRow.TryGetProperty (colName, out JsonElement cellCont) ? cellCont : new JsonElement ();

                                    // string tmpCellValue;
                                    double tmpDouble;
                                    DateTime tmpDate;

                                    // Assign default  cell values
                                    cell.DataType = CellValues.Error;
                                    cell.CellValue = new CellValue ();

                                    // Manage missing data
                                    if (!string.Equals (cellContent.ToString (), "")) {
                                        switch (colType) {
                                            case "string":
                                                cell.DataType = CellValues.String;
                                                string cellValuString = cellContent.ToString ();
                                                // Strip the text of any html tags
                                                cellValuString = Regex.Replace (cellValuString, @"<[^>]+>|&nbsp;", "").Trim ();
                                                cell.CellValue = new CellValue (cellValuString);
                                                break;
                                            case "numeric":
                                                if (cellContent.TryGetDouble (out tmpDouble)) {
                                                    cell.DataType = CellValues.Number;
                                                    cell.CellValue = new CellValue (tmpDouble);
                                                }
                                                break;
                                            case "date":
                                                if (cellContent.TryGetDateTime (out tmpDate)) {
                                                    cell.DataType = CellValues.String;
                                                    cell.CellValue = new CellValue (cellContent.GetDateTime ().ToString ());
                                                }
                                                break;
                                            case "boolean":
                                                cell.DataType = CellValues.String;
                                                cell.CellValue = new CellValue (cellContent.GetBoolean ().ToString ());
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                    newRow.AppendChild (cell);
                                }
                                sheetData.AppendChild (newRow);
                            }
                            
                        }
                         sheets.Append (sheet);
                    workbookPart.Workbook.Save ();
                    }
                    

                }
            } catch (Exception e) {
                throw e;
            }
            return generatedDocument;
        }


        public byte[] GenerateExcelFromJsonData(JObject jsonData)
        {

            using (MemoryStream generatedDocument = new MemoryStream())
            {
                using (SpreadsheetDocument document = SpreadsheetDocument.Create(generatedDocument, SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    var sheetData = new SheetData();
                    worksheetPart.Worksheet = new Worksheet(sheetData);

                    Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Sheet1" };
                    sheets.Append(sheet);

                    // Get data rows
                    JArray tableRows = (JArray)jsonData["table_data"];

                    // Create Header Row
                    Row headerRow = new Row();
                    foreach (JObject row in tableRows)
                    {
                        foreach (var property in row.Properties())
                        {
                            string colName = property.Name;
                            Cell cell = new Cell();
                            cell.DataType = CellValues.String;
                            cell.CellValue = new CellValue(colName);
                            headerRow.AppendChild(cell);
                        }
                    }
                    sheetData.AppendChild(headerRow);

                    // Create Data rows
                    foreach (JObject row in tableRows)
                    {
                        Row newRow = new Row();
                        foreach (var property in row.Properties())
                        {
                            string colValue = property.Value.ToString();
                            Cell cell = new Cell();
                            cell.DataType = CellValues.String;
                            cell.CellValue = new CellValue(colValue);
                            newRow.AppendChild(cell);
                        }
                        sheetData.AppendChild(newRow);
                    }
                    workbookPart.Workbook.Save();

                }
                byte[] byteArray = generatedDocument.ToArray();
                return byteArray;
            }

        }


    }

    public class ExcelStructure {
        public string jsonTableInfo { get; set; }
        public GenericExcelColumnLabel[] columnLabels { get; set; }
        public string sheetName { get; set; }
    }
}