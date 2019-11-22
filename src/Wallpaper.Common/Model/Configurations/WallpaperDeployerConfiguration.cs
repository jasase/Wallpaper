using Framework.Abstraction.Entities;

namespace Plugin.Application.Wallpaper.Common.Model.Configurations
{
    public abstract class WallpaperDeployerConfiguration : Entity
    {
        public string Name { get; set; }
        public bool Active { get; set; }

        public abstract void Accept(IWallpaperDeployerConfigurationVisitor visitor);
        public abstract TReturn Accept<TReturn>(IWallpaperDeployerConfigurationVisitor<TReturn> visitor);
    }
}
