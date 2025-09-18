<%@ Assembly Name="Microsoft.SharePoint.IdentityModel, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Assembly Name="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Page Language="C#" Inherits="Microsoft.SharePoint.IdentityModel.Pages.FormsSignInPage" MasterPageFile="~/_layouts/15/errorv15.master" %>

<%@ Import Namespace="Microsoft.SharePoint.WebControls" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharepointIdentity" Namespace="Microsoft.SharePoint.IdentityModel" Assembly="Microsoft.SharePoint.IdentityModel, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<asp:Content ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    <SharePoint:EncodedLiteral runat="server" EncodeMethod="HtmlEncode" ID="ClaimsFormsPageTitle" />
</asp:Content>
<asp:Content ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <style>
        .center {
            margin: auto;
            width: 50%;
            border: 1px solid black;
            padding: 30px;
        }

        .img-center {
            display: block;
            margin-left: auto;
            margin-right: auto;
            width: 200px;
        }
    </style>
    <div id="SslWarning" style="color: red; display: none">
        <SharePoint:EncodedLiteral runat="server" EncodeMethod="HtmlEncode" ID="ClaimsFormsPageMessage" />
    </div>
    <div class="center">
        <img src="/_layouts/15/AeonHR/ClientApp/assets/images/logo-white.png" alt="Aeon" class="img-center" />
        <br />
        <br />
        <asp:Login ID="signInControl" FailureText="<%$Resources:wss,login_pageFailureText%>" runat="server" Width="100%">
            <LayoutTemplate>
                <asp:Label ID="FailureText" class="ms-error" runat="server" />
                <table width="100%">
                    <tr>
                        <td nowrap="nowrap">
                            <SharePoint:EncodedLiteral runat="server" Text="Tên đăng nhập/Username:" EncodeMethod='HtmlEncode' />
                        </td>
                        <td width="100%">
                            <asp:TextBox ID="UserName" autocomplete="off" runat="server" class="ms-inputuserfield" Width="99%" /></td>
                    </tr>
                    <tr>
                        <td nowrap="nowrap">
                          <SharePoint:EncodedLiteral runat="server" Text="Mật mã/Password:" EncodeMethod='HtmlEncode' />
                        </td>
                        <td width="100%">
                            <asp:TextBox ID="password" TextMode="Password" autocomplete="off" runat="server" class="ms-inputuserfield" Width="99%" /></td>
                    </tr>
                    <tr>
                        <td colspan="2" align="right">
                            <asp:Button ID="login" CommandName="Login" Text="Đăng nhập/Signin" runat="server" /></td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <%--<asp:CheckBox ID="RememberMe"  Text="<%$SPHtmlEncodedResources:wss,login_pageRememberMe%>" runat="server" />--%>

                        </td>
                    </tr>
                </table>
            </LayoutTemplate>
        </asp:Login>
    </div>
</asp:Content>
