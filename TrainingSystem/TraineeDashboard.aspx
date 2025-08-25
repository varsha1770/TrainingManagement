<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TraineeDashboard.aspx.cs" Inherits="TrainingSystem.TraineeDashboard" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Trainee Dashboard</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />

        <div class="container mt-4">
            <!-- Filter Controls -->
            <div class="row mb-3">
                <div class="col-md-3">
                    <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control" placeholder="Start Date (yyyy-mm-dd)" />
                </div>
                <div class="col-md-3">
                    <asp:TextBox ID="txtEndDate" runat="server" CssClass="form-control" placeholder="End Date (yyyy-mm-dd)" />
                </div>
                <div class="col-md-3">
                    <asp:DropDownList ID="ddlStatusFilter" runat="server" CssClass="form-select">
                        <asp:ListItem Text="All" Value="All" />
                        <asp:ListItem Text="Active" Value="Active" />
                        <asp:ListItem Text="Inactive" Value="Inactive" />
                        <asp:ListItem Text="Pending" Value="Pending" />
                        <asp:ListItem Text="Deleted" Value="Deleted" />
                    </asp:DropDownList>
                </div>
                <div class="col-md-3">
                    <asp:TextBox ID="txtTraineeSearch" runat="server" CssClass="form-control" placeholder="Search by Name or ID" />
                </div>
            </div>

            <!-- Export Button -->
           

            <!-- Page Size Dropdown -->
            <div class="mb-3">
                <label for="ddlPageSize" class="form-label">Records per page:</label>
                <asp:DropDownList ID="ddlPageSize" runat="server" AutoPostBack="true" CssClass="form-select w-auto d-inline-block"
                    OnSelectedIndexChanged="ddlPageSize_SelectedIndexChanged">
                    <asp:ListItem Text="10" Value="10" />
                    <asp:ListItem Text="25" Value="25" />
                    <asp:ListItem Text="50" Value="50" />
                </asp:DropDownList>
            </div>

            <!-- GridView -->
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" AllowPaging="true"
                OnPageIndexChanging="GridView1_PageIndexChanging" PageSize="10"
                OnRowCommand="GridView1_RowCommand"
                CssClass="table table-bordered table-striped">
                <Columns>
                    <asp:BoundField DataField="TraineeId" HeaderText="ID" />
                    <asp:BoundField DataField="Name" HeaderText="Name" />
                    <asp:BoundField DataField="MailID" HeaderText="Email" />
                    <asp:BoundField DataField="Mobile" HeaderText="Phone" />
                    <asp:BoundField DataField="JoiningDate" HeaderText="Joining Date" DataFormatString="{0:dd-MM-yyyy}" HtmlEncode="false" />

                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <asp:Label ID="lblStatus" runat="server" Text='<%# Eval("Status") %>'
                                CssClass='<%# "badge " + ((TrainingSystem.TraineeDashboard)Page).GetStatusClass(Eval("Status").ToString()) %>' />
                        </ItemTemplate>
                    </asp:TemplateField>

                   <asp:TemplateField HeaderText="Actions">
    <ItemTemplate>
        <asp:LinkButton ID="lnkDetails" runat="server" CommandName="ShowDetails"
            CommandArgument='<%# Eval("TraineeId") %>' Text="View"
            CssClass="btn btn-sm btn-info me-1" />

        <asp:LinkButton ID="lnkDelete" runat="server" CommandName="SoftDelete"
            CommandArgument='<%# Eval("TraineeId") %>' Text="Delete"
            CssClass="btn btn-sm btn-danger me-1"
            Visible='<%# Eval("Status").ToString() != "Deleted" %>' />

        <asp:LinkButton ID="lnkRestore" runat="server" CommandName="Restore"
            CommandArgument='<%# Eval("TraineeId") %>' Text="Restore"
            CssClass="btn btn-sm btn-warning"
            Visible='<%# Eval("Status").ToString() == "Deleted" %>' />
    </ItemTemplate>
</asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>

        <!-- ✅ Dashboard Summary Row -->
        <div class="row mb-3 text-center">
            <asp:Label ID="lblTotal" runat="server" />
<asp:Label ID="lblActive" runat="server" />
<asp:Label ID="lblInactive" runat="server" />
<asp:Label ID="lblPending" runat="server" />
<asp:Label ID="lblDeleted" runat="server" />
        </div>

        <!-- Export to PDF -->
        <asp:Button ID="btnExportPDF" runat="server" Text="Export to PDF" CssClass="btn btn-danger ms-2" OnClick="btnExportPDF_Click" />
        <asp:Button ID="Button1" runat="server" Text="Export to Excel" OnClick="btnExportExcel_Click" CssClass="btn btn-success" />

        <!-- ✅ Editable Modal -->
        <div class="modal fade" id="myModal" tabindex="-1" aria-labelledby="modalLabel" aria-hidden="true">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="modalLabel">Edit Trainee Details</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <asp:HiddenField ID="hfTraineeId" runat="server" />

                        <div class="mb-2">
                            <strong>Name:</strong>
                            <asp:TextBox ID="txtName" runat="server" CssClass="form-control" />
                        </div>
                        <div class="mb-2">
                            <strong>Email:</strong>
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" />
                        </div>
                        <div class="mb-2">
                            <strong>Phone:</strong>
                            <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" />
                        </div>
                        <div class="mb-2">
                            <strong>Status:</strong>
                            <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-select">
                                <asp:ListItem Text="Active" Value="Active" />
                                <asp:ListItem Text="Inactive" Value="Inactive" />
                                <asp:ListItem Text="Pending" Value="Pending" />
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <asp:Button ID="btnSaveChanges" runat="server" Text="Save Changes"
                            CssClass="btn btn-primary" OnClick="btnSaveChanges_Click" />
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </div>
        <div class="row mb-3">
   <div class="col-md-3">
    <label for="txtStartDate"><strong>Start Date:</strong></label>
    <asp:TextBox ID="TextBox1" runat="server" CssClass="form-control" TextMode="Date" />
</div>
<div class="col-md-3">
    <label for="txtEndDate"><strong>End Date:</strong></label>
    <asp:TextBox ID="TextBox2" runat="server" CssClass="form-control" TextMode="Date" />
</div>
    <div class="col-md-3">
        <asp:DropDownList ID="DropDownList1" runat="server" CssClass="form-control">
            <asp:ListItem Text="All" Value="" />
            <asp:ListItem Text="Active" Value="Active" />
            <asp:ListItem Text="Inactive" Value="Inactive" />
            <asp:ListItem Text="Pending" Value="Pending" />
        </asp:DropDownList>
    </div>
    <div class="col-md-3">
        <asp:Button ID="btnFilter" runat="server" Text="Filter" CssClass="btn btn-primary" OnClick="btnFilter_Click" />
        <asp:Label ID="lblMessage" runat="server" CssClass="text-danger" Visible="false" />
    </div>
    <asp:GridView ID="GridView2" runat="server" AllowPaging="true" PageSize="10"
    OnPageIndexChanging="GridView1_PageIndexChanging" AutoGenerateColumns="true" />
    <div class="mb-3">
    <asp:Label ID="lblActiveCount" runat="server" CssClass="badge bg-success me-2" />
    <asp:Label ID="lblInactiveCount" runat="server" CssClass="badge bg-secondary me-2" />
    <asp:Label ID="lblPendingCount" runat="server" CssClass="badge bg-warning" />
</div>
</div>
        <!-- Modal -->
<div class="modal fade" id="traineeModal" tabindex="-1" aria-labelledby="traineeModalLabel" aria-hidden="true">
  <div class="modal-dialog">
    <div class="modal-content">
      <div class="modal-header">
        <h5 class="modal-title" id="traineeModalLabel">Trainee Details</h5>
        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
      </div>
      <div class="modal-body">
        <asp:Label ID="lblTraineeDetails" runat="server" CssClass="form-control-plaintext" />
      </div>
    </div>
  </div>
