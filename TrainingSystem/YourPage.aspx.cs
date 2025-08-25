using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace TrainingSystem
{
    public partial class YourPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGrid();
                LoadTraineeGrid();

            }

        }
        protected void ddlStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGrid(); // Reloads GridView based on selected status
        }
        private void BindGrid()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["TrainingDB"].ConnectionString;
            string filter = ddlStatusFilter.SelectedValue;
            string query = "SELECT * FROM Trainees";

            if (filter != "All")
            {
                query += " WHERE Status = @Status";
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                if (filter != "All")
                {
                    cmd.Parameters.AddWithValue("@Status", filter);
                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                GridView1.DataSource = dt;
                GridView1.DataBind();
            }
        }
        protected void btnTraineeSearch_Click(object sender, EventArgs e)
        {
            LoadTraineeGrid(); // Calls the updated grid binding method
        }
        protected void btnDateFilter_Click(object sender, EventArgs e)
        {
            LoadTraineeGrid(); // Reuse your existing logic
        }
        private void LoadTraineeGrid()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["TrainingDB"].ConnectionString;
            string statusFilter = ddlStatusFilter.SelectedValue;
            string keyword = txtTraineeSearch.Text.Trim();
            string startDate = txtStartDate.Text.Trim();
            string endDate = txtEndDate.Text.Trim();

            string query = "SELECT * FROM Trainees WHERE 1=1";

            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                query += " AND RegistrationDate BETWEEN @StartDate AND @EndDate";
            }

            if (statusFilter != "All")
            {
                query += " AND Status = @Status";
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                query += " AND (Name LIKE @Keyword OR TraineeId LIKE @Keyword)";
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, con);

                if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                {
                    cmd.Parameters.AddWithValue("@StartDate", startDate);
                    cmd.Parameters.AddWithValue("@EndDate", endDate);
                }

                if (statusFilter != "All")
                {
                    cmd.Parameters.AddWithValue("@Status", statusFilter);
                }

                if (!string.IsNullOrEmpty(keyword))
                {
                    cmd.Parameters.AddWithValue("@Keyword", "%" + keyword + "%");
                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                GridView1.DataSource = dt;
                GridView1.DataBind();
            }
        }
    }
}