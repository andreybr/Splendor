using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using XNACS1Lib;
using Microsoft.Xna.Framework;

namespace Splendor
{
    public class Player 
    {
        //private float fWidth = MyLibrary.Resize(212);
        //private float fHeight = MyLibrary.Resize(124);
        private float fNobleWidth = MyLibrary.Resize(19);
        private float fNobleHeight = MyLibrary.Resize(19);
        public XNACS1Rectangle oBox = new XNACS1Rectangle();

        public string strName;
        public int nPoints = 0;
        public int nMaxCoins2 = 10;
        public bool bIsHumanPlayer = false;
        //public int[] nOwnedCards = new int[] { 1, 1, 1, 1, 1 };
        public int[] nOwnedCards = new int[] { 0, 0, 0, 0, 0 };
        //public int[] nOwnedCoins = new int[] { 1, 1, 0, 1, 1, 1 }; 
        public int[] nOwnedCoins = new int[] { 0, 0, 0, 0, 0, 0 };
        //public int[] nOwnedCoins = new int[] { 10, 10, 10, 10, 10, 10 };
        //public int[] nOwnedCoins = new int[] { 50, 50, 50, 50, 50, 50 };
        public XNACS1Rectangle[] oNoblesAcquired;
        XNACS1Rectangle oScore;
        XNACS1Rectangle oName;
        XNACS1Rectangle oAvatar;
        public int[] nBoughtCardTiers = new int[3];

        List<XNACS1Rectangle>[] oCardIcons = new List<XNACS1Rectangle>[5];
        List<XNACS1Rectangle>[] oCoinIcons = new List<XNACS1Rectangle>[6];
        float fCoinIconRadius = 0.7f;
        Vector2 oSpaceBetweenCoins = new Vector2(MyLibrary.Resize(11), MyLibrary.Resize(19));
        Vector2 oCardIconOffset = new Vector2(MyLibrary.Resize(-18), MyLibrary.Resize(38));

        XNACS1Rectangle oMessageBox;
        Vector2 oMessageBoxOffset = new Vector2(MyLibrary.Resize(130), MyLibrary.Resize(67));
        int nCharactersPerLine = 30;
        int nMessageBoxFadeTimer = 0;

        public BotBase oBot;

