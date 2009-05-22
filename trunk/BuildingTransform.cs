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
    class BuildingTransform : TransformNode, IObservingTransform
    {

        //TransformNode originalTransform;
        public float Footprint { get; set; }
        public int Stories { get; set; }
        //public Vector3 OriginalScale { get; set; }
        public float MaxXScale { get; set; }
        public float MaxYScale { get; set; }
        public Vector3 origScale { get; set; }
        private List<IObservingTransform> observers = new List<IObservingTransform>();
        private float scaleRatioToEditable;
        public Building ModelBuilding { get; set; }
        public bool real = false;
        public float zOffset { get; set; }

        public BuildingTransform(String name)
            : base(name)
        {
            origScale = Vector3.One;
        }

        public BuildingTransform(String name, Building modelbuilding, float multiplierRatio)
            : base(name)
        {
            this.ModelBuilding = modelbuilding;
            this.scaleRatioToEditable = multiplierRatio;
            this.MaxXScale = this.Scale.X;
            this.MaxYScale = this.Scale.Y;
            origScale = Vector3.One;
        }

        public BuildingTransform(String name, Vector3 translation, Quaternion rotation, Vector3 scaling, float multiplierRatio)
            : base(name, translation, rotation, scaling)
        {
            this.scaleRatioToEditable = multiplierRatio;
            this.MaxXScale = this.Scale.X;
            this.MaxYScale = this.Scale.Y;
            origScale = scaling;
        }

        public void addObserver(IObservingTransform b)
        {
            observers.Add(b);
        }

        public void broadcast()
        {
            foreach (IObservingTransform ot in observers)
            {
                ot.observe(this);
            }
        }

        public virtual void observe(BuildingTransform bt)
        {
            //what does this footprint do?
            this.Footprint = bt.Footprint * this.scaleRatioToEditable;
            this.Stories = bt.Stories;
            if (real)
            {
                Vector3 s = this.Scale;
                s.X = s.X * (bt.Scale.Z/bt.Scale.Y);
                this.Scale = s;
            }
            else
            {
                //this.Scale = bt.Scale * this.scaleRatioToEditable;// new Vector3(this.Scale.X, b.Scale.Y * this.scaleRatioToEditable, this.Scale.Z);//.Y * this.scaleRatioToEditable;
                this.Scale = new Vector3(
                    this.origScale.X * bt.Scale.X,
                    this.origScale.Y * bt.Scale.Y,
                    this.origScale.Z * bt.Scale.Z
                    );

                if (Settings.BuildingsDetailed)
                {
                    this.Translation = new Vector3(
                        this.Translation.X * bt.Scale.X,
                        this.Translation.Y * bt.Scale.Y,
                        -(zOffset * (bt.Scale.Z - 1))
                        );
                }
            }
        }
    }
}
