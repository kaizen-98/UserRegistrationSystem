using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows.Forms;

namespace UserRegistrationSystem
{
    public partial class UserDataGridForm : Form
    {
        public UserDataGridForm()
        {
            InitializeComponent();
        }

        private void UserDataGridForm_Load(object sender, EventArgs e)
        {
            LoadUserData();
            AddButtonsToDataGridView();
        }

        private void LoadUserData()
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["UserRegistration"].ConnectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Users", conn); 
                DataTable dt = new DataTable();
                da.Fill(dt);
                userDataGridView.DataSource = dt;

                // Disable Email column if email editing is not allowed
                userDataGridView.Columns["Email"].ReadOnly = true;
            }
        }

        private void AddButtonsToDataGridView()
        {
            DataGridViewButtonColumn editButtonColumn = new DataGridViewButtonColumn();
            editButtonColumn.Name = "Edit";
            editButtonColumn.Text = "Edit";
            editButtonColumn.UseColumnTextForButtonValue = true;
            userDataGridView.Columns.Add(editButtonColumn);

            DataGridViewButtonColumn deleteButtonColumn = new DataGridViewButtonColumn();
            deleteButtonColumn.Name = "Delete";
            deleteButtonColumn.Text = "Delete";
            deleteButtonColumn.UseColumnTextForButtonValue = true;
            userDataGridView.Columns.Add(deleteButtonColumn);
        }

        private void userDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == userDataGridView.Columns["Edit"].Index && e.RowIndex >= 0)
            {
                // Handle Edit button click
                int userID = Convert.ToInt32(userDataGridView.Rows[e.RowIndex].Cells["UserID"].Value);
                EditUser(userID);
            }
            else if (e.ColumnIndex == userDataGridView.Columns["Delete"].Index && e.RowIndex >= 0)
            {
                // Handle Delete button click
                int userID = Convert.ToInt32(userDataGridView.Rows[e.RowIndex].Cells["UserID"].Value);
                DeleteUser(userID);
            }
        }

        private void EditUser(int userID)
        {
            // Implement the logic to edit user
            // You might want to open a new form or enable editing in the DataGridView
            MessageBox.Show($"Edit user with ID: {userID}");
        }

        private void DeleteUser(int userID)
        {
            var result = MessageBox.Show("Are you sure you want to delete this user?", "Confirm Deletion", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["UserRegistration"].ConnectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM Users WHERE UserID = @UserID", conn);
                    cmd.Parameters.AddWithValue("@UserID", userID);
                    cmd.ExecuteNonQuery();
                }
                LoadUserData(); // Refresh the DataGridView
            }
        }

        // Ensure this method exists to handle user actions
        private void UserDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Handle any cell content click events here if necessary
        }
    }
}
