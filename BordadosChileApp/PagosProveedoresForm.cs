// PagosProveedoresForm.cs
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
    public partial class PagosProveedoresForm : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        public PagosProveedoresForm()
        {
            InitializeComponent();
            this.Font = new DrawingFont("Segoe UI", 10);
            this.BackColor = Color.WhiteSmoke;
        }

        private void PagosProveedoresForm_Load(object sender, EventArgs e)
        {
            btnGuardar.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnGuardar.Width, btnGuardar.Height, 10, 10));
            EstiloTabla(dgvProveedores);

            // Llenar el combo con los nombres de los proveedores
            CargarProveedores();
            MostrarPagos();
        }

        private void CargarProveedores()
        {
            using (SqlConnection cn = Conexion.Conectar())
            {
                cn.Open();
                string query = "SELECT NombreProveedor FROM Proveedores ORDER BY NombreProveedor ASC";
                SqlCommand cmd = new SqlCommand(query, cn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    cmbProveedor.Items.Add(reader["NombreProveedor"].ToString());
                }
                reader.Close();
            }
        }

        private void MostrarPagos()
        {
            using (SqlConnection cn = Conexion.Conectar())
            {
                cn.Open();
                string query = "SELECT Fecha, Nombre, Monto, Observacion FROM PagosProveedores ORDER BY Fecha DESC";
                SqlDataAdapter da = new SqlDataAdapter(query, cn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvProveedores.DataSource = dt;
            }
        }

        private void Limpiar()
        {
            dtpFecha.Value = DateTime.Now;
            cmbProveedor.SelectedIndex = -1;
            txtMonto.Clear();
            txtObservacion.Clear();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            DateTime fecha = dtpFecha.Value;
            string nombre = cmbProveedor.Text; // Seleccionado del combo
            decimal monto;
            string observacion = txtObservacion.Text;

            if (string.IsNullOrWhiteSpace(nombre) || !decimal.TryParse(txtMonto.Text, out monto))
            {
                MessageBox.Show("Completa todos los campos correctamente.");
                return;
            }

            using (SqlConnection cn = Conexion.Conectar())
            {
                cn.Open();
                string query = @"INSERT INTO PagosProveedores (Fecha, Nombre, Monto, Observacion)
                                 VALUES (@Fecha, @Nombre, @Monto, @Observacion)";
                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@Fecha", fecha);
                cmd.Parameters.AddWithValue("@Nombre", nombre);
                cmd.Parameters.AddWithValue("@Monto", monto);
                cmd.Parameters.AddWithValue("@Observacion", observacion);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("✅ Pago guardado exitosamente");
            Limpiar();
            MostrarPagos();
        }

        private void txtBuscarPagoProveedor_TextChanged(object sender, EventArgs e)
        {
            if (dgvProveedores.DataSource is DataTable dt)
            {
                dt.DefaultView.RowFilter =
                    $"Nombre LIKE '%{txtBuscarPagoProveedor.Text.Trim()}%'";
            }
        }

        private void btnExportarExcel_Click(object sender, EventArgs e)
        {
            if (dgvProveedores.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar.");
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel Workbook|*.xlsx" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        var dt = (dgvProveedores.DataSource as DataTable).Copy();
                        var ws = wb.Worksheets.Add(dt, "Pagos Proveedores");

                        ws.Cell("A1").Value = "PROMOBORDADOS - Pagos a Proveedores";
                        ws.Cell("A1").Style.Font.Bold = true;
                        ws.Cell("A1").Style.Font.FontSize = 16;
                        ws.Range("A1:D1").Merge().Style.Alignment.Horizontal =
                            ClosedXML.Excel.XLAlignmentHorizontalValues.Center;

                        ws.Columns().AdjustToContents();
                        wb.SaveAs(sfd.FileName);
                    }
                    MessageBox.Show("✅ Exportado a Excel correctamente.");
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
                sfd.FileName = $"PagosProveedores_{DateTime.Now:yyyyMMdd}.pdf";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    using (FileStream stream = new FileStream(sfd.FileName, FileMode.Create))
                    {
                        Document pdfDoc = new Document(PageSize.A4);
                        PdfWriter.GetInstance(pdfDoc, stream);
                        pdfDoc.Open();

                        Paragraph title = new Paragraph(
                            "Pagos a Proveedores",
                            new iTextSharp.text.Font(
                                iTextSharp.text.Font.FontFamily.HELVETICA,
                                16,
                                iTextSharp.text.Font.BOLD
                            )
                        );
                        title.Alignment = Element.ALIGN_CENTER;
                        pdfDoc.Add(title);
                        pdfDoc.Add(new Phrase("\n"));

                        PdfPTable pdfTable = new PdfPTable(dgvProveedores.Columns.Count);
                        pdfTable.WidthPercentage = 100;

                        // Encabezados
                        foreach (DataGridViewColumn column in dgvProveedores.Columns)
                        {
                            PdfPCell cell = new PdfPCell(new Phrase(column.HeaderText));
                            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                            pdfTable.AddCell(cell);
                        }

                        // Filas
                        foreach (DataGridViewRow row in dgvProveedores.Rows)
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
                    MessageBox.Show("✅ Exportado a PDF correctamente.");
                }
            }
        }

        private void EstiloTabla(DataGridView dgv)
        {
            dgv.BackgroundColor = Color.White;
            dgv.DefaultCellStyle.Font = new DrawingFont("Segoe UI", 10);
            dgv.ColumnHeadersDefaultCellStyle.Font = new DrawingFont("Segoe UI", 10, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.EnableHeadersVisualStyles = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
    }
}
