using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Framework.Abstraction.Extension;
using Plugin.Application.Wallpaper.Common.Model;
using Plugin.Application.Wallpaper.Common.Model.Visitors;
using Plugin.Application.Wallpaper.DataAccess.Contracts.Managers;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Drawing;
using SixLabors.ImageSharp.Processing.Text;
using SixLabors.ImageSharp.Processing.Transforms;
using SixLabors.Primitives;

namespace Plugin.Application.Wallpaper.Activities.ImageConverter
{
    public class ImageConverterWallpaperHandler : IWallpaperFileOriginalVisitor
    {
        private readonly ILogger _logger;
        private readonly Common.Model.Wallpaper _wallpaper;
        private readonly IWallpaperManager _wallpaperManager;
        private readonly List<WallpaperFileGenerated> _generatedFiles;
        private readonly List<WallpaperFileImage> _images;
        private readonly FontCollection _fonts;
        private readonly FontFamily _robotoFont;

        public ImageConverterWallpaperHandler(ILogger logger,
                                              Common.Model.Wallpaper wallpaper,
                                              IWallpaperManager wallpaperManager)
        {
            _logger = logger;
            _wallpaper = wallpaper;
            _wallpaperManager = wallpaperManager;

            _generatedFiles = new List<WallpaperFileGenerated>();
            _images = new List<WallpaperFileImage>();

            using (var fontStream = GetType().Assembly.GetManifestResourceStream("Plugin.Application.Wallpaper.Roboto-Regular.ttf"))
            {
                _fonts = new FontCollection();
                _robotoFont = _fonts.Install(fontStream);
            }
        }

        public void Handle(WallpaperFileGenerated wallpaperFile)
            => _generatedFiles.Add(wallpaperFile);
        public void Handle(WallpaperFileVideo wallpaperFile)
        { }
        public void Handle(WallpaperFileImage wallpaperFile)
            => _images.Add(wallpaperFile);

        public void ProcessFiles()
        {
            foreach (var generatedFile in _generatedFiles)
            {
                _wallpaperManager.DeleteFile(_wallpaper, generatedFile);
            }

            _generatedFiles.Clear();

            foreach (var file in _images)
            {
                try
                {
                    _logger.Debug("Creating image with caption for file '{fileId}' of wallpaper '{wallpaperId}'", file.FileId, _wallpaper.Id);
                    GeneratedImageWithCaption(file);

                    _logger.Debug("Creating thumbnail image for file '{fileId}' of wallpaper '{wallpaperId}'", file.FileId, _wallpaper.Id);
                    GenerateThumbnail(file);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error during generating images occurred");
                }

            }

        }

        private void GenerateThumbnail(WallpaperFileImage original)
        {
            var fileContent = _wallpaperManager.GetFile(_wallpaper, original);
            if (!fileContent.HasValue) return;

            using (var image = Image.Load(fileContent.Value.Data))
            using (var destStream = new MemoryStream())
            {
                const int targetHeight = 300;
                var ratio = image.Size().Height / targetHeight;
                var targetSize = new Size(image.Size().Width / ratio, targetHeight);

                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = targetSize
                }));

                image.SaveAsPng(destStream);

                var file = new WallpaperFileWithData()
                {
                    Data = destStream.ToArray(),
                    FileDto = new WallpaperFileThumbnail
                    {
                        OriginalFileId = original.FileId,
                        Height = targetSize.Height,
                        Width = targetSize.Width
                    }
                };
                _wallpaperManager.AddFile(_wallpaper, file);
            }
        }

        private void GeneratedImageWithCaption(WallpaperFileImage original)
        {
            _logger.Debug("1");
            var fileContent = _wallpaperManager.GetFile(_wallpaper, original);
            if (!fileContent.HasValue) return;

            _logger.Debug("2");
            var caption = CreateCaptions();
            if (string.IsNullOrEmpty(caption)) return;

            _logger.Debug("3");
            using (var image = Image.Load(fileContent.Value.Data))
            using (var destStream = new MemoryStream())
            {
                _logger.Debug("4");
                var font = _robotoFont.CreateFont(14);
                var size = TextMeasurer.Measure(caption, new RendererOptions(font));
                var bounds = image.Bounds();

                _logger.Debug("5");
                var start = (bounds.Width / 2) - (size.Width / 2);
                var point = new PointF(start, 14);
                var boxLocation = new RectangleF(point, size);
                boxLocation.Inflate(4, 4);

                _logger.Debug("6");
                image.Mutate(x => x.Fill(Rgba32.White, boxLocation));
                image.Mutate(x => x.DrawText(caption, font, Rgba32.Black, point));
                image.SaveAsPng(destStream);

                var file = new WallpaperFileWithData()
                {
                    Data = destStream.ToArray(),
                    FileDto = new WallpaperFileCaption
                    {
                        OriginalFileId = original.FileId,
                        Position = WallpaperFileCaption.WallpaperFileThumbnailPosition.Top
                    }
                };
                _logger.Debug("7");
                _wallpaperManager.AddFile(_wallpaper, file);
            }
        }

        private string CreateCaptions() => _wallpaper.Information.Caption;
    }

}
