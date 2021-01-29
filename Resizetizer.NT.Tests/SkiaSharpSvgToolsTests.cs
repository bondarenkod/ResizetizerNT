using SkiaSharp;
using System;
using System.Drawing;
using System.IO;
using Xunit;

namespace Resizetizer.NT.Tests
{
    public class SkiaSharpSvgToolsTests
    {
        public class Resize : IDisposable
        {
            readonly string DestinationFilename;
            readonly TestLogger Logger;

            public Resize()
            {
                DestinationFilename = Path.GetTempFileName();
                Logger = new TestLogger();
            }

            public void Dispose()
            {
                //Logger.Persist();
                //File.Copy(DestinationFilename, "output.png", true);
                File.Delete(DestinationFilename);
            }

            [Fact]
            public void BasicNoScaleReturnsOriginalSize()
            {
                var info = new SharedImageInfo();
                info.Filename = "images/camera.svg";
                var tools = new SkiaSharpSvgTools(info, Logger);
                var dpiPath = new DpiPath("", 1);

                tools.Resize(dpiPath, DestinationFilename);

                using var resultImage = SKBitmap.Decode(DestinationFilename);
                Assert.Equal(1792, resultImage.Width);
                Assert.Equal(1792, resultImage.Height);

                using var pixmap = resultImage.PeekPixels();
                Assert.Equal(SKColors.Empty, pixmap.GetPixelColor(10, 10));
                Assert.Equal(SKColors.White, pixmap.GetPixelColor(350, 350));
            }

            [Fact]
            public void BasicWithDownScaleReturnsDownScaledSize()
            {
                var info = new SharedImageInfo();
                info.Filename = "images/camera.svg";
                var tools = new SkiaSharpSvgTools(info, Logger);
                var dpiPath = new DpiPath("", 0.5m);

                tools.Resize(dpiPath, DestinationFilename);

                using var resultImage = SKBitmap.Decode(DestinationFilename);
                Assert.Equal(896, resultImage.Width);
                Assert.Equal(896, resultImage.Height);

                using var pixmap = resultImage.PeekPixels();
                Assert.Equal(SKColors.Empty, pixmap.GetPixelColor(10, 10));
                Assert.Equal(SKColors.White, pixmap.GetPixelColor(175, 175));
            }

            [Fact]
            public void BasicWithColorsKeepsColors()
            {
                var info = new SharedImageInfo();
                info.Filename = "images/camera_color.svg";
                var tools = new SkiaSharpSvgTools(info, Logger);
                var dpiPath = new DpiPath("", 1);

                tools.Resize(dpiPath, DestinationFilename);

                using var resultImage = SKBitmap.Decode(DestinationFilename);
                Assert.Equal(256, resultImage.Width);
                Assert.Equal(256, resultImage.Height);

                using var pixmap = resultImage.PeekPixels();
                Assert.Equal(SKColors.Empty, pixmap.GetPixelColor(10, 10));
                Assert.Equal(SKColors.Red, pixmap.GetPixelColor(37, 137));
                Assert.Equal(SKColors.Lime, pixmap.GetPixelColor(81, 137));
                Assert.Equal(SKColors.Blue, pixmap.GetPixelColor(125, 137));
            }

            [Fact]
            public void WithBaseSizeResizes()
            {
                var info = new SharedImageInfo();
                info.Filename = "images/camera_color.svg";
                info.BaseSize = new Size(512, 512);
                var tools = new SkiaSharpSvgTools(info, Logger);
                var dpiPath = new DpiPath("", 1);

                tools.Resize(dpiPath, DestinationFilename);

                using var resultImage = SKBitmap.Decode(DestinationFilename);
                Assert.Equal(512, resultImage.Width);
                Assert.Equal(512, resultImage.Height);

                using var pixmap = resultImage.PeekPixels();
                Assert.Equal(SKColors.Empty, pixmap.GetPixelColor(10, 10));
                Assert.Equal(SKColors.Red, pixmap.GetPixelColor(74, 274));
                Assert.Equal(SKColors.Lime, pixmap.GetPixelColor(162, 274));
                Assert.Equal(SKColors.Blue, pixmap.GetPixelColor(250, 274));
            }

