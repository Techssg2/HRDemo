using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.Academy.Common.Utils
{
    class WindowsClientContext : ClientContext
    {
        public WindowsClientContext(string webUrl) : base(webUrl)
        {
            this.ExecutingWebRequest += new EventHandler<WebRequestEventArgs>(AddWindowsAuthRequestHeader); //  register a 
        }
        private void AddWindowsAuthRequestHeader(object sender, WebRequestEventArgs e)
        {
            try
            {
                if (!e.WebRequestExecutor.RequestHeaders.AllKeys.Contains("X-FORMS_BASED_AUTH_ACCEPTED"))
                    e.WebRequestExecutor.RequestHeaders.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f"); // f to denote that use windows auth
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
