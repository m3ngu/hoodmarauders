using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Manhattanville
{
    class Utilities
    {
        public static void SaveScreenShot(GraphicsDevice dev, String filename)
        {
            ResolveTexture2D backBufferTexture = new ResolveTexture2D(
                                                         dev,
                                                         dev.Viewport.Width,
                                                         dev.Viewport.Height,
                                                         1,
                                                         dev.PresentationParameters.BackBufferFormat);

            dev.ResolveBackBuffer(backBufferTexture);

            backBufferTexture.Save(filename, ImageFileFormat.Jpg);

        }
    }
}
