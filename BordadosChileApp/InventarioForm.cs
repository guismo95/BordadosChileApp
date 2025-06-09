// InventarioForm.cs - COMPLETO Y ACTUALIZADO con cmbProveedor autocompletable y filtro por cmbBuscar
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2013.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using DrawingFont = System.Drawing.Font;

namespace BordadosChileApp
{
    public partial class InventarioForm : Form
    {
        public InventarioForm()
        {
            InitializeComponent();
            this.BackColor = Color.WhiteSmoke;
            this.Font = new DrawingFont("Segoe UI", 10);
        }

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        int productoSeleccionadoId = -1;

        private void InventarioForm_Load(object sender, EventArgs e)
        {
            cmbTipo.Items.AddRange(new string[] { "Hilo", "Tela", "Accesorio" });
            cmbTipo.SelectedIndex = 0;

            cmbBuscar.Items.AddRange(new string[] { "Tipo", "Producto", "Unidad", "Proveedor" });
            cmbBuscar.SelectedIndex = 0;

            ConfigurarAutoCompletarProveedor();
            CargarProveedores();
            EstiloTabla(dgvInventario);
            MostrarInventario();

            txtBuscar.TextChanged += TxtBuscar_TextChanged;
        }

        private void ConfigurarAutoCompletarProveedor()
        {
            cmbProveedor.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cmbProveedor.AutoCompleteSource = AutoCompleteSource.CustomSource;
        }

        private void CargarProveedores()
        {
            cmbProveedor.Items.Clear();
            AutoCompleteStringCollection autoCompletar = new AutoCompleteStringCollection();

            using (SqlConnection cn = Conexion.Conectar())
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SELECT DISTINCT NombreProveedor FROM Proveedores WHERE NombreProveedor IS NOT NULL AND LTRIM(RTRIM(NombreProveedor)) != ''", cn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string proveedor = dr["NombreProveedor"].ToString();
                    if (!cmbProveedor.Items.Contains(proveedor))
                    {
                        cmbProveedor.Items.Add(proveedor);
                        autoCompletar.Add(proveedor);
                    }
                }
            }

