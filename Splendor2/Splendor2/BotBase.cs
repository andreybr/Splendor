using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using XNACS1Lib;
using Microsoft.Xna.Framework;

namespace Splendor
{
    public abstract class BotBase
    {
        protected List<Card>[] oCardStack;
        protected List<Card>[] oPickedUpCards;
        private List<Noble> oNobleStack;
        protected XNACS1Rectangle[] oCardBacks;
        private XNACS1Rectangle[] oCardCounts;
        private Player[] oPlayers;
        private ActionLog oActionLog;
        private int[] nCoinsOnBoard;
        private Stats oStats;
        private PlayerMessage oPlayerMessage;
        protected XNACS1Rectangle oCoinActionMenu;
        protected Cursor oCursor;
        protected XNACS1Rectangle[] oCoinStack;

        private int nMaxNumberOfLogs = 17;
        private int nPlayerNumber;
        private XNACS1Rectangle oMessageBox;
        public bool bTurnTaken = false;

        public TurnInfo oTurnInfo;

        public BotBase(List<Card>[] oNewCardStack, List<Card>[] oNewPickedUpCards, List<Noble> oNewNobleStack, Player[] oNewPlayers, 
                       ActionLog oNewActionLog, int[] nNewCoinCount, XNACS1Rectangle[] oNewCardBacks, XNACS1Rectangle[] oNewCardCounts,
                       Stats oNewStats, XNACS1Rectangle oNewCoinActionMenu, Cursor oNewCursor, XNACS1Rectangle[] oNewCoinStack, TurnInfo oNewTurnInfo)
        {
            oCardStack = oNewCardStack;      
            oPickedUpCards = oNewPickedUpCards;
            oNobleStack = oNewNobleStack;
            oPlayers = oNewPlayers;
            oActionLog = oNewActionLog;
            nCoinsOnBoard = nNewCoinCount;
            oCardBacks = oNewCardBacks;
            oCardCounts = oNewCardCounts;
            oStats = oNewStats;
            oCoinActionMenu = oNewCoinActionMenu;
            oCursor = oNewCursor;
            oCoinStack = oNewCoinStack;
            oTurnInfo = oNewTurnInfo;
        }

        public abstract void TakeTurn();
        public abstract void ReactToMove(string sName, int nPlayerID, TurnInfo.ActionType eAction, int[] arrnSelectedCoins, Card oCard);
        public abstract string Name();
        public abstract string Picture();

        public bool HasTakenTurn()
        {
            bool bTurn = bTurnTaken;
            return bTurn;
        }
        public void ResetTurn()
        {
            bTurnTaken = false;
        }
        public void SetPlayerNumber(int nNewPlayerNumber)
        {
            nPlayerNumber = nNewPlayerNumber;
        }

        protected int NumberOfPlayers()
        {
            int nToReturn = oStats.nNumberOfPlayers;
            return nToReturn;
        }
        protected int CurrentRound()
        {
            int nToReturn = oStats.nCurrentRound;
            return nToReturn;
        }
        public int GetMyID()
        {
            int nToReturn = nPlayerNumber;
            return nToReturn;
        }
        protected int PlayerPoints(int nID)
        {
            return oPlayers[nID].nPoints;
        }

        protected int CardsInDeck(int nTier)
        {
            if (oCardStack[nTier].Count < 4)
                return 0;
            else
                return oCardStack[nTier].Count - 4;
        }
        protected int CardsOnBoard(int nTier)
        {
            if (oCardStack[nTier].Count < 4)
                return oCardStack[nTier].Count;
            else
                return 4;
        }
        protected int CardsInTier(int nTier) 
        {
            return oCardStack[nTier].Count;
        }

        protected int CountNobles()
        {
            int nToReturn = oStats.nNobleCount;
            return nToReturn;
        }
        protected int CountNoblesAvailable()
        {
            int nTotal = 0;

            for (int x = 0; x < oStats.nNobleCount; x++)
                if (oNobleStack[x].IsAvailable() == true)
                    nTotal++;

            return nTotal;
        }
        protected Noble CheckNoble(int nCardNum)
        {
            if (nCardNum <= oStats.nNobleCount - 1)
            {
                Noble oToReturn = oNobleStack[nCardNum].Copy();
                oToReturn.Center = new Vector2(-50f, -50f);
                return oToReturn;
            }
            else
                return null;
        }

