namespace MLP_Price_Retriever
{
    partial class AppForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.GetPricesButton = new System.Windows.Forms.Button();
            this.MLPGridView = new System.Windows.Forms.DataGridView();
            this.mlpTicker = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mlpName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mlpType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mlpStartDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.mlpStatusStrip = new System.Windows.Forms.StatusStrip();
            this.stripLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.stripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.SetPathButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.MLPGridView)).BeginInit();
            this.mlpStatusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // GetPricesButton
            // 
            this.GetPricesButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.GetPricesButton.Location = new System.Drawing.Point(12, 470);
            this.GetPricesButton.Name = "GetPricesButton";
            this.GetPricesButton.Size = new System.Drawing.Size(75, 23);
            this.GetPricesButton.TabIndex = 0;
            this.GetPricesButton.Text = "Get Prices";
            this.GetPricesButton.UseVisualStyleBackColor = true;
            this.GetPricesButton.Click += new System.EventHandler(this.GetPricesButton_Click);
            // 
            // MLPGridView
            // 
            this.MLPGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MLPGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.MLPGridView.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.MLPGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.MLPGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.mlpTicker,
            this.mlpName,
            this.mlpType,
            this.mlpStartDate});
            this.MLPGridView.Location = new System.Drawing.Point(13, 12);
            this.MLPGridView.Name = "MLPGridView";
            this.MLPGridView.Size = new System.Drawing.Size(851, 452);
            this.MLPGridView.TabIndex = 1;
            this.MLPGridView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PasteToDGV);
            this.MLPGridView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.RightClickOnDGV);
            // 
            // mlpTicker
            // 
            this.mlpTicker.HeaderText = "MLP Ticker";
            this.mlpTicker.Name = "mlpTicker";
            // 
            // mlpName
            // 
            this.mlpName.HeaderText = "LongName";
            this.mlpName.Name = "mlpName";
            // 
            // mlpType
            // 
            this.mlpType.HeaderText = "MLP Type";
            this.mlpType.Name = "mlpType";
            // 
            // mlpStartDate
            // 
            this.mlpStartDate.HeaderText = "Start Date";
            this.mlpStartDate.Name = "mlpStartDate";
            // 
            // mlpStatusStrip
            // 
            this.mlpStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stripLabel,
            this.stripProgressBar});
            this.mlpStatusStrip.Location = new System.Drawing.Point(0, 496);
            this.mlpStatusStrip.Name = "mlpStatusStrip";
            this.mlpStatusStrip.Size = new System.Drawing.Size(876, 22);
            this.mlpStatusStrip.TabIndex = 4;
            // 
            // stripLabel
            // 
            this.stripLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.stripLabel.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.stripLabel.Name = "stripLabel";
            this.stripLabel.Size = new System.Drawing.Size(728, 17);
            this.stripLabel.Spring = true;
            this.stripLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // stripProgressBar
            // 
            this.stripProgressBar.Minimum = 1;
            this.stripProgressBar.Name = "stripProgressBar";
            this.stripProgressBar.Size = new System.Drawing.Size(100, 16);
            this.stripProgressBar.Step = 1;
            this.stripProgressBar.Value = 1;
            // 
            // SetPathButton
            // 
            this.SetPathButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.SetPathButton.Enabled = false;
            this.SetPathButton.Location = new System.Drawing.Point(93, 470);
            this.SetPathButton.Name = "SetPathButton";
            this.SetPathButton.Size = new System.Drawing.Size(75, 23);
            this.SetPathButton.TabIndex = 5;
            this.SetPathButton.Text = "Save File";
            this.SetPathButton.UseVisualStyleBackColor = true;
            this.SetPathButton.Click += new System.EventHandler(this.SetPathButton_Click);
            // 
            // AppForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(876, 518);
            this.Controls.Add(this.SetPathButton);
            this.Controls.Add(this.mlpStatusStrip);
            this.Controls.Add(this.MLPGridView);
            this.Controls.Add(this.GetPricesButton);
            this.Name = "AppForm";
            this.Text = "MLP Price Retriever";
            ((System.ComponentModel.ISupportInitialize)(this.MLPGridView)).EndInit();
            this.mlpStatusStrip.ResumeLayout(false);
            this.mlpStatusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button GetPricesButton;
        private System.Windows.Forms.DataGridView MLPGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn mlpTicker;
        private System.Windows.Forms.DataGridViewTextBoxColumn mlpName;
        private System.Windows.Forms.DataGridViewTextBoxColumn mlpType;
        private System.Windows.Forms.DataGridViewTextBoxColumn mlpStartDate;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.StatusStrip mlpStatusStrip;
        private volatile System.Windows.Forms.ToolStripStatusLabel stripLabel;
        private volatile System.Windows.Forms.ToolStripProgressBar stripProgressBar;
        private System.Windows.Forms.Button SetPathButton;
    }
}

