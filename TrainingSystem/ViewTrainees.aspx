<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewTrainees.aspx.cs" Inherits="TrainingApp.ViewTrainees" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>View Trainees</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="container mt-5">
            <h3 class="mb-4">View Trainees</h3>

            <!-- 🔍 Filter Section -->
            <div class="row mb-4">
                <div class="col-md-3">
                    <asp:TextBox ID="txtSearchId" runat="server" CssClass="form-control" placeholder="Search by Trainee ID" />
                </div>
                <div class="col-md-3">
                    <asp:TextBox ID="txtSearchName" runat="server" CssClass="form-control" placeholder="Search by Name" />
                </div>
                <div class="col-md-3">
                    <asp:TextBox ID="txtSearchDept" runat="server" CssClass="form-control" placeholder="Search by Department ID" />
                </div>
                <div class="col-md-3 d-grid gap-2">
                    <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="btnSearch_Click" />
                    <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="btn btn-secondary" OnClick="btnReset_Click" />
                </div>
            </div>

            <!-- 📋 GridView -->
            <asp:GridView ID="gvTrainees" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered"
                DataKeyNames="TraineeID" OnRowEditing="gvTrainees_RowEditing" OnRowUpdating="gvTrainees_RowUpdating"
                OnRowCancelingEdit="gvTrainees_RowCancelingEdit" OnRowDeleting="gvTrainees_RowDeleting">
                <Columns>
                    <asp:TemplateField HeaderText="Trainee ID">
                        <ItemTemplate>
                            <%# Eval("TraineeID") %>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Name">
                        <ItemTemplate>
                            <%# Eval("Name") %>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtName" runat="server" Text='<%# Bind("Name") %>' />
                        </EditItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Join Date">
                        <ItemTemplate>
                            <%# Eval("JoiningDate", "{0:yyyy-MM-dd}") %>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtJoinDate" runat="server" Text='<%# Bind("JoiningDate", "{0:yyyy-MM-dd}") %>' />
                        </EditItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Department ID">
                        <ItemTemplate>
                            <%# Eval("DepartmentId") %>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtDeptId" runat="server" Text='<%# Bind("DepartmentId") %>' />
                        </EditItemTemplate>
                    </asp:TemplateField>

                    <asp:CommandField ShowEditButton="True" ShowDeleteButton="True" />
                </Columns>
            </asp:GridView>

            <asp:Label ID="lblMessage" runat="server" CssClass="text-success mt-3" />
        </div>
    </form>
</body>
</html>