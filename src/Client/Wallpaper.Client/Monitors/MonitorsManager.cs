using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Framework.Abstraction.Extension;
using Microsoft.Win32;

namespace Plugin.Application.Wallpaper.Client.Monitors
{
    public class MonitorsManager
    {
        private List<MonitorHandle> _handles;
        private readonly IntPtr _workerW;
        private readonly ILogger _logger;
        private readonly ILogManager _logManager;

        public int MonitorCount
            => _handles.Count;

        public MonitorsManager(ILogger logger, ILogManager logManager)
        {
            _handles = new List<MonitorHandle>();
            _logger = logger;
            _logManager = logManager;

            _workerW = Init();
            SetupMonitorHandles();

            SystemEvents.DisplaySettingsChanged += (s, e) =>
            {
                _logger.Info("Change of display settings detected. Rearrange background handles");
                SetupMonitorHandles();
            };

        }

        public void SetImageToMonitor(FileInfo image, int monitorId)
        {
            if (_handles.Count <= monitorId)
            {
                throw new InvalidOperationException();
            }

            _logger.Info("Setting image '{imagePath}' to monitor '{monitorId}'", image, monitorId);
            _handles[monitorId].DisplayImage(image);
        }

        public void ResetMonitorHandles()
        {
            foreach (var monitorHandle in _handles.ToArray())
            {
                _handles.Remove(monitorHandle);
                monitorHandle.Dispose();
            }

            SetupMonitorHandles();
        }

        private IntPtr Init()
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

            return workerw;
        }

        private void SetupMonitorHandles()
        {
            _logger.Debug("Setup of monitor handles");
            foreach (var screen in Screen.AllScreens)
            {
                var handle = _handles.FirstOrDefault(x => x.DeviceName.Equals(screen.DeviceName));
                if (handle != null)
                {
                    _logger.Debug("Monitor handle for device '{0}' already exists.", screen.DeviceName);
                    handle.FixLocation();
                    continue;
                }

                _logger.Info("Creating new monitor handle for device '{0}'", screen.DeviceName);
                handle = new MonitorHandle(_logManager.GetLogger<MonitorHandle>(), screen);
                _handles.Add(handle);
                handle.Show(_workerW);
            }

            foreach (var monitorHandle in _handles.ToArray())
            {
                if (!monitorHandle.FixLocation())
                {
                    _logger.Info("Monitor handle for device '{0}' obsolete. Removing it.", monitorHandle.DeviceName);
                    monitorHandle.Dispose();
                    _handles.Remove(monitorHandle);
                }
            }
        }
    }
}
