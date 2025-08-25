using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace TrainingSystem
{
    public partial class BulkInsert : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["TrainingDB"].ConnectionString;

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (!fileUpload.HasFile)
            {
                lblStatus.Text = "⚠️ Please select a CSV file to upload.";
                return;
            }

            try
            {
                DataTable traineeTable = new DataTable();
                traineeTable.Columns.Add("FullName");
                traineeTable.Columns.Add("Email");
                traineeTable.Columns.Add("JoinDate", typeof(DateTime));
                traineeTable.Columns.Add("Role");

                using (StreamReader reader = new StreamReader(fileUpload.FileContent))
                {
                    string line;
                    bool isHeader = true;

                    while ((line = reader.ReadLine()) != null)
                    {
                        string trimmedLine = line.Trim();
                        if (string.IsNullOrWhiteSpace(trimmedLine)) continue;

                        if (isHeader)
                        {
                            isHeader = false;
                            continue;
                        }

                        string[] fields = trimmedLine.Split(',');
                        if (fields.Length != 4) continue;

                        string name = fields[0].Trim();
                        string email = fields[1].Trim();
                        string dateStr = fields[2].Trim();
                        string role = fields[3].Trim();

                        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(role)) continue;

                        if (DateTime.TryParse(dateStr, out DateTime joinDate))
                        {
                            traineeTable.Rows.Add(name, email, joinDate, role);
                        }
                    }
                }

                if (traineeTable.Rows.Count > 0)
                {
                    ViewState["TraineeData"] = traineeTable;
                    gvPreview.DataSource = traineeTable;
                    gvPreview.DataBind();
                    gvPreview.Visible = true;
                    btnConfirmInsert.Visible = true;
                    lblStatus.Text = $"✅ Preview loaded: {traineeTable.Rows.Count} record(s).";
                }
                else
                {
                    lblStatus.Text = "⚠️ No valid records found in the file.";
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"❌ Error: {ex.Message}";
            }
        }

        protected void btnConfirmInsert_Click(object sender, EventArgs e)
        {
            DataTable traineeData = ViewState["TraineeData"] as DataTable;

            if (traineeData == null || traineeData.Rows.Count == 0)
            {
                lblStatus.Text = "⚠️ No data available to insert.";
                return;
            }

            int inserted = 0;
            int failed = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    foreach (DataRow row in traineeData.Rows)
                    {
                        string query = @"INSERT INTO BulkProfiles (FullName, Email, JoinDate, Role)
                                         VALUES (@Name, @Email, @JoinDate, @Role)";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@Name", row["FullName"]);
                            cmd.Parameters.AddWithValue("@Email", row["Email"]);
                            cmd.Parameters.AddWithValue("@JoinDate", row["JoinDate"]);
                            cmd.Parameters.AddWithValue("@Role", row["Role"]);

                            try
                            {
                                cmd.ExecuteNonQuery();
                                inserted++;
                            }
                            catch (SqlException)
                            {
                                failed++;
                            }
                        }
                    }
                }

                lblStatus.Text = $"✅ {inserted} trainee(s) inserted successfully!" +
                                 (failed > 0 ? $" ⚠️ {failed} failed due to errors." : "");

                gvPreview.Visible = false;
                btnConfirmInsert.Visible = false;
                ViewState["TraineeData"] = null;

                LoadDepartmentSummary(); // Show summary after insert
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"❌ Insert failed: {ex.Message}";
            }
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT FullName, Email, JoinDate, Role FROM BulkProfiles";
                using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                {
                    da.Fill(dt);
                }
            }

            if (dt.Rows.Count > 0)
            {
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=Trainees.csv");
                Response.Charset = "";
                Response.ContentType = "application/text";

                using (StringWriter sw = new StringWriter())
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        sw.Write(dt.Columns[i]);
                        if (i < dt.Columns.Count - 1) sw.Write(",");
                    }
                    sw.Write(sw.NewLine);

                    foreach (DataRow row in dt.Rows)
                    {
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            string value = row[i].ToString().Replace(",", " ");
                            sw.Write(value);
                            if (i < dt.Columns.Count - 1) sw.Write(",");
                        }
                        sw.Write(sw.NewLine);
                    }

                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                }
            }
            else
            {
                lblStatus.Text = "⚠️ No data available to export.";
            }
        }

        protected void btnShowSummary_Click(object sender, EventArgs e)
        {
            LoadDepartmentSummary();
        }

        private void LoadDepartmentSummary()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
    SELECT Role AS Department, COUNT(*) AS TraineeCount
    FROM BulkProfiles
    GROUP BY Role
    ORDER BY Role";

                using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                {
                    da.Fill(dt);
                }
            }

            gvDeptSummary.DataSource = dt;
            gvDeptSummary.DataBind();
            gvDeptSummary.Visible = true;
        }
    }
}