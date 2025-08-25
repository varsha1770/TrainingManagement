using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Drawing;

// iTextSharp for PDF
using iTextSharp.text;
using iTextSharp.text.pdf;

// EPPlus for Excel
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Web;

//using OfficeOpenXml.License;

namespace TrainingSystem
{
    public partial class TraineeList : System.Web.UI.Page
    {
        protected string connectionString = ConfigurationManager.ConnectionStrings["TrainingDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDepartments();
                BindGrid();           // ✅ Load active trainees into GridView
                LoadTraineeCounts();  // ✅ Count logic (assumed implemented)
                // Removed LoadActiveTrainees(); — not valid without traineeId
            }
        }

        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                // Ensure all rows are loaded
                GridView1.AllowPaging = false;
                BindGrid(); // ✅ Rebind full data before export

                using (ExcelPackage package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Trainee List");

                    // Add header row
                    for (int i = 0; i < GridView1.HeaderRow.Cells.Count; i++)
                    {
                        var cell = worksheet.Cells[1, i + 1];
                        cell.Value = GridView1.HeaderRow.Cells[i].Text.Trim();
                        cell.Style.Font.Bold = true;
                        cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        cell.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    // Add data rows
                    for (int i = 0; i < GridView1.Rows.Count; i++)
                    {
                        for (int j = 0; j < GridView1.Rows[i].Cells.Count; j++)
                        {
                            worksheet.Cells[i + 2, j + 1].Value = GridView1.Rows[i].Cells[j].Text.Trim();
                        }
                    }


                    // Auto-fit columns
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    // Send Excel file to browser
                    Response.Clear();
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("Content-Disposition", "attachment; filename=TraineeList.xlsx");
                    Response.BinaryWrite(package.GetAsByteArray());
                    Response.Flush();
                    Response.SuppressContent = true;
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
            }
            catch (Exception ex)
            {
                Response.Write("Error exporting to Excel: " + Server.HtmlEncode(ex.Message));
            }
        }
    


    private void LoadDepartments()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT Id, Name FROM Departments", conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                ddlDepartmentFilter.DataSource = reader;
                ddlDepartmentFilter.DataTextField = "Name";     // Displayed text
                ddlDepartmentFilter.DataValueField = "Id";      // Actual value
                ddlDepartmentFilter.DataBind();

