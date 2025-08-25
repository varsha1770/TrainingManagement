<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TraineeList.aspx.cs" Inherits="TrainingSystem.TraineeList" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Trainee List</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script type="text/javascript">
        function showDeleteModal(id) {
            $('#<%= hfDeleteTraineeId.ClientID %>').val(id);
            var deleteModal = new bootstrap.Modal(document.getElementById('deleteConfirmModal'));
            deleteModal.show();
            var toast = new bootstrap.Toast(document.getElementById('successToast'));
            toast.show();
        }
        function showEditModal() {
            var modal = new bootstrap.Modal(document.getElementById('traineeModalEdit'));
            modal.show();
        }
        function hideEditModal() {
            $('#editModal').modal('hide');
        }

</script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container mt-5">
            <h2 class="mb-4">Trainee List</h2>

            <!-- 🔍 Filter Panel -->
            <div class="row mb-4">
                <div class="col-md-4">
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Search by name or email" />
                </div>
                <div class="col-md-3">
                    <asp:DropDownList ID="ddlDepartmentFilter" runat="server" CssClass="form-select" />
                </div>
                <div class="col-md-2">
                    <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control" TextMode="Date" placeholder="Start Date" />
                </div>
                <div class="col-md-2">
                    <asp:TextBox ID="txtEndDate" runat="server" CssClass="form-control" TextMode="Date" placeholder="End Date" />
                </div>
                <div class="col-md-1">
                    <asp:Button ID="btnFilter" runat="server" Text="Filter" CssClass="btn btn-primary w-100" OnClick="btnFilter_Click" />
                </div>
            </div>

            <!-- 📋 GridView -->
             <asp:GridView ID="GridView1" runat="server"
    CssClass="table table-bordered table-striped"
    AutoGenerateColumns="False"
    OnRowCommand="GridView1_RowCommand"
    DataKeyNames="TraineeId">

    <Columns>
        <asp:BoundField DataField="TraineeId" HeaderText="ID" />
        <asp:BoundField DataField="Name" HeaderText="Name" />
        <asp:BoundField DataField="DepartmentId" HeaderText="Department" />
        
        <asp:BoundField DataField="Phone" HeaderText="Phone" />
        <asp:BoundField DataField="JoiningDate" HeaderText="Joining Date" DataFormatString="{0:dd MMM yyyy}" />

        <asp:TemplateField HeaderText="Actions">
            <ItemTemplate>
                <asp:Button ID="btnView" runat="server" Text="View"
                    CssClass="btn btn-info btn-sm me-1"
                    CommandName="ViewDetails"
                    CommandArgument='<%# Eval("TraineeId") %>' />

                <asp:Button ID="btnEdit" runat="server" Text="Edit"
                    CssClass="btn btn-warning btn-sm me-1"
                    CommandName="EditTrainee"
                    CommandArgument='<%# Eval("TraineeId") %>' />

                <asp:Button ID="btnDelete" runat="server" Text="Delete"
                    CssClass="btn btn-danger btn-sm"
                    CommandName="SoftDelete"
                    CommandArgument='<%# Eval("TraineeId") %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Restore">
            <ItemTemplate>
                <asp:Button ID="btnRestore" runat="server" Text="Restore"
                    CssClass="btn btn-success btn-sm"
                    CommandName="Restore"
                    CommandArgument='<%# Eval("TraineeId") %>'
                    Visible='<%# Eval("Status").ToString() == "Inactive" %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Status">
            <ItemTemplate>
                <asp:Label ID="lblStatus" runat="server"
                    Text='<%# Eval("Status") %>'
                    CssClass='<%# Eval("Status").ToString() == "Active" ? "badge bg-success" : "badge bg-danger" %>' />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>

            <!-- 📤 Export Buttons -->
            <div class="mt-3">
                <asp:Button ID="btnExportExcel" runat="server" Text="Export to Excel" CssClass="btn btn-success me-2" OnClick="btnExportExcel_Click" />
                <asp:Button ID="btnExportPdf" runat="server" Text="Export to PDF" CssClass="btn btn-danger" OnClick="btnExportPdf_Click" />
            </div>
        </div>


        <!-- 👤 Trainee Details Modal -->
        <div class="modal fade" id="traineeModal" tabindex="-1" aria-labelledby="traineeModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="traineeModalLabel">Trainee Details</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <asp:Literal ID="litTraineeDetails" runat="server" />
                    </div>
                </div>
            </div>
        </div>

        <!-- ✏️ Edit Trainee Modal -->
        <div class="modal fade" id="editTraineeModal" tabindex="-1" aria-labelledby="editTraineeModalLabel" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Edit Trainee</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <asp:HiddenField ID="hfTraineeId" runat="server" />
                        <div class="mb-3">
                            <label>Name</label>
                            <asp:TextBox ID="txtEditName" runat="server" CssClass="form-control" />
                        </div>
                        <div class="mb-3">
                            <label>Email</label>
                            <asp:TextBox ID="txtEditEmail" runat="server" CssClass="form-control" />
                        </div>
                        <div class="mb-3">
                            <label>Phone</label>
                            <asp:TextBox ID="txtEditPhone" runat="server" CssClass="form-control" />
                        </div>
                        <div class="mb-3">
                            <label>Department</label>
                            <asp:DropDownList ID="ddlEditDepartment" runat="server" CssClass="form-select" />
                        </div>
                    </div>
                    <div class="modal-footer">
                        <asp:Button ID="btnUpdateTrainee" runat="server" Text="Update" CssClass="btn btn-primary" OnClick="btnUpdateTrainee_Click" />
                    </div>
                    <asp:Button ID="btnViewActive" runat="server" Text="View Active Trainees" OnClick="btnViewActive_Click" />
                    <asp:Button ID="btnViewDeleted" runat="server" Text="View Deleted Trainees" OnClick="btnViewDeleted_Click" />
                </div>
            </div>
        </div>

        <!-- 🗑️ Delete Confirmation Modal -->
        <div class="modal fade" id="deleteConfirmModal" tabindex="-1" aria-labelledby="deleteConfirmLabel" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Confirm Delete</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        Are you sure you want to delete this trainee?
                        <asp:HiddenField ID="hfDeleteTraineeId" runat="server" />
                    </div>
                   <div class="modal-footer">
                        <asp:Button ID="btnConfirmDelete" runat="server" Text="Delete" CssClass="btn btn-danger" OnClick="btnConfirmDelete_Click" />
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                    </div>
                </div>
            </div>
        </div>
        <div class="toast-container position-fixed bottom-0 end-0 p-3">
    <div class="toast align-items-center text-bg-success border-0" id="successToast" role="alert" aria-live="assertive" aria-atomic="true">
        <div class="d-flex">
            <div class="toast-body">
                Trainee deleted successfully!
            </div>
            <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
        </div>
    </div>
