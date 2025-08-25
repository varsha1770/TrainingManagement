<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="YourPage.aspx.cs" Inherits="TrainingSystem.YourPage" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Trainee Filter</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="container mt-4">

            <!-- 🔽 Status Filter -->
            <div class="mb-3">
                <asp:DropDownList ID="ddlStatusFilter" runat="server" AutoPostBack="true" CssClass="form-select" OnSelectedIndexChanged="ddlStatusFilter_SelectedIndexChanged">
                    <asp:ListItem Text="All" Value="All" />
                    <asp:ListItem Text="Active" Value="Active" />
                    <asp:ListItem Text="Inactive" Value="Inactive" />
                </asp:DropDownList>
            </div>

            <!-- 🔍 Search Filter -->
            <div class="mb-3 d-flex gap-2">
                <asp:TextBox ID="txtTraineeSearch" runat="server" placeholder="Search by Name or ID" CssClass="form-control" />
                <asp:Button ID="btnTraineeSearch" runat="server" Text="Search" OnClick="btnTraineeSearch_Click" CssClass="btn btn-primary" />
            </div>

            <!-- 📅 Date Filter -->
            <div class="mb-3 d-flex gap-2">
                <asp:TextBox ID="txtStartDate" runat="server" placeholder="Start Date" CssClass="form-control" TextMode="Date" />
                <asp:TextBox ID="txtEndDate" runat="server" placeholder="End Date" CssClass="form-control" TextMode="Date" />
                <asp:Button ID="btnDateFilter" runat="server" Text="Filter by Date" OnClick="btnDateFilter_Click" CssClass="btn btn-secondary" />
            </div>

            <!-- 📊 GridView -->
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="false">
               <Columns>
                  <asp:BoundField DataField="TraineeId" HeaderText="ID" />
                  <asp:BoundField DataField="Name" HeaderText="Name" />
                  <asp:BoundField DataField="Status" HeaderText="Status" />
               </Columns>
           </asp:GridView>
<!-- Add more fields as needed -->

        </div>
    </form>
</body>
</html>
