using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml.Linq;


namespace Aeon.ManagementPortalUI.Helpers
{
    public static class FileHelper
    {
        public static SPFile GetSPFileBySharepointList(this string filePath, string folderName)
        {
            SPFile returnValue = null;
            try
            {
                Uri currentURI = HttpContext.Current.Request.Url;
                SPSecurity.RunWithElevatedPrivileges(delegate
                {
                    using (SPSite oSite = new SPSite(currentURI.GetSiteURL()))
                    {
                        using (SPWeb sPWeb = oSite.OpenWeb())
                        {
                            if (System.IO.File.Exists(filePath))
                            {
                                String fileName = $"{Path.GetFileNameWithoutExtension(filePath)}{Path.GetExtension(filePath)}";
                                String documentLibraryName = "Documents";
                                SPList documentList = sPWeb.Lists.TryGetList(documentLibraryName);
                                SPFolder myLibrary = documentList.RootFolder;
                                SPFolder targetFolder = myLibrary.CreateFolderIfNotExist(folderName);
                                targetFolder = myLibrary.SubFolders[folderName];
                                SPFile file = targetFolder.Files[fileName];
                                if (file != null)
                                    returnValue = file;
                                else
                                    returnValue = null;

                            }
                        }
                    }
                });
            }
            catch
            {
                returnValue = null;
            }
            return returnValue;
        }

        public static SPFile UploadFileToDocumentLibrary(this string filePath, string folderName)
        {
            SPFile returnValue = null;
            try
            {
                Uri currentURI = HttpContext.Current.Request.Url;
                SPSecurity.RunWithElevatedPrivileges(delegate
                {
                    using (SPSite oSite = new SPSite(currentURI.GetSiteURL()))
                    {
                        using (SPWeb oWeb = oSite.OpenWeb())
                        {
                            if (System.IO.File.Exists(filePath))
                            {
                                String fileName = $"{Path.GetFileNameWithoutExtension(filePath)}{Path.GetExtension(filePath)}";
                                String documentLibraryName = "Documents";
                                SPList documentList = oWeb.Lists.TryGetList(documentLibraryName);
                                SPFolder myLibrary = documentList.RootFolder;
                                SPFolder targetFolder = myLibrary.CreateFolderIfNotExist(folderName);
                                targetFolder = myLibrary.SubFolders[folderName];
                                // Prepare to upload
                                Boolean replaceExistingFiles = true;
                                //String fileName = System.IO.Path.GetFileName(filePath);

                                oWeb.AllowUnsafeUpdates = true;
                                using (FileStream fileStream = File.OpenRead(filePath))
                                {
                                    // Upload document
                                    returnValue = targetFolder.Files.Add($"{targetFolder.Url}/{fileName}", fileStream, replaceExistingFiles);
                                    if (returnValue.CheckOutStatus != SPFile.SPCheckOutStatus.None)
                                    {
                                        returnValue.CheckIn("Check-in");
                                    }
                                    // Commit 
                                    targetFolder.Update();
                                }
                                oWeb.AllowUnsafeUpdates = false;

                            }
                        }
                    }
                });
            }
            catch
            {
                returnValue = null;
            }
            return returnValue;
        }
        public static bool CheckChildFolderExist(this SPFolder spFolder, string folderName)
        {
            bool returnValue = false;
            try
            {
                if (!(spFolder is null))
                {
                    returnValue = spFolder.ParentWeb.GetFolder($"{spFolder.ServerRelativeUrl}/{folderName}").Exists;
                }
            }
            catch
            {
                returnValue = false;
            }
            return returnValue;
        }

        public static SPFolder CreateFolderIfNotExist(this SPFolder spFolder, string folderName)
        {
            SPFolder returnValue = null;
            try
            {
                if (!spFolder.CheckChildFolderExist(folderName))
                {
                    spFolder.ParentWeb.AllowUnsafeUpdates = true;
                    returnValue = spFolder.SubFolders.Add(folderName);
                    spFolder.Update();
                    spFolder.ParentWeb.AllowUnsafeUpdates = false;
                }
                else
                {
                    returnValue = spFolder.SubFolders[folderName];
                }
            }
            catch
            {
                returnValue = null;
            }
            return returnValue;
        }

        public static string GetSiteURL(this Uri uri)
        {
            string returnValue = string.Empty;
            try
            {
                if (!(uri is null))
                {
                    returnValue = $"{uri.Scheme}://{uri.Authority}";
                }
            }
            catch
            {
                returnValue = string.Empty;
            }
            return returnValue;
        }
    }
}
