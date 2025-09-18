using System.Reflection;
using Newtonsoft.Json;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.Infrastructure.Enums;
using FastMember;
using Aeon.HR.API.Helpers;
using Aeon.HR.ViewModels;
using System.Drawing;
using OfficeOpenXml.Style;

namespace Aeon.HR.BusinessObjects.Handlers.File
{
    public abstract class BaseExcelProcessing
    {
        protected IUnitOfWork _uow;
        public BaseExcelProcessing(IUnitOfWork uow)
        {
            _uow = uow;
        }
        /// <summary>
        /// Lấy dữ liệu từ file excel
        /// </summary>
        /// <param name="stream">Stream được convert từ file excel</param>
        /// <param name="hasHeader">Kiểm tra file excel có header hay không</param>
        /// <returns></returns>
        protected virtual DataTable GetDataTableFromExcel(Stream stream, bool hasHeader = true, int sheetIndex = 0)
        {
            using (var pck = new ExcelPackage(stream))
            {
                if (pck.Workbook.Worksheets.Count <= sheetIndex) { return new DataTable(); }
                // pck.Load(stream);
                var ws = pck.Workbook.Worksheets[sheetIndex]; // chỉ có 1 sheet, nếu nhiều sheet phải foreach
                if (ws != null)
                {
                    DataTable tbl = new DataTable();
                    tbl.TableName = ws.Name;
                    try
                    {
                        // Lấy data của dòng đầu tiên của sheet 
                        foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column]) //[int FromRow, int FromCol, int ToRow, int ToCol]
                        {
                            if (!hasHeader)
                            {
                                tbl.Columns.Add(string.Format("Column {0}", firstRowCell.Start.Column));
                            }
                            else
                            {
                                var colName = firstRowCell.Text;
                                if (tbl.Columns[colName] == null)
                                {
                                    tbl.Columns.Add(colName);
                                }
                                else
                                {
                                    tbl.Columns.Add(string.Format("Column {0}", firstRowCell.Start.Column));
                                }

                            }
                        }
                        // Lấy data từ dòng thứ 2 nếu có header, nếu không thì lấy data từ dòng đầu tiên (1)
                        var startRow = hasHeader ? 2 : 1;
                        var dateTimeType = typeof(DateTime);
                        for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                        {

                            var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                            bool isNotEmpty = wsRow.Any(x => x.Value != null && !string.IsNullOrEmpty(x.Value.ToString()));
                            if (isNotEmpty)
                            {
                                DataRow row = tbl.Rows.Add(); // Tạo mới 1 dòng trong Datatable
                                foreach (var cell in wsRow)
                                {
                                    if (cell.Value != null && cell.Value.GetType() == dateTimeType)
                                    {
                                        row[cell.Start.Column - 1] = (cell.Value as DateTime?)?.ToOADate();
                                    }
                                    else
                                        row[cell.Start.Column - 1] = cell.Value; // Lấy data từng cell và đổ vào row vừa tạo ở trên
                                }
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    return tbl;
                }
                else return null;
            }
        }
        protected List<MappingFieldToExport> ReadConfigurationFromFile()
        {
            List<MappingFieldToExport> result = null;
            var fileName = Json;
            var fileContent = JsonHelper.GetJsonContentFromFile("Mappings", fileName);
            if (!string.IsNullOrEmpty(fileContent))
            {
                var jsonGroup = JsonHelper.GetGroupDataByName(fileContent, JsonGroupName);
                if (!string.IsNullOrEmpty(jsonGroup))
                {
                    result = JsonConvert.DeserializeObject<List<MappingFieldToExport>>(jsonGroup);
                }
            }
            return result.Where(x=>x.Visible).ToList();
        }
        /// <summary>
        /// Lấy tất cả các fields sẽ được mapping từ file json
        /// </summary>
        /// <param name="json">Tên file json trong thư mục JsonConfiguration</param>
        /// <returns></returns>
        protected virtual List<FieldMappingDTO> LoadFields(string json)
        {
            //var currentPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            //return JsonConvert.DeserializeObject<List<FieldMappingDTO>>(File.ReadAllText(Path.Combine(currentPath, "JsonConfiguration", json)));
            return JsonConvert.DeserializeObject<List<FieldMappingDTO>>(JsonHelper.GetJsonContentFromFile("JsonConfiguration", json));
        }
        protected abstract string Json { get; }
        protected abstract string JsonGroupName { get; }
        protected List<FieldMappingDTO> Fields { get; set; }
        private Dictionary<string, TypeAccessor> _accessorCaching;
        /// <summary>
        /// Xử lý data từ DataRow để tạo 1 record
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="fields">Các fileds đã được lấy từ file json ở trên </param>
        /// <param name="row">Row trong Datatable đã lấy được từ hàm GetDataTableFromExcel</param>
        /// <param name="instance">Một record sẽ lưu xuống DB sau khi đã được fill data từ row</param>
        /// <param name="errors">Các lỗi nếu có khi process fields</param>
        protected void ProcessingFields<T>(List<FieldMappingDTO> fields, DataRow row, T instance, List<string> errors, TypeAccessor accessor = null)
        {
            foreach (var field in fields)
            {
                try
                {
                    var value = Convert.ToString(row[field.SourceField]);
                    if (accessor != null && !string.IsNullOrEmpty(value))
                    {
                        switch (field.Type)
                        {
                            case FieldType.Date:
                                var datetime = DateTime.MinValue;
                                if (instance.GetType().Name == "TransactionFromExcelViewModel" && field.TargetField == "TransactionDate")
                                {
                                    double dValue = 0d;
                                    if (Double.TryParse(value, out dValue))
                                    {
                                        accessor[instance, field.TargetField] = datetime.ToString();
                                    }
                                    else if (DateTime.TryParse(value, CultureInfo.CreateSpecificCulture("vi-VN"), DateTimeStyles.AssumeLocal, out datetime))
                                    {
                                        accessor[instance, field.TargetField] = datetime.ToString();
                                    }
                                }
                                else
                                {
                                    double dValue = 0d;
                                    if (Double.TryParse(value, out dValue))
                                    {
                                        DateTimeOffset dOffset = DateTime.FromOADate(dValue);
                                        accessor[instance, field.TargetField] = dOffset;
                                    }
                                    else if (DateTime.TryParse(value, CultureInfo.CreateSpecificCulture("vi-VN"), DateTimeStyles.AssumeLocal, out datetime))
                                    {
                                        DateTimeOffset dOffset = datetime;
                                        accessor[instance, field.TargetField] = dOffset;
                                    }
                                }

                                break;
                            case FieldType.Float:
                                accessor[instance, field.TargetField] = float.Parse(value);
                                break;
                            case FieldType.Double:
                                accessor[instance, field.TargetField] = double.Parse(value);
                                break;
                            case FieldType.Int:
                                accessor[instance, field.TargetField] = int.Parse(value);
                                break;
                            case FieldType.Guid:
                                accessor[instance, field.TargetField] = Guid.Parse(value);
                                break;
                            case FieldType.Boolean:
                                accessor[instance, field.TargetField] = bool.Parse(value);
                                break;
                            default:
                                accessor[instance, field.TargetField] = value.Trim();
                                break;
                        }
                    }
                    else
                    {
                        var errorMessage = $"{field.SourceField} Không có dữ liệu";
                        errors.Add(errorMessage);
                    }
                }
                catch (Exception ex)
                {
                    var errorMessage = $"{field.SourceField}: {ex.Message}";
                    errors.Add(errorMessage);
                }
            }
        }
        protected TypeAccessor GetTypeAccessor<T>(T instance)
        {
            TypeAccessor accessor = null;
            if (_accessorCaching == null)
            {
                _accessorCaching = new Dictionary<string, TypeAccessor>();
            }
            var eName = typeof(T).ToString(); // Lấy tên entity từ T
            if (_accessorCaching.ContainsKey(eName))
            {
                accessor = _accessorCaching[eName];
            }
            else
            {
                accessor = TypeAccessor.Create(typeof(T));
                _accessorCaching[eName] = accessor;
            }
            return accessor;
        }

        protected byte[] ExportExcel(DataTable tbl, bool allAsText = false)
        {           
            if (tbl.Rows.Count > 0)
            {
                using (var mStr = new MemoryStream())
                {
                    using (var pck = new ExcelPackage(mStr))
                    {
                        var ws = pck.Workbook.Worksheets.Add(JsonGroupName);                      
                        ws.Cells["A1"].LoadFromDataTable(tbl, true);
                        ws.Cells.AutoFitColumns();
                        if(allAsText)
                        {
                            ws.Cells.Style.Numberformat.Format = "@";
                        }
                        StyleHeader(ws);
                        pck.Save();
                    }
                    return mStr.ToArray();
                }
            }
            return null;
        }
        private void StyleHeader(ExcelWorksheet pck)
        {
            var headerCells = pck.Cells[1, 1, 1, pck.Dimension.End.Column];
            var headerFont = headerCells.Style.Font;
            headerFont.SetFromFont(new Font("Times New Roman", 12));
            headerFont.Bold = true;
        }
        protected void HandleCommonType(DataRow row, object value, int currentCell, MappingFieldToExport fieldMapping)
        {
            if (value != null)
            {
                row[currentCell] = value;
                if (fieldMapping.Type == FieldType.Date)
                {
                    row[currentCell] = DateTime.Parse(value.ToString()).ToLocalTime().ToString("dd/MM/yyyy");
                }
                else if (fieldMapping.Type == FieldType.Boolean)
                {
                    row[currentCell] = bool.Parse(value.ToString()) ? "Yes" : "No";
                }
                else if (fieldMapping.Type == FieldType.BooleanAsInt)
                {
                    row[currentCell] = bool.Parse(value.ToString()) ? "1" : "0";
                }
                else if (fieldMapping.Type == FieldType.Enum)
                {   
                    row[currentCell] = value.ToString();
                } 
                else if(fieldMapping.Type == FieldType.Double)
                {
                    CultureInfo cul = CultureInfo.CurrentCulture;
                    string decimalSep = cul.NumberFormat.CurrencyDecimalSeparator;//decimalSep ='.'
                    string groupSep = cul.NumberFormat.CurrencyGroupSeparator;//groupSep=','
                    var sFormat = string.Format("#{0}###", groupSep);
                    string a = double.Parse(value.ToString()).ToString(sFormat);
                    row[currentCell] = a;
                } 
                else if (fieldMapping.Type == FieldType.DateTimeOfSet) // 9
                {
                    row[currentCell] = DateTimeOffset.Parse(value.ToString()).ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss");
                }
            }
        }
        protected byte[] ExportManySheetExcel(List<DataTable> tbls)
        {
            if (tbls.Count > 0)
            {
                using (var mStr = new MemoryStream())
                {
                    using (var pck = new ExcelPackage(mStr))
                    {
                        for (int i = 0; i < tbls.Count; i++)
                        {
                            if (tbls[i].Rows.Count > 0)
                            {
                                var ws = pck.Workbook.Worksheets.Add(tbls[i].TableName);
                                ws.Cells["A1"].LoadFromDataTable(tbls[i], true);
                                ws.Cells.AutoFitColumns();
                            }
                        }
                        pck.Save();
                    }
                    return mStr.ToArray();
                }
            }
            return null;
        }
    }
}