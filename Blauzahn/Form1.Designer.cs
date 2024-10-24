namespace Blauzahn
{
    partial class Form1
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.outputFormatLabel = new System.Windows.Forms.Label();
            this.connectButton = new System.Windows.Forms.Button();
            this.disconnectButton = new System.Windows.Forms.Button();
            this.BluetoothDevicesListBox = new System.Windows.Forms.ListBox();
            this.sensorNamesTextBox = new System.Windows.Forms.TextBox();
            this.sensorNamesLabel = new System.Windows.Forms.Label();
            this.saveFormatComboBox = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoScroll = true;
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33777F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33778F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66222F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66222F));
            this.tableLayoutPanel1.Controls.Add(this.outputFormatLabel, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.connectButton, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.disconnectButton, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.BluetoothDevicesListBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.sensorNamesTextBox, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.sensorNamesLabel, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.saveFormatComboBox, 3, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(800, 450);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // outputFormatLabel
            // 
            this.outputFormatLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.outputFormatLabel.AutoSize = true;
            this.outputFormatLabel.Location = new System.Drawing.Point(535, 187);
            this.outputFormatLabel.Name = "outputFormatLabel";
            this.outputFormatLabel.Padding = new System.Windows.Forms.Padding(4);
            this.outputFormatLabel.Size = new System.Drawing.Size(117, 24);
            this.outputFormatLabel.TabIndex = 5;
            this.outputFormatLabel.Text = "Speicher Format:";
            // 
            // connectButton
            // 
            this.connectButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.connectButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.connectButton.Location = new System.Drawing.Point(3, 401);
            this.connectButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.connectButton.MaximumSize = new System.Drawing.Size(0, 39);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(260, 39);
            this.connectButton.TabIndex = 0;
            this.connectButton.Text = "Verbinden";
            this.connectButton.UseVisualStyleBackColor = true;
            // 
            // disconnectButton
            // 
            this.disconnectButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.disconnectButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.disconnectButton.Location = new System.Drawing.Point(269, 401);
            this.disconnectButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.disconnectButton.MaximumSize = new System.Drawing.Size(0, 39);
            this.disconnectButton.Name = "disconnectButton";
            this.disconnectButton.Size = new System.Drawing.Size(260, 39);
            this.disconnectButton.TabIndex = 1;
            this.disconnectButton.Text = "Trennen";
            this.disconnectButton.UseVisualStyleBackColor = true;
            // 
            // BluetoothDevicesListBox
            // 
            this.BluetoothDevicesListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.BluetoothDevicesListBox, 2);
            this.BluetoothDevicesListBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.BluetoothDevicesListBox.FormattingEnabled = true;
            this.BluetoothDevicesListBox.ItemHeight = 16;
            this.BluetoothDevicesListBox.Location = new System.Drawing.Point(3, 2);
            this.BluetoothDevicesListBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.BluetoothDevicesListBox.Name = "BluetoothDevicesListBox";
            this.tableLayoutPanel1.SetRowSpan(this.BluetoothDevicesListBox, 3);
            this.BluetoothDevicesListBox.Size = new System.Drawing.Size(526, 388);
            this.BluetoothDevicesListBox.TabIndex = 2;
            // 
            // sensorNamesTextBox
            // 
            this.sensorNamesTextBox.AcceptsReturn = true;
            this.sensorNamesTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sensorNamesTextBox.Location = new System.Drawing.Point(668, 3);
            this.sensorNamesTextBox.Multiline = true;
            this.sensorNamesTextBox.Name = "sensorNamesTextBox";
            this.sensorNamesTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.sensorNamesTextBox.Size = new System.Drawing.Size(129, 127);
            this.sensorNamesTextBox.TabIndex = 3;
            this.sensorNamesTextBox.TextChanged += new System.EventHandler(this.sensorNamesTextBox_TextChanged);
            // 
            // sensorNamesLabel
            // 
            this.sensorNamesLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.sensorNamesLabel.AutoSize = true;
            this.sensorNamesLabel.Location = new System.Drawing.Point(535, 22);
            this.sensorNamesLabel.Name = "sensorNamesLabel";
            this.sensorNamesLabel.Padding = new System.Windows.Forms.Padding(4);
            this.sensorNamesLabel.Size = new System.Drawing.Size(125, 88);
            this.sensorNamesLabel.TabIndex = 4;
            this.sensorNamesLabel.Text = "Sensorenname(n):\r\n(ein Name pro Zeile; z.B. \'LPMS\') \r\n(filtert die Liste links)\r\n" +
    "";
            // 
            // saveFormatComboBox
            // 
            this.saveFormatComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.saveFormatComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.saveFormatComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.saveFormatComboBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.saveFormatComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.saveFormatComboBox.Location = new System.Drawing.Point(668, 187);
            this.saveFormatComboBox.Name = "saveFormatComboBox";
            this.saveFormatComboBox.Size = new System.Drawing.Size(129, 24);
            this.saveFormatComboBox.TabIndex = 6;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.Button disconnectButton;
        private System.Windows.Forms.ListBox BluetoothDevicesListBox;
        private System.Windows.Forms.TextBox sensorNamesTextBox;
        private System.Windows.Forms.Label sensorNamesLabel;
        private System.Windows.Forms.Label outputFormatLabel;
        private System.Windows.Forms.ComboBox saveFormatComboBox;
    }
}

