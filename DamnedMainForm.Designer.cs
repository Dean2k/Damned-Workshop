namespace DamnedWorkshop
{
    partial class DamnedMainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DamnedMainForm));
            this.damnedWelcomeTextbox = new System.Windows.Forms.RichTextBox();
            this.buttonPatcherForm = new System.Windows.Forms.Button();
            this.buttonMappingForm = new System.Windows.Forms.Button();
            this.labelDamnedDirectory = new System.Windows.Forms.Label();
            this.browseStagesButton = new System.Windows.Forms.Button();
            this.toolTipSelectDamnedDirectory = new System.Windows.Forms.ToolTip(this.components);
            this.toolTipSelectPatchingTools = new System.Windows.Forms.ToolTip(this.components);
            this.toolTipMappingTools = new System.Windows.Forms.ToolTip(this.components);
            this.toolTipBrowseCommunityStages = new System.Windows.Forms.ToolTip(this.components);
            this.toolTipCheck = new System.Windows.Forms.ToolTip(this.components);
            this.btnCheck = new System.Windows.Forms.Button();
            this.gbDamnedInformation = new System.Windows.Forms.GroupBox();
            this.lblDamnedValid = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtInstallLocation = new System.Windows.Forms.TextBox();
            this.gbDamnedInformation.SuspendLayout();
            this.SuspendLayout();
            // 
            // damnedWelcomeTextbox
            // 
            this.damnedWelcomeTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.damnedWelcomeTextbox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(66)))), ((int)(((byte)(66)))));
            this.damnedWelcomeTextbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.damnedWelcomeTextbox.Cursor = System.Windows.Forms.Cursors.Default;
            this.damnedWelcomeTextbox.Font = new System.Drawing.Font("Romance Fatal Serif Std", 14.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.damnedWelcomeTextbox.ForeColor = System.Drawing.Color.White;
            this.damnedWelcomeTextbox.Location = new System.Drawing.Point(10, 240);
            this.damnedWelcomeTextbox.Name = "damnedWelcomeTextbox";
            this.damnedWelcomeTextbox.ReadOnly = true;
            this.damnedWelcomeTextbox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.damnedWelcomeTextbox.Size = new System.Drawing.Size(838, 114);
            this.damnedWelcomeTextbox.TabIndex = 0;
            this.damnedWelcomeTextbox.Text = resources.GetString("damnedWelcomeTextbox.Text");
            this.damnedWelcomeTextbox.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.DamnedWelcomeTextbox_LinkClicked);
            // 
            // buttonPatcherForm
            // 
            this.buttonPatcherForm.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(66)))), ((int)(((byte)(66)))));
            this.buttonPatcherForm.Enabled = false;
            this.buttonPatcherForm.FlatAppearance.BorderSize = 0;
            this.buttonPatcherForm.Font = new System.Drawing.Font("Romance Fatal Serif Std", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonPatcherForm.ForeColor = System.Drawing.Color.Black;
            this.buttonPatcherForm.Location = new System.Drawing.Point(9, 55);
            this.buttonPatcherForm.Name = "buttonPatcherForm";
            this.buttonPatcherForm.Size = new System.Drawing.Size(201, 36);
            this.buttonPatcherForm.TabIndex = 1;
            this.buttonPatcherForm.Text = "Patching Tools";
            this.buttonPatcherForm.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolTipSelectPatchingTools.SetToolTip(this.buttonPatcherForm, resources.GetString("buttonPatcherForm.ToolTip"));
            this.buttonPatcherForm.UseVisualStyleBackColor = true;
            this.buttonPatcherForm.Click += new System.EventHandler(this.ButtonPatcherForm_Click);
            // 
            // buttonMappingForm
            // 
            this.buttonMappingForm.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(66)))), ((int)(((byte)(66)))));
            this.buttonMappingForm.Enabled = false;
            this.buttonMappingForm.FlatAppearance.BorderSize = 0;
            this.buttonMappingForm.Font = new System.Drawing.Font("Romance Fatal Serif Std", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonMappingForm.ForeColor = System.Drawing.Color.Black;
            this.buttonMappingForm.Location = new System.Drawing.Point(9, 115);
            this.buttonMappingForm.Name = "buttonMappingForm";
            this.buttonMappingForm.Size = new System.Drawing.Size(201, 36);
            this.buttonMappingForm.TabIndex = 2;
            this.buttonMappingForm.Text = "Mapping Tools";
            this.buttonMappingForm.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolTipMappingTools.SetToolTip(this.buttonMappingForm, resources.GetString("buttonMappingForm.ToolTip"));
            this.buttonMappingForm.UseVisualStyleBackColor = true;
            this.buttonMappingForm.Click += new System.EventHandler(this.ButtonMappingForm_Click);
            // 
            // labelDamnedDirectory
            // 
            this.labelDamnedDirectory.AutoSize = true;
            this.labelDamnedDirectory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelDamnedDirectory.Font = new System.Drawing.Font("Romance Fatal Serif Std", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDamnedDirectory.ForeColor = System.Drawing.Color.Black;
            this.labelDamnedDirectory.Location = new System.Drawing.Point(5, 16);
            this.labelDamnedDirectory.Name = "labelDamnedDirectory";
            this.labelDamnedDirectory.Size = new System.Drawing.Size(155, 21);
            this.labelDamnedDirectory.TabIndex = 4;
            this.labelDamnedDirectory.Text = "Damned Directory:";
            this.labelDamnedDirectory.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // browseStagesButton
            // 
            this.browseStagesButton.Enabled = false;
            this.browseStagesButton.FlatAppearance.BorderSize = 0;
            this.browseStagesButton.Font = new System.Drawing.Font("Romance Fatal Serif Std", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.browseStagesButton.ForeColor = System.Drawing.Color.Black;
            this.browseStagesButton.Location = new System.Drawing.Point(9, 175);
            this.browseStagesButton.Name = "browseStagesButton";
            this.browseStagesButton.Size = new System.Drawing.Size(201, 36);
            this.browseStagesButton.TabIndex = 7;
            this.browseStagesButton.Text = "Browse Community Stages...";
            this.browseStagesButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolTipBrowseCommunityStages.SetToolTip(this.browseStagesButton, "Opens up a new window where you can install new stages into Damned from the commu" +
        "nity repository.");
            this.browseStagesButton.UseVisualStyleBackColor = true;
            this.browseStagesButton.Click += new System.EventHandler(this.BrowseStagesButton_Click);
            // 
            // btnCheck
            // 
            this.btnCheck.Font = new System.Drawing.Font("Romance Fatal Serif Std", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCheck.Location = new System.Drawing.Point(751, 175);
            this.btnCheck.Name = "btnCheck";
            this.btnCheck.Size = new System.Drawing.Size(74, 36);
            this.btnCheck.TabIndex = 7;
            this.btnCheck.Text = "Check";
            this.toolTipCheck.SetToolTip(this.btnCheck, "Checks the selected path to make sure that it is a valid Damned directory.");
            this.btnCheck.UseVisualStyleBackColor = true;
            this.btnCheck.Click += new System.EventHandler(this.btnCheck_Click);
            // 
            // gbDamnedInformation
            // 
            this.gbDamnedInformation.Controls.Add(this.lblDamnedValid);
            this.gbDamnedInformation.Controls.Add(this.btnCheck);
            this.gbDamnedInformation.Controls.Add(this.browseStagesButton);
            this.gbDamnedInformation.Controls.Add(this.btnBrowse);
            this.gbDamnedInformation.Controls.Add(this.buttonMappingForm);
            this.gbDamnedInformation.Controls.Add(this.txtInstallLocation);
            this.gbDamnedInformation.Controls.Add(this.buttonPatcherForm);
            this.gbDamnedInformation.Controls.Add(this.labelDamnedDirectory);
            this.gbDamnedInformation.Location = new System.Drawing.Point(10, 12);
            this.gbDamnedInformation.Name = "gbDamnedInformation";
            this.gbDamnedInformation.Size = new System.Drawing.Size(839, 222);
            this.gbDamnedInformation.TabIndex = 8;
            this.gbDamnedInformation.TabStop = false;
            // 
            // lblDamnedValid
            // 
            this.lblDamnedValid.Font = new System.Drawing.Font("Romance Fatal Serif Std", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDamnedValid.Location = new System.Drawing.Point(216, 183);
            this.lblDamnedValid.Name = "lblDamnedValid";
            this.lblDamnedValid.Size = new System.Drawing.Size(529, 21);
            this.lblDamnedValid.TabIndex = 8;
            this.lblDamnedValid.Text = "LABEL TO STATE DAMNED LOCATION VALID OR NOT";
            this.lblDamnedValid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblDamnedValid.Visible = false;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Font = new System.Drawing.Font("Romance Fatal Serif Std", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBrowse.Location = new System.Drawing.Point(687, 55);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(138, 36);
            this.btnBrowse.TabIndex = 6;
            this.btnBrowse.Text = "Select Damned";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtInstallLocation
            // 
            this.txtInstallLocation.Font = new System.Drawing.Font("Romance Fatal Serif Std", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtInstallLocation.Location = new System.Drawing.Point(239, 15);
            this.txtInstallLocation.Name = "txtInstallLocation";
            this.txtInstallLocation.Size = new System.Drawing.Size(587, 29);
            this.txtInstallLocation.TabIndex = 5;
            this.txtInstallLocation.TextChanged += new System.EventHandler(this.txtInstallLocation_TextChanged);
            // 
            // DamnedMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(859, 366);
            this.Controls.Add(this.gbDamnedInformation);
            this.Controls.Add(this.damnedWelcomeTextbox);
            this.Font = new System.Drawing.Font("Romance Fatal Serif Std", 8.249999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Black;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DamnedMainForm";
            this.Text = "Damned Workshop";
            this.Load += new System.EventHandler(this.DamnedMainForm_Load);
            this.gbDamnedInformation.ResumeLayout(false);
            this.gbDamnedInformation.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox damnedWelcomeTextbox;
        private System.Windows.Forms.Button buttonPatcherForm;
        private System.Windows.Forms.Button buttonMappingForm;
        private System.Windows.Forms.Label labelDamnedDirectory;
        private System.Windows.Forms.Button browseStagesButton;
        private System.Windows.Forms.ToolTip toolTipSelectDamnedDirectory;
        private System.Windows.Forms.ToolTip toolTipSelectPatchingTools;
        private System.Windows.Forms.ToolTip toolTipMappingTools;
        private System.Windows.Forms.ToolTip toolTipBrowseCommunityStages;
        private System.Windows.Forms.ToolTip toolTipCheck;
        private System.Windows.Forms.GroupBox gbDamnedInformation;
        private System.Windows.Forms.TextBox txtInstallLocation;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnCheck;
        private System.Windows.Forms.Label lblDamnedValid;
    }
}