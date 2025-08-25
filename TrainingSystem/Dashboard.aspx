<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="TrainingApp.Dashboard" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Trainee Management Dashboard</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="container mt-5">
            <h2 class="text-center mb-4">Welcome to the Trainee Management System</h2>

            <!-- Summary Section -->
            <div class="row text-center mb-4">
                <div class="col-md-4">
                    <asp:Label ID="lblTotalTrainees" runat="server" CssClass="h4 text-primary" />
                </div>
                <div class="col-md-4">
                    <asp:Label ID="lblTotalDepartments" runat="server" CssClass="h4 text-success" />
                </div>
                <div class="col-md-4">
                    <asp:Label ID="lblRecentJoinees" runat="server" CssClass="h4 text-warning" />
                </div>
            </div>

            <!-- Navigation Buttons -->
            <div class="row g-3">
                <div class="col-md-3">
                    <asp:Button ID="btnAddTrainee" runat="server" Text="Add Trainee" CssClass="btn btn-outline-primary w-100" OnClick="btnAddTrainee_Click" />
                </div>
                <div class="col-md-3">
                     <asp:Button ID="btnManageTrainees" runat="server" Text="View Trainees" CssClass="btn btn-outline-success w-100" OnClick="btnManageTrainees_Click" />
                </div>
                <div class="col-md-3">
                    <asp:Button ID="btnBulkInsert" runat="server" Text="Bulk Upload" CssClass="btn btn-outline-info w-100" OnClick="btnBulkInsert_Click" />
                </div>
                <div class="col-md-3">
                    <asp:Button ID="btnExportCSV" runat="server" Text="Export to CSV" CssClass="btn btn-outline-dark w-100" OnClick="btnExportCSV_Click" />
                </div>
            </div>
        </div>
    </form>
</body>
</html>