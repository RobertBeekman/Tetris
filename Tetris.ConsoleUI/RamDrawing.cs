using System;
using System.Collections.Generic;
using System.Linq;
using RGB.NET.Core;
using RGB.NET.Devices.Corsair;
using Tetris.ConsoleUI.Interfaces;
using Tetris.GameEngine;

namespace Tetris.ConsoleUI
{
    public class RamDrawing : IDrawing
    {
        public RamDrawing()
        {
            Surface = RGBSurface.Instance;
            Surface.Exception += SurfaceOnException;
            Surface.LoadDevices(CorsairDeviceProvider.Instance);
            Ram = Surface.Devices.Where(d => d.DeviceInfo.DeviceType == RGBDeviceType.DRAM).Take(2).ToList();

            var width = Ram.Count;
            var height = Ram.Max(r => r.Count());

            Console.WriteLine($"Detected {Ram.Count} RAM stick(s), available space: {width}x{height}");
            if (width < 3)
                Console.WriteLine("Warning: The available width is less than 3 so larger blocks will skip, this program literally needs more RAM ;)");
            Console.WriteLine();
        }

        public List<IRGBDevice> Ram { get; set; }

        public RGBSurface Surface { get; set; }

        public void DrawScene(Game game)
        {
            lock (game)
            {
                var array = game.ActualBoard.ToArray();
                for (var y = 0; y <= array.GetUpperBound(0); ++y)
                for (var x = 0; x <= array.GetUpperBound(1); ++x)
                    switch (array[y, x])
                    {
                        case 1:
                            DrawColor(new Color((int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue), x, y);
                            break;
                        case 2:
                            DrawColor(new Color((int) byte.MaxValue, 0, (int) byte.MaxValue), x, y);
                            break;
                        case 3:
                            DrawColor(new Color(0, 0, (int) byte.MaxValue), x, y);
                            break;
                        case 4:
                            DrawColor(new Color(0, (int) byte.MaxValue, 0), x, y);
                            break;
                        case 5:
                            DrawColor(new Color((int) byte.MaxValue, (int) byte.MaxValue, 0), x, y);
                            break;
                        case 6:
                            DrawColor(new Color((int) byte.MaxValue, 0, 0), x, y);
                            break;
                        case 7:
                            DrawColor(new Color(0, (int) byte.MaxValue, (int) byte.MaxValue), x, y);
                            break;
                        case 8:
                            DrawColor(new Color(0, 0, 0), x, y);
                            break;
                        default:
                            DrawColor(new Color(0, 0, 0), x, y);
                            break;
                    }

                Surface.Update();
            }
        }

        private void SurfaceOnException(ExceptionEventArgs args)
        {
            throw args.Exception;
        }

        private void DrawColor(Color color, int x, int y)
        {
            if (Ram.Count > x - 1 && Ram[x].ElementAtOrDefault(y) != null)
                Ram[x].ElementAt(y).Color = color;
        }
    }
}