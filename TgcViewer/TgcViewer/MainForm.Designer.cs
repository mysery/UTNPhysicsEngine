﻿namespace TgcViewer
{
    partial class MainForm
    {
        /// <summary>
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben eliminar; false en caso contrario, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.verToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wireframeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contadorFPSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ejesCartesianosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mostrarPosiciónDeCámaraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusCurrentExample = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusPosition = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainerConsole = new System.Windows.Forms.SplitContainer();
            this.splitContainerExamples = new System.Windows.Forms.SplitContainer();
            this.splitContainerDescripcionExample = new System.Windows.Forms.SplitContainer();
            this.groupBoxExamples = new System.Windows.Forms.GroupBox();
            this.treeViewExamples = new System.Windows.Forms.TreeView();
            this.textBoxExampleDescription = new System.Windows.Forms.TextBox();
            this.splitContainerUserVars = new System.Windows.Forms.SplitContainer();
            this.panel3d = new System.Windows.Forms.Panel();
            this.splitContainerModifiers = new System.Windows.Forms.SplitContainer();
            this.groupBoxModifiers = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanelModifiers = new System.Windows.Forms.FlowLayoutPanel();
            this.groupBoxUserVars = new System.Windows.Forms.GroupBox();
            this.dataGridUserVars = new System.Windows.Forms.DataGridView();
            this.ColumnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBoxConsole = new System.Windows.Forms.GroupBox();
            this.logConsole = new System.Windows.Forms.RichTextBox();
            this.menuStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerConsole)).BeginInit();
            this.splitContainerConsole.Panel1.SuspendLayout();
            this.splitContainerConsole.Panel2.SuspendLayout();
            this.splitContainerConsole.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerExamples)).BeginInit();
            this.splitContainerExamples.Panel1.SuspendLayout();
            this.splitContainerExamples.Panel2.SuspendLayout();
            this.splitContainerExamples.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerDescripcionExample)).BeginInit();
            this.splitContainerDescripcionExample.Panel1.SuspendLayout();
            this.splitContainerDescripcionExample.Panel2.SuspendLayout();
            this.splitContainerDescripcionExample.SuspendLayout();
            this.groupBoxExamples.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerUserVars)).BeginInit();
            this.splitContainerUserVars.Panel1.SuspendLayout();
            this.splitContainerUserVars.Panel2.SuspendLayout();
            this.splitContainerUserVars.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerModifiers)).BeginInit();
            this.splitContainerModifiers.Panel1.SuspendLayout();
            this.splitContainerModifiers.Panel2.SuspendLayout();
            this.splitContainerModifiers.SuspendLayout();
            this.groupBoxModifiers.SuspendLayout();
            this.groupBoxUserVars.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridUserVars)).BeginInit();
            this.groupBoxConsole.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.verToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(781, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip";
            // 
            // verToolStripMenuItem
            // 
            this.verToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.wireframeToolStripMenuItem,
            this.contadorFPSToolStripMenuItem,
            this.ejesCartesianosToolStripMenuItem,
            this.mostrarPosiciónDeCámaraToolStripMenuItem});
            this.verToolStripMenuItem.Name = "verToolStripMenuItem";
            this.verToolStripMenuItem.Size = new System.Drawing.Size(36, 20);
            this.verToolStripMenuItem.Text = "Ver";
            // 
            // wireframeToolStripMenuItem
            // 
            this.wireframeToolStripMenuItem.CheckOnClick = true;
            this.wireframeToolStripMenuItem.Name = "wireframeToolStripMenuItem";
            this.wireframeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.W)));
            this.wireframeToolStripMenuItem.Size = new System.Drawing.Size(258, 22);
            this.wireframeToolStripMenuItem.Text = "Wireframe";
            this.wireframeToolStripMenuItem.Click += new System.EventHandler(this.wireframeToolStripMenuItem_Click);
            // 
            // contadorFPSToolStripMenuItem
            // 
            this.contadorFPSToolStripMenuItem.Checked = true;
            this.contadorFPSToolStripMenuItem.CheckOnClick = true;
            this.contadorFPSToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.contadorFPSToolStripMenuItem.Name = "contadorFPSToolStripMenuItem";
            this.contadorFPSToolStripMenuItem.Size = new System.Drawing.Size(258, 22);
            this.contadorFPSToolStripMenuItem.Text = "Contador FPS";
            this.contadorFPSToolStripMenuItem.Click += new System.EventHandler(this.contadorFPSToolStripMenuItem_Click);
            // 
            // ejesCartesianosToolStripMenuItem
            // 
            this.ejesCartesianosToolStripMenuItem.Checked = true;
            this.ejesCartesianosToolStripMenuItem.CheckOnClick = true;
            this.ejesCartesianosToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ejesCartesianosToolStripMenuItem.Name = "ejesCartesianosToolStripMenuItem";
            this.ejesCartesianosToolStripMenuItem.Size = new System.Drawing.Size(258, 22);
            this.ejesCartesianosToolStripMenuItem.Text = "Ejes cartesianos";
            this.ejesCartesianosToolStripMenuItem.Click += new System.EventHandler(this.ejesCartesianosToolStripMenuItem_Click);
            // 
            // mostrarPosiciónDeCámaraToolStripMenuItem
            // 
            this.mostrarPosiciónDeCámaraToolStripMenuItem.CheckOnClick = true;
            this.mostrarPosiciónDeCámaraToolStripMenuItem.Name = "mostrarPosiciónDeCámaraToolStripMenuItem";
            this.mostrarPosiciónDeCámaraToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.P)));
            this.mostrarPosiciónDeCámaraToolStripMenuItem.Size = new System.Drawing.Size(258, 22);
            this.mostrarPosiciónDeCámaraToolStripMenuItem.Text = "Mostrar posición de cámara";
            this.mostrarPosiciónDeCámaraToolStripMenuItem.Click += new System.EventHandler(this.mostrarPosiciónDeCámaraToolStripMenuItem_Click);
            // 
            // toolStrip
            // 
            this.toolStrip.Location = new System.Drawing.Point(0, 24);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(781, 25);
            this.toolStrip.TabIndex = 1;
            this.toolStrip.Text = "toolStrip1";
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusCurrentExample,
            this.toolStripStatusPosition});
            this.statusStrip.Location = new System.Drawing.Point(0, 506);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(781, 22);
            this.statusStrip.TabIndex = 2;
            this.statusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusCurrentExample
            // 
            this.toolStripStatusCurrentExample.BackColor = System.Drawing.Color.GreenYellow;
            this.toolStripStatusCurrentExample.Name = "toolStripStatusCurrentExample";
            this.toolStripStatusCurrentExample.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripStatusPosition
            // 
            this.toolStripStatusPosition.Name = "toolStripStatusPosition";
            this.toolStripStatusPosition.Size = new System.Drawing.Size(0, 17);
            // 
            // splitContainerConsole
            // 
            this.splitContainerConsole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerConsole.IsSplitterFixed = true;
            this.splitContainerConsole.Location = new System.Drawing.Point(0, 49);
            this.splitContainerConsole.Name = "splitContainerConsole";
            this.splitContainerConsole.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerConsole.Panel1
            // 
            this.splitContainerConsole.Panel1.Controls.Add(this.splitContainerExamples);
            // 
            // splitContainerConsole.Panel2
            // 
            this.splitContainerConsole.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainerConsole.Panel2.Controls.Add(this.groupBoxConsole);
            this.splitContainerConsole.Size = new System.Drawing.Size(781, 457);
            this.splitContainerConsole.SplitterDistance = 369;
            this.splitContainerConsole.TabIndex = 3;
            // 
            // splitContainerExamples
            // 
            this.splitContainerExamples.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerExamples.IsSplitterFixed = true;
            this.splitContainerExamples.Location = new System.Drawing.Point(0, 0);
            this.splitContainerExamples.Name = "splitContainerExamples";
            // 
            // splitContainerExamples.Panel1
            // 
            this.splitContainerExamples.Panel1.Controls.Add(this.splitContainerDescripcionExample);
            // 
            // splitContainerExamples.Panel2
            // 
            this.splitContainerExamples.Panel2.Controls.Add(this.splitContainerUserVars);
            this.splitContainerExamples.Size = new System.Drawing.Size(781, 369);
            this.splitContainerExamples.SplitterDistance = 113;
            this.splitContainerExamples.TabIndex = 0;
            // 
            // splitContainerDescripcionExample
            // 
            this.splitContainerDescripcionExample.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerDescripcionExample.Location = new System.Drawing.Point(0, 0);
            this.splitContainerDescripcionExample.Name = "splitContainerDescripcionExample";
            this.splitContainerDescripcionExample.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerDescripcionExample.Panel1
            // 
            this.splitContainerDescripcionExample.Panel1.Controls.Add(this.groupBoxExamples);
            // 
            // splitContainerDescripcionExample.Panel2
            // 
            this.splitContainerDescripcionExample.Panel2.Controls.Add(this.textBoxExampleDescription);
            this.splitContainerDescripcionExample.Size = new System.Drawing.Size(113, 369);
            this.splitContainerDescripcionExample.SplitterDistance = 281;
            this.splitContainerDescripcionExample.TabIndex = 1;
            // 
            // groupBoxExamples
            // 
            this.groupBoxExamples.BackColor = System.Drawing.SystemColors.Control;
            this.groupBoxExamples.Controls.Add(this.treeViewExamples);
            this.groupBoxExamples.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxExamples.Location = new System.Drawing.Point(0, 0);
            this.groupBoxExamples.Name = "groupBoxExamples";
            this.groupBoxExamples.Size = new System.Drawing.Size(113, 281);
            this.groupBoxExamples.TabIndex = 0;
            this.groupBoxExamples.TabStop = false;
            this.groupBoxExamples.Text = "Ejemplos";
            // 
            // treeViewExamples
            // 
            this.treeViewExamples.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewExamples.Location = new System.Drawing.Point(3, 16);
            this.treeViewExamples.Name = "treeViewExamples";
            this.treeViewExamples.Size = new System.Drawing.Size(107, 262);
            this.treeViewExamples.TabIndex = 4;
            this.treeViewExamples.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewExamples_AfterSelect);
            this.treeViewExamples.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.treeViewExamples_MouseDoubleClick);
            // 
            // textBoxExampleDescription
            // 
            this.textBoxExampleDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxExampleDescription.Location = new System.Drawing.Point(0, 0);
            this.textBoxExampleDescription.Multiline = true;
            this.textBoxExampleDescription.Name = "textBoxExampleDescription";
            this.textBoxExampleDescription.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxExampleDescription.Size = new System.Drawing.Size(113, 84);
            this.textBoxExampleDescription.TabIndex = 0;
            // 
            // splitContainerUserVars
            // 
            this.splitContainerUserVars.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerUserVars.IsSplitterFixed = true;
            this.splitContainerUserVars.Location = new System.Drawing.Point(0, 0);
            this.splitContainerUserVars.Name = "splitContainerUserVars";
            // 
            // splitContainerUserVars.Panel1
            // 
            this.splitContainerUserVars.Panel1.Controls.Add(this.panel3d);
            // 
            // splitContainerUserVars.Panel2
            // 
            this.splitContainerUserVars.Panel2.Controls.Add(this.splitContainerModifiers);
            this.splitContainerUserVars.Size = new System.Drawing.Size(664, 369);
            this.splitContainerUserVars.SplitterDistance = 548;
            this.splitContainerUserVars.TabIndex = 0;
            // 
            // panel3d
            // 
            this.panel3d.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.panel3d.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3d.Location = new System.Drawing.Point(0, 0);
            this.panel3d.Name = "panel3d";
            this.panel3d.Size = new System.Drawing.Size(548, 369);
            this.panel3d.TabIndex = 0;
            // 
            // splitContainerModifiers
            // 
            this.splitContainerModifiers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerModifiers.Location = new System.Drawing.Point(0, 0);
            this.splitContainerModifiers.Name = "splitContainerModifiers";
            this.splitContainerModifiers.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerModifiers.Panel1
            // 
            this.splitContainerModifiers.Panel1.Controls.Add(this.groupBoxModifiers);
            this.splitContainerModifiers.Panel1MinSize = 50;
            // 
            // splitContainerModifiers.Panel2
            // 
            this.splitContainerModifiers.Panel2.Controls.Add(this.groupBoxUserVars);
            this.splitContainerModifiers.Size = new System.Drawing.Size(112, 369);
            this.splitContainerModifiers.SplitterDistance = 221;
            this.splitContainerModifiers.TabIndex = 1;
            // 
            // groupBoxModifiers
            // 
            this.groupBoxModifiers.BackColor = System.Drawing.SystemColors.Control;
            this.groupBoxModifiers.Controls.Add(this.flowLayoutPanelModifiers);
            this.groupBoxModifiers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxModifiers.Location = new System.Drawing.Point(0, 0);
            this.groupBoxModifiers.Name = "groupBoxModifiers";
            this.groupBoxModifiers.Size = new System.Drawing.Size(112, 221);
            this.groupBoxModifiers.TabIndex = 0;
            this.groupBoxModifiers.TabStop = false;
            this.groupBoxModifiers.Text = "Modifiers";
            // 
            // flowLayoutPanelModifiers
            // 
            this.flowLayoutPanelModifiers.AutoScroll = true;
            this.flowLayoutPanelModifiers.AutoSize = true;
            this.flowLayoutPanelModifiers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanelModifiers.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanelModifiers.Location = new System.Drawing.Point(3, 16);
            this.flowLayoutPanelModifiers.Name = "flowLayoutPanelModifiers";
            this.flowLayoutPanelModifiers.Size = new System.Drawing.Size(106, 202);
            this.flowLayoutPanelModifiers.TabIndex = 0;
            this.flowLayoutPanelModifiers.WrapContents = false;
            // 
            // groupBoxUserVars
            // 
            this.groupBoxUserVars.BackColor = System.Drawing.SystemColors.Control;
            this.groupBoxUserVars.Controls.Add(this.dataGridUserVars);
            this.groupBoxUserVars.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxUserVars.Location = new System.Drawing.Point(0, 0);
            this.groupBoxUserVars.Name = "groupBoxUserVars";
            this.groupBoxUserVars.Size = new System.Drawing.Size(112, 144);
            this.groupBoxUserVars.TabIndex = 0;
            this.groupBoxUserVars.TabStop = false;
            this.groupBoxUserVars.Text = "User variables";
            // 
            // dataGridUserVars
            // 
            this.dataGridUserVars.AllowUserToAddRows = false;
            this.dataGridUserVars.AllowUserToDeleteRows = false;
            this.dataGridUserVars.AllowUserToOrderColumns = true;
            this.dataGridUserVars.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridUserVars.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnName,
            this.ColumnValue});
            this.dataGridUserVars.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridUserVars.Location = new System.Drawing.Point(3, 16);
            this.dataGridUserVars.MultiSelect = false;
            this.dataGridUserVars.Name = "dataGridUserVars";
            this.dataGridUserVars.ReadOnly = true;
            this.dataGridUserVars.RowHeadersWidth = 10;
            this.dataGridUserVars.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridUserVars.Size = new System.Drawing.Size(106, 125);
            this.dataGridUserVars.TabIndex = 0;
            // 
            // ColumnName
            // 
            this.ColumnName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColumnName.DataPropertyName = "Name";
            this.ColumnName.HeaderText = "Name";
            this.ColumnName.Name = "ColumnName";
            this.ColumnName.ReadOnly = true;
            this.ColumnName.Width = 60;
            // 
            // ColumnValue
            // 
            this.ColumnValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnValue.DataPropertyName = "Value";
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ColumnValue.DefaultCellStyle = dataGridViewCellStyle2;
            this.ColumnValue.HeaderText = "Value";
            this.ColumnValue.Name = "ColumnValue";
            this.ColumnValue.ReadOnly = true;
            // 
            // groupBoxConsole
            // 
            this.groupBoxConsole.Controls.Add(this.logConsole);
            this.groupBoxConsole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxConsole.Location = new System.Drawing.Point(0, 0);
            this.groupBoxConsole.Name = "groupBoxConsole";
            this.groupBoxConsole.Size = new System.Drawing.Size(781, 84);
            this.groupBoxConsole.TabIndex = 0;
            this.groupBoxConsole.TabStop = false;
            this.groupBoxConsole.Text = "Console";
            // 
            // logConsole
            // 
            this.logConsole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logConsole.Location = new System.Drawing.Point(3, 16);
            this.logConsole.Name = "logConsole";
            this.logConsole.Size = new System.Drawing.Size(775, 65);
            this.logConsole.TabIndex = 0;
            this.logConsole.Text = "";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(781, 528);
            this.Controls.Add(this.splitContainerConsole);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.MinimumSize = new System.Drawing.Size(789, 558);
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.Text = "UTNPhysicsEngine - Visualizador de prototipos";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.splitContainerConsole.Panel1.ResumeLayout(false);
            this.splitContainerConsole.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerConsole)).EndInit();
            this.splitContainerConsole.ResumeLayout(false);
            this.splitContainerExamples.Panel1.ResumeLayout(false);
            this.splitContainerExamples.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerExamples)).EndInit();
            this.splitContainerExamples.ResumeLayout(false);
            this.splitContainerDescripcionExample.Panel1.ResumeLayout(false);
            this.splitContainerDescripcionExample.Panel2.ResumeLayout(false);
            this.splitContainerDescripcionExample.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerDescripcionExample)).EndInit();
            this.splitContainerDescripcionExample.ResumeLayout(false);
            this.groupBoxExamples.ResumeLayout(false);
            this.splitContainerUserVars.Panel1.ResumeLayout(false);
            this.splitContainerUserVars.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerUserVars)).EndInit();
            this.splitContainerUserVars.ResumeLayout(false);
            this.splitContainerModifiers.Panel1.ResumeLayout(false);
            this.splitContainerModifiers.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerModifiers)).EndInit();
            this.splitContainerModifiers.ResumeLayout(false);
            this.groupBoxModifiers.ResumeLayout(false);
            this.groupBoxModifiers.PerformLayout();
            this.groupBoxUserVars.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridUserVars)).EndInit();
            this.groupBoxConsole.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.SplitContainer splitContainerConsole;
        private System.Windows.Forms.SplitContainer splitContainerExamples;
        private System.Windows.Forms.GroupBox groupBoxExamples;
        private System.Windows.Forms.GroupBox groupBoxConsole;
        private System.Windows.Forms.SplitContainer splitContainerUserVars;
        private System.Windows.Forms.GroupBox groupBoxUserVars;
        private System.Windows.Forms.Panel panel3d;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusPosition;
        private System.Windows.Forms.ToolStripMenuItem verToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem wireframeToolStripMenuItem;
        private System.Windows.Forms.RichTextBox logConsole;
        private System.Windows.Forms.ToolStripMenuItem contadorFPSToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusCurrentExample;
        private System.Windows.Forms.DataGridView dataGridUserVars;
        private System.Windows.Forms.SplitContainer splitContainerDescripcionExample;
        private System.Windows.Forms.TextBox textBoxExampleDescription;
        private System.Windows.Forms.SplitContainer splitContainerModifiers;
        private System.Windows.Forms.GroupBox groupBoxModifiers;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelModifiers;
        private System.Windows.Forms.ToolStripMenuItem ejesCartesianosToolStripMenuItem;
        private System.Windows.Forms.TreeView treeViewExamples;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnValue;
        private System.Windows.Forms.ToolStripMenuItem mostrarPosiciónDeCámaraToolStripMenuItem;
    }
}

