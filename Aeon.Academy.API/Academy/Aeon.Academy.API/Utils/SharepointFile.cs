using Aeon.Academy.Common.Configuration;
using Aeon.Academy.Common.Consts;
using Aeon.Academy.Common.Utils;
using Aeon.Academy.Services;
using Aeon.HR.Infrastructure.Utilities;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Web;
using SPC = Microsoft.SharePoint.Client;

namespace Aeon.Academy.API.Utils
{
    public class SharepointFile
    {
        private static readonly ILogger _logger = ServiceLocator.Resolve<ILogger>();
        private string SiteUrl { get; set; }
        private string DocumentLibrary { get; set; }
        private string FileName { get; set; }
        private string CourseFolder { get; set; }
        private string UserName { get; set; }
        private string Password { get; set; }
        private string ServerFolder { get; set; }

        public SharepointFile(string documentLibrary)
        {
            FileName = SharepointSettings.FileName;
            CourseFolder = SharepointSettings.CourseFolder;
            SiteUrl = SharepointSettings.SiteUrl;

            if (documentLibrary == DocumentLibraryName.Course)
            {
                DocumentLibrary = SharepointSettings.CourseDocumentLibrary;
            }
            if (documentLibrary == DocumentLibraryName.TrainingReport)
            {
                DocumentLibrary = SharepointSettings.ReportDocumentLibrary;
            }
            if (documentLibrary == DocumentLibraryName.TrainingRequest)
            {
                DocumentLibrary = SharepointSettings.RequestDocumentLibrary;
            }
            if (documentLibrary == DocumentLibraryName.TrainingInvitation)
            {
                DocumentLibrary = SharepointSettings.InvitationDocumentLibrary;
            }
            UserName = SharepointSettings.UserName;
            Password = SharepointSettings.Password;
            ServerFolder = SharepointSettings.ServerFolder;
        }

        public string UploadFiles(string folderName, object documents)
        {
            if (documents != null)
            {
                var uploadFiles = documents as Newtonsoft.Json.Linq.JArray;
                if (uploadFiles != null && uploadFiles.Any())
                {
                    foreach (var file in uploadFiles)
                    {
                        var file64Str = file["file"] != null ? file["file"].ToString() : string.Empty;
                        var fileBytes = Convert.FromBase64String(file64Str.Substring(file64Str.IndexOf(",") + 1));
                        var fileName = file["fileName"] != null ? file["fileName"].ToString() : Guid.NewGuid().ToString();
                        var state = file["state"] != null ? file["state"].ToString() : null;
                        if (!string.IsNullOrEmpty(ServerFolder))
                        {
                            // Dev testing - Write document to folder on server.
                            System.IO.File.WriteAllBytes(ServerFolder + fileName, fileBytes);
                        }
                        else
                        {
                            var result = string.Empty;
                            if (state == "Added")
                            {
                                //Upload file to SP library
                                result = UploadCourseDocumentsFromRequest(folderName, fileName, fileBytes);
                            }
                            else if (state == "Deleted")
                            {
                                //delete
                                result = DeleteDocuments(folderName, fileName);
                            }

                            if (!string.IsNullOrEmpty(result))
                            {
                                _logger.LogInfo("Uploading document error - " + result);
                                return result;
                            }
                        }
                    }
                }
            }
            return string.Empty;
        }
        public string UploadCourseDocuments(string courseSubFolder, string fileName)
        {
            try
            {
                #region ConnectToSharePoint
                //var securePassword = new SecureString();
                //foreach (char c in Password)
                //{ securePassword.AppendChar(c); }
                //var SPCredentials = new NetworkCredential(UserName, securePassword);
                #endregion
                #region Insert the data
                using (ClientContext CContext = new WindowsClientContext(SiteUrl))
                {
                    CContext.AuthenticationMode = ClientAuthenticationMode.FormsAuthentication;
                    CContext.FormsAuthenticationLoginInfo = new FormsAuthenticationLoginInfo(UserName, Password);
                    //CContext.Credentials = SPCredentials;
                    Web web = CContext.Web;
                    FileCreationInformation newFile = new FileCreationInformation();
                    byte[] FileContent = System.IO.File.ReadAllBytes(!string.IsNullOrEmpty(fileName) ? fileName : FileName);

                    newFile.ContentStream = new MemoryStream(FileContent);
                    newFile.Overwrite = true;
                    newFile.Url = Path.GetFileName(!string.IsNullOrEmpty(fileName) ? fileName : FileName);

                    List DocumentLibrary = web.Lists.GetByTitle(this.DocumentLibrary);
                    Folder Clientfolder = DocumentLibrary.RootFolder.Folders.Add(courseSubFolder);
                    Clientfolder.Update();
                    SPC.File uploadFile = Clientfolder.Files.Add(newFile);
                    CContext.Load(DocumentLibrary);
                    CContext.Load(uploadFile);

                    CContext.ExecuteQuery();
                }

                _logger.LogInfo("Ending uploading document");
                #endregion
            }
            catch (Exception exp)
            {
                _logger.LogError(exp);
                return exp.Message;
            }
            return string.Empty;
        }

