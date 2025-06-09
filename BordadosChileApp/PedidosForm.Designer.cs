namespace BordadosChileApp
{
    partial class PedidosForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PedidosForm));
            this.cmbCliente = new System.Windows.Forms.ComboBox();
            this.dtpFecha = new System.Windows.Forms.DateTimePicker();
            this.cmbEstado = new System.Windows.Forms.ComboBox();
            this.txtBordado = new System.Windows.Forms.TextBox();
            this.btnGuardarPedido = new System.Windows.Forms.Button();
            this.btnEliminarPedido = new System.Windows.Forms.Button();
            this.dgvPedidos = new System.Windows.Forms.DataGridView();
            this.picPromoBordados = new System.Windows.Forms.PictureBox();
            this.txtBuscarPedido = new System.Windows.Forms.TextBox();
            this.cmbClienteFiltro = new System.Windows.Forms.ComboBox();
            this.cmbEstadoFiltro = new System.Windows.Forms.ComboBox();
            this.lblCliente = new System.Windows.Forms.Label();
            this.lblFecha = new System.Windows.Forms.Label();
            this.lblEstado = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblBuscar = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtCajas = new System.Windows.Forms.TextBox();
            this.picLogo = new System.Windows.Forms.PictureBox();
            this.btnEditarPedido = new System.Windows.Forms.Button();
            this.btnCargarLogo = new System.Windows.Forms.Button();
            this.dgvDetallePedido = new System.Windows.Forms.DataGridView();
            this.label6 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtTipoPrenda = new System.Windows.Forms.TextBox();
            this.txtBordadoPrenda = new System.Windows.Forms.TextBox();
            this.txtCantidad = new System.Windows.Forms.TextBox();
            this.txtColor = new System.Windows.Forms.TextBox();
            this.btnAgregarPrenda = new System.Windows.Forms.Button();
            this.btnGenerarFactura = new System.Windows.Forms.Button();
            this.btnGenerarGuia = new System.Windows.Forms.Button();
            this.btnExportarPdf = new System.Windows.Forms.Button();
            this.btnExportarExcel = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.txtCosto = new System.Windows.Forms.TextBox();
            this.btnActualizarPrenda = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPedidos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPromoBordados)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetallePedido)).BeginInit();
            this.SuspendLayout();
            // 
            // cmbCliente
            // 
            this.cmbCliente.Location = new System.Drawing.Point(496, 51);
            this.cmbCliente.Name = "cmbCliente";
            this.cmbCliente.Size = new System.Drawing.Size(150, 25);
            this.cmbCliente.TabIndex = 6;
            // 
            // dtpFecha
            // 
            this.dtpFecha.Location = new System.Drawing.Point(496, 82);
            this.dtpFecha.Name = "dtpFecha";
            this.dtpFecha.Size = new System.Drawing.Size(230, 25);
            this.dtpFecha.TabIndex = 7;
            // 
            // cmbEstado
            // 
            this.cmbEstado.Location = new System.Drawing.Point(496, 175);
            this.cmbEstado.Name = "cmbEstado";
            this.cmbEstado.Size = new System.Drawing.Size(150, 25);
            this.cmbEstado.TabIndex = 8;
            // 
            // txtBordado
            // 
            this.txtBordado.Location = new System.Drawing.Point(496, 144);
            this.txtBordado.Name = "txtBordado";
            this.txtBordado.Size = new System.Drawing.Size(120, 25);
            this.txtBordado.TabIndex = 10;
            // 
            // btnGuardarPedido
            // 
            this.btnGuardarPedido.BackColor = System.Drawing.Color.DarkGray;
            this.btnGuardarPedido.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGuardarPedido.FlatAppearance.BorderSize = 0;
            this.btnGuardarPedido.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGuardarPedido.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGuardarPedido.Location = new System.Drawing.Point(375, 524);
            this.btnGuardarPedido.Name = "btnGuardarPedido";
            this.btnGuardarPedido.Size = new System.Drawing.Size(120, 30);
            this.btnGuardarPedido.TabIndex = 11;
            this.btnGuardarPedido.Text = "Guardar Pedido";
            this.btnGuardarPedido.UseVisualStyleBackColor = false;
            this.btnGuardarPedido.Click += new System.EventHandler(this.btnGuardarPedido_Click);
            // 
            // btnEliminarPedido
            // 
            this.btnEliminarPedido.BackColor = System.Drawing.Color.DarkGray;
            this.btnEliminarPedido.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEliminarPedido.FlatAppearance.BorderSize = 0;
            this.btnEliminarPedido.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEliminarPedido.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEliminarPedido.Location = new System.Drawing.Point(627, 524);
            this.btnEliminarPedido.Name = "btnEliminarPedido";
            this.btnEliminarPedido.Size = new System.Drawing.Size(130, 30);
            this.btnEliminarPedido.TabIndex = 13;
            this.btnEliminarPedido.Text = "Eliminar Pedido";
            this.btnEliminarPedido.UseVisualStyleBackColor = false;
            this.btnEliminarPedido.Click += new System.EventHandler(this.btnEliminarPedido_Click);
            // 
            // dgvPedidos
            // 
            this.dgvPedidos.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dgvPedidos.Location = new System.Drawing.Point(0, 609);
            this.dgvPedidos.Name = "dgvPedidos";
            this.dgvPedidos.Size = new System.Drawing.Size(1140, 200);
            this.dgvPedidos.TabIndex = 18;
            this.dgvPedidos.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPedidos_CellClick);
            // 
            // picPromoBordados
            // 
            this.picPromoBordados.Image = ((System.Drawing.Image)(resources.GetObject("picPromoBordados.Image")));
            this.picPromoBordados.Location = new System.Drawing.Point(0, -31);
            this.picPromoBordados.Name = "picPromoBordados";
            this.picPromoBordados.Size = new System.Drawing.Size(322, 283);
            this.picPromoBordados.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picPromoBordados.TabIndex = 16;
            this.picPromoBordados.TabStop = false;
            // 
            // txtBuscarPedido
            // 
            this.txtBuscarPedido.Location = new System.Drawing.Point(157, 570);
            this.txtBuscarPedido.Name = "txtBuscarPedido";
            this.txtBuscarPedido.Size = new System.Drawing.Size(120, 25);
            this.txtBuscarPedido.TabIndex = 17;
            this.txtBuscarPedido.TextChanged += new System.EventHandler(this.txtBuscarPedido_TextChanged);
            // 
            // cmbClienteFiltro
            // 
            this.cmbClienteFiltro.Location = new System.Drawing.Point(0, 0);
            this.cmbClienteFiltro.Name = "cmbClienteFiltro";
            this.cmbClienteFiltro.Size = new System.Drawing.Size(121, 21);
            this.cmbClienteFiltro.TabIndex = 0;
            // 
            // cmbEstadoFiltro
            // 
            this.cmbEstadoFiltro.Location = new System.Drawing.Point(0, 0);
            this.cmbEstadoFiltro.Name = "cmbEstadoFiltro";
            this.cmbEstadoFiltro.Size = new System.Drawing.Size(121, 21);
            this.cmbEstadoFiltro.TabIndex = 0;
            // 
            // lblCliente
            // 
            this.lblCliente.Location = new System.Drawing.Point(416, 54);
            this.lblCliente.Name = "lblCliente";
            this.lblCliente.Size = new System.Drawing.Size(60, 20);
            this.lblCliente.TabIndex = 0;
            this.lblCliente.Text = "Cliente";
            // 
            // lblFecha
            // 
            this.lblFecha.Location = new System.Drawing.Point(416, 88);
            this.lblFecha.Name = "lblFecha";
            this.lblFecha.Size = new System.Drawing.Size(60, 20);
            this.lblFecha.TabIndex = 1;
            this.lblFecha.Text = "Fecha";
            // 
            // lblEstado
            // 
            this.lblEstado.Location = new System.Drawing.Point(416, 178);
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.Size = new System.Drawing.Size(60, 20);
            this.lblEstado.TabIndex = 2;
            this.lblEstado.Text = "Estado";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(416, 149);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 20);
            this.label3.TabIndex = 4;
            this.label3.Text = "Bordado";
            // 
            // lblBuscar
            // 
            this.lblBuscar.Location = new System.Drawing.Point(11, 573);
            this.lblBuscar.Name = "lblBuscar";
            this.lblBuscar.Size = new System.Drawing.Size(140, 20);
            this.lblBuscar.TabIndex = 5;
            this.lblBuscar.Text = "Buscar N° de pedido";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(416, 116);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 20);
            this.label2.TabIndex = 21;
            this.label2.Text = "N° de cajas";
            // 
            // txtCajas
            // 
            this.txtCajas.Location = new System.Drawing.Point(496, 113);
            this.txtCajas.Name = "txtCajas";
            this.txtCajas.Size = new System.Drawing.Size(120, 25);
            this.txtCajas.TabIndex = 22;
            // 
            // picLogo
            // 
            this.picLogo.Location = new System.Drawing.Point(754, 54);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(179, 108);
            this.picLogo.TabIndex = 23;
            this.picLogo.TabStop = false;
            // 
            // btnEditarPedido
            // 
            this.btnEditarPedido.BackColor = System.Drawing.Color.DarkGray;
            this.btnEditarPedido.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEditarPedido.FlatAppearance.BorderSize = 0;
            this.btnEditarPedido.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEditarPedido.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEditarPedido.Location = new System.Drawing.Point(501, 524);
            this.btnEditarPedido.Name = "btnEditarPedido";
            this.btnEditarPedido.Size = new System.Drawing.Size(120, 30);
            this.btnEditarPedido.TabIndex = 24;
            this.btnEditarPedido.Text = "Editar Pedido";
            this.btnEditarPedido.UseVisualStyleBackColor = false;
            this.btnEditarPedido.Click += new System.EventHandler(this.btnEditarPedido_Click);
            // 
            // btnCargarLogo
            // 
            this.btnCargarLogo.BackColor = System.Drawing.Color.DarkGray;
            this.btnCargarLogo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCargarLogo.FlatAppearance.BorderSize = 0;
            this.btnCargarLogo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCargarLogo.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCargarLogo.Location = new System.Drawing.Point(788, 178);
            this.btnCargarLogo.Name = "btnCargarLogo";
            this.btnCargarLogo.Size = new System.Drawing.Size(120, 30);
            this.btnCargarLogo.TabIndex = 26;
            this.btnCargarLogo.Text = "Cagar Logo";
            this.btnCargarLogo.UseVisualStyleBackColor = false;
            this.btnCargarLogo.Click += new System.EventHandler(this.btnCargarLogo_Click);
            // 
            // dgvDetallePedido
            // 
            this.dgvDetallePedido.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDetallePedido.Location = new System.Drawing.Point(12, 356);
            this.dgvDetallePedido.Name = "dgvDetallePedido";
            this.dgvDetallePedido.Size = new System.Drawing.Size(1140, 148);
            this.dgvDetallePedido.TabIndex = 27;
            this.dgvDetallePedido.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDetallePedido_CellClick);
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(71, 289);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(148, 20);
            this.label6.TabIndex = 31;
            this.label6.Text = "Tipo de Prenda";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(637, 289);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(148, 20);
            this.label1.TabIndex = 32;
            this.label1.Text = "Bordado";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(451, 289);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(148, 20);
            this.label4.TabIndex = 33;
            this.label4.Text = "Cantidad";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(263, 289);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(148, 20);
            this.label5.TabIndex = 34;
            this.label5.Text = "Color";
            // 
            // txtTipoPrenda
            // 
            this.txtTipoPrenda.Location = new System.Drawing.Point(65, 312);
            this.txtTipoPrenda.Name = "txtTipoPrenda";
            this.txtTipoPrenda.Size = new System.Drawing.Size(145, 25);
            this.txtTipoPrenda.TabIndex = 35;
            // 
            // txtBordadoPrenda
            // 
            this.txtBordadoPrenda.Location = new System.Drawing.Point(630, 312);
            this.txtBordadoPrenda.Name = "txtBordadoPrenda";
            this.txtBordadoPrenda.Size = new System.Drawing.Size(145, 25);
            this.txtBordadoPrenda.TabIndex = 36;
            // 
            // txtCantidad
            // 
            this.txtCantidad.Location = new System.Drawing.Point(445, 312);
            this.txtCantidad.Name = "txtCantidad";
            this.txtCantidad.Size = new System.Drawing.Size(145, 25);
            this.txtCantidad.TabIndex = 37;
            // 
            // txtColor
            // 
            this.txtColor.Location = new System.Drawing.Point(257, 312);
            this.txtColor.Name = "txtColor";
            this.txtColor.Size = new System.Drawing.Size(145, 25);
            this.txtColor.TabIndex = 38;
            // 
            // btnAgregarPrenda
            // 
            this.btnAgregarPrenda.BackColor = System.Drawing.Color.DarkGray;
            this.btnAgregarPrenda.FlatAppearance.BorderSize = 0;
            this.btnAgregarPrenda.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAgregarPrenda.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnAgregarPrenda.Location = new System.Drawing.Point(805, 289);
            this.btnAgregarPrenda.Name = "btnAgregarPrenda";
            this.btnAgregarPrenda.Size = new System.Drawing.Size(137, 39);
            this.btnAgregarPrenda.TabIndex = 39;
            this.btnAgregarPrenda.Text = "Agregar Prenda";
            this.btnAgregarPrenda.UseVisualStyleBackColor = false;
            this.btnAgregarPrenda.Click += new System.EventHandler(this.btnAgregarPrenda_Click);
            // 
            // btnGenerarFactura
            // 
            this.btnGenerarFactura.BackColor = System.Drawing.Color.DarkGray;
            this.btnGenerarFactura.FlatAppearance.BorderSize = 0;
            this.btnGenerarFactura.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGenerarFactura.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGenerarFactura.Location = new System.Drawing.Point(956, 562);
            this.btnGenerarFactura.Name = "btnGenerarFactura";
            this.btnGenerarFactura.Size = new System.Drawing.Size(71, 39);
            this.btnGenerarFactura.TabIndex = 40;
            this.btnGenerarFactura.Text = "Factura";
            this.btnGenerarFactura.UseVisualStyleBackColor = false;
            this.btnGenerarFactura.Click += new System.EventHandler(this.btnGenerarFactura_Click);
            // 
            // btnGenerarGuia
            // 
            this.btnGenerarGuia.BackColor = System.Drawing.Color.DarkGray;
            this.btnGenerarGuia.FlatAppearance.BorderSize = 0;
            this.btnGenerarGuia.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGenerarGuia.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnGenerarGuia.Location = new System.Drawing.Point(1038, 562);
            this.btnGenerarGuia.Name = "btnGenerarGuia";
            this.btnGenerarGuia.Size = new System.Drawing.Size(80, 39);
            this.btnGenerarGuia.TabIndex = 100;
            this.btnGenerarGuia.Text = "Guía";
            this.btnGenerarGuia.UseVisualStyleBackColor = false;
            this.btnGenerarGuia.Click += new System.EventHandler(this.btnGenerarGuia_Click);
            // 
            // btnExportarPdf
            // 
            this.btnExportarPdf.BackColor = System.Drawing.Color.DarkGray;
            this.btnExportarPdf.FlatAppearance.BorderSize = 0;
            this.btnExportarPdf.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportarPdf.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnExportarPdf.Location = new System.Drawing.Point(981, 82);
            this.btnExportarPdf.Name = "btnExportarPdf";
            this.btnExportarPdf.Size = new System.Drawing.Size(137, 39);
            this.btnExportarPdf.TabIndex = 42;
            this.btnExportarPdf.Text = "Exportar PDF";
            this.btnExportarPdf.UseVisualStyleBackColor = false;
            this.btnExportarPdf.Click += new System.EventHandler(this.btnExportarPdf_Click);
            // 
            // btnExportarExcel
            // 
            this.btnExportarExcel.BackColor = System.Drawing.Color.DarkGray;
            this.btnExportarExcel.FlatAppearance.BorderSize = 0;
            this.btnExportarExcel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportarExcel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnExportarExcel.Location = new System.Drawing.Point(981, 35);
            this.btnExportarExcel.Name = "btnExportarExcel";
            this.btnExportarExcel.Size = new System.Drawing.Size(137, 39);
            this.btnExportarExcel.TabIndex = 43;
            this.btnExportarExcel.Text = "Exportar Excel";
            this.btnExportarExcel.UseVisualStyleBackColor = false;
            this.btnExportarExcel.Click += new System.EventHandler(this.btnExportarExcel_Click);
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(416, 212);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(60, 20);
            this.label7.TabIndex = 44;
            this.label7.Text = "Costo";
            // 
            // txtCosto
            // 
            this.txtCosto.Location = new System.Drawing.Point(496, 209);
            this.txtCosto.Name = "txtCosto";
            this.txtCosto.Size = new System.Drawing.Size(120, 25);
            this.txtCosto.TabIndex = 45;
            // 
            // btnActualizarPrenda
            // 
            this.btnActualizarPrenda.BackColor = System.Drawing.Color.DarkGray;
            this.btnActualizarPrenda.FlatAppearance.BorderSize = 0;
            this.btnActualizarPrenda.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnActualizarPrenda.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnActualizarPrenda.Location = new System.Drawing.Point(956, 289);
            this.btnActualizarPrenda.Name = "btnActualizarPrenda";
            this.btnActualizarPrenda.Size = new System.Drawing.Size(137, 39);
            this.btnActualizarPrenda.TabIndex = 46;
            this.btnActualizarPrenda.Text = "Actualizar Prenda";
            this.btnActualizarPrenda.UseVisualStyleBackColor = false;
            this.btnActualizarPrenda.Click += new System.EventHandler(this.btnActualizarPrenda_Click);
            // 
            // PedidosForm
            // 
            this.ClientSize = new System.Drawing.Size(1140, 809);
            this.Controls.Add(this.btnActualizarPrenda);
            this.Controls.Add(this.txtCosto);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.btnExportarExcel);
            this.Controls.Add(this.btnExportarPdf);
            this.Controls.Add(this.btnGenerarGuia);
            this.Controls.Add(this.btnGenerarFactura);
            this.Controls.Add(this.btnAgregarPrenda);
            this.Controls.Add(this.txtColor);
            this.Controls.Add(this.txtCantidad);
            this.Controls.Add(this.txtBordadoPrenda);
            this.Controls.Add(this.txtTipoPrenda);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.dgvDetallePedido);
            this.Controls.Add(this.btnCargarLogo);
            this.Controls.Add(this.btnEditarPedido);
            this.Controls.Add(this.picLogo);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtCajas);
            this.Controls.Add(this.lblCliente);
            this.Controls.Add(this.lblFecha);
            this.Controls.Add(this.lblEstado);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblBuscar);
            this.Controls.Add(this.cmbCliente);
            this.Controls.Add(this.dtpFecha);
            this.Controls.Add(this.cmbEstado);
            this.Controls.Add(this.txtBordado);
            this.Controls.Add(this.btnGuardarPedido);
            this.Controls.Add(this.btnEliminarPedido);
            this.Controls.Add(this.picPromoBordados);
            this.Controls.Add(this.txtBuscarPedido);
            this.Controls.Add(this.dgvPedidos);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.Name = "PedidosForm";
            this.Text = "Pedidos";
            this.Load += new System.EventHandler(this.PedidosForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPedidos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPromoBordados)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetallePedido)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbCliente;
        private System.Windows.Forms.DateTimePicker dtpFecha;
        private System.Windows.Forms.ComboBox cmbEstado;
        private System.Windows.Forms.TextBox txtBordado;
        private System.Windows.Forms.Button btnGuardarPedido;
        private System.Windows.Forms.Button btnEliminarPedido;
        private System.Windows.Forms.DataGridView dgvPedidos;
        private System.Windows.Forms.PictureBox picPromoBordados;
        private System.Windows.Forms.TextBox txtBuscarPedido;
        private System.Windows.Forms.ComboBox cmbClienteFiltro;
        private System.Windows.Forms.ComboBox cmbEstadoFiltro;
        private System.Windows.Forms.Label lblCliente;
        private System.Windows.Forms.Label lblFecha;
        private System.Windows.Forms.Label lblEstado;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblBuscar;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtCajas;
        private System.Windows.Forms.PictureBox picLogo;
        private System.Windows.Forms.Button btnEditarPedido;
        private System.Windows.Forms.Button btnCargarLogo;
        private System.Windows.Forms.DataGridView dgvDetallePedido;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtTipoPrenda;
        private System.Windows.Forms.TextBox txtBordadoPrenda;
        private System.Windows.Forms.TextBox txtCantidad;
        private System.Windows.Forms.TextBox txtColor;
        private System.Windows.Forms.Button btnAgregarPrenda;
        private System.Windows.Forms.Button btnGenerarFactura;
        private System.Windows.Forms.Button btnGenerarGuia;
        private System.Windows.Forms.Button btnExportarPdf;
        private System.Windows.Forms.Button btnExportarExcel;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtCosto;
        private System.Windows.Forms.Button btnActualizarPrenda;
    }
}
 