        public Player(int nNumNobles, BotBase oNewBot, string strNewName = null)
        {
            oBot = oNewBot;

            oScore = new XNACS1Rectangle();
            oScore.LabelFont = "PlayerScore";
            oScore.Texture = "Empty";
            oScore.LabelColor = Color.White;

            strName = strNewName;
            oName = new XNACS1Rectangle();
            oName.LabelFont = "PlayerName";
            oName.Texture = "Empty";
            oName.LabelColor = Color.White;
            oName.Label = strNewName;
            while (oName.Label.Length < 15)
                oName.Label += " ";

            oMessageBox = new XNACS1Rectangle(new Vector2(), MyLibrary.Resize(325), MyLibrary.Resize(65), "PlayerMessage");
            oMessageBox.Visible = false;
            oMessageBox.TextureTintColor = new Color(0, 0, 0, 200);
            oMessageBox.LabelColor = Color.White;
            oMessageBox.LabelFont = "PlayerMessage";

            oAvatar = new XNACS1Rectangle(new Vector2(), MyLibrary.Resize(110), MyLibrary.Resize(110));
            //oAvatar.Width += 0.1f;
            //oAvatar.Height += 0.1f;
            oAvatar.Texture = oBot.Picture();
            oAvatar.SetTextureSpriteSheet("JakeFaces3", 3, 1, 0);
            oAvatar.UseSpriteSheet = true;
            oAvatar.UseSpriteSheetAnimation = true;
            oAvatar.SetTextureSpriteAnimationFrames(0, 0, 0, 0, 5, SpriteSheetAnimationMode.AnimateBackward);

            oNoblesAcquired = new XNACS1Rectangle[nNumNobles];

            for (int x = 0; x < oCardIcons.Count(); x++)
                oCardIcons[x] = new List<XNACS1Rectangle>();

            for (int x = 0; x < oCoinIcons.Count(); x++)
                oCoinIcons[x] = new List<XNACS1Rectangle>();

            if (oNewBot.GetType() != typeof(PlayerBot))
            {
                //oBox.Width = MyLibrary.Resize(340);
                //oBox.Height = MyLibrary.Resize(140);
                oBox.Width = MyLibrary.Resize(293);
                oBox.Height = MyLibrary.Resize(143);
                oBox.Color = Color.Black;
                oBox.LabelColor = Color.White;
                oBox.Texture = "PlayerBoxV5";
                oBox.LabelFont = "PlayerBox";

                for (int x = 0; x < oNoblesAcquired.Count(); x++)
                {
                    oNoblesAcquired[x] = new XNACS1Rectangle(new Vector2(), fNobleWidth, fNobleHeight, "NobleIcon");
                    //oNoblesAcquired[x].Color = new Color(147, 62, 190);
                    //oNoblesAcquired[x].Color = new Color(49, 21, 63);
                    oNoblesAcquired[x].TextureTintColor = new Color(125, 125, 125);
                    //oNoblesAcquired[x].Visible = false;
                }

                if (strNewName != null)
                    strName = strNewName;
                else
                    strName = oBot.Name();

                while (strName.Length < 14)
                    strName += " ";
            }
            else
            {
                //oBox.Texture = "PlayerBoxBottom";
                //oBox.Width = MyLibrary.Resize(1200);
                //oBox.Height = MyLibrary.Resize(27);
                //oBox.Center = new Vector2(MyLibrary.Resize(600), MyLibrary.Resize(14));

                for (int x = 0; x < oNoblesAcquired.Count(); x++)
                {
                    oNoblesAcquired[x] = new XNACS1Rectangle(new Vector2(), fNobleWidth, fNobleHeight, "NobleIcon");
                    oNoblesAcquired[x].Color = new Color(147, 62, 190);
                    oNoblesAcquired[x].Color = new Color(49, 21, 63);
                    oNoblesAcquired[x].Center = new Vector2(MyLibrary.Resize(20) + MyLibrary.Resize(23) * x, MyLibrary.Resize(15));
                }

                oScore.Center = new Vector2(MyLibrary.Resize(130), MyLibrary.Resize(15));
                oAvatar.Center = new Vector2(MyLibrary.Resize(65), MyLibrary.Resize(85));
            }
           
            Update();
        }

        public void Update()
        {
            //this.Label = "\n" +
            // "              " + nOwnedCoins[0] + " " + nOwnedCoins[1] + " " + nOwnedCoins[2] + " " + nOwnedCoins[3] + " " + nOwnedCoins[4] + " " + nOwnedCoins[5] + "\n" +
            // "              " + nOwnedCards[0] + " " + nOwnedCards[1] + " " + nOwnedCards[2] + " " + nOwnedCards[3] + " " + nOwnedCards[4] + "  \n";

            //this.Label = "\n" +
            //"       " + nOwnedCoins[0] + " " + nOwnedCoins[1] + " " + nOwnedCoins[2] + " " + nOwnedCoins[3] + " " + nOwnedCoins[4] + " " + nOwnedCoins[5] + "\n" +
            //"       " + nOwnedCards[0] + " " + nOwnedCards[1] + " " + nOwnedCards[2] + " " + nOwnedCards[3] + " " + nOwnedCards[4] + "  \n";

            if (nPoints < 10)
                oScore.Label = " " + nPoints;
            else
                oScore.Label = "" + nPoints;

            if (nMessageBoxFadeTimer > 0)
            {
                int nFade = 180 - nMessageBoxFadeTimer * 15;
                oMessageBox.Color = new Color(0, 0, 0, nFade);
                nMessageBoxFadeTimer--;
            }
        }

