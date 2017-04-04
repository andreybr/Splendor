using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using XNACS1Lib;
using Microsoft.Xna.Framework;

namespace Splendor
{
    public class Noble : XNACS1Rectangle
    {
        private float fWidth = MyLibrary.Resize(86);
        private float fHeight = MyLibrary.Resize(86);

        private int[] nCost = new int[5];
        private int nPoints;
        private bool bAvailable = true;
        private int nOwner = -1;
        XNACS1Rectangle oScore;

        public Noble(Vector2 vNewLocation, int[] nNewCost, int nNewPoints)
        {
            this.Center = vNewLocation;
            this.Width = fWidth;
            this.Height = fHeight;   
            this.LabelColor = Color.White;

            string strTexture = "Noble";
            if (nNewCost[0] > 0)
                strTexture += "W";
            if (nNewCost[1] > 0)
                strTexture += "Bu";
            if (nNewCost[2] > 0)
                strTexture += "G";
            if (nNewCost[3] > 0)
                strTexture += "R";
            if (nNewCost[4] > 0)
                strTexture += "Ba";
            this.Texture = strTexture;

            oScore = new XNACS1Rectangle();
            oScore.LabelFont = "CardValue";
            oScore.Texture = "Empty";
            oScore.Label = "" + nNewPoints;
            oScore.LabelColor = Color.White;
            Recenter(Center);

            for (int x = 0; x < nNewCost.Length; x++)
                nCost[x] = nNewCost[x];

            nPoints = nNewPoints;

            //this.Label = "" + nCost[0] + nCost[1] + nCost[2] + nCost[3] + nCost[4] + "\n" +
            //             nPoints + "\n";
            this.Label = "";
            for (int x = 0; x < nNewCost.Count(); x++)
                if (nNewCost[x] > 0)
                    this.Label += "   " + nCost[x] + "       \n";

            //this.Label = "   " + nCost[0] + "       \n" +
            //             "   " + nCost[0] + "       \n" +
            //             "   " + nCost[1] + "       \n";
        }

        public void Recenter(Vector2 oNewCenter)
        {
            this.Center = oNewCenter;
            oScore.Center = this.Center + new Vector2(1f, 0f);
        }

        public int Points()
        {
            int nToReturn = nPoints;
            return nToReturn;
        }

        public int[] Cost()
        {
            int[] arrnToReturn = new int[5];
            nCost.CopyTo(arrnToReturn, 0);
            return arrnToReturn;
        }

        public int NetCost()
        {
            return nCost[0] + nCost[1] + nCost[2] + nCost[3] + nCost[4] + nCost[5];
        }

        public bool IsAvailable()
        {
            bool bToReturn = bAvailable;
            return bToReturn;
        }        

        public int OwnedBy()
        {
            int nToReturn = nOwner;
            return nToReturn;
        }

        public void SetOwner(int nNewOwner)
        {
            this.TextureTintColor = new Color(100, 100, 100);
            nOwner = nNewOwner;
            bAvailable = false;
        }

        public Noble Copy()
        {
            Noble oToReturn = new Noble(new Vector2(), new int[] { nCost[0], nCost[1], nCost[2], nCost[3], nCost[4] }, nPoints);
            oToReturn.RemoveFromAutoDrawSet();
            oToReturn.Visible = false;
            return oToReturn;
        }

        public void Clear()
        {
            oScore.RemoveFromAutoDrawSet();
            this.RemoveFromAutoDrawSet();
        }
    }
}
