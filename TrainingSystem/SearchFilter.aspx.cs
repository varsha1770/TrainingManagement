using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace TrainingApp
{
    public partial class SearchFilter : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["TrainingDB"].ConnectionString;

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";
            string name = txtSearchName.Text.Trim();
            string fromDate = txtFromDate.Text.Trim();
            string toDate = txtToDate.Text.Trim();

            string query = @"SELECT T.TraineeID, T.Name, T.JoiningDate, D.Name AS DepartmentName 
                 FROM Trainees T 
                 INNER JOIN Departments D ON T.DepartmentId = D.Id 
                 WHERE 1=1";

            SqlCommand cmd = new SqlCommand();
            if (!string.IsNullOrEmpty(name))
            {
                query += " AND T.Name LIKE @Name";
                cmd.Parameters.AddWithValue("@Name", "%" + name + "%");
            }

            if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(toDate))
            {
                query += "AND T.JoiningDate BETWEEN @FromDate AND @ToDate";
                cmd.Parameters.AddWithValue("@FromDate", DateTime.Parse(fromDate));
                cmd.Parameters.AddWithValue("@ToDate", DateTime.Parse(toDate));
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                cmd.Connection = conn;
                cmd.CommandText = query;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    gvResults.DataSource = dt;
                    gvResults.DataBind();
                }
                else
                {
                    gvResults.DataSource = null;
                    gvResults.DataBind();
                    lblMessage.Text = "No trainees found for the given criteria.";
                }
            }
        }
    }
}