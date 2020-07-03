using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Explorer
{
    public partial class ProgressForm : Form
    {
        public ProgressForm()
        {
            InitializeComponent();
        }

        public void ShowText(string text)
        {
            label1.Text = text;
        }

        public void ShowProgress(int max,int step,int min=0)
        {
            progressBar1.Maximum = max;
            progressBar1.Minimum = min;
            progressBar1.Value = step;
        }

        public void SetPosition(int parentLeft, int parentWidth,int parentTop,int parentHeight)
        {
            this.Left = parentLeft + (parentWidth - this.Width) / 2;
            this.Top = parentTop + (parentHeight - this.Height) / 2;
        }
    }
}
