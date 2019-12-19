namespace winformhuffman
{
    partial class Form1
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxCHEMIN = new System.Windows.Forms.TextBox();
            this.buttonCHEMIN = new System.Windows.Forms.Button();
            this.buttonENREIGISTRER = new System.Windows.Forms.Button();
            this.buttonDECOMPRESSER = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(144, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(217, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "SELECTIONNER LE FICHIER :";
            // 
            // textBoxCHEMIN
            // 
            this.textBoxCHEMIN.Location = new System.Drawing.Point(12, 55);
            this.textBoxCHEMIN.Name = "textBoxCHEMIN";
            this.textBoxCHEMIN.Size = new System.Drawing.Size(452, 20);
            this.textBoxCHEMIN.TabIndex = 1;
            // 
            // buttonCHEMIN
            // 
            this.buttonCHEMIN.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCHEMIN.Location = new System.Drawing.Point(497, 52);
            this.buttonCHEMIN.Name = "buttonCHEMIN";
            this.buttonCHEMIN.Size = new System.Drawing.Size(33, 23);
            this.buttonCHEMIN.TabIndex = 2;
            this.buttonCHEMIN.Text = "...";
            this.buttonCHEMIN.UseVisualStyleBackColor = true;
            this.buttonCHEMIN.Click += new System.EventHandler(this.buttonCHEMIN_Click);
            // 
            // buttonENREIGISTRER
            // 
            this.buttonENREIGISTRER.AutoSize = true;
            this.buttonENREIGISTRER.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonENREIGISTRER.Location = new System.Drawing.Point(12, 105);
            this.buttonENREIGISTRER.Name = "buttonENREIGISTRER";
            this.buttonENREIGISTRER.Size = new System.Drawing.Size(131, 26);
            this.buttonENREIGISTRER.TabIndex = 3;
            this.buttonENREIGISTRER.Text = "COMPRESSER";
            this.buttonENREIGISTRER.UseVisualStyleBackColor = true;
            this.buttonENREIGISTRER.Click += new System.EventHandler(this.buttonENREIGISTRER_Click);
            // 
            // buttonDECOMPRESSER
            // 
            this.buttonDECOMPRESSER.AutoSize = true;
            this.buttonDECOMPRESSER.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonDECOMPRESSER.Location = new System.Drawing.Point(320, 105);
            this.buttonDECOMPRESSER.Name = "buttonDECOMPRESSER";
            this.buttonDECOMPRESSER.Size = new System.Drawing.Size(144, 26);
            this.buttonDECOMPRESSER.TabIndex = 4;
            this.buttonDECOMPRESSER.Text = "DECOMPRESSER";
            this.buttonDECOMPRESSER.UseVisualStyleBackColor = true;
            this.buttonDECOMPRESSER.Click += new System.EventHandler(this.buttonDECOMPRESSER_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(558, 262);
            this.Controls.Add(this.buttonDECOMPRESSER);
            this.Controls.Add(this.buttonENREIGISTRER);
            this.Controls.Add(this.buttonCHEMIN);
            this.Controls.Add(this.textBoxCHEMIN);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxCHEMIN;
        private System.Windows.Forms.Button buttonCHEMIN;
        private System.Windows.Forms.Button buttonENREIGISTRER;
        private System.Windows.Forms.Button buttonDECOMPRESSER;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}

