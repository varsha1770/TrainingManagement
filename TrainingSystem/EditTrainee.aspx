<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditTrainee.aspx.cs" Inherits="TrainingSystem.EditTrainee" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Edit Trainee</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="container mt-5">
            <h2 class="mb-4">Edit Trainee</h2>

            <!-- 🔴 Error Message -->
            <asp:Label ID="lblError" runat="server" CssClass="text-danger mb-3 d-block" />

            <!-- 🟢 Success Message -->
            <asp:Label ID="lblMessage" runat="server" CssClass="text-success mb-3 d-block" />

            <!-- ⚙️ Debug Info -->
            <asp:Label ID="lblDebug" runat="server" CssClass="text-muted mb-3 d-block" />

            <!-- Name -->
            <div class="mb-3">
                <label for="txtName" class="form-label">Name</label>
                <asp:TextBox ID="txtName" runat="server" CssClass="form-control" />
            </div>

            <!-- Department -->
            <div class="mb-3">
                <label for="ddlDepartment" class="form-label">Department</label>
                <asp:DropDownList ID="ddlDepartment" runat="server" CssClass="form-select" />
            </div>

            <!-- Email -->
            <div class="mb-3">
                <label for="txtEmail" class="form-label">Email</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" />
            </div>

            <!-- Phone -->
            <div class="mb-3">
                <label for="txtPhone" class="form-label">Phone</label>
                <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" />
            </div>

            <!-- Joined Date -->
            <div class="mb-3">
                <label for="txtJoinedDate" class="form-label">Joined Date</label>
                <asp:TextBox ID="txtJoinedDate" runat="server" CssClass="form-control" TextMode="Date" />
            </div>

            <!-- Status -->
            <div class="mb-3">
                <label for="ddlStatus" class="form-label">Status</label>
                <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-select">
                    <asp:ListItem Text="Active" Value="Active" />
                    <asp:ListItem Text="Inactive" Value="Inactive" />
                </asp:DropDownList>
            </div>

            <!-- Buttons -->
            <div class="d-flex">
                <asp:Button ID="btnUpdate" runat="server" Text="Update" CssClass="btn btn-primary" OnClick="btnUpdate_Click" />
                <a href="TraineeList.aspx" class="btn btn-secondary ms-2">Back to List</a>
            </div>
        </div>
    </form>
</body>
</html>