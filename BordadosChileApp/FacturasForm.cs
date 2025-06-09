using System;
using System.Collections.Generic;
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
    public partial class FacturasForm : Form
    {
        public FacturasForm()
        {
            InitializeComponent();
            EstiloTabla(dgvFacturas);
        }

        private int idFacturaSeleccionada = -1;

        private void FacturasForm_Load(object sender, EventArgs e)
        {
            CargarClientes();
            MostrarFacturas();
        }


        private void CargarClientes()
        {
            using (SqlConnection cn = Conexion.Conectar())
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT Id, Nombre, RUT, Direccion, Telefono, Correo FROM Clientes", cn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                cmbCliente.DataSource = dt;
                cmbCliente.DisplayMember = "Nombre";
                cmbCliente.ValueMember = "Id";
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (cmbCliente.SelectedItem == null)
            {
                MessageBox.Show("Por favor selecciona un cliente.");
                return;
            }

            decimal monto;
            if (!decimal.TryParse(txtMonto.Text.Trim(), out monto))
            {
                MessageBox.Show("Por favor ingresa un monto válido.");
                return;
            }

            // Aplicar IVA 19%
            decimal montoConIva = monto * 1.19m;

            DataRowView clienteSeleccionado = cmbCliente.SelectedItem as DataRowView;

            using (SqlConnection cn = Conexion.Conectar())
            {
                cn.Open();

                int numeroFactura = ObtenerNuevoNumeroFactura(cn);

                SqlCommand cmd = new SqlCommand(@"
                    INSERT INTO Facturas (Numero, Fecha, Cliente, RUT, Direccion, Telefono, Correo, Monto, Observaciones)
                    VALUES (@Numero, @Fecha, @Cliente, @RUT, @Direccion, @Telefono, @Correo, @Monto, @Observaciones)", cn);

                cmd.Parameters.AddWithValue("@Numero", numeroFactura);
                cmd.Parameters.AddWithValue("@Fecha", dtpFecha.Value);
                cmd.Parameters.AddWithValue("@Cliente", clienteSeleccionado["Nombre"].ToString());
                cmd.Parameters.AddWithValue("@RUT", clienteSeleccionado["RUT"].ToString());
                cmd.Parameters.AddWithValue("@Direccion", clienteSeleccionado["Direccion"].ToString());
                cmd.Parameters.AddWithValue("@Telefono", clienteSeleccionado["Telefono"].ToString());
                cmd.Parameters.AddWithValue("@Correo", clienteSeleccionado["Correo"].ToString());
                cmd.Parameters.AddWithValue("@Monto", montoConIva);
                cmd.Parameters.AddWithValue("@Observaciones", txtObservaciones.Text.Trim());

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("✅ Factura registrada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            MostrarFacturas();
            LimpiarCampos();
        }
        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (idFacturaSeleccionada == -1)
            {
                MessageBox.Show("Selecciona una factura para editar.");
                return;
            }

            decimal monto;
            if (!decimal.TryParse(txtMonto.Text.Trim(), out monto))
            {
                MessageBox.Show("Por favor ingresa un monto válido.");
                return;
            }

            decimal montoConIva = monto * 1.19m;
            DataRowView clienteSeleccionado = cmbCliente.SelectedItem as DataRowView;

            using (SqlConnection cn = Conexion.Conectar())
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand(@"
            UPDATE Facturas
            SET Fecha = @Fecha, Cliente = @Cliente, RUT = @RUT, Direccion = @Direccion, 
                Telefono = @Telefono, Correo = @Correo, Monto = @Monto, Observaciones = @Observaciones
            WHERE Id = @Id", cn);

                cmd.Parameters.AddWithValue("@Fecha", dtpFecha.Value);
                cmd.Parameters.AddWithValue("@Cliente", clienteSeleccionado["Nombre"].ToString());
                cmd.Parameters.AddWithValue("@RUT", clienteSeleccionado["RUT"].ToString());
                cmd.Parameters.AddWithValue("@Direccion", clienteSeleccionado["Direccion"].ToString());
                cmd.Parameters.AddWithValue("@Telefono", clienteSeleccionado["Telefono"].ToString());
                cmd.Parameters.AddWithValue("@Correo", clienteSeleccionado["Correo"].ToString());
                cmd.Parameters.AddWithValue("@Monto", montoConIva);
                cmd.Parameters.AddWithValue("@Observaciones", txtObservaciones.Text.Trim());
                cmd.Parameters.AddWithValue("@Id", idFacturaSeleccionada);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("✅ Factura actualizada exitosamente.");
            MostrarFacturas();
            LimpiarCampos();
        }
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (idFacturaSeleccionada == -1)
            {
                MessageBox.Show("Selecciona una factura para eliminar.");
                return;
            }

            DialogResult result = MessageBox.Show("¿Estás seguro que deseas eliminar esta factura?", "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                using (SqlConnection cn = Conexion.Conectar())
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM Facturas WHERE Id = @Id", cn);
                    cmd.Parameters.AddWithValue("@Id", idFacturaSeleccionada);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("✅ Factura eliminada exitosamente.");
                MostrarFacturas();
                LimpiarCampos();
            }
        }

        private int ObtenerNuevoNumeroFactura(SqlConnection cn)
        {
            using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(Numero), 0) + 1 FROM Facturas", cn))
            {
                return (int)cmd.ExecuteScalar();
            }
        }

        private void MostrarFacturas()
        {
            using (SqlConnection cn = Conexion.Conectar())
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Facturas ORDER BY Fecha DESC", cn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvFacturas.DataSource = dt;
            }
        }

        private void LimpiarCampos()
        {
            cmbCliente.SelectedIndex = -1;
            txtMonto.Clear();
            txtObservaciones.Clear();
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
                        var ws = wb.Worksheets.Add(dt, "Facturas");

                        ws.Cell("A1").Value = "PROMOBORDADOS - Facturas Locales";
                        ws.Cell("A1").Style.Font.Bold = true;
                        ws.Cell("A1").Style.Font.FontSize = 16;
                        ws.Range("A1:J1").Merge().Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;

                        ws.Columns().AdjustToContents();
                        wb.SaveAs(sfd.FileName);
                    }

                    MessageBox.Show("✅ Excel exportado correctamente.");
                }
            }
        }

        // NUEVO: Exportar solo una factura seleccionada desde el DataGridView
        private void btnExportarPdf_Click(object sender, EventArgs e)
        {
            if (dgvFacturas.CurrentRow == null)
            {
                MessageBox.Show("Selecciona una factura para exportar.");
                return;
            }

            int numeroFactura = Convert.ToInt32(dgvFacturas.CurrentRow.Cells["Numero"].Value);
            object pedidoIdObj = dgvFacturas.CurrentRow.Cells["PedidoId"].Value;

            using (SqlConnection cn = Conexion.Conectar())
            {
                cn.Open();

                string cliente = "", rut = "", direccion = "", telefono = "", correo = "", observaciones = "";
                string estado = "", bordadoGeneral = "";
                DateTime fecha = DateTime.Now;
                decimal costo = 0;
                int cajas = 0;
                List<(string Tipo, string Color, int Cantidad, string Bordado)> detalles = new List<(string Tipo, string Color, int Cantidad, string Bordado)>();

                if (pedidoIdObj != DBNull.Value)
                {
                    int pedidoId = Convert.ToInt32(pedidoIdObj);

                    SqlCommand cmdCliente = new SqlCommand(@"
                SELECT c.Nombre, c.RUT, c.Direccion, c.Telefono, c.Correo, p.Fecha, p.Bordado, p.Estado, p.Costo, p.Cajas
                FROM Pedidos p
                INNER JOIN Clientes c ON p.ClienteId = c.Id
                WHERE p.Id = @PedidoId", cn);

                    cmdCliente.Parameters.AddWithValue("@PedidoId", pedidoId);

                    using (SqlDataReader reader = cmdCliente.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            cliente = reader["Nombre"].ToString();
                            rut = reader["RUT"].ToString();
                            direccion = reader["Direccion"].ToString();
                            telefono = reader["Telefono"].ToString();
                            correo = reader["Correo"].ToString();
                            fecha = Convert.ToDateTime(reader["Fecha"]);
                            observaciones = reader["Bordado"].ToString();
                            estado = reader["Estado"].ToString();
                            bordadoGeneral = reader["Bordado"].ToString();
                            costo = Convert.ToDecimal(reader["Costo"]);
                            cajas = Convert.ToInt32(reader["Cajas"]);
                        }
                    }

                    SqlCommand cmdDetalle = new SqlCommand("SELECT TipoPrenda, Color, Cantidad, Bordado FROM DetallePedido WHERE PedidoId = @PedidoId", cn);
                    cmdDetalle.Parameters.AddWithValue("@PedidoId", pedidoId);
                    using (SqlDataReader reader = cmdDetalle.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            detalles.Add((
                                reader["TipoPrenda"].ToString(),
                                reader["Color"].ToString(),
                                Convert.ToInt32(reader["Cantidad"]),
                                reader["Bordado"].ToString()
                            ));
                        }
                    }
                }
                else
                {
                    cliente = dgvFacturas.CurrentRow.Cells["Cliente"].Value?.ToString();
                    rut = dgvFacturas.CurrentRow.Cells["RUT"].Value?.ToString();
                    direccion = dgvFacturas.CurrentRow.Cells["Direccion"].Value?.ToString();
                    telefono = dgvFacturas.CurrentRow.Cells["Telefono"].Value?.ToString();
                    correo = dgvFacturas.CurrentRow.Cells["Correo"].Value?.ToString();
                    fecha = Convert.ToDateTime(dgvFacturas.CurrentRow.Cells["Fecha"].Value);
                    observaciones = dgvFacturas.CurrentRow.Cells["Observaciones"].Value?.ToString();
                    costo = Convert.ToDecimal(dgvFacturas.CurrentRow.Cells["Monto"].Value);
                    detalles.Add(("Servicio", "-", 1, observaciones));
                }

                using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "PDF files (*.pdf)|*.pdf" })
                {
                    sfd.FileName = $"Factura_FAPB{numeroFactura:D3}.pdf";

                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        using (FileStream stream = new FileStream(sfd.FileName, FileMode.Create))
                        {
                            Document pdfDoc = new Document(PageSize.A4, 40, 40, 40, 40);
                            PdfWriter.GetInstance(pdfDoc, stream);
                            pdfDoc.Open();

                            string rutaLogo = Path.Combine(Application.StartupPath, "Resources", "Logo.png");
                            if (File.Exists(rutaLogo))
                            {
                                var logo = iTextSharp.text.Image.GetInstance(rutaLogo);
                                logo.ScaleAbsolute(120, 60);
                                logo.Alignment = Element.ALIGN_RIGHT;
                                pdfDoc.Add(logo);
                            }

                            var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
                            var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);

                            PdfPTable headerTable = new PdfPTable(2);
                            headerTable.WidthPercentage = 100;
                            headerTable.SetWidths(new float[] { 70, 30 });

                            PdfPCell leftCell = new PdfPCell { Border = iTextSharp.text.Rectangle.NO_BORDER };
                            leftCell.AddElement(new Paragraph("RAZÓN SOCIAL EMPRESA", boldFont));
                            leftCell.AddElement(new Paragraph("RUT: xx.xxx.xxx-x", normalFont));
                            leftCell.AddElement(new Paragraph("DIRECCIÓN: Pendiente", normalFont));
                            leftCell.AddElement(new Paragraph("TELÉFONO: +56 9 00000000", normalFont));
                            leftCell.AddElement(new Paragraph("EMAIL: correo@empresa.cl", normalFont));

                            PdfPCell rightCell = new PdfPCell { Border = iTextSharp.text.Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT };
                            rightCell.AddElement(new Paragraph("DOCUMENTO:", boldFont));
                            rightCell.AddElement(new Paragraph("FACTURA", boldFont));
                            rightCell.AddElement(new Paragraph($"Nº FAPB{numeroFactura:D3}", normalFont));
                            rightCell.AddElement(new Paragraph("Fecha: " + fecha.ToString("dd-MM-yyyy"), normalFont));

                            headerTable.AddCell(leftCell);
                            headerTable.AddCell(rightCell);
                            pdfDoc.Add(headerTable);
                            pdfDoc.Add(new Paragraph(" "));

                            PdfPTable infoCliente = new PdfPTable(1) { WidthPercentage = 100 };
                            infoCliente.AddCell(new Phrase($"Cliente: {cliente}", normalFont));
                            infoCliente.AddCell(new Phrase($"RUT: {rut}", normalFont));
                            infoCliente.AddCell(new Phrase($"Dirección: {direccion}", normalFont));
                            infoCliente.AddCell(new Phrase($"Teléfono: {telefono}", normalFont));
                            infoCliente.AddCell(new Phrase($"Correo: {correo}", normalFont));
                            infoCliente.AddCell(new Phrase($"Fecha Pedido: {fecha}", normalFont));
                            if (!string.IsNullOrEmpty(estado)) infoCliente.AddCell(new Phrase($"Estado: {estado}", normalFont));
                            if (!string.IsNullOrEmpty(bordadoGeneral)) infoCliente.AddCell(new Phrase($"Bordado General: {bordadoGeneral}", normalFont));
                            infoCliente.AddCell(new Phrase($"Observaciones: {observaciones}", normalFont));
                            pdfDoc.Add(infoCliente);
                            pdfDoc.Add(new Paragraph(" "));

                            PdfPTable detalle = new PdfPTable(6);
                            detalle.WidthPercentage = 100;
                            detalle.SetWidths(new float[] { 20, 20, 10, 25, 10, 15 });

                            string[] headers = { "Tipo Prenda", "Color", "Cantidad", "Bordado", "Costo", "Total" };
                            foreach (var h in headers)
                            {
                                PdfPCell cell = new PdfPCell(new Phrase(h, boldFont))
                                {
                                    BackgroundColor = new BaseColor(255, 204, 0)
                                };
                                detalle.AddCell(cell);
                            }

                            decimal subtotal = 0;
                            foreach (var d in detalles)
                            {
                                decimal totalItem = d.Cantidad * costo;
                                subtotal += totalItem;

                                detalle.AddCell(new Phrase(d.Tipo, normalFont));
                                detalle.AddCell(new Phrase(d.Color, normalFont));
                                detalle.AddCell(new Phrase(d.Cantidad.ToString(), normalFont));
                                detalle.AddCell(new Phrase(d.Bordado, normalFont));
                                detalle.AddCell(new Phrase("$" + costo.ToString("N0"), normalFont));
                                detalle.AddCell(new Phrase("$" + totalItem.ToString("N0"), normalFont));
                            }
                            pdfDoc.Add(detalle);

                            decimal iva = subtotal * 0.19m;
                            decimal total = subtotal + iva;

                            PdfPTable totales = new PdfPTable(2);
                            totales.WidthPercentage = 40;
                            totales.HorizontalAlignment = Element.ALIGN_RIGHT;
                            totales.SetWidths(new float[] { 50, 50 });

                            totales.AddCell(new Phrase("Subtotal", boldFont));
                            totales.AddCell(new Phrase("$" + subtotal.ToString("N0"), normalFont));
                            totales.AddCell(new Phrase("IVA 19%", boldFont));
                            totales.AddCell(new Phrase("$" + iva.ToString("N0"), normalFont));
                            totales.AddCell(new Phrase("TOTAL", boldFont));
                            totales.AddCell(new Phrase("$" + total.ToString("N0"), normalFont));

                            pdfDoc.Add(totales);

                            pdfDoc.Close();
                            stream.Close();
                        }

                        MessageBox.Show("✅ PDF exportado correctamente.");
                    }
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
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 229, 127);
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgv.RowTemplate.Height = 28;
            dgv.BorderStyle = BorderStyle.None;
            dgv.GridColor = Color.LightGray;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
        }

        private void dgvFacturas_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                try
                {
                    DataGridViewRow row = dgvFacturas.Rows[e.RowIndex];

                    // Validar ID
                    if (row.Cells["Id"].Value != DBNull.Value)
                        idFacturaSeleccionada = Convert.ToInt32(row.Cells["Id"].Value);
                    else
                        idFacturaSeleccionada = 0;

                    // Cliente
                    cmbCliente.Text = row.Cells["Cliente"].Value?.ToString() ?? "";

                    // Monto
                    txtMonto.Text = row.Cells["Monto"].Value?.ToString() ?? "";

                    // Observaciones
                    txtObservaciones.Text = row.Cells["Observaciones"].Value?.ToString() ?? "";

                    // Fecha
                    if (row.Cells["Fecha"].Value != DBNull.Value)
                        dtpFecha.Value = Convert.ToDateTime(row.Cells["Fecha"].Value);
                    else
                        dtpFecha.Value = DateTime.Now;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("⚠️ Error al seleccionar la factura: " + ex.Message);
                }
            }
        }

    }
}

