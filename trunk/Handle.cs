using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GoblinXNA.SceneGraph;
using GoblinXNA.Graphics;
using GoblinXNA.Graphics.Geometry;

namespace Manhattanville
{
    class Handle : TransformNode
    {

        public GeometryNode GeoNode { get; set; }

        public enum Location : int
        {
            Top = 0,
            BottomNE = 1,
            BottomNW = 2,
            BottomSE = 3,
            BottomSW = 4
        }

        public Handle(String name, Material mat) : base(name) {

            GeoNode = new GeometryNode(/*name+"gn"*/);
            GeoNode.Model = new Sphere(1.0f, 10, 10);
            GeoNode.Physics.Shape = GoblinXNA.Physics.ShapeType.Sphere;
            GeoNode.Material = mat;

            //this.Physics.Pickable = true;
            //gn.Physics.Interactable = true;
            //gn.Physics.Collidable = true;
            //gn.Model.CastShadows = false;
            //gn.Model.ReceiveShadows = false;

            GeoNode.AddToPhysicsEngine = true;
            this.AddChild(GeoNode);

            GeoNode.Enabled = true;

        }
    }
}
