namespace BordadosChileApp
{
    partial class ReportesForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReportesForm));
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend4 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea5 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend5 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series5 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea6 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend6 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series6 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.lblTotalVentasSemana = new System.Windows.Forms.Label();
            this.dgvVentasGenerales = new System.Windows.Forms.DataGridView();
            this.dtpDesde = new System.Windows.Forms.DateTimePicker();
            this.dtpHasta = new System.Windows.Forms.DateTimePicker();
            this.btnFiltrarVentas = new System.Windows.Forms.Button();
            this.btnExportarExcel = new System.Windows.Forms.Button();
            this.picLogo = new System.Windows.Forms.PictureBox();
            this.btnExportarPdf = new System.Windows.Forms.Button();
            this.lblTotalVentasMes = new System.Windows.Forms.Label();
            this.txtBuscarCliente = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chartVentasAño = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartVentasSemana = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartVentasMes = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVentasGenerales)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartVentasAño)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartVentasSemana)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartVentasMes)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTotalVentasSemana
            // 
            this.lblTotalVentasSemana.AutoSize = true;
            this.lblTotalVentasSemana.Location = new System.Drawing.Point(58, 105);
            this.lblTotalVentasSemana.Name = "lblTotalVentasSemana";
            this.lblTotalVentasSemana.Size = new System.Drawing.Size(0, 17);
            this.lblTotalVentasSemana.TabIndex = 8;
            // 
            // dgvVentasGenerales
            // 
            this.dgvVentasGenerales.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvVentasGenerales.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dgvVentasGenerales.Location = new System.Drawing.Point(0, 656);
            this.dgvVentasGenerales.Name = "dgvVentasGenerales";
            this.dgvVentasGenerales.Size = new System.Drawing.Size(1609, 206);
            this.dgvVentasGenerales.TabIndex = 6;
            // 
            // dtpDesde
            // 
            this.dtpDesde.Location = new System.Drawing.Point(548, 248);
            this.dtpDesde.Name = "dtpDesde";
            this.dtpDesde.Size = new System.Drawing.Size(241, 25);
            this.dtpDesde.TabIndex = 5;
            // 
            // dtpHasta
            // 
            this.dtpHasta.Location = new System.Drawing.Point(795, 248);
            this.dtpHasta.Name = "dtpHasta";
            this.dtpHasta.Size = new System.Drawing.Size(241, 25);
            this.dtpHasta.TabIndex = 4;
            // 
            // btnFiltrarVentas
            // 
            this.btnFiltrarVentas.BackColor = System.Drawing.Color.DarkGray;
            this.btnFiltrarVentas.FlatAppearance.BorderSize = 0;
            this.btnFiltrarVentas.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFiltrarVentas.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnFiltrarVentas.Location = new System.Drawing.Point(721, 299);
            this.btnFiltrarVentas.Name = "btnFiltrarVentas";
            this.btnFiltrarVentas.Size = new System.Drawing.Size(156, 30);
            this.btnFiltrarVentas.TabIndex = 3;
            this.btnFiltrarVentas.Text = "Filtrar por fechas";
            this.btnFiltrarVentas.UseVisualStyleBackColor = false;
            this.btnFiltrarVentas.Click += new System.EventHandler(this.btnFiltrar_Click);
            // 
            // btnExportarExcel
            // 
            this.btnExportarExcel.BackColor = System.Drawing.Color.DarkGray;
            this.btnExportarExcel.FlatAppearance.BorderSize = 0;
            this.btnExportarExcel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportarExcel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnExportarExcel.Location = new System.Drawing.Point(1464, 9);
            this.btnExportarExcel.Name = "btnExportarExcel";
            this.btnExportarExcel.Size = new System.Drawing.Size(133, 30);
            this.btnExportarExcel.TabIndex = 2;
            this.btnExportarExcel.Text = "Exportar Excel";
            this.btnExportarExcel.UseVisualStyleBackColor = false;
            this.btnExportarExcel.Click += new System.EventHandler(this.btnExportarExcel_Click);
            // 
            // picLogo
            // 
            this.picLogo.Image = ((System.Drawing.Image)(resources.GetObject("picLogo.Image")));
            this.picLogo.Location = new System.Drawing.Point(306, -106);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(980, 419);
            this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLogo.TabIndex = 9;
            this.picLogo.TabStop = false;
            // 
            // btnExportarPdf
            // 
            this.btnExportarPdf.BackColor = System.Drawing.Color.DarkGray;
            this.btnExportarPdf.FlatAppearance.BorderSize = 0;
            this.btnExportarPdf.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportarPdf.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnExportarPdf.Location = new System.Drawing.Point(1464, 45);
            this.btnExportarPdf.Name = "btnExportarPdf";
            this.btnExportarPdf.Size = new System.Drawing.Size(133, 30);
            this.btnExportarPdf.TabIndex = 0;
            this.btnExportarPdf.Text = "Exportar PDF";
            this.btnExportarPdf.UseVisualStyleBackColor = false;
            this.btnExportarPdf.Click += new System.EventHandler(this.btnExportarPdf_Click);
            // 
            // lblTotalVentasMes
            // 
            this.lblTotalVentasMes.AutoSize = true;
            this.lblTotalVentasMes.Location = new System.Drawing.Point(58, 45);
            this.lblTotalVentasMes.Name = "lblTotalVentasMes";
            this.lblTotalVentasMes.Size = new System.Drawing.Size(0, 17);
            this.lblTotalVentasMes.TabIndex = 7;
            // 
            // txtBuscarCliente
            // 
            this.txtBuscarCliente.Location = new System.Drawing.Point(795, 339);
            this.txtBuscarCliente.Name = "txtBuscarCliente";
            this.txtBuscarCliente.Size = new System.Drawing.Size(100, 25);
            this.txtBuscarCliente.TabIndex = 10;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(675, 342);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(117, 17);
            this.label1.TabIndex = 11;
            this.label1.Text = "Buscar por Cliente:";
            // 
            // chartVentasAño
            // 
            chartArea4.Name = "ChartArea1";
            this.chartVentasAño.ChartAreas.Add(chartArea4);
            legend4.Name = "Legend1";
            this.chartVentasAño.Legends.Add(legend4);
            this.chartVentasAño.Location = new System.Drawing.Point(81, 367);
            this.chartVentasAño.Name = "chartVentasAño";
            series4.ChartArea = "ChartArea1";
            series4.Legend = "Legend1";
            series4.Name = "Series1";
            this.chartVentasAño.Series.Add(series4);
            this.chartVentasAño.Size = new System.Drawing.Size(405, 261);
            this.chartVentasAño.TabIndex = 12;
            this.chartVentasAño.Text = "chart1";
            // 
            // chartVentasSemana
            // 
            chartArea5.Name = "ChartArea1";
            this.chartVentasSemana.ChartAreas.Add(chartArea5);
            legend5.Name = "Legend1";
            this.chartVentasSemana.Legends.Add(legend5);
            this.chartVentasSemana.Location = new System.Drawing.Point(1122, 367);
            this.chartVentasSemana.Name = "chartVentasSemana";
            series5.ChartArea = "ChartArea1";
            series5.Legend = "Legend1";
            series5.Name = "Series1";
            this.chartVentasSemana.Series.Add(series5);
            this.chartVentasSemana.Size = new System.Drawing.Size(405, 261);
            this.chartVentasSemana.TabIndex = 13;
            this.chartVentasSemana.Text = "chart1";
            // 
            // chartVentasMes
            // 
            chartArea6.Name = "ChartArea1";
            this.chartVentasMes.ChartAreas.Add(chartArea6);
            legend6.Name = "Legend1";
            this.chartVentasMes.Legends.Add(legend6);
            this.chartVentasMes.Location = new System.Drawing.Point(600, 367);
            this.chartVentasMes.Name = "chartVentasMes";
            series6.ChartArea = "ChartArea1";
            series6.Legend = "Legend1";
            series6.Name = "Series1";
            this.chartVentasMes.Series.Add(series6);
            this.chartVentasMes.Size = new System.Drawing.Size(405, 261);
            this.chartVentasMes.TabIndex = 14;
            this.chartVentasMes.Text = "chart1";
            // 
            // ReportesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(1609, 862);
            this.Controls.Add(this.chartVentasMes);
            this.Controls.Add(this.chartVentasSemana);
            this.Controls.Add(this.chartVentasAño);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtBuscarCliente);
            this.Controls.Add(this.btnExportarPdf);
            this.Controls.Add(this.btnExportarExcel);
            this.Controls.Add(this.btnFiltrarVentas);
            this.Controls.Add(this.dtpHasta);
            this.Controls.Add(this.dtpDesde);
            this.Controls.Add(this.dgvVentasGenerales);
            this.Controls.Add(this.lblTotalVentasMes);
            this.Controls.Add(this.lblTotalVentasSemana);
            this.Controls.Add(this.picLogo);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.Name = "ReportesForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ReportesForm";
            this.Load += new System.EventHandler(this.ReportesForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvVentasGenerales)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartVentasAño)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartVentasSemana)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartVentasMes)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTotalVentasSemana;
        private System.Windows.Forms.DataGridView dgvVentasGenerales;
        private System.Windows.Forms.DateTimePicker dtpDesde;
        private System.Windows.Forms.DateTimePicker dtpHasta;
        private System.Windows.Forms.Button btnFiltrarVentas;
        private System.Windows.Forms.Button btnExportarExcel;
        private System.Windows.Forms.PictureBox picLogo;
        private System.Windows.Forms.Button btnExportarPdf;
        private System.Windows.Forms.Label lblTotalVentasMes;
        private System.Windows.Forms.TextBox txtBuscarCliente;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartVentasAño;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartVentasSemana;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartVentasMes;
    }
}