        private string UploadCourseDocumentsFromRequest(string courseSubFolder, string fileName, byte[] document)
        {
            try
            {
                #region ConnectToSharePoint
                //var securePassword = new SecureString();
                //foreach (char c in Password)
                //{ securePassword.AppendChar(c); }
                //var SPCredentials = new NetworkCredential(UserName, securePassword, "aeon");
                #endregion
                #region Insert the data
                using (ClientContext CContext = new WindowsClientContext(SiteUrl))
                {
                    CContext.AuthenticationMode = ClientAuthenticationMode.FormsAuthentication;
                    CContext.FormsAuthenticationLoginInfo = new FormsAuthenticationLoginInfo(UserName, Password);

                    Web web = CContext.Web;
                    FileCreationInformation newFile = new FileCreationInformation();
                    newFile.Overwrite = true;

                    //Get files the request that are posted from frontend
                    var httpRequest = System.Web.HttpContext.Current.Request;
                    if (document != null)
                    {
                        newFile.ContentStream = new MemoryStream(document);
                        newFile.Url = fileName;
                    }
                    else if (System.Web.HttpContext.Current.Request.Files.Count > 0)
                    {
                        try
                        {
                            foreach (string file in httpRequest.Files)
                            {
                                var postedFile = httpRequest.Files[file];
                                BinaryReader binReader = new BinaryReader(postedFile.InputStream);
                                byte[] byteArray = binReader.ReadBytes(postedFile.ContentLength);
                                newFile.ContentStream = new MemoryStream(byteArray);
                            }
                        }
                        catch (System.Exception ex)
                        {
                            _logger.LogInfo("Get document from request error - " + ex.Message);
                        }
                    }
                    else
                    {
                        byte[] FileContent = System.IO.File.ReadAllBytes(!string.IsNullOrEmpty(fileName) ? fileName : FileName);
                        newFile.ContentStream = new MemoryStream(FileContent);
                        newFile.Url = Path.GetFileName(!string.IsNullOrEmpty(fileName) ? fileName : FileName);
                    }

                    List DocumentLibrary = web.Lists.GetByTitle(this.DocumentLibrary);
                    //Folder Clientfolder = DocumentLibrary.RootFolder.Folders.Add(courseSubFolder);
                    Folder Clientfolder = CreateFolderInternal(web, DocumentLibrary.RootFolder, courseSubFolder);
                    Clientfolder.Update();
                    SPC.File uploadFile = Clientfolder.Files.Add(newFile);
                    CContext.Load(DocumentLibrary);
                    CContext.Load(uploadFile);

                    CContext.ExecuteQuery();
                }

                _logger.LogInfo("Ending uploading document");
                #endregion
            }
            catch (Exception exp)
            {
                _logger.LogError(exp);
                return exp.Message;
            }
            return string.Empty;
        }

