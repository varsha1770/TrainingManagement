using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace TrainingApp
{
    public partial class Login : Page
    {
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (AuthenticateUser(username, password))
                Response.Redirect("Dashboard.aspx");
            else
                lblMessage.Text = "Invalid username or password.";
        }

        private bool AuthenticateUser(string username, string password)
        {
            string connStr = ConfigurationManager.ConnectionStrings["TrainingDB"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"SELECT COUNT(*) 
                 FROM AccountCredentials 
                 WHERE LOWER(UserLogin) = LOWER(@UserLogin) 
                   AND LOWER(UserSecret) = LOWER(@UserSecret)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add("@UserLogin", SqlDbType.VarChar).Value = username;
                cmd.Parameters.Add("@UserSecret", SqlDbType.VarChar).Value = password;

                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count == 1;
            }
        }
    }
}