            [Fact]
            public void WithBaseSizeAndScaleResizes()
            {
                var info = new SharedImageInfo();
                info.Filename = "images/camera_color.svg";
                info.BaseSize = new Size(512, 512);
                var tools = new SkiaSharpSvgTools(info, Logger);
                var dpiPath = new DpiPath("", 0.5m);

                tools.Resize(dpiPath, DestinationFilename);

                using var resultImage = SKBitmap.Decode(DestinationFilename);
                Assert.Equal(256, resultImage.Width);
                Assert.Equal(256, resultImage.Height);

                using var pixmap = resultImage.PeekPixels();
                Assert.Equal(SKColors.Empty, pixmap.GetPixelColor(10, 10));
                Assert.Equal(SKColors.Red, pixmap.GetPixelColor(37, 137));
                Assert.Equal(SKColors.Lime, pixmap.GetPixelColor(81, 137));
                Assert.Equal(SKColors.Blue, pixmap.GetPixelColor(125, 137));
            }

            [Fact]
            public void ColorizedReturnsColored()
            {
                var info = new SharedImageInfo();
                info.Filename = "images/camera.svg";
                info.TintColor = Color.Red;
                var tools = new SkiaSharpSvgTools(info, Logger);
                var dpiPath = new DpiPath("", 1);

                tools.Resize(dpiPath, DestinationFilename);

                using var resultImage = SKBitmap.Decode(DestinationFilename);
                Assert.Equal(1792, resultImage.Width);
                Assert.Equal(1792, resultImage.Height);

                using var pixmap = resultImage.PeekPixels();
                Assert.Equal(SKColors.Empty, pixmap.GetPixelColor(10, 10));
                Assert.Equal(SKColors.Red, pixmap.GetPixelColor(350, 350));
            }

            [Fact]
            public void ColorizedWithAlphaReturnsColored()
            {
                var info = new SharedImageInfo();
                info.Filename = "images/camera.svg";
                info.TintColor = Color.FromArgb(127, Color.Red);
                var tools = new SkiaSharpSvgTools(info, Logger);
                var dpiPath = new DpiPath("", 1);

                tools.Resize(dpiPath, DestinationFilename);

                using var resultImage = SKBitmap.Decode(DestinationFilename);
                Assert.Equal(1792, resultImage.Width);
                Assert.Equal(1792, resultImage.Height);

                using var pixmap = resultImage.PeekPixels();
                Assert.Equal(SKColors.Empty, pixmap.GetPixelColor(10, 10));
                Assert.Equal(SKColors.Red.WithAlpha(127), pixmap.GetPixelColor(350, 350));
            }

            [Fact]
            public void ColorizedWithNamedReturnsColored()
            {
                var info = new SharedImageInfo();
                info.Filename = "images/camera.svg";
                info.TintColor = Color.FromName("Red");
                var tools = new SkiaSharpSvgTools(info, Logger);
                var dpiPath = new DpiPath("", 1);

                tools.Resize(dpiPath, DestinationFilename);

                using var resultImage = SKBitmap.Decode(DestinationFilename);
                Assert.Equal(1792, resultImage.Width);
                Assert.Equal(1792, resultImage.Height);

                using var pixmap = resultImage.PeekPixels();
                Assert.Equal(SKColors.Empty, pixmap.GetPixelColor(10, 10));
                Assert.Equal(SKColors.Red, pixmap.GetPixelColor(350, 350));
            }

