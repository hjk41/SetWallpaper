﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;
using System.IO;
using Microsoft.Win32;

namespace SetWallpaper
{
    public sealed class Wallpaper
    {
        Wallpaper() { }

        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        public enum Style : int
        {
            Fill,
            Fit,
            Span,
            Stretch,
            Tile,
            Center            
        }

        public static void Set(String path, String styleString)
        {
            Style style = Style.Center;
            if (styleString == "fill")
                style = Style.Fill;
            else if (styleString == "fit")
                style = Style.Fit;
            else if (styleString == "span")
                style = Style.Span;
            else if (styleString == "stretch")
                style = Style.Stretch;
            else if (styleString == "tile")
                style = Style.Tile;
            else if (styleString == "center")
                style = Style.Center;
            System.IO.Stream s = new System.Net.WebClient().OpenRead(path);

            Image img = Image.FromStream(s);
            string tempPath = Path.Combine(Path.GetTempPath(), "wallpaper.bmp");
            img.Save(tempPath, System.Drawing.Imaging.ImageFormat.Bmp);

            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            if (style == Style.Fill)
            {
                key.SetValue(@"WallpaperStyle", 10.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }
            if (style == Style.Fit)
            {
                key.SetValue(@"WallpaperStyle", 6.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }
            if (style == Style.Span) // Windows 8 or newer only!
            {
                key.SetValue(@"WallpaperStyle", 22.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }
            if (style == Style.Stretch)
            {
                key.SetValue(@"WallpaperStyle", 2.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }
            if (style == Style.Tile)
            {
                key.SetValue(@"WallpaperStyle", 0.ToString());
                key.SetValue(@"TileWallpaper", 1.ToString());
            }
            if (style == Style.Center)
            {
                key.SetValue(@"WallpaperStyle", 0.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }

            SystemParametersInfo(SPI_SETDESKWALLPAPER,
                0,
                tempPath,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                System.Console.WriteLine("usage: SetWallPaper filePath [fill/fit/span/stretch/tile/center]");
                return;
            }
            string file = args[0];
            string style = "center";
            if (args.Length >= 2)
                style = args[1];
            Wallpaper.Set(file, style);
        }
    }
}
