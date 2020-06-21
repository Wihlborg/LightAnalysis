<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Forgot.aspx.cs" Inherits="Frontend.Forgot" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>

    <link rel="stylesheet" href="css/style.css" type="text/css"/>
    <div class="center" runat="server">
        
        <img class="image" src="css/logo.png" />
        <form id="forgot" runat="server">
             <a>Enter email: </a>
            <br />
            <asp:TextBox CssClass="textbox" ID="emailForgot" runat="server"></asp:TextBox>
            <asp:Button id="forgogtBtn" runat="server" CssClass="button" onclick="ButtonForgot" Text="Send"></asp:Button>

        </form>

       <asp:label CssClass="label" ID="LabelForgot" runat="server" Text=""></asp:label>


    </div>

</body>
</html>
