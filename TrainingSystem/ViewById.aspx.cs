using System;
using System.Data.SqlClient;
using System.Configuration;

namespace TrainingApp
{
    public partial class ViewById : System.Web.UI.Page
    {
        protected void btnFetch_Click(object sender, EventArgs e)
        {
            lblResult.Text = ""; // Clear previous result

            int traineeId;
            if (!int.TryParse(txtTraineeId.Text.Trim(), out traineeId))
            {
                lblResult.Text = "⚠️ Please enter a valid numeric Trainee ID.";
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["TrainingDB"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("GetTraineeById", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TraineeID", traineeId); // ✅ Correct parameter name

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string name = reader["Name"].ToString();
                    string joinedDate = Convert.ToDateTime(reader["JoiningDate"]).ToString("yyyy-MM-dd");
                    string status = reader["Status"].ToString();

                    lblResult.Text = $"👤 <b>Name:</b> {name}<br/>" +
                                     $"📅 <b>Joined:</b> {joinedDate}<br/>" +
                                     $"✅ <b>Status:</b> {status}";
                }
                else
                {
                    lblResult.Text = "❌ No trainee found with that ID.";
                }
            }
        }
    }
}