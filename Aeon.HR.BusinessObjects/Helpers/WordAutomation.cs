using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Spire.Doc;
using BookmarkStart = DocumentFormat.OpenXml.Wordprocessing.BookmarkStart;
using DocumentFormat.OpenXml.Office2010.Word;
using System.Diagnostics;

namespace Aeon.HR.BusinessObjects.Helpers
{
    class WordAutomation
    {
        public static IEnumerable<string> FindTokens(string str, string pattern)
        {
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            var matches = regex.Matches(str);
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    yield return match.Value;
                }
            }
        }
        public static IEnumerable<string> FindFieldTokens(string str)
        {
            var tokens = FindTokens(str, @"\[\[[\d\w\s]*\]\]");
            foreach (var token in tokens)
            {
                yield return token;
            }
        }
        // To search and replace content in a document part.
        internal static void SearchAndReplace(Stream document, Dictionary<string, string> pros)
        {
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(document, true))
            {
                string docText = null;
                using (StreamReader sr = new StreamReader(wordDoc.MainDocumentPart.GetStream()))
                {
                    docText = sr.ReadToEnd();
                }
                var tokens = FindFieldTokens(docText);
                foreach (var token in tokens)
                {
                    var fieldToken = token.Trim(new char[] { '[', ']' });
                    if (pros.ContainsKey(fieldToken))
                    {
                        docText = docText.Replace(token, pros[fieldToken]);
                    }
                }
                using (StreamWriter sw = new StreamWriter(wordDoc.MainDocumentPart.GetStream(FileMode.Create)))
                {
                    sw.Write(docText);
                }
            }
        }
        private static void UpdateCheckBox(Stream str, List<string> checkedBoxs)
        {
            try
            {
                using (var wordDoc = WordprocessingDocument.Open(str, true))
                {
                    string uncheckValue = "☐";
                    string checkValue = "☒";
                    foreach (SdtContentCheckBox ctrl in wordDoc.MainDocumentPart.Document.Body.Descendants<SdtContentCheckBox>())
                    {
                        var tagProperty = ctrl.Parent.Descendants<Tag>().FirstOrDefault();
                        if (checkedBoxs.Any(x => x == tagProperty.Val))
                        {
                            SdtContentRun text = (SdtContentRun)ctrl.Parent.Parent.Descendants<SdtContentRun>().ToList()[0];
                            text.InnerXml = text.InnerXml.Replace(uncheckValue, checkValue);
                        }
                    }
                    wordDoc.MainDocumentPart.Document.Save();
                }
            }
            catch (Exception ex)
            {

            }
        }

        public static Stream GenerateWordDocumentForPrinter(Stream originalDocument, Dictionary<string, string> pros, List<Dictionary<string, string>> tblPros, List<string> checkedBoxs = null)
        {
            var tempms = new MemoryStream();
            originalDocument.Seek(0, SeekOrigin.Begin);
            originalDocument.CopyTo(tempms);
            tempms.Seek(0, SeekOrigin.Begin);
            if (tblPros.Count > 0)
            {
                GenerateTable(tempms, tblPros);
            }
            tempms.Seek(0, SeekOrigin.Begin);
            SearchAndReplace(tempms, pros);
            if (checkedBoxs != null && checkedBoxs.Any())
            {
                UpdateCheckBox(tempms, checkedBoxs);
            }
            tempms.Seek(0, SeekOrigin.Begin);
            return tempms;
        }

        public static Stream GenerateWordDocumentForPrinter(Stream originalDocument, Dictionary<string, string> pros, List<List<Dictionary<string, string>>> tblPros, List<string> checkedBoxs = null)
        {
            var tempms = new MemoryStream();
            originalDocument.Seek(0, SeekOrigin.Begin);
            originalDocument.CopyTo(tempms);
            tempms.Seek(0, SeekOrigin.Begin);
            if (tblPros.Count > 0)
            {
                GenerateTable(tempms, tblPros);
            }
            tempms.Seek(0, SeekOrigin.Begin);
            SearchAndReplace(tempms, pros);
            if (checkedBoxs != null && checkedBoxs.Any())
            {
                UpdateCheckBox(tempms, checkedBoxs);
            }
            tempms.Seek(0, SeekOrigin.Begin);
            return tempms;
        }

        public static byte[] MergeFiles(List<byte[]> files)
        {
            using (var mstr = new MemoryStream())
            {
                using (PdfDocument outPdf = new PdfDocument())
                {
                    foreach (var file in files)
                    {
                        using (var fileStr = new MemoryStream(file))
                        {
                            using (var pdfFile = PdfReader.Open(fileStr, PdfDocumentOpenMode.Import))
                            {
                                CopyPages(pdfFile, outPdf);
                            }
                        }
                    }
                    outPdf.Save(mstr);
                }
                return mstr.ToArray();
            }
        }

        private static void GenerateTable(Stream str, List<Dictionary<string, string>> properties)
        {
            using (var wordDoc = WordprocessingDocument.Open(str, true))
            {
                var tables = wordDoc.MainDocumentPart.Document.Descendants<DocumentFormat.OpenXml.Wordprocessing.Table>().ToList();
                foreach (var table in tables)
                {
                    var previousSection = table.PreviousSibling();
                    var siblingSection = table.NextSibling();
                    if (previousSection != null && siblingSection != null)
                    {
                        var previousText = previousSection.Descendants<Text>().FirstOrDefault();
                        var siblingText = siblingSection.Descendants<Text>().FirstOrDefault();
                        if (previousText != null && siblingText != null)
                        {
                            if (previousText.Text == "~~" && siblingText.Text == "~~")
                            {
                                previousSection.Remove();
                                siblingSection.Remove();
                                var rows = table.Descendants<DocumentFormat.OpenXml.Wordprocessing.TableRow>().ToList();
                                var lastRow = rows.Last();
                                foreach (var property in properties)
                                {
                                    var newRow = lastRow.CloneNode(true);
                                    var xmlRowContent = newRow.InnerXml;
                                    var tokens = FindFieldTokens(xmlRowContent);
                                    foreach (var token in tokens)
                                    {
                                        var fieldValueToken = token.Trim(new char[] { '[', ']' });
                                        if (property.ContainsKey(fieldValueToken))
                                        {
                                            xmlRowContent = xmlRowContent.Replace(token, property[fieldValueToken]);
                                        }
                                        else
                                        {
                                            xmlRowContent = xmlRowContent.Replace(token, string.Empty);
                                        }
                                    }
                                    newRow.InnerXml = xmlRowContent;
                                    table.Append(newRow);
                                }
                                lastRow.Remove();
                            }
                        }
                    }
                }
                wordDoc.MainDocumentPart.Document.Save();
            }
        }
        private static void GenerateTable(Stream str, List<List<Dictionary<string, string>>> properties)
        {
            using (var wordDoc = WordprocessingDocument.Open(str, true))
            {
                var tables = wordDoc.MainDocumentPart.Document.Descendants<DocumentFormat.OpenXml.Wordprocessing.Table>().ToList();
                var index = 0;
                foreach (var table in tables)
                {
                    var previousSection = table.PreviousSibling();
                    var siblingSection = table.NextSibling();
                    if (previousSection != null && siblingSection != null)
                    {
                        var previousText = previousSection.Descendants<Text>().FirstOrDefault();
                        var siblingText = siblingSection.Descendants<Text>().FirstOrDefault();
                        if (previousText != null && siblingText != null)
                        {
                            if (previousText.Text == "~~" && siblingText.Text == "~~")
                            {
                                previousSection.Remove();
                                siblingSection.Remove();
                                var rows = table.Descendants<DocumentFormat.OpenXml.Wordprocessing.TableRow>().ToList();
                                var lastRow = rows.Last();
                                if (properties.ElementAtOrDefault(index) != null)
                                {
                                    foreach (var property in properties[index])
                                    {
                                        var newRow = lastRow.CloneNode(true);
                                        var xmlRowContent = newRow.InnerXml;
                                        var tokens = FindFieldTokens(xmlRowContent);
                                        foreach (var token in tokens)
                                        {
                                            var fieldValueToken = token.Trim(new char[] { '[', ']' });
                                            if (property.ContainsKey(fieldValueToken))
                                            {
                                                xmlRowContent = xmlRowContent.Replace(token, property[fieldValueToken]);
                                            }
                                            else
                                            {
                                                xmlRowContent = xmlRowContent.Replace(token, string.Empty);
                                            }
                                        }
                                        newRow.InnerXml = xmlRowContent;
                                        table.Append(newRow);
                                    }
                                }
                                lastRow.Remove();
                            }
                        }
                    }
                    index++;
                }
                wordDoc.MainDocumentPart.Document.Save();
            }
        }

        private static void CopyPages(PdfSharp.Pdf.PdfDocument from, PdfSharp.Pdf.PdfDocument to)
        {
            for (int i = 0; i < from.PageCount; i++)
            {
                to.AddPage(from.Pages[i]);
            }
        }
        public static byte[] ExportPDF(string template, Dictionary<string, string> pros, List<Dictionary<string, string>> tblPros, List<string> checkedBoxs = null)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory; // You get main rott
            var filePath = System.IO.Path.Combine(path, "PrintDocument", template);
            byte[] content = null;
            using (var file = File.Open(filePath, FileMode.Open))
            {
                using (var str = new MemoryStream())
                {
                    using (var mStr = GenerateWordDocumentForPrinter(file, pros, tblPros, checkedBoxs))
                    {
                        var document = new Spire.Doc.Document();
                        document.LoadFromStream(mStr, FileFormat.Auto);
                        document.SaveToStream(str, FileFormat.PDF);
                        content = str.ToArray();
                    }
                }
            }
            return content;
        }

        public static byte[] ExportPDF(string template, Dictionary<string, string> pros, List<List<Dictionary<string, string>>> tblPros, List<string> checkedBoxs = null)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory; // You get main rott
            var filePath = System.IO.Path.Combine(path, "PrintDocument", template);
            byte[] content = null;
            using (var file = File.Open(filePath, FileMode.Open))
            {
                using (var str = new MemoryStream())
                {
                    using (var mStr = GenerateWordDocumentForPrinter(file, pros, tblPros, checkedBoxs))
                    {
                        var document = new Spire.Doc.Document();
                        document.LoadFromStream(mStr, FileFormat.Auto);
                        document.SaveToStream(str, FileFormat.PDF);
                        content = str.ToArray();
                    }
                }
            }
            return content;
        }


    }
}
