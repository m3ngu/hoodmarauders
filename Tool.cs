using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using GoblinXNA.SceneGraph;
using GoblinXNA.Graphics;
using GoblinXNA.Graphics.Geometry;


namespace Manhattanville
{
    class Tool : TransformNode
    {
        public Tool()
        {
            Material mat = new Material();
            mat.Specular = Color.White.ToVector4();
            mat.Diffuse = Color.Red.ToVector4();
            mat.SpecularPower = 10;

            GeometryNode toolGeoNode1 = new GeometryNode("Tool1");
            toolGeoNode1.Model = new Box(5, 5, 0.1f);
            toolGeoNode1.Material = mat;
            TransformNode toolTransNode1 = new TransformNode(new Vector3(0, 5, 0));
            toolTransNode1.AddChild(toolGeoNode1);

            base.AddChild(toolTransNode1);

            GeometryNode toolGeoNode2 = new GeometryNode("Tool2");
            toolGeoNode2.Model = new Box(5, 5, 0.1f);
            toolGeoNode2.Material = mat;
            TransformNode toolTransNode2 = new TransformNode(new Vector3(7.5f, 5, 0));
            toolTransNode2.AddChild(toolGeoNode2);

            base.AddChild(toolTransNode2);

            GeometryNode toolGeoNode3 = new GeometryNode("Tool3");
            toolGeoNode3.Model = new Box(5, 5, 0.1f);
            toolGeoNode3.Material = mat;
            TransformNode toolTransNode3 = new TransformNode(new Vector3(15, 5, 0));
            toolTransNode3.AddChild(toolGeoNode3);

            base.AddChild(toolTransNode3);

        }
    }
}
