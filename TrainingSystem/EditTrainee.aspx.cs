using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TrainingSystem
{
    public partial class EditTrainee : System.Web.UI.Page
    {
        protected string connectionString = ConfigurationManager.ConnectionStrings["TrainingDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            string traineeId = Request.QueryString["traineeId"]; // ✅ Consistent key

            if (!IsPostBack)
            {
                LoadDepartments();

                if (!string.IsNullOrWhiteSpace(traineeId))
                {
                    lblError.Text = "";
                    lblDebug.Text = "✅ Query ID: [" + traineeId + "]";
                    LoadTraineeDetails(traineeId);
                }
                else
                {
                    lblError.Text = "❌ Error: Missing trainee ID in query string.";
                    lblDebug.Text = "⚠️ Query ID is null or empty.";
                }
            }
        }

        private void LoadDepartments()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT Id, Name FROM Departments", conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                ddlDepartment.DataSource = reader;
                ddlDepartment.DataTextField = "Name";
                ddlDepartment.DataValueField = "Id";
                ddlDepartment.DataBind();
            }
        }

        private void LoadTraineeDetails(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                lblMessage.Text = "Invalid trainee ID.";
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT Name, DepartmentId, MailID, Mobile, JoiningDate, Status 
                                 FROM Trainees 
                                 WHERE TraineeID = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtName.Text = reader["Name"].ToString();
                            ddlDepartment.SelectedValue = reader["DepartmentId"].ToString();
                            txtEmail.Text = reader["MailID"].ToString();
                            txtPhone.Text = reader["Mobile"].ToString();

                            txtJoinedDate.Text = reader["JoiningDate"] != DBNull.Value
                                ? Convert.ToDateTime(reader["JoiningDate"]).ToString("yyyy-MM-dd")
                                : "";

                            ddlStatus.SelectedValue = reader["Status"].ToString(); // ✅ Added
                        }
                        else
                        {
                            lblMessage.Text = "Trainee not found.";
                        }
                    }
                }
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            string traineeId = Request.QueryString["traineeId"]; // ✅ Match key

            if (string.IsNullOrWhiteSpace(traineeId))
            {
                lblMessage.Text = "Error: Missing trainee ID in query string.";
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(@"
                    UPDATE Trainees SET 
                        Name = @Name, 
                        DepartmentId = @DepartmentId, 
                        MailID = @MailID, 
                        Mobile = @Mobile, 
                        JoiningDate = @JoiningDate, 
                        Status = @Status 
                    WHERE TraineeID = @Id", conn);

                string mailId = string.IsNullOrWhiteSpace(txtEmail.Text) ? "unknown@example.com" : txtEmail.Text;
                string status = string.IsNullOrWhiteSpace(ddlStatus.SelectedValue) ? "Active" : ddlStatus.SelectedValue;

                cmd.Parameters.AddWithValue("@Name", txtName.Text);
                cmd.Parameters.AddWithValue("@DepartmentId", ddlDepartment.SelectedValue);
                cmd.Parameters.AddWithValue("@MailID", mailId);
                cmd.Parameters.AddWithValue("@Mobile", txtPhone.Text);
                cmd.Parameters.AddWithValue("@JoiningDate", Convert.ToDateTime(txtJoinedDate.Text));
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@Id", traineeId);

                conn.Open();
                cmd.ExecuteNonQuery();
                lblMessage.Text = "✅ Trainee updated successfully!";
            }
        }
    }
}