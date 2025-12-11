using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using EKANBAN; // เพิ่ม using สำหรับ IniFile

namespace DCIWashing
{
    public partial class FrmLogin : Form
    {
        private SqlConnectDB dbSCM = new SqlConnectDB("dbSCM");
        private IniFile ini = new IniFile("Config.ini");
        public string LoggedInUserName { get; private set; } = "";
        public bool IsLoginSuccessful { get; private set; } = false;
        public string SelectedScanMode { get; private set; } = "DWG"; // เพิ่มตัวแปรเก็บ SCAN MODE ที่เลือก

        public FrmLogin()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.btnLogin = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblScanMode = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.rbDWG = new System.Windows.Forms.RadioButton();
            this.rbMODEL = new System.Windows.Forms.RadioButton();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.lblTitle.Location = new System.Drawing.Point(160, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(117, 29);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "เข้าสู่ระบบ";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPassword
            // 
            this.lblPassword.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblPassword.AutoSize = true;
            this.lblPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.lblPassword.Location = new System.Drawing.Point(81, 15);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(66, 20);
            this.lblPassword.TabIndex = 1;
            this.lblPassword.Text = "รหัสผ่าน :";
            // 
            // txtPassword
            // 
            this.txtPassword.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.txtPassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.txtPassword.Location = new System.Drawing.Point(153, 10);
            this.txtPassword.MaxLength = 5;
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(150, 29);
            this.txtPassword.TabIndex = 2;
            this.txtPassword.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPassword_KeyDown);
            // 
            // btnLogin
            // 
            this.btnLogin.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnLogin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(173)))), ((int)(((byte)(130)))));
            this.btnLogin.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnLogin.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.btnLogin.ForeColor = System.Drawing.Color.White;
            this.btnLogin.Location = new System.Drawing.Point(25, 3);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(100, 35);
            this.btnLogin.TabIndex = 3;
            this.btnLogin.Text = "เข้าสู่ระบบ";
            this.btnLogin.UseVisualStyleBackColor = false;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(89)))), ((int)(((byte)(93)))));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(175, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 35);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "ยกเลิก";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel1.Controls.Add(this.lblPassword, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtPassword, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblScanMode, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 1, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(52, 67);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(375, 100);
            this.tableLayoutPanel1.TabIndex = 5;
            // 
            // lblScanMode
            // 
            this.lblScanMode.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblScanMode.AutoSize = true;
            this.lblScanMode.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.lblScanMode.Location = new System.Drawing.Point(34, 65);
            this.lblScanMode.Name = "lblScanMode";
            this.lblScanMode.Size = new System.Drawing.Size(113, 20);
            this.lblScanMode.TabIndex = 7;
            this.lblScanMode.Text = "SCAN MODE :";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this.rbDWG, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.rbMODEL, 1, 0);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(153, 53);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(205, 44);
            this.tableLayoutPanel3.TabIndex = 10;
            // 
            // rbDWG
            // 
            this.rbDWG.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.rbDWG.AutoSize = true;
            this.rbDWG.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.rbDWG.Location = new System.Drawing.Point(3, 10);
            this.rbDWG.Name = "rbDWG";
            this.rbDWG.Size = new System.Drawing.Size(67, 24);
            this.rbDWG.TabIndex = 8;
            this.rbDWG.TabStop = true;
            this.rbDWG.Text = "DWG";
            this.rbDWG.UseVisualStyleBackColor = true;
            // 
            // rbMODEL
            // 
            this.rbMODEL.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.rbMODEL.AutoSize = true;
            this.rbMODEL.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.rbMODEL.Location = new System.Drawing.Point(105, 10);
            this.rbMODEL.Name = "rbMODEL";
            this.rbMODEL.Size = new System.Drawing.Size(84, 24);
            this.rbMODEL.TabIndex = 9;
            this.rbMODEL.TabStop = true;
            this.rbMODEL.Text = "MODEL";
            this.rbMODEL.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.btnLogin, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnCancel, 1, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(83, 183);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(300, 41);
            this.tableLayoutPanel2.TabIndex = 6;
            // 
            // FrmLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(474, 241);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "เข้าสู่ระบบ - ระบบเครื่องล้าง";
            this.Load += new System.EventHandler(this.FrmLogin_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void FrmLogin_Load(object sender, EventArgs e)
        {
            // Focus ไปที่ช่องรหัสผ่าน
            txtPassword.Focus();
            
            // โหลดค่า SCAN_MODE จาก Config.ini
            LoadScanModeFromConfig();
        }

        private void LoadScanModeFromConfig()
        {
            try
            {
                string scanMode = ini.GetString("CONFIG", "SCAN_MODE", "DWG");
                
                if (scanMode == "MODEL")
                {
                    rbMODEL.Checked = true;
                    rbDWG.Checked = false;
                }
                else
                {
                    rbDWG.Checked = true;
                    rbMODEL.Checked = false;
                }
            }
            catch (Exception ex)
            {
                // หากอ่านไฟล์ไม่ได้ ให้ใช้ค่าเริ่มต้นเป็น DWG
                rbDWG.Checked = true;
                rbMODEL.Checked = false;
            }
        }

        private void SaveScanModeToConfig()
        {
            try
            {
                string scanMode = rbDWG.Checked ? "DWG" : "MODEL";
                ini.WriteValue("CONFIG", "SCAN_MODE", scanMode);
                SelectedScanMode = scanMode;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ไม่สามารถบันทึกการตั้งค่าได้: {ex.Message}", 
                               "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            // กด Enter เท่ากับกดปุ่มเข้าสู่ระบบ
            if (e.KeyCode == Keys.Enter)
            {
                btnLogin_Click(sender, e);
                e.Handled = true;
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string password = txtPassword.Text.Trim();

            // ตรวจสอบว่ากรอกรหัสผ่านหรือไม่
            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("กรุณากรอกรหัสผ่าน", "ข้อมูลไม่ครบถ้วน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            // ตรวจสอบว่ารหัสผ่านเป็นตัวเลข 5 หลักหรือไม่
            if (password.Length != 5 || !System.Text.RegularExpressions.Regex.IsMatch(password, @"^\d{5}$"))
            {
                MessageBox.Show("รหัสผ่านต้องเป็นตัวเลข 5 หลัก", "รหัสผ่านไม่ถูกต้อง", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.SelectAll();
                txtPassword.Focus();
                return;
            }

            // ตรวจสอบว่าเลือก SCAN MODE หรือไม่
            if (!rbDWG.Checked && !rbMODEL.Checked)
            {
                MessageBox.Show("กรุณาเลือก SCAN MODE", "ข้อมูลไม่ครบถ้วน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // เช็คการเข้าสู่ระบบกับฐานข้อมูล
            if (CheckLogin(password))
            {
                // บันทึกการตั้งค่า SCAN MODE ลง Config.ini
                SaveScanModeToConfig();
                
                IsLoginSuccessful = true;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("รหัสผ่านไม่ถูกต้อง กรุณาลองใหม่", "เข้าสู่ระบบไม่สำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPassword.SelectAll();
                txtPassword.Focus();
            }
        }

        private bool CheckLogin(string password)
        {
            try
            {
                string sql = $"exec sp_aps_washing 'login_washing', '{password}'";
                SqlCommand command = new SqlCommand(sql);
                DataTable dtResult = dbSCM.Query(command);

                if (dtResult != null && dtResult.Rows.Count > 0)
                {
                    // ดึงชื่อผู้ใช้จากผลลัพธ์
                    LoggedInUserName = dtResult.Rows[0]["name"].ToString();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"เกิดข้อผิดพลาดในการเข้าสู่ระบบ: {ex.Message}", 
                               "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            IsLoginSuccessful = false;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label lblScanMode;
        private System.Windows.Forms.RadioButton rbDWG;
        private System.Windows.Forms.RadioButton rbMODEL;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
    }
}