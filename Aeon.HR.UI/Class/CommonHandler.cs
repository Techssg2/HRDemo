using Aeon.ManagementPortalUI.Helpers;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using LacViet.Lib;

namespace Aeon.ManagementPortalUI
{
    public class CommonHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            string returnValue = string.Empty;
            BaseHandlerResult result = new BaseHandlerResult();
            SPUser currentUser = SPContext.Current.Web.CurrentUser;
            try
            {
                SPUser user = SPContext.Current.Web.CurrentUser;
                string actionValue = context.Request.Params["action"];
                switch (actionValue)
                {
                    case "viewFileOnWeb":
                        {
                            string filePath = context.Request.Params["filePath"];
                            string itemID = context.Request.Params["itemID"];
                            SPFile file = filePath.GetSPFileBySharepointList(itemID);

                            this.LogMessage("Tudm-1");

                            if (file == null)
                            {
                                file = filePath.UploadFileToDocumentLibrary(itemID);
                                this.LogMessage("Tudm-2:"+ file.Name);
                            }
                            this.LogMessage("Tudm-3");
                            List<string> imageExts = new List<string>() { "png", "jpeg", "jpg" };
                            string fileURL = "";

                            if (imageExts.Contains(Path.GetExtension(filePath).TrimStart('.').ToLower()))
                            {
                                fileURL = file.ServerRelativeUrl;
                            }
                            else
                            {
                                fileURL = file.Item.GetWOPIFrameUrl(SPWOPIFrameAction.View);
                            }
                            result.data = fileURL;
                            result.success = true;
                            break;
                        }
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                result.success = false;
                result.data = new
                {
                    ItemID = ex.Message + ex.StackTrace
                };
            }

            returnValue = new JavaScriptSerializer().Serialize(result);
            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.Write(returnValue);
            context.Response.Flush();
            context.ApplicationInstance.CompleteRequest();
        }
    }

    public class BaseHandlerResult
    {
        public BaseHandlerResult()
        {

        }

        #region Properties
        public bool success
        {
            get;
            set;
        }

        public string message
        {
            get;
            set;
        }

        public object data
        {
            get;
            set;
        }
        #endregion
    }
}
