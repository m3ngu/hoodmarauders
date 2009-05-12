using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using GoblinXNA.SceneGraph;
using GoblinXNA.Graphics;
using GoblinXNA.Graphics.Geometry;

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

        public static TransformNode debugSphere(float radius)
        {
            Material m = new Material();
            m.Diffuse = Color.Red.ToVector4();
            m.Specular = Color.White.ToVector4();
            m.SpecularPower = 3f;

            GeometryNode g = new GeometryNode();
            g.Model = new Box(radius);
            g.Material = m;
            
            TransformNode t = new TransformNode();
            t.AddChild(g);

            return t;
        }
    }
}
