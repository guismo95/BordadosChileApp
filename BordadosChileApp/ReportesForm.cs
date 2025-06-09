// ReportesForm.cs - Actualizado COMPLETO con correcciones finales
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Globalization;
using DrawingFont = System.Drawing.Font;
using System.Linq;

namespace BordadosChileApp
{
    public partial class ReportesForm : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        private DataTable ventasTabla;

        public ReportesForm()
        {
            InitializeComponent();
            this.BackColor = Color.WhiteSmoke;
        }

        private void ReportesForm_Load(object sender, EventArgs e)
        {
            btnFiltrarVentas.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnFiltrarVentas.Width, btnFiltrarVentas.Height, 10, 10));
            btnExportarExcel.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnExportarExcel.Width, btnExportarExcel.Height, 10, 10));
            btnExportarPdf.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnExportarPdf.Width, btnExportarPdf.Height, 10, 10));

            MostrarVentasGenerales();
            EstiloTabla(dgvVentasGenerales);
            RedondearBotones();
            ActualizarTotalesYGraficos();
            CargarGraficoVentasMes();
            CargarGraficoVentasSemana();
            AplicarEstiloLabels();
        }

        private void RedondearBotones()
        {
            btnFiltrarVentas.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnFiltrarVentas.Width, btnFiltrarVentas.Height, 10, 10));
            btnExportarExcel.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnExportarExcel.Width, btnExportarExcel.Height, 10, 10));
            btnExportarPdf.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnExportarPdf.Width, btnExportarPdf.Height, 10, 10));
        }

        private void MostrarVentasGenerales()
        {
            using (SqlConnection cn = Conexion.Conectar())
            {
                cn.Open();
                string query = @"
                SELECT 
                    c.Nombre AS Cliente,
                    c.RUT,
                    c.Telefono,
                    p.Id AS PedidoId,
                    p.Fecha,
                    p.Estado,
                    (SUM(d.Cantidad) * p.Costo) * 1.19 AS ValorTotal
                FROM Pedidos p
                INNER JOIN Clientes c ON p.ClienteId = c.Id
                INNER JOIN DetallePedido d ON p.Id = d.PedidoId
                GROUP BY 
                    c.Nombre, c.RUT, c.Telefono,
                    p.Id, p.Fecha, p.Estado, p.Costo";

                SqlDataAdapter da = new SqlDataAdapter(query, cn);
                ventasTabla = new DataTable();
                da.Fill(ventasTabla);
                dgvVentasGenerales.DataSource = ventasTabla;
            }
        }

        private void btnFiltrar_Click(object sender, EventArgs e)
        {
            FiltrarVentas();
        }

        private void FiltrarVentas()
        {
            if (ventasTabla == null) return;

            string filtroCliente = txtBuscarCliente.Text.Trim();
            DateTime desde = dtpDesde.Value.Date;
            DateTime hasta = dtpHasta.Value.Date.AddDays(1).AddSeconds(-1);

            DataView dv = ventasTabla.DefaultView;

            // Filtro por rango de fecha
            string filtroFinal = $"Fecha >= #{desde:yyyy-MM-dd}# AND Fecha <= #{hasta:yyyy-MM-dd}#";

            if (!string.IsNullOrEmpty(filtroCliente))
            {
                // Si también hay cliente, agregarlo
                filtroFinal += $" AND Cliente LIKE '%{filtroCliente}%'";
            }

            dv.RowFilter = filtroFinal;
        }


        private void txtBuscarCliente_TextChanged(object sender, EventArgs e)
        {
            FiltrarVentas();
        }

        private void btnExportarPdf_Click(object sender, EventArgs e)
        {
            if (dgvVentasGenerales.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar.");
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "PDF files (*.pdf)|*.pdf";
                sfd.FileName = $"Reporte_Inventario_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (FileStream stream = new FileStream(sfd.FileName, FileMode.Create, FileAccess.Write))
                        {
                            Document pdfDoc = new Document(PageSize.A4);
                            PdfWriter.GetInstance(pdfDoc, stream);
                            pdfDoc.Open();

                            // Agregar el contenido al PDF
                            PdfPTable pdfTable = new PdfPTable(dgvVentasGenerales.Columns.Count);
                            pdfTable.WidthPercentage = 100;

                            // Encabezados
                            foreach (DataGridViewColumn column in dgvVentasGenerales.Columns)
                            {
                                PdfPCell cell = new PdfPCell(new Phrase(column.HeaderText));
                                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                                pdfTable.AddCell(cell);
                            }

                            // Filas
                            foreach (DataGridViewRow row in dgvVentasGenerales.Rows)
                            {
                                foreach (DataGridViewCell cell in row.Cells)
                                {
                                    pdfTable.AddCell(cell.Value?.ToString() ?? "");
                                }
                            }

                            pdfDoc.Add(pdfTable);
                            pdfDoc.Close();
                        }

                        MessageBox.Show("✅ PDF exportado correctamente.");
                    }
                    catch (IOException ex)
                    {
                        MessageBox.Show($"❌ Error al exportar PDF: El archivo ya está en uso.\n\n{ex.Message}", "Archivo en Uso", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"❌ Error al exportar PDF: {ex.Message}");
                    }
                }
            }
        }
        private void btnExportarExcel_Click(object sender, EventArgs e)
        {
            if (dgvVentasGenerales.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar.");
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel Workbook|*.xlsx" })
            {
                sfd.Filter = "Excel Workbook|*.xlsx";
                sfd.FileName = $"Reporte_Inventario_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        var ws = wb.Worksheets.Add("Ventas Generales");

                        // Logo
                        string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logo.png");
                        if (File.Exists(imagePath))
                        {
                            var img = ws.AddPicture(imagePath).MoveTo(ws.Cell("A1")).Scale(0.2);
                        }

                        // Título
                        ws.Cell("A5").Value = "PROMOBORDADOS - Ventas Generales";
                        ws.Cell("A5").Style.Font.Bold = true;
                        ws.Cell("A5").Style.Font.FontSize = 16;
                        ws.Range("A5:E5").Merge().Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;

                        // Fecha
                        ws.Cell("A6").Value = $"Fecha de generación: {DateTime.Now:dd/MM/yyyy}";
                        ws.Range("A6:E6").Merge().Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;

                        // Columnas
                        for (int i = 0; i < dgvVentasGenerales.Columns.Count; i++)
                            ws.Cell(8, i + 1).Value = dgvVentasGenerales.Columns[i].HeaderText;

                        // Datos
                        for (int i = 0; i < dgvVentasGenerales.Rows.Count; i++)
                            for (int j = 0; j < dgvVentasGenerales.Columns.Count; j++)
                                ws.Cell(i + 9, j + 1).Value = dgvVentasGenerales.Rows[i].Cells[j].Value?.ToString();

                        ws.Columns().AdjustToContents();
                        wb.SaveAs(sfd.FileName);
                    }
                    MessageBox.Show("✅ Excel exportado correctamente.");
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
        private void ActualizarTotalesYGraficos()
        {
            if (ventasTabla == null) return;

            DateTime hoy = DateTime.Today;
            DateTime primerDiaSemana = hoy.AddDays(-(int)hoy.DayOfWeek + (hoy.DayOfWeek == DayOfWeek.Sunday ? -6 : 1));
            DateTime ultimoDiaSemana = primerDiaSemana.AddDays(6);

            decimal totalSemana = ventasTabla.AsEnumerable()
                .Where(r => Convert.ToDateTime(r["Fecha"]).Date >= primerDiaSemana && Convert.ToDateTime(r["Fecha"]).Date <= ultimoDiaSemana)
                .Sum(r => r.Field<decimal>("ValorTotal"));

            decimal totalMes = ventasTabla.AsEnumerable()
                .Where(r => Convert.ToDateTime(r["Fecha"]).Month == hoy.Month && Convert.ToDateTime(r["Fecha"]).Year == hoy.Year)
                .Sum(r => r.Field<decimal>("ValorTotal"));

            lblTotalVentasSemana.Text = $"🗓️ Ventas Semana: ${totalSemana:N0}";
            lblTotalVentasMes.Text = $"📅 Ventas Mes: ${totalMes:N0}";

            CargarGraficoVentasMes();
            CargarGraficoVentasSemana();
            CargarGraficoVentasAño();
        }

        private void CargarGraficoVentasMes()
        {
            chartVentasMes.Series.Clear();
            Series serie = new Series("Ventas x Mes")
            {
                ChartType = SeriesChartType.Column,
                Color = Color.FromArgb(0, 122, 204)
            };

            DateTime ahora = DateTime.Now;
            decimal[] ventasMes = new decimal[12];

            foreach (DataRow row in ventasTabla.Rows)
            {
                DateTime fecha = Convert.ToDateTime(row["Fecha"]);
                if (fecha.Year == ahora.Year)
                {
                    int mes = fecha.Month - 1;
                    ventasMes[mes] += Convert.ToDecimal(row["ValorTotal"]);
                }
            }

            for (int i = 0; i < 12; i++)
                serie.Points.AddXY(new DateTime(ahora.Year, i + 1, 1).ToString("MMM"), ventasMes[i]);

            chartVentasMes.Series.Add(serie);
        }
        private void CargarGraficoVentasAño()
        {
            chartVentasAño.Series.Clear();
            var series = chartVentasAño.Series.Add("VentasAño");
            series.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;

            var ventasPorAño = ventasTabla.AsEnumerable()
                .GroupBy(r => Convert.ToDateTime(r["Fecha"]).Year)
                .OrderBy(g => g.Key)
                .Select(g => new { Año = g.Key, Total = g.Sum(x => x.Field<decimal>("ValorTotal")) });

            foreach (var v in ventasPorAño)
            {
                series.Points.AddXY(v.Año, v.Total);
            }
        }

        private void CargarGraficoVentasSemana()
        {
            chartVentasSemana.Series.Clear();
            Series serie = new Series("Ventas x Semana")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Green
            };

            DateTime hoy = DateTime.Today;
            for (int i = 6; i >= 0; i--)
            {
                DateTime dia = hoy.AddDays(-i);
                decimal totalDia = 0;

                foreach (DataRow row in ventasTabla.Rows)
                {
                    DateTime fecha = Convert.ToDateTime(row["Fecha"]);
                    if (fecha.Date == dia)
                        totalDia += Convert.ToDecimal(row["ValorTotal"]);
                }

                serie.Points.AddXY(dia.ToString("dd-MMM"), totalDia);
            }

            chartVentasSemana.Series.Add(serie);
        }
    
        private void AplicarEstiloLabels()
        {
            // Ventas Semana
            lblTotalVentasSemana.BackColor = Color.WhiteSmoke;
            lblTotalVentasSemana.Font = new System.Drawing.Font("Segoe UI", 12, FontStyle.Bold);
            lblTotalVentasSemana.TextAlign = ContentAlignment.MiddleCenter;

            // Ventas Mes
            lblTotalVentasMes.BackColor = Color.WhiteSmoke;
            lblTotalVentasMes.Font = new System.Drawing.Font("Segoe UI", 12, FontStyle.Bold);
            lblTotalVentasMes.TextAlign = ContentAlignment.MiddleCenter;
        }

    }
}
