using ServiceHost.Docker;

namespace Wallpaper.Service
{
    class Program : Startup
    {
        static void Main(string[] args)
            => new Program().Run(args);
    }
}
