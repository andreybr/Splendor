using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNACS1Lib;
using Microsoft.Xna.Framework;

namespace Splendor
{
    public class JerryBot : BotBase
    {
        public JerryBot(List<Card>[] oNewCardStack, List<Card>[] oNewPickedUpCards, List<Noble> oNewNobleStack, Player[] oNewPlayers,
                         ActionLog oNewActionLog, int[] nNewCoinCount, XNACS1Rectangle[] oNewCardBacks, XNACS1Rectangle[] oNewCardCounts,
                         Stats oNewStats, XNACS1Rectangle oNewCoinActionMenu,
                         Cursor oNewCursor, XNACS1Rectangle[] oNewCoinStack, TurnInfo oNewTurnInfo)
                         : base(oNewCardStack, oNewPickedUpCards, oNewNobleStack, oNewPlayers, oNewActionLog, nNewCoinCount, oNewCardBacks,
                                oNewCardCounts, oNewStats, oNewCoinActionMenu, oNewCursor, oNewCoinStack, oNewTurnInfo)
        {}

        public override void TakeTurn()
        {
            int coinTotal = CountCoinTotal(GetMyID()); //my total amount of coins I start with this turn
            int[] cardsCloseTo = PopulateCardsCloseTo(); //contains how many more resources I need for each card on the board
            int[] reservedCardsCloseTo = PopulateReservedCardsCloseTo(); //contains how many more resources I need for each card in my hand
            Card bestCardToBuy = DetermineBestCardToBuy(cardsCloseTo, reservedCardsCloseTo);
            Card bestCardCloseToBuying = DetermineBestCardCloseToBuying(cardsCloseTo, reservedCardsCloseTo);
            List<Card> myReservedCards = CardsReservedByPlayer(GetMyID()); //given that this exists

            //if (bestCardToBuy != null && bestCardCloseToBuying != null)
            //    XNACS1Base.EchoToTopStatus("Jerry: best" + bestCardToBuy.Tier() + " " + bestCardToBuy.Points() + " close: " + bestCardToBuy.Type() + "      " +
            //                                          bestCardCloseToBuying.Tier() + " " + bestCardCloseToBuying.Points() + " " + bestCardCloseToBuying.Type());
            //else if (bestCardCloseToBuying != null)
            //    XNACS1Base.EchoToTopStatus("Jerry:  close: " + bestCardCloseToBuying.Tier() + " " + bestCardCloseToBuying.Points() + " " + bestCardCloseToBuying.Type());
            //else if (bestCardToBuy != null)
            //    XNACS1Base.EchoToTopStatus("Jerry:  best: " + bestCardToBuy.Tier() + " " + bestCardToBuy.Points() + " " + bestCardToBuy.Type());
            //else
            //    XNACS1Base.EchoToTopStatus("Jerry:");

            if (bestCardToBuy != null && bestCardCloseToBuying != null)
            { //can buy something and close to buying something else
                if (coinTotal < 8)
                {
                    if (bestCardToBuy.Points() + 2 < bestCardCloseToBuying.Points())
                    {
                        if (ReserveCardFromBoard(FindCardNum(bestCardCloseToBuying), FindCardTier(bestCardCloseToBuying)))
                        {
                            return;
                        }
                        else if (GetCoins(bestCardCloseToBuying))
                        {
                            return;
                        }
                        else
                        {
                            ShowMessage("Somebody poisoned the waterhole!");
                        }
                    }
                    else if (bestCardToBuy.Points() + 1 < bestCardCloseToBuying.Points())
                    {
                        if (GetCoins(bestCardCloseToBuying))
                        {
                            return;
                        }
                        else
                        {
                            ShowMessage("I'm standing right behind you...");
                        }
                    }
                    else
                    {
                        if (BuyCardFromBoard(FindCardNum(bestCardToBuy), FindCardTier(bestCardToBuy)))
                        {
                            return;
                        }
                        else if (BuyCardFromHand(FindCardNum(bestCardToBuy)))
                        {
                            return;
                        }
                        else
                        {
                            ShowMessage("error 1<38008135.");
                        }
                    }
                }
                else
                {
                    if (BuyCardFromBoard(FindCardNum(bestCardToBuy), FindCardTier(bestCardToBuy)))
                    {
                        return;
                    }
                    else if ((BuyCardFromHand(FindCardNum(bestCardToBuy))))
                    {
                        return;
                    }
                    else
                    {
                        ShowMessage("This is some straight up bullshit..");
                    }
                }
            }
            else if (bestCardToBuy == null && bestCardCloseToBuying == null)
            { //can't buy anything and not close to buying anything

                int coinsNeededForCard = 100;
                int lowestCard = -1;

                //determine the card that I'm closest to obtaining from the cards on the board
                for (int i = 0; i < 12; i++)
                {
                    if (cardsCloseTo[i] <= coinsNeededForCard)
                    {
                        coinsNeededForCard = cardsCloseTo[i];
                        lowestCard = i;
                    }
                }

                //determine the card that I'm closest to obtaining from the cards in my hand
                for (int i = 0; i < myReservedCards.Count(); i++)
                {
                    if (reservedCardsCloseTo[i] <= coinsNeededForCard)
                    {
                        coinsNeededForCard = reservedCardsCloseTo[i];
                        lowestCard = 12 + i;
                    }
                }

                if (lowestCard < 4)
                {
                    if (GetCoins(CheckCardFromBoard(lowestCard, 0)))
                    {
                        return;
                    }
                    else
                    {
                        ShowMessage("Where was I? Oh ya!");
                    }
                }
                else if (lowestCard < 8)
                {
                    if (GetCoins(CheckCardFromBoard(lowestCard - 4, 1)))
                    {
                        return;
                    }
                    else
                    {
                        ShowMessage("Suck it Trebek!");
                    }
                }
                else if (lowestCard < 12)
                {
                    if (GetCoins(CheckCardFromBoard(lowestCard - 8, 2)))
                    {
                        return;
                    }
                    else
                    {
                        ShowMessage("(laughs at player to the left)");
                    }
                }
                else
                {
                    if (GetCoins(CheckCardFromHand(lowestCard - 12)))
                    {
                        return;
                    }
                    else
                    {
                        ShowMessage("(laughs at player to the right)");
                    }
                }
            }
            else if (bestCardCloseToBuying != null)
            { //can't buy anything, but close to buying something
                if (coinTotal < 8)
                {
                    if (GetCoins(bestCardCloseToBuying))
                    {
                        return;
                    }
                    else
                    {
                        ShowMessage("I thought you were my friend.");
                    }
                }
                else
                {
                    if (FindCardTier(bestCardCloseToBuying) != -1 && ReserveCardFromBoard(FindCardNum(bestCardCloseToBuying), FindCardTier(bestCardCloseToBuying)))
                    {
                        return;
                    }
                    else if (GetCoins(bestCardCloseToBuying))
                    {
                        return;
                    }
                    else
                    {
                        ShowMessage("I quit!");
                    }
                }
            }
            else
            { //can buy something and not close to buying anything else
                if (BuyCardFromBoard(FindCardNum(bestCardToBuy), FindCardTier(bestCardToBuy)))
                {
                    return;
                }
                else if (BuyCardFromHand(FindCardNum(bestCardToBuy)))
                {
                    return;
                }
                else
                {
                    ShowMessage("Hodor!!");
                }
            }       
        }