            [Fact]
            public void ColorizedWithColorsReplacesColors()
            {
                var info = new SharedImageInfo();
                info.Filename = "images/camera_color.svg";
                info.TintColor = Color.Red;
                var tools = new SkiaSharpSvgTools(info, Logger);
                var dpiPath = new DpiPath("", 1);

                tools.Resize(dpiPath, DestinationFilename);

                using var resultImage = SKBitmap.Decode(DestinationFilename);
                Assert.Equal(256, resultImage.Width);
                Assert.Equal(256, resultImage.Height);

                using var pixmap = resultImage.PeekPixels();
                Assert.Equal(SKColors.Empty, pixmap.GetPixelColor(10, 10));
                Assert.Equal(SKColors.Red, pixmap.GetPixelColor(37, 137));
                Assert.Equal(SKColors.Red, pixmap.GetPixelColor(81, 137));
                Assert.Equal(SKColors.Red, pixmap.GetPixelColor(125, 137));
            }

            [Fact]
            public void ColorizedWithAlphaWithColorsReplacesColors()
            {
                var info = new SharedImageInfo();
                info.Filename = "images/camera_color.svg";
                info.TintColor = Color.FromArgb(127, Color.Red);
                var tools = new SkiaSharpSvgTools(info, Logger);
                var dpiPath = new DpiPath("", 1);

                tools.Resize(dpiPath, DestinationFilename);

                using var resultImage = SKBitmap.Decode(DestinationFilename);
                Assert.Equal(256, resultImage.Width);
                Assert.Equal(256, resultImage.Height);

                using var pixmap = resultImage.PeekPixels();
                Assert.Equal(SKColors.Empty, pixmap.GetPixelColor(10, 10));
                Assert.Equal(SKColors.Red.WithAlpha(127), pixmap.GetPixelColor(37, 137));
                Assert.Equal(SKColors.Red.WithAlpha(127), pixmap.GetPixelColor(81, 137));
                Assert.Equal(SKColors.Red.WithAlpha(127), pixmap.GetPixelColor(125, 137));
            }


            [Fact]
            public void __issue_39()
            {
                var paths = new (string svg, string standard, Point[] pointsToCompare)[]
                {
                    ("images/issue_39_0.svg", "images/issue_39_0_shouldbe.png", new [] {new Point(0,0) })
                };

                foreach (var item in paths)
                    CompareSVGOutputWithStandardImage(item.svg, item.standard, item.pointsToCompare);
            }

            [Fact]
            public void __issue_43()
            {
                var paths = new (string svg, string standard, Point[] pointsToCompare)[]
               {
                    ("images/issue_43_0.svg", "images/issue_39_0_shouldbe.png", new [] {new Point(0,0) }),
                    ("images/issue_43_1.svg", "images/issue_39_1_shouldbe.png", new [] {new Point(0,0) }),
                    ("images/issue_43_2.svg", "images/issue_39_2_shouldbe.png", new [] {new Point(0,0) })
               };

                foreach (var item in paths)
                    CompareSVGOutputWithStandardImage(item.svg, item.standard, item.pointsToCompare);
            }

            private void CompareSVGOutputWithStandardImage(string svg_path, string standard_image_path, params Point[] pixelToCheck)
            {

                foreach (var pxl_loc in pixelToCheck)
                {
                    Assert.True(false, $"SVG compare with the standard image is failed, svg is: {svg_path} at location {pxl_loc}");
                }

                //var info = new SharedImageInfo();
                //info.Filename = "images/camera.svg";
                //var tools = new SkiaSharpSvgTools(info, Logger);
                //var dpiPath = new DpiPath("", 1);

                //tools.Resize(dpiPath, DestinationFilename);

                //using var resultImage = SKBitmap.Decode(DestinationFilename);
                //Assert.Equal(1792, resultImage.Width);
                //Assert.Equal(1792, resultImage.Height);

                //using var pixmap = resultImage.PeekPixels();
                //Assert.Equal(SKColors.Empty, pixmap.GetPixelColor(10, 10));
                //Assert.Equal(SKColors.White, pixmap.GetPixelColor(350, 350));
            }
        }
    }
}