</div>

        <asp:GridView ID="GridView3" runat="server" AutoGenerateColumns="False"
    OnRowCommand="GridView1_RowCommand">
    <Columns>
        <asp:BoundField DataField="TraineeID" HeaderText="ID" />
        <asp:BoundField DataField="Name" HeaderText="Name" />
        <asp:BoundField DataField="JoiningDate" HeaderText="Joining Date" />
        <asp:BoundField DataField="Status" HeaderText="Status" />
        <asp:ButtonField ButtonType="Button" CommandName="ViewDetails" Text="View" />
    </Columns>
</asp:GridView>
        <!-- HiddenField to store TraineeID -->
<asp:HiddenField ID="HiddenField1" runat="server" />

<!-- Delete Confirmation Modal -->
<div class="modal fade" id="softDeleteModal" tabindex="-1" role="dialog">
  <div class="modal-dialog" role="document">
    <div class="modal-content">
      <div class="modal-header">
        <h5 class="modal-title">Confirm Delete</h5>
        <button type="button" class="close" data-dismiss="modal">&times;</button>
      </div>
      <div class="modal-body">
        Are you sure you want to delete this trainee?
      </div>
      <div class="modal-footer">
        <asp:Button ID="btnSoftDelete" runat="server" Text="Delete" CssClass="btn btn-danger"
            OnClick="btnSoftDelete_Click" />
        <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
          <button type="button" onclick="openSoftDeleteModal('<%# Eval("TraineeID") %>')">Delete</button>
          
      </div>
    </div>
  </div>
</div>
<asp:GridView ID="traineeGrid" runat="server" AutoGenerateColumns="false"
    CssClass="table table-bordered" DataKeyNames="TraineeId">

    <Columns>
        <asp:BoundField DataField="TraineeId" HeaderText="ID" />
        <asp:BoundField DataField="Name" HeaderText="Name" />
        <asp:BoundField DataField="MailID" HeaderText="Email" />
        <asp:BoundField DataField="Mobile" HeaderText="Phone" />
        <asp:BoundField DataField="Status" HeaderText="Status" />
        <asp:BoundField DataField="RegistrationDate" HeaderText="Registered On" />

        <asp:TemplateField HeaderText="Actions">
            <ItemTemplate>
                <asp:Button ID="btnRestore" runat="server" Text="Restore"
                    CssClass="btn btn-success"
                    CommandArgument='<%# Eval("TraineeId") %>'
                    OnClick="btnRestore_Click" />

                <button type="button" class="btn btn-danger"
                    onclick="openSoftDeleteModal('<%# Eval("TraineeId") %>')">
                    Delete
                </button>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>


<!-- Feedback Label -->
<asp:Label ID="Label1" runat="server" CssClass="text-success" />


<asp:Button ID="btnRestore" runat="server" Text="Restore" OnClick="btnRestore_Click" CssClass="btn btn-success" />
<asp:HiddenField ID="HiddenField2" runat="server" />
        <asp:DropDownList ID="DropDownList2" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlStatusFilter_SelectedIndexChanged">
    <asp:ListItem Text="Active" Value="0" />
    <asp:ListItem Text="Deleted" Value="1" />
    <asp:ListItem Text="All" Value="All" />
</asp:DropDownList>

        <!-- Bootstrap JS -->
        <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>

        <!-- JavaScript to trigger modal -->
        <script type="text/javascript">
            function showModal() {
                var myModal = new bootstrap.Modal(document.getElementById('myModal'));
                myModal.show();
            }


            function openSoftDeleteModal(traineeID) {
                document.getElementById('hfTraineeID').value = traineeID;
                $('#softDeleteModal').modal('show');
            }
</script>
    </form>
</body>
</html>