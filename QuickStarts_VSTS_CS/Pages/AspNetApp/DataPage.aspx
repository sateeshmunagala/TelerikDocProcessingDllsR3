<%@ Page Language="C#" MasterPageFile="~/AppMaster.master" Title="Untitled Page" %>

<%@ Import Namespace="System.Data" %>
<%@ Register Assembly="ArtOfTest.WebAii.AspNet" Namespace="ArtOfTest.WebAii.AspNet.WebControls"
    TagPrefix="test" %>

<script runat="server">

    /// <summary>
    /// Page Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            // Bind the grid and cache the data in the session.
            grid1.DataSource = GetData();
            DataBind();
        }
    }

    /// <summary>
    /// Return a DataSet of some random data.
    /// </summary>
    /// <returns>DataSet</returns>
    private DataSet GetData()
    {
        DataSet data = new DataSet();
        DataTable dt = new DataTable();
        Random r = new Random(3);


        data.Tables.Add(dt);
        dt.Columns.Add("ID", typeof(int));
        dt.Columns.Add("Date", typeof(DateTime));
        dt.Columns.Add("FirstName", typeof(string));
        dt.Columns.Add("LastName", typeof(string));
        dt.Columns.Add("Balance", typeof(int));

        for (int i = 0; i < 15; i++)
        {
            DataRow row = dt.NewRow();
            row.ItemArray = new object[] {i,DateTime.Today.AddDays(i),"Name" + i.ToString(),
                "Last" + i.ToString(), r.Next(3000)};

            dt.Rows.Add(row);
        }

        return data;
    }

    /// <summary>
    /// OnCalendarSelectionChange()
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void cal1_SelectionChanged(object sender, EventArgs e)
    {
        // Get DataSet from Session
        TimeSpan diffDate = cal1.SelectedDate.Subtract(DateTime.Today);

        if (cal1.SelectedDate >= DateTime.Today && diffDate.Days < 15)
        {
            grid1.SelectedIndex = diffDate.Days;
            grid1.EditIndex = diffDate.Days;
        }

        grid1.DataSource = GetData();
        grid1.DataBind();
    }

</script>

<asp:Content ID="Content1" ContentPlaceHolderID="PageContent" runat="Server">
    <!-- This Data Page Content -->
    <test:TestRegion ID="DataPageContent" runat='server'>

        You are logged in.

        <table style="border: dotted 1px black; background-color: beige; width:80%;" cellpadding="15">
            <tr>
                <td>
                    Select A Date:
                    <!-- The Calendar Control Region -->
                    <test:TestRegion ID="CalendarControl" runat="server">
                        <asp:Calendar ID="cal1" runat="server" OnSelectionChanged="cal1_SelectionChanged"
                            BackColor="#FFFFCC" BorderColor="#FFCC66" BorderWidth="1px" Font-Names="Verdana"
                            Font-Size="8pt" ForeColor="#663399" Height="200px" Width="220px" DayNameFormat="Shortest"
                            ShowGridLines="True">
                            <SelectedDayStyle BackColor="#CCCCFF" Font-Bold="True" />
                            <TodayDayStyle BackColor="#FFCC66" ForeColor="White" />
                            <OtherMonthDayStyle ForeColor="#CC9966" />
                            <NextPrevStyle Font-Size="9pt" ForeColor="#FFFFCC" />
                            <DayHeaderStyle Font-Bold="True" BackColor="#FFCC66" Height="1px" />
                            <TitleStyle BackColor="#990000" Font-Bold="True" Font-Size="9pt" ForeColor="#FFFFCC" />
                            <SelectorStyle BackColor="#FFCC66" />
                        </asp:Calendar>
                    </test:TestRegion>
                    <!-- The Calendar Control Region -->

                </td>
                <td valign="top" style="border: solid 1px black;">
                     Customer Balances:
                    <!-- The GridView Region -->
                    <test:TestRegion ID="GridControl" runat="server">
                        <asp:GridView ID="grid1" runat="server" AutoGenerateColumns="False" BackColor="White"
                            BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" Font-Size="XX-Small">
                            <Columns>
                                <asp:TemplateField HeaderText="ID">
                                    <ItemTemplate>
                                        <%# Eval("ID") %>
                                    </ItemTemplate>
                                    <EditItemTemplate>

                                        <!-- ID Column Region [Will persist only in Edit Mode] -->
                                        <test:TestRegion ID="GridEditId" runat="server">
                                            <%# Eval("ID") %>
                                        </test:TestRegion>
                                        <!-- ID Column Region -->

                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField HeaderText="FirstName" DataField="FirstName" ReadOnly="true" />
                                <asp:BoundField HeaderText="LastName" DataField="LastName" ReadOnly="true" />
                                <asp:BoundField HeaderText="Date" DataField="Date" ReadOnly="true" />
                                <asp:TemplateField HeaderText="Balance">
                                    <ItemTemplate>
                                        <%# Eval("Balance") %>
                                    </ItemTemplate>
                                    <EditItemTemplate>

                                        <!-- Balance Column Region  [Will persist only in Edit Mode] -->
                                        <test:TestRegion ID="GridEditBalance" runat="server">
                                            <asp:TextBox ID="newBalance" Text='<%# Eval("Balance") %>' runat="server"></asp:TextBox>
                                        </test:TestRegion>
                                        <!-- Balance Column Region -->

                                    </EditItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <FooterStyle BackColor="#FFFFCC" ForeColor="#330099" />
                            <RowStyle BackColor="White" ForeColor="#330099" />
                            <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="#663399" />
                            <PagerStyle BackColor="#FFFFCC" ForeColor="#330099" HorizontalAlign="Center" />
                            <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="#FFFFCC" />
                        </asp:GridView>
                    </test:TestRegion>
                    <!-- The GridView Region -->

                </td>
            </tr>
        </table>
    </test:TestRegion>
    <!-- This Data Page Content -->
</asp:Content>
