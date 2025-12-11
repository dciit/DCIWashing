using DCIWashing.Props;
using EKANBAN;
using EKBPartStock;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace DCIWashing
{
    public partial class FrmMain : Form
    {
        SqlConnectDB dbSCM = new SqlConnectDB("dbSCM");
        private IniFile ini = new IniFile("Config.ini");
        private string ConfWCNO = "";
        private string ConfShift = "";
        private string MethodStart = "";
        private string ConfScanBy = "";
        private string Mode = "";
        private string filter_ymd = DateTime.Now.AddHours(-8).ToString("yyyyMMdd");
        private string YmdNow = DateTime.Now.AddHours(-8).ToString("yyyyMMdd");
        private Services oServ = new Services();
        private List<PropTransaction> dtTransaction = new List<PropTransaction>();
        private DataTable dtWIPs = new DataTable();
        private DataTable dtSummaryOfDay = new DataTable();
        private List<PropRMInfo> ListScanInfo = new List<PropRMInfo>();

        // เพิ่มตัวแปรเพื่อติดตามโหมดปัจจุบัน
        private string CurrentScanMode = "IN"; // Default เป็น "IN" (สแกนรับ)
        private string LoggedInUserName = ""; // เก็บชื่อผู้ใช้ที่เข้าสู่ระบบ
        private DateTime LastActivityTime = DateTime.Now; // เก็บเวลาการใช้งานล่าสุด
        private Timer sessionTimer; // Timer สำหรับตรวจสอบ session timeout

        // Constructor ใหม่ที่รับชื่อผู้ใช้
        public FrmMain(string userName)
        {
            InitializeComponent();
            LoggedInUserName = userName;

            InitializeSessionTimer();
        }

        public FrmMain()
        {
            InitializeComponent();
            InitializeSessionTimer();
        }

        private void InitializeSessionTimer()
        {
            // ตั้งค่า Timer สำหรับตรวจสอบ session timeout (30 นาที)
            sessionTimer = new Timer();
            sessionTimer.Interval = 60000; // ตรวจสอบทุก 1 นาที
            sessionTimer.Tick += SessionTimer_Tick;
            sessionTimer.Start();

            // อัพเดต LastActivityTime เมื่อมีการใช้งาน
            this.MouseMove += (s, e) => UpdateLastActivity();
            this.KeyDown += (s, e) => UpdateLastActivity();
        }

        private void SessionTimer_Tick(object sender, EventArgs e)
        {
            // ตรวจสอบว่าไม่มีการใช้งานเกิน 30 นาทีหรือไม่
            if (DateTime.Now.Subtract(LastActivityTime).TotalMinutes > 30)
            {
                sessionTimer.Stop();
                MessageBox.Show("Session หมดอายุ กรุณาเข้าสู่ระบบใหม่", "Session Timeout",
                               MessageBoxButtons.OK, MessageBoxIcon.Information);
                BtnLogout_Click(sender, e);
            }
        }

        private void UpdateLastActivity()
        {
            LastActivityTime = DateTime.Now;
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("ออกจากระบบ ใช่หรือไม่?", "ยืนยันการออก", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                // ซ่อน Main Form
                this.Hide();

                // แสดง Login Dialog อีกครั้ง
                FrmLogin loginForm = new FrmLogin();
                DialogResult loginResult = loginForm.ShowDialog();

                if (loginResult == DialogResult.OK && loginForm.IsLoginSuccessful)
                {
                    // ตรวจสอบ SCAN MODE ที่เลือกและเปิดฟอร์มที่เหมาะสม
                    if (loginForm.SelectedScanMode == "MODEL")
                    {
                        // ปิด FrmMain และเปิด FrmScanByModel
                        this.Close();
                        FrmScanByModel modelForm = new FrmScanByModel(loginForm.LoggedInUserName);
                        modelForm.Show();
                    }
                    else
                    {
                        // ถ้าเลือก DWG ให้อัพเดตข้อมูลผู้ใช้และแสดง Main Form อีกครั้ง
                        LoggedInUserName = loginForm.LoggedInUserName;
                        TxtEmpcode.Text = LoggedInUserName;

                        // ล้างข้อมูลเดิมในระบบ
                        gv_list_scan.Rows.Clear();
                        LoadLogOfDay();
                        LoadWIP();
                        LoadAccu();

                        this.Show();
                    }
                }
                else
                {
                    // ถ้า Login ไม่สำเร็จ ให้ออกจากโปรแกรม
                    Application.Exit();
                }
            }
        }


        internal DataTable GetRMInfo(string scanValue)
        {
            string str = $@"exec sp_aps_washing 'rm_qr_washing_info' ,'{scanValue}'";
            SqlCommand sql = new SqlCommand();
            sql.CommandText = str;
            DataTable dtRMInfo = dbSCM.Query(sql);
            if (dtRMInfo != null && dtRMInfo.Rows.Count > 0)
            {
                string case_of_part = dtRMInfo.Rows[0]["case"].ToString();
                if (case_of_part == "NORMAL")
                {
                    return dtRMInfo;
                }
                else
                {
                    MessageBox.Show("ไม่สามารถสแกนงานได้ เนื่องจาก " + dtRMInfo.Rows[0]["message"].ToString());
                    return null;
                }
            }
            else
            {
                MessageBox.Show("ไม่สามารถสแกนงานได้ เนื่องจากข้อมูล Master ไม่ครบถ้วน ติดต่อทีมงาน IT (250) เบียร์");
                return null;
            }
        }

        private void PnToolbar_Paint(object sender, PaintEventArgs e)
        {
        }

        // ปรับปรุง helper method สำหรับ multiline wrapping
        private void EnableMultiLineWrap(DataGridView dgv, params string[] columnNames)
        {
            if (dgv == null) return;

            // ตั้งค่าพื้นฐานของ DataGridView
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.False;

            foreach (var columnName in columnNames)
            {
                if (dgv.Columns.Contains(columnName))
                {
                    var column = dgv.Columns[columnName];

                    // ตั้งค่า text wrapping
                    column.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

                    // ตั้งค่าการจัดตำแหน่งข้อความ
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopLeft;

                    // ห้ามให้คอลัมน์ปรับขนาดอัตโนมัติตามเนื้อหา
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;

                    // ถ้าคอลัมน์ไม่มี width ที่กำหนดไว้ ให้ตั้งค่าเริ่มต้น
                    if (column.Width < 50)
                    {
                        column.Width = 150; // กำหนดความกว้างเริ่มต้น
                    }
                }
            }

            // บังคับให้ DataGridView อัปเดตการแสดงผล
            dgv.Invalidate();
        }

        // เพิ่ม method สำหรับตั้งค่า DataGridView ให้รองรับ multiline
        private void SetupDataGridViewForMultiLine(DataGridView dgv)
        {
            if (dgv == null) return;

            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            dgv.AllowUserToResizeRows = true;

            // ตั้งค่าให้แสดงผลได้อย่างถูกต้อง
            dgv.RowTemplate.Resizable = DataGridViewTriState.True;
        }

        private void LoadWIP()
        {
            if (gv_wip.DataSource != null)
            {
                gv_wip.DataSource = null;   // ตัดการ bind
            }
            string ym = filter_ymd.Substring(0, 6);
            string wcno = "80" + ConfWCNO[ConfWCNO.Length - 1];
            string strGetWIP = $@"exec sp_aps_washing 'get_wip','{ym}','{wcno}'";
            DataTable dtGetWIP = dbSCM.Query(strGetWIP);
            if (dtGetWIP.Rows.Count > 0)
            {
                gv_wip.AutoGenerateColumns = false;
                gv_wip.DataSource = dtGetWIP;

                gv_wip.Columns["col_lbal_gv_wip"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                gv_wip.Columns["col_lbal_gv_wip"].DefaultCellStyle.Format = "N2";
                gv_wip.Columns["col_rec_gv_wip"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                gv_wip.Columns["col_rec_gv_wip"].DefaultCellStyle.Format = "N2";
                gv_wip.Columns["col_issue_gv_wip"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                gv_wip.Columns["col_issue_gv_wip"].DefaultCellStyle.Format = "N2";
                gv_wip.Columns["col_bal_gv_wip"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                gv_wip.Columns["col_bal_gv_wip"].DefaultCellStyle.Format = "N2";

                // เปิดใช้งาน text wrapping สำหรับคอลัมน์ที่มีข้อความยาว
                EnableMultiLineWrap(gv_wip, "col_model_gv_wip");

                // บังคับให้ DataGridView คำนวณขนาดแถวใหม่
                gv_wip.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);
            }
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            ConfScanBy = ini.GetString("CONFIG", "SCAN_MODE", "");
            ConfShift = ini.GetString("CONFIG", "SHIFT", "");
            // ใช้เวลาปัจจุบันของเครื่อง
            string shift = (DateTime.Now.TimeOfDay >= TimeSpan.FromHours(8) &&
                            DateTime.Now.TimeOfDay < TimeSpan.FromHours(20))
                           ? "D" : "N";
            if (ConfShift != shift)
            {
                ini.WriteValue("CONFIG", "SHIFT", shift);
                ConfShift = ini.GetString("CONFIG", "SHIFT", "");
            }

            cb_filter_shift.SelectedItem = shift;
            // 1) ให้ tableLayoutPanel1 กินเต็มฟอร์มแบบสัดส่วน ไม่ AutoSize ตามคอนเทนต์
            tableLayoutPanel1.AutoSize = false;
            tableLayoutPanel1.ColumnStyles.Clear();
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            tableLayoutPanel1.RowStyles.Clear();
            // แถวบน (toolbar) ให้ AutoSize ตามความสูงจริง
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            // แถวล่าง (content) เอาพื้นที่ที่เหลือทั้งหมด
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            tableLayoutPanel1.Dock = DockStyle.Fill;

            // 2) ปิด AutoSize ของกล่องย่อยที่ชอบขยายตัวเอง
            box_app_name.AutoSize = false;
            box_logout.AutoSize = false;

            // 3) ของใน tableLayoutPanel ให้ใช้ Dock = Fill เพื่อไม่ดันออกนอกเซลล์
            box_toolbar.Dock = DockStyle.Fill;
            box_content.Dock = DockStyle.Fill;

            // 4) ถ้าเคยกำหนด Size ตายตัวจากดีไซน์ไว้เยอะ ๆ ให้ลบอิทธิพลด้วย Minimum/Maximum
            box_toolbar.MinimumSize = Size.Empty;
            box_content.MinimumSize = Size.Empty;

            // 5) (แนะนำ) ปิด AutoSize ของคอนเทนเนอร์อื่น ๆ ที่ไม่จำเป็น
            box_wip.AutoSize = false;
            box_log_scan_of_day.AutoSize = false;

            ConfWCNO = ini.GetString("CONFIG", "WCNO", "");
            MethodStart = ini.GetString("CONFIG", "METHOD", "");
            Mode = ini.GetString("CONFIG", "MODE", "IN");
            LoadMode();
            LoadWCNO();
            // แสดงชื่อผู้ใช้ที่เข้าสู่ระบบ
            if (!string.IsNullOrEmpty(LoggedInUserName))
            {
                TxtEmpcode.Text = LoggedInUserName;
            }
            dpk_filter_ymd.Value = DateTime.Now.AddHours(-8);
            // ตั้งโหมดเริ่มต้นจากการตั้งค่า หรือ default เป็น "IN"
            CurrentScanMode = string.IsNullOrEmpty(MethodStart) ? "IN" : MethodStart;
            ViewMethod(CurrentScanMode);

            // ตั้งค่าให้ DataGridView รองรับการแสดงหลายบรรทัด
            SetupDataGridViewForMultiLine(gv_wip);
            SetupDataGridViewForMultiLine(gv_log_scan_of_day);
            SetupDataGridViewForMultiLine(gv_list_scan);

            // เพิ่ม event handler สำหรับการตั้งค่า wrapping หลังจาก data binding
            gv_wip.DataBindingComplete += (s, ev) =>
            {
                EnableMultiLineWrap(gv_wip, "col_model_gv_wip");
                gv_wip.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);
            };

            gv_log_scan_of_day.DataBindingComplete += (s, ev) =>
            {
                EnableMultiLineWrap(gv_log_scan_of_day, "col_partno_gv_log");
                gv_log_scan_of_day.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);
            };

            ConfWCNO = ini.GetString("CONFIG", "WCNO", "");
            MethodStart = ini.GetString("CONFIG", "METHOD", "");
            Mode = ini.GetString("CONFIG", "MODE", "IN");
            LoadMode();
            LoadWCNO();
            LoadAccu();
            // แสดงชื่อผู้ใช้ที่เข้าสู่ระบบ
            if (!string.IsNullOrEmpty(LoggedInUserName))
            {
                TxtEmpcode.Text = LoggedInUserName;
            }
            dpk_filter_ymd.Value = DateTime.Now.AddHours(-8);
            // ตั้งโหมดเริ่มต้นจากการตั้งค่า หรือ default เป็น "IN"
            CurrentScanMode = string.IsNullOrEmpty(MethodStart) ? "IN" : MethodStart;
            ViewMethod(CurrentScanMode);
        }

        private void LoadAccu()
        {
            string selectedDate = dpk_filter_ymd.Value.ToString("yyyyMMdd");
            string strGetAccu = $@"SELECT N.* FROM (SELECT CONCAT(A.PARTNO,CASE WHEN A.CM <> '' THEN ' (' + A.CM + ')' ELSE '' END) PARTNO,B.PART_GROUP,SUM(A.TransQty) ACCU,MAX(A.CreateDate) LAST_UPDATE FROM EKB_WIP_PART_STOCK_TRANSACTION A
LEFT JOIN (SELECT DISTINCT WCNO,PARTNO,PART_GROUP FROM APS_PART_MASTER WHERE WCNO = '" + ConfWCNO + "' AND PART_PROCESS = 'MACHINE' AND ACTIVE = 'ACTIVE') B ON B.PARTNO = A.PARTNO  WHERE A.YMD = '" + selectedDate + "'  AND A.REFNO LIKE '%WASHING%' AND B.WCNO = '" + ConfWCNO + "' GROUP BY CONCAT(A.PARTNO,CASE WHEN A.CM <> '' THEN ' (' + A.CM + ')' ELSE '' END) ,B.PART_GROUP) N ORDER BY N.LAST_UPDATE DESC";

            if (gv_summary_transtion.DataSource != null)
            {
                gv_summary_transtion.DataSource = null;   // ตัดการ bind
            }
            DataTable dtAccu = dbSCM.Query(strGetAccu);
            if (dtAccu.Rows.Count > 0)
            {
                gv_summary_transtion.AutoGenerateColumns = false;
                gv_summary_transtion.DataSource = dtAccu; 
            }
        }

        private void gv_log_scan_of_day_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // กันกรณีคลิก header
            if (e.RowIndex < 0) return;

            if (gv_list_scan.Columns[e.ColumnIndex].Name == "col_action_gv_input")
            {
                var confirm = MessageBox.Show("คุณต้องการลบแถวนี้หรือไม่?",
                                       "ยืนยันการลบ",
                                       MessageBoxButtons.YesNo,
                                       MessageBoxIcon.Question);
                if (confirm == DialogResult.Yes)
                {
                    gv_list_scan.Rows.RemoveAt(e.RowIndex);
                }
            }
            gv_list_scan.Refresh();
        }

        private void LoadMode()
        {
            //btnOut.Enabled = Mode == "OUT" ? true : false;
            //btnIn.Enabled = Mode == "IN" ? true : false;
            if (Mode == "IN")
            {
                btnOut.BackColor = Color.FromArgb(224, 224, 224);
                btnIn.BackColor = Color.LightGreen;
            }
            else
            { // MODE = "OUT"
                btnOut.BackColor = Color.FromArgb(255, 89, 93);
                btnIn.BackColor = Color.FromArgb(224, 224, 224);
            }
        }

        private void LoadWCNO()
        {
            try
            {
                cb_wcno.Items.Clear();
                string strGetWCNO = $@"exec sp_aps_washing 'master_wcno'";
                DataTable dtListWCNO = dbSCM.Query(strGetWCNO);
                if (dtListWCNO.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtListWCNO.Rows)
                    {
                        cb_wcno.Items.Add(dr["wcno"].ToString());
                    }
                    if (ConfWCNO != "")
                    {
                        cb_wcno.SelectedItem = ConfWCNO;
                    }
                }
                else
                {
                    MessageBox.Show("ไม่พบข้อมูลของ WCNO ! Notice = exec sp_aps_wasging 'master_wcno' not return");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void LoadLogOfDay()
        {
            if (gv_log_scan_of_day.DataSource != null)
            {
                gv_log_scan_of_day.DataSource = null;   // ตัดการ bind
            }
            ConfShift = ini.GetString("CONFIG", "SHIFT", "");
            string str = $@"select  YMD ,SHIFT,WCNO,PARTNO,CM,TransType trantype,SUM(TransQty) QTY,CreateBy col_create_by_gv_log,CreateDate col_datetime_gv_log from EKB_WIP_PART_STOCK_TRANSACTION 
where  RefNo = 'WASHING-SYSTEM' AND WCNO = '80" + ConfWCNO[ConfWCNO.Length - 1] + "' AND YMD = '" + filter_ymd + "' AND SHIFT = '" + ConfShift + "' GROUP BY YMD,SHIFT,WCNO,PARTNO,CM,TransType ,CreateBy ,CreateDate  ORDER BY CreateDate DESC";
            DataTable dtLogs = dbSCM.Query(str);
            if (dtLogs.Rows.Count > 0)
            {
                gv_log_scan_of_day.AutoGenerateColumns = false;
                gv_log_scan_of_day.DataSource = dtLogs;
                gv_log_scan_of_day.Columns["col_qty_gv_log"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                gv_log_scan_of_day.Columns["col_qty_gv_log"].DefaultCellStyle.Format = "N2";
                // เปิดใช้งาน text wrapping สำหรับคอลัมน์ PARTNO
                EnableMultiLineWrap(gv_log_scan_of_day, "col_partno_gv_log");

                // บังคับให้ DataGridView คำนวณขนาดแถวใหม่
                gv_log_scan_of_day.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);
            }
        }

        private void ViewMethod(string methodStart)
        {
            CurrentScanMode = methodStart;
        }


        private bool SaveToWIPDatabase(DataTable dtData)
        {
            try
            {
                bool allSuccess = true;

                // วนลูปบันทึกข้อมูลลงฐานข้อมูลแต่ละรายการ
                foreach (DataRow row in dtData.Rows)
                {
                    string wcno = row["WCNO"].ToString();
                    string partno = row["Partno"].ToString();
                    string cm = row["CM"].ToString();
                    int qty = Convert.ToInt32(row["Qty"]);
                    string type = row["Type"].ToString();
                    // เรียก stored procedure
                    string sql = $"exec sp_aps_washing 'insert_wip', '{wcno}', '{partno}', '{cm}', {qty},'{type}','{LoggedInUserName}'";
                    SqlCommand command = new SqlCommand(sql);
                    DataTable result = dbSCM.Query(command);

                    // ตรวจสอบผลลัพธ์
                    if (result != null && result.Rows.Count > 0)
                    {
                        int status = Convert.ToInt32(result.Rows[0]["status"]);
                        string message = result.Rows[0]["message"].ToString();

                        if (status == 0)
                        {
                            // บันทึกไม่สำเร็จ
                            string errorMsg = string.IsNullOrEmpty(message) ? "บันทึกข้อมูลไม่สำเร็จ" : message;
                            MessageBox.Show($"บันทึกรายการ {partno} ไม่สำเร็จ\n{errorMsg}",
                                           "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            allSuccess = false;
                            break; // หยุดการบันทึกทันที
                        }
                    }
                    else
                    {
                        MessageBox.Show($"ไม่สามารถบันทึกรายการ {partno} ได้ - ไม่ได้รับผลลัพธ์จากฐานข้อมูล",
                                       "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        allSuccess = false;
                        break;
                    }
                }

                return allSuccess;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"เกิดข้อผิดพลาดในการบันทึกฐานข้อมูล: {ex.Message}",
                               "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void tb_scan_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string inputText = tb_scan.Text.Trim(); 
                if (!string.IsNullOrEmpty(inputText))
                {
                    AddScanItemToGrid(inputText, CurrentScanMode);
                    tb_scan.Clear();
                }
                e.Handled = true;
            }
        }

        private void AddScanItemToGrid(string scanText, string mode)
        {
            try
            {
                string[] rDataOfQrCode = scanText.Split('|');
                if (rDataOfQrCode.Length == 0)
                {
                    MessageBox.Show("รูปแบบการสแกนไม่ถูกต้อง กรุณาตรวจสอบใหม่", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (rDataOfQrCode.Length >= 3 && rDataOfQrCode[2].ToString().Substring(rDataOfQrCode[2].ToString().Length - 1) != ConfWCNO.Substring(ConfWCNO.Length - 1))
                {
                    MessageBox.Show("QRCode มีพื้นที่ไม่ตรงกับที่ระบบเลือกพื้นที่ไว้", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                var rmInfo = GetRMInfo(scanText);
                if (rmInfo == null || rmInfo.Rows.Count == 0) return;
                string part_wcno = rmInfo.Rows[0]["PART_WCNO"].ToString();
                string partno = rmInfo.Rows[0]["PARTNO"].ToString();
                string part_group = rmInfo.Rows[0]["PART_GROUP"].ToString();
                string cm = rmInfo.Rows[0]["CM"].ToString();
                string qty = rmInfo.Columns.Contains("QTY") ? rmInfo.Rows[0]["QTY"].ToString() : "0";
                if (qty == "0")
                {
                    MessageBox.Show("ไม่สามารถเพิ่ม-ลด ได้ เนื่องจากไม่มี std : BASKET (จำนวนชิ้นในตะกร้า) ติดต่อ IT (250)", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                bool HasData = false;
                foreach (DataGridViewRow item in gv_list_scan.Rows)
                {
                    if (item.Cells["col_partno"].Value != null && item.Cells["col_partno"].Value.ToString() == partno)
                    {
                        int newVal = 0;
                        if (CurrentScanMode == "IN")
                        {
                            newVal = Convert.ToInt32(item.Cells["col_qty"].Value) + Convert.ToInt32(qty);
                            newVal = newVal >= 2000 ? 2000 : newVal;
                        }
                        else
                        {
                            newVal = Convert.ToInt32(item.Cells["col_qty"].Value) - Convert.ToInt32(qty);
                            newVal = newVal > 0 ? newVal : 0;
                        }
                        item.Cells["col_qty"].Value = newVal.ToString();
                        HasData = true;
                    }
                }
                if (!HasData)
                {
                    if (gv_list_scan.Columns["col_wcno"] == null)
                    {
                        gv_list_scan.Columns.Add(new DataGridViewTextBoxColumn()
                        {
                            Name = "col_wcno",
                            Visible = false
                        });
                    }
                    if (gv_list_scan.Columns["col_cm"] == null)
                    {
                        gv_list_scan.Columns.Add(new DataGridViewTextBoxColumn()
                        {
                            Name = "col_cm",
                            Visible = false
                        });
                    }

                    int rowIndex = gv_list_scan.Rows.Add();
                    var row = gv_list_scan.Rows[rowIndex];
                    row.Cells["col_type"].Value = mode;
                    row.Cells["col_part_group"].Value = part_group;
                    row.Cells["col_partno"].Value = partno;
                    row.Cells["col_qty"].Value = qty;
                    row.Cells["col_part_wcno"].Value = part_wcno;
                    row.Cells["col_cm"].Value = cm;

                    // ตั้งค่า text wrapping สำหรับคอลัมน์ที่อาจมีข้อความยาว
                    if (gv_list_scan.Columns.Contains("col_partno"))
                    {
                        gv_list_scan.Columns["col_partno"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                        gv_list_scan.Columns["col_partno"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopLeft;
                    }


                    if (string.Equals(mode, "IN", StringComparison.OrdinalIgnoreCase))
                    {
                        row.Cells["col_type"].Style.BackColor = Color.LightGreen;
                    }
                    else
                    {
                        row.Cells["col_type"].Style.BackColor = Color.FromArgb(255, 89, 93);
                    }
                    row.Cells["col_qty"].Style.BackColor = ColorTranslator.FromHtml("#fef9c3");
                    // บังคับให้คำนวณขนาดแถวใหม่
                    gv_list_scan.AutoResizeRow(rowIndex, DataGridViewAutoSizeRowMode.AllCells);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"เกิดข้อผิดพลาดในการดึงข้อมูล: {ex.Message}", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private class ModalItem
        {
            public string Mode { get; set; }
            public string PartNo { get; set; }
            public string CM { get; set; }
            public string PartType { get; set; }
            public int Qty { get; set; }
            public string WCNO { get; set; }
        }

        private List<ModalItem> CollectItemsFromGrid()
        {
            var list = new List<ModalItem>();
            foreach (DataGridViewRow row in gv_list_scan.Rows)
            {
                if (row.IsNewRow) continue;
                var mode = Convert.ToString(row.Cells["col_type"].Value);
                var partNo = Convert.ToString(row.Cells["col_partno"].Value) ?? string.Empty;
                var qtyText = Convert.ToString(row.Cells["col_qty"].Value) ?? "0";
                var wcno = Convert.ToString(row.Cells["col_part_wcno"].Value) ?? string.Empty;
                var cm = Convert.ToString(row.Cells["col_cm"].Value) ?? string.Empty;

                int qty;
                if (!int.TryParse(qtyText, out qty)) qty = 0;

                list.Add(new ModalItem
                {
                    Mode = mode,
                    PartNo = partNo,
                    CM = cm,
                    Qty = qty,
                    WCNO = wcno
                });
            }
            return list;
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            var list = CollectItemsFromGrid();
            if (list.Count == 0)
            {
                MessageBox.Show("ไม่มีรายการให้บันทึก", "ข้อมูลว่าง", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataTable dtToSave = new DataTable();
            dtToSave.Columns.Add("WCNO", typeof(string));
            dtToSave.Columns.Add("Partno", typeof(string));
            dtToSave.Columns.Add("CM", typeof(string));
            dtToSave.Columns.Add("Qty", typeof(int));
            dtToSave.Columns.Add("Type", typeof(string));

            List<PropItemScan> propList = new List<PropItemScan>();
            foreach (var item in list)
            {
                dtToSave.Rows.Add(item.WCNO, item.PartNo, item.CM, item.Qty, item.Mode);
                propList.Add(new PropItemScan
                {
                    WCNO = item.WCNO,
                    PARTNO = item.PartNo,
                    QTY = item.Qty,
                    TYPE = item.Mode,
                    CM = item.CM
                });
            }
            if (SaveToWIPDatabase(dtToSave))
            {
                MessageBox.Show($"บันทึกรายการ {list.Count} รายการแล้ว");
                gv_list_scan.Rows.Clear();
                LoadLogOfDay();
                LoadWIP();
            }
        }

        private void btn_claer_Click(object sender, EventArgs e)
        {
            gv_list_scan.Rows.Clear();
        }

        private void btnIn_Click(object sender, EventArgs e)
        {
            CurrentScanMode = "IN";
            ViewMethod(CurrentScanMode);
            btnOut.BackColor = Color.FromArgb(224, 224, 224);
            btnIn.BackColor = Color.FromArgb(66, 173, 130);
            ini.WriteValue("CONFIG", "MODE", "IN");
        }

        private void btnOut_Click(object sender, EventArgs e)
        {
            CurrentScanMode = "OUT";
            ViewMethod(CurrentScanMode);
            btnOut.BackColor = Color.FromArgb(255, 89, 93);
            btnIn.BackColor = Color.FromArgb(224, 224, 224);
            ini.WriteValue("CONFIG", "MODE", "OUT");
        }

        public class PropItemScan
        {
            public string WCNO { get; set; }
            public string PARTNO { get; set; }
            public int QTY { get; set; }
            public string TYPE { get; set; }
            public string CM { get; set; }
        }


        private void gv_log_scan_of_day_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (gv_log_scan_of_day.Columns[e.ColumnIndex].Name == "col_partno_gv_log")
            {
                gv_log_scan_of_day.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = ColorTranslator.FromHtml("#e9e9e9");
            }
            if (gv_log_scan_of_day.Columns[e.ColumnIndex].Name == "col_trantype_gv_log")
            {
                if (e.Value != null)
                {
                    if (e.Value.ToString() == "IN")
                    {
                        gv_log_scan_of_day.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.LightGreen;
                    }
                    else
                    {
                        gv_log_scan_of_day.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = ColorTranslator.FromHtml("#fbb7b8");
                    }
                }
                gv_log_scan_of_day.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.Font = new Font(gv_log_scan_of_day.Font, FontStyle.Bold);
            }
            if (gv_log_scan_of_day.Columns[e.ColumnIndex].Name == "col_qty_gv_log")
            {
                string TransType = gv_log_scan_of_day.Rows[e.RowIndex].Cells["col_trantype_gv_log"].Value.ToString();
                if (TransType == "OUT")
                {
                    gv_log_scan_of_day.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = ColorTranslator.FromHtml("#fbb7b8");
                }
                else
                {
                    gv_log_scan_of_day.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.LightGreen;
                }
                gv_log_scan_of_day.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.Font = new Font(gv_log_scan_of_day.Font, FontStyle.Bold);
            }
            if (gv_log_scan_of_day.Columns[e.ColumnIndex].Name == "col_partno_gv_log")
            {
                gv_log_scan_of_day.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.Font = new Font(gv_log_scan_of_day.Font, FontStyle.Bold);
            }
        }


        private void gv_wip_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (gv_wip.Columns[e.ColumnIndex].Name == "col_lbal_gv_wip")
            {
                if (Convert.ToInt32(e.Value) > 0)
                {
                    gv_wip.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = ColorTranslator.FromHtml("#e9e9e9");
                }
                else
                {
                    gv_wip.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = ColorTranslator.FromHtml("#fbb7b8");
                    gv_wip.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.Font = new Font(gv_wip.Font, FontStyle.Bold);
                }
            }
            if (gv_wip.Columns[e.ColumnIndex].Name == "col_rec_gv_wip")
            {
                gv_wip.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.LightGreen;
            }
            if (gv_wip.Columns[e.ColumnIndex].Name == "col_issue_gv_wip")
            {
                gv_wip.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = ColorTranslator.FromHtml("#fbb7b8");
            }
            if (gv_wip.Columns[e.ColumnIndex].Name == "col_bal_gv_wip")
            {
                if (Convert.ToInt32(e.Value) < 0)
                {
                    gv_wip.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = ColorTranslator.FromHtml("#fbb7b8");
                    gv_wip.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.Font = new Font(gv_wip.Font, FontStyle.Bold);
                }
                else
                {
                    gv_wip.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = ColorTranslator.FromHtml("#97c1ff");
                }
            }
        }

        private void gv_list_scan_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            // ใช้ AutoSizeRowsMode แทนการกำหนดความสูงคงที่
            gv_list_scan.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            // บังคับให้คำนวณขนาดแถวใหม่สำหรับแถวที่เพิ่มเข้ามา
            for (int i = e.RowIndex; i < e.RowIndex + e.RowCount; i++)
            {
                if (i < gv_list_scan.Rows.Count)
                {
                    gv_list_scan.AutoResizeRow(i, DataGridViewAutoSizeRowMode.AllCells);
                }
            }
        }

        private void cb_wcno_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConfWCNO = cb_wcno.Text;
            ini.WriteValue("CONFIG", "WCNO", ConfWCNO);
            LoadLogOfDay();
            LoadWIP();
        }

        private void dpk_filter_ymd_ValueChanged(object sender, EventArgs e)
        {
            DateTime selectedDate = dpk_filter_ymd.Value;
            filter_ymd = dpk_filter_ymd.Value.ToString("yyyyMMdd");
            if (ConfWCNO != "")
            {
                LoadLogOfDay();
                LoadWIP();
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadLogOfDay();
            LoadWIP();
        }

        private void cb_filter_shift_TextChanged(object sender, EventArgs e)
        {
            string valueChange = cb_filter_shift.Text.ToString();
            ini.WriteValue("CONFIG", "SHIFT", valueChange);
            this.BeginInvoke(new Action(() => btnSearch.PerformClick()));
        }

        private void gv_summary_transtion_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (gv_summary_transtion.Columns[e.ColumnIndex].Name == "col_qty_gv_summary_transtion")
            {
                gv_summary_transtion.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.LightGreen;
            }
            //if (gv_summary_transtion.Columns[e.ColumnIndex].Name == "col_partno_gv_summary_transtion")
            //{
            //    gv_summary_transtion.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.LightGreen;
            //}

        }

        private void selectModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ini.WriteValue("CONFIG", "SCAN_MODE", "DRAWING");
        }

        private void scanByModelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ini.WriteValue("CONFIG", "SCAN_MODE", "MODEL");
        }
    }
}
