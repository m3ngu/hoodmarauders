using System;
using System.Collections.Generic;
using System.Collections;
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
        /* this is mostly just the model */
        public Building building;
        
        /* this is the transformNode that can be manipulated */
        public TransformNode transformNode;

        public String name;
        public String knownAs;
        public String realAddress;
        public String blockAndLot;
        public String censusTract;
        public String actualLand;
        public String actualTotal;
        public int stories;
        public String residentialUnits;
        public String commercialUnits;
        public String yearBuilt;
        public String buildingClass;
        public String toxicSites;
        public String salePrice;
        public String saleDate;
        public float airRights;
        public String zoningMapNum;
        public String zoningDistrict;
        public float lotFrontage;
        public float lotDepth;
        public float lotArea;
        public float bldgGrossArea;
        public int maxFlrAreaRatio;
        public int floors;

        /*mostly for output*/
        private Hashtable infoTable;

        public Lot(String[] chunks)
        {
            this.name = chunks[0];
            readInfo(chunks);
            infoTable = getInfo();
        }

        /* gets data from buildings_plain_subset.csv. building specific data is loaded into building class
         * and the rest into this */
        public Boolean readInfo(String[] chunks)
        {
            this.knownAs = chunks[5];
            this.realAddress = chunks[6];
            this.blockAndLot = chunks[7];
            this.censusTract = chunks[8];
            this.actualLand = chunks[9];
            this.actualTotal = chunks[10];
            this.stories = int.Parse(chunks[11]);
            this.residentialUnits = chunks[12];
            this.commercialUnits = chunks[13];
            this.yearBuilt = chunks[14];
            this.buildingClass = chunks[15];
            this.toxicSites = chunks[16];
            this.salePrice = chunks[17];
            this.saleDate = chunks[18];
            this.airRights = float.Parse(chunks[19]) + Settings.AdditionalAirRights;
            this.zoningMapNum = chunks[20];
            this.zoningDistrict = chunks[21];
            this.lotFrontage = float.Parse(chunks[22]);
            this.lotDepth = float.Parse(chunks[23]);
            this.lotArea = float.Parse(chunks[24]);
            this.bldgGrossArea = float.Parse(chunks[25]);
            this.maxFlrAreaRatio = int.Parse(chunks[26]);
            this.floors = int.Parse(chunks[27]);
            return true;
        }

        /* returns hash table of all properties, mostly for output */
        public Hashtable getInfo()
        {
            Hashtable allInfo = new Hashtable();
            allInfo.Add("Known As",this.knownAs);
            allInfo.Add("Real Address",this.realAddress);
            allInfo.Add("Block and Lot",this.blockAndLot);
            allInfo.Add("Census Tract",this.censusTract);
            allInfo.Add("Actual Land",this.actualLand);
            allInfo.Add("Actual Total",this.actualTotal);
            allInfo.Add("Stories",this.stories);
            allInfo.Add("Residential Units",this.residentialUnits);
            allInfo.Add("Commercial Units",this.commercialUnits);
            allInfo.Add("Year Built", this.yearBuilt);
            allInfo.Add("Building Class",this.buildingClass);
            allInfo.Add("Toxic Sites",this.toxicSites);
            allInfo.Add("Sale Price",this.salePrice);
            allInfo.Add("Sale Date",this.saleDate);
            allInfo.Add("Air Rights",this.airRights);
            allInfo.Add("Zoning Map Number",this.zoningMapNum);
            allInfo.Add("Zoning District",this.zoningDistrict);
            allInfo.Add("Lot Frontage",this.lotFrontage);
            allInfo.Add("Lot Depth",this.lotDepth);
            allInfo.Add("Lot Area",this.lotArea);
            allInfo.Add("Building Gross Area",this.bldgGrossArea);
            allInfo.Add("Max Floor Area Ratio",this.maxFlrAreaRatio);
            allInfo.Add("Floors",this.floors);
            return allInfo;
        }

        public Hashtable getInfoTable()
        {
            return infoTable;
        }
        
        public Building addBuilding(Building building)
        {
            this.building = building;
            building.Stories = this.stories;
            return building;
        }
    }


}
