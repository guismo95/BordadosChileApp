using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using DrawingFont = System.Drawing.Font;

namespace BordadosChileApp
{
    public partial class ProveedoresForm : Form
    {
        public ProveedoresForm()
        {
            InitializeComponent();
            this.BackColor = Color.WhiteSmoke;
            EstiloTabla(dgvProveedores);
            MostrarProveedores();
        }

        private void MostrarProveedores()
        {
            using (SqlConnection cn = Conexion.Conectar())
            {
                cn.Open();
                string query = "SELECT * FROM Proveedores ORDER BY NombreProveedor";
                SqlDataAdapter da = new SqlDataAdapter(query, cn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvProveedores.DataSource = dt;
            }
        }

        private void Limpiar()
        {
            txtNombreProveedor.Clear();
            txtDireccion.Clear();
            txtTelefono.Clear();
            txtCorreo.Clear();
            txtNombreContacto.Clear();
        }

        private void EstiloTabla(DataGridView dgv)
        {
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 204, 0);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgv.ColumnHeadersDefaultCellStyle.Font = new DrawingFont("Segoe UI", 10, FontStyle.Bold);

            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.ForeColor = Color.Black;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 229, 127);
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgv.DefaultCellStyle.Font = new DrawingFont("Segoe UI", 9);

            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgv.RowTemplate.Height = 28;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.BorderStyle = BorderStyle.None;
            dgv.GridColor = Color.LightGray;
        }



        private void btnGuardar_Click(object sender, EventArgs e)
        {
            using (SqlConnection cn = Conexion.Conectar())
            {
                cn.Open();
                string query = @"INSERT INTO Proveedores (NombreProveedor, Direccion, Telefono, Correo, NombreContacto)
                                 VALUES (@Nombre, @Direccion, @Telefono, @Correo, @Contacto)";
                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@Nombre", txtNombreProveedor.Text);
                cmd.Parameters.AddWithValue("@Direccion", txtDireccion.Text);
                cmd.Parameters.AddWithValue("@Telefono", txtTelefono.Text);
                cmd.Parameters.AddWithValue("@Correo", txtCorreo.Text);
                cmd.Parameters.AddWithValue("@Contacto", txtNombreContacto.Text);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("✅ Proveedor guardado correctamente.");
            Limpiar();
            MostrarProveedores();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvProveedores.CurrentRow != null)
            {
                int id = Convert.ToInt32(dgvProveedores.CurrentRow.Cells["Id"].Value);
                using (SqlConnection cn = Conexion.Conectar())
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM Proveedores WHERE Id = @Id", cn);
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("🗑️ Proveedor eliminado.");
                MostrarProveedores();
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvProveedores.CurrentRow != null)
            {
                int id = Convert.ToInt32(dgvProveedores.CurrentRow.Cells["Id"].Value);
                using (SqlConnection cn = Conexion.Conectar())
                {
                    cn.Open();
                    string query = @"UPDATE Proveedores
                                     SET NombreProveedor=@Nombre, Direccion=@Direccion, Telefono=@Telefono,
                                         Correo=@Correo, NombreContacto=@Contacto
                                     WHERE Id=@Id";
                    SqlCommand cmd = new SqlCommand(query, cn);
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Nombre", txtNombreProveedor.Text);
                    cmd.Parameters.AddWithValue("@Direccion", txtDireccion.Text);
                    cmd.Parameters.AddWithValue("@Telefono", txtTelefono.Text);
                    cmd.Parameters.AddWithValue("@Correo", txtCorreo.Text);
                    cmd.Parameters.AddWithValue("@Contacto", txtNombreContacto.Text);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("✏️ Proveedor actualizado.");
                MostrarProveedores();
            }
        }

        private void txtBuscarProveedor_TextChanged(object sender, EventArgs e)
        {
            using (SqlConnection cn = Conexion.Conectar())
            {
                cn.Open();
                string query = "SELECT * FROM Proveedores WHERE NombreProveedor LIKE @Nombre";
                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@Nombre", "%" + txtBuscarProveedor.Text + "%");

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvProveedores.DataSource = dt;
            }
        }

        private void btnExportarExcel_Click(object sender, EventArgs e)
        {
            if (dgvProveedores.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar.");
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog { Filter = "Excel Workbook|*.xlsx" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        var ws = wb.Worksheets.Add("Proveedores");

                        // Encabezados
                        for (int i = 0; i < dgvProveedores.Columns.Count; i++)
                            ws.Cell(1, i + 1).Value = dgvProveedores.Columns[i].HeaderText;

                        // Filas
                        for (int i = 0; i < dgvProveedores.Rows.Count; i++)
                            for (int j = 0; j < dgvProveedores.Columns.Count; j++)
                                ws.Cell(i + 2, j + 1).Value = dgvProveedores.Rows[i].Cells[j].Value?.ToString();

                        wb.SaveAs(sfd.FileName);
                    }
                    MessageBox.Show("📁 Datos exportados a Excel.");
                }
            }
        }

        private void btnExportarPdf_Click(object sender, EventArgs e)
        {
            if (dgvProveedores.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar.");
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "PDF files (*.pdf)|*.pdf" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    using (FileStream stream = new FileStream(sfd.FileName, FileMode.Create))
                    {
                        Document doc = new Document(PageSize.A4.Rotate(), 10f, 10f, 20f, 20f);
                        PdfWriter.GetInstance(doc, stream);
                        doc.Open();

                        PdfPTable table = new PdfPTable(dgvProveedores.Columns.Count);
                        table.WidthPercentage = 100;

                        // Encabezados
                        foreach (DataGridViewColumn column in dgvProveedores.Columns)
                        {
                            PdfPCell cell = new PdfPCell(new Phrase(column.HeaderText));
                            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                            table.AddCell(cell);
                        }

                        // Filas
                        foreach (DataGridViewRow row in dgvProveedores.Rows)
                        {
                            foreach (DataGridViewCell cell in row.Cells)
                            {
                                table.AddCell(cell.Value?.ToString());
                            }
                        }

                        doc.Add(table);
                        doc.Close();
                        stream.Close();
                    }
                    MessageBox.Show("📄 PDF exportado correctamente.");
                }
            }
        }

        private void dgvProveedores_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvProveedores.Rows[e.RowIndex];
                txtNombreProveedor.Text = row.Cells["NombreProveedor"].Value?.ToString();
                txtDireccion.Text = row.Cells["Direccion"].Value?.ToString();
                txtTelefono.Text = row.Cells["Telefono"].Value?.ToString();
                txtCorreo.Text = row.Cells["Correo"].Value?.ToString();
                txtNombreContacto.Text = row.Cells["NombreContacto"].Value?.ToString();
            }
        }
    }
}
