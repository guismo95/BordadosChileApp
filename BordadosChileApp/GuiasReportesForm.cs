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
    public partial class GuiasReportesForm : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        private DataTable guiasTabla;

        public GuiasReportesForm()
        {
            InitializeComponent();
            this.BackColor = Color.WhiteSmoke;
        }

        private void GuiasReportesForm_Load(object sender, EventArgs e)
        {
            btnFiltrar.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnFiltrar.Width, btnFiltrar.Height, 10, 10));
            btnExportarExcel.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnExportarExcel.Width, btnExportarExcel.Height, 10, 10));
            btnExportarPdf.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnExportarPdf.Width, btnExportarPdf.Height, 10, 10));

            dgvGuias.AutoGenerateColumns = true;
            MostrarGuiasGenerales();
            EstiloTabla(dgvGuias);
        }

        private void MostrarGuiasGenerales()
        {
            using (SqlConnection cn = Conexion.Conectar())
            {
                cn.Open();
                string query = @"
            SELECT
                f.Numero AS NumeroGuia,
                ISNULL(c.Nombre, 'Sin Cliente') AS Cliente,
                ISNULL(c.RUT, 'Sin RUT') AS RUT,
                f.FechaGeneracion AS Fecha,
                (ISNULL(SUM(d.Cantidad * p.Costo), 0)) * 1.19 AS ValorTotal
            FROM GuiasDespacho f
            LEFT JOIN Pedidos p ON f.PedidoId = p.Id
            LEFT JOIN DetallePedido d ON p.Id = d.PedidoId
            LEFT JOIN Clientes c ON p.ClienteId = c.Id
            GROUP BY
                f.Numero, c.Nombre, c.RUT, f.FechaGeneracion";

                SqlDataAdapter da = new SqlDataAdapter(query, cn);
                guiasTabla = new DataTable();
                da.Fill(guiasTabla);
                dgvGuias.DataSource = guiasTabla;
            }

            if (dgvGuias.Columns.Contains("ValorTotal"))
            {
                dgvGuias.Columns["ValorTotal"].DefaultCellStyle.Format = "N0";
            }
        }


        private void btnFiltrar_Click(object sender, EventArgs e)
        {
            FiltrarGuias();
        }

        private void txtBuscarCliente_TextChanged(object sender, EventArgs e)
        {
            FiltrarGuias();
        }

        private void FiltrarGuias()
        {
            if (guiasTabla == null) return;

            string filtro = txtBuscarCliente.Text.Trim();
            DateTime desde = dtpDesde.Value.Date;
            DateTime hasta = dtpHasta.Value.Date.AddDays(1).AddSeconds(-1); // Hasta fin del día

            DataView dv = guiasTabla.DefaultView;

            // El formato de fecha debe ser compatible con DataView
            string filtroFinal = $"Fecha >= #{desde:yyyy-MM-dd}# AND Fecha <= #{hasta:yyyy-MM-dd}#";

            if (!string.IsNullOrEmpty(filtro))
            {
                filtroFinal += $" AND (Cliente LIKE '%{filtro}%' OR CONVERT(NumeroGuia, 'System.String') LIKE '%{filtro}%')";
            }

            try
            {
                dv.RowFilter = filtroFinal;
                dgvGuias.DataSource = dv.ToTable(); // IMPORTANTÍSIMO: actualiza el DGV con el filtrado
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al filtrar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnExportarExcel_Click(object sender, EventArgs e)
        {
            if (dgvGuias.Rows.Count == 0)
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
                        var dt = new DataTable();

                        // Cargar los datos visibles del dgv
                        foreach (DataGridViewColumn col in dgvGuias.Columns)
                        {
                            dt.Columns.Add(col.HeaderText);
                        }
                        foreach (DataGridViewRow row in dgvGuias.Rows)
                        {
                            DataRow dr = dt.NewRow();
                            for (int i = 0; i < dgvGuias.Columns.Count; i++)
                            {
                                dr[i] = row.Cells[i].Value;
                            }
                            dt.Rows.Add(dr);
                        }

                        var ws = wb.Worksheets.Add(dt, "Guías Generales");

                        ws.Cell("A1").Value = "PROMOBORDADOS - Guías Generales";
                        ws.Cell("A1").Style.Font.Bold = true;
                        ws.Cell("A1").Style.Font.FontSize = 16;
                        ws.Range("A1:F1").Merge().Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;

                        ws.Cell("A2").Value = $"Fecha de generación: {DateTime.Now:dd/MM/yyyy}";
                        ws.Range("A2:F2").Merge().Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;

                        ws.Columns().AdjustToContents();
                        wb.SaveAs(sfd.FileName);
                    }

                    MessageBox.Show("✅ Excel exportado correctamente.");
                }
            }
        }


        private void btnExportarPdf_Click(object sender, EventArgs e)
        {
            if (dgvGuias.Rows.Count == 0)
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
                        Document pdfDoc = new Document(PageSize.A4);
                        PdfWriter.GetInstance(pdfDoc, stream);
                        pdfDoc.Open();

                        Paragraph title = new Paragraph(
                            "PROMOBORDADOS - Guías Generales",
                            new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 16, iTextSharp.text.Font.BOLDITALIC)
                        );
                        title.Alignment = Element.ALIGN_CENTER;
                        pdfDoc.Add(title);
                        pdfDoc.Add(new Phrase("\n"));

                        PdfPTable pdfTable = new PdfPTable(dgvGuias.Columns.Count);
                        pdfTable.WidthPercentage = 100;

                        foreach (DataGridViewColumn column in dgvGuias.Columns)
                        {
                            PdfPCell cell = new PdfPCell(new Phrase(column.HeaderText));
                            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                            pdfTable.AddCell(cell);
                        }

                        foreach (DataGridViewRow row in dgvGuias.Rows)
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

                    MessageBox.Show("✅ PDF exportado correctamente.");
                }
            }
        }


        private void EstiloTabla(DataGridView dgv)
        {
            dgv.BackgroundColor = Color.White;
            dgv.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10);
            dgv.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 204, 0);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 229, 127);
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgv.RowTemplate.Height = 28;
            dgv.EnableHeadersVisualStyles = false;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgv.BorderStyle = BorderStyle.None;
            dgv.GridColor = Color.LightGray;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
    }
}