        public void RedrawBox()
        {
            //oBox.RemoveFromAutoDrawSet();
            //oBox = new XNACS1Rectangle();
            //oBox.Texture = "PlayerBoxBottom";
            //oBox.Width = MyLibrary.Resize(1200);
            //oBox.Height = MyLibrary.Resize(27);
            //oBox.Center = new Vector2(MyLibrary.Resize(600), MyLibrary.Resize(14));
        }

        public void SetAvatar(int nImageIndex)
        {
            nImageIndex = 0;
            oAvatar.SetTextureSpriteAnimationFrames(nImageIndex, 0, nImageIndex, 0, 5, SpriteSheetAnimationMode.AnimateBackward);
            //nAvatarTimer = nAvatarTimerStart;
        }

        public void Recenter(Vector2 oNewCenter)
        {
            oBox.Center = oNewCenter;

            oScore.Center = oBox.Center + new Vector2(MyLibrary.Resize(123), MyLibrary.Resize(56));
            oName.Center = oBox.Center + new Vector2(MyLibrary.Resize(55), MyLibrary.Resize(56));
            oAvatar.Center = oBox.Center + new Vector2(MyLibrary.Resize(-85), MyLibrary.Resize(10));

            // message box
            if (oMessageBox != null)
            {
                oMessageBox.CenterX = oBox.CenterX;
                oMessageBox.CenterY = oBox.CenterY - oMessageBoxOffset.Y - (oMessageBox.Height / 2);
            }

            // noble indicators
            for (int x = 0; x < oNoblesAcquired.Count(); x++)
            {
                oNoblesAcquired[x].Center = oBox.Center + new Vector2(MyLibrary.Resize(30) + x * MyLibrary.Resize(25), MyLibrary.Resize(-58));
            }

            // old
            //oScore.Center = this.Center + new Vector2(-6.8f, 3f);
            //oName.Center = this.Center + new Vector2(2f, 3f);
            //oPicture.Center = this.Center + new Vector2(MyLibrary.Resize(-155), 0);

            //for (int x = 0; x < oNoblesAcquired.Count(); x++)
            //{
            //    oNoblesAcquired[x].Center = this.Center + new Vector2(MyLibrary.Resize(-36) + x * MyLibrary.Resize(22), MyLibrary.Resize(13));
            //}
        }

        public Vector2 Center()
        {
            return oBox.Center;
        }

        public void ShowMessage(string strMessage)
        {
            //if (oMessageBox != null)
            //{
            //    oMessageBox.RemoveFromAutoDrawSet();
            //    oMessageBox = null;
            //}

            //oMessageBox= new XNACS1Rectangle(new Vector2(), MyLibrary.Resize(580), MyLibrary.Resize(46));
            //oMessageBox.Color = new Color(0, 0, 0, 180);
            //oMessageBox.LabelColor = Color.White;
            //oMessageBox.LabelFont = "PlayerMessage";
            oMessageBox.Visible = true;
            nMessageBoxFadeTimer = 10;

            int nFade = 200 - nMessageBoxFadeTimer * 15;
            oMessageBox.Color = new Color(0, 0, 0, nFade);

            int nCharCount = 0;
            int nLastBreak = -1;
            List<string> oSubStrings = new List<string>();
            bool bEnd = false;

            while (strMessage.Length > 0)
            {
                while (nCharCount < strMessage.Length && nCharCount < nCharactersPerLine)
                {
                    if (strMessage[nCharCount] == ' ')
                        nLastBreak = nCharCount;

                    if (nCharCount == strMessage.Length - 1)
                    {
                        nLastBreak = nCharCount + 1;
                        bEnd = true;
                    }

                    nCharCount++;
                }

                if (bEnd == true)
                {
                    oSubStrings.Add(strMessage.Substring(0, nLastBreak));
                    strMessage = strMessage.Substring(nLastBreak);
                }

                else if (nLastBreak > -1)
                {
                    oSubStrings.Add(strMessage.Substring(0, nLastBreak));
                    strMessage = strMessage.Substring(nLastBreak + 1);
                }
                else
                {
                    oSubStrings.Add(strMessage.Substring(0, nCharCount));
                    strMessage = "";
                }

                while (oSubStrings.Last().Length < nCharactersPerLine - 1)
                {
                    oSubStrings[oSubStrings.Count() - 1] += " ";
                }
                oSubStrings[oSubStrings.Count() - 1] += "\n";

                nCharCount = 0;
                nLastBreak = -1;
            }

            // recombine the substrings
            oMessageBox.Label = "";
            for (int x = 0; x < oSubStrings.Count(); x++)
                oMessageBox.Label += oSubStrings[x];

            //oMessageBox = new XNACS1Rectangle(new Vector2(), MyLibrary.Resize(320), MyLibrary.Resize(50));
        }

