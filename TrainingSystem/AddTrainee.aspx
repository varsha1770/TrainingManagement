<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddTrainee.aspx.cs" Inherits="TrainingApp.AddTrainee" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Add Trainee</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="container mt-5">
            <h3 class="mb-4">Add New Trainee</h3>

            <div class="mb-3">
                <label for="txtName" class="form-label">Name</label>
                <asp:TextBox ID="txtName" runat="server" CssClass="form-control" />
            </div>

            <div class="mb-3">
                <label for="txtJoinDate" class="form-label">Join Date</label>
                <asp:TextBox ID="txtJoinDate" runat="server" CssClass="form-control" TextMode="Date" />
            </div>

            <div class="mb-3">
                <label for="ddlDepartment" class="form-label">Department</label>
                <asp:DropDownList ID="ddlDepartment" runat="server" CssClass="form-select" />
            </div>

            <asp:Button ID="btnSubmit" runat="server" Text="Add Trainee" CssClass="btn btn-primary" OnClick="btnSubmit_Click" />
            <br /><br />
            <asp:Label ID="lblMessage" runat="server" CssClass="text-success" />
        </div>
    </form>
</body>
</html>