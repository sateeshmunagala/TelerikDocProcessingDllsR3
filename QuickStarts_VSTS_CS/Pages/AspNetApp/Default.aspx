<%@ Page Language="C#" MasterPageFile="~/AppMaster.master" Title="Untitled Page" %>

<%@ Register Assembly="ArtOfTest.WebAii.AspNet"
             Namespace="ArtOfTest.WebAii.AspNet.WebControls"
             TagPrefix="test" %>

<script runat="server">
    protected void MyLoginControl_Authenticate(object sender, AuthenticateEventArgs e)
    {
        e.Authenticated = true;
    }
</script>
<asp:Content ID="Content1" ContentPlaceHolderID="PageContent" runat="Server">

    <!-- This Page Login Content -->
    <test:TestRegion ID="LoginContent" runat='server'>
        <asp:Login ID="MyLoginControl"
         InstructionText="Please log in."
         TitleText="Welcome to Company.com"
          DestinationPageUrl="~/DataPage.aspx"
         runat="server" OnAuthenticate="MyLoginControl_Authenticate">
        </asp:Login>
    </test:TestRegion>
    <!-- This Page Login Content -->

</asp:Content>
