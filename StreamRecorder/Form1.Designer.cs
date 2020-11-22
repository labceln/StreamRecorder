namespace StreamRecorder
{
  partial class Form1
  {
    /// <summary>
    /// 必要なデザイナー変数です。
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// 使用中のリソースをすべてクリーンアップします。
    /// </summary>
    /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows フォーム デザイナーで生成されたコード

    /// <summary>
    /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
    /// コード エディターで変更しないでください。
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
      this.dataGridView1 = new System.Windows.Forms.DataGridView();
      this.url = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.menuStrip1 = new System.Windows.Forms.MenuStrip();
      this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.saveFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.station = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.bitrateDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.audioTypeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.songTitleDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.isRipping = new System.Windows.Forms.DataGridViewButtonColumn();
      this.userModelBindingSource = new System.Windows.Forms.BindingSource(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
      this.menuStrip1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.userModelBindingSource)).BeginInit();
      this.SuspendLayout();
      // 
      // dataGridView1
      // 
      this.dataGridView1.AllowUserToAddRows = false;
      this.dataGridView1.AutoGenerateColumns = false;
      this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
      this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.station,
            this.url,
            this.bitrateDataGridViewTextBoxColumn,
            this.audioTypeDataGridViewTextBoxColumn,
            this.songTitleDataGridViewTextBoxColumn,
            this.isRipping});
      this.dataGridView1.DataSource = this.userModelBindingSource;
      this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dataGridView1.Location = new System.Drawing.Point(0, 24);
      this.dataGridView1.Name = "dataGridView1";
      this.dataGridView1.RowTemplate.Height = 21;
      this.dataGridView1.Size = new System.Drawing.Size(998, 550);
      this.dataGridView1.TabIndex = 0;
      this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
      // 
      // url
      // 
      this.url.DataPropertyName = "url";
      this.url.HeaderText = "URL";
      this.url.Name = "url";
      this.url.ReadOnly = true;
      // 
      // menuStrip1
      // 
      this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.saveFolderToolStripMenuItem});
      this.menuStrip1.Location = new System.Drawing.Point(0, 0);
      this.menuStrip1.Name = "menuStrip1";
      this.menuStrip1.Size = new System.Drawing.Size(998, 24);
      this.menuStrip1.TabIndex = 1;
      this.menuStrip1.Text = "menuStrip1";
      // 
      // fileToolStripMenuItem
      // 
      this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
      this.fileToolStripMenuItem.Size = new System.Drawing.Size(74, 20);
      this.fileToolStripMenuItem.Text = "Open m3u";
      this.fileToolStripMenuItem.Click += new System.EventHandler(this.fileToolStripMenuItem_Click);
      // 
      // saveFolderToolStripMenuItem
      // 
      this.saveFolderToolStripMenuItem.Name = "saveFolderToolStripMenuItem";
      this.saveFolderToolStripMenuItem.Size = new System.Drawing.Size(77, 20);
      this.saveFolderToolStripMenuItem.Text = "Save folder";
      this.saveFolderToolStripMenuItem.Click += new System.EventHandler(this.saveFolderToolStripMenuItem_Click);
      // 
      // station
      // 
      this.station.DataPropertyName = "station";
      this.station.HeaderText = "Station";
      this.station.Name = "station";
      this.station.ReadOnly = true;
      this.station.Resizable = System.Windows.Forms.DataGridViewTriState.True;
      // 
      // bitrateDataGridViewTextBoxColumn
      // 
      this.bitrateDataGridViewTextBoxColumn.DataPropertyName = "bitrate";
      this.bitrateDataGridViewTextBoxColumn.FillWeight = 30F;
      this.bitrateDataGridViewTextBoxColumn.HeaderText = "Bitrate";
      this.bitrateDataGridViewTextBoxColumn.Name = "bitrateDataGridViewTextBoxColumn";
      this.bitrateDataGridViewTextBoxColumn.ReadOnly = true;
      // 
      // audioTypeDataGridViewTextBoxColumn
      // 
      this.audioTypeDataGridViewTextBoxColumn.DataPropertyName = "audioType";
      this.audioTypeDataGridViewTextBoxColumn.FillWeight = 40F;
      this.audioTypeDataGridViewTextBoxColumn.HeaderText = "AudioType";
      this.audioTypeDataGridViewTextBoxColumn.Name = "audioTypeDataGridViewTextBoxColumn";
      this.audioTypeDataGridViewTextBoxColumn.ReadOnly = true;
      // 
      // songTitleDataGridViewTextBoxColumn
      // 
      this.songTitleDataGridViewTextBoxColumn.DataPropertyName = "songTitle";
      this.songTitleDataGridViewTextBoxColumn.HeaderText = "SongTitle";
      this.songTitleDataGridViewTextBoxColumn.Name = "songTitleDataGridViewTextBoxColumn";
      this.songTitleDataGridViewTextBoxColumn.ReadOnly = true;
      // 
      // isRipping
      // 
      this.isRipping.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
      this.isRipping.DataPropertyName = "isRipping";
      dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
      dataGridViewCellStyle1.NullValue = "RIP";
      this.isRipping.DefaultCellStyle = dataGridViewCellStyle1;
      this.isRipping.FillWeight = 60F;
      this.isRipping.HeaderText = "isRipping";
      this.isRipping.Name = "isRipping";
      this.isRipping.Resizable = System.Windows.Forms.DataGridViewTriState.True;
      this.isRipping.Text = "";
      // 
      // userModelBindingSource
      // 
      this.userModelBindingSource.DataSource = typeof(StreamRecorder.UserModel);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(998, 574);
      this.Controls.Add(this.dataGridView1);
      this.Controls.Add(this.menuStrip1);
      this.MainMenuStrip = this.menuStrip1;
      this.Name = "Form1";
      this.Text = "StreamRecorder";
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
      this.menuStrip1.ResumeLayout(false);
      this.menuStrip1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.userModelBindingSource)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.DataGridView dataGridView1;
    private System.Windows.Forms.BindingSource userModelBindingSource;
    private System.Windows.Forms.MenuStrip menuStrip1;
    private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem saveFolderToolStripMenuItem;
    private System.Windows.Forms.DataGridViewTextBoxColumn station;
    private System.Windows.Forms.DataGridViewTextBoxColumn url;
    private System.Windows.Forms.DataGridViewTextBoxColumn bitrateDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn audioTypeDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn songTitleDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewButtonColumn isRipping;
  }
}