        protected int CheckPlayerPoints(int nID)
        {
            return oPlayers[nID].Points();
        }
        protected int CountCardsHeld(int nPlayerID)
        {
            int nToReturn = oPickedUpCards[nPlayerID].Count();
            return nToReturn;
        }

        protected int[] CountCardsBought(int nPlayerID) 
        {
            return oPlayers[nPlayerID].OwnedCards();
        }
        protected int CountNetCardsBought(int nPlayerID)
        {         
            return ArrayTotal(oPlayers[nPlayerID].OwnedCards());
        }
    
        protected int[] CountCoinsHeld(int nPlayerID)
        {
            return oPlayers[nPlayerID].OwnedCoins();
        }
        protected int CountNetCoinsHeld(int nPlayerID)
        {
            int nToReturn = 0;
            for (int x = 0; x < oPlayers[nPlayerID].nOwnedCoins.Count(); x++)
                nToReturn += oPlayers[nPlayerID].nOwnedCoins[x];

            return nToReturn;
        }
        protected Card CheckCardFromBoard(int nCardNum, int nTier) // remember to fix
        {
            if (nCardNum < oCardStack[nTier].Count())
            {
                Card oToReturn = oCardStack[nTier][nCardNum].Copy();
                return oToReturn;
            }
            else
            {
                return null;
            }
            
        }
        protected Card CheckCardFromHand(int nCardNum)
        {
            Card oToReturn = oPickedUpCards[nPlayerNumber][nCardNum].Copy();
            return oPickedUpCards[nPlayerNumber][nCardNum];
        }

        protected int[] CoinsRemaining()
        {
            int[] nToReturn = { nCoinsOnBoard[0], nCoinsOnBoard[1], nCoinsOnBoard[2], nCoinsOnBoard[3], nCoinsOnBoard[4], nCoinsOnBoard[5] };
            return nToReturn;
        }
        protected int[] CoinsMax()
        {
            int[] nToReturn = { oStats.nMaxRegularCoins, oStats.nMaxRegularCoins, oStats.nMaxRegularCoins, oStats.nMaxRegularCoins, oStats.nMaxRegularCoins, oStats.nMaxGoldCoins };
            return nToReturn;
        }

        protected int[] SumResources(int nID)
        {
            int[] arrnResources = new int[6];

            for (int x = 0; x < 6; x++)
            {
                if (x == 5)
                    arrnResources[x] = CountCoinsHeld(nID)[x];
                else
                    arrnResources[x] = CountCoinsHeld(nID)[x] + CountCardsBought(nID)[x];
            }

            return arrnResources;
        }
        protected int ArrayTotal(int[] arrnArray)
        {
            int nTotal = 0;

            for (int x = 0; x < arrnArray.Count(); x++)
            {
                nTotal += arrnArray[x];
            }

            return nTotal;
        }
        protected List<Card> CardsReservedByPlayer(int nID)
        {
            List<Card> oReservedCards = new List<Card>();

            for (int x = 0; x < oPickedUpCards[nID].Count(); x++)
            {
                oReservedCards.Add(oPickedUpCards[nID][x].Copy());
            }

            return oReservedCards;
        }

