namespace GoToBrowser.Options
{
    partial class GeneralOptionPage
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

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.formatDescription = new System.Windows.Forms.Label();
            this.urlFormat = new System.Windows.Forms.TextBox();
            this.urlFormatHeader = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.solutionGroup = new System.Windows.Forms.GroupBox();
            this.solutionName = new System.Windows.Forms.TextBox();
            this.solutionGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // formatDescription
            // 
            this.formatDescription.AutoSize = true;
            this.formatDescription.Location = new System.Drawing.Point(9, 102);
            this.formatDescription.Name = "formatDescription";
            this.formatDescription.Size = new System.Drawing.Size(367, 48);
            this.formatDescription.TabIndex = 0;
            this.formatDescription.Text = "Defines the URL format of destination file.\r\nex) http://code.google.com/p/{SN}/so" +
                "urce/browse/trunk/{SN}{FP}#{LN}\r\n\r\nThe following special keywords will be replac" +
                "ed with current values.";
            // 
            // urlFormat
            // 
            this.urlFormat.Location = new System.Drawing.Point(15, 71);
            this.urlFormat.Name = "urlFormat";
            this.urlFormat.Size = new System.Drawing.Size(421, 19);
            this.urlFormat.TabIndex = 1;
            // 
            // urlFormatHeader
            // 
            this.urlFormatHeader.AutoSize = true;
            this.urlFormatHeader.Location = new System.Drawing.Point(13, 56);
            this.urlFormatHeader.Name = "urlFormatHeader";
            this.urlFormatHeader.Size = new System.Drawing.Size(67, 12);
            this.urlFormatHeader.TabIndex = 2;
            this.urlFormatHeader.Text = "URL Format";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 160);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(239, 48);
            this.label2.TabIndex = 5;
            this.label2.Text = "{FileName} or {FN}: Currnt file name\r\n{FilePath} or {FP}: Current file relative p" +
                "ath\r\n{LineNumber} or {LN}: Current line number\r\n{SolutionName} or {SN}: Current " +
                "solution name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 30);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "Solution Name";
            // 
            // solutionGroup
            // 
            this.solutionGroup.Controls.Add(this.solutionName);
            this.solutionGroup.Controls.Add(this.label3);
            this.solutionGroup.Controls.Add(this.label2);
            this.solutionGroup.Controls.Add(this.urlFormatHeader);
            this.solutionGroup.Controls.Add(this.formatDescription);
            this.solutionGroup.Controls.Add(this.urlFormat);
            this.solutionGroup.Location = new System.Drawing.Point(3, 13);
            this.solutionGroup.Name = "solutionGroup";
            this.solutionGroup.Size = new System.Drawing.Size(450, 233);
            this.solutionGroup.TabIndex = 8;
            this.solutionGroup.TabStop = false;
            this.solutionGroup.Text = "Solution Settings";
            // 
            // solutionName
            // 
            this.solutionName.Location = new System.Drawing.Point(98, 27);
            this.solutionName.Name = "solutionName";
            this.solutionName.ReadOnly = true;
            this.solutionName.Size = new System.Drawing.Size(338, 19);
            this.solutionName.TabIndex = 7;
            // 
            // GeneralOptionPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.solutionGroup);
            this.Name = "GeneralOptionPage";
            this.Size = new System.Drawing.Size(456, 260);
            this.solutionGroup.ResumeLayout(false);
            this.solutionGroup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label formatDescription;
        private System.Windows.Forms.TextBox urlFormat;
        private System.Windows.Forms.Label urlFormatHeader;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox solutionGroup;
        private System.Windows.Forms.TextBox solutionName;

    }
}
