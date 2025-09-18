using System;
using System.Net;
using System.Runtime.InteropServices;

namespace Aeon.HR.BusinessObjects.Handlers
{
    public class NetworkConnection : IDisposable
    {
        private string _networkPath;

        public NetworkConnection(string networkPath, NetworkCredential credentials)
        {
            _networkPath = networkPath;

            NETRESOURCE netResource = new NETRESOURCE
            {
                dwType = RESOURCETYPE_DISK,
                lpRemoteName = networkPath
            };

            int result = WNetAddConnection2(netResource, credentials.Password, credentials.UserName, 0);

            if (result != 0)
            {
                throw new ApplicationException($"Không thể kết nối đến '{networkPath}'. Mã lỗi: {result}");
            }
        }

        public void Dispose()
        {
            WNetCancelConnection2(_networkPath, 0, true);
        }

        #region Win32 API
        private const int RESOURCETYPE_DISK = 0x00000001;

        [StructLayout(LayoutKind.Sequential)]
        private class NETRESOURCE
        {
            public int dwScope = 0;
            public int dwType = 0;
            public int dwDisplayType = 0;
            public int dwUsage = 0;
            public string lpLocalName = null;
            public string lpRemoteName = null;
            public string lpComment = null;
            public string lpProvider = null;
        }

        [DllImport("mpr.dll")]
        private static extern int WNetAddConnection2(NETRESOURCE lpNetResource, string lpPassword, string lpUsername, int dwFlags);

        [DllImport("mpr.dll")]
        private static extern int WNetCancelConnection2(string lpName, int dwFlags, bool fForce);
        #endregion
    }
}