        public void TakeTurn()
        {
            oBot.TakeTurn();
        }

        public void ReactToMove(string sName, int nPlayerID, TurnInfo.ActionType eAction, int[] arrnSelectedCoins, Card oCard)
        {
            oBot.ReactToMove(sName, nPlayerID, eAction, arrnSelectedCoins, oCard);
        }

        public bool HasTakenTurn()
        {
            return oBot.HasTakenTurn();
        }

        public void GetCards(int nIndex, int nPoints)
        {
            nOwnedCards[nIndex]++;

            if (oBot.GetType() != typeof(PlayerBot))
            {
                //XNACS1Rectangle oTempCard = (new XNACS1Rectangle(new Vector2(), MyLibrary.Resize(19), MyLibrary.Resize(23), "CardIconTest"));
                //oCardIcons[nIndex].Insert(0, oTempCard);
                oCardIcons[nIndex].Add(new XNACS1Rectangle(new Vector2(), MyLibrary.Resize(16), MyLibrary.Resize(19), "CardIconTest"));
                //oCardIcons[nIndex].Last().RotateAngle = 20;

                if (nPoints > 0)
                {
                    oCardIcons[nIndex].Last().LabelFont = "CardIcon";
                    oCardIcons[nIndex].Last().LabelColor = Color.White;
                    oCardIcons[nIndex].Last().Label = "" + nPoints;
                }

                if (nIndex == 0)
                    oCardIcons[nIndex].Last().TextureTintColor = new Color(230, 230, 230);
                else if (nIndex == 1)
                    oCardIcons[nIndex].Last().TextureTintColor = new Color(62, 127, 190);
                else if (nIndex == 2)
                    oCardIcons[nIndex].Last().TextureTintColor = new Color(62, 190, 86);
                else if (nIndex == 3)
                    oCardIcons[nIndex].Last().TextureTintColor = new Color(190, 62, 62);
                else if (nIndex == 4)
                    oCardIcons[nIndex].Last().TextureTintColor = new Color(70, 50, 28);
                else if (nIndex == 5)
                    oCardIcons[nIndex].Last().TextureTintColor = Color.Gold;
            }
            else
            {
                XNACS1Rectangle oNewCard = new XNACS1Rectangle(new Vector2(), MyLibrary.Resize(84), MyLibrary.Resize(122));

                if (nIndex == 0)
                    oNewCard.Texture = "CardWhite5";
                else if (nIndex == 1)
                    oNewCard.Texture = "CardBlue5";
                else if (nIndex == 2)
                    oNewCard.Texture = "CardGreen5";
                else if (nIndex == 3)
                    oNewCard.Texture = "CardRed5";
                else if (nIndex == 4)
                    oNewCard.Texture = "CardBlack5";

                oNewCard.Label = " \n     " + nPoints + "  \n\n\n\n\n \n\n\n\n \n\n\n\n ";
                oNewCard.LabelFont = "CardValue";
                oNewCard.LabelColor = Color.White;

                oCardIcons[nIndex].Insert(0, oNewCard);
                RedrawBox();
            }

            PositionCardIcons();
            PositionCoinIcons();
        }
        public void PositionCardIcons()
        {
            if (oBot.GetType() != typeof(PlayerBot))
            {
                for (int x = 0; x < oCardIcons.Count(); x++)
                {
                    for (int y = 0; y < oCardIcons[x].Count(); y++)
                    {
                        oCardIcons[x][y].Center = oBox.Center + oCardIconOffset + new Vector2(oSpaceBetweenCoins.X * y, -oSpaceBetweenCoins.Y * x);
                        //oCardIcons[x][y].Center = oBox.Center + oCardIconOffset + new Vector2(oSpaceBetweenCoins.X * x, -oSpaceBetweenCoins.Y * y);
                    }
                }
            }
            else
            {
                for (int x = 0; x < oCardIcons.Count(); x++)
                {
                    for (int y = 0; y < oCardIcons[x].Count(); y++)
                    {
                        oCardIcons[x][y].Center = new Vector2(15f + (oCardIcons[x][y].Width + 0.3f) * x, 6.3f + 0.8f * y);
                    }
                }
            }
            PositionCoinIcons();
        }