                ddlDepartmentFilter.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Select Department --", ""));
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            BindGrid();
        }

        private void BindGrid()
        {
            string query = @"
        SELECT 
            TraineeId, 
            Name, 
            MailID AS Email, 
            Mobile AS Phone, 
            DepartmentId, 
            JoiningDate, 
            Status 
        FROM Trainees 
        WHERE IsDeleted = 0"; // ✅ Only active trainees

            List<SqlParameter> parameters = new List<SqlParameter>();

            // 🔍 Search by Name or Email
            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                query += " AND (Name LIKE @Search OR MailID LIKE @Search)";
                parameters.Add(new SqlParameter("@Search", "%" + txtSearch.Text.Trim() + "%"));
            }

            // 🏢 Filter by Department
            if (!string.IsNullOrWhiteSpace(ddlDepartmentFilter.SelectedValue))
            {
                query += " AND DepartmentId = @Dept";
                parameters.Add(new SqlParameter("@Dept", ddlDepartmentFilter.SelectedValue));
            }

            // 📅 Filter by Joining Date Range
            DateTime startDate, endDate;

            if (DateTime.TryParse(txtStartDate.Text, out startDate))
            {
                query += " AND JoiningDate >= @StartDate";
                parameters.Add(new SqlParameter("@StartDate", startDate));
            }

            if (DateTime.TryParse(txtEndDate.Text, out endDate))
            {
                query += " AND JoiningDate <= @EndDate";
                parameters.Add(new SqlParameter("@EndDate", endDate));
            }

            // 🔄 Bind to GridView
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddRange(parameters.ToArray());

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    GridView1.DataSource = dt;
                    GridView1.DataBind();
                }
            }
        }



        protected void btnExportPdf_Click(object sender, EventArgs e)
        {
            // Ensure all data is loaded
            GridView1.AllowPaging = false;
            BindGrid();

            // Set response headers for PDF
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=TraineeList.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            using (MemoryStream ms = new MemoryStream())
            {
                // Create PDF document
                Document doc = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(doc, ms);
                doc.Open();

                // Title
                iTextSharp.text.Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                iTextSharp.text.Paragraph title = new iTextSharp.text.Paragraph("Trainee List", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 20f
                };
                doc.Add(title);

                // Table with column count from GridView
                PdfPTable table = new PdfPTable(GridView1.HeaderRow.Cells.Count)
                {
                    WidthPercentage = 100
                };

                // Add headers
                foreach (TableCell headerCell in GridView1.HeaderRow.Cells)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(headerCell.Text))
                    {
                        BackgroundColor = BaseColor.LIGHT_GRAY,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    };
                    table.AddCell(cell);
                }

                // Add data rows
                foreach (GridViewRow row in GridView1.Rows)
                {
                    foreach (TableCell cell in row.Cells)
                    {
                        table.AddCell(new Phrase(cell.Text));
                    }
                }

                // Add table to document
                doc.Add(table);
                doc.Close();

                // Write PDF to response
                Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
                Response.OutputStream.Flush();
                Response.End();
            }
        }
        private void DeleteTraineeFromDatabase(string id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["YourConnectionStringName"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("DELETE FROM Trainees WHERE Id = @Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }



        }
        // Required override for export rendering
        public override void VerifyRenderingInServerForm(Control control)
        {
            // Required for exporting GridView
        }
        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int traineeId = Convert.ToInt32(e.CommandArgument);

            switch (e.CommandName)
            {
                case "ViewDetails":
                    ShowTraineeDetails(traineeId.ToString());
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showModal", "$('#traineeModalView').modal('show');", true);
                    break;

                case "EditTrainee":
                    Response.Redirect($"EditTrainee.aspx?id={traineeId}");
                    break;

                case "Restore":
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        SqlCommand cmd = new SqlCommand("UPDATE Trainees SET Status = 'Active', IsDeleted = 0 WHERE TraineeId = @TraineeId", con);
                        cmd.Parameters.AddWithValue("@TraineeId", traineeId);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                    LoadGridView();
                    break;

                case "SoftDelete":
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        SqlCommand cmd = new SqlCommand("UPDATE Trainees SET Status = 'Inactive', IsDeleted = 1 WHERE TraineeId = @TraineeId", con);
                        cmd.Parameters.AddWithValue("@TraineeId", traineeId);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                    LoadGridView();
                    break;

                case "DeleteTrainee":
                    DeleteTraineeFromDatabase(traineeId.ToString());
                    LoadGridView();
                    break;
            }
        }

        private void RestoreTrainee(int traineeId)
{
    string connectionString = ConfigurationManager.ConnectionStrings["TrainingDB"].ConnectionString;

    using (SqlConnection con = new SqlConnection(connectionString))
    {
        string query = "UPDATE Trainees SET IsDeleted = 0, Status = 'Active' WHERE TraineeId = @TraineeId";

        using (SqlCommand cmd = new SqlCommand(query, con))
        {
            cmd.Parameters.AddWithValue("@TraineeId", traineeId);
            con.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
        private void LoadTraineeForEdit(string id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Trainees WHERE Id = @Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    hfTraineeId.Value = id;
                    txtEditName.Text = reader["Name"].ToString();
                    txtEditEmail.Text = reader["Email"].ToString();
                    txtEditPhone.Text = reader["Phone"].ToString();

                    // Load department dropdown
                    ddlEditDepartment.Items.Clear();
                    using (SqlCommand deptCmd = new SqlCommand("SELECT DepartmentId, DepartmentName FROM Departments", conn))
                    {
                        SqlDataReader deptReader = deptCmd.ExecuteReader();
                        ddlEditDepartment.DataSource = deptReader;
                        ddlEditDepartment.DataTextField = "DepartmentName";
                        ddlEditDepartment.DataValueField = "DepartmentId";
                        ddlEditDepartment.DataBind();
                    }

                    ddlEditDepartment.SelectedValue = reader["DepartmentId"].ToString();

                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowEditModal", "$('#editTraineeModal').modal('show');", true);
                }
            }
        }
        protected void btnUpdateTrainee_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(@"UPDATE Trainees 
        SET Name = @Name, Email = @Email, Phone = @Phone, DepartmentId = @Dept 
        WHERE Id = @Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", hfTraineeId.Value);
                cmd.Parameters.AddWithValue("@Name", txtEditName.Text.Trim());
                cmd.Parameters.AddWithValue("@Email", txtEditEmail.Text.Trim());
                cmd.Parameters.AddWithValue("@Phone", txtEditPhone.Text.Trim());
                cmd.Parameters.AddWithValue("@Dept", ddlEditDepartment.SelectedValue);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            BindGrid(); // Refresh grid
        }
        private void LoadTraineeList()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["YourConnectionStringName"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Trainees"; // You can add filters later
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                GridView1.DataSource = dt;
                GridView1.DataBind();
            }
        }
        protected void btnConfirmDelete_Click(object sender, EventArgs e)
        {
            string traineeId = hfDeleteTraineeId.Value;

            if (!string.IsNullOrEmpty(traineeId))
            {
                DeleteTraineeFromDatabase(traineeId);
                DeleteTraineeFromCache(traineeId);
                LoadTraineeList();
            }

        }
        private void DeleteTraineeFromCache(string id)
        {
            string cacheKey = $"Trainee_{id}";
            HttpRuntime.Cache.Remove(cacheKey);


        }
       

        private void ShowTraineeDetails(string id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Trainees WHERE Id = @Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    string html = $@"
                <div class='row'>
                    <div class='col-md-6'><strong>Name:</strong> {reader["Name"]}</div>
                    <div class='col-md-6'><strong>Email:</strong> {reader["Email"]}</div>
                </div>
                <div class='row mt-2'>
                    <div class='col-md-6'><strong>Phone:</strong> {reader["Phone"]}</div>
                    <div class='col-md-6'><strong>Department:</strong> {reader["DepartmentId"]}</div>
                </div>
                <div class='row mt-2'>
                    <div class='col-md-6'><strong>Joined Date:</strong> {Convert.ToDateTime(reader["JoinedDate"]).ToString("dd MMM yyyy")}</div>
                </div>";
                    litTraineeDetails.Text = html;

                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "$('#traineeModal').modal('show');", true);
                }
            }
        }
        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int traineeId = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Value);
            SoftDeleteTrainee(traineeId);
            BindGrid(); // Refresh the grid
        }
        private void SoftDeleteTrainee(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["YourConnectionStringName"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "UPDATE Trainees SET IsDeleted = 1 WHERE Id = @Id";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
        private void LoadActiveTrainees(int traineeId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["TrainingDB"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "UPDATE Trainees SET IsDeleted = 1 WHERE TraineeID = @id";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@id", traineeId);

                    con.Open();               // ✅ Only once
                    cmd.ExecuteNonQuery();    // Executes the update
                }
            }

            // ✅ Refresh GridView to show updated list
            LoadGridView();
        }
        private void LoadGridView()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["TrainingDB"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Trainees WHERE IsDeleted = 0";

                using (SqlDataAdapter da = new SqlDataAdapter(query, con))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    GridView1.DataSource = dt;
                    GridView1.DataBind();
                }
            }
        }
        private void LoadDeletedTrainees()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["TrainingDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Trainees WHERE IsDeleted = 1";
                using (SqlDataAdapter da = new SqlDataAdapter(query, con))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    GridView1.DataSource = dt;
                    GridView1.DataBind();
                }
            }
        }
        protected void btnViewActive_Click(object sender, EventArgs e)
        {
            LoadActiveTraineesList(); // ✅ Correct method, no ID needed // ✅ Pass actual ID
        }

        protected void btnViewDeleted_Click(object sender, EventArgs e)
        {
            LoadDeletedTrainees();
        }
        private void LoadTraineeCounts()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["TrainingDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT 
                (SELECT COUNT(*) FROM Trainees WHERE IsDeleted = 0) AS ActiveCount,
                (SELECT COUNT(*) FROM Trainees WHERE IsDeleted = 1) AS DeletedCount,
                (SELECT COUNT(*) FROM Trainees) AS TotalCount";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        lblActiveCount.Text = "Active Trainees: " + reader["ActiveCount"].ToString();
                        lblDeletedCount.Text = "Deleted Trainees: " + reader["DeletedCount"].ToString();
                        lblTotalCount.Text = "Total Trainees: " + reader["TotalCount"].ToString();
                    }
                }
            }
        }
        protected void ddlStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedStatus = ddlStatusFilter.SelectedValue;

            switch (selectedStatus)
            {
                case "Active":
                    LoadActiveTraineesList();   // ✅ Loads all active trainees
                    break;

                case "Deleted":
                    LoadDeletedTraineesList();  // ✅ Loads all deleted trainees
                    break;

                case "All":
                    LoadAllTrainees();          // ✅ Loads all trainees
                    break;
            }

            LoadTraineeCounts(); // ✅ Refresh counts
        }
        private void LoadActiveTraineesList()
        {
            string query = "SELECT * FROM Trainees WHERE IsDeleted = 0 AND Status = 'Active'";

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlDataAdapter da = new SqlDataAdapter(query, con))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                GridView1.DataSource = dt;
                GridView1.DataBind();
            }
        }
        private void LoadDeletedTraineesList()
        {
            string query = "SELECT * FROM Trainees WHERE IsDeleted = 1";

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlDataAdapter da = new SqlDataAdapter(query, con))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                GridView1.DataSource = dt;
                GridView1.DataBind();
            }
        }
        private void LoadAllTrainees()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["TrainingDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Trainees ORDER BY TraineeID DESC";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    GridView1.DataSource = dt;
                    GridView1.DataBind();
                }
            }

        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            string connectionString = ConfigurationManager.ConnectionStrings["TrainingDB"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT Id, Name, DepartmentId, Email, Phone, JoinedDate, IsDeleted
            FROM Trainees
            WHERE Name LIKE @keyword OR Email LIKE @keyword
            ORDER BY TraineeID DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@keyword", "%" + keyword + "%");
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    GridView1.DataSource = dt;
                    GridView1.DataBind();
                }
            }

            LoadTraineeCounts(); 
        }
        protected void btnFilterDate_Click(object sender, EventArgs e)
        {
            DateTime startDate, endDate;
            bool isStartValid = DateTime.TryParse(txtStartDate.Text, out startDate);
            bool isEndValid = DateTime.TryParse(txtEndDate.Text, out endDate);

            string connectionString = ConfigurationManager.ConnectionStrings["TrainingDB"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT Id, Name, DepartmentId, Email, Phone, JoinedDate, IsDeleted
            FROM Trainees
            WHERE (@isStartValid = 0 OR JoinedDate >= @startDate)
              AND (@isEndValid = 0 OR JoinedDate <= @endDate)
            ORDER BY TraineeID DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@isStartValid", isStartValid ? 1 : 0);
                    cmd.Parameters.AddWithValue("@isEndValid", isEndValid ? 1 : 0);
                    cmd.Parameters.AddWithValue("@startDate", startDate);
                    cmd.Parameters.AddWithValue("@endDate", endDate);

                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    GridView1.DataSource = dt;
                    GridView1.DataBind();
                }
            }

            LoadTraineeCounts(); // Optional
        }
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            string traineeId = hdnEditTraineeId.Value;
            string name = txtNameEdit.Text.Trim();
            string email = txtEmailEdit.Text.Trim();
            string phone = txtPhoneEdit.Text.Trim();
            string status = ddlStatusEdit.SelectedValue;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "UPDATE Trainees SET Name = @Name, Email = @Email, Phone = @Phone, Status = @Status WHERE TraineeId = @TraineeId";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Phone", phone);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@TraineeId", traineeId);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            BindGrid(); // Refresh the grid
            ScriptManager.RegisterStartupScript(this, GetType(), "HideEditModal", "hideEditModal();", true);
        }
    }

}