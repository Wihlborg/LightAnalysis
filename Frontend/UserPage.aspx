<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserPage.aspx.cs" Inherits="Frontend.UserPage" %>

<!DOCTYPE html>


<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <link rel="stylesheet" href="css/style.css" type="text/css"/>
    <div class="center" runat="server">

        <form id="admin" runat="server">
            <asp:Image ID="imageAnalyze" runat="server" Height="350px" Width="1297px" />
            <br />
            <asp:TextBox ID="analyze" runat="server" TextMode="MultiLine"></asp:TextBox>
            <br />
            <asp:Button CssClass="button" ID="lastPicture" runat="server" OnClick="lastP" Text="Last Picture!" />
            <br />
            <asp:Button CssClass="button" ID="nextPicture" runat="server" OnClick="nextP" Text="Next Picture!" />
            <br />
            <asp:Button CssClass="button" ID="userLogout" runat="server" OnClick="ExitUserPage" Text="Logout" />

        </form>

    </div>

</body>
</html>
