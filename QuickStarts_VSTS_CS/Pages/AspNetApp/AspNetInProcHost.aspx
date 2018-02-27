<%@ Page Language="C#" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">

    protected void treeView1_SelectedNodeChanged(object sender, EventArgs e)
    {
        label1.Text = "TreeView1 SelectedNodeChanged To: " + treeView1.SelectedNode.Text;
    }

    protected void Calendar1_SelectionChanged(object sender, EventArgs e)
    {
        label1.Text = "Calendar1 SelectionChanged To: " + Calendar1.SelectedDate.ToString();
    }

    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        label1.Text = "LinkButton1 Clicked";
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        label1.Text = "Button1 Clicked";
    }

    protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        label1.Text = "DropDownList1 Changed To: " + DropDownList1.SelectedItem.Text;
    }

    protected void TextBox1_TextChanged(object sender, EventArgs e)
    {
        label1.Text = "TextBox1 TextChanged To: " + TextBox1.Text;
    }
</script>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>

        <asp:Label Font-Bold="true" Font-Size="X-Large" ID="label1" runat="server" Text="NoAction"></asp:Label>
        <br />
        <br />
        <asp:Button ID="Button1" runat="server" Text="Button" OnClick="Button1_Click" />
        <asp:LinkButton ID="LinkButton1" runat="server" OnClick="LinkButton1_Click">LinkButton</asp:LinkButton>
        <asp:TextBox AutoPostBack ="true" ID="TextBox1" runat="server" OnTextChanged="TextBox1_TextChanged"></asp:TextBox>
        <asp:DropDownList ID="DropDownList1" AutoPostBack="true" runat="server" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged">
            <asp:ListItem>Item1</asp:ListItem>
            <asp:ListItem>Item2</asp:ListItem>
        </asp:DropDownList>
        <asp:Calendar ID="Calendar1" runat="server" OnSelectionChanged="Calendar1_SelectionChanged"></asp:Calendar>

    </div>
        <asp:TreeView ID="treeView1" runat="server" OnSelectedNodeChanged="treeView1_SelectedNodeChanged">
            <Nodes>
                <asp:TreeNode Text="Node1" Value="Node1"></asp:TreeNode>
                <asp:TreeNode Text="Node2" Value="Node2"></asp:TreeNode>
            </Nodes>
        </asp:TreeView>
    </form>
</body>
</html>
