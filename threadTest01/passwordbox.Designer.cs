namespace threadTest01
{
    partial class passwordbox
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
            this.lb_pwerror = new System.Windows.Forms.Label();
            this.tb_password = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lb_pwerror
            // 
            this.lb_pwerror.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_pwerror.ForeColor = System.Drawing.Color.Blue;
            this.lb_pwerror.Location = new System.Drawing.Point(22, 98);
            this.lb_pwerror.Name = "lb_pwerror";
            this.lb_pwerror.Size = new System.Drawing.Size(282, 48);
            this.lb_pwerror.TabIndex = 7;
            this.lb_pwerror.Text = "Error : Please Check Fail data";
            this.lb_pwerror.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tb_password
            // 
            this.tb_password.Location = new System.Drawing.Point(26, 74);
            this.tb_password.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tb_password.Name = "tb_password";
            this.tb_password.Size = new System.Drawing.Size(278, 21);
            this.tb_password.TabIndex = 6;
            this.tb_password.UseSystemPasswordChar = true;
            this.tb_password.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tb_password_KeyDown);
            this.tb_password.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tb_password_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 22.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(126, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 41);
            this.label1.TabIndex = 5;
            this.label1.Text = "FAIL";
            // 
            // passwordbox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(327, 164);
            this.Controls.Add(this.lb_pwerror);
            this.Controls.Add(this.tb_password);
            this.Controls.Add(this.label1);
            this.Name = "passwordbox";
            this.Text = "passwordbox";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lb_pwerror;
        private System.Windows.Forms.TextBox tb_password;
        private System.Windows.Forms.Label label1;
    }
}