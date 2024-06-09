using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows.Forms;
using System.IO;
using System.Drawing;

namespace UserRegistrationSystem
{
    public partial class Form1 : Form
    {
        private string photoFilePath;

        public Form1()
        {
            InitializeComponent();
            LoadDesignations();
            LoadCountries();
            LoadUserData(); // Load data when form initializes

            // Handle SelectedIndexChanged event for cmbCountry
            cmbCountry.SelectedIndexChanged += cmbCountry_SelectedIndexChanged;

            // Handle TextChanged event for MobiletextBox
            this.MobiletextBox.TextChanged += new System.EventHandler(this.MobiletextBox_TextChanged);
        }

        private void label1_Click(object sender, EventArgs e)
        {
            // Handle label click event if necessary
        }

        private void LoadDesignations()
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["UserRegistration"].ConnectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT DesignationID, DesignationName FROM Designations", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                cmbDesignation.DisplayMember = "DesignationName";
                cmbDesignation.ValueMember = "DesignationID";
                cmbDesignation.DataSource = dt;
            }
        }

        private void LoadCountries()
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["UserRegistration"].ConnectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT CountryID, CountryName FROM Countries", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                cmbCountry.DisplayMember = "CountryName";
                cmbCountry.ValueMember = "CountryID";
                cmbCountry.DataSource = dt;
            }
        }

        private void LoadStates(int countryID)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["UserRegistration"].ConnectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT StateID, StateName FROM States WHERE CountryID = @CountryID", conn);
                da.SelectCommand.Parameters.AddWithValue("@CountryID", countryID);
                DataTable dt = new DataTable();
                da.Fill(dt);
                cmbState.DisplayMember = "StateName";
                cmbState.ValueMember = "StateID";
                cmbState.DataSource = dt;
            }
        }

        private void cmbCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCountry.SelectedValue != null)
            {
                int countryID = Convert.ToInt32(cmbCountry.SelectedValue);
                LoadStates(countryID);
            }
        }

        private void desiCombox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Handle designation combo box selected index change if necessary
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Handle form load event if necessary
        }

        private void Uploadbutton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files(*.jpeg; *.png)|*.jpeg; *.png";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                FileInfo fileInfo = new FileInfo(ofd.FileName);
                if (fileInfo.Length > 300 * 1024)
                {
                    MessageBox.Show("File size should not exceed 300KB.");
                    return;
                }
                photopictureBox.Image = new Bitmap(ofd.FileName);
                photoFilePath = ofd.FileName; // Store the file path to save later
            }
        }

        private void submitbutton_Click(object sender, EventArgs e)
        {
            // Validate required fields
            if (string.IsNullOrEmpty(nameTextBox.Text) ||
                string.IsNullOrEmpty(emailTextbox.Text) ||
                string.IsNullOrEmpty(MobiletextBox.Text) ||
                cmbDesignation.SelectedIndex == -1 ||
                cmbCountry.SelectedIndex == -1 ||
                cmbState.SelectedIndex == -1)
            {
                MessageBox.Show("Please fill in all required fields.");
                return;
            }

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["UserRegistration"].ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO Users (Name, Email, Mobile, DesignationID, CountryID, StateID, Photo) VALUES (@Name, @Email, @Mobile, @DesignationID, @CountryID, @StateID, @Photo)", conn);
                cmd.Parameters.AddWithValue("@Name", nameTextBox.Text);
                cmd.Parameters.AddWithValue("@Email", emailTextbox.Text);
                cmd.Parameters.AddWithValue("@Mobile", MobiletextBox.Text);
                cmd.Parameters.AddWithValue("@DesignationID", (int)cmbDesignation.SelectedValue);
                cmd.Parameters.AddWithValue("@CountryID", (int)cmbCountry.SelectedValue);
                cmd.Parameters.AddWithValue("@StateID", (int)cmbState.SelectedValue);

                if (!string.IsNullOrEmpty(photoFilePath))
                {
                    byte[] photoBytes = File.ReadAllBytes(photoFilePath);
                    cmd.Parameters.AddWithValue("@Photo", photoBytes);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Photo", DBNull.Value);
                }

                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("User registered successfully!");
                    LoadUserData(); // Refresh the DataGridView
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message);
                }
            }
        }

        private void LoadUserData()
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["UserRegistration"].ConnectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Users", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                //userDataGridView.DataSource = dt;
            }
        }

        // Event handler for MobiletextBox TextChanged event
        private void MobiletextBox_TextChanged(object sender, EventArgs e)
        {

            if (System.Text.RegularExpressions.Regex.IsMatch(MobiletextBox.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.");
                MobiletextBox.Text = MobiletextBox.Text.Remove(MobiletextBox.Text.Length - 1);
            }
        }

        private void clearbuttom_Click(object sender, EventArgs e)
        {
            nameTextBox.Clear();
            emailTextbox.Clear();
            MobiletextBox.Clear();
            cmbDesignation.SelectedIndex = -1;
            cmbState.SelectedIndex = -1;
            cmbCountry.SelectedIndex = -1;
            photopictureBox.Image = null;
        }

        private void viewUsersButton_Click(object sender, EventArgs e)
        {
            UserDataGridForm userDataGridForm = new UserDataGridForm();
            userDataGridForm.Show();
        }
    }
}
