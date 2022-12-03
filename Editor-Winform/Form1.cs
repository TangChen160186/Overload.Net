using OpenTK;
using OpenTK.Graphics.OpenGL4;
using WeifenLuo.WinFormsUI.Docking;
using Color = System.Drawing.Color;

namespace Editor_Winform
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            var f2 = new Form2() { TabText = "Document" };
            dockPanel1.Theme = new VS2015DarkTheme();
            f2.Show(this.dockPanel1, DockState.Document);
            f2 = new Form2() { TabText = "DockLeft" }; ;
            f2.Show(this.dockPanel1, DockState.DockLeft);
            f2 = new Form2() { TabText = "DockRight" }; ;
            f2.Show(this.dockPanel1, DockState.DockRight);
            f2 = new Form2() { TabText = "DockBottom" }; ;
            f2.Show(this.dockPanel1, DockState.DockBottom);
            f2 = new Form2() { TabText = "DockLeftAutoHide" }; ;
            f2.Show(this.dockPanel1, DockState.DockLeftAutoHide);
            f2 = new Form2() { TabText = "Float" }; ;
            f2.Show(this.dockPanel1, DockState.DockLeft);
        }
    }
}