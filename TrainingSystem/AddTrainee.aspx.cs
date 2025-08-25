using System;
using System.Data.SqlClient;
using System.Configuration;

namespace TrainingApp
{
    public partial class AddTrainee : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDepartments();
            }
        }

        private void LoadDepartments()
        {
            string connStr = ConfigurationManager.ConnectionStrings["TrainingDB"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT Id, Name FROM Departments";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                ddlDepartment.DataSource = reader;
                ddlDepartment.DataTextField = "Name";
                ddlDepartment.DataValueField = "Id";
                ddlDepartment.DataBind();
                ddlDepartment.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Select Department --", "0"));
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            lblMessage.Text = ""; // Clear previous messages

            // Validate inputs
            string name = txtName.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                lblMessage.Text = "Please enter trainee name.";
                return;
            }

            DateTime joinDate;
            if (!DateTime.TryParse(txtJoinDate.Text, out joinDate))
            {
                lblMessage.Text = "Please enter a valid join date.";
                return;
            }

            if (ddlDepartment.SelectedValue == "0")
            {
                lblMessage.Text = "Please select a department.";
                return;
            }

            int departmentId = int.Parse(ddlDepartment.SelectedValue);

            // Insert into database
            string connStr = ConfigurationManager.ConnectionStrings["TrainingDB"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "INSERT INTO Trainees (Name, JoiningDate, DepartmentId) VALUES (@Name, @JoiningDate, @DepartmentId)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@JoiningDate", joinDate);
                cmd.Parameters.AddWithValue("@DepartmentId", departmentId);

                try
                {
                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        lblMessage.Text = "✅ Trainee added successfully!";
                        txtName.Text = "";
                        txtJoinDate.Text = "";
                        ddlDepartment.ClearSelection();
                    }
                    else
                    {
                        lblMessage.Text = "❌ Failed to add trainee.";
                    }
                }
                catch (Exception ex)
                {
                    lblMessage.Text = "⚠️ Error: " + ex.Message;
                }
            }
        }
    }
}