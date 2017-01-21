using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using WaveSim;

namespace CellularAutomataScreensaver
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // Used to host WPF content in preview mode, attach HwndSource to parent Win32 window.
        private HwndSource winWPFContent;
        private MainWindow winSaver;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Preview mode--display in little window in Screen Saver dialog
            // (Not invoked with Preview button, which runs Screen Saver in
            // normal /s mode).
            if (e.Args[0].ToLower().StartsWith("/p"))
            {
                winSaver = new MainWindow(true);

                Int32 previewHandle = Convert.ToInt32(e.Args[1]);
                //WindowInteropHelper interopWin1 = new WindowInteropHelper(win);
                //interopWin1.Owner = new IntPtr(previewHandle);

                IntPtr pPreviewHnd = new IntPtr(previewHandle);

                RECT lpRect = new RECT();
                bool bGetRect = Win32API.GetClientRect(pPreviewHnd, ref lpRect);

                HwndSourceParameters sourceParams = new HwndSourceParameters("sourceParams");

                sourceParams.PositionX = 0;
                sourceParams.PositionY = 0;
                sourceParams.Height = lpRect.Bottom - lpRect.Top;
                sourceParams.Width = lpRect.Right - lpRect.Left;
                sourceParams.ParentWindow = pPreviewHnd;
                sourceParams.WindowStyle = (int)(WindowStyles.WS_VISIBLE | WindowStyles.WS_CHILD | WindowStyles.WS_CLIPCHILDREN);

                winWPFContent = new HwndSource(sourceParams);
                winWPFContent.Disposed += winWPFContent_Disposed;
                winWPFContent.RootVisual = winSaver.LayoutRoot;
            }

            // Normal screensaver mode.  Either screen saver kicked in normally,
            // or was launched from Preview button
            else if (e.Args[0].ToLower().StartsWith("/s"))
            {
                MainWindow win = new MainWindow();
                win.WindowState = WindowState.Maximized;
                win.Show();
            }

            // Config mode, launched from Settings button in screen saver dialog
            else if (e.Args[0].ToLower().StartsWith("/c"))
            {
                SettingsWindow win = new SettingsWindow();
                win.Show();
            }

            // If not running in one of the sanctioned modes, shut down the app
            // immediately (because we don't have a GUI).
            else
            {
                Application.Current.Shutdown();
            }
        }

        void winWPFContent_Disposed(object sender, EventArgs e)
        {
            winSaver.Close();
        }
    }
}
