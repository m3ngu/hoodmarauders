using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GoblinXNA.SceneGraph;
using GoblinXNA.Graphics;
using GoblinXNA.Graphics.Geometry;
using Microsoft.Xna.Framework;

namespace Manhattanville
{
    class Handle : TransformNode, IObservingTransform
    {

        public GeometryNode GeoNode { get; set; }
        public Location Loc { get; set; }

        public enum Location : int
        {
            Top = 0,
            BottomNE = 1,
            BottomNW = 2,
            BottomSE = 3,
            BottomSW = 4
        }

        public Handle(Location loc, String name, Material mat)
            : base(name)
        {

            Loc = loc;

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

        public virtual void observe(BuildingTransform bt)
        {
            switch (Loc)
            {
                case Handle.Location.Top:
                    Translation = bt.ModelBuilding.CenterOfCeilWithOffset;
                    break;
                case Handle.Location.BottomSW:
                    Translation = bt.ModelBuilding.MinPointWithOffset;
                    break;
                case Handle.Location.BottomSE:
                    Translation = new Vector3(bt.ModelBuilding.MaxPointWithOffset.X, bt.ModelBuilding.MinPointWithOffset.Y, bt.ModelBuilding.MinPointWithOffset.Z);
                    break;
                case Handle.Location.BottomNE:
                    Translation = new Vector3(bt.ModelBuilding.MaxPointWithOffset.X, bt.ModelBuilding.MaxPointWithOffset.Y, bt.ModelBuilding.MinPointWithOffset.Z);
                    break;
                case Handle.Location.BottomNW:
                    Translation = new Vector3(bt.ModelBuilding.MinPointWithOffset.X, bt.ModelBuilding.MaxPointWithOffset.Y, bt.ModelBuilding.MinPointWithOffset.Z);
                    break;
            }
            
        }
    }
}
