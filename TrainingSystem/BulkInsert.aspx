<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BulkInsert.aspx.cs" Inherits="TrainingSystem.BulkInsert" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Bulk Upload Profiles</title>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server" enctype="multipart/form-data">
        <div class="container mt-5">
            <div class="row justify-content-center">
                <div class="col-md-8">

                    <h2 class="text-success mb-4 text-center">📤 Bulk Upload Profiles</h2>

                    <!-- File Upload -->
                    <div class="mb-3">
                        <label for="fileUpload" class="form-label fw-bold">Select CSV File</label>
                        <asp:FileUpload ID="fileUpload" runat="server" CssClass="form-control" />
                    </div>

                    <!-- Upload Button -->
                    <div class="mb-3 text-center">
                        <asp:Button ID="btnUpload" runat="server" Text="Upload & Preview" CssClass="btn btn-success px-4" OnClick="btnUpload_Click" />
                    </div>

                    <!-- Status Message -->
                    <div class="mb-3 text-center">
                        <asp:Label ID="lblStatus" runat="server" CssClass="text-info fw-semibold" />
                    </div>

                    <!-- Preview Table -->
                    <asp:GridView ID="gvPreview" runat="server" AutoGenerateColumns="true" CssClass="table table-bordered table-striped" Visible="false" />

                    <!-- Confirm Insert Button -->
                    <div class="mt-3 text-center">
                        <asp:Button ID="btnConfirmInsert" runat="server" Text="Confirm & Insert" CssClass="btn btn-primary px-4" OnClick="btnConfirmInsert_Click" Visible="false" />
                    </div>

                    <!-- Export to CSV Button -->
                    <div class="mt-3 text-center">
                        <asp:Button ID="btnExport" runat="server" Text="Export to CSV" CssClass="btn btn-outline-secondary px-4" OnClick="btnExport_Click" />
                    </div>

                    <!-- Show Department Summary Button -->
                    <div class="mt-4 text-center">
                        <asp:Button ID="btnShowSummary" runat="server" Text="Show Department Summary" CssClass="btn btn-info px-4" OnClick="btnShowSummary_Click" />
                    </div>

                    <!-- Department Summary Grid -->
                    <div class="mt-3">
                        <h4 class="text-primary text-center">📊 Department Summary</h4>
                        <asp:GridView ID="gvDeptSummary" runat="server" AutoGenerateColumns="true" CssClass="table table-bordered table-hover" Visible="false" />
                    </div>

                </div>
            </div>
        </div>
    </form>
</body>
</html>