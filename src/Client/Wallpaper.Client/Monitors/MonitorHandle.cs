using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Framework.Contracts.Extension;

namespace Plugin.Application.Wallpaper.Client.Monitors
{
    public class MonitorHandle : IDisposable
    {
        private readonly Window _window;
        private readonly ILogger _logger;
        private Image _image;
        //private Unosquare.FFME.MediaElement _video2;
        private System.Windows.Controls.MediaElement _video;

        public string DeviceName { get; }

        public MonitorHandle(ILogger logger, Screen screen)
        {
            DeviceName = screen.DeviceName;
            _window = Setup(screen);
            _logger = logger;

            _logger.Debug("Bounds of monitor '{0}'. W: {1} H: {2} T: {3} L: {4}", DeviceName, _window.Width, _window.Height, _window.Top, _window.Left);
        }

        public bool FixLocation()
        {
            _logger.Debug("Fix location for monitor '{0}'", DeviceName);
            var screen = Screen.AllScreens.FirstOrDefault(x => x.DeviceName.Equals(DeviceName));
            if (screen != null)
            {
                _window.Dispatcher.Invoke(() =>
                {
                    var minLeftScreen = Screen.AllScreens.Min(x => x.Bounds.Left) * -1;
                    var minTopScreen = Screen.AllScreens.Min(x => x.Bounds.Top) * -1;

                    while (_window.Left != screen.Bounds.Left + minLeftScreen)
                    {
                        _window.Left = screen.Bounds.Left + minLeftScreen;
                    }
                    while (_window.Top != screen.Bounds.Top + minTopScreen)
                    {
                        _window.Top = screen.Bounds.Top + minTopScreen;
                    }


                    _window.Height = screen.Bounds.Height;
                    _window.Width = screen.Bounds.Width;

                    _logger.Debug("Bounds of monitor '{0}'. W: {1} H: {2} T: {3} L: {4}", DeviceName, _window.Width, _window.Height, _window.Top, _window.Left);
                });
                return true;
            }
            else
            {
                _logger.Debug("Monitor '{0}' does not exists any more.", DeviceName);
            }

            return false;
        }

        public void DisplayImage(FileInfo image)
        {
            image.Refresh();
            if (image.Exists)
            {
                _logger.Debug("Setting image '{0}' to monitor '{1}'", image.FullName, DeviceName);

                if (image.Extension.Contains("mp4"))
                {
                    _video.Dispatcher.Invoke(() =>
                    {
                        try
                        {
                            _image.Visibility = Visibility.Hidden;
                            _video.Visibility = Visibility.Visible;
                            _video.Source = new Uri(image.FullName);
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex, "Setting of image '{0}' to monitor '{1}' not possible", image.FullName, DeviceName);
                            _image.Source = null;
                        }
                    });
                }
                else
                {
                    _image.Dispatcher.Invoke(() =>
                    {
                        try
                        {
                            _video.Visibility = Visibility.Hidden;
                            _image.Visibility = Visibility.Visible;
                            _image.Source = new BitmapImage(new Uri(image.FullName));
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex, "Setting of image '{0}' to monitor '{1}' not possible", image.FullName, DeviceName);
                            _image.Source = null;
                        }
                    });
                }

            }
            else
            {
                _logger.Debug("Setting empty image to monitor '{1}' because file '{0}' does not exists", image.FullName, DeviceName);
                _image.Dispatcher.Invoke(() =>
                {
                    _image.Source = null;
                });
            }
        }


        public void Show(IntPtr parentHandle)
        {
            _window.Show();
            var windowHandle = new WindowInteropHelper(_window).Handle;
            W32.SetParent(windowHandle, parentHandle);
        }

        public void Dispose()
        {
            if (_window != null)
            {
                _window.Close();
            }
        }

        private Window Setup(Screen screen)
        {
            var minLeftScreen = Screen.AllScreens.Min(x => x.Bounds.Left) * -1;
            var minTopScreen = Screen.AllScreens.Min(x => x.Bounds.Top) * -1;
            _image = new Image()
            {
                Margin = new Thickness(0),
                Stretch = Stretch.UniformToFill
            };
            //_video2 = new Unosquare.FFME.MediaElement
            //{
            //    LoadedBehavior = MediaState.Manual,
            //    Margin = new Thickness(0),
            //    Stretch = Stretch.UniformToFill
            //};
            //_video2.MediaEnded += (s, e) =>
            //{
            //    _video.Position = TimeSpan.FromSeconds(0);
            //    _video.Play();
            //};
            //_video2.MediaOpened += (s, e) =>
            //{
            //    _video.Play();
            //};
            //_video2.Loaded += (s, e) =>
            //{
            //    _video.Play();
            //};

            _video = new System.Windows.Controls.MediaElement
            {
                LoadedBehavior = MediaState.Manual,
                Margin = new Thickness(0),
                Stretch = Stretch.UniformToFill
            };
            _video.MediaEnded += (s, e) =>
            {
                _video.Position = TimeSpan.FromSeconds(0);
                _video.Play();
            };
            _video.MediaOpened += (s, e) =>
            {
                _video.Play();
            };
            _video.Loaded += (s, e) =>
            {
                _video.Play();
            };

            var grid = new Grid();
            grid.Children.Add(_image);
            grid.Children.Add(_video);

            _image.Visibility = Visibility.Hidden;
            _video.Visibility = Visibility.Hidden;

            return new Window
            {
                Height = screen.Bounds.Height,
                Width = screen.Bounds.Width,
                WindowStyle = WindowStyle.None,
                Background = new SolidColorBrush(Colors.DarkSlateGray),
                ShowInTaskbar = false,
                //AllowsTransparency = false,
                ResizeMode = ResizeMode.NoResize,
                Left = screen.Bounds.Left + minLeftScreen,
                Top = screen.Bounds.Top + minTopScreen,
                Content = new Border
                {
                    Background = new SolidColorBrush(Colors.Black),
                    Margin = new Thickness(0),
                    Child = grid
                },
            };


        }


    }
}
