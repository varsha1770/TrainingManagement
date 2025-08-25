<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SearchFilter.aspx.cs" Inherits="TrainingApp.SearchFilter" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Search and Filter Trainees</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="container mt-5">
            <h3 class="mb-4">Search and Filter Trainees</h3>

            <div class="row mb-3">
                <div class="col-md-4">
                    <label for="txtSearchName" class="form-label">Search by Name</label>
                    <asp:TextBox ID="txtSearchName" runat="server" CssClass="form-control" />
                </div>
                <div class="col-md-3">
                    <label for="txtFromDate" class="form-label">From Date</label>
                    <asp:TextBox ID="txtFromDate" runat="server" CssClass="form-control" TextMode="Date" />
                </div>
                <div class="col-md-3">
                    <label for="txtToDate" class="form-label">To Date</label>
                    <asp:TextBox ID="txtToDate" runat="server" CssClass="form-control" TextMode="Date" />
                </div>
                <div class="col-md-2 d-flex align-items-end">
                    <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary w-100" OnClick="btnSearch_Click" />
                </div>
            </div>

            <asp:GridView ID="gvResults" runat="server" AutoGenerateColumns="True" CssClass="table table-bordered" />
            <asp:Label ID="lblMessage" runat="server" CssClass="text-danger mt-3" />
        </div>
    </form>
</body>
</html>
