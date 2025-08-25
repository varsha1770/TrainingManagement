using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using OfficeOpenXml;
using System.Web.UI.WebControls;
using System.Web.UI;
using OfficeOpenXml.Style;
using System.Collections.Generic;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;
using System.Drawing;
using System.Collections;

namespace TrainingSystem
{
    public partial class TraineeDashboard : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["TrainingDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadTraineeGrid();
                LoadDashboardSummary();
                BindGrid();
                btnFilter_Click(null, null);


            }
        }

        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridView1.PageSize = int.Parse(ddlPageSize.SelectedValue);
            LoadTraineeGrid();
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            btnFilter_Click(null, null); // Rebind with current filters
        }

        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            DataTable dt = GetFilteredTrainees(); // Use your current filter logic

            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Trainees");
                ws.Cells["A1"].LoadFromDataTable(dt, true);
                ws.Cells.AutoFitColumns();

                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename=Trainees.xlsx");
                Response.BinaryWrite(pck.GetAsByteArray());
                Response.End();
            }
        }
        private DataTable GetFilteredTrainees()
        {
            string query = "SELECT * FROM Trainees WHERE Name LIKE @Name AND Status = @Status";
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["TrainingDB"].ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@Name", "%" + txtTraineeSearch.Text.Trim() + "%");
                cmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        public string GetStatusClass(string status)
        {
            switch (status)
            {
                case "Active": return "badge bg-success text-white";
                case "Inactive": return "badge bg-secondary text-white";
                case "Pending": return "badge bg-warning text-dark";
                default: return "badge bg-light text-dark";
            }
        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ViewDetails")
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                string traineeId = GridView1.DataKeys[rowIndex].Value.ToString();

                LoadTraineeDetails(traineeId); // Fetch full details from DB

                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowModal", "$('#traineeModal').modal('show');", true);
            }
        }
        private void LoadTraineeDetails(string traineeId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT Name, RegistrationDate, Status FROM Trainees WHERE TraineeID = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", traineeId);
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    string name = reader["Name"].ToString();
                    string joiningDate = Convert.ToDateTime(reader["RegistrationDate"]).ToString("dd-MM-yyyy");
                    string status = reader["Status"].ToString();

                    lblTraineeDetails.Text = $"ID: {traineeId}<br/>Name: {name}<br/>Joining Date: {joiningDate}<br/>Status: {status}";
                    btnSoftDelete.Visible = status != "Deleted";
                    btnRestore.Visible = status == "Deleted";
                }
            }
        }
        protected void btnRestore_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int traineeId = Convert.ToInt32(btn.CommandArgument);

            string query = "UPDATE Trainees SET IsDeleted = 0 WHERE Id = @Id";

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["TrainingDB"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", traineeId);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }
        protected void btnSoftDelete_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string traineeID = btn.CommandArgument;

            if (!string.IsNullOrEmpty(traineeID))
            {
                SoftDeleteTrainee(traineeID);
                lblMessage.Text = "Trainee soft-deleted successfully.";
                BindTraineeGrid();
            }
            else
            {
                lblMessage.Text = "Trainee ID is missing.";
            }
        }
        protected void ddlStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            string status = ddlStatusFilter.SelectedValue;
            string query = "";

            if (status == "All")
                query = "SELECT Id, Name, JoinedDate, DepartmentId, IsDeleted FROM Trainees";
            else
                query = "SELECT Id, Name, JoinedDate, DepartmentId FROM Trainees WHERE IsDeleted = @IsDeleted";

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["TrainingDB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    if (status != "All")
                        cmd.Parameters.AddWithValue("@IsDeleted", status);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    traineeGrid.DataSource = dt;
                    traineeGrid.DataBind();
                }
            }
        }
        private void BindTraineeGrid()
        {
            string query = "SELECT * FROM Trainees WHERE IsDeleted = 0";

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["TrainingDB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        traineeGrid.DataSource = dt;
                        traineeGrid.DataBind();
                    }
                }
            }
        }

        private void SoftDeleteTrainee(string traineeID)
        {
            string query = "UPDATE Trainees SET IsDeleted = 1 WHERE TraineeID = @TraineeID";

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["TrainingDB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@TraineeID", traineeID);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
        private void UpdateTraineeStatus(int traineeId, string newStatus)
        {
            string query = "UPDATE Trainees SET Status = @Status WHERE TraineeId = @TraineeId";
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["TrainingDB"].ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@Status", newStatus);
                cmd.Parameters.AddWithValue("@TraineeId", traineeId);
                con.Open();
                cmd.ExecuteNonQuery();
            }

            BindGrid();
        }

        private DataTable GetTraineeDetailsById(string traineeId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["TrainingDB"].ConnectionString;
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT Name, Email, Status FROM Trainees WHERE TraineeId = @TraineeId";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@TraineeId", traineeId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            return dt;
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["TrainingDB"].ConnectionString;
            string traineeId = ((Button)sender).CommandArgument;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("UPDATE Trainees SET IsDeleted = 1 WHERE TraineeId = @TraineeId", con);
                cmd.Parameters.AddWithValue("@TraineeId", traineeId);
                con.Open();
                cmd.ExecuteNonQuery();
            }

            LoadTraineeGrid();
        }

        private void LoadTraineeGrid()
        {
            string query = "SELECT TraineeId, Name, MailID, Mobile, Status, JoiningDate FROM Trainees WHERE Status != 'Deleted'";
            List<SqlParameter> parameters = new List<SqlParameter>();

            if (ddlStatusFilter.SelectedValue != "All")
            {
                query += " AND Status = @Status";
                parameters.Add(new SqlParameter("@Status", ddlStatusFilter.SelectedValue));
            }

            if (!string.IsNullOrEmpty(txtStartDate.Text))
            {
                query += " AND JoiningDate >= @StartDate";
                parameters.Add(new SqlParameter("@StartDate", txtStartDate.Text));
            }

            if (!string.IsNullOrEmpty(txtEndDate.Text))
            {
                query += " AND JoiningDate <= @EndDate";
                parameters.Add(new SqlParameter("@EndDate", txtEndDate.Text));
            }

            if (!string.IsNullOrEmpty(txtTraineeSearch.Text))
            {
                query += " AND (Name LIKE @Search OR CAST(TraineeId AS VARCHAR) LIKE @Search)";
                parameters.Add(new SqlParameter("@Search", "%" + txtTraineeSearch.Text + "%"));
            }

            string connectionString = ConfigurationManager.ConnectionStrings["TrainingDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddRange(parameters.ToArray());
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    GridView1.DataSource = dt;
                    GridView1.DataBind();
                }
            }
        }
        private void LoadDashboardSummary()
        {
            // Defensive check for missing connection string
            var connSetting = ConfigurationManager.ConnectionStrings["TrainingDB"];
            if (connSetting == null)
            {
                lblTotal.Text = "Error: Connection string 'TraineeDB' not found.";
                return;
            }

            string connectionString = connSetting.ConnectionString;

            string query = @"
        SELECT
            COUNT(*) AS TotalTrainees,
            SUM(CASE WHEN Status = 'Active' AND IsDeleted = 0 THEN 1 ELSE 0 END) AS ActiveTrainees,
            SUM(CASE WHEN Status = 'Inactive' AND IsDeleted = 0 THEN 1 ELSE 0 END) AS InactiveTrainees,
            SUM(CASE WHEN Status = 'Pending' AND IsDeleted = 0 THEN 1 ELSE 0 END) AS PendingTrainees,
            SUM(CASE WHEN IsDeleted = 1 THEN 1 ELSE 0 END) AS DeletedTrainees
        FROM Trainees";

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            lblTotal.Text = "Total: " + reader["TotalTrainees"].ToString();
                            lblActive.Text = "Active: " + reader["ActiveTrainees"].ToString();
                            lblInactive.Text = "Inactive: " + reader["InactiveTrainees"].ToString();
                            lblPending.Text = "Pending: " + reader["PendingTrainees"].ToString();
                            lblDeleted.Text = "Deleted: " + reader["DeletedTrainees"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblTotal.Text = "Error loading dashboard: " + ex.Message;
            }
        }
        protected void btnExportPDF_Click(object sender, EventArgs e)
        {
            DataTable dt = GetFilteredTrainees(); // Same filter logic

            Document doc = new Document(PageSize.A4);
            MemoryStream ms = new MemoryStream();
            PdfWriter.GetInstance(doc, ms);
            doc.Open();

            PdfPTable table = new PdfPTable(dt.Columns.Count);
            foreach (DataColumn col in dt.Columns)
            {
                table.AddCell(new Phrase(col.ColumnName));
            }

            foreach (DataRow row in dt.Rows)
            {
                foreach (var cell in row.ItemArray)
                {
                    table.AddCell(new Phrase(cell.ToString()));
                }
            }

            doc.Add(table);
            doc.Close();

            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment; filename=Trainees.pdf");
            Response.BinaryWrite(ms.ToArray());
            Response.End();
        }


        private void ShowTraineeDetails(int traineeId)
        {
            string query = "SELECT Name, MailID, Mobile, Status FROM Trainees WHERE TraineeId = @TraineeId";

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["TrainingDB"].ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@TraineeId", traineeId);
                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    txtName.Text = reader["Name"].ToString();
                    txtEmail.Text = reader["MailID"].ToString();   // ✅ Match actual column name
                    txtPhone.Text = reader["Mobile"].ToString();   // ✅ Match actual column name
                    ddlStatus.SelectedValue = reader["Status"].ToString();
                    hfTraineeId.Value = traineeId.ToString();
                }
            }

            // ✅ Trigger Bootstrap modal
            ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "$('#myModal').modal('show');", true);
        }
        protected void btnSaveChanges_Click(object sender, EventArgs e)
        {
            int traineeId = Convert.ToInt32(hfTraineeId.Value);
            string query = "UPDATE Trainees SET Name = @Name, Email = @Email, Phone = @Phone, Status = @Status WHERE TraineeId = @TraineeId";

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["TrainingDB"].ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@Name", txtName.Text.Trim());
                cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                cmd.Parameters.AddWithValue("@Phone", txtPhone.Text.Trim());
                cmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue);
                cmd.Parameters.AddWithValue("@TraineeId", traineeId);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            BindGrid();
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Trainee details updated successfully.');", true);
        }
        private void BindGrid()
        {
            string query = "SELECT * FROM Trainees";
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["TrainingDB"].ConnectionString))
            using (SqlDataAdapter da = new SqlDataAdapter(query, con))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                GridView1.DataSource = dt;
                GridView1.DataBind();
            }
        }
        protected void btnFilter_Click(object sender, EventArgs e)
        {
            DateTime startDate, endDate;

            // Validate Start Date
            if (!DateTime.TryParse(txtStartDate.Text.Trim(), out startDate))
            {
                lblMessage.Text = "Please enter a valid Start Date.";
                lblMessage.Visible = true;
                return;
            }

            // Validate End Date
            if (!DateTime.TryParse(txtEndDate.Text.Trim(), out endDate))
            {
                lblMessage.Text = "Please enter a valid End Date.";
                lblMessage.Visible = true;
                return;
            }

            string query = @"
        SELECT * FROM Trainees
        WHERE IsDeleted = 0
        AND JoiningDate BETWEEN @StartDate AND @EndDate";

            if (!string.IsNullOrEmpty(ddlStatus.SelectedValue))
            {
                query += " AND Status = @Status";
            }

            string connectionString = ConfigurationManager.ConnectionStrings["TrainingDB"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@StartDate", startDate);
                    cmd.Parameters.AddWithValue("@EndDate", endDate);

                    if (!string.IsNullOrEmpty(ddlStatus.SelectedValue))
                    {
                        cmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue);
                    }

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    GridView1.DataSource = dt;
                    GridView1.DataBind();

                    lblMessage.Visible = dt.Rows.Count == 0;
                    lblMessage.Text = dt.Rows.Count == 0 ? "No trainees found for the selected filters." : "";
                }

                // Summary count query
                string countQuery = @"
            SELECT Status, COUNT(*) AS Count 
            FROM Trainees 
            WHERE IsDeleted = 0 
            GROUP BY Status";

                using (SqlCommand countCmd = new SqlCommand(countQuery, con))
                {
                    SqlDataAdapter countDa = new SqlDataAdapter(countCmd);
                    DataTable countDt = new DataTable();
                    countDa.Fill(countDt);

                    int active = 0, inactive = 0, pending = 0;

                    foreach (DataRow row in countDt.Rows)
                    {
                        string status = row["Status"].ToString();
                        int count = Convert.ToInt32(row["Count"]);

                        if (status == "Active") active = count;
                        else if (status == "Inactive") inactive = count;
                        else if (status == "Pending") pending = count;
                    }

                    lblActiveCount.Text = $"Active: {active}";
                    lblInactiveCount.Text = $"Inactive: {inactive}";
                    lblPendingCount.Text = $"Pending: {pending}";
                }
            }
        }

    }

}