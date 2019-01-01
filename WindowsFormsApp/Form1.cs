using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WindowsFormsApp.Shapes;

namespace WindowsFormsApp
{
    public partial class Form1 : Form
    {
        private IToolboxService toolboxService = new ToolboxService();

        public Form1()
        {
            InitializeComponent();

            this.designerPanel1.ToolboxService = this.toolboxService;
        }

        private void toolStripMenuItem11_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            item.Checked = !item.Checked;
            this.toolboxService.Index = item.Checked ? 0 : -1;
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;

            float value = float.Parse(item.Text.TrimEnd('%'));

            this.designerPanel1.ScaleValue = value / 100;
        }

        private void 新建字段ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EntityShape entity = this.designerPanel1.Shapes[0] as EntityShape;

            entity.Properties.Add(new EntityShape.EntityProperty() { Name = "Name", Type = typeof(string).Name });
        }
    }
}
