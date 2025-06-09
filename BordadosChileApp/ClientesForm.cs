using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using DrawingFont = System.Drawing.Font;


namespace BordadosChileApp
{
    public partial class ClientesForm : Form
    {
        public ClientesForm()
        {
            InitializeComponent();
        }

        private void ClientesForm_Load(object sender, EventArgs e)
        {
            MostrarClientes();
            EstilizarGrid();

            // Para que se actualicen los campos al hacer clic
            dgvClientes.CellClick += dgvClientes_CellClick;
        }

        private void MostrarClientes()
        {
            using (SqlConnection cn = Conexion.Conectar())
            {
                string query = "SELECT * FROM Clientes ORDER BY Nombre";
                SqlDataAdapter da = new SqlDataAdapter(query, cn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvClientes.DataSource = dt;
            }
        }

        private void LimpiarCampos()
        {
            txtNombre.Text = "";
            txtRut.Text = "";
            txtCorreo.Text = "";
            txtTelefono.Text = "";
            txtDireccion.Text = "";
        }

        private void EstilizarGrid()
        {
            dgvClientes.EnableHeadersVisualStyles = false;
            dgvClientes.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 204, 0);
            dgvClientes.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvClientes.ColumnHeadersDefaultCellStyle.Font = new DrawingFont("Segoe UI", 10, FontStyle.Bold);

            dgvClientes.DefaultCellStyle.BackColor = Color.White;
            dgvClientes.DefaultCellStyle.ForeColor = Color.Black;
            dgvClientes.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 229, 127);
            dgvClientes.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvClientes.DefaultCellStyle.Font = new DrawingFont("Segoe UI", 9);

            dgvClientes.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgvClientes.RowTemplate.Height = 28;
            dgvClientes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvClientes.BorderStyle = BorderStyle.None;
            dgvClientes.GridColor = Color.LightGray;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            using (SqlConnection cn = Conexion.Conectar())
            {
                string query = @"INSERT INTO Clientes (Nombre, RUT, Correo, Telefono, Direccion)
                                 VALUES (@Nombre, @RUT, @Correo, @Telefono, @Direccion)";
                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@Nombre", txtNombre.Text);
                cmd.Parameters.AddWithValue("@RUT", txtRut.Text);
                cmd.Parameters.AddWithValue("@Correo", txtCorreo.Text);
                cmd.Parameters.AddWithValue("@Telefono", txtTelefono.Text);
                cmd.Parameters.AddWithValue("@Direccion", txtDireccion.Text);
                cn.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Cliente guardado correctamente");

                MostrarClientes();
                LimpiarCampos();
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvClientes.SelectedRows.Count > 0)
            {
                int id = Convert.ToInt32(dgvClientes.SelectedRows[0].Cells["Id"].Value);
                using (SqlConnection cn = Conexion.Conectar())
                {
                    cn.Open();
                    string query = @"UPDATE Clientes
                                     SET Nombre=@Nombre, RUT=@RUT, Correo=@Correo, Telefono=@Telefono, Direccion=@Direccion
                                     WHERE Id=@Id";
                    SqlCommand cmd = new SqlCommand(query, cn);
                    cmd.Parameters.AddWithValue("@Nombre", txtNombre.Text.Trim());
                    cmd.Parameters.AddWithValue("@RUT", txtRut.Text.Trim());
                    cmd.Parameters.AddWithValue("@Correo", txtCorreo.Text.Trim());
                    cmd.Parameters.AddWithValue("@Telefono", txtTelefono.Text.Trim());
                    cmd.Parameters.AddWithValue("@Direccion", txtDireccion.Text.Trim());
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("✅ Cliente actualizado correctamente");
                    MostrarClientes();
                    LimpiarCampos();
                }
            }
            else
            {
                MessageBox.Show("Selecciona un cliente de la tabla para editar.");
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvClientes.SelectedRows.Count > 0)
            {
                int id = Convert.ToInt32(dgvClientes.SelectedRows[0].Cells["Id"].Value);

                DialogResult confirm = MessageBox.Show(
                    "¿Estás seguro de eliminar este cliente?",
                    "Confirmar",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );
                if (confirm == DialogResult.Yes)
                {
                    using (SqlConnection cn = Conexion.Conectar())
                    {
                        cn.Open();
                        string query = "DELETE FROM Clientes WHERE Id = @Id";
                        SqlCommand cmd = new SqlCommand(query, cn);
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("🗑️ Cliente eliminado correctamente");
                        MostrarClientes();
                        LimpiarCampos();
                    }
                }
            }
            else
            {
                MessageBox.Show("Selecciona un cliente de la tabla para eliminar.");
            }
        }

        private void dgvClientes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvClientes.Rows[e.RowIndex];
                txtNombre.Text = row.Cells["Nombre"].Value.ToString();
                txtRut.Text = row.Cells["RUT"].Value.ToString();
                txtCorreo.Text = row.Cells["Correo"].Value.ToString();
                txtTelefono.Text = row.Cells["Telefono"].Value.ToString();
                txtDireccion.Text = row.Cells["Direccion"].Value.ToString();
            }
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            (dgvClientes.DataSource as DataTable).DefaultView.RowFilter =
                $"Nombre LIKE '%{txtBuscar.Text}%'";
        }

        private void btnExportarExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvClientes.Rows.Count == 0)
                {
                    MessageBox.Show("No hay datos para exportar.");
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Archivos Excel (*.xlsx)|*.xlsx",
                    Title = "Guardar como Excel",
                    FileName = "Clientes.xlsx"
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var dt = (DataTable)dgvClientes.DataSource;
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add(dt, "Clientes");
                        wb.SaveAs(saveFileDialog.FileName);
                    }
                    MessageBox.Show("✅ Clientes exportados a Excel correctamente.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Error al exportar: " + ex.Message);
            }
        }

        private void btnExportarPdf_Click(object sender, EventArgs e)
        {
            if (dgvClientes.Rows.Count > 0)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Archivo PDF (*.pdf)|*.pdf",
                    FileName = "Clientes.pdf"
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (FileStream stream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                        {
                            Document pdfDoc = new Document(PageSize.A4, 25, 25, 30, 30);
                            PdfWriter.GetInstance(pdfDoc, stream);
                            pdfDoc.Open();

                            Paragraph titulo = new Paragraph(
                                "Listado de Clientes",
                                new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 16, iTextSharp.text.Font.BOLD)
                            );
                            titulo.Alignment = Element.ALIGN_CENTER;
                            pdfDoc.Add(titulo);
                            pdfDoc.Add(new Phrase("\n"));

                            PdfPTable pdfTable = new PdfPTable(dgvClientes.Columns.Count);
                            pdfTable.WidthPercentage = 100;

                            // Encabezados
                            foreach (DataGridViewColumn column in dgvClientes.Columns)
                            {
                                PdfPCell cell = new PdfPCell(new Phrase(column.HeaderText));
                                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                                pdfTable.AddCell(cell);
                            }

                            // Filas
                            foreach (DataGridViewRow row in dgvClientes.Rows)
                            {
                                foreach (DataGridViewCell cell in row.Cells)
                                {
                                    pdfTable.AddCell(cell.Value?.ToString() ?? "");
                                }
                            }

                            pdfDoc.Add(pdfTable);
                            pdfDoc.Close();
                            stream.Close();
                        }
                        MessageBox.Show("📄 PDF exportado exitosamente.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("❌ Error al exportar PDF: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("No hay datos para exportar.");
            }
        }

        private void dgvClientes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