        public override void ReactToMove(string sName, int nPlayerID, TurnInfo.ActionType eAction, int[] arrnSelectedCoins, Card oCard)
        {         
            Random rand = new Random();
            int sNum = rand.Next(3);
            int mNum = rand.Next(7);

            //if (rand.Next() % 2 != 0)
            //    return;

            if (sNum == 0){

                if( eAction == TurnInfo.ActionType.COINS){
                    if (mNum == 0){
                        ShowMessage("What? Can't afford anything " + sName + "?");
                    }else if (mNum == 1){
                        ShowMessage(sName + " es un campeon del taco.");
                    }else if (mNum == 2){
                        ShowMessage("I think you picked up the wrong coins, " + sName + ".");
                    }else if (mNum == 3){
                        ShowMessage("I don't understand that move.");
                    }else if (mNum == 4){
                        ShowMessage(sName + " is an idiot.");
                    }else if (mNum == 5){
                        ShowMessage("You're such a coin piggy, " + sName + ".");            
                    }else{
                        ShowMessage("Crazy ass white people!");
                    }        
                }else if(eAction == TurnInfo.ActionType.BUY_BOARD){
                    if (oCard.Points() > 3){
                        mNum = 7;
                    }
                       
                    if (mNum == 0){
                        ShowMessage("Look at you " + sName + ". Being all productive and stuff.");
                    }else if (mNum == 1){
                        ShowMessage("(laughs at " + sName + ")");
                    }else if (mNum == 2){
                        ShowMessage("Give me that card, " + sName + "!");
                    }else if (mNum == 3){
                        ShowMessage("Do you like fishdicks, " + sName + "?");
                    }else if (mNum == 4){
                        ShowMessage("Now I'm not going to say that was a bad play, but I will think it.");
                    }else if (mNum == 5){
                        ShowMessage("...");            
                    }else if (mNum == 7){
                        ShowMessage("Wow, that was an expensive card!");
                    }else{
                        ShowMessage("I have a crush on you, " + sName);
                    }      
                }else if(eAction == TurnInfo.ActionType.BUY_HAND){
                    if (oCard.Points() > 3){
                            mNum = 7;
                    }

                    if (mNum == 0){
                        ShowMessage("Look at you " + sName + ". Being all productive and stuff.");
                    }else if (mNum == 1){
                        ShowMessage(sName + ", " + sName + ", " + sName + ". What am I going to do with you?");
                    }else if (mNum == 2){
                        ShowMessage("Give me that card, " + sName + "!");
                    }else if (mNum == 3){
                        ShowMessage("That was actually pretty smart.");
                    }else if (mNum == 4){
                        ShowMessage("If I had a nickle for every time I've seen that move...");
                    }else if (mNum == 5){
                        ShowMessage("What?");
                    }else if (mNum == 7){
                        ShowMessage("Wow, that was an expensive card!");                 
                    }else{
                        ShowMessage("Too bad that card wasn't worth many points.");
                    }     
                }else if(eAction == TurnInfo.ActionType.RESERVE_BOARD){
                    if (mNum == 0){
                        ShowMessage("Now that's just terrible strategy.");
                    }else if (mNum == 1){
                        ShowMessage("Reserving? Might as well just kill yourself!");
                    }else if (mNum == 2){
                        ShowMessage("Give me that card right now, " + sName + "!");   
                    }else if (mNum == 3){
                        ShowMessage("Did you say something, " + sName + "?"); 
                    }else if (mNum == 4){
                        ShowMessage("HA!");               
                    }else{
                        ShowMessage("What was that card? Can you flip it back over?");
                    } 
                }else if(eAction == TurnInfo.ActionType.RESERVE_DECK){
                    ShowMessage("People actually make that move?");
                }
            } 
        }