        // actions
        protected bool PickUpCoins(int[] nSelectedCoins)
        {
            if (bTurnTaken == false)
            {
                int nSelectedTotal = nSelectedCoins[0] + nSelectedCoins[1] + nSelectedCoins[2] + nSelectedCoins[3] + nSelectedCoins[4];

                if (ArrayTotal(nSelectedCoins) == 0)
                {
                    AddToLog("[" + oPlayers[oStats.nCurrentPlayer].strName + "] tried to pick up 0 coins", Color.Red);
                    return false;
                }

                if (oPlayers[oStats.nCurrentPlayer].CoinTotal() + nSelectedTotal > oStats.nMaxCoinsHeld) // more coins than can hold
                {
                    AddToLog("[" + oPlayers[oStats.nCurrentPlayer].strName + "] tried to pick up more coins than he/she can hold ["
                                     + nSelectedCoins[0] + +nSelectedCoins[1] + +nSelectedCoins[2] + +nSelectedCoins[3] + +nSelectedCoins[4] + "]", Color.Red);
                    return false;
                }

                for (int x = 0; x < 5; x++) // negative values
                {
                    if (nSelectedCoins[x] < 0)
                    {
                        AddToLog("[" + oPlayers[oStats.nCurrentPlayer].strName + "] tried to pick up negative coins ["
                                     + nSelectedCoins[0] + +nSelectedCoins[1] + +nSelectedCoins[2] + +nSelectedCoins[3] + +nSelectedCoins[4] + "]", Color.Red);
                        return false;
                    }
                }

                for(int x = 0; x < 5; x++) // not enough coins on board
                {
                    if (nSelectedCoins[x] > CoinsRemaining()[x])
                    {
                        AddToLog("[" + oPlayers[oStats.nCurrentPlayer].strName + "] tried to pick up coins that aren't available ["
                                     + nSelectedCoins[0] + +nSelectedCoins[1] + +nSelectedCoins[2] + +nSelectedCoins[3] + +nSelectedCoins[4] + "]", Color.Red);
                        return false;
                    }
                }

                if (nSelectedTotal > 3) // picking up too many
                {
                    AddToLog("[" + oPlayers[oStats.nCurrentPlayer].strName + "] tried to pick up too many coins(" + nSelectedTotal + ")", Color.Red);
                    return false;
                }
                else if (nSelectedTotal == 3) // invalid combination
                {
                    for (int x = 0; x < 5; x++)
                    {
                        if (nSelectedCoins[x] > 1)
                        {
                            AddToLog("[" + oPlayers[oStats.nCurrentPlayer].strName + "] tried to pick up an invalid combination of coins ["
                                     + nSelectedCoins[0] + +nSelectedCoins[1] + +nSelectedCoins[2] + +nSelectedCoins[3] + +nSelectedCoins[4] + "]",  Color.Red);
                            return false;
                        }
                    }
                }
                else if (nSelectedTotal == 2) // 4 coin rule
                {
                    for (int x = 0; x < 5; x++)
                    {
                        if (nSelectedCoins[x] == 2 && CoinsRemaining()[x] < 4)
                        {
                            AddToLog("[" + oPlayers[oStats.nCurrentPlayer].strName + "] tried to break the 4 coin rule", Color.Red);
                            return false;
                        }
                    }
                }

                // coin pick up succesful
                string strLog = "[" + oPlayers[oStats.nCurrentPlayer].strName + "] picked up ";


                for (int x = 0; x < 5; x++)
                {
                    if (nSelectedCoins[x] > 0)
                    {
                        strLog += "" + nSelectedCoins[x] + " " + (Card.TYPE)x + " ";
                        //oPlayers[oStats.nCurrentPlayer].nOwnedCoins[x] += nSelectedCoins[x];
                        //nCoinsOnBoard[x] -= nSelectedCoins[x];
                        //nSelectedCoins[x] = 0;
                    }                                  
                }

                oTurnInfo.eActionType = TurnInfo.ActionType.COINS;
                oTurnInfo.strAction = "pick up coins";
                oTurnInfo.oTarget.X = -1;
                oTurnInfo.oTarget.Y = -1;

                for(int x = 0; x < 5; x++)
                    oTurnInfo.nSelectedCoins[x] = nSelectedCoins[x];

                strLog += "coin(s)";
                AddToLog(strLog, Color.LightGreen);
                bTurnTaken = true;
                return true;          
            }
            else
            {
                //AddToLog("[" + oPlayers[oStats.nCurrentPlayer].strName + "] already took a turn(pick up coins)", Color.Red);
                return false;
            }
        }
        protected bool BuyCardFromBoard(int nCardNum, int nTier)
        {
            if (bTurnTaken == false)
            {
                if (nTier > 2 || nTier < 0 || nCardNum < 0 || nCardNum > 3)
                {
                    AddToLog("[" + oPlayers[GetMyID()].strName + "] tried to buy a card(board) with an invalid index(" + nCardNum + "," + nTier + ")", Color.Red);
                    return false;
                }

                if (CardsOnBoard(nTier) - 1 < nCardNum)
                {
                    AddToLog("[" + oPlayers[GetMyID()].strName + "] tried to buy a card(board) that doesn't exist", Color.Red);
                    return false;
                }

                // calculate how many gold coins needed
                int nLeftOverCost = 0;
                for (int x = 0; x < 5; x++)
                {
                    if ((oPlayers[GetMyID()].nOwnedCards[x] + oPlayers[GetMyID()].nOwnedCoins[x]) < oCardStack[nTier][nCardNum].Cost()[x])
                        nLeftOverCost += oCardStack[nTier][nCardNum].Cost()[x] - (oPlayers[GetMyID()].nOwnedCards[x] + oPlayers[GetMyID()].nOwnedCoins[x]);
                }

                // calculate if enough gold coins
                if (oPlayers[GetMyID()].nOwnedCoins[5] >= nLeftOverCost)
                {
                    //int nCost;
                    //for (int x = 0; x < 5; x++)
                    //{
                    //    nCost = oCardStack[nTier][nCardNum].Cost()[x] - oPlayers[GetMyID()].nOwnedCards[x];

                    //    if (nCost <= 0)
                    //        continue;

                    //    if (nCost > oPlayers[GetMyID()].nOwnedCoins[x])
                    //    {
                    //        nCoinsOnBoard[x] += oPlayers[GetMyID()].nOwnedCoins[x];
                    //        nCoinsOnBoard[5] += (nCost - oPlayers[GetMyID()].nOwnedCoins[x]);
                    //        oPlayers[GetMyID()].nOwnedCoins[5] -= (nCost - oPlayers[GetMyID()].nOwnedCoins[x]);
                    //        oPlayers[GetMyID()].nOwnedCoins[x] = 0;
                    //    }
                    //    else if (nCost <= oPlayers[GetMyID()].nOwnedCoins[x])
                    //    {
                    //        nCoinsOnBoard[x] += nCost;
                    //        oPlayers[GetMyID()].nOwnedCoins[x] -= nCost;
                    //    }
                    //}

                    //oPlayers[GetMyID()].nPoints += oCardStack[nTier][nCardNum].Points();
                    //oPlayers[GetMyID()].nOwnedCards[(int)oCardStack[nTier][nCardNum].Type()]++;
                    //oCardStack[nTier][nCardNum].Clear();
                    //oCardStack[nTier].RemoveAt(nCardNum);

                    //if (oCardStack[nTier].Count == 4)
                    //{
                    //    oCardBacks[nTier].TextureTintColor = new Color(50, 50, 50, 50);
                    //    oCardCounts[nTier].RemoveFromAutoDrawSet();
                    //    oCardCounts[nTier] = null;
                    //}

                    oTurnInfo.eActionType = TurnInfo.ActionType.BUY_BOARD;
                    oTurnInfo.strAction = "buy from board";
                    oTurnInfo.oTarget.X = nCardNum;
                    oTurnInfo.oTarget.Y = nTier;
                    oTurnInfo.nSelectedCoins = new int[] { 0, 0, 0, 0, 0 };

                    AddToLog("[" + oPlayers[GetMyID()].strName + "] bought a tier " + nTier + " card from the board", Color.SteelBlue);
                    bTurnTaken = true;
                    return true;
                }
                else
                {
                    AddToLog("[" + oPlayers[GetMyID()].strName + "] could't afford a card(" + nCardNum + "," + nTier + ") from the board", Color.Red);   
                    return false;
                }
            }
            else
            {
                //AddToLog("[" + oPlayers[oStats.nCurrentPlayer].strName + "] already took a turn(buy card from board)", Color.Red);
                return false;
            }      
        }
        protected bool BuyCardFromHand(int nCardNum)
        {
            if (bTurnTaken == false)
            {
                if (nCardNum < 0 || nCardNum > 3) // valid index
                {
                    AddToLog("[" + oPlayers[oStats.nCurrentPlayer].strName + "] tried to buy a card(hand) with an invalid index(" + nCardNum + ")", Color.Red);
                    return false;
                }

                if (CountCardsHeld(GetMyID()) - 1 < nCardNum) 
                {
                    AddToLog("[" + oPlayers[oStats.nCurrentPlayer].strName + "] tried to buy a card(hand) that doesn't exist", Color.Red);
                    return false;
                }

                // calculate how many gold coins needed
                int nLeftOverCost = 0;
                for (int x = 0; x < 5; x++)
                {
                    if ((oPlayers[oStats.nCurrentPlayer].nOwnedCards[x] + oPlayers[oStats.nCurrentPlayer].nOwnedCoins[x]) - oPickedUpCards[oStats.nCurrentPlayer][nCardNum].Cost()[x] < 0)
                        nLeftOverCost -= (oPlayers[oStats.nCurrentPlayer].nOwnedCards[x] + oPlayers[oStats.nCurrentPlayer].nOwnedCoins[x]) - oPickedUpCards[oStats.nCurrentPlayer][nCardNum].Cost()[x];
                }

                if (oPlayers[oStats.nCurrentPlayer].nOwnedCoins[5] >= nLeftOverCost)
                {
                    //int nCost;
                    //for (int x = 0; x < 5; x++)
                    //{
                    //    nCost = oPickedUpCards[oStats.nCurrentPlayer][nCardNum].Cost()[x] - oPlayers[oStats.nCurrentPlayer].nOwnedCards[x];

                    //    if (nCost <= 0)
                    //        continue;

                    //    if (nCost > oPlayers[oStats.nCurrentPlayer].nOwnedCoins[x])
                    //    {
                    //        nCoinsOnBoard[x] += oPlayers[oStats.nCurrentPlayer].nOwnedCoins[x];
                    //        nCoinsOnBoard[5] += (nCost - oPlayers[oStats.nCurrentPlayer].nOwnedCoins[x]);
                    //        oPlayers[oStats.nCurrentPlayer].nOwnedCoins[5] -= (nCost - oPlayers[oStats.nCurrentPlayer].nOwnedCoins[x]);
                    //        oPlayers[oStats.nCurrentPlayer].nOwnedCoins[x] = 0;
                    //    }
                    //    else if (nCost <= oPlayers[oStats.nCurrentPlayer].nOwnedCoins[x])
                    //    {
                    //        nCoinsOnBoard[x] += nCost;
                    //        oPlayers[oStats.nCurrentPlayer].nOwnedCoins[x] -= nCost;
                    //    }
                    //}

                    //oPlayers[oStats.nCurrentPlayer].nPoints += oPickedUpCards[oStats.nCurrentPlayer][nCardNum].Points();
                    //oPlayers[oStats.nCurrentPlayer].nOwnedCards[(int)oPickedUpCards[oStats.nCurrentPlayer][nCardNum].Type()]++;
                    //oPickedUpCards[oStats.nCurrentPlayer][nCardNum].Clear();
                    //oPickedUpCards[oStats.nCurrentPlayer].RemoveAt(nCardNum);

                    oTurnInfo.eActionType = TurnInfo.ActionType.BUY_HAND;
                    oTurnInfo.strAction = "buy from hand";
                    oTurnInfo.oTarget.X = nCardNum;
                    oTurnInfo.oTarget.Y = -1;
                    oTurnInfo.nSelectedCoins = new int[] { 0, 0, 0, 0, 0 };

                    AddToLog("[" + oPlayers[oStats.nCurrentPlayer].strName + "] bought a card from his hand", Color.Teal);
                    bTurnTaken = true;
                    return true;
                }
                else
                {
                    AddToLog("[" + oPlayers[oStats.nCurrentPlayer].strName + "] could't afford a card(" + nCardNum + ") from his/her hand ", Color.Red);                   
                    return false;
                }
            }
            else
            {
                //AddToLog("[" + oPlayers[oStats.nCurrentPlayer].strName + "] already took a turn(buy card from hand)", Color.Red);
                return false;
            }  
        }
        protected bool ReserveCardFromDeck(int nTier)
        {
            // does not check for hand max
            if (bTurnTaken == false)
            {
                if (nTier < 0 || nTier > 2) // valid index
                {
                    AddToLog("[" + oPlayers[oStats.nCurrentPlayer].strName + "] tried to reserve from invalid deck index(" + nTier + ")", Color.Red);
                    return false;
                }

                if (oCardStack[nTier].Count > 4 && oPickedUpCards[GetMyID()].Count < MyLibrary.nMaxHandSize)
                {
                    //if (oPlayers[oStats.nCurrentPlayer].CoinTotal() < 10 && nCoinsOnBoard[5] > 0)
                    //{
                    //    oPlayers[oStats.nCurrentPlayer].nOwnedCoins[5]++;
                    //    nCoinsOnBoard[5]--;
                    //}

                    //oPickedUpCards[oStats.nCurrentPlayer].Add(oCardStack[nTier][4]);
                    //oCardStack[nTier].RemoveAt(4);

                    //if (oCardStack[nTier].Count == 4)
                    //{
                    //    oCardBacks[nTier].TextureTintColor = new Color(50, 50, 50, 50);
                    //    oCardCounts[nTier].RemoveFromAutoDrawSet();
                    //    oCardCounts[nTier] = null;
                    //}

                    oTurnInfo.eActionType = TurnInfo.ActionType.RESERVE_DECK;
                    oTurnInfo.strAction = "reserve from deck";
                    oTurnInfo.oTarget.X = -1;
                    oTurnInfo.oTarget.Y = nTier;
                    oTurnInfo.nSelectedCoins = new int[] { 0, 0, 0, 0, 0 };

                    AddToLog("[" + oPlayers[oStats.nCurrentPlayer].strName + "] reserved a tier " + nTier + " card from the deck", Color.Orange);
                    bTurnTaken = true;
                    return true;
                }
                else
                {
                    AddToLog("[" + oPlayers[oStats.nCurrentPlayer].strName + "] failed to reserve from empty deck(" + nTier + ")", Color.Red);
                    return false;
                }
            }
            else
            {
                //AddToLog("[" + oPlayers[oStats.nCurrentPlayer].strName + "] already took a turn(reserve from deck)", Color.Red);
                return false;
            }  
        }
        protected bool ReserveCardFromBoard(int nCardNum, int nTier)
        {
            if (bTurnTaken == false)
            {
                if (nTier > 2 || nTier < 0 || nCardNum < 0 || nCardNum > 3)
                {
                    AddToLog("[" + oPlayers[oStats.nCurrentPlayer].strName + "] tried to reserve a card(board) with an invalid index(" + nCardNum + "," + nTier + ")", Color.Red);
                    return false;
                }

                if (oCardStack[nTier].Count - 1 >= nCardNum && oPickedUpCards[GetMyID()].Count < MyLibrary.nMaxHandSize)
                {
                    //// give gold coin if possible
                    //if (oPlayers[oStats.nCurrentPlayer].CoinTotal() < 10 && nCoinsOnBoard[5] > 0)
                    //{
                    //    oPlayers[oStats.nCurrentPlayer].nOwnedCoins[5]++;
                    //    nCoinsOnBoard[5]--;
                    //}

                    //// add card to player hand and remove from board
                    //oPickedUpCards[oStats.nCurrentPlayer].Add(oCardStack[nTier][nCardNum]);
                    //oCardStack[nTier].RemoveAt(nCardNum);

                    //// show that deck is empty
                    //if (oCardStack[nTier].Count == 4)
                    //{
                    //    oCardBacks[nTier].TextureTintColor = new Color(50, 50, 50, 50);
                    //    oCardCounts[nTier].RemoveFromAutoDrawSet();
                    //    oCardCounts[nTier] = null;
                    //}

                    oTurnInfo.eActionType = TurnInfo.ActionType.RESERVE_BOARD;
                    oTurnInfo.strAction = "reserve from board";
                    oTurnInfo.oTarget.X = nCardNum;
                    oTurnInfo.oTarget.Y = nTier;
                    oTurnInfo.nSelectedCoins = new int[] { 0, 0, 0, 0, 0 };

                    AddToLog("[" + oPlayers[GetMyID()].strName + "] reserved a tier " + nTier + " card from the board", Color.Gold);
                    bTurnTaken = true;
                    return true;
                }
                else
                {
                    AddToLog("[" + oPlayers[oStats.nCurrentPlayer].strName + "] failed to reserve a card from the board", Color.Red);
                    return false;
                }
            }
            else
            {
                //AddToLog("[" + oPlayers[oStats.nCurrentPlayer].strName + "] already took a turn(reserve from board)", Color.Red);
                return false;
            }
        }

