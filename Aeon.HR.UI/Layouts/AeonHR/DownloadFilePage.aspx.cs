using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.IO;
using System.Web;
using Aeon.HR.UI;

namespace Aeon.ManagementPortalUI
{
    public partial class DownloadFilePage : LayoutsPageBase
    {
        #region Constructers

        public DownloadFilePage()
        {
        }

        #endregion

        #region Properties

        public string FileUrl { get; set; }

        #endregion

        #region Methods

        protected override bool AllowAnonymousAccess
        {
            get
            {
                return true;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                this.Page.Response.CacheControl = "no-cache";
                this.FileUrl = WebUtility.GetQueryString<string>(this.Page, DownloadFilePage.QueryString_FileUrl);
                if (!string.IsNullOrEmpty(this.FileUrl))
                {
                    this.DownloadFile(this.FileUrl);
                    this.Page.Response.Redirect("/");
                }
                else
                    this.Page.Response.Redirect("/");
            }
            catch (Exception ex)
            {
                this.LogMessage(ex.ToString());
            }

        }

        public void DownloadFile(string url)
        {
            Stream m_Stream = null;

            int bytesToRead = 10000;
            byte[] buffers = new Byte[bytesToRead];
            if (!url.Contains("http://") && !url.Contains("https://"))
                url = SPContext.Current.Site.Url + url;
            try
            {
                SPSecurity.RunWithElevatedPrivileges(() =>
                {
                    //1. Get File Stream
                    using (SPSite m_SPSite = new SPSite(url))
                    {
                        using (SPWeb m_SPWeb = m_SPSite.OpenWeb())
                        {
                            SPFile m_SPFile = m_SPWeb.GetFile(url);
                            m_Stream = m_SPFile.OpenBinaryStream();

                            //2. Return data
                            var m_Response = HttpContext.Current.Response;
                            m_Response.ContentType = "application/octet-stream";

                            //Name the file 
                            m_Response.AddHeader("Content-Disposition", "attachment; filename=\"" + m_SPFile.Name + "\"");
                            m_Response.AddHeader("Content-Length", m_SPFile.Length.ToString());

                            int m_Length;
                            do
                            {
                                // Verify that the client is connected.
                                if (m_Response.IsClientConnected)
                                {
                                    m_Length = m_Stream.Read(buffers, 0, bytesToRead);
                                    m_Response.OutputStream.Write(buffers, 0, m_Length);

                                    // Flush the data
                                    m_Response.Flush();

                                    //Clear the buffer
                                    buffers = new Byte[bytesToRead];
                                }
                                else
                                {
                                    // cancel the download if client has disconnected
                                    m_Length = -1;
                                }
                            } while (m_Length > 0); //Repeat until no data is read                    
                        }
                    }
                });
            }
            finally
            {
                if (m_Stream != null)
                {
                    //Close the input stream
                    m_Stream.Close();
                }
            }
        }

        #endregion

        #region Actions


        #endregion

        #region Constants

        public const string QueryString_FileUrl = "FileUrl";

        #endregion
    }
}
