using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using GoblinXNA;
using GoblinXNA.Graphics;
using GoblinXNA.SceneGraph;
using Model = GoblinXNA.Graphics.Model;
using GoblinXNA.Graphics.Geometry;
using GoblinXNA.Device.Generic;
using GoblinXNA.Physics;
using GoblinXNA.Helpers;

using GoblinXNA.UI.UI2D;
using GoblinXNA.UI.Events;

using GoblinXNA.Device.Capture;
using GoblinXNA.Device.Vision;
using GoblinXNA.Device.Vision.Marker;

namespace Manhattanville
{
    class AirRightsNode : GeometryNode
    {
        //override public String Name { get; set; }

        public AirRightsNode(String name, Building modelBuilding)
            : base(name)
        {
            this.Name = name;
            Lot lot = modelBuilding.Lot;
            float surfaceArea = lot.footprint;
            //Console.WriteLine("airRights=" + lot.airRights);
            float height = (Math.Abs(lot.airRights) / surfaceArea);

            //Console.WriteLine("height = Math.Abs(lot.airRights) / surfaceArea = " + height);
            //Vector3 realSize = new Vector3((float)Math.Sqrt(surfaceArea), (float)Math.Sqrt(surfaceArea), height);
            //Vector3 adjustedSize = realSize * groundToFootRatio;
            //System.Console.WriteLine(realSize.X + " " + realSize.Y + " " + realSize.Z + " ratio:" + groundToFootRatio);
            float radius = (float)Math.Sqrt(surfaceArea) * Settings.GroundToFootRatio * Settings.AirAdjustment;
            this.Model = new Cylinder(radius,radius, height, 20);//3,3,height,20);//

            Material airRightsMaterial = new Material();
            if (lot.airRights < 0) {
                airRightsMaterial.Diffuse = new Vector4(.5f * 255, .5f * 0, .5f * 0, 0.5f);
            } else {
                airRightsMaterial.Diffuse = new Vector4(.5f * 0, .5f * 255, .5f * 0, 0.5f);
            }
            airRightsMaterial.Specular = Color.White.ToVector4(); //new Vector4(.5f * 255, .5f * 0, .5f * 0, 1f);
            airRightsMaterial.SpecularPower = 3f;
            //airRightsMaterial.Emissive = Color.White.ToVector4();
            
            this.Material = airRightsMaterial;
            
            this.Physics.Shape = GoblinXNA.Physics.ShapeType.Box;
            this.Physics.Pickable = true;
            this.AddToPhysicsEngine = true;

            this.Model.CastShadows = true;
            this.Model.ReceiveShadows = true;
            //this.Model.OffsetToOrigin = true;
        }
    }
}
