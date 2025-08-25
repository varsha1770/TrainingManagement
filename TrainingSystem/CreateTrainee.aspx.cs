using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TrainingSystem
{
    public partial class CreateTrainee : System.Web.UI.Page
    {
        protected string connectionString = ConfigurationManager.ConnectionStrings["TrainingDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDepartments();
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

                ddlDepartment.Items.Insert(0, new ListItem("-- Select Department --", ""));
            }
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(@"INSERT INTO Trainees 
(Name, DepartmentId, MailID, Mobile, JoiningDate, Status) 
VALUES (@Name, @DepartmentId, @MailID, @Mobile, @JoiningDate, @Status)", conn);

                cmd.Parameters.AddWithValue("@Name", txtName.Text);
                cmd.Parameters.AddWithValue("@DepartmentId", ddlDepartment.SelectedValue);
                cmd.Parameters.AddWithValue("@MailID", txtEmail.Text);     // txtEmail maps to MailID
                cmd.Parameters.AddWithValue("@Mobile", txtPhone.Text);     // txtPhone maps to Mobile
                cmd.Parameters.AddWithValue("@JoiningDate", Convert.ToDateTime(txtJoinedDate.Text));
                cmd.Parameters.AddWithValue("@Status", "Active");          // or use a dropdown if needed

                conn.Open();
                cmd.ExecuteNonQuery();
                lblMessage.Text = "Trainee created successfully!";
                ClearForm();
            }
        }

        private void ClearForm()
        {
            txtName.Text = "";
            ddlDepartment.ClearSelection();
            txtEmail.Text = "";
            txtPhone.Text = "";
            txtJoinedDate.Text = "";

        }
    }
}