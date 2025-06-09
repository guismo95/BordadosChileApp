using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using DrawingFont = System.Drawing.Font;

namespace BordadosChileApp
{
    public partial class EmpleadosForm : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect, int nTopRect, int nRightRect, int nBottomRect,
            int nWidthEllipse, int nHeightEllipse
        );

        public EmpleadosForm()
        {
            InitializeComponent();
            this.Font = new DrawingFont("Segoe UI", 10);
            this.BackColor = Color.WhiteSmoke;
        }

        private void EmpleadosForm_Load(object sender, EventArgs e)
        {
            // Botones redondeados
            btnGuardar.Region = Region.FromHrgn(
                CreateRoundRectRgn(0, 0, btnGuardar.Width, btnGuardar.Height, 10, 10));
            btnEditar.Region = Region.FromHrgn(
                CreateRoundRectRgn(0, 0, btnEditar.Width, btnEditar.Height, 10, 10));
            btnEliminar.Region = Region.FromHrgn(
                CreateRoundRectRgn(0, 0, btnEliminar.Width, btnEliminar.Height, 10, 10));

            EstiloTabla(dgvEmpleados);
            MostrarEmpleados();
        }

        private void MostrarEmpleados()
        {
            using (SqlConnection cn = Conexion.Conectar())
            {
                string query = "SELECT * FROM Empleados ORDER BY Nombres";
                SqlDataAdapter da = new SqlDataAdapter(query, cn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvEmpleados.DataSource = dt;
            }
        }

        private void Limpiar()
        {
            txtNombre.Clear();
            txtCorreo.Clear();
            txtTelefono.Clear();
            txtDireccion.Clear();
            txtSueldo.Clear();
            txtCargo.Clear();
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
                string query = @"INSERT INTO Empleados (Nombres, Correo, Telefono, Direccion, SueldoBase, Cargo, RutEmpleado)
                                 VALUES (@Nombres, @Correo, @Telefono, @Direccion, @SueldoBase, @Cargo, @RutEmpleado)";
                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@Nombres", txtNombre.Text);
                cmd.Parameters.AddWithValue("@Correo", txtCorreo.Text);
                cmd.Parameters.AddWithValue("@Telefono", txtTelefono.Text);
                cmd.Parameters.AddWithValue("@Direccion", txtDireccion.Text);
                cmd.Parameters.AddWithValue("@SueldoBase", decimal.Parse(txtSueldo.Text));
                cmd.Parameters.AddWithValue("@Cargo", txtCargo.Text);
                cmd.Parameters.AddWithValue("@RutEmpleado", txtRutEmpleado.Text);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("✅ Empleado guardado correctamente");
            MostrarEmpleados();
            Limpiar();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvEmpleados.CurrentRow != null)
            {
                int id = Convert.ToInt32(dgvEmpleados.CurrentRow.Cells["Id"].Value);
                using (SqlConnection cn = Conexion.Conectar())
                {
                    cn.Open();
                    string query = @"UPDATE Empleados
                                     SET Nombres=@Nombres, Correo=@Correo, Telefono=@Telefono,
                                         Direccion=@Direccion, SueldoBase=@SueldoBase, Cargo=@Cargo, RutEmpleado=@RutEmpleado
                                     WHERE Id=@Id";
                    SqlCommand cmd = new SqlCommand(query, cn);
                    cmd.Parameters.AddWithValue("@Nombres", txtNombre.Text);
                    cmd.Parameters.AddWithValue("@Correo", txtCorreo.Text);
                    cmd.Parameters.AddWithValue("@Telefono", txtTelefono.Text);
                    cmd.Parameters.AddWithValue("@Direccion", txtDireccion.Text);
                    cmd.Parameters.AddWithValue("@SueldoBase", decimal.Parse(txtSueldo.Text));
                    cmd.Parameters.AddWithValue("@Cargo", txtCargo.Text);
                    cmd.Parameters.AddWithValue("@RutEmpleado", txtRutEmpleado.Text);
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("✅ Empleado editado correctamente");
                MostrarEmpleados();
                Limpiar();
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvEmpleados.CurrentRow != null)
            {
                int id = Convert.ToInt32(dgvEmpleados.CurrentRow.Cells["Id"].Value);

                using (SqlConnection cn = Conexion.Conectar())
                {
                    cn.Open();
                    string query = "DELETE FROM Empleados WHERE Id=@Id";
                    SqlCommand cmd = new SqlCommand(query, cn);
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("✅ Empleado eliminado correctamente");
                MostrarEmpleados();
                Limpiar();
            }
        }

        private void txtBuscarEmpleado_TextChanged(object sender, EventArgs e)
        {
            (dgvEmpleados.DataSource as DataTable).DefaultView.RowFilter =
                $"Nombres LIKE '%{txtBuscarEmpleado.Text}%'";
        }

        private void btnExportarExcel_Click(object sender, EventArgs e)
        {
            if (dgvEmpleados.Rows.Count == 0)
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
                        var dt = (DataTable)dgvEmpleados.DataSource;
                        wb.Worksheets.Add(dt, "Empleados");
                        wb.SaveAs(sfd.FileName);
                    }
                    MessageBox.Show("✅ Exportado a Excel correctamente.");
                }
            }
        }

        private void btnExportarPdf_Click(object sender, EventArgs e)
        {
            if (dgvEmpleados.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar.");
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog { Filter = "PDF files (*.pdf)|*.pdf" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    using (FileStream stream = new FileStream(sfd.FileName, FileMode.Create))
                    {
                        Document pdfDoc = new Document(PageSize.A4);
                        PdfWriter.GetInstance(pdfDoc, stream);
                        pdfDoc.Open();

                        Paragraph title = new Paragraph(
                            "Lista de Empleados",
                            new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 16, iTextSharp.text.Font.BOLD)
                        );
                        title.Alignment = Element.ALIGN_CENTER;
                        pdfDoc.Add(title);
                        pdfDoc.Add(new Phrase("\n"));

                        PdfPTable table = new PdfPTable(dgvEmpleados.Columns.Count);
                        table.WidthPercentage = 100;

                        // Encabezados
                        foreach (DataGridViewColumn col in dgvEmpleados.Columns)
                        {
                            PdfPCell cell = new PdfPCell(new Phrase(col.HeaderText));
                            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                            table.AddCell(cell);
                        }

                        // Filas
                        foreach (DataGridViewRow row in dgvEmpleados.Rows)
                        {
                            foreach (DataGridViewCell cell in row.Cells)
                            {
                                table.AddCell(cell.Value?.ToString() ?? "");
                            }
                        }

                        pdfDoc.Add(table);
                        pdfDoc.Close();
                        stream.Close();
                    }
                    MessageBox.Show("✅ Exportado a PDF correctamente.");
                }
            }
        }
        private void dgvEmpleados_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvEmpleados.Rows[e.RowIndex];
                txtNombre.Text = row.Cells["Nombres"].Value?.ToString();
                txtDireccion.Text = row.Cells["Direccion"].Value?.ToString();
                txtTelefono.Text = row.Cells["Telefono"].Value?.ToString();
                txtCorreo.Text = row.Cells["Correo"].Value?.ToString();
                txtSueldo.Text = row.Cells["SueldoBase"].Value?.ToString();
                txtCargo.Text = row.Cells["Cargo"].Value?.ToString();
                txtRutEmpleado.Text = row.Cells["RutEmpleado"].Value?.ToString();
            }
        }




    }
}
