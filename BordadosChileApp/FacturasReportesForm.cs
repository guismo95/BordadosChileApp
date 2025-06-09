// FacturasReportesForm.cs - COMPLETO Y ACTUALIZADO
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
    public partial class FacturasReportesForm : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        private DataTable facturasTabla;

        public FacturasReportesForm()
        {
            InitializeComponent();
            this.BackColor = Color.WhiteSmoke;
        }

        private void FacturasReportesForm_Load(object sender, EventArgs e)
        {
            // Redondear botones
            btnFiltrarVentas.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnFiltrarVentas.Width, btnFiltrarVentas.Height, 10, 10));
            btnExportarExcel.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnExportarExcel.Width, btnExportarExcel.Height, 10, 10));
            btnExportarPdf.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnExportarPdf.Width, btnExportarPdf.Height, 10, 10));

            MostrarFacturasGenerales();
            EstiloTabla(dgvFacturas);
        }

        private void MostrarFacturasGenerales()
        {
            using (SqlConnection cn = Conexion.Conectar())
            {
                cn.Open();
                string query = @"
                    SELECT
                        f.Numero AS NumeroFactura,
                        c.Nombre AS Cliente,
                        c.RUT,
                        f.Fecha AS Fecha,
                        (SUM(d.Cantidad) * p.Costo) * 1.19 AS ValorTotal
                    FROM Facturas f
                        INNER JOIN Pedidos p ON f.PedidoId = p.Id
                    INNER JOIN DetallePedido d ON p.Id = d.PedidoId
                    INNER JOIN Clientes c ON p.ClienteId = c.Id
                    GROUP BY
                        f.Numero, c.Nombre, c.RUT, f.Fecha, p.Costo";

                SqlDataAdapter da = new SqlDataAdapter(query, cn);
                facturasTabla = new DataTable();
                da.Fill(facturasTabla);
                dgvFacturas.DataSource = facturasTabla;
            }
            if (dgvFacturas.Columns.Contains("ValorTotal"))
            {
                dgvFacturas.Columns["ValorTotal"].DefaultCellStyle.Format = "N0"; 
            }
        }


        private void btnFiltrar_Click(object sender, EventArgs e)
        {
            FiltrarFacturas();
        }

        private void FiltrarFacturas()
        {
            if (facturasTabla == null) return;

            string filtro = txtBuscarCliente.Text.Trim();
            DateTime desde = dtpDesde.Value.Date;
            DateTime hasta = dtpHasta.Value.Date.AddDays(1).AddSeconds(-1);

            DataView dv = facturasTabla.DefaultView;

            string filtroFinal = $"Fecha >= '#{desde:yyyy-MM-dd}#' AND Fecha <= '#{hasta:yyyy-MM-dd}#'";

            if (!string.IsNullOrEmpty(filtro))
            {
                filtroFinal += $" AND (Cliente LIKE '%{filtro}%' OR CONVERT(NumeroFactura, 'System.String') LIKE '%{filtro}%')";
            }

            dv.RowFilter = filtroFinal;
        }

        private void txtBuscarCliente_TextChanged(object sender, EventArgs e)
        {
            FiltrarFacturas();
        }

        private void btnExportarExcel_Click(object sender, EventArgs e)
        {
            if (dgvFacturas.Rows.Count == 0)
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
                        var dt = (dgvFacturas.DataSource as DataTable).Copy();
                        var ws = wb.Worksheets.Add(dt, "Facturas Generales");

                        ws.Cell("A1").Value = "PROMOBORDADOS - Facturas Generales";
                        ws.Cell("A1").Style.Font.Bold = true;
                        ws.Cell("A1").Style.Font.FontSize = 16;
                        ws.Range("A1:E1").Merge().Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;

                        ws.Cell("A2").Value = $"Fecha de generación: {DateTime.Now:dd/MM/yyyy}";
                        ws.Range("A2:E2").Merge().Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;

                        ws.Columns().AdjustToContents();
                        wb.SaveAs(sfd.FileName);
                    }
                    MessageBox.Show("✅ Excel exportado correctamente.");
                }
            }
        }

        private void btnExportarPdf_Click(object sender, EventArgs e)
        {
            if (dgvFacturas.Rows.Count == 0)
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

                        Paragraph title = new Paragraph("Reporte de Facturas Generales", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 16, iTextSharp.text.Font.BOLDITALIC));
                        title.Alignment = Element.ALIGN_CENTER;
                        pdfDoc.Add(title);
                        pdfDoc.Add(new Phrase("\n"));

                        PdfPTable pdfTable = new PdfPTable(dgvFacturas.Columns.Count);
                        pdfTable.WidthPercentage = 100;

                        foreach (DataGridViewColumn column in dgvFacturas.Columns)
                        {
                            PdfPCell cell = new PdfPCell(new Phrase(column.HeaderText));
                            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                            pdfTable.AddCell(cell);
                        }

                        foreach (DataGridViewRow row in dgvFacturas.Rows)
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
            dgv.DefaultCellStyle.Font = new DrawingFont("Segoe UI", 10);
            dgv.ColumnHeadersDefaultCellStyle.Font = new DrawingFont("Segoe UI", 10, FontStyle.Bold);
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
