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

        public MarkerNode Marker { get; set; }

        public Tool()
        {
            Material mat = new Material();
            mat.Specular = Color.White.ToVector4();
            mat.Diffuse = Color.Red.ToVector4();
            mat.SpecularPower = 10;

            GeometryNode toolGeoNode0 = new GeometryNode("Tool0");
            toolGeoNode0.Model = new Box(5, 5, 0.1f);
            toolGeoNode0.Material = mat;
            TransformNode toolTransNode0 = new TransformNode(
                new Vector3(0, 0, 0), // Translation
                Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.ToRadians(45)) // Rotation
                );
            toolTransNode0.AddChild(toolGeoNode0);

            base.AddChild(toolTransNode0);

            GeometryNode toolGeoNode1 = new GeometryNode("Tool1");
            toolGeoNode1.Model = new Box(5, 5, 0.1f);
            toolGeoNode1.Material = mat;
            TransformNode toolTransNode1 = new TransformNode(new Vector3(0, 0, 0));
            toolTransNode1.AddChild(toolGeoNode1);

            base.AddChild(toolTransNode1);

            GeometryNode toolGeoNode2 = new GeometryNode("Tool2");
            toolGeoNode2.Model = new Box(5, 5, 0.1f);
            toolGeoNode2.Material = mat;
            TransformNode toolTransNode2 = new TransformNode(new Vector3(7.5f, 0, 0));
            toolTransNode2.AddChild(toolGeoNode2);

            base.AddChild(toolTransNode2);

            GeometryNode toolGeoNode3 = new GeometryNode("Tool3");
            toolGeoNode3.Model = new Box(5, 5, 0.1f);
            toolGeoNode3.Material = mat;
            TransformNode toolTransNode3 = new TransformNode(new Vector3(15, 0, 0));
            toolTransNode3.AddChild(toolGeoNode3);

            base.AddChild(toolTransNode3);

            base.Translation = new Vector3(2.5f, 0, 0);
        }

        public Matrix getWorldTransformation()
        {
            return Marker.WorldTransformation;
        }
    }
}
