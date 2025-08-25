<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewById.aspx.cs" Inherits="TrainingApp.ViewById" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>View Trainee by ID</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="container mt-5">
            <h3 class="mb-4">Fetch Trainee Details by ID</h3>

            <div class="mb-3">
                <label for="txtTraineeId" class="form-label">Trainee ID</label>
                <asp:TextBox ID="txtTraineeId" runat="server" CssClass="form-control" />
            </div>

            <asp:Button ID="btnFetch" runat="server" Text="Fetch Details" CssClass="btn btn-primary" OnClick="btnFetch_Click" />
            <br /><br />

            <asp:Label ID="lblResult" runat="server" CssClass="text-info" />
        </div>
    </form>
</body>
</html>