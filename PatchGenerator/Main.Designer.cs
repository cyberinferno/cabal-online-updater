namespace PatchGenerator
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.headingLabel = new System.Windows.Forms.Label();
            this.inputFolderLabel = new System.Windows.Forms.Label();
            this.outputFolderLabel = new System.Windows.Forms.Label();
            this.inputFolderTb = new System.Windows.Forms.TextBox();
            this.outputFolderTb = new System.Windows.Forms.TextBox();
            this.inputError = new System.Windows.Forms.Label();
            this.outputError = new System.Windows.Forms.Label();
            this.generateBtn = new System.Windows.Forms.Button();
            this.patchWorker = new System.ComponentModel.BackgroundWorker();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.outputLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // headingLabel
            // 
            this.headingLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.headingLabel.Location = new System.Drawing.Point(122, 9);
            this.headingLabel.Name = "headingLabel";
            this.headingLabel.Size = new System.Drawing.Size(196, 32);
            this.headingLabel.TabIndex = 0;
            this.headingLabel.Text = "Generate Patch";
            this.headingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // inputFolderLabel
            // 
            this.inputFolderLabel.AutoSize = true;
            this.inputFolderLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.inputFolderLabel.Location = new System.Drawing.Point(12, 50);
            this.inputFolderLabel.Name = "inputFolderLabel";
            this.inputFolderLabel.Size = new System.Drawing.Size(95, 17);
            this.inputFolderLabel.TabIndex = 1;
            this.inputFolderLabel.Text = "Input Folder";
            // 
            // outputFolderLabel
            // 
            this.outputFolderLabel.AutoSize = true;
            this.outputFolderLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.outputFolderLabel.Location = new System.Drawing.Point(12, 125);
            this.outputFolderLabel.Name = "outputFolderLabel";
            this.outputFolderLabel.Size = new System.Drawing.Size(108, 17);
            this.outputFolderLabel.TabIndex = 2;
            this.outputFolderLabel.Text = "Output Folder";
            // 
            // inputFolderTb
            // 
            this.inputFolderTb.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.inputFolderTb.Location = new System.Drawing.Point(15, 83);
            this.inputFolderTb.Name = "inputFolderTb";
            this.inputFolderTb.Size = new System.Drawing.Size(426, 23);
            this.inputFolderTb.TabIndex = 3;
            this.inputFolderTb.TextChanged += new System.EventHandler(this.inputFolderTb_TextChanged);
            // 
            // outputFolderTb
            // 
            this.outputFolderTb.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.outputFolderTb.Location = new System.Drawing.Point(15, 162);
            this.outputFolderTb.Name = "outputFolderTb";
            this.outputFolderTb.Size = new System.Drawing.Size(426, 23);
            this.outputFolderTb.TabIndex = 4;
            this.outputFolderTb.TextChanged += new System.EventHandler(this.outputFolderTb_TextChanged);
            // 
            // inputError
            // 
            this.inputError.AutoSize = true;
            this.inputError.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.inputError.ForeColor = System.Drawing.Color.Red;
            this.inputError.Location = new System.Drawing.Point(143, 67);
            this.inputError.Name = "inputError";
            this.inputError.Size = new System.Drawing.Size(175, 13);
            this.inputError.TabIndex = 5;
            this.inputError.Text = "Invalid input directory chosen";
            this.inputError.Visible = false;
            // 
            // outputError
            // 
            this.outputError.AutoSize = true;
            this.outputError.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.outputError.ForeColor = System.Drawing.Color.Red;
            this.outputError.Location = new System.Drawing.Point(143, 146);
            this.outputError.Name = "outputError";
            this.outputError.Size = new System.Drawing.Size(183, 13);
            this.outputError.TabIndex = 6;
            this.outputError.Text = "Invalid output directory chosen";
            this.outputError.Visible = false;
            // 
            // generateBtn
            // 
            this.generateBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.generateBtn.Location = new System.Drawing.Point(43, 244);
            this.generateBtn.Name = "generateBtn";
            this.generateBtn.Size = new System.Drawing.Size(119, 40);
            this.generateBtn.TabIndex = 7;
            this.generateBtn.Text = "Generate";
            this.generateBtn.UseVisualStyleBackColor = true;
            this.generateBtn.Click += new System.EventHandler(this.generateBtn_Click);
            // 
            // patchWorker
            // 
            this.patchWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.patchWorker_DoWork);
            this.patchWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.patchWorker_ProgressChanged);
            this.patchWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.patchWorker_RunWorkerCompleted);
            // 
            // cancelBtn
            // 
            this.cancelBtn.Enabled = false;
            this.cancelBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelBtn.Location = new System.Drawing.Point(243, 244);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(119, 40);
            this.cancelBtn.TabIndex = 8;
            this.cancelBtn.Text = "Cancel";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // outputLabel
            // 
            this.outputLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.outputLabel.Location = new System.Drawing.Point(3, 198);
            this.outputLabel.Name = "outputLabel";
            this.outputLabel.Size = new System.Drawing.Size(457, 31);
            this.outputLabel.TabIndex = 9;
            this.outputLabel.Text = "Click on generate to start";
            this.outputLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(463, 307);
            this.Controls.Add(this.outputLabel);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.generateBtn);
            this.Controls.Add(this.outputError);
            this.Controls.Add(this.inputError);
            this.Controls.Add(this.outputFolderTb);
            this.Controls.Add(this.inputFolderTb);
            this.Controls.Add(this.outputFolderLabel);
            this.Controls.Add(this.inputFolderLabel);
            this.Controls.Add(this.headingLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cabal Online Patch Generator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label headingLabel;
        private System.Windows.Forms.Label inputFolderLabel;
        private System.Windows.Forms.Label outputFolderLabel;
        private System.Windows.Forms.TextBox inputFolderTb;
        private System.Windows.Forms.TextBox outputFolderTb;
        private System.Windows.Forms.Label inputError;
        private System.Windows.Forms.Label outputError;
        private System.Windows.Forms.Button generateBtn;
        private System.ComponentModel.BackgroundWorker patchWorker;
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.Label outputLabel;
    }
}