        // other
        protected void ShowAvatar(int nAvatarIndex)
        {
            oPlayers[GetMyID()].SetAvatar(nAvatarIndex);
        }
        protected void ShowMessage(string strMessage)
        {
            oPlayers[GetMyID()].ShowMessage(strMessage);
        }
        private void AddToLog(string strAction, Color oTextColor)
        {
            oActionLog.AddLog(strAction, oTextColor);

            //while (strAction.Length < 100)
            //    strAction += " ";

            //oActionLog.Add(new XNACS1Rectangle(new Vector2(), 70f, 1.5f));
            //oActionLog.Last().Color = new Color(0, 0, 0, 150);
            ////oActionLog.Last().Texture = "Empty";
            //oActionLog.Last().Label = strAction;
            //oActionLog.Last().LabelFont = "Log";
            //oActionLog.Last().LabelColor = oTextColor;

            //if (oActionLog.Count > nMaxNumberOfLogs)
            //{
            //    oActionLog[0].RemoveFromAutoDrawSet();
            //    oActionLog.RemoveAt(0);
            //}

            //for (int x = 0; x < oActionLog.Count(); x++)
            //{
            //    if (x == 0)
            //        oActionLog[x].Center = new Vector2(MyLibrary.Resize(1120), MyLibrary.Resize(566) - x * MyLibrary.Resize(30));
            //    else
            //        oActionLog[x].Center = new Vector2(oActionLog[x - 1].CenterX, oActionLog[x - 1].CenterY - (oActionLog[x].Height + 0.37f));
            //}
        }
    }
}