using System;
using System.Data.SqlClient;
using System.Configuration;

namespace TrainingApp
{
    public partial class Dashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDashboardStats();
            }
        }

        private void LoadDashboardStats()
        {
            string connStr = ConfigurationManager.ConnectionStrings["TrainingDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                // Total Trainees
                using (SqlCommand cmd1 = new SqlCommand("SELECT COUNT(*) FROM Trainees", conn))
                {
                    int totalTrainees = (int)cmd1.ExecuteScalar();
                    lblTotalTrainees.Text = "Total Trainees: " + totalTrainees;
                }

                // Total Departments
                using (SqlCommand cmd2 = new SqlCommand("SELECT COUNT(*) FROM Departments", conn))
                {
                    int totalDepartments = (int)cmd2.ExecuteScalar();
                    lblTotalDepartments.Text = "Total Departments: " + totalDepartments;
                }

                // Trainees Joined in Last 7 Days
                using (SqlCommand cmd3 = new SqlCommand(@"
            SELECT COUNT(*)  
            FROM Trainees  
            WHERE JoiningDate IS NOT NULL  
              AND DATEDIFF(DAY, JoiningDate, GETDATE()) <= 7", conn))
                {
                    int recentJoinees = (int)cmd3.ExecuteScalar();
                    lblRecentJoinees.Text = "Joined in Last 7 Days: " + recentJoinees;
                }
            }
        }
        protected void btnAddTrainee_Click(object sender, EventArgs e)
        {
            Response.Redirect("AddTrainee.aspx");
        }

        protected void btnManageTrainees_Click(object sender, EventArgs e)
        {
            Response.Redirect("ViewTrainees.aspx");

        }

        protected void btnBulkInsert_Click(object sender, EventArgs e)
        {
            Response.Redirect("BulkInsert.aspx");
        }

        protected void btnExportCSV_Click(object sender, EventArgs e)
        {
            Response.Redirect("ExportCSV.aspx");
        }
    }
}