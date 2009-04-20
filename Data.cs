using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Manhattanville
{
    class Data
    {
        public static Texture2D txt2Txt(GraphicsDevice dev, String t, int x, int y, SpriteFont font)
        {

            RenderTarget2D shaderRenderTarget = new RenderTarget2D(
                                                    dev,
                                                    x,
                                                    y,
                                                    1,
                                                    dev.PresentationParameters.BackBufferFormat,
                                                    dev.PresentationParameters.MultiSampleType,
                                                    dev.PresentationParameters.MultiSampleQuality);

            Texture2D shaderRenderTexture = new Texture2D(
                                                     dev,
                                                     x,
                                                     y,
                                                     1,
                                                     TextureUsage.None,
                                                     shaderRenderTarget.Format);

            ResolveTexture2D backBufferTexture = new ResolveTexture2D(
                                                          dev,
                                                          dev.Viewport.Width,
                                                          dev.Viewport.Height,
                                                          1,
                                                          shaderRenderTarget.Format);

            // Save old RenderTarget
            //dev.ResolveBackBuffer(backBufferTexture);

            // Set the RenderTarget
            dev.SetRenderTarget(0, shaderRenderTarget);
            dev.Clear(new Color(0, 0, 0, 0));

            // Draw into the RenderTarget
            GoblinXNA.UI.UI2D.UI2DRenderer.WriteText(Vector2.Zero, t, Color.Red, font);

            // Set render target back to the back buffer
            dev.SetRenderTarget(0, null);
            shaderRenderTexture = shaderRenderTarget.GetTexture();

            // Return the shadow map as a texture
            return shaderRenderTexture;

        }
    }
}
