namespace test01
{
    partial class MainForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
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
            this.btnExecute = new System.Windows.Forms.Button();
            this.txtlnput = new System.Windows.Forms.TextBox();
            this.lsvOutput = new System.Windows.Forms.ListView();
            this.clhTitle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clhSummary = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clhUrl = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chblsBigram = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnExecute
            // 
            this.btnExecute.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExecute.Location = new System.Drawing.Point(98, 37);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(271, 23);
            this.btnExecute.TabIndex = 0;
            this.btnExecute.Text = "実行";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // txtlnput
            // 
            this.txtlnput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtlnput.Location = new System.Drawing.Point(12, 12);
            this.txtlnput.Name = "txtlnput";
            this.txtlnput.Size = new System.Drawing.Size(357, 19);
            this.txtlnput.TabIndex = 1;
            this.txtlnput.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // lsvOutput
            // 
            this.lsvOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lsvOutput.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clhTitle,
            this.clhSummary,
            this.clhUrl});
            this.lsvOutput.Location = new System.Drawing.Point(12, 66);
            this.lsvOutput.Name = "lsvOutput";
            this.lsvOutput.Size = new System.Drawing.Size(357, 225);
            this.lsvOutput.TabIndex = 2;
            this.lsvOutput.UseCompatibleStateImageBehavior = false;
            this.lsvOutput.View = System.Windows.Forms.View.Details;
            // 
            // clhTitle
            // 
            this.clhTitle.Text = "Title";
            // 
            // clhSummary
            // 
            this.clhSummary.Text = "Summary";
            // 
            // clhUrl
            // 
            this.clhUrl.Text = "URL";
            // 
            // chblsBigram
            // 
            this.chblsBigram.AutoSize = true;
            this.chblsBigram.Location = new System.Drawing.Point(12, 41);
            this.chblsBigram.Name = "chblsBigram";
            this.chblsBigram.Size = new System.Drawing.Size(65, 16);
            this.chblsBigram.TabIndex = 3;
            this.chblsBigram.Text = "Bigram?";
            this.chblsBigram.UseVisualStyleBackColor = true;
            this.chblsBigram.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(385, 303);
            this.Controls.Add(this.chblsBigram);
            this.Controls.Add(this.lsvOutput);
            this.Controls.Add(this.txtlnput);
            this.Controls.Add(this.btnExecute);
            this.Name = "MainForm";
            this.Text = "Enshu4";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.TextBox txtlnput;
        private System.Windows.Forms.ListView lsvOutput;
        private System.Windows.Forms.ColumnHeader clhTitle;
        private System.Windows.Forms.ColumnHeader clhSummary;
        private System.Windows.Forms.ColumnHeader clhUrl;
        private System.Windows.Forms.CheckBox chblsBigram;
    }
}

