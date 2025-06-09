using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BordadosChileApp
{
    public partial class LoginForm : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        public LoginForm()
        {
            InitializeComponent();
            this.BackColor = Color.WhiteSmoke;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Redondear botón
            btnLogin.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnLogin.Width, btnLogin.Height, 10, 10));

            // Ocultar contraseña
            txtClave.UseSystemPasswordChar = true;
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            cmbRol.Items.Clear();
            cmbRol.Items.Add("Administrador");
            cmbRol.Items.Add("Empleado");
            cmbRol.SelectedIndex = 0;
            CentrarContenido();
        }

        private void LoginForm_Resize(object sender, EventArgs e)
        {
            CentrarContenido();
        }

        private void CentrarContenido()
        {
            panelContenidoLogin.Left = (this.ClientSize.Width - panelContenidoLogin.Width) / 2;
            panelContenidoLogin.Top = (this.ClientSize.Height - panelContenidoLogin.Height) / 2;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string clave = txtClave.Text.Trim();
            string rol = cmbRol.Text;

            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(clave))
            {
                MessageBox.Show("Por favor completa todos los campos.");
                return;
            }

            try
            {
                using (SqlConnection cn = Conexion.Conectar())
                {
                    cn.Open();
                    string query = "SELECT COUNT(*) FROM Usuarios WHERE Usuario=@usuario AND Clave=@clave AND Rol=@rol";
                    SqlCommand cmd = new SqlCommand(query, cn);
                    cmd.Parameters.AddWithValue("@usuario", usuario);
                    cmd.Parameters.AddWithValue("@clave", clave);
                    cmd.Parameters.AddWithValue("@rol", rol);

                    int count = (int)cmd.ExecuteScalar();
                    if (count > 0)
                    {
                        MessageBox.Show($"✅ Bienvenido, {rol}!");
                        MainForm mainForm = new MainForm(rol);
                        this.Hide();
                        mainForm.Show();
                    }
                    else
                    {
                        MessageBox.Show("❌ Credenciales incorrectas.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al iniciar sesión: " + ex.Message);
            }
        }

        private void chkMostrarClave_CheckedChanged(object sender, EventArgs e)
        {
            txtClave.UseSystemPasswordChar = !chkMostrarClave.Checked;
        }

        private void cmbRol_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Puedes dejarlo vacío por ahora o usarlo luego.
        }

    }
}
