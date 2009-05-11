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
    class AirRightsTransform : BuildingTransform, IObservingTransform
    {
        private float scaleRatioToEditable;
        private float airRightsSum;
        private float initialModelFootprint;
        private Material greenmat;
        private Material redmat;

        public AirRightsTransform( Building b ) : base() {
            this.ModelBuilding = b;
            this.initialModelFootprint = this.ModelBuilding.Lot.footprint * Settings.AirAdjustment;
            this.Footprint = this.initialModelFootprint;
            this.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.ToRadians(90));
            //this.Rotation = new Quaternion(MathHelper.ToRadians(90), 0, 0, 1);
            this.Rotation = Quaternion.CreateFromAxisAngle(new Vector3(1,0,0),(float)MathHelper.ToRadians(90));
            //Console.WriteLine("airrights column = "+this.Rotation.ToString());

            redmat = new Material();
            redmat.Diffuse = new Vector4(.5f * 255, .5f * 0, .5f * 0, 0.5f);
            redmat.Specular = Color.White.ToVector4(); //new Vector4(.5f * 255, .5f * 0, .5f * 0, 1f);
            redmat.SpecularPower = 3f;

            greenmat = new Material();
            greenmat.Diffuse = new Vector4(.5f * 0, .5f * 255, .5f * 0, 0.5f);
            greenmat.Specular = Color.White.ToVector4(); //new Vector4(.5f * 255, .5f * 0, .5f * 0, 1f);
            greenmat.SpecularPower = 3f;
        }

        public override void observe(BuildingTransform bt)
        {
            float newAirRights = bt.ModelBuilding.Lot.airRights;
            float oldAirRights = bt.ModelBuilding.Lot.previousAirRights;
            float newFootprint = bt.ModelBuilding.Lot.footprint;
            float oldFootprint = bt.ModelBuilding.Lot.previousFootprint;

            Console.WriteLine("air rights now " + newAirRights);

            //float newHeight = (newAirRights) / newFootprint; //Math.Abs
            //float oldHeight = (oldAirRights) / oldFootprint;

            //float heightScale = newHeight / oldHeight; // = something like 1.1 or .9

            //float radius = (float)Math.Sqrt(bt.ModelBuilding.Lot.footprint) * Settings.GroundToFootRatio * Settings.AirAdjustment;

            float airRightsRatio = newAirRights / (20000 + Settings.AirAdjustment);
            float heightRatio = Math.Abs(airRightsRatio);

            this.Scale = new Vector3(this.Scale.X, this.Scale.Y, heightRatio);
            
            //if(newAirRights > 0) 
            //    if (heightScale != 0) 
            //        this.Scale /= new Vector3(1f, 1f, heightScale);
            //else 
            //    this.Scale *= new Vector3(1f, 1f, heightScale);

            bt.ModelBuilding.Lot.previousAirRights = newAirRights;
            bt.ModelBuilding.Lot.previousFootprint = newFootprint;
            //System.Console.WriteLine("for " + bt.ToString()+" with child of type "+bt.Children[0].GetType());

            //foreach (Building b in bt.Children)
            //{
            //System.Console.WriteLine("bt.ModelBuilding.Lot.airRights = " + bt.ModelBuilding.Lot.airRights);
            if (newAirRights < 0)
            {
                ((AirRightsNode)this.Children[0]).Material = redmat;
            }
            else
            {
                ((AirRightsNode)this.Children[0]).Material = greenmat;
            }
                //airRightsSum += ((Lot)b.Lot).airRights;
                //System.Console.WriteLine("can't seem to access airrights value for this lot.");//AirRightsTransform sum = " + airRightsSum);
            //}
            this.Footprint = bt.Footprint * scaleRatioToEditable;
            this.Stories = bt.Stories;
            //this.Scale = bt.Scale * this.scaleRatioToEditable;// new Vector3(this.Scale.X, b.Scale.Y * this.scaleRatioToEditable, this.Scale.Z);//.Y * this.scaleRatioToEditable;
        }
    }

}
