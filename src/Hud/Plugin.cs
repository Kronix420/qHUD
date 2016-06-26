﻿namespace qHUD.Hud
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Controllers;
    using Interfaces;
    using Settings;
    using UI;
    using Models;
    using SharpDX;
    public abstract class Plugin<TSettings> : IPlugin where TSettings : SettingsBase
    {
        protected readonly GameController GameController;

        protected readonly Graphics Graphics;

        protected Plugin(GameController gameController, Graphics graphics, TSettings settings)
        {
            GameController = gameController;
            Graphics = graphics;
            Settings = settings;
            gameController.EntityListWrapper.EntityAdded += OnEntityAdded;
            gameController.EntityListWrapper.EntityRemoved += OnEntityRemoved;
        }

        protected TSettings Settings { get; private set; }

        public virtual void Dispose()
        {
        }

        public abstract void Render();

        protected static RectangleF GetDirectionsUV(double phi, double distance)
        {
            // could not find a better place yet
            phi += Math.PI * 0.25; // fix rotation due to projection
            if (phi > 2 * Math.PI)
            {
                phi -= 2 * Math.PI;
            }
            var xSprite = (float)Math.Round(phi / Math.PI * 4);
            if (xSprite >= 8)
            {
                xSprite = 0;
            }
            float ySprite = distance > 60 ? distance > 120 ? 2 : 1 : 0;
            float x = xSprite / 8;
            float y = ySprite / 3;
            return new RectangleF(x, y, (xSprite + 1) / 8 - x, (ySprite + 1) / 3 - y);
        }

        protected static IEnumerable<string[]> LoadConfigBase(string path, int columnsCount = 2)
        {
            return File.ReadAllLines(path)
                .Where(line => !string.IsNullOrWhiteSpace(line) && line.IndexOf(';') >= 0 && !line.StartsWith("#"))
                .Select(line => line.Split(new[] { ';' }, columnsCount).Select(parts => parts.Trim()).ToArray());
        }
        protected static IEnumerable<string[]> LoadConfigBase2(string path, int columnsCount = 2)
        {
            return File.ReadAllLines(path).Where(line => !string.IsNullOrWhiteSpace(line) && line.IndexOf(';') >= 0 && !line.StartsWith("#"))
                .Select(line => line.Split(new[] { ';' }, columnsCount).Select(parts => parts.Trim()).ToArray());
        }

        protected virtual void OnEntityAdded(EntityWrapper entityWrapper){}
        protected virtual void OnEntityRemoved(EntityWrapper entityWrapper){}
    }
}