        public object GetCourseDocument(string subFolder)
        {
            try
            {
                #region ConnectToSharePoint
                //var securePassword = new SecureString();
                //foreach (char c in Password)
                //{ securePassword.AppendChar(c); }
                //var SPCredentials = new NetworkCredential(UserName, securePassword);
                #endregion
                #region Insert the data
                using (ClientContext CContext = new WindowsClientContext(SiteUrl))
                {
                    CContext.AuthenticationMode = ClientAuthenticationMode.FormsAuthentication;
                    CContext.FormsAuthenticationLoginInfo = new FormsAuthenticationLoginInfo(UserName, Password);
                    // CContext.Credentials = SPCredentials;
                    Web web = CContext.Web;
                    List courseList = CContext.Web.Lists.GetByTitle(this.DocumentLibrary);
                    var root = courseList.RootFolder;
                    CContext.Load(root);
                    CContext.ExecuteQuery();
                    Folder folder = CContext.Web.GetFolderByServerRelativeUrl(root.ServerRelativeUrl + "/" + subFolder);
                    CContext.Load(folder);
                    CContext.ExecuteQuery();

                    CamlQuery query = new CamlQuery();
                    query.ViewXml = @"<View Scope='Recursive'>
                                     <Query>
                                     </Query>
                                 </View>";
                    query.FolderServerRelativeUrl = folder.ServerRelativeUrl;

                    ListItemCollection items = courseList.GetItems(query);

                    CContext.Load(items);

                    CContext.ExecuteQuery();
                    var documents = new List<object>();
                    foreach (ListItem listItem in items)
                    {
                        var view = listItem.GetWOPIFrameUrl(SPC.Utilities.SPWOPIFrameAction.View);
                        CContext.Load(listItem);
                        CContext.ExecuteQuery();
                        var fileRef = listItem["FileRef"] != null ? listItem["FileRef"].ToString() : string.Empty;
                        var index = fileRef.LastIndexOf("/");
                        var itemName = fileRef.ToString().Substring(index < fileRef.Length - 1 ? index + 1 : index);
                        var relativeUrl = view.Value ?? string.Empty;
                        if (!string.IsNullOrEmpty(relativeUrl))
                        {
                            relativeUrl = relativeUrl.Replace("http://", string.Empty).Replace("https://", string.Empty);
                            var i = relativeUrl.IndexOf("/");
                            if (i > 0 && relativeUrl.Length - 1 > i)
                                relativeUrl = relativeUrl.Substring(i);
                        }
                        documents.Add(new
                        {
                            LinkView = relativeUrl,
                            FileName = itemName,
                            FileRef = listItem["FileRef"]
                        });
                    }

                    _logger.LogInfo("Ending get documents");
                    return documents;
                }

                #endregion
            }
            catch (Exception exp)
            {
                _logger.LogError(exp);
            }
            return null;
        }

