using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;

namespace Plugin.Application.Wallpaper.Client.Monitors
{
    class MonitorSampleCode
    {
        public void Do()
        {
            var progman = W32.FindWindow("Progman", null);

            var result = IntPtr.Zero;

            W32.SendMessageTimeout(progman,
                                   0x052C,
                                   new IntPtr(0),
                                   IntPtr.Zero,
                                   W32.SendMessageTimeoutFlags.SMTO_NORMAL,
                                   1000,
                                   out result);

            var workerw = IntPtr.Zero;

            W32.EnumWindows(new W32.EnumWindowsProc((tophandle, topparamhandle) =>
            {
                var p = W32.FindWindowEx(tophandle,
                                            IntPtr.Zero,
                                            "SHELLDLL_DefView",
                                            IntPtr.Zero);

                if (p != IntPtr.Zero)
                {
                    // Gets the WorkerW Window after the current one.
                    workerw = W32.FindWindowEx(IntPtr.Zero,
                                               tophandle,
                                               "WorkerW",
                                               IntPtr.Zero);
                }

                return true;
            }), IntPtr.Zero);


            W32.RECT rectResult;
            W32.GetWindowRect(workerw, out rectResult);

            var app = new System.Windows.Application();
            var color = new[]
            {
                new SolidColorBrush(Colors.Red),
                new SolidColorBrush(Colors.Yellow),
                new SolidColorBrush(Colors.Green),
                new SolidColorBrush(Colors.Purple)
            };
            //List<Window> windows = new List<Window>();
            //var counter = 0;

            var screens = Screen.AllScreens;
            var windows = screens.Select((x, i) => new Window()
            {
                Height = x.Bounds.Height,
                Width = x.Bounds.Width,
                WindowStyle = WindowStyle.None,
                Background = color[i],
                //AllowsTransparency = true,
                ResizeMode = ResizeMode.NoResize,
                Left = x.Bounds.Left,
                Top = x.Bounds.Top
            }).ToArray();

            foreach (var w in windows)
            {
                w.Show();
                var windowHandle = new WindowInteropHelper(w).Handle;
                W32.SetParent(windowHandle, progman);
                w.LocationChanged += (s, e) =>
                {

                };
            }

            for (var i = 0; i < windows.Length; i++)
            {
                var s = screens[i];
                var w = windows[i];

                while (w.Left != s.Bounds.Left)
                {
                    w.Left = s.Bounds.Left;
                }
            }


            var t = new System.Threading.Timer(o =>
            {
                windows[0].Dispatcher.Invoke(() =>
                {
                    var minLeftScreen = screens.Min(x => x.Bounds.Left) * -1;
                    var minTopScreen = screens.Min(x => x.Bounds.Top) * -1;
                    for (var i = 0; i < windows.Length; i++)
                    {
                        var s = screens[i];
                        var w = windows[i];


                        while (w.Left != s.Bounds.Left + minLeftScreen)
                        {
                            w.Left = s.Bounds.Left + minLeftScreen;
                        }
                        while (w.Top != s.Bounds.Top + minTopScreen)
                        {
                            w.Top = s.Bounds.Top + minTopScreen;
                        }
                    }
                });
            }, null, 1000, 5000);
        }
    }
}
