using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DCIWashing
{
    public class CustomTextBox : TextBox
    {
        public Color BorderColor { get; set; } = Color.Blue;
        public int BorderSize { get; set; } = 2;

        public CustomTextBox()
        {
            this.BorderStyle = BorderStyle.None; // ตัด border เดิมออก
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 0x000F) // WM_PAINT
            {
                using (Graphics g = this.CreateGraphics())
                {
                    using (Pen pen = new Pen(BorderColor, BorderSize))
                    {
                        Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
                        g.DrawRectangle(pen, rect);
                    }
                }
            }
        }
    }
}
