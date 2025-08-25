using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;
using System.Collections.Generic;

namespace TrainingApp
{
    public partial class ViewTrainees : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["TrainingDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadTrainees();
            }
        }

        private void LoadTrainees()
        {
            string query = "SELECT TraineeID, Name, JoiningDate, DepartmentId FROM Trainees";
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                gvTrainees.DataSource = dt;
                gvTrainees.DataBind();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string idFilter = txtSearchId.Text.Trim();
            string nameFilter = txtSearchName.Text.Trim();
            string deptFilter = txtSearchDept.Text.Trim();

            string query = "SELECT TraineeID, Name, JoiningDate, DepartmentId FROM Trainees WHERE 1=1";
            var parameters = new List<SqlParameter>();

            if (int.TryParse(idFilter, out int traineeId))
            {
                query += " AND TraineeID = @TraineeID";
                parameters.Add(new SqlParameter("@TraineeID", traineeId));
            }
            else if (!string.IsNullOrEmpty(idFilter))
            {
                lblMessage.Text = "❌ Invalid Trainee ID format.";
                gvTrainees.DataSource = null;
                gvTrainees.DataBind();
                return;
            }

            if (!string.IsNullOrEmpty(nameFilter))
            {
                query += " AND Name LIKE @Name";
                parameters.Add(new SqlParameter("@Name", "%" + nameFilter + "%"));
            }

            if (int.TryParse(deptFilter, out int deptId))
            {
                query += " AND DepartmentId = @DeptId";
                parameters.Add(new SqlParameter("@DeptId", deptId));
            }
            else if (!string.IsNullOrEmpty(deptFilter))
            {
                lblMessage.Text = "❌ Invalid Department ID format.";
                gvTrainees.DataSource = null;
                gvTrainees.DataBind();
                return;
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddRange(parameters.ToArray());
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvTrainees.DataSource = dt;
                    gvTrainees.DataBind();

                    lblMessage.Text = dt.Rows.Count > 0 ? "" : "⚠️ No matching trainees found.";
                }
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtSearchId.Text = "";
            txtSearchName.Text = "";
            txtSearchDept.Text = "";
            lblMessage.Text = "";
            LoadTrainees();
        }

        protected void gvTrainees_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvTrainees.EditIndex = e.NewEditIndex;
            LoadTrainees();
        }

        protected void gvTrainees_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvTrainees.EditIndex = -1;
            LoadTrainees();
        }

        protected void gvTrainees_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            GridViewRow row = gvTrainees.Rows[e.RowIndex];
            int traineeId = Convert.ToInt32(gvTrainees.DataKeys[e.RowIndex].Value);

            TextBox txtName = (TextBox)row.FindControl("txtName");
            TextBox txtJoinDate = (TextBox)row.FindControl("txtJoinDate");
            TextBox txtDeptId = (TextBox)row.FindControl("txtDeptId");

            string name = txtName?.Text.Trim() ?? "";
            string dateText = txtJoinDate?.Text.Trim() ?? "";
            string deptText = txtDeptId?.Text.Trim() ?? "";

            if (!DateTime.TryParse(dateText, out DateTime joiningDate))
            {
                lblMessage.Text = "❌ Invalid date format. Use yyyy-MM-dd.";
                return;
            }

            if (!int.TryParse(deptText, out int departmentId))
            {
                lblMessage.Text = "❌ Invalid Department ID.";
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand(@"
                    UPDATE Trainees
                    SET Name = @Name,
                        JoiningDate = @JoiningDate,
                        DepartmentId = @DepartmentId
                    WHERE TraineeID = @TraineeID", conn))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@JoiningDate", joiningDate);
                    cmd.Parameters.AddWithValue("@DepartmentId", departmentId);
                    cmd.Parameters.AddWithValue("@TraineeID", traineeId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    lblMessage.Text = "✅ Trainee updated successfully.";
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "❌ Update failed: " + ex.Message;
            }

            gvTrainees.EditIndex = -1;
            LoadTrainees();
        }

        protected void gvTrainees_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int traineeId = Convert.ToInt32(gvTrainees.DataKeys[e.RowIndex].Value);

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand("DELETE FROM Trainees WHERE TraineeID = @TraineeID", conn))
                {
                    cmd.Parameters.AddWithValue("@TraineeID", traineeId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    lblMessage.Text = "✅ Trainee deleted successfully.";
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "❌ Delete failed: " + ex.Message;
            }

            LoadTrainees();
        }
    }
}