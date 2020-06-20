<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="Frontend.Register" %>

<!DOCTYPE html>


<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <link rel="stylesheet" href="css/style.css" type="text/css"/>
    <div class="center" runat="server">

       <img class="image" src="css/logo.png" />
        <form id="form1" runat="server">
            <a>Enter email: </a>
            <br />
            <asp:TextBox CssClass="textbox" ID="Email" runat="server"></asp:TextBox>
            <br />
            <a>Enter password: </a>
            <br />
            <asp:TextBox CssClass="textbox" ID="Password" runat="server"></asp:TextBox>
            <br />
            <asp:Button id="Button1" runat="server" CssClass="button" onclick="ButtonRegister" Text="Register"></asp:Button>
        </form>

        <asp:label CssClass="label" ID="LabelRegister" runat="server" Text=""></asp:label>

    </div>

</body>
</html>
