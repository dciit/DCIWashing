using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DCIWashing
{
    public partial class ControlScan : UserControl
    {
        private DataTable ObjRMInfo = null;
        public ControlScan(DataTable objRMInfo)
        {
            InitializeComponent();
            ObjRMInfo = objRMInfo;
            if (ObjRMInfo != null)
            {
                txtRM.Text = objRMInfo.Rows[0]["rm"].ToString();
            }
        }
        public string RM
        {
            get { return txtRM.Text; }
            set { txtRM.Text = value; }
        }
    }
}