</div>
        <div class="trainee-summary mb-3">
           <asp:Label ID="lblActiveCount" runat="server" CssClass="badge bg-success me-2" />
           <asp:Label ID="lblDeletedCount" runat="server" CssClass="badge bg-danger me-2" />
           <asp:Label ID="lblTotalCount" runat="server" CssClass="badge bg-primary" />
        </div>
        <div class="row mb-4">
    <div class="col-md-4">
        <div class="card text-white bg-success">
            <div class="card-body">
                <h5 class="card-title">Active Trainees</h5>
                <asp:Label ID="Label1" runat="server" CssClass="card-text fs-4" />
            </div>
        </div>
    </div>
    <div class="col-md-4">
        <div class="card text-white bg-danger">
            <div class="card-body">
                <h5 class="card-title">Deleted Trainees</h5>
                <asp:Label ID="Label2" runat="server" CssClass="card-text fs-4" />
            </div>
        </div>
    </div>
    <div class="col-md-4">
        <div class="card text-white bg-primary">
            <div class="card-body">
                <h5 class="card-title">Total Trainees</h5>
                <asp:Label ID="Label3" runat="server" CssClass="card-text fs-4" />
            </div>
        </div>
    </div>
</div>
        <div class="row mb-3">
    <div class="col-md-4">
        <asp:DropDownList ID="ddlStatusFilter" runat="server" AutoPostBack="true" CssClass="form-select" OnSelectedIndexChanged="ddlStatusFilter_SelectedIndexChanged">
            <asp:ListItem Text="All" Value="All" />
            <asp:ListItem Text="Active" Value="Active" />
            <asp:ListItem Text="Deleted" Value="Deleted" />
        </asp:DropDownList>
    </div>
