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

        static RenderTarget2D shaderRenderTarget;
        static DepthStencilBuffer depthStencilBuffer;

        public static void initialize(GraphicsDevice dev, int width, int heigth)
        {
            shaderRenderTarget = new RenderTarget2D(dev,
                                                    width,
                                                    heigth,
                                                    1,
                                                    dev.PresentationParameters.BackBufferFormat,
                                                    dev.PresentationParameters.MultiSampleType,
                                                    dev.PresentationParameters.MultiSampleQuality);
            
            depthStencilBuffer = Data.CreateDepthStencil(shaderRenderTarget);
        }

        public static Texture2D txt2Txt(GraphicsDevice dev, String t, int x, int y, SpriteFont font, Color color)
        {

            if (shaderRenderTarget == null)
            {
                initialize(dev, x, y);
            }
            else if (shaderRenderTarget.Width < x || shaderRenderTarget.Height < y)
            {
                shaderRenderTarget.Dispose();
                initialize(dev, x, y);
            }

            Texture2D shaderRenderTexture = new Texture2D(
                                                     dev,
                                                     x,
                                                     y,
                                                     1,
                                                     TextureUsage.None,
                                                     shaderRenderTarget.Format);
                /*
                ResolveTexture2D backBufferTexture = new ResolveTexture2D(
                                                              dev,
                                                              dev.Viewport.Width,
                                                              dev.Viewport.Height,
                                                              1,
                                                              shaderRenderTarget.Format);
                */
                // Save old RenderTarget
                //dev.ResolveBackBuffer(backBufferTexture);
                DepthStencilBuffer old = dev.DepthStencilBuffer;
                // Set our custom depth buffer
                dev.DepthStencilBuffer = depthStencilBuffer;


                // Set the RenderTarget
                dev.SetRenderTarget(0, shaderRenderTarget);
                dev.Clear(color);
                //font.
                // Draw into the RenderTarget
                GoblinXNA.UI.UI2D.UI2DRenderer.WriteText(Vector2.Zero, t, Color.Black, font);


                // Set render target back to the back buffer
                dev.SetRenderTarget(0, null);
                dev.DepthStencilBuffer = old;

                //shaderRenderTexture = shaderRenderTarget.GetTexture();

            int pixelCount = x * y;
            Color[] pixelData = new Color[pixelCount];

            shaderRenderTarget.GetTexture().GetData<Color>(0, new Rectangle(0, 0, x, y), pixelData, 0, pixelCount);
            shaderRenderTexture.SetData<Color>(pixelData);

            // Return the shadow map as a texture
            return shaderRenderTexture;

        }

        public static DepthStencilBuffer CreateDepthStencil(RenderTarget2D target)
        {
            return new DepthStencilBuffer(target.GraphicsDevice, target.Width,
                target.Height, target.GraphicsDevice.DepthStencilBuffer.Format,
                target.MultiSampleType, target.MultiSampleQuality);
        }
        
        public static DepthStencilBuffer CreateDepthStencil(RenderTarget2D target, DepthFormat depth)
        {
            if (GraphicsAdapter.DefaultAdapter.CheckDepthStencilMatch(DeviceType.Hardware,
               GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Format, target.Format,
                depth))
            {
                return new DepthStencilBuffer(target.GraphicsDevice, target.Width,
                    target.Height, depth, target.MultiSampleType, target.MultiSampleQuality);
            }
            else
                return CreateDepthStencil(target);
        }


    }
}
