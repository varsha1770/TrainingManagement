<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CreateTrainee.aspx.cs" Inherits="TrainingSystem.CreateTrainee" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Create Trainee</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="container mt-5">
            <h2 class="mb-4">Add New Trainee</h2>

            <asp:Label ID="lblMessage" runat="server" CssClass="text-success mb-3" />

            <div class="mb-3">
                <label for="txtName" class="form-label">Name</label>
                <asp:TextBox ID="txtName" runat="server" CssClass="form-control" />
            </div>

            <div class="mb-3">
                <label for="ddlDepartment" class="form-label">Department</label>
                <asp:DropDownList ID="ddlDepartment" runat="server" CssClass="form-select" />
            </div>

            <div class="mb-3">
                <label for="txtEmail" class="form-label">Email</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" />
            </div>

            <div class="mb-3">
                <label for="txtPhone" class="form-label">Phone</label>
                <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" />
            </div>

            <div class="mb-3">
                <label for="txtJoinedDate" class="form-label">Joined Date</label>
                <asp:TextBox ID="txtJoinedDate" runat="server" CssClass="form-control" TextMode="Date" />
            </div>

            <asp:Button ID="btnCreate" runat="server" Text="Create" CssClass="btn btn-success" OnClick="btnCreate_Click" />
            <a href="TraineeList.aspx" class="btn btn-secondary ms-2">Back to List</a>
        </div>
    </form>
</body>
</html>