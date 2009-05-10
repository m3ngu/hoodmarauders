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
        public Lot                  Lot                    { get; set; }
        public TransformNode        TransformNode          { get; set; }
        public BuildingTransform    EditBuildingTransform  { get; set; }
        public AirRightsTransform   AirRightsTransformNode { get; set; }
        public Vector3              CenterOfCeilWithOffset    { get; set; }
        public Vector3              CenterOfCeilWithoutOffset { get; set; }
        public Vector3              CenterOfBaseWithOffset    { get; set; }
        public Vector3              CenterOfBaseWithoutOffset { get; set; }
        public Vector3              MinPointWithOffset     { get; set; }
        public Vector3              MaxPointWithOffset     { get; set; }
        public Vector3              MinPointWithoutOffset  { get; set; }
        public Vector3              MaxPointWithoutOffset  { get; set; }
        public float                ModelHeight            { get; set; }
        public float                Footprint              { get; set; }
        public int                  Stories                { get; set; }
        public Matrix               CompTransWithOffset    { get; set; }
        public Matrix               CompTransWithoutOffset { get; set; }
        //private List<Building> observers;
        //private float           scaleRatioToEditable;

        public Building(string address)
            : base(address)
        {
//            this.scaleRatioToEditable = scaleRatioToEditable;
//            System.Console.WriteLine(name);
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

        public void calcModelCoordinatesHelper(
            bool withOffset,
            out Matrix store,
            out Vector3 min,
            out Vector3 max,
            out Vector3 centerOfCeil,
            out Vector3 centerOfBase)
        {
            //Vector3 minPoint = this.Model.MinimumBoundingBox.Min;
            //Vector3 maxPoint = this.Model.MinimumBoundingBox.Max;

            List<Vector3> verts = this.Model.Vertices;

            Vector3 minPoint, maxPoint, transformedPoint;
            Matrix transformation, finalRotation;
            transformation = finalRotation = Matrix.Identity;

            transformation = 
                  Matrix.CreateFromQuaternion(this.TransformNode.Rotation)  // This flips the Y and Z axes
                * Matrix.CreateScale(this.TransformNode.Scale);   // This scaled the model from 1000s to 10s of pixels

            transformation.Translation = this.TransformNode.Translation;  // This is the small adjustment in the Z direction

            finalRotation = Matrix.CreateFromQuaternion(
                Quaternion.CreateFromAxisAngle(Vector3.UnitZ, 119 * MathHelper.Pi / 180));  // This rotates around the Z axis

            if (withOffset)
                transformation = Matrix.Multiply(this.Model.OffsetTransform, transformation);  // This brings the model to the origin
            else
                finalRotation.Translation = new Vector3(-12.5f, -15.69f, 0);

            store = Matrix.Multiply(transformation, finalRotation);

            //Vector3 temp = Vector3.Transform(new Vector3(-12.5f, -15.69f, 0), Quaternion.CreateFromAxisAngle(Vector3.UnitZ, 119 * MathHelper.Pi / 180));
            //Log.Write("temp=" + temp.ToString());

            /*
            if (!withOffset)
                store.Translation += new Vector3(-12.5f, -15.69f, 0);
            */

            minPoint = maxPoint = Vector3.Transform(verts[0], store);

            foreach (Vector3 v in verts)
            {
                transformedPoint = Vector3.Transform(v, store);

                if (transformedPoint.X > maxPoint.X) maxPoint.X = transformedPoint.X;
                if (transformedPoint.X < minPoint.X) minPoint.X = transformedPoint.X;

                if (transformedPoint.Y > maxPoint.Y) maxPoint.Y = transformedPoint.Y;
                if (transformedPoint.Y < minPoint.Y) minPoint.Y = transformedPoint.Y;

                if (transformedPoint.Z > maxPoint.Z) maxPoint.Z = transformedPoint.Z;
                if (transformedPoint.Z < minPoint.Z) minPoint.Z = transformedPoint.Z;
            }

            // Project max point onto the base plane
            Vector3 maxPointProjDown = new Vector3(maxPoint.X, maxPoint.Y, minPoint.Z);
            Vector3 minPointProjUp = new Vector3(minPoint.X, minPoint.Y, maxPoint.Z);

            // Find the middle point between the projection-of-max and min
            centerOfBase = ((maxPointProjDown - minPoint) / 2.0f) + minPoint;
            centerOfCeil = ((maxPoint - minPointProjUp) / 2.0f) + minPointProjUp;

            min = minPoint;
            max = maxPoint;

            //this.Model.ShowBoundingBox = true;
            
            /*
            Log.Write(name
                + " minPoint: " + minPoint.ToString()
                + ", maxPoint: " + maxPoint.ToString()
                + ", maxPointProjDown: " + maxPointProjDown.ToString()
                + ", minPointProjUp: " + minPointProjUp.ToString()
                + ", CenterOfBase: " + CenterOfBase.ToString()
                + ", CenterOfCeil: " + CenterOfCeil.ToString()
                + ", ModelHeight:" + ModelHeight);
            */
        }

        public void calcModelCoordinates()
        {
            Matrix store;
            Vector3 min, max, centerCeil, centerBase;

            calcModelCoordinatesHelper(true, out store, out min, out max, out centerCeil, out centerBase);

            CompTransWithOffset = store;
            MinPointWithOffset = min;
            MaxPointWithOffset = max;
            CenterOfCeilWithOffset = centerCeil;
            CenterOfBaseWithOffset = centerBase;

            calcModelCoordinatesHelper(false, out store, out min, out max, out centerCeil, out centerBase);

            CompTransWithoutOffset = store;
            MinPointWithoutOffset = min;
            MaxPointWithoutOffset = max;
            CenterOfCeilWithoutOffset = centerCeil;
            CenterOfBaseWithoutOffset = centerBase;

            ModelHeight = MaxPointWithOffset.Z - MinPointWithOffset.Z;
        }

        public Vector3 getBaseWorld()
        {
            // Combines the marker and within-marker (i.e. World) transformations  CenterOfCeil
            Vector3 t = CenterOfBaseWithoutOffset;// CenterOfCeilWithoutOffset;
            Vector3 result = Vector3.Transform(t, this.MarkerTransform);

            Log.Write("------------------------------------");
            Log.Write("t =" + t.ToString());
            Log.Write("CenterOfBaseWithOffset =" + CenterOfBaseWithOffset.ToString());
            Log.Write("result =" + result.ToString());
            Log.Write("------------------------------------");
            // Applies the combined matrix to CenterOfBase to get the true world
            // location of CenterOfBase
            return result;
        }

    }
}
