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
    class Building : GeometryNode
    {
        public Lot              Lot                    { get; set; }
        public TransformNode    TransformNode          { get; set; }
        public BuildingTransform    EditBuildingTransform  { get; set; }
        public Vector3          CenterOfBase           { get; set; }
        public float            ModelHeight            { get; set; }
        public float            Footprint               { get; set; }
        public int              Stories                { get; set; }
        //private List<Building> observers;
        //private float           scaleRatioToEditable;

        public Building(string address)
            : base(address)
        {
//            this.scaleRatioToEditable = scaleRatioToEditable;
            System.Console.WriteLine(name);
        }

        //public void addObserver(Building b)
        //{
        //    observers.Add( b );
        //}

        //public void broadcast()
        //{
        //    foreach (Building b in observers)
        //    {
        //        b.observe(this);
        //    }
        //}

        //public void observe(Building b)
        //{
        //    this.Footprint = b.Footprint * scaleRatioToEditable;
        //    this.Stories = b.Stories;
        //    this.TransformNode.Scale = b.TransformNode.Scale * this.scaleRatioToEditable;
        //}

        public void calcModelCoordinates()
        {
            //Vector3 minPoint = this.Model.MinimumBoundingBox.Min;
            //Vector3 maxPoint = this.Model.MinimumBoundingBox.Max;

            List<Vector3> verts = this.Model.Vertices;

            Vector3 minPoint = verts[0];
            Vector3 maxPoint = verts[0];

            foreach (Vector3 v in verts) {
                if (v.X > maxPoint.X) maxPoint.X = v.X;
                if (v.X < minPoint.X) minPoint.X = v.X;

                if (v.Y > maxPoint.Y) maxPoint.Y = v.Y;
                if (v.Y < minPoint.Y) minPoint.Y = v.Y;

                if (v.Z > maxPoint.Z) maxPoint.Z = v.Z;
                if (v.Z < minPoint.Z) minPoint.Z = v.Z;
            }

            ModelHeight = maxPoint.Y - minPoint.Y;

            // Project max point onto the base plane
            Vector3 maxPointProj = new Vector3(maxPoint.X, minPoint.Y, maxPoint.Z);

            // Find the middle point between the projection-of-max and min
            CenterOfBase = (maxPointProj + minPoint) / 2.0f;

            //this.Model.ShowBoundingBox = true;
            /*
            Log.Write(name
                + " minPoint: " + minPoint.ToString()
                + ", maxPoint: " + maxPoint.ToString()
                + ", maxPointProj: " + maxPointProj.ToString()
                + ", CenterOfBase: " + CenterOfBase.ToString()
                + ", ModelHeight:" + ModelHeight + "\n");
             */
        }

        public Vector3 getBaseWorld()
        {
            // Combines the marker and within-marker (i.e. World) transformations
            Matrix m = this.WorldTransformation * this.MarkerTransform;

            // Applies the combined matrix to CenterOfBase to get the true world
            // location of CenterOfBase
            return Vector3.Transform(CenterOfBase, m);
        }

        public Vector3 getBaseOnGround()
        {
            // Combines the marker and within-marker (i.e. World) transformations
            Matrix m = Matrix.CreateFromQuaternion(TransformNode.Rotation) *
                           Matrix.CreateScale(TransformNode.Scale);
            m.Translation = TransformNode.Translation;
            
            // Applies the combined matrix to CenterOfBase to get the true world
            // location of CenterOfBase
            return Vector3.Transform(CenterOfBase, m);
        }
    }
}