        public override string Picture()
        {
            return "FaceTest5";
        }
        /* -----------------------------------------------------------------------------------------------------
        ------------------------------------------------------------------------------------------------------*/

        //might need to check if CheckCardFromBoard(x,y) returns null when no card is in that spot
        private int[] PopulateCardsCloseTo()
        {
            int[] cardsCloseTo = new int[12];
            int[] resources = SumResources(GetMyID()); //contains the the amount of resources + coins I have for each color

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    for (int k = 0; k < 5; k++)
                    {
                        if (CheckCardFromBoard(j, i) != null)
                        {
                            if (CheckCardFromBoard(j, i) == null)
                                continue;

                            int[] cardCost = CheckCardFromBoard(j, i).Cost();
                            if (i == 0)
                            {
                                if (cardCost[k] - resources[k] > 0)
                                {
                                    cardsCloseTo[j] += cardCost[k] - resources[k];
                                }
                            }
                            else if (i == 1)
                            {
                                if (cardCost[k] - resources[k] > 0)
                                {
                                    cardsCloseTo[4 + j] += cardCost[k] - resources[k];
                                }
                            }
                            else if (i == 2)
                            {
                                if (cardCost[k] - resources[k] > 0)
                                {
                                    cardsCloseTo[8 + j] += cardCost[k] - resources[k];
                                }
                            }
                        }
                        else
                        {
                            if (i == 0)
                            {
                                cardsCloseTo[j] = 100;
                            }
                            else if (i == 1)
                            {
                                cardsCloseTo[4 + j] = 100;
                            }
                            else
                            {
                                cardsCloseTo[8 + j] = 100;
                            }
                        }

                    }
                }
            }

            if (CountCoinsHeld(GetMyID())[5] > 0)
            {
                for (int i = 0; i < 12; i++)
                {
                    if (cardsCloseTo[i] - CountCoinsHeld(GetMyID())[5] < 0)
                    {
                        cardsCloseTo[i] = 0;
                    }
                    else
                    {
                        cardsCloseTo[i] -= CountCoinsHeld(GetMyID())[5];
                    }
                }
            }

