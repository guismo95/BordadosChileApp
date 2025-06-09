using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using ClosedXML.Excel;
using DrawingFont = System.Drawing.Font;
using DocumentFormat.OpenXml.Drawing.Diagrams;

namespace BordadosChileApp
{
    public partial class PedidosForm : Form
    {
        private int filaSeleccionadaDetalle = -1;

        public PedidosForm()
        {
            InitializeComponent();
            this.BackColor = Color.WhiteSmoke;
        }

        private void PedidosForm_Load(object sender, EventArgs e)
        {
            CargarClientes();
            cmbEstado.Items.AddRange(new string[] { "Pendiente", "En Proceso", "Lista", "Entregada" });

            dgvDetallePedido.ColumnCount = 4;
            dgvDetallePedido.Columns[0].Name = "TipoPrenda";
            dgvDetallePedido.Columns[1].Name = "Color";
            dgvDetallePedido.Columns[2].Name = "Cantidad";
            dgvDetallePedido.Columns[3].Name = "Bordado";

            EstilizarTabla(dgvDetallePedido);
            EstilizarTabla(dgvPedidos);
            CargarPedidos();

            if (SesionUsuario.Rol != "admin")
            {
                btnGenerarFactura.Visible = false;
                btnGenerarGuia.Visible = false;
            }
        }

        private void CargarClientes()
        {
            using (SqlConnection cn = Conexion.Conectar())
            {
                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter("SELECT Id, Nombre FROM Clientes", cn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cmbCliente.DataSource = dt;
                cmbCliente.DisplayMember = "Nombre";
                cmbCliente.ValueMember = "Id";
                cmbCliente.SelectedIndex = -1;
            }
        }

        private void EstilizarTabla(DataGridView dgv)
        {
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 204, 0);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgv.ColumnHeadersDefaultCellStyle.Font = new DrawingFont("Segoe UI", 10, FontStyle.Bold);
            dgv.DefaultCellStyle.Font = new DrawingFont("Segoe UI", 9);
            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.ForeColor = Color.Black;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 229, 127);
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgv.RowTemplate.Height = 28;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.BorderStyle = BorderStyle.None;
            dgv.GridColor = Color.LightGray;
        }

        private void Limpiar()
        {
            cmbCliente.SelectedIndex = -1;
            txtCajas.Clear();
            txtBordado.Clear();
            txtCosto.Clear();
            cmbEstado.SelectedIndex = -1;
            picLogo.Image = null;
            dgvDetallePedido.Rows.Clear();
            txtTipoPrenda.Clear();
            txtColor.Clear();
            txtCantidad.Clear();
            txtBordadoPrenda.Clear();
        }

        private void CargarPedidos()
        {
            using (SqlConnection cn = Conexion.Conectar())
            {
                cn.Open();
                string query = @"
                    SELECT 
                       p.Id AS PedidoId,
                       c.Nombre AS Cliente,
                       p.Fecha,
                       p.Cajas,
                       p.Bordado,
                       p.Estado,
                       p.Costo,
                       d.TipoPrenda,
                       d.Color,
                       d.Cantidad,
                       d.Bordado AS BordadoDetalle,
                       p.Logo
                    FROM Pedidos p
                    INNER JOIN Clientes c ON p.ClienteId = c.Id
                    INNER JOIN DetallePedido d ON p.Id = d.PedidoId
                    ORDER BY p.Id DESC";

                SqlDataAdapter da = new SqlDataAdapter(query, cn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvPedidos.DataSource = dt;

                if (dgvPedidos.Columns.Contains("Logo"))
                {
                    dgvPedidos.Columns["Logo"].Visible = false;
                }

                // Si aún no se ha agregado la columna de imagen
                if (!dgvPedidos.Columns.Contains("LogoImage"))
                {
                    DataGridViewImageColumn imgCol = new DataGridViewImageColumn();
                    imgCol.Name = "LogoImage";
                    imgCol.HeaderText = "Logo";
                    imgCol.ImageLayout = DataGridViewImageCellLayout.Zoom;
                    dgvPedidos.Columns.Add(imgCol);
                }

                // Cargar imágenes visuales en la columna LogoImage
                foreach (DataGridViewRow row in dgvPedidos.Rows)
                {
                    if (row.Cells["Logo"].Value != DBNull.Value && row.Cells["Logo"].Value != null)
                    {
                        try
                        {
                            byte[] bytes;

                            if (row.Cells["Logo"].Value is byte[])
                            {
                                bytes = (byte[])row.Cells["Logo"].Value;
                            }
                            else if (row.Cells["Logo"].Value is System.Drawing.Image)
                            {
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    ((System.Drawing.Image)row.Cells["Logo"].Value).Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                    bytes = ms.ToArray();
                                }
                            }
                            else continue;

                            using (MemoryStream ms = new MemoryStream(bytes))
                            {
                                row.Cells["LogoImage"].Value = System.Drawing.Image.FromStream(ms);
                            }
                        }
                        catch
                        {
                            row.Cells["LogoImage"].Value = null; // Por si falla
                        }
                    }
                }

            }
        }

