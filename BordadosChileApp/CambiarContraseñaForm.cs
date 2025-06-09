// CambiarContraseñaForm.cs
using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace BordadosChileApp
{
    public partial class CambiarContraseñaForm : Form
    {
        private string rolActual;

        public CambiarContraseñaForm(string rol)
        {
            InitializeComponent();
            rolActual = rol;
            lblRol.Text = $"Rol actual: {rol}";

            if (rol != "Administrador")
            {
                btnCambiar.Enabled = false;
                MessageBox.Show("Solo el Administrador puede cambiar contraseñas.",
                                "Acceso denegado",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
            }
        }

        private void CambiarContraseñaForm_Load(object sender, EventArgs e)
        {
            cmbUsuario.Items.Add("admin");
            cmbUsuario.Items.Add("empleado");
            cmbUsuario.SelectedIndex = 0;
        }

        private void btnCambiar_Click(object sender, EventArgs e)
        {
            string usuario = cmbUsuario.Text.Trim();
            string nuevaClave = txtNuevaClave.Text.Trim();

            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(nuevaClave))
            {
                MessageBox.Show("Por favor completa todos los campos.");
                return;
            }

            try
            {
                using (SqlConnection cn = Conexion.Conectar())
                {
                    cn.Open();
                    string query = "UPDATE Usuarios SET Clave = @clave WHERE Usuario = @usuario";
                    SqlCommand cmd = new SqlCommand(query, cn);
                    cmd.Parameters.AddWithValue("@clave", nuevaClave);
                    cmd.Parameters.AddWithValue("@usuario", usuario);

                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        MessageBox.Show("✅ Contraseña actualizada correctamente");
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("❌ No se encontró el usuario o no se actualizó la contraseña.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cambiar la contraseña: " + ex.Message);
            }
        }
    }
}
