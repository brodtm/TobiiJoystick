using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TobiiJoystick
{
    public partial class PropEditor : Form
    {
        public PropEditor()
        {
            InitializeComponent();
        }
        public void SetProps(object o)
        {
            propertyGrid1.SelectedObject = o;
        }
    }
}