        public void GetCoins(int nIndex, int nCoinAmount)
        {
            nOwnedCoins[nIndex] += nCoinAmount;

            if (oBot.GetType() != typeof(PlayerBot))
            {
                XNACS1Rectangle oNewCoin;
                while (nOwnedCoins[nIndex] > oCoinIcons[nIndex].Count())
                {
                    oNewCoin = new XNACS1Rectangle(new Vector2(), MyLibrary.Resize(18), MyLibrary.Resize(18));
                    oCoinIcons[nIndex].Add(oNewCoin);
                    oCoinIcons[nIndex].Last().Texture = "CoinIconTest";
                    //oCoinIcons[nIndex].Insert(0, oTemp);
                    //oCoinIcons[nIndex][0].Texture = "CoinIconTest";

                    if (nIndex == 0)
                        oNewCoin.TextureTintColor = new Color(230, 230, 230);
                    else if (nIndex == 1)
                        oNewCoin.TextureTintColor = new Color(62, 127, 190);
                    else if (nIndex == 2)
                        oNewCoin.TextureTintColor = new Color(62, 190, 86);
                    else if (nIndex == 3)
                        oNewCoin.TextureTintColor = new Color(190, 62, 62);
                    else if (nIndex == 4)
                        oNewCoin.TextureTintColor = new Color(70, 50, 28);
                    else if (nIndex == 5)
                        oNewCoin.TextureTintColor = Color.Gold;
                }
                while (nOwnedCoins[nIndex] < oCoinIcons[nIndex].Count())
                {
                    oCoinIcons[nIndex][0].RemoveFromAutoDrawSet();
                    oCoinIcons[nIndex].RemoveAt(0);
                }
            }
            else
            {
                XNACS1Rectangle oNewCoin;
                while (nOwnedCoins[nIndex] > oCoinIcons[nIndex].Count())
                {
                    oNewCoin = new XNACS1Rectangle(new Vector2(), MyLibrary.Resize(50), MyLibrary.Resize(34), "CoinIconWhite");

                    if (nIndex == 0)
                        oNewCoin.Texture = "CoinIconWhite";
                    else if (nIndex == 1)
                        oNewCoin.Texture = "CoinIconBlue";
                    else if (nIndex == 2)
                        oNewCoin.Texture = "CoinIconGreen";
                    else if (nIndex == 3)
                        oNewCoin.Texture = "CoinIconRed";
                    else if (nIndex == 4)
                        oNewCoin.Texture = "CoinIconBlack";
                    else if (nIndex == 5)
                        oNewCoin.Texture = "CoinIconGold";

                    oCoinIcons[nIndex].Add(oNewCoin);
                }
                while (nOwnedCoins[nIndex] < oCoinIcons[nIndex].Count())
                {
                    oCoinIcons[nIndex][0].RemoveFromAutoDrawSet();
                    oCoinIcons[nIndex].RemoveAt(0);
                }

                RedrawBox();
            }

            PositionCoinIcons();
        }
        public void PositionCoinIcons()
        {
            if (oBot.GetType() != typeof(PlayerBot))
            {
                for (int x = 0; x < oCoinIcons.Count(); x++)
                {
                    for (int y = 0; y < oCoinIcons[x].Count(); y++)
                    {
                        //if (x != 5)
                        //    if (oCardIcons[x].Count() > 0)
                        //        oCoinIcons[x][y].Center = oBox.Center + oCardIconOffset + new Vector2(oSpaceBetweenCoins.X * x, -oSpaceBetweenCoins.Y * (y + 2 + oCardIcons[x].Count()));
                        //    else
                        //        oCoinIcons[x][y].Center = oBox.Center + oCardIconOffset + new Vector2(oSpaceBetweenCoins.X * x, -oSpaceBetweenCoins.Y * (y + oCardIcons[x].Count()));
                        //else
                        //    oCoinIcons[x][y].Center = oBox.Center + oCardIconOffset + new Vector2(oSpaceBetweenCoins.X * x, -oSpaceBetweenCoins.Y * y);

                        if (x != 5)
                            if (oCardIcons[x].Count() > 0)
                                oCoinIcons[x][y].Center = oBox.Center + oCardIconOffset + new Vector2(oSpaceBetweenCoins.X * y + oCardIcons[x].Count() + 1, -oSpaceBetweenCoins.Y * x);
                            else
                                oCoinIcons[x][y].Center = oBox.Center + oCardIconOffset + new Vector2(oSpaceBetweenCoins.X * y, -oSpaceBetweenCoins.Y * x);
                        else
                            oCoinIcons[x][y].Center = oBox.Center + oCardIconOffset + new Vector2(oSpaceBetweenCoins.X * y, -oSpaceBetweenCoins.Y * x);
                    }
                }
            }
            else
            {
                for (int x = 0; x < oCoinIcons.Count(); x++)
                {
                    for (int y = 0; y < oCoinIcons[x].Count(); y++)
                    {
                        oCoinIcons[x][y].Center = new Vector2(55f + (oCoinIcons[x][y].Width + 0.35f) * x, 3f + 0.5f * y);
                    }
                }
            }
        }

