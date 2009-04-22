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
    class Lot
    {
        public Building building;
        public TransformNode transformNode;
        public String name;
        public String knownAs;
        public String realAddress;
        public String blockAndLot;
        public String censusTract;
        public String actualLand;
        public String actualTotal;
        public String stories;
        public String residentialUnits;
        public String commercialUnits;
        public String yearBuilt;
        public String buildingClass;
        public String toxicSites;
        public String salePrice;
        public String saleDate;
        public String airRights;
        public String zoningMapNum;
        public String zoningDistrict;
        public String lotFrontage;
        public String lotDepth;
        public String lotArea;
        public String bldgGrossArea;
        public String maxFlrAreaRatio;
        public String floors;

        public Lot(String name)
        {
            this.name = name;
        }

        public Boolean readInfo(String[] chunks)
        {
            knownAs = chunks[5];
            realAddress = chunks[6];
            blockAndLot = chunks[7];
            censusTract = chunks[8];
            actualLand = chunks[9];
            actualTotal = chunks[10];
            stories = chunks[11];
            residentialUnits = chunks[12];
            commercialUnits = chunks[13];
            yearBuilt = chunks[14];
            buildingClass = chunks[15];
            toxicSites = chunks[16];
            salePrice = chunks[17];
            saleDate = chunks[18];
            airRights = chunks[19];
            zoningMapNum = chunks[20];
            zoningDistrict = chunks[21];
            lotFrontage = chunks[22];
            lotDepth = chunks[23];
            lotArea = chunks[24];
            bldgGrossArea = chunks[25];
            maxFlrAreaRatio = chunks[26];
            floors = chunks[27];
            return true;
        }

        public Building addBuilding(Building building)
        {
            this.building = building;
            return building;
        }
    }


}
