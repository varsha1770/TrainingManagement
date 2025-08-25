using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using OfficeOpenXml; // For Excel export
using iTextSharp.text;
using iTextSharp.text.pdf; // For PDF export

namespace TrainingSystem
{
    public partial class TraineeManagement : Page
    {
        private int PageSize = 10;
        private int CurrentPage
        {
            get => ViewState["Page"] != null ? Convert.ToInt32(ViewState["Page"]) : 1;
            set => ViewState["Page"] = value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDepartments();

                BindTraineeGrid();


                BindGrid();
            }
        }

        private string GetConnectionString() => ConfigurationManager.ConnectionStrings["TrainingDB"].ConnectionString;

        private void LoadDepartments()
        {
            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT Id, Name FROM Departments", con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                ddlDepartment.DataSource = dt;
                ddlDepartment.DataTextField = "Name";
                ddlDepartment.DataValueField = "Id";
                ddlDepartment.DataBind();
                ddlDepartment.Items.Insert(0, new System.Web.UI.WebControls.ListItem("All", "0"));
            }
        }

        private void BindGrid()
        {
            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            using (SqlCommand cmd = new SqlCommand("sp_GetFilteredTrainees", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // Search text
                string searchText = txtSearch.Text.Trim();
                cmd.Parameters.AddWithValue("@Search", string.IsNullOrWhiteSpace(searchText) ? DBNull.Value : (object)searchText);

                // DepartmentId (safe parse)
                if (int.TryParse(ddlDepartment.SelectedValue, out int departmentId) && departmentId > 0)
                    cmd.Parameters.AddWithValue("@DepartmentId", departmentId);
                else
                    cmd.Parameters.AddWithValue("@DepartmentId", DBNull.Value);

                // StartDate
                if (DateTime.TryParse(txtStartDate.Text, out DateTime startDate))
                    cmd.Parameters.AddWithValue("@StartDate", startDate);
                else
                    cmd.Parameters.AddWithValue("@StartDate", DBNull.Value);

                // EndDate
                if (DateTime.TryParse(txtEndDate.Text, out DateTime endDate))
                    cmd.Parameters.AddWithValue("@EndDate", endDate);
                else
                    cmd.Parameters.AddWithValue("@EndDate", DBNull.Value);

                // Paging
                cmd.Parameters.AddWithValue("@Page", CurrentPage);
                cmd.Parameters.AddWithValue("@PageSize", PageSize);

                // Execute and bind
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    GridView1.DataSource = dt;
                    GridView1.DataBind();

                    lblPageInfo.Text = $"Page {CurrentPage}";
                }
            }
        }
        private void BindTraineeGrid()
        {
            string connStr = ConfigurationManager.ConnectionStrings["TrainingDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("usp_GetTrainees", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

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
            CurrentPage = 1;
            BindGrid();
        }

        protected void ddlDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentPage = 1;
            BindGrid();
        }

        protected void btnPrevious_Click(object sender, EventArgs e)
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                BindGrid();
            }
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            CurrentPage++;
            BindGrid();
        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string traineeId = e.CommandArgument.ToString();

            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = con;
                cmd.Parameters.AddWithValue("@TraineeID", traineeId);

                if (e.CommandName == "DeleteTrainee")
                {
                    cmd.CommandText = "UPDATE Trainees SET Status = 'Deleted' WHERE TraineeID = @TraineeID";
                }
                else if (e.CommandName == "Restore")
                {
                    cmd.CommandText = "UPDATE Trainees SET Status = 'Active' WHERE TraineeID = @TraineeID";
                }
                else if (e.CommandName == "ViewDetails")
                {
                    cmd.CommandText = "SELECT Name, JoiningDate, Status, DepartmentId FROM Trainees WHERE TraineeID = @TraineeID";

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];
                        string name = row["Name"] != DBNull.Value ? row["Name"].ToString() : "N/A";
                        string joiningDate = row["JoiningDate"] != DBNull.Value
                            ? Convert.ToDateTime(row["JoiningDate"]).ToString("dd-MM-yyyy")
                            : "N/A";
                        string status = row["Status"] != DBNull.Value ? row["Status"].ToString() : "N/A";
                        string department = row["DepartmentId"] != DBNull.Value ? row["DepartmentId"].ToString() : "Unassigned";

                        lblDetails.Text = $"<strong>Name:</strong> {name}<br/>" +
                                          $"<strong>Joined:</strong> {joiningDate}<br/>" +
                                          $"<strong>Status:</strong> {status}<br/>" +
                                          $"<strong>Department:</strong> {department}";
                    }
                    else
                    {
                        lblDetails.Text = "No details found for the selected trainee.";
                    }

                    return;
                }
                else
                {
                    return;
                }

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                BindGrid(); // ✅ Refresh the GridView
            }
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            // Set license context before using EPPlus
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Trainees");

                using (SqlConnection con = new SqlConnection(GetConnectionString()))
                {
                    SqlDataAdapter da = new SqlDataAdapter("SELECT Id, Name, DepartmentName, JoinedDate FROM TraineeView", con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    ws.Cells["A1"].LoadFromDataTable(dt, true);
                }

                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename=Trainees.xlsx");
                Response.BinaryWrite(pck.GetAsByteArray());
                Response.End();
            }
        }

        protected void btnExportPDF_Click(object sender, EventArgs e)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Document doc = new Document();
                PdfWriter.GetInstance(doc, ms);
                doc.Open();

                using (SqlConnection con = new SqlConnection(GetConnectionString()))
                {
                    SqlDataAdapter da = new SqlDataAdapter("SELECT Id, Name, DepartmentName, JoinedDate FROM TraineeView", con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    PdfPTable table = new PdfPTable(dt.Columns.Count);
                    foreach (DataColumn col in dt.Columns)
                        table.AddCell(new Phrase(col.ColumnName));

                    foreach (DataRow row in dt.Rows)
                        foreach (var cell in row.ItemArray)
                            table.AddCell(new Phrase(cell.ToString()));

                    doc.Add(table);
                }

                doc.Close();
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment; filename=Trainees.pdf");
                Response.BinaryWrite(ms.ToArray());
                Response.End();
            }
        }

        protected void btnBulkInsert_Click(object sender, EventArgs e)
        {
            if (fileUpload.HasFile)
            {
                string ext = Path.GetExtension(fileUpload.FileName);
                if (ext == ".xlsx")
                {
                    using (var stream = new MemoryStream(fileUpload.FileBytes))
                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet sheet = package.Workbook.Worksheets[0];
                        DataTable dt = new DataTable();
                        dt.Columns.Add("Name");
                        dt.Columns.Add("Email");
                        dt.Columns.Add("DepartmentId");
                        dt.Columns.Add("JoinedDate");

                        for (int row = 2; row <= sheet.Dimension.End.Row; row++)
                        {
                            dt.Rows.Add(
                                sheet.Cells[row, 1].Text,
                                sheet.Cells[row, 2].Text,
                                sheet.Cells[row, 3].Text,
                                sheet.Cells[row, 4].Text
                            );
                        }

                        using (SqlConnection con = new SqlConnection(GetConnectionString()))
                        {
                            SqlBulkCopy bulk = new SqlBulkCopy(con);
                            bulk.DestinationTableName = "Trainees";
                            con.Open();
                            bulk.WriteToServer(dt);
                            con.Close();
                        }

                        lblStatus.Text = "Bulk insert successful.";
                        BindGrid();
                    }
                }
                else
                {
                    lblStatus.Text = "Only .xlsx files are supported.";
                }
            }
            else
            {
                lblStatus.Text = "Please select a file.";
            }
        }
    }
}