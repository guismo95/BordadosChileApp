// PagosPersonalForm.cs COMPLETO Y ACTUALIZADO
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using DrawingFont = System.Drawing.Font;

namespace BordadosChileApp
{
    public partial class PagosPersonalForm : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        private DataTable pagosTabla;

        private List<Empleado> listaEmpleados = new List<Empleado>();

        public PagosPersonalForm()
        {
            InitializeComponent();
            this.BackColor = Color.WhiteSmoke;
        }
        // Clase para manejar empleados internamente
        public class Empleado
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
            public string RutEmpleado { get; set; }
            public int SueldoBase { get; set; }
        }

        private void PagosPersonalForm_Load(object sender, EventArgs e)
        {
            btnGuardar.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnGuardar.Width, btnGuardar.Height, 10, 10));
            btnExportarExcel.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnExportarExcel.Width, btnExportarExcel.Height, 10, 10));
            btnExportarPdf.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnExportarPdf.Width, btnExportarPdf.Height, 10, 10));

            CargarEmpleados();
            MostrarPagosPersonal();
            EstiloTabla(dgvPagosPersonal);
            dgvPagosPersonal.CellClick += dgvPagosPersonal_CellClick;

        }

        private void CargarEmpleados()
        {
            listaEmpleados.Clear();
            cmbEmpleado.Items.Clear();

            using (SqlConnection cn = Conexion.Conectar())
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SELECT Id, Nombres, RutEmpleado, SueldoBase FROM Empleados", cn);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Empleado emp = new Empleado
                    {
                        Id = Convert.ToInt32(dr["Id"]),
                        Nombre = dr["Nombres"].ToString(),
                        RutEmpleado = dr["RutEmpleado"].ToString(),
                        SueldoBase = Convert.ToInt32(dr["SueldoBase"])
                    };

                    listaEmpleados.Add(emp);
                }
                dr.Close();
            }

            cmbEmpleado.DataSource = listaEmpleados;
            cmbEmpleado.DisplayMember = "Nombre";
            cmbEmpleado.ValueMember = "Id";
        }

        private void MostrarPagosPersonal()
        {
            using (SqlConnection cn = Conexion.Conectar())
            {
                cn.Open();
                string query = @"
            SELECT 
                pp.Id,
                pp.Fecha,
                e.Nombres AS Empleado,
                pp.RutEmpleado,
                pp.SueldoBase,
                pp.TurnosExtras,
                pp.MontoTurnosExtras,
                pp.Observacion,
                pp.TotalPago
            FROM PagosPersonal pp
            INNER JOIN Empleados e ON pp.EmpleadoId = e.Id";

                SqlDataAdapter da = new SqlDataAdapter(query, cn);
                pagosTabla = new DataTable();
                da.Fill(pagosTabla);
                dgvPagosPersonal.DataSource = pagosTabla;
            }
            if (dgvPagosPersonal.Columns.Contains("SueldoBase"))
                dgvPagosPersonal.Columns["SueldoBase"].DefaultCellStyle.Format = "C0";

            if (dgvPagosPersonal.Columns.Contains("MontoTurnosExtras"))
                dgvPagosPersonal.Columns["MontoTurnosExtras"].DefaultCellStyle.Format = "C0";

            if (dgvPagosPersonal.Columns.Contains("TotalPago"))
                dgvPagosPersonal.Columns["TotalPago"].DefaultCellStyle.Format = "C0";

        }
        private void LimpiarCampos()
        {
            cmbEmpleado.SelectedIndex = -1;
            txtRut.Text = "";
            txtSueldoBase.Text = "";
            txtTurnosExtras.Text = "";
            txtObservacion.Text = "";
            dtpFecha.Value = DateTime.Now;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (cmbEmpleado.SelectedItem == null)
            {
                MessageBox.Show("Selecciona un empleado.");
                return;
            }

            Empleado seleccionado = cmbEmpleado.SelectedItem as Empleado;

            if (seleccionado == null)
            {
                MessageBox.Show("Error al seleccionar el empleado.");
                return;
            }

            DateTime fecha = dtpFecha.Value;
            string rutEmpleado = seleccionado.RutEmpleado;
            int empleadoId = seleccionado.Id;

            // Limpiar el formato de moneda para poder convertir
            string sueldoBaseTexto = txtSueldoBase.Text.Replace("$", "").Replace(".", "").Trim();

            int sueldoBaseCompleto;
            if (!int.TryParse(sueldoBaseTexto, out sueldoBaseCompleto))
            {
                MessageBox.Show("El sueldo base ingresado no es válido.");
                return;
            }

            // Aquí recién dividimos entre 4 el sueldo completo para calcular el pago semanal
            int sueldoSemanal = sueldoBaseCompleto / 4;

            int turnosExtras = 0;
            int.TryParse(txtTurnosExtras.Text.Trim(), out turnosExtras);

            string observacion = txtObservacion.Text.Trim();

            int montoTurnosExtras = CalcularMontoExtras(turnosExtras);

            // Ahora sí el total es sueldo semanal + extras
            int totalPago = sueldoSemanal + montoTurnosExtras;

            using (SqlConnection cn = Conexion.Conectar())
            {
                cn.Open();

                SqlCommand cmd = new SqlCommand(@"
            INSERT INTO PagosPersonal 
            (Fecha, EmpleadoId, RutEmpleado, SueldoBase, TurnosExtras, MontoTurnosExtras, Observacion, TotalPago)
            VALUES 
            (@Fecha, @EmpleadoId, @RutEmpleado, @SueldoBase, @TurnosExtras, @MontoTurnosExtras, @Observacion, @TotalPago)", cn);

                cmd.Parameters.AddWithValue("@Fecha", fecha);
                cmd.Parameters.AddWithValue("@EmpleadoId", empleadoId);
                cmd.Parameters.AddWithValue("@RutEmpleado", rutEmpleado);
                cmd.Parameters.AddWithValue("@SueldoBase", sueldoSemanal); // Guardamos el semanal en la base
                cmd.Parameters.AddWithValue("@TurnosExtras", turnosExtras);
                cmd.Parameters.AddWithValue("@MontoTurnosExtras", montoTurnosExtras);
                cmd.Parameters.AddWithValue("@Observacion", observacion);
                cmd.Parameters.AddWithValue("@TotalPago", totalPago);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("✅ Pago registrado correctamente.");
            MostrarPagosPersonal();
            LimpiarCampos();
        }
        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvPagosPersonal.CurrentRow == null)
            {
                MessageBox.Show("Selecciona un pago para editar.");
                return;
            }

            int idPago = Convert.ToInt32(dgvPagosPersonal.CurrentRow.Cells["Id"].Value);

            if (cmbEmpleado.SelectedItem == null)
            {
                MessageBox.Show("Selecciona un empleado.");
                return;
            }

            Empleado seleccionado = cmbEmpleado.SelectedItem as Empleado;

            if (seleccionado == null)
            {
                MessageBox.Show("Error al seleccionar el empleado.");
                return;
            }

            DateTime fecha = dtpFecha.Value;
            string rutEmpleado = seleccionado.RutEmpleado;
            int empleadoId = seleccionado.Id;

            string sueldoBaseTexto = txtSueldoBase.Text.Replace("$", "").Replace(".", "").Trim();
            if (!int.TryParse(sueldoBaseTexto, out int sueldoBaseCompleto))
            {
                MessageBox.Show("El sueldo base ingresado no es válido.");
                return;
            }

            int sueldoSemanal = sueldoBaseCompleto / 4;

            int turnosExtras = 0;
            int.TryParse(txtTurnosExtras.Text.Trim(), out turnosExtras);

            string observacion = txtObservacion.Text.Trim();

            int montoTurnosExtras = CalcularMontoExtras(turnosExtras);
            int totalPago = sueldoSemanal + montoTurnosExtras;

            using (SqlConnection cn = Conexion.Conectar())
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand(@"
            UPDATE PagosPersonal 
            SET Fecha = @Fecha, EmpleadoId = @EmpleadoId, RutEmpleado = @RutEmpleado, SueldoBase = @SueldoBase, 
                TurnosExtras = @TurnosExtras, MontoTurnosExtras = @MontoTurnosExtras, Observacion = @Observacion, TotalPago = @TotalPago
            WHERE Id = @Id", cn);

                cmd.Parameters.AddWithValue("@Fecha", fecha);
                cmd.Parameters.AddWithValue("@EmpleadoId", empleadoId);
                cmd.Parameters.AddWithValue("@RutEmpleado", rutEmpleado);
                cmd.Parameters.AddWithValue("@SueldoBase", sueldoSemanal);
                cmd.Parameters.AddWithValue("@TurnosExtras", turnosExtras);
                cmd.Parameters.AddWithValue("@MontoTurnosExtras", montoTurnosExtras);
                cmd.Parameters.AddWithValue("@Observacion", observacion);
                cmd.Parameters.AddWithValue("@TotalPago", totalPago);
                cmd.Parameters.AddWithValue("@Id", idPago);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("✅ Pago actualizado correctamente.");
            MostrarPagosPersonal();
            LimpiarCampos();
            dgvPagosPersonal.ClearSelection();

        }
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvPagosPersonal.CurrentRow == null)
            {
                MessageBox.Show("Selecciona un pago para eliminar.");
                return;
            }

            int idPago = Convert.ToInt32(dgvPagosPersonal.CurrentRow.Cells["Id"].Value);

            DialogResult result = MessageBox.Show("¿Estás seguro que quieres eliminar este pago?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                using (SqlConnection cn = Conexion.Conectar())
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM PagosPersonal WHERE Id = @Id", cn);
                    cmd.Parameters.AddWithValue("@Id", idPago);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("✅ Pago eliminado correctamente.");
                MostrarPagosPersonal();
                LimpiarCampos();
                dgvPagosPersonal.ClearSelection();

            }
        }
        private int CalcularMontoExtras(int turnos)
        {
            int monto = 0;
            for (int i = 0; i < turnos; i++)
            {
                DialogResult result = MessageBox.Show($"¿El turno {(i + 1)} fue con 2 máquinas?", "Turno extra", MessageBoxButtons.YesNo);
                monto += (result == DialogResult.Yes) ? 45000 : 35000;
            }
            return monto;
        }

        private void txtBuscarEmpleado_TextChanged(object sender, EventArgs e)
        {
            if (pagosTabla == null) return;

            string filtro = txtBuscarEmpleado.Text.Trim();
            DataView dv = pagosTabla.DefaultView;

            if (!string.IsNullOrEmpty(filtro))
            {
                dv.RowFilter = $"EmpleadoId LIKE '%{filtro}%' OR RutEmpleado LIKE '%{filtro}%'";
            }
            else
            {
                dv.RowFilter = string.Empty;
            }
        }


        private void btnExportarExcel_Click(object sender, EventArgs e)
        {
            if (dgvPagosPersonal.Rows.Count == 0)
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
                        var dt = (dgvPagosPersonal.DataSource as DataTable).Copy();
                        var ws = wb.Worksheets.Add(dt, "Pagos Personal");

                        // Título bonito
                        ws.Cell("A1").Value = "PROMOBORDADOS - Pagos Personal";
                        ws.Cell("A1").Style.Font.Bold = true;
                        ws.Cell("A1").Style.Font.FontSize = 16;
                        ws.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Range("A1:H1").Merge();

                        // Fecha
                        ws.Cell("A2").Value = $"Fecha de generación: {DateTime.Now:dd/MM/yyyy}";
                        ws.Cell("A2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Range("A2:H2").Merge();

                        // Estilos generales
                        ws.SheetView.FreezeRows(3); // Congela primeras 3 filas
                        ws.Rows().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Rows().Style.Font.FontName = "Segoe UI";
                        ws.Rows().Style.Font.FontSize = 11;
                        ws.Row(3).Style.Font.Bold = true; // Encabezados
                        ws.Rows().AdjustToContents();
                        ws.Columns().AdjustToContents();

                        wb.SaveAs(sfd.FileName);
                    }

                    MessageBox.Show("✅ Excel exportado correctamente.");
                }
            }
        }

        private void btnExportarPdf_Click(object sender, EventArgs e)
        {
            if (dgvPagosPersonal.Rows.Count == 0)
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
                        Document pdfDoc = new Document(PageSize.A4, 20f, 20f, 20f, 20f);
                        PdfWriter.GetInstance(pdfDoc, stream);
                        pdfDoc.Open();

                        // Fuentes correctas
                        BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                        iTextSharp.text.Font titleFont = new iTextSharp.text.Font(bf, 16, iTextSharp.text.Font.BOLDITALIC);
                        iTextSharp.text.Font headerFont = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.BOLD);
                        iTextSharp.text.Font cellFont = new iTextSharp.text.Font(bf, 9);

                        // Título
                        Paragraph title = new Paragraph("PROMOBORDADOS - Pagos Personal", titleFont);
                        title.Alignment = Element.ALIGN_CENTER;
                        pdfDoc.Add(title);
                        pdfDoc.Add(new Phrase("\n"));

                        // Tabla
                        PdfPTable pdfTable = new PdfPTable(dgvPagosPersonal.Columns.Count);
                        pdfTable.WidthPercentage = 100;

                        // Cabecera
                        foreach (DataGridViewColumn column in dgvPagosPersonal.Columns)
                        {
                            PdfPCell cell = new PdfPCell(new Phrase(column.HeaderText, headerFont));
                            cell.BackgroundColor = BaseColor.YELLOW;
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            pdfTable.AddCell(cell);
                        }

                        // Contenido
                        foreach (DataGridViewRow row in dgvPagosPersonal.Rows)
                        {
                            if (!row.IsNewRow)
                            {
                                foreach (DataGridViewCell cell in row.Cells)
                                {
                                    string texto = cell.Value?.ToString() ?? "";
                                    PdfPCell cellBody = new PdfPCell(new Phrase(texto, cellFont));
                                    pdfTable.AddCell(cellBody);
                                }
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
        private void cmbEmpleado_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbEmpleado.SelectedIndex != -1)
            {
                Empleado seleccionado = cmbEmpleado.SelectedItem as Empleado;

                if (seleccionado != null)
                {
                    txtRut.Text = seleccionado.RutEmpleado;
                    txtSueldoBase.Text = seleccionado.SueldoBase.ToString("C0"); // Formato moneda
                }
            }
        }
        private void dgvPagosPersonal_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvPagosPersonal.Rows[e.RowIndex];

                // ⚡ Buscar y seleccionar automáticamente el empleado en el ComboBox
                string nombreEmpleado = row.Cells["Empleado"].Value?.ToString();

                Empleado empleadoSeleccionado = listaEmpleados.FirstOrDefault(emp => emp.Nombre == nombreEmpleado);

                if (empleadoSeleccionado != null)
                {
                    cmbEmpleado.SelectedItem = empleadoSeleccionado;
                }

                // ⚡ Asignar los demás campos del formulario
                txtRut.Text = row.Cells["RutEmpleado"].Value?.ToString();
                txtSueldoBase.Text = string.Format("{0:C0}", row.Cells["SueldoBase"].Value); // Mostrar en formato moneda
                txtTurnosExtras.Text = row.Cells["TurnosExtras"].Value?.ToString();
                txtObservacion.Text = row.Cells["Observacion"].Value?.ToString();

                if (row.Cells["Fecha"].Value != null && DateTime.TryParse(row.Cells["Fecha"].Value.ToString(), out DateTime fecha))
                {
                    dtpFecha.Value = fecha;
                }
            }
        }
    }
}
