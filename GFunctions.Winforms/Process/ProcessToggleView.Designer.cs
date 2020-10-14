namespace GFunctions.Winforms.Process
{
    partial class ProcessToggleView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button_toggle = new System.Windows.Forms.Button();
            this.label_status = new System.Windows.Forms.Label();
            this.progressBar_progress = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // button_toggle
            // 
            this.button_toggle.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_toggle.Location = new System.Drawing.Point(3, 3);
            this.button_toggle.Name = "button_toggle";
            this.button_toggle.Size = new System.Drawing.Size(206, 68);
            this.button_toggle.TabIndex = 0;
            this.button_toggle.Text = "Start";
            this.button_toggle.UseVisualStyleBackColor = true;
            this.button_toggle.Click += new System.EventHandler(this.button_toggle_Click);
            // 
            // label_status
            // 
            this.label_status.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label_status.Location = new System.Drawing.Point(3, 74);
            this.label_status.Name = "label_status";
            this.label_status.Size = new System.Drawing.Size(206, 23);
            this.label_status.TabIndex = 1;
            this.label_status.Text = "Idle";
            this.label_status.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // progressBar_progress
            // 
            this.progressBar_progress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar_progress.Location = new System.Drawing.Point(3, 100);
            this.progressBar_progress.Name = "progressBar_progress";
            this.progressBar_progress.Size = new System.Drawing.Size(206, 24);
            this.progressBar_progress.TabIndex = 2;
            // 
            // ProcessToggleView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.progressBar_progress);
            this.Controls.Add(this.label_status);
            this.Controls.Add(this.button_toggle);
            this.Name = "ProcessToggleView";
            this.Size = new System.Drawing.Size(212, 127);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_toggle;
        private System.Windows.Forms.Label label_status;
        private System.Windows.Forms.ProgressBar progressBar_progress;
    }
}