            return cardsCloseTo;
        }

        private int[] PopulateReservedCardsCloseTo()
        {
            int[] reservedCardsCloseTo = new int[3];
            List<Card> reservedCards = CardsReservedByPlayer(GetMyID());
            int[] resources = SumResources(GetMyID()); //contains the the amount of resources + coins I have for each color

            for (int i = 0; i < CountCardsHeld(GetMyID()); i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    int[] reservedCardCost = reservedCards[i].Cost();
                    if (reservedCardCost[j] - resources[j] > 0)
                    {
                        reservedCardsCloseTo[i] += reservedCardCost[j] - resources[j];
                    }
                }
            }

            if (CountCoinsHeld(GetMyID())[5] > 0)
            {
                for (int i = 0; i < CountCardsHeld(GetMyID()); i++)
                {
                    if (reservedCardsCloseTo[i] - CountCoinsHeld(GetMyID())[5] < 0)
                    {
                        reservedCardsCloseTo[i] = 0;
                    }
                    else
                    {
                        reservedCardsCloseTo[i] -= CountCoinsHeld(GetMyID())[5];
                    }
                }
            }

            return reservedCardsCloseTo;
        }

        //returns the best card I can currently buy
        private Card DetermineBestCardToBuy(int[] cardsCloseTo, int[] reservedCardsCloseTo)
        {
            Card bestCardToBuy = null;
            int bestCardPointValue = -1;

            for (int i = 0; i < 12; i++)
            {
                if (cardsCloseTo[i] == 0)
                {
                    if (i < 4)
                    {
                        if (CheckCardFromBoard(i, 0) == null)
                            continue;

                        if (CheckCardFromBoard(i, 0).Points() > bestCardPointValue)
                        {
                            bestCardPointValue = CheckCardFromBoard(i, 0).Points();
                            bestCardToBuy = CheckCardFromBoard(i, 0);
                        }
                    }
                    else if (i < 8)
                    {
                        if (CheckCardFromBoard(i - 4, 1) == null)
                            continue;

                        if (CheckCardFromBoard(i - 4, 1).Points() > bestCardPointValue)
                        {
                            bestCardPointValue = CheckCardFromBoard(i - 4, 1).Points();
                            bestCardToBuy = CheckCardFromBoard(i - 4, 1);
                        }
                    }
                    else
                    {
                        if (CheckCardFromBoard(i - 8, 2) == null)
                            continue;

                        if (CheckCardFromBoard(i - 8, 2).Points() > bestCardPointValue)
                        {
                            bestCardPointValue = CheckCardFromBoard(i - 8, 2).Points();
                            bestCardToBuy = CheckCardFromBoard(i - 8, 2);
                        }
                    }
                }
            }

            List<Card> reservedCards = CardsReservedByPlayer(GetMyID());
            for (int i = 0; i < CountCardsHeld(GetMyID()); i++)
            {
                if (reservedCardsCloseTo[i] == 0)
                {
                    if (reservedCards[i].Points() >= bestCardPointValue)
                    {
                        bestCardPointValue = reservedCards[i].Points();
                        bestCardToBuy = reservedCards[i];
                    }
                }
            }

            return bestCardToBuy;
        }

        //returns the best card I'm close to buying (can only be up to 3 resources away)
        private Card DetermineBestCardCloseToBuying(int[] cardsCloseTo, int[] reservedCardsCloseTo)
        {
            Card bestCardCloseToBuying = null;
            int bestCardCloseToBuyingPointValue = -1;

            for (int i = 0; i < 12; i++)
            {
                if (0 < cardsCloseTo[i] && cardsCloseTo[i] < 4)
                {
                    if (i < 4)
                    {
                        if (CheckCardFromBoard(i, 0) == null)
                            continue;

                        if (CheckCardFromBoard(i, 0).Points() > bestCardCloseToBuyingPointValue)
                        {
                            bestCardCloseToBuyingPointValue = CheckCardFromBoard(i, 0).Points();
                            bestCardCloseToBuying = CheckCardFromBoard(i, 0);
                        }
                    }
                    else if (i < 8)
                    {
                        if (CheckCardFromBoard(i - 4, 1) == null)
                            continue;

                        if (CheckCardFromBoard(i - 4, 1).Points() > bestCardCloseToBuyingPointValue)
                        {
                            bestCardCloseToBuyingPointValue = CheckCardFromBoard(i - 4, 1).Points();
                            bestCardCloseToBuying = CheckCardFromBoard(i - 4, 1);
                        }
                    }
                    else
                    {
                        if (CheckCardFromBoard(i - 8, 2) == null)
                            continue;

                        if (CheckCardFromBoard(i - 8, 2).Points() > bestCardCloseToBuyingPointValue)
                        {
                            bestCardCloseToBuyingPointValue = CheckCardFromBoard(i - 8, 2).Points();
                            bestCardCloseToBuying = CheckCardFromBoard(i - 8, 2);
                        }
                    }
                }
            }

            List<Card> reservedCards = CardsReservedByPlayer(GetMyID()); //given that this exists
            for (int i = 0; i < CountCardsHeld(GetMyID()); i++)
            {
                if (0 < reservedCardsCloseTo[i] && reservedCardsCloseTo[i] < 4)
                {
                    if (reservedCards[i].Points() >= bestCardCloseToBuyingPointValue)
                    {
                        bestCardCloseToBuyingPointValue = reservedCards[i].Points();
                        bestCardCloseToBuying = reservedCards[i];
                    }
                }
            }

            return bestCardCloseToBuying;
        }

        //returns the coin count for the given player
        private int CountCoinTotal(int playerID)
        {
            int[] coinArray = CountCoinsHeld(playerID);
            int sum = 0;

            for (int i = 0; i < 6; i++)
            {
                sum += coinArray[i];
            }

            return sum;
        }

        //returns the player's name
        public override string Name()
        {
            string name = "TP-CL4P";
            return name;
        }

        //returns the card's number (0-3)
        private int FindCardNum(Card card)
        {
            int[] cardCost = card.Cost();
            List<Card> myReservedCards = CardsReservedByPlayer(GetMyID()); //given that this exists

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (CheckCardFromBoard(j, i) == null)
                        continue;

                    int[] boardCardCost = CheckCardFromBoard(j, i).Cost();
                    if (cardCost[0] == boardCardCost[0] && cardCost[1] == boardCardCost[1] && cardCost[2] == boardCardCost[2] &&
                        cardCost[3] == boardCardCost[3] && cardCost[4] == boardCardCost[4])
                    {
                        return j;
                    }
                }
            }

            for (int i = 0; i < myReservedCards.Count; i++)
            {
                int[] reservedCardCost = CheckCardFromHand(i).Cost();
                if (cardCost[0] == reservedCardCost[0] && cardCost[1] == reservedCardCost[1] && cardCost[2] == reservedCardCost[2] &&
                    cardCost[3] == reservedCardCost[3] && cardCost[4] == reservedCardCost[4])
                {
                    return i;
                }
            }

            //this should never happen, just necessary for compilation
            return -1;
        }

        //returns the card's tier (0-2)
        private int FindCardTier(Card card)
        {
            int[] cardCost = card.Cost();

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (CheckCardFromBoard(j, i) == null)
                        continue;

                    int[] boardCardCost = CheckCardFromBoard(j, i).Cost();
                    if (cardCost[0] == boardCardCost[0] && cardCost[1] == boardCardCost[1] && cardCost[2] == boardCardCost[2] &&
                        cardCost[3] == boardCardCost[3] && cardCost[4] == boardCardCost[4])
                    {
                        return i;
                    }
                }
            }

            //this should never happen, just necessary for compilation
            return -1;
        }

        //will pick up coins for the given card
        private bool GetCoins(Card card)
        {
            int[] coinsOnBoard = CoinsRemaining(); //contains the amount of coins of each color remaining on the board
            int[] myCoins = CountCoinsHeld(GetMyID()); //contains the amount of coins I have of each color
            int[] coinsNeeded = new int[5]; //shows how many coins I need of each color to buy the card (-1 if the coin isn't available on the board)
            int[] cardResourceCost = card.Cost(); //the resources needed to buy the card
            int[] coinsToBuyInOrder = new int[5]; //orders/ranks which coins to get from most important to least important
            int[] coinAccountedFor = new int[5]; //0 if the coin is not accounted for, 1 otherwise (used for ordering coinsToBuyInOrder)
            int coinTotal = CountCoinTotal(GetMyID()); //the total amount of coins I have

            //checks what coins I need and how many of each and checks if they are obtainable
            for (int i = 0; i < 5; i++)
            {
                if (cardResourceCost[i] - myCoins[i] < 0)
                {
                    coinsNeeded[i] = 0;
                }
                else
                {
                    coinsNeeded[i] = cardResourceCost[i] - myCoins[i];
                }

                if (coinsOnBoard[i] == 0)
                {
                    coinsNeeded[i] = -1;
                }
            }

            //orders the coins to buy
            for (int i = 0; i < 5; i++)
            {
                int highestCoinAmt = -2;
                for (int j = 0; j < 5; j++)
                {
                    if (highestCoinAmt <= coinsNeeded[j])
                    {
                        if (coinAccountedFor[j] == 0)
                        {
                            highestCoinAmt = coinsNeeded[j];
                            coinsToBuyInOrder[i] = j;
                        }
                    }
                }
                coinAccountedFor[coinsToBuyInOrder[i]] = 1;
            }

            //XNACS1Base.EchoToTopStatus("Jerry:" + coinsToBuyInOrder[0] + "" + coinsToBuyInOrder[1] + "" + coinsToBuyInOrder[2] + "" + coinsToBuyInOrder[3] + "" + coinsToBuyInOrder[4] + "");
            int[] coinsTobuy = new int[5];

            for (int i = 0; i < 5; i++)
            {
                if (coinsNeeded[coinsToBuyInOrder[i]] == -1)
                {
                    coinsToBuyInOrder[i] = -1;
                }
            }

            if (coinTotal < 8)
            {
                if (coinsToBuyInOrder[2] == -1)
                {
                    if (coinsToBuyInOrder[1] == -1)
                    {
                        if (coinsToBuyInOrder[0] == -1)
                        {
                            ShowMessage("There are no coins? Not cool man.");
                            return false;
                        }
                        else
                        {
                            if (coinsOnBoard[coinsToBuyInOrder[0]] < 4)
                            {
                                coinsTobuy[coinsToBuyInOrder[0]] = 1;
                            }
                            else
                            {
                                coinsTobuy[coinsToBuyInOrder[0]] = 2;
                            }
                            if (PickUpCoins(coinsTobuy))
                            {
                                return true;
                            }
                            else
                            {
                                ShowMessage("This is fucked up, yo.");
                                return false;
                            }
                        }
                    }
                    else
                    {
                        coinsTobuy[coinsToBuyInOrder[0]] = 1;
                        coinsTobuy[coinsToBuyInOrder[1]] = 1;
                        if (PickUpCoins(coinsTobuy))
                        {
                            return true;
                        }
                        else
                        {
                            ShowMessage("This is fucked up, ho.");
                            return false;
                        }
                    }
                }
                else
                {
                    coinsTobuy[coinsToBuyInOrder[0]] = 1;
                    coinsTobuy[coinsToBuyInOrder[1]] = 1;
                    coinsTobuy[coinsToBuyInOrder[2]] = 1;
                    if (PickUpCoins(coinsTobuy))
                    {
                        return true;
                    }
                    else
                    {
                        ShowMessage("This is fucked up, bro.");
                        return false;
                    }
                }
            }
            else if (coinTotal == 8)
            {
                if (coinsToBuyInOrder[1] == -1)
                {
                    if (coinsToBuyInOrder[0] == -1)
                    {
                        ShowMessage("There are no coins? Not cool man.");
                        return false;
                    }
                    else
                    {
                        if (coinsOnBoard[coinsToBuyInOrder[0]] < 4)
                        {
                            coinsTobuy[coinsToBuyInOrder[0]] = 1;
                        }
                        else
                        {
                            coinsTobuy[coinsToBuyInOrder[0]] = 2;
                        }
                        if (PickUpCoins(coinsTobuy))
                        {
                            return true;
                        }
                        else
                        {
                            ShowMessage("This is fucked up, meng.");
                            return false;
                        }
                    }
                }
                else
                {
                    coinsTobuy[coinsToBuyInOrder[0]] = 1;
                    coinsTobuy[coinsToBuyInOrder[1]] = 1;
                    if (PickUpCoins(coinsTobuy))
                    {
                        return true;
                    }
                    else
                    {
                        ShowMessage("This is fucked up, wang.");
                        return false;
                    }
                }
            }
            else if (coinTotal == 9)
            {
                if (coinsToBuyInOrder[0] == -1)
                {
                    ShowMessage("There are no coins? Not cool man.");
                }
                else
                {
                    coinsTobuy[coinsToBuyInOrder[0]] = 1;
                    if (PickUpCoins(coinsTobuy))
                    {
                        return true;
                    }
                    else
                    {
                        ShowMessage("This is fucked up, cholo.");
                        return false;
                    }
                }
            }

            return false;
        }
    }
}
