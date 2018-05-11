namespace NimbusFox.FoxCore.Forms {
    partial class VariantLoader {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.prgrssMods = new System.Windows.Forms.ProgressBar();
            this.lblMod = new System.Windows.Forms.Label();
            this.lblTileItem = new System.Windows.Forms.Label();
            this.prgrssTileItems = new System.Windows.Forms.ProgressBar();
            this.lblPalette = new System.Windows.Forms.Label();
            this.prgrssPalettes = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // prgrssMods
            // 
            this.prgrssMods.Location = new System.Drawing.Point(12, 29);
            this.prgrssMods.Name = "prgrssMods";
            this.prgrssMods.Size = new System.Drawing.Size(376, 23);
            this.prgrssMods.TabIndex = 0;
            // 
            // lblMod
            // 
            this.lblMod.AutoSize = true;
            this.lblMod.Location = new System.Drawing.Point(13, 13);
            this.lblMod.Name = "lblMod";
            this.lblMod.Size = new System.Drawing.Size(64, 13);
            this.lblMod.TabIndex = 1;
            this.lblMod.Text = "Mod (0 of 0)";
            // 
            // lblTileItem
            // 
            this.lblTileItem.AutoSize = true;
            this.lblTileItem.Location = new System.Drawing.Point(12, 73);
            this.lblTileItem.Name = "lblTileItem";
            this.lblTileItem.Size = new System.Drawing.Size(85, 13);
            this.lblTileItem.TabIndex = 3;
            this.lblTileItem.Text = "Tile/Item (0 of 0)";
            // 
            // prgrssTileItems
            // 
            this.prgrssTileItems.Location = new System.Drawing.Point(12, 89);
            this.prgrssTileItems.Name = "prgrssTileItems";
            this.prgrssTileItems.Size = new System.Drawing.Size(376, 23);
            this.prgrssTileItems.TabIndex = 2;
            // 
            // lblPalette
            // 
            this.lblPalette.AutoSize = true;
            this.lblPalette.Location = new System.Drawing.Point(13, 131);
            this.lblPalette.Name = "lblPalette";
            this.lblPalette.Size = new System.Drawing.Size(76, 13);
            this.lblPalette.TabIndex = 5;
            this.lblPalette.Text = "Palette (0 of 0)";
            // 
            // prgrssPalettes
            // 
            this.prgrssPalettes.Location = new System.Drawing.Point(12, 147);
            this.prgrssPalettes.Name = "prgrssPalettes";
            this.prgrssPalettes.Size = new System.Drawing.Size(376, 23);
            this.prgrssPalettes.TabIndex = 4;
            // 
            // VariantLoader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(396, 178);
            this.ControlBox = false;
            this.Controls.Add(this.lblPalette);
            this.Controls.Add(this.prgrssPalettes);
            this.Controls.Add(this.lblTileItem);
            this.Controls.Add(this.prgrssTileItems);
            this.Controls.Add(this.lblMod);
            this.Controls.Add(this.prgrssMods);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "VariantLoader";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Fox Core Variant Loader";
            this.Shown += new System.EventHandler(this.VariantLoader_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar prgrssMods;
        private System.Windows.Forms.Label lblMod;
        private System.Windows.Forms.Label lblTileItem;
        private System.Windows.Forms.ProgressBar prgrssTileItems;
        private System.Windows.Forms.Label lblPalette;
        private System.Windows.Forms.ProgressBar prgrssPalettes;
    }
}