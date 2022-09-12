using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
namespace LinkInterceptor;

static class Program {
    [STAThread]
    static void Main()
    {
        int WM_NCLBUTTONDOWN = 0xA1;
        int HTCAPTION = 0x2;
        [DllImport("User32.dll")]
        static extern bool ReleaseCapture();
        [DllImport("User32.dll")]
        static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        Form f = new Form();
        f.BackColor = Color.Black;
        f.FormBorderStyle = FormBorderStyle.None;
        var screen = Screen.PrimaryScreen;
        f.StartPosition = FormStartPosition.Manual;
        f.Bounds = screen.Bounds;
        f.Location = screen.WorkingArea.Location;
        f.TopMost = true;
        // f.DoubleClick += (object? sender, System.EventArgs e) =>
        // {
        // };
        f.MouseDown += (object? sender, MouseEventArgs e) =>
        {
            if (e.Button == MouseButtons.Middle) {
                // exit on middle click
                Application.Exit();
            }
            if (e.Button == MouseButtons.Right) {
                var me = Process.GetCurrentProcess()?.MainModule?.FileName;
                if (me == null) return;
                Process.Start(me);
            }
            if (e.Button != MouseButtons.Left) return;
            var isFull = f.FormBorderStyle == FormBorderStyle.None;
            if (e.Clicks >= 2) {
                // toggle fullscreen
                var screen = Screen.FromRectangle(new(f.Location, f.Size));
                if (isFull)
                {
                    f.FormBorderStyle = FormBorderStyle.FixedDialog;
                    f.Bounds = new(screen.Bounds.X + screen.Bounds.Width / 4, screen.Bounds.Y + screen.Bounds.Height / 4, screen.Bounds.Width/2, screen.Bounds.Height/2);
                }
                else
                {
                    f.FormBorderStyle = FormBorderStyle.None;
                    f.Bounds = screen.Bounds;

                }
            } else {
                // click and drag support
                if (isFull) return;
                ReleaseCapture();
                SendMessage(f.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        };
        Application.EnableVisualStyles();
        Application.Run(f);
    }
}