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
    class BuildingTransform : TransformNode
    {

        //TransformNode originalTransform;
        public float Footprint { get; set; }
        public int Stories { get; set; }
        //public Vector3 OriginalScale { get; set; }
        public float MaxXScale { get; set; }
        public float MaxZScale { get; set; }
        private List<BuildingTransform> observers = new List<BuildingTransform>();
        private float scaleRatioToEditable;

        public BuildingTransform(float multiplierRatio)
            : base()
        {
            this.scaleRatioToEditable = multiplierRatio;
            this.MaxXScale = this.Scale.X;
            this.MaxZScale = this.Scale.Z;
        }

        public BuildingTransform(String name, Vector3 translation, Quaternion rotation, Vector3 scaling, float multiplierRatio)
            : base(name, translation, rotation, scaling)
        {
            this.scaleRatioToEditable = multiplierRatio;
        }

        public void addObserver(BuildingTransform b)
        {
            observers.Add(b);
        }

        public void broadcast()
        {
            foreach (BuildingTransform b in observers)
            {
                b.observe(this);
            }
        }

        public void observe(BuildingTransform b)
        {
            this.Footprint = b.Footprint * scaleRatioToEditable;
            this.Stories = b.Stories;
            this.Scale = b.Scale * this.scaleRatioToEditable;// new Vector3(this.Scale.X, b.Scale.Y * this.scaleRatioToEditable, this.Scale.Z);//.Y * this.scaleRatioToEditable;
        }

        //public void setOriginalTransform(TransformNode original)
        //{
        //    this.originalTransform = original;
        //}

        public void update() {
        }

        //internal void mimic(TransformNode original)
        //{
        //    this.originalTransform = original;
        //    this.Translation = new Vector3(0, 0, 3);
        //    this.Rotation = original.Rotation;
        //    this.Scale = original.Scale * new Vector3(3, 3, 3);
        //}


        //internal object getOriginalTransform()
        //{
        //    return originalTransform;
        //}
    }
}
