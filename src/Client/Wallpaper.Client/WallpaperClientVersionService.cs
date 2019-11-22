using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoUpdate.Abstraction;
using AutoUpdate.Abstraction.Model;

namespace Plugin.Application.Wallpaper.Client
{
    public class WallpaperClientVersionService : ICurrentVersionDeterminer
    {
        private readonly object _lock;
        private bool _initialized;

        private VersionNumber _version;

        public WallpaperClientVersionService()
        {
            _lock = new object();
            _initialized = false;
        }

        private void Init()
        {
            if (_initialized) return;
            lock (_lock)
            {
                if (_initialized) return;

                _version = new VersionNumber(1, 1, 1, 1);
            }
        }

        public VersionNumber DetermineCurrentVersionNumber()
        {
            Init();
            return _version;
        }
    }
}
