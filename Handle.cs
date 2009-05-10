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
        public enum Location : int
        {
            Top = 0,
            BottomNE = 1,
            BottomNW = 2,
            BottomSE = 3,
            BottomSW = 4
        }

        public Handle(String name, Material mat) : base(name) {

            GeometryNode gn = new GeometryNode(/*name+"gn"*/);
            gn.Model = new Sphere(1.0f, 10, 10);
            gn.Physics.Shape = GoblinXNA.Physics.ShapeType.Sphere;
            gn.Material = mat;

            //this.Physics.Pickable = true;
            //gn.Physics.Interactable = true;
            //gn.Physics.Collidable = true;
            //gn.Model.CastShadows = false;
            //gn.Model.ReceiveShadows = false;

            gn.AddToPhysicsEngine = true;
            this.AddChild(gn);

            gn.Enabled = true;

        }
    }
}