        public string DeleteDocuments(string subFolder, string fileName)
        {
            try
            {
                //var securePassword = new SecureString();
                //foreach (char c in Password)
                //{ securePassword.AppendChar(c); }
                //var SPCredentials = new NetworkCredential(UserName, securePassword);
                using (ClientContext CContext = new WindowsClientContext(SiteUrl))
                {
                    CContext.AuthenticationMode = ClientAuthenticationMode.FormsAuthentication;
                    CContext.FormsAuthenticationLoginInfo = new FormsAuthenticationLoginInfo(UserName, Password);
                    // CContext.Credentials = SPCredentials;
                    Web web = CContext.Web;


                    List DocumentLibrary = web.Lists.GetByTitle(this.DocumentLibrary);
                    // Get Item using CAML Query
                    CamlQuery oQuery = new CamlQuery();

                    oQuery.ViewXml = @"<View Scope='Recursive'><Query><Where>
                    <Eq>
                    <FieldRef Name='LinkFilename' />
                    <Value Type='Text'>" + fileName + "</Value></Eq></Where></Query></View>";
                    oQuery.FolderServerRelativeUrl = "/" + this.DocumentLibrary + "/" + subFolder;

                    ListItemCollection oItems = DocumentLibrary.GetItems(oQuery);
                    CContext.Load(oItems);
                    CContext.ExecuteQuery();

                    var oItem = oItems.FirstOrDefault();

                    if (oItem != null)
                    {
                        // Get File object of the list item
                        SPC.File targetFile = oItem.File;

                        targetFile.DeleteObject();
                        CContext.ExecuteQuery();
                    }
                }

                _logger.LogInfo("Ending delete document - " + fileName);
            }
            catch (Exception exp)
            {
                _logger.LogError(exp);
                return exp.Message;
            }
            return string.Empty;
        }
        private static Folder CreateFolderInternal(Web web, Folder parentFolder, string fullFolderUrl)
        {
            var folderUrls = fullFolderUrl.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            string folderUrl = folderUrls[0];
            var curFolder = parentFolder.Folders.Add(folderUrl);
            web.Context.Load(curFolder);
            web.Context.ExecuteQuery();

            if (folderUrls.Length > 1)
            {
                var subFolderUrl = string.Join("/", folderUrls, 1, folderUrls.Length - 1);
                return CreateFolderInternal(web, curFolder, subFolderUrl);
            }
            return curFolder;
        }
        public string DeleteFolder(string folder)
        {
            try
            {
                #region ConnectToSharePoint
                //var securePassword = new SecureString();
                //foreach (char c in Password)
                //{ securePassword.AppendChar(c); }
                //var SPCredentials = new NetworkCredential(UserName, securePassword);
                #endregion
                #region Insert the data
                using (ClientContext CContext = new WindowsClientContext(SiteUrl))
                {
                    CContext.AuthenticationMode = ClientAuthenticationMode.FormsAuthentication;
                    CContext.FormsAuthenticationLoginInfo = new FormsAuthenticationLoginInfo(UserName, Password);
                    //CContext.Credentials = SPCredentials;
                    Web web = CContext.Web;

                    Folder targetFolder = web.GetFolderByServerRelativeUrl(this.DocumentLibrary + "/" + folder);

                    // Delete target folder
                    targetFolder.DeleteObject();

                    CContext.ExecuteQuery();
                    _logger.LogInfo("Ending get documents");
                }
                #endregion
            }
            catch (Exception exp)
            {
                _logger.LogError(exp);
                return exp.Message;
            }
            return string.Empty;
        }
        public Stream Download(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return null;
            try
            {
                #region ConnectToSharePoint
                //var securePassword = new SecureString();
                //foreach (char c in Password)
                //{ securePassword.AppendChar(c); }
                //var SPCredentials = new NetworkCredential(UserName, securePassword);
                #endregion
                #region Insert the data
                using (ClientContext CContext = new WindowsClientContext(SiteUrl))
                {
                    CContext.AuthenticationMode = ClientAuthenticationMode.FormsAuthentication;
                    CContext.FormsAuthenticationLoginInfo = new FormsAuthenticationLoginInfo(UserName, Password);
                    //CContext.Credentials = SPCredentials;
                    Web web = CContext.Web;

                    SPC.File file = web.GetFileByServerRelativeUrl(filePath);
                    if (file != null)
                    {
                        CContext.Load(file);
                        CContext.ExecuteQuery();

                        var fileInformation = Microsoft.SharePoint.Client.File.OpenBinaryDirect(CContext, file.ServerRelativeUrl);
                        return fileInformation.Stream;
                    }
                    _logger.LogInfo("Ending download document");
                }
                #endregion
            }
            catch (Exception exp)
            {
                _logger.LogError(exp);
                throw exp;
            }
            return null;
        }
    }
}