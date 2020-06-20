<%@ Page Title="Home Page" Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Frontend._Default" %>

<!DOCTYPE html>


<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <link rel="stylesheet" href="css/style.css" type="text/css"/>
    <div class="center" runat="server">

        <img class="image" src="css/logo.png" />
        <form id="post" runat="server">
            <a>Enter email: </a>
            <br />
            <asp:TextBox CssClass="textbox" ID="accountNamez" runat="server"></asp:TextBox>
            <br />
            <a>Enter password: </a>
            <br />
            <asp:TextBox CssClass="textbox" ID="passwordz" runat="server"></asp:TextBox>
            <br />
            <asp:Button CssClass="button" ID="LoginB" runat="server" Text="Login" OnClick="OnclickLogin" />
            <asp:Button CssClass="button" ID="LoginR" runat="server" Text="Register" OnClick="OnclickRegister" />
        </form>

        <asp:Label CssClass="label" ID="LabelLogin" runat="server" Text=""></asp:Label>

    </div>

</body>
</html>