        public void ResetTurn()
        {
            oBot.ResetTurn();
            oAvatar.SetTextureSpriteAnimationFrames(0, 0, 0, 0, 5, SpriteSheetAnimationMode.AnimateBackward);
            oMessageBox.Visible = false;
        } 

        public int Points()
        {
            int nToReturn = nPoints;
            return nToReturn;
        }

        public int CoinTotal()
        {
            return nOwnedCoins[0] + nOwnedCoins[1] + nOwnedCoins[2] + nOwnedCoins[3] + nOwnedCoins[4] + nOwnedCoins[5];
        }

        public int CardTotal()
        {
            return nOwnedCards[0] + nOwnedCards[1] + nOwnedCards[2] + nOwnedCards[3] + nOwnedCards[4];
        }

        public int GetMyID()
        {
            return oBot.GetMyID();
        }

        public string GetName()
        {
            return oBot.Name();
        }

        public int[] OwnedCards()
        {
            int[] arrnToReturn = new int[5];
            nOwnedCards.CopyTo(arrnToReturn, 0);
            return arrnToReturn;
        }

        public int[] OwnedCoins()
        {
            int[] arrnToReturn = new int[6];
            nOwnedCoins.CopyTo(arrnToReturn, 0);
            return arrnToReturn;
        }
    }
}