        private byte[] ImagenABytes(System.Drawing.Image imagen)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                imagen.Save(ms, imagen.RawFormat);
                return ms.ToArray();
            }
        }

        private void btnCargarLogo_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Archivos de imagen|*.jpg;*.jpeg;*.png;*.bmp";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    picLogo.Image = System.Drawing.Image.FromFile(ofd.FileName);
                    picLogo.SizeMode = PictureBoxSizeMode.Zoom;
                }
            }
        }

        private void btnAgregarPrenda_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTipoPrenda.Text) || string.IsNullOrWhiteSpace(txtColor.Text) || string.IsNullOrWhiteSpace(txtCantidad.Text))
            {
                MessageBox.Show("Completa Tipo de prenda, Color y Cantidad.");
                return;
            }

            dgvDetallePedido.Rows.Add(txtTipoPrenda.Text, txtColor.Text, txtCantidad.Text, txtBordadoPrenda.Text);
            txtTipoPrenda.Clear();
            txtColor.Clear();
            txtCantidad.Clear();
            txtBordadoPrenda.Clear();
        }

        private void btnActualizarPrenda_Click(object sender, EventArgs e)
        {
            if (filaSeleccionadaDetalle >= 0)
            {
                dgvDetallePedido.Rows[filaSeleccionadaDetalle].Cells["TipoPrenda"].Value = txtTipoPrenda.Text;
                dgvDetallePedido.Rows[filaSeleccionadaDetalle].Cells["Color"].Value = txtColor.Text;
                dgvDetallePedido.Rows[filaSeleccionadaDetalle].Cells["Cantidad"].Value = txtCantidad.Text;
                dgvDetallePedido.Rows[filaSeleccionadaDetalle].Cells["Bordado"].Value = txtBordadoPrenda.Text;
                filaSeleccionadaDetalle = -1;
                txtTipoPrenda.Clear();
                txtColor.Clear();
                txtCantidad.Clear();
                txtBordadoPrenda.Clear();
                MessageBox.Show("🧵 Prenda actualizada.");
            }
            else
            {
                MessageBox.Show("Selecciona una prenda primero.");
            }
        }

        private void dgvDetallePedido_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && !dgvDetallePedido.Rows[e.RowIndex].IsNewRow)
            {
                filaSeleccionadaDetalle = e.RowIndex;
                DataGridViewRow row = dgvDetallePedido.Rows[e.RowIndex];
                txtTipoPrenda.Text = row.Cells["TipoPrenda"].Value?.ToString();
                txtColor.Text = row.Cells["Color"].Value?.ToString();
                txtCantidad.Text = row.Cells["Cantidad"].Value?.ToString();
                txtBordadoPrenda.Text = row.Cells["Bordado"].Value?.ToString();
            }
        }

        private void btnGuardarPedido_Click(object sender, EventArgs e)
        {
            if (cmbCliente.SelectedIndex == -1 || string.IsNullOrWhiteSpace(txtCajas.Text) || string.IsNullOrWhiteSpace(txtCosto.Text))
            {
                MessageBox.Show("Completa cliente, cajas y costo.");
                return;
            }

            using (SqlConnection cn = Conexion.Conectar())
            {
                cn.Open();
                SqlTransaction trans = cn.BeginTransaction();

                try
                {
                    SqlCommand cmd = new SqlCommand(@"INSERT INTO Pedidos (ClienteId, Fecha, Cajas, Bordado, Logo, Estado, Costo)
                        VALUES (@ClienteId, @Fecha, @Cajas, @Bordado, @Logo, @Estado, @Costo);
                        SELECT SCOPE_IDENTITY();", cn, trans);

                    cmd.Parameters.AddWithValue("@ClienteId", cmbCliente.SelectedValue);
                    cmd.Parameters.AddWithValue("@Fecha", dtpFecha.Value);
                    cmd.Parameters.AddWithValue("@Cajas", txtCajas.Text);
                    cmd.Parameters.AddWithValue("@Bordado", txtBordado.Text);
                    cmd.Parameters.AddWithValue("@Logo", picLogo.Image != null ? ImagenABytes(picLogo.Image) : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Estado", cmbEstado.Text);
                    cmd.Parameters.AddWithValue("@Costo", decimal.Parse(txtCosto.Text));

                    int pedidoId = Convert.ToInt32(cmd.ExecuteScalar());

                    foreach (DataGridViewRow row in dgvDetallePedido.Rows)
                    {
                        if (row.IsNewRow) continue;
                        SqlCommand detalle = new SqlCommand("INSERT INTO DetallePedido (PedidoId, TipoPrenda, Color, Cantidad, Bordado) VALUES (@PedidoId, @TipoPrenda, @Color, @Cantidad, @Bordado)", cn, trans);
                        detalle.Parameters.AddWithValue("@PedidoId", pedidoId);
                        detalle.Parameters.AddWithValue("@TipoPrenda", row.Cells[0].Value ?? "");
                        detalle.Parameters.AddWithValue("@Color", row.Cells[1].Value ?? "");
                        detalle.Parameters.AddWithValue("@Cantidad", row.Cells[2].Value ?? "0");
                        detalle.Parameters.AddWithValue("@Bordado", row.Cells[3].Value ?? "");
                        detalle.ExecuteNonQuery();
                    }

                    trans.Commit();
                    MessageBox.Show("✅ Pedido guardado correctamente.");
                    CargarPedidos();
                    Limpiar();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    MessageBox.Show("❌ Error: " + ex.Message);
                }
            }
        }

        private void dgvPedidos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvPedidos.Rows[e.RowIndex];

                // Validaciones nulas para evitar errores con DBNull
                cmbCliente.Text = row.Cells["Cliente"].Value?.ToString() ?? "";

                if (row.Cells["Fecha"].Value != DBNull.Value)
                    dtpFecha.Value = Convert.ToDateTime(row.Cells["Fecha"].Value);
                else
                    dtpFecha.Value = DateTime.Now;

                txtCajas.Text = row.Cells["Cajas"].Value?.ToString() ?? "";
                txtBordado.Text = row.Cells["Bordado"].Value?.ToString() ?? "";
                cmbEstado.Text = row.Cells["Estado"].Value?.ToString() ?? "";
                txtCosto.Text = row.Cells["Costo"].Value?.ToString() ?? "";

                // Validar si viene el PedidoId correctamente
                if (row.Cells["PedidoId"].Value != DBNull.Value)
                {
                    int pedidoId = Convert.ToInt32(row.Cells["PedidoId"].Value);
                    CargarDetalleDelPedido(pedidoId);
                    MostrarDetalleEnFormulario(pedidoId);
                }
                else
                {
                    MessageBox.Show("⚠️ El pedido seleccionado no tiene un ID válido.");
                }
                if (row.Cells["Logo"].Value != DBNull.Value)
                {
                    byte[] bytes = (byte[])row.Cells["Logo"].Value;
                    using (MemoryStream ms = new MemoryStream(bytes))
                    {
                        picLogo.Image = System.Drawing.Image.FromStream(ms);
                        picLogo.SizeMode = PictureBoxSizeMode.Zoom;
                    }
                }
                else
                {
                    picLogo.Image = null;
                }
                // Mostrar imagen del logo en el PictureBox
                if (row.Cells["Logo"].Value != DBNull.Value && row.Cells["Logo"].Value != null)
                {
                    try
                    {
                        byte[] bytes = (byte[])row.Cells["Logo"].Value;
                        using (MemoryStream ms = new MemoryStream(bytes))
                        {
                            picLogo.Image = System.Drawing.Image.FromStream(ms);
                            picLogo.SizeMode = PictureBoxSizeMode.Zoom;
                        }
                    }
                    catch
                    {
                        picLogo.Image = null;
                    }
                }
                else
                {
                    picLogo.Image = null;
                }


            }

        }

        private void CargarDetalleDelPedido(int pedidoId)
        {
            dgvDetallePedido.Rows.Clear();

            using (SqlConnection cn = Conexion.Conectar())
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SELECT TipoPrenda, Color, Cantidad, Bordado FROM DetallePedido WHERE PedidoId = @PedidoId", cn);
                cmd.Parameters.AddWithValue("@PedidoId", pedidoId);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    dgvDetallePedido.Rows.Add(
                        reader["TipoPrenda"].ToString(),
                        reader["Color"].ToString(),
                        reader["Cantidad"].ToString(),
                        reader["Bordado"].ToString()
                    );
                }
            }
        }
        private void MostrarDetalleEnFormulario(int pedidoId)
        {
            using (SqlConnection cn = Conexion.Conectar())
            {
                string query = @"SELECT TOP 1 TipoPrenda, Color, Cantidad, Bordado 
                         FROM DetallePedido 
                         WHERE PedidoId = @PedidoId 
                         ORDER BY Id DESC"; // Para mostrar el más reciente

                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@PedidoId", pedidoId);
                cn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    txtTipoPrenda.Text = reader["TipoPrenda"].ToString();
                    txtColor.Text = reader["Color"].ToString();
                    txtCantidad.Text = reader["Cantidad"].ToString();
                    txtBordadoPrenda.Text = reader["Bordado"].ToString(); // Se arregla esto
                }
            }
        }


        private void btnEditarPedido_Click(object sender, EventArgs e)
        {
            if (dgvPedidos.CurrentRow != null)
            {
                int id = Convert.ToInt32(dgvPedidos.CurrentRow.Cells["PedidoId"].Value);

                using (SqlConnection cn = Conexion.Conectar())
                {
                    cn.Open();
                    SqlTransaction trans = cn.BeginTransaction();

                    try
                    {
                        SqlCommand cmd = new SqlCommand(@"UPDATE Pedidos SET ClienteId=@ClienteId, Fecha=@Fecha, Cajas=@Cajas, Bordado=@Bordado, Estado=@Estado, Costo=@Costo, Logo=@Logo WHERE Id=@Id", cn, trans);
                        cmd.Parameters.AddWithValue("@ClienteId", cmbCliente.SelectedValue);
                        cmd.Parameters.AddWithValue("@Fecha", dtpFecha.Value);
                        cmd.Parameters.AddWithValue("@Cajas", txtCajas.Text);
                        cmd.Parameters.AddWithValue("@Bordado", txtBordado.Text);
                        cmd.Parameters.AddWithValue("@Estado", cmbEstado.Text);
                        cmd.Parameters.AddWithValue("@Costo", decimal.Parse(txtCosto.Text));
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.Parameters.AddWithValue("@Logo", picLogo.Image != null ? ImagenABytes(picLogo.Image) : (object)DBNull.Value);
                        cmd.ExecuteNonQuery();

                        SqlCommand delDetalle = new SqlCommand("DELETE FROM DetallePedido WHERE PedidoId = @Id", cn, trans);
                        delDetalle.Parameters.AddWithValue("@Id", id);
                        delDetalle.ExecuteNonQuery();

                        foreach (DataGridViewRow row in dgvDetallePedido.Rows)
                        {
                            if (row.IsNewRow) continue;
                            SqlCommand detalleCmd = new SqlCommand("INSERT INTO DetallePedido (PedidoId, TipoPrenda, Color, Cantidad, Bordado) VALUES (@PedidoId, @TipoPrenda, @Color, @Cantidad, @Bordado)", cn, trans);
                            detalleCmd.Parameters.AddWithValue("@PedidoId", id);
                            detalleCmd.Parameters.AddWithValue("@TipoPrenda", row.Cells["TipoPrenda"].Value ?? "");
                            detalleCmd.Parameters.AddWithValue("@Color", row.Cells["Color"].Value ?? "");
                            detalleCmd.Parameters.AddWithValue("@Cantidad", row.Cells["Cantidad"].Value ?? "0");
                            detalleCmd.Parameters.AddWithValue("@Bordado", row.Cells["Bordado"].Value ?? "");
                            detalleCmd.ExecuteNonQuery();
                        }

                        trans.Commit();
                        MessageBox.Show("✅ Pedido actualizado correctamente.");
                        CargarPedidos();
                        Limpiar();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        MessageBox.Show("❌ Error al actualizar: " + ex.Message);
                    }
                }
            }
        }

        private void btnEliminarPedido_Click(object sender, EventArgs e)
        {
            if (dgvPedidos.CurrentRow != null)
            {
                int id = Convert.ToInt32(dgvPedidos.CurrentRow.Cells["PedidoId"].Value);
                if (MessageBox.Show("¿Estás seguro que deseas eliminar este pedido?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    using (SqlConnection cn = Conexion.Conectar())
                    {
                        cn.Open();
                        SqlCommand deleteDetalle = new SqlCommand("DELETE FROM DetallePedido WHERE PedidoId = @Id", cn);
                        deleteDetalle.Parameters.AddWithValue("@Id", id);
                        deleteDetalle.ExecuteNonQuery();

                        SqlCommand deletePedido = new SqlCommand("DELETE FROM Pedidos WHERE Id = @Id", cn);
                        deletePedido.Parameters.AddWithValue("@Id", id);
                        deletePedido.ExecuteNonQuery();
                    }

                    MessageBox.Show("🗑️ Pedido eliminado correctamente.");
                    CargarPedidos();
                    Limpiar();
                }
            }
        }

        private void txtBuscarPedido_TextChanged(object sender, EventArgs e)
        {
            if (dgvPedidos.DataSource is DataTable dt)
            {
                string filtro = txtBuscarPedido.Text.Trim();
                dt.DefaultView.RowFilter = $"Convert(PedidoId, 'System.String') LIKE '%{filtro}%'";
            }
        }

        private void btnExportarPdf_Click(object sender, EventArgs e)
        {
            ExportarPedidosAPdf();
        }


        private void btnExportarExcel_Click(object sender, EventArgs e)
        {
            if (dgvPedidos.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar.");
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel Workbook|*.xlsx";
                sfd.FileName = $"Reporte_Pedidos_{DateTime.Now:yyyyMMdd}.xlsx";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        var ws = wb.Worksheets.Add("Pedidos");

                        // Título
                        ws.Cell("A1").Value = "PROMOBORDADOS - Reporte de Pedidos";
                        ws.Cell("A1").Style.Font.Bold = true;
                        ws.Cell("A1").Style.Font.FontSize = 16;
                        ws.Range("A1:H1").Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        // Fecha
                        ws.Cell("A2").Value = $"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}";
                        ws.Range("A2:H2").Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("A2").Style.Font.Italic = true;

                        // Encabezados
                        string[] headers = { "PedidoId", "Cliente", "Fecha", "Cajas", "Estado", "Costo", "Cantidad", "TotalPedido" };
                        for (int i = 0; i < headers.Length; i++)
                        {
                            ws.Cell(4, i + 1).Value = headers[i];
                            ws.Cell(4, i + 1).Style.Font.Bold = true;
                            ws.Cell(4, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
                            ws.Cell(4, i + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        }

                        // Datos
                        decimal totalGeneral = 0;
                        for (int i = 0; i < dgvPedidos.Rows.Count; i++)
                        {
                            var row = dgvPedidos.Rows[i];

                            int col = 1;
                            ws.Cell(i + 5, col++).Value = row.Cells["PedidoId"].Value?.ToString();
                            ws.Cell(i + 5, col++).Value = row.Cells["Cliente"].Value?.ToString();
                            ws.Cell(i + 5, col++).Value = Convert.ToDateTime(row.Cells["Fecha"].Value).ToShortDateString();
                            ws.Cell(i + 5, col++).Value = row.Cells["Cajas"].Value?.ToString();
                            ws.Cell(i + 5, col++).Value = row.Cells["Estado"].Value?.ToString();
                            ws.Cell(i + 5, col++).Value = row.Cells["Costo"].Value?.ToString();
                            ws.Cell(i + 5, col++).Value = row.Cells["Cantidad"].Value?.ToString();

                            // Calcular Total por pedido
                            decimal costo = Convert.ToDecimal(row.Cells["Costo"].Value);
                            int cantidad = Convert.ToInt32(row.Cells["Cantidad"].Value);
                            decimal totalPedido = costo * cantidad;
                            ws.Cell(i + 5, col++).Value = totalPedido;

                            totalGeneral += totalPedido;
                        }

                        int filaTotal = dgvPedidos.Rows.Count + 6;
                        ws.Cell(filaTotal, 6).Value = "TOTAL GENERAL:";
                        ws.Cell(filaTotal, 6).Style.Font.Bold = true;
                        ws.Cell(filaTotal, 7).Value = $"${totalGeneral:N0}";
                        ws.Cell(filaTotal, 7).Style.Font.Bold = true;

                        ws.Columns().AdjustToContents();
                        wb.SaveAs(sfd.FileName);
                    }

                    MessageBox.Show("✅ Excel exportado correctamente.");
                }
            }
        }

        private void ExportarPedidosAPdf()
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Archivo PDF (*.pdf)|*.pdf";
                sfd.FileName = "ReportePedidos.pdf";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Document doc = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
                        PdfWriter.GetInstance(doc, new FileStream(sfd.FileName, FileMode.Create));
                        doc.Open();

                        // Título
                        Paragraph titulo = new Paragraph("Reporte de Pedidos",
                            new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 18, iTextSharp.text.Font.BOLD));
                        titulo.Alignment = Element.ALIGN_CENTER;
                        doc.Add(titulo);
                        doc.Add(new Paragraph(" ")); // Espacio

                        // Crear tabla
                        PdfPTable table = new PdfPTable(12); // 11 columnas + TotalPedido
                        table.WidthPercentage = 100;

                        // Encabezados
                        string[] headers = { "PedidoId", "Cliente", "Fecha", "Cajas", "Bordado", "Estado", "Costo", "TipoPrenda", "Color", "Cantidad", "BordadoDetalle", "TotalPedido" };
                        foreach (var h in headers)
                        {
                            PdfPCell cell = new PdfPCell(new Phrase(h, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.BOLD)));
                            cell.BackgroundColor = new BaseColor(255, 204, 0);
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            table.AddCell(cell);
                        }

                        decimal totalGeneral = 0;

                        using (SqlConnection cn = Conexion.Conectar())
                        {
                            string query = @"
                        SELECT 
                            p.Id AS PedidoId,
                            c.Nombre AS Cliente,
                            p.Fecha,
                            p.Cajas,
                            p.Bordado,
                            p.Estado,
                            p.Costo,
                            d.TipoPrenda,
                            d.Color,
                            d.Cantidad,
                            d.Bordado AS BordadoDetalle
                        FROM Pedidos p
                        INNER JOIN Clientes c ON p.ClienteId = c.Id
                        INNER JOIN DetallePedido d ON p.Id = d.PedidoId
                        ORDER BY p.Id DESC";

                            SqlCommand cmd = new SqlCommand(query, cn);
                            cn.Open();
                            SqlDataReader reader = cmd.ExecuteReader();

                            while (reader.Read())
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    table.AddCell(reader[i].ToString());
                                }

                                // Cálculo de total por pedido (Costo x Cantidad)
                                decimal costo = Convert.ToDecimal(reader["Costo"]);
                                int cantidad = Convert.ToInt32(reader["Cantidad"]);
                                decimal totalPedido = costo * cantidad;

                                table.AddCell(totalPedido.ToString("N2"));
                                totalGeneral += totalPedido;
                            }
                        }

                        doc.Add(table);

                        // Total general
                        doc.Add(new Paragraph(" "));
                        Paragraph totalParrafo = new Paragraph("TOTAL GENERAL: $" + totalGeneral.ToString("N2"),
                            new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.BOLD));
                        totalParrafo.Alignment = Element.ALIGN_RIGHT;
                        doc.Add(totalParrafo);

                        doc.Close();

                        MessageBox.Show("✅ Reporte PDF generado correctamente.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("❌ Error al exportar PDF: " + ex.Message);
                    }
                }
            }
        }
        private void GenerarFacturaPDF(int pedidoId)
        {
            using (SqlConnection cn = Conexion.Conectar())
            {
                cn.Open();

                // Obtener datos del pedido y cliente
                string query = @"SELECT p.Fecha, p.Bordado AS BordadoGeneral, p.Costo, p.Cajas, p.Estado,
                                c.Nombre AS Cliente, c.RUT, c.Direccion, c.Telefono, c.Correo
                         FROM Pedidos p
                         INNER JOIN Clientes c ON p.ClienteId = c.Id
                         WHERE p.Id = @PedidoId";

                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@PedidoId", pedidoId);
                SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.Read())
                {
                    MessageBox.Show("❌ No se encontró el pedido.");
                    return;
                }

                string cliente = reader["Cliente"].ToString();
                string rut = reader["RUT"].ToString();
                string direccion = reader["Direccion"].ToString();
                string telefono = reader["Telefono"].ToString();
                string correo = reader["Correo"].ToString();
                string estado = reader["Estado"].ToString();
                string bordadoGeneral = reader["BordadoGeneral"].ToString();
                DateTime fecha = Convert.ToDateTime(reader["Fecha"]);
                decimal costo = Convert.ToDecimal(reader["Costo"]);
                int cajas = Convert.ToInt32(reader["Cajas"]);
                reader.Close();

                // Obtener detalles del pedido
                SqlCommand cmdDetalle = new SqlCommand(@"SELECT TipoPrenda, Color, Cantidad, Bordado 
                                                 FROM DetallePedido 
                                                 WHERE PedidoId = @PedidoId", cn);
                cmdDetalle.Parameters.AddWithValue("@PedidoId", pedidoId);
                SqlDataReader detalleReader = cmdDetalle.ExecuteReader();

                var detalles = new List<(string Tipo, string Color, int Cantidad, string Bordado)>();
                while (detalleReader.Read())
                {
                    detalles.Add((
                        detalleReader["TipoPrenda"].ToString(),
                        detalleReader["Color"].ToString(),
                        Convert.ToInt32(detalleReader["Cantidad"]),
                        detalleReader["Bordado"].ToString()
                    ));
                }
                detalleReader.Close();

                // Obtener número de factura
                int numeroFactura = ObtenerNuevoNumeroFactura(cn);
                SqlCommand cmdCliente = new SqlCommand("SELECT Nombre, RUT, Direccion, Telefono, Correo FROM Clientes WHERE Nombre = @Nombre", cn);
                cmdCliente.Parameters.AddWithValue("@Nombre", cliente);
                SqlDataReader datosCliente = cmdCliente.ExecuteReader();
                datosCliente.Read();
                rut = datosCliente["RUT"].ToString();
                direccion = datosCliente["Direccion"].ToString();
                telefono = datosCliente["Telefono"].ToString();
                correo = datosCliente["Correo"].ToString();
                datosCliente.Close();



                // Crear archivo PDF
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Archivo PDF (*.pdf)|*.pdf";
                sfd.FileName = $"Factura_FAPB{numeroFactura:D3}.pdf";

                if (sfd.ShowDialog() != DialogResult.OK)
                    return;

                string rutaArchivo = sfd.FileName;


                Document doc = new Document(PageSize.A4, 40, 40, 40, 40);
                PdfWriter.GetInstance(doc, new FileStream(rutaArchivo, FileMode.Create));
                doc.Open();

                // LOGO
                string rutaLogo = Path.Combine(Application.StartupPath, "Resources", "Logo.png");
                if (File.Exists(rutaLogo))
                {
                    var logo = iTextSharp.text.Image.GetInstance(rutaLogo);
                    logo.ScaleAbsolute(120, 60);
                    logo.Alignment = Element.ALIGN_RIGHT;
                    doc.Add(logo);
                }

                // ENCABEZADO
                PdfPTable headerTable = new PdfPTable(2);
                headerTable.WidthPercentage = 100;
                headerTable.SetWidths(new float[] { 70, 30 });

                var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
                var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 9);
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);

                PdfPCell leftCell = new PdfPCell();
                leftCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                leftCell.AddElement(new Paragraph("RAZÓN SOCIAL EMPRESA", boldFont));
                leftCell.AddElement(new Paragraph("RUT: xx.xxx.xxx-x", normalFont));
                leftCell.AddElement(new Paragraph("DIRECCIÓN: Pendiente", normalFont));
                leftCell.AddElement(new Paragraph("TELÉFONO: +56 9 00000000", normalFont));
                leftCell.AddElement(new Paragraph("EMAIL: correo@empresa.cl", normalFont));

                PdfPCell rightCell = new PdfPCell();
                rightCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                rightCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                rightCell.AddElement(new Paragraph("DOCUMENTO:", boldFont));
                rightCell.AddElement(new Paragraph("FACTURA", titleFont));
                rightCell.AddElement(new Paragraph("N° FAPB" + numeroFactura.ToString("D3"), normalFont));
                rightCell.AddElement(new Paragraph("Fecha: " + DateTime.Now.ToString("dd-MM-yyyy"), normalFont));

                headerTable.AddCell(leftCell);
                headerTable.AddCell(rightCell);
                doc.Add(headerTable);
                doc.Add(new Paragraph(" ")); // Espacio

                // DATOS DEL CLIENTE
                PdfPTable clienteTable = new PdfPTable(1);
                clienteTable.WidthPercentage = 100;
                clienteTable.SpacingBefore = 10;

                clienteTable.AddCell(new Phrase($"Cliente: {cliente}", normalFont));
                clienteTable.AddCell(new Phrase($"RUT: {rut}", normalFont));
                clienteTable.AddCell(new Phrase($"Dirección: {direccion}", normalFont));
                clienteTable.AddCell(new Phrase($"Teléfono: {telefono}", normalFont));
                clienteTable.AddCell(new Phrase($"Correo: {correo}", normalFont));
                clienteTable.AddCell(new Phrase($"Fecha Pedido: {fecha}", normalFont));
                clienteTable.AddCell(new Phrase($"Estado: {estado}", normalFont));
                clienteTable.AddCell(new Phrase($"Bordado General: {bordadoGeneral}", normalFont));
                doc.Add(clienteTable);
                doc.Add(new Paragraph(" "));

                // TABLA DETALLE
                PdfPTable detalleTable = new PdfPTable(6);
                detalleTable.WidthPercentage = 100;
                detalleTable.SpacingBefore = 10;
                detalleTable.SetWidths(new float[] { 20, 15, 10, 25, 15, 15 });

                string[] headers = { "Tipo Prenda", "Color", "Cantidad", "Bordado", "Costo", "Total" };
                foreach (string h in headers)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(h, boldFont));
                    cell.BackgroundColor = new BaseColor(255, 204, 0);
                    detalleTable.AddCell(cell);
                }

                decimal subtotal = 0;
                foreach (var d in detalles)
                {
                    decimal totalPrenda = d.Cantidad * costo;
                    subtotal += totalPrenda;

                    detalleTable.AddCell(new Phrase(d.Tipo, normalFont));
                    detalleTable.AddCell(new Phrase(d.Color, normalFont));
                    detalleTable.AddCell(new Phrase(d.Cantidad.ToString(), normalFont));
                    detalleTable.AddCell(new Phrase(d.Bordado, normalFont));
                    detalleTable.AddCell(new Phrase("$" + costo.ToString("N0"), normalFont));
                    detalleTable.AddCell(new Phrase("$" + totalPrenda.ToString("N0"), normalFont));
                }

                doc.Add(detalleTable);

                // TOTALES
                decimal iva = subtotal * 0.19M;
                decimal total = subtotal + iva;

                PdfPTable totales = new PdfPTable(2);
                totales.WidthPercentage = 40;
                totales.HorizontalAlignment = Element.ALIGN_RIGHT;
                totales.SetWidths(new float[] { 50, 50 });
                totales.SpacingBefore = 10;

                totales.AddCell(new Phrase("Subtotal", boldFont));
                totales.AddCell(new Phrase("$" + subtotal.ToString("N0"), normalFont));

                totales.AddCell(new Phrase("IVA 19%", boldFont));
                totales.AddCell(new Phrase("$" + iva.ToString("N0"), normalFont));

                totales.AddCell(new Phrase("TOTAL", boldFont));
                totales.AddCell(new Phrase("$" + total.ToString("N0"), normalFont));

                doc.Add(totales);
                doc.Close();

                // GUARDAR EN BASE DE DATOS
                SqlCommand cmdInsert = new SqlCommand("INSERT INTO Facturas (Numero, PedidoId, Fecha, RutaArchivo, Cliente, RUT, Direccion, Telefono, Correo, Monto, Observaciones) VALUES (@Numero, @PedidoId, @Fecha, @Ruta, @Cliente, @RUT, @Direccion, @Telefono, @Correo, @Monto, @Observaciones)", cn);
                cmdInsert.Parameters.AddWithValue("@Numero", numeroFactura);
                cmdInsert.Parameters.AddWithValue("@PedidoId", pedidoId);
                cmdInsert.Parameters.AddWithValue("@Fecha", DateTime.Now);
                cmdInsert.Parameters.AddWithValue("@Ruta", rutaArchivo);
                cmdInsert.Parameters.AddWithValue("@Cliente", cliente);
                cmdInsert.Parameters.AddWithValue("@RUT", rut);
                cmdInsert.Parameters.AddWithValue("@Direccion", direccion);
                cmdInsert.Parameters.AddWithValue("@Telefono", telefono);
                cmdInsert.Parameters.AddWithValue("@Correo", correo);
                cmdInsert.Parameters.AddWithValue("@Monto", total);
                cmdInsert.Parameters.AddWithValue("@Observaciones", "Generada desde módulo Pedidos");
                cmdInsert.ExecuteNonQuery();

                MessageBox.Show("✅ Factura generada exitosamente.");
            }
        }

        private void GenerarGuiaPDF(int pedidoId)
        {
            try
            {
                using (SqlConnection cn = Conexion.Conectar())
                {
                    cn.Open();

                    SqlCommand cmd = new SqlCommand(@"
                SELECT p.Id, c.Nombre, c.Direccion, c.Telefono, c.Correo, c.RUT,
                       p.Bordado, p.Estado, p.Fecha, p.Costo, p.Cajas
                FROM Pedidos p
                INNER JOIN Clientes c ON p.ClienteId = c.Id
                WHERE p.Id = @PedidoId", cn);
                    cmd.Parameters.AddWithValue("@PedidoId", pedidoId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (!reader.Read()) return;

                    string cliente = reader["Nombre"].ToString();
                    string rutCliente = reader["RUT"].ToString();
                    string direccion = reader["Direccion"].ToString();
                    string telefono = reader["Telefono"].ToString();
                    string correo = reader["Correo"].ToString();
                    string estado = reader["Estado"].ToString();
                    string bordadoGeneral = reader["Bordado"].ToString();
                    DateTime fecha = Convert.ToDateTime(reader["Fecha"]);
                    decimal costo = Convert.ToDecimal(reader["Costo"]);
                    int cajas = Convert.ToInt32(reader["Cajas"]);
                    reader.Close();

                    SqlCommand cmdDetalle = new SqlCommand(@"SELECT TipoPrenda, Color, Cantidad, Bordado 
                                                     FROM DetallePedido WHERE PedidoId = @PedidoId", cn);
                    cmdDetalle.Parameters.AddWithValue("@PedidoId", pedidoId);
                    SqlDataReader detalleReader = cmdDetalle.ExecuteReader();

                    var detalles = new List<(string Tipo, string Color, int Cantidad, string Bordado)>();
                    while (detalleReader.Read())
                    {
                        detalles.Add((
                            detalleReader["TipoPrenda"].ToString(),
                            detalleReader["Color"].ToString(),
                            Convert.ToInt32(detalleReader["Cantidad"]),
                            detalleReader["Bordado"].ToString()
                        ));
                    }
                    detalleReader.Close();

                    int numeroGuia = ObtenerNuevoNumeroGuia(cn);
                    SqlCommand cmdClienteG = new SqlCommand("SELECT Nombre, RUT, Direccion, Telefono, Correo FROM Clientes WHERE Nombre = @Nombre", cn);
                    cmdClienteG.Parameters.AddWithValue("@Nombre", cliente);
                    SqlDataReader datosClienteG = cmdClienteG.ExecuteReader();
                    datosClienteG.Read();
                    string rutG = datosClienteG["RUT"].ToString();
                    string direccionG = datosClienteG["Direccion"].ToString();
                    string telefonoG = datosClienteG["Telefono"].ToString();
                    string correoG = datosClienteG["Correo"].ToString();
                    datosClienteG.Close();


                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter = "Archivo PDF (*.pdf)|*.pdf";
                    sfd.FileName = $"Guia_GDPB{numeroGuia:D3}.pdf";
                    if (sfd.ShowDialog() != DialogResult.OK) return;

                    string rutaArchivo = sfd.FileName;

                    Document doc = new Document(PageSize.A4, 40, 40, 40, 40);
                    PdfWriter.GetInstance(doc, new FileStream(rutaArchivo, FileMode.Create));
                    doc.Open();

                    string rutaLogo = Path.Combine(Application.StartupPath, "Resources", "Logo.png");
                    if (File.Exists(rutaLogo))
                    {
                        var logo = iTextSharp.text.Image.GetInstance(rutaLogo);
                        logo.ScaleAbsolute(120, 60);
                        logo.Alignment = Element.ALIGN_RIGHT;
                        doc.Add(logo);
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
                    rightCell.AddElement(new Paragraph("GUÍA DE DESPACHO", boldFont));
                    rightCell.AddElement(new Paragraph($"N° GDPB{numeroGuia:D3}", normalFont));
                    rightCell.AddElement(new Paragraph($"Fecha: {fecha:dd-MM-yyyy}", normalFont));
                    
                    headerTable.AddCell(leftCell);
                    headerTable.AddCell(rightCell);
                    doc.Add(headerTable);
                    doc.Add(new Paragraph(" "));

                    PdfPTable infoCliente = new PdfPTable(1) { WidthPercentage = 100 };
                    infoCliente.AddCell(new Phrase($"Cliente: {cliente}", normalFont));
                    infoCliente.AddCell(new Phrase($"RUT: {rutCliente}", normalFont));
                    infoCliente.AddCell(new Phrase($"Dirección: {direccion}", normalFont));
                    infoCliente.AddCell(new Phrase($"Teléfono: {telefono}", normalFont));
                    infoCliente.AddCell(new Phrase($"Correo: {correo}", normalFont));
                    infoCliente.AddCell(new Phrase($"Fecha Pedido: {fecha}", normalFont));
                    infoCliente.AddCell(new Phrase($"Estado: {estado}", normalFont));
                    infoCliente.AddCell(new Phrase($"Bordado General: {bordadoGeneral}", normalFont));
                    doc.Add(infoCliente);
                    doc.Add(new Paragraph(" "));

                    PdfPTable detalle = new PdfPTable(6);
                    detalle.WidthPercentage = 100;
                    detalle.SetWidths(new float[] { 20, 20, 15, 25, 10, 10 });

                    string[] headers = { "Tipo Prenda", "Color", "Cantidad", "Bordado", "Costo", "Total" };
                    foreach (var h in headers)
                        detalle.AddCell(new PdfPCell(new Phrase(h, boldFont)) { BackgroundColor = new BaseColor(255, 204, 0) });

                    decimal total = 0;
                    foreach (var item in detalles)
                    {
                        decimal totalItem = costo * item.Cantidad;
                        total += totalItem;

                        detalle.AddCell(new Phrase(item.Tipo, normalFont));
                        detalle.AddCell(new Phrase(item.Color, normalFont));
                        detalle.AddCell(new Phrase(item.Cantidad.ToString(), normalFont));
                        detalle.AddCell(new Phrase(item.Bordado, normalFont));
                        detalle.AddCell(new Phrase($"${costo:N0}", normalFont));
                        detalle.AddCell(new Phrase($"${totalItem:N0}", normalFont));
                    }
                    doc.Add(detalle);
                    doc.Add(new Paragraph(" "));

                    decimal iva = total * 0.19m;
                    decimal totalFinal = total + iva;

                    PdfPTable totales = new PdfPTable(2);
                    totales.WidthPercentage = 40;
                    totales.HorizontalAlignment = Element.ALIGN_RIGHT;
                    totales.SetWidths(new float[] { 50, 50 });

                    totales.AddCell(new Phrase("Subtotal", boldFont));
                    totales.AddCell(new Phrase($"${total:N0}", normalFont));
                    totales.AddCell(new Phrase("IVA 19%", boldFont));
                    totales.AddCell(new Phrase($"${iva:N0}", normalFont));
                    totales.AddCell(new Phrase("TOTAL", boldFont));
                    totales.AddCell(new Phrase($"${totalFinal:N0}", normalFont));
                    doc.Add(totales);

                    doc.Close();

                    SqlCommand insertar = new SqlCommand("INSERT INTO GuiasDespacho (Numero, PedidoId, RutaArchivo, FechaGeneracion, Cliente, RUT, Direccion, Telefono, Correo, Monto, Observaciones) VALUES (@Numero, @PedidoId, @Ruta, @FechaGeneracion, @Cliente, @RUT, @Direccion, @Telefono, @Correo, @Monto, @Observaciones)", cn);
                    insertar.Parameters.AddWithValue("@Numero", numeroGuia);
                    insertar.Parameters.AddWithValue("@PedidoId", pedidoId);
                    insertar.Parameters.AddWithValue("@Ruta", rutaArchivo);
                    insertar.Parameters.AddWithValue("@FechaGeneracion", DateTime.Now);
                    insertar.Parameters.AddWithValue("@Cliente", cliente);
                    insertar.Parameters.AddWithValue("@RUT", rutG);
                    insertar.Parameters.AddWithValue("@Direccion", direccionG);
                    insertar.Parameters.AddWithValue("@Telefono", telefonoG);
                    insertar.Parameters.AddWithValue("@Correo", correoG);
                    insertar.Parameters.AddWithValue("@Monto", totalFinal);
                    insertar.Parameters.AddWithValue("@Observaciones", "Generada desde módulo Pedidos");
                    insertar.ExecuteNonQuery();


                    MessageBox.Show("🚚 Guía generada correctamente.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Error al generar guía: " + ex.Message);
            }
        }

        private void btnGenerarFactura_Click(object sender, EventArgs e)
        {
            if (dgvPedidos.CurrentRow != null)
            {
                int pedidoId = Convert.ToInt32(dgvPedidos.CurrentRow.Cells["PedidoId"].Value);
                GenerarFacturaPDF(pedidoId);
            }
        }


        private void btnGenerarGuia_Click(object sender, EventArgs e)
        {
            if (dgvPedidos.CurrentRow != null)
            {
                int pedidoId = Convert.ToInt32(dgvPedidos.CurrentRow.Cells["PedidoId"].Value);
                GenerarGuiaPDF(pedidoId);
            }
        }
        private int ObtenerNuevoNumeroFactura(SqlConnection cn)
        {
            SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(Numero), 0) + 1 FROM Facturas", cn);
            return (int)cmd.ExecuteScalar();
        }
        private int ObtenerNuevoNumeroGuia(SqlConnection cn)
        {
            SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(Numero), 0) + 1 FROM GuiasDespacho", cn);
            return (int)cmd.ExecuteScalar();
        }
    }
}