            cmbProveedor.AutoCompleteCustomSource = autoCompletar;
        }


        private void MostrarInventario()
        {
            using (SqlConnection cn = Conexion.Conectar())
            {
                cn.Open();
                string query = "SELECT * FROM Inventario";
                SqlDataAdapter da = new SqlDataAdapter(query, cn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvInventario.DataSource = dt;
            }
        }

        private void Limpiar()
        {
            cmbTipo.SelectedIndex = 0;
            txtProducto.Text = "";
            txtUnidad.Text = "";
            cmbProveedor.SelectedIndex = -1;
            numCantidad.Value = 0;
            productoSeleccionadoId = -1;
            CargarProveedores();
        }

        private void dgvInventario_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvInventario.Rows[e.RowIndex].Cells["Id"].Value != DBNull.Value)
            {
                DataGridViewRow row = dgvInventario.Rows[e.RowIndex];
                cmbTipo.SelectedItem = row.Cells["Tipo"].Value?.ToString();
                txtProducto.Text = row.Cells["Producto"].Value?.ToString();
                numCantidad.Value = Convert.ToDecimal(row.Cells["Cantidad"].Value ?? 0);
                txtUnidad.Text = row.Cells["Unidad"].Value?.ToString();
                cmbProveedor.SelectedItem = row.Cells["Proveedor"].Value?.ToString();
                productoSeleccionadoId = Convert.ToInt32(row.Cells["Id"].Value);
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            string tipo = cmbTipo.Text;
            string producto = txtProducto.Text;
            int cantidad = (int)numCantidad.Value;
            string unidad = txtUnidad.Text;
            string proveedor = cmbProveedor.Text;

            if (string.IsNullOrWhiteSpace(tipo) || string.IsNullOrWhiteSpace(producto) ||
                string.IsNullOrWhiteSpace(unidad) || string.IsNullOrWhiteSpace(proveedor))
            {
                MessageBox.Show("Completa todos los campos.");
                return;
            }

            using (SqlConnection cn = Conexion.Conectar())
            {
                cn.Open();
                string query = "INSERT INTO Inventario (Tipo, Producto, Cantidad, Unidad, Proveedor) VALUES (@Tipo, @Producto, @Cantidad, @Unidad, @Proveedor)";
                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@Tipo", tipo);
                cmd.Parameters.AddWithValue("@Producto", producto);
                cmd.Parameters.AddWithValue("@Cantidad", cantidad);
                cmd.Parameters.AddWithValue("@Unidad", unidad);
                cmd.Parameters.AddWithValue("@Proveedor", proveedor);
                cmd.ExecuteNonQuery();
            }
            if (cantidad < 5)
            {
                MessageBox.Show("⚠️ Este producto tiene un stock muy bajo. Considera reponerlo pronto.", "Alerta de stock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }


            MessageBox.Show("Producto agregado ✅");
            MostrarInventario();
            Limpiar();
            CargarProveedores();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (productoSeleccionadoId == -1)
            {
                MessageBox.Show("Selecciona un producto primero.");
                return;
            }

            using (SqlConnection cn = Conexion.Conectar())
            {
                cn.Open();
                string query = "UPDATE Inventario SET Tipo=@Tipo, Producto=@Producto, Cantidad=@Cantidad, Unidad=@Unidad, Proveedor=@Proveedor WHERE Id=@Id";
                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@Tipo", cmbTipo.Text);
                cmd.Parameters.AddWithValue("@Producto", txtProducto.Text);
                cmd.Parameters.AddWithValue("@Cantidad", (int)numCantidad.Value);
                cmd.Parameters.AddWithValue("@Unidad", txtUnidad.Text);
                cmd.Parameters.AddWithValue("@Proveedor", cmbProveedor.Text);
                cmd.Parameters.AddWithValue("@Id", productoSeleccionadoId);
                cmd.ExecuteNonQuery();
            }
            if ((int)numCantidad.Value < 5)
            {
                MessageBox.Show("⚠️ Este producto tiene un stock muy bajo. Considera reponerlo pronto.", "Alerta de stock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }


            MessageBox.Show("Producto editado ✅");
            MostrarInventario();
            Limpiar();
        }
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (productoSeleccionadoId == -1)
            {
                MessageBox.Show("Selecciona un producto para eliminar.");
                return;
            }

            DialogResult confirm = MessageBox.Show(
                "¿Estás seguro de eliminar este producto del inventario?",
                "Confirmación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (confirm == DialogResult.Yes)
            {
                using (SqlConnection cn = Conexion.Conectar())
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM Inventario WHERE Id=@Id", cn);
                    cmd.Parameters.AddWithValue("@Id", productoSeleccionadoId);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("🗑️ Producto eliminado del inventario.");
                MostrarInventario();
                Limpiar();
            }
        }


        private void TxtBuscar_TextChanged(object sender, EventArgs e)
        {
            string columna = cmbBuscar.Text;
            string texto = txtBuscar.Text.Trim().ToLower();
            DataTable dt = dgvInventario.DataSource as DataTable;

            if (dt != null && !string.IsNullOrWhiteSpace(columna))
            {
                dt.DefaultView.RowFilter = string.Format("{0} LIKE '%{1}%'", columna, texto.Replace("'", "''"));
            }
        }

        // Estilo visual de la tabla
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

        // Pintar cantidad en rojo si es menor a 5
        private void dgvInventario_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvInventario.Columns[e.ColumnIndex].Name == "Cantidad")
            {
                if (e.Value != null && int.TryParse(e.Value.ToString(), out int cantidad))
                {
                    if (cantidad < 5)
                    {
                        dgvInventario.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightCoral;
                    }
                    else
                    {
                        dgvInventario.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                    }
                }
            }
        }

        // Método para exportar Excel filtrado
        private void btnExportarExcel_Click(object sender, EventArgs e)
        {
            if (dgvInventario.DataSource is DataTable originalTable)
            {
                DataView view = originalTable.DefaultView;
                DataTable dt = view.ToTable();

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No hay productos para exportar.");
                    return;
                }

                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "Excel Workbook|*.xlsx";
                    sfd.FileName = $"Reporte_Inventario_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        using (XLWorkbook wb = new XLWorkbook())
                        {
                            var ws = wb.Worksheets.Add("Inventario");

                            // Logo
                            var imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logo.png");
                            if (File.Exists(imagePath))
                            {
                                var image = ws.AddPicture(imagePath)
                                    .MoveTo(ws.Cell("A1"))
                                    .Scale(0.3);
                            }

                            // Encabezado
                            ws.Cell("A5").Value = "Reporte de Inventario - PROMOBORDADOS";
                            ws.Cell("A5").Style.Font.Bold = true;
                            ws.Cell("A5").Style.Font.FontSize = 16;
                            ws.Range("A5:F5").Merge().Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;

                            // Encabezados
                            for (int i = 0; i < dt.Columns.Count; i++)
                            {
                                ws.Cell(7, i + 1).Value = dt.Columns[i].ColumnName;
                                ws.Cell(7, i + 1).Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightGray;
                                ws.Cell(7, i + 1).Style.Font.Bold = true;
                            }

                            // Datos
                            for (int i = 0; i < dt.Rows.Count; i++)
                                for (int j = 0; j < dt.Columns.Count; j++)
                                    ws.Cell(i + 8, j + 1).Value = dt.Rows[i][j]?.ToString();

                            ws.Columns().AdjustToContents();
                            wb.SaveAs(sfd.FileName);
                        }

                        MessageBox.Show("✅ Excel exportado con formato y logo.");
                    }
                }
            }
            else
            {
                MessageBox.Show("No se pudo acceder a los datos de la tabla.");
            }
        }
        // Método para exportar PDF filtrado
        private void btnExportarPdf_Click(object sender, EventArgs e)
        {
            if (dgvInventario.DataSource is DataTable originalTable)
            {
                DataView view = originalTable.DefaultView;
                DataTable dt = view.ToTable();

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No hay productos para exportar.");
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
                            using (FileStream stream = new FileStream(sfd.FileName, FileMode.Create))
                            {
                                Document doc = new Document(PageSize.A4.Rotate(), 10f, 10f, 20f, 20f);
                                PdfWriter.GetInstance(doc, stream);
                                doc.Open();

                                var imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logo.png");
                                if (File.Exists(imagePath))
                                {
                                    iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(imagePath);
                                    logo.ScalePercent(30f);
                                    logo.Alignment = Element.ALIGN_LEFT;
                                    doc.Add(logo);
                                }

                                Paragraph title = new Paragraph("Reporte de Inventario - PROMOBORDADOS",
                                    FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16));
                                title.Alignment = Element.ALIGN_CENTER;
                                doc.Add(title);
                                doc.Add(new Paragraph(""));

                                PdfPTable pdfTable = new PdfPTable(dt.Columns.Count);
                                pdfTable.WidthPercentage = 100;

                                foreach (DataColumn col in dt.Columns)
                                {
                                    PdfPCell cell = new PdfPCell(new Phrase(col.ColumnName));
                                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                                    pdfTable.AddCell(cell);
                                }

                                foreach (DataRow row in dt.Rows)
                                {
                                    foreach (var item in row.ItemArray)
                                        pdfTable.AddCell(item?.ToString());
                                }

                                doc.Add(pdfTable);
                                doc.Close();
                                stream.Close();
                            }

                            MessageBox.Show("✅ PDF exportado con formato y logo.");
                        }
                        catch (IOException)
                        {
                            MessageBox.Show("El archivo ya está en uso. Ciérralo antes de exportar nuevamente.", "Error al exportar PDF", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("No se pudo acceder a los datos de la tabla.");
            }
        }
    }
}
