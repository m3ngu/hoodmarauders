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
        //private AirRightsNode airRightsNode;

        public AirRightsTransform() : base() {
            //airRightsNode = new AirRightsNode("airRights");
            //this.AddChild(airRightsNode);
        }

        public override void observe(BuildingTransform bt)
        {
            System.Console.WriteLine("for " + bt.ToString()+" with child of type "+bt.Children[0].GetType());

            foreach (Building b in bt.Children)
            {
                //airRightsSum += ((Lot)b.Lot).airRights;
                //System.Console.WriteLine("can't seem to access airrights value for this lot.");//AirRightsTransform sum = " + airRightsSum);
            }
            this.Footprint = bt.Footprint * scaleRatioToEditable;
            this.Stories = bt.Stories;
            this.Scale = bt.Scale * this.scaleRatioToEditable;// new Vector3(this.Scale.X, b.Scale.Y * this.scaleRatioToEditable, this.Scale.Z);//.Y * this.scaleRatioToEditable;
        }
    }

}