</div>
        <div class="row mb-3">
    <div class="col-md-4">
        <asp:TextBox ID="TextBox1" runat="server" CssClass="form-control" placeholder="Search by name or email" />
    </div>
    <div class="col-md-2">
        <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="btnSearch_Click" />
    </div>
</div>
        <div class="row mb-3">
    <div class="col-md-3">
        <asp:TextBox ID="TextBox2" runat="server" CssClass="form-control" placeholder="Start Date" TextMode="Date" />
    </div>
    <div class="col-md-3">
        <asp:TextBox ID="TextBox3" runat="server" CssClass="form-control" placeholder="End Date" TextMode="Date" />
    </div>
    <div class="col-md-2">
        <asp:Button ID="btnFilterDate" runat="server" Text="Filter" CssClass="btn btn-secondary" OnClick="btnFilterDate_Click" />
    </div>
</div>
        <!-- View Modal -->
<div class="modal fade" id="traineeModalView" tabindex="-1" aria-labelledby="traineeModalViewLabel" aria-hidden="true">
  <div class="modal-dialog modal-lg">
    <div class="modal-content">
      <div class="modal-header">
        <h5 class="modal-title" id="traineeModalViewLabel">Trainee Details</h5>
        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
      </div>
      <div class="modal-body">
        <asp:Label ID="lblModalContent" runat="server" CssClass="form-control-plaintext" />
      </div>
    </div>
  </div>
</div>
        <!-- Edit Modal -->
<div class="modal fade" id="traineeModalEdit" tabindex="-1" aria-labelledby="traineeModalEditLabel" aria-hidden="true">
  <div class="modal-dialog">
    <div class="modal-content">
      <div class="modal-header">
        <h5 class="modal-title" id="traineeModalEditLabel">Edit Trainee</h5>
        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
      </div>
      <div class="modal-body">
        <asp:HiddenField ID="hfEditId" runat="server" />
        <div class="mb-3">
            <label>Name</label>
            <asp:TextBox ID="TextBox4" runat="server" CssClass="form-control" />
        </div>
        <div class="mb-3">
            <label>Email</label>
            <asp:TextBox ID="TextBox5" runat="server" CssClass="form-control" />
        </div>
        <div class="mb-3">
            <label>Phone</label>
            <asp:TextBox ID="TextBox6" runat="server" CssClass="form-control" />
        </div>
        <div class="mb-3">
            <label>Department</label>
            <asp:TextBox ID="txtEditDepartment" runat="server" CssClass="form-control" />
        </div>
      </div>
      <div class="modal-footer">
        <asp:Button ID="btnSaveEdit" runat="server" Text="Save Changes" CssClass="btn btn-success" />
      </div>
    </div>
  </div>
</div>
        <asp:Button ID="btnEdit" runat="server" Text="Edit" CssClass="btn btn-warning btn-sm me-1"
    CommandName="EditTrainee" CommandArgument='<%# Eval("Id") %>' />
        <asp:Button ID="Button1" runat="server" Text="Edit" CssClass="btn btn-warning btn-sm"
    CommandName="EditTrainee" CommandArgument='<%# Eval("Id") %>' />
        <div class="modal fade" id="editModal" tabindex="-1" role="dialog">
  <div class="modal-dialog">
    <div class="modal-content">
      <div class="modal-body">
         <asp:HiddenField ID="hdnEditTraineeId" runat="server" />

<asp:TextBox ID="txtNameEdit" runat="server" CssClass="form-control" placeholder="Name" />

<asp:TextBox ID="txtEmailEdit" runat="server" CssClass="form-control" placeholder="Email" />

<asp:TextBox ID="txtPhoneEdit" runat="server" CssClass="form-control" placeholder="Phone" />

<asp:DropDownList ID="ddlStatusEdit" runat="server" CssClass="form-control">
    <asp:ListItem Text="Active" Value="Active" />
    <asp:ListItem Text="Inactive" Value="Inactive" />
</asp:DropDownList>
<asp:Button ID="btnUpdate" runat="server" Text="Update" CssClass="btn btn-primary" OnClick="btnUpdate_Click" />
      </div>
    </div>
  </div>
</div>
    </form>
</body>
</html>
