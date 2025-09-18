<%@ Assembly Name="Microsoft.SharePoint.IdentityModel, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharepointIdentity" Namespace="Microsoft.SharePoint.IdentityModel" Assembly="Microsoft.SharePoint.IdentityModel, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Assembly Name="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"%> <%@ Page Language="C#" Inherits="Microsoft.SharePoint.IdentityModel.Pages.MultiLogonPage" MasterPageFile="~/_layouts/15/errorv15.master"       %> <%@ Import Namespace="Microsoft.SharePoint.WebControls" %> <%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> <%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> <%@ Import Namespace="Microsoft.SharePoint" %> <%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<asp:Content ContentPlaceHolderId="PlaceHolderPageTitle" runat="server">
	<SharePoint:EncodedLiteral runat="server"  EncodeMethod="HtmlEncode" Id="ClaimsLogonPageTitle" />
</asp:Content>
<asp:Content ContentPlaceHolderId="PlaceHolderPageTitleInTitleArea" runat="server"> <div style="display:none">
	<SharePoint:EncodedLiteral runat="server" EncodeMethod="HtmlEncode" Id="ClaimsLogonPageTitleInTitleArea" /></div>
</asp:Content>
<asp:Content ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <style type="text/css">
        .center {
            margin: auto;
            width: 70%;
            border: 1px solid black;
            padding: 30px;
        }

        .img-center {
            display: block;
            margin-left: auto;
            margin-right: auto;
            width: 200px;
        }

        .warning-browser {
            text-align: center;
            width: 100% !important;
            color: #b60081 !important;
            font-family: 18px !important;
            font-weight: 600 !important;
            border: none !important;
            position: absolute;
            top: 50%;
            right: 0%;
        }
    </style>

    <div class="center" id="claim_logon">
        <img src="/_layouts/15/AeonHR/ClientApp/assets/images/logo-white.png" alt="Aeon" class="img-center" />
        <br />
        <p style="text-align: center">
            <SharePoint:EncodedLiteral runat="server" EncodeMethod="HtmlEncode" ID="ClaimsLogonPageMessage" />
            <br />
            "Chọn loại thông tin đăng nhập bạn muốn sử dụng:"<br />
            <br />
            <SharepointIdentity:LogonSelector ID="ClaimsLogonSelector" runat="server" />
        </p>
        <br />
        <br />
    </div>
    <script type="text/javascript">        
        window.onload = function () {
            var agent = window.navigator.userAgent;
            if (!!document.documentMode || agent.indexOf("Edge") > -1) {
                this.document.body.innerHTML = "";
                var IE = document.createElement("div");
                if (typeof (IE) != 'undefined' && IE != null) {
                    IE.innerHTML = "";
                    IE.innerHTML = "<div style='margin: auto;width: 70%;border: 1px solid black;padding: 30px;text-align: center;width: 100% !important;color: #b60081 !important;font-family: 18px !important; font-weight: 600 !important;border: none !important; position: absolute; top:50%; right:0%'>"+
                    "This system is not supported in Internet Explorer/Microsoft Edge (old version). Please run it in Chrome, Firefox hoặc Microsoft Edge (latest version)<br>" +
                        "Hệ thống này không được hỗ trợ trên Internet Explorer/Microsoft Edge (phiên bản cũ). Vui lòng sử dụng hệ thống trên Chrome, Firefox hoặc Microsoft Edge (phiên bản mới nhất)</div>"
                    document.body.appendChild(IE);
                }
            }
        }
    </script>
</asp:Content>
