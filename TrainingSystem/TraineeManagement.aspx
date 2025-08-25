<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TraineeManagement.aspx.cs" Inherits="TrainingSystem.TraineeManagement" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Trainee Management</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
</head>
<body>
    <form id="form1" runat="server" class="container mt-4">

        <!-- 🔍 Search and Filter -->
        <div class="row mb-3">
            <div class="col-md-3">
                <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" Placeholder="Search by name" />
            </div>
            <div class="col-md-3">
                <asp:DropDownList ID="ddlDepartment" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlDepartment_SelectedIndexChanged" CssClass="form-control" />
            </div>
            <div class="col-md-2">
                <asp:TextBox ID="txtStartDate" runat="server" TextMode="Date" CssClass="form-control" />
            </div>
            <div class="col-md-2">
                <asp:TextBox ID="txtEndDate" runat="server" TextMode="Date" CssClass="form-control" />
            </div>
            <div class="col-md-2">
                <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" CssClass="btn btn-primary w-100" />
            </div>
        </div>

        <!-- 📊 GridView with Sorting and Actions -->
        <asp:GridView ID="GridView1" runat="server" CssClass="table table-bordered"
            AllowSorting="true"
            AutoGenerateColumns="False"
            OnRowCommand="GridView1_RowCommand">
            <Columns>
               <asp:BoundField DataField="TraineeID" HeaderText="ID" />
               <asp:BoundField DataField="Name" HeaderText="Name" />
               <asp:BoundField DataField="JoiningDate" HeaderText="Joined Date" DataFormatString="{0:dd-MM-yyyy}" />
               <asp:BoundField DataField="Status" HeaderText="Status" />
               <asp:BoundField DataField="DepartmentId" HeaderText="Department" />
                <asp:TemplateField HeaderText="Actions">
                    <ItemTemplate>
                       <asp:Button ID="btnDetails" runat="server" Text="Details" CommandName="ViewDetails" CommandArgument='<%# Eval("TraineeID") %>' CssClass="btn btn-info btn-sm me-1" />
                       <asp:Button ID="btnEdit" runat="server" Text="Edit" CommandName="EditTrainee" CommandArgument='<%# Eval("TraineeID") %>' CssClass="btn btn-warning btn-sm me-1" />
                       <asp:Button ID="btnDelete" runat="server" Text="Delete" CommandName="DeleteTrainee" CommandArgument='<%# Eval("TraineeID") %>'  CssClass="btn btn-danger btn-sm"  OnClientClick="return confirm('Are you sure you want to delete this trainee?');"  Visible='<%# !(bool)Eval("IsDeleted") %>' />
                       <asp:Button ID="btnRestore" runat="server" Text="Restore" CommandName="Restore" CommandArgument='<%# Eval("TraineeID") %>' CssClass="btn btn-sm btn-warning" Visible='<%# (bool)Eval("IsDeleted") %>' />

                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>

        <!-- ⏮️ Pagination -->
        <div class="d-flex justify-content-between align-items-center mt-3">
            <asp:Button ID="btnPrevious" runat="server" Text="Previous" OnClick="btnPrevious_Click" CssClass="btn btn-secondary" />
            <asp:Label ID="lblPageInfo" runat="server" CssClass="badge bg-info text-dark" />
            <asp:Button ID="btnNext" runat="server" Text="Next" OnClick="btnNext_Click" CssClass="btn btn-secondary" />
        </div>

        <!-- 📤 Export -->
        <div class="mt-3">
            <asp:Button ID="btnExport" runat="server" Text="Export to Excel" OnClick="btnExport_Click" CssClass="btn btn-success me-2" />
            <asp:Button ID="btnExportPDF" runat="server" Text="Export to PDF" OnClick="btnExportPDF_Click" CssClass="btn btn-danger" />
        </div>

        <!-- 📂 Bulk Insert -->
        <h4 class="mt-4">📂 Bulk Insert Trainees</h4>
        <div class="row mb-3">
            <div class="col-md-6">
                <asp:FileUpload ID="fileUpload" runat="server" CssClass="form-control" />
            </div>
            <div class="col-md-2">
                <asp:Button ID="btnBulkInsert" runat="server" Text="Upload & Insert" CssClass="btn btn-primary w-100" OnClick="btnBulkInsert_Click" />
            </div>
        </div>
        <div class="mt-2">
            <asp:Label ID="lblStatus" runat="server" CssClass="text-danger fw-bold" />
        </div>

        <!-- 📊 Department Summary -->
        <h4 class="mt-5">📊 Department Summary</h4>
        <asp:GridView ID="gvDeptSummary" runat="server" AutoGenerateColumns="False" CssClass="table table-striped">
            <Columns>
                <asp:BoundField DataField="DepartmentName" HeaderText="Department" />
                <asp:BoundField DataField="TraineeCount" HeaderText="Trainees" />
            </Columns>
        </asp:GridView>

        <!-- 🧾 Trainee Details Modal -->
        <div class="modal fade" id="traineeModal" tabindex="-1" aria-labelledby="traineeModalLabel" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="traineeModalLabel">Trainee Details</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <asp:Label ID="lblDetails" runat="server" CssClass="form-text" />
                    </div>
                </div>
            </div>
        </div>
        

    </form>
</body>
</html>