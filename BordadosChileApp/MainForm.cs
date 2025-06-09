using System;
using System.Windows.Forms;
using System.Drawing;

namespace BordadosChileApp
{
    public partial class MainForm : Form
    {
        private string rolUsuario;

        public MainForm()
        {
            InitializeComponent();
            this.BackColor = Color.WhiteSmoke;
        }

        public MainForm(string rol)
        {
            InitializeComponent();
            rolUsuario = rol;
            AplicarPermisosPorRol();
        }

        private void AplicarPermisosPorRol()
        {
            if (rolUsuario == "Empleado")
            {
                // Ejemplo: ocultar reportes y pagos
                reporteToolStripMenuItem.Visible = false;
                pagosToolStripMenuItem.Visible = false;
                configuraciónToolStripMenuItem.Visible = false;
                // O lo que prefieras
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (SesionUsuario.Rol != "admin")
            {
                empleadosToolStripMenuItem.Visible = false;
                facturasToolStripMenuItem.Visible = false;
                guiasToolStripMenuItem.Visible = false;
            }
        }

        private void cerrarSesiónToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // Cerrar sesión
            this.Hide();
            LoginForm login = new LoginForm();
            login.Show();
        }

        private void configuraciónToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (rolUsuario == "Administrador")
            {
                CambiarContraseñaForm form = new CambiarContraseñaForm(rolUsuario);
                form.ShowDialog();
            }
            else
            {
                MessageBox.Show("Solo el administrador puede acceder a esta opción.",
                                "Acceso denegado",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
            }
        }

        private void clientesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // Abre ClientesForm
            ClientesForm cf = new ClientesForm();
            cf.Show();
        }

        private void proveedoresToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // Abre ProveedoresForm
            ProveedoresForm pf = new ProveedoresForm();
            pf.Show();
        }

        private void empleadosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Abre EmpleadosForm
            EmpleadosForm ef = new EmpleadosForm();
            ef.Show();
        }

        private void pedidosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Abre PedidosForm
            PedidosForm pf = new PedidosForm();
            pf.Show();
        }

        private void inventarioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Abre InventarioForm
            InventarioForm inv = new InventarioForm();
            inv.Show();
        }

        private void reporteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Abre ReportesForm
            ReportesForm rep = new ReportesForm();
            rep.Show();
        }

        private void pagosProveedoresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Abre PagosProveedores
            PagosProveedoresForm pagosProv = new PagosProveedoresForm();
            pagosProv.Show();
        }

        private void pagosPersonalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Abre PagosPersonal
            PagosPersonalForm pagosPers = new PagosPersonalForm();
            pagosPers.Show();
        }
        private void facturasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FacturasForm().ShowDialog();
        }

        private void guiasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new GuiasForm().ShowDialog();
        }

        private void facturasToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FacturasReportesForm frm = new FacturasReportesForm();
            frm.ShowDialog();
        }

        private void guiasToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            GuiasReportesForm frm = new GuiasReportesForm();
            frm.ShowDialog();
        }
    }
}
