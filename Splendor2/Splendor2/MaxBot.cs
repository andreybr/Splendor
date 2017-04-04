using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNACS1Lib;
using Microsoft.Xna.Framework;

namespace Splendor
{
    public class MaxBot : BotBase
    {
        /*
        Strategy Priorities
        ================
        + Rewrite canBecomeAffordable to accurately use gold in expense calculation and to properly calculate the available reserve gold
        + Reserve cards for gold if needed coins are not present
        - If multiple cards tie for best value, pick the cheaper one


        - Plan for combos of cards
            - Consider what cards to buy based on the cards the other players have
        - Hate play
        - Build weight graph with cost of turns

        - Incorporate consideration for combination values of cards. Like if 2 cards would cost 5 coins but are worth 4 points total and another worth 4 for 3 points, aim for the 4/5 points/coins ratio
        */

        public MaxBot(List<Card>[] oNewCardStack, List<Card>[] oNewPickedUpCards, List<Noble> oNewNobleStack, Player[] oNewPlayers,
                         ActionLog oNewActionLog, int[] nNewCoinCount, XNACS1Rectangle[] oNewCardBacks, XNACS1Rectangle[] oNewCardCounts,
                         Stats oNewStats, XNACS1Rectangle oNewCoinActionMenu,
                         Cursor oNewCursor, XNACS1Rectangle[] oNewCoinStack, TurnInfo oNewTurnInfo)
                         : base(oNewCardStack, oNewPickedUpCards, oNewNobleStack, oNewPlayers, oNewActionLog, nNewCoinCount, oNewCardBacks,
                                oNewCardCounts, oNewStats, oNewCoinActionMenu, oNewCursor, oNewCoinStack, oNewTurnInfo)
        {}

        const float GEM_MODIFIER = 0.5f;	//Arbitrary value to give to each gem. This helps prioritize the value-less gems from tier 0 for early game

        const int BOARD_WIDTH = 4;
        const int BOARD_HEIGHT = 3;

        const int COINS_MAX = 10;
        const int MAX_RESERVED_CARDS = 3;

        const int POINTS_FOR_VICTORY = 15;

        public enum COINS
        {
            White,
            Blue,
            Green,
            Red,
            Black,
            Gold
        };

        private readonly string[] Names = { "Andrey", "Jerry" };
        private readonly string[] TimePeriods = { "always", "often", "sometimes", "frequently", "daily", "regurlarly", "repeatedly" };
        private readonly string[] Verbs = { "ate", "admired", "elbow dropped", "beat", "sucked", "stroked", "spun", "slapped", "sniffed", "put his face in", "rubbed against", "fingered", "spanked" };
        private readonly string[] Quantities = { "two", "two and a half", "three", "10s of", "dozens of", "large amounts of", "25", "50", "100", "1000", "1000000", "innumerable", "infinite" };
        private readonly string[] Adjectives = { "Andrey's", "Jerry's", "soft", "slimy", "moist", "crusty", "rotten", "screeching", "whooping", "supple", "deformed", "fresh" };
        private readonly string[] Nouns = { "balls", "buttholes", "bags of penises", "nipples", "cats", "trouser snakes", "macaques", "buckets of earthworms", "jizzed underwear" };

        private int maxNobles;

        public class Coords
        {
            public int x;
            public int y;

            public Coords()
            {
                x = -1;
                y = -1;
            }
        }

        public override void TakeTurn()
        {
            Random rand = new Random();
            string message = Names[rand.Next(0, Names.Length)] + " " + TimePeriods[rand.Next(0, TimePeriods.Length)] + " "
            + Verbs[rand.Next(0, Verbs.Length)] + " " + Quantities[rand.Next(0, Quantities.Length)] + " " + Adjectives[rand.Next(0, Adjectives.Length)]
            + " " + Nouns[rand.Next(0, Nouns.Length)];
            
            //ShowMessage(message);

            int myId = GetMyID();
            int[] myCoins = CountCoinsHeld(myId);
            int[] myGems = CountCardsBought(myId);
            int[] myResources = sumEachResource(myCoins, myGems);
            int totalCoins = sumArray(myCoins);
            int totalGems = sumArray(CountCardsBought(myId));
            int totalResources = sumArray(myResources);
            int myPoints = CheckPlayerPoints(myId);
            int[] cardsInTierDecks = getTierDeckCount();
            int[] coinsInBank = CoinsRemaining();
            Card[][] cardsOnBoard = getCardsOnBoard();
            List<Card> cardsReservedByMe = CardsReservedByPlayer(myId);
            int totalCardsReservedByMe = cardsReservedByMe.Count;
            maxNobles = CountNobles();


            //Gather info
            Coords bestValueICanAfford = getBestValueOnBoard(cardsOnBoard, cardsReservedByMe, totalCardsReservedByMe, myCoins, myGems, totalCoins, true);
            Card targetCard;
            if (bestValueICanAfford.y < 0)
            {
                targetCard = cardsReservedByMe[bestValueICanAfford.x];
            }
            else
            {
                targetCard = cardsOnBoard[bestValueICanAfford.x][bestValueICanAfford.y];
            }

            //XNACS1Base.EchoToTopStatus("Max:" + targetCard.Tier() + " " + targetCard.Points() + " " + targetCard.Type());

            int[] difference = findCostVsResourceDifference(targetCard.Cost(), myResources);
            int totalDifference = sumArray(difference);

            //Take the turn
            int cardPoints = targetCard.Points();
            if (totalDifference >= 0)
            {
                if (bestValueICanAfford.y < 0)
                {
                    BuyCardFromHand(bestValueICanAfford.x);
                }
                else
                {
                    BuyCardFromBoard(bestValueICanAfford.x, bestValueICanAfford.y);
                }
            }
            //Reserve IF can reserve AND card points would be enough to win OR I must take Gold because other coins I need are gone
            else if (cardsReservedByMe.Count < MAX_RESERVED_CARDS && bestValueICanAfford.y >= 0
                && ((myPoints + cardPoints) >= POINTS_FOR_VICTORY || needToTakeGold(difference, coinsInBank)))
            {
                ReserveCardFromBoard(bestValueICanAfford.x, bestValueICanAfford.y);
            }
            else
            {
                int[] coinsToGet = getCoins(targetCard, myResources, difference, totalDifference, totalCoins);
                int totalCoinsToGet = sumArray(coinsToGet);
                if (totalCoinsToGet <= 0)
                {
                    Coords bestValueFromBoardICanAfford = getBestValueOnBoard(cardsOnBoard, cardsReservedByMe, totalCardsReservedByMe, myCoins, myGems, totalCoins, false);
                    ReserveCardFromBoard(bestValueFromBoardICanAfford.x, bestValueFromBoardICanAfford.y);
                }
                PickUpCoins(coinsToGet);
            }
        }
        public override void ReactToMove(string sName, int nPlayerID, TurnInfo.ActionType eAction, int[] arrnSelectedCoins, Card oCard)
        {

        }
        public override string Name()
        {
            return "Error";
        }
        public override string Picture()
        {
            return "FaceTest3";
        }

        private int[] sumEachResource(int[] coins, int[] gems)
        {
            int[] resources = new int[6];

            for (int i = 0; i < 6; i++)
            {
                if (i == 5)
                    resources[i] = coins[i];
                else
                    resources[i] = coins[i] + gems[i];
            }

            return resources;
        }

        //Sum up the passed array
        private int sumArray(int[] arr)
        {
            int total = 0;

            for (int i = 0; i < arr.Length; i++)
            {
                total += arr[i];
            }

            return total;
        }

        //Retrieve quantity of cards in each deck
        private int[] getTierDeckCount()
        {
            int[] tierDecks = new int[BOARD_HEIGHT];

            for (int i = 0; i < BOARD_HEIGHT; i++)
            {
                tierDecks[i] = CardsInDeck(2 - i);
            }

            return tierDecks;
        }

        //Retrieve all cards on the board
        private Card[][] getCardsOnBoard()
        {
            Card[][] boardState = new Card[BOARD_WIDTH][];			//x is column, y is row

            for (int i = 0; i < BOARD_WIDTH; i++)
            {
                boardState[i] = new Card[BOARD_HEIGHT];
                for (int j = 0; j < BOARD_HEIGHT; j++)
                {
                    boardState[i][j] = CheckCardFromBoard(i, j);
                }
            }

            return boardState;
        }

        //Finds the first card that has the best point/cost ratio. Cards with no points come back as negative cost
        private Coords getBestValueOnBoard(Card[][] cardsOnBoard, List<Card> cardsReservedByMe, int totalCardsReservedByMe, int[] myCoins, int[] myGems, int myCoinCount, bool checkReserved)
        {
            Coords curBestValueCoords = new Coords();
            curBestValueCoords.x = 0;
            curBestValueCoords.y = 01;
            float curBestValue = -10.0f;
            float tempValue;

            //Search board for best card
            for (int i = 0; i < BOARD_WIDTH; i++)
            {
                for (int j = 0; j < BOARD_HEIGHT; j++)
                {
                    if (canBecomeAffordable(cardsOnBoard[i][j], totalCardsReservedByMe, sumEachResource(myCoins, myGems), myCoinCount))
                    {
                        tempValue = getValueVsCost(myCoins, myGems, cardsOnBoard[i][j]);
                        if (tempValue > curBestValue)
                        {
                            curBestValue = tempValue;
                            curBestValueCoords.x = i;
                            curBestValueCoords.y = j;
                        }
                    }
                }
            }

            //Select best card between board best and reserves
            if (checkReserved)
            {
                for (int i = 0; i < cardsReservedByMe.Count; i++)
                {
                    if (canBecomeAffordable(cardsReservedByMe[i], totalCardsReservedByMe, sumEachResource(myCoins, myGems), myCoinCount))
                    {
                        tempValue = getValueVsCost(myCoins, myGems, cardsReservedByMe[i]);
                        if (tempValue > curBestValue)
                        {
                            curBestValue = tempValue;
                            curBestValueCoords.x = i;
                            curBestValueCoords.y = -1;
                        }
                    }
                }
            }

            return curBestValueCoords;
        }

        /*//Check if I have enough resources of each color to afford the card.
        //If some are negative, add them up against the gold coins I have.
        //Then if the gold goins are negative, I cannot afford it.
        private bool isAffordable(Card card, int[] myResources) {
            int[] cardCost = card.Cost();
            int[] difference = findDifference(myResources, cardCost);

            int goldCoinsAvailable = myResources[COINS.Gold];
            for (int i = 0; i < difference.Length; i++) {
                if (difference[i] < 0)
                    goldCoinsAvailable += difference[i];
            }

            return goldCoinsAvailable >= 0;
        }*/

        //Checks if there are enough coins in the bank to be able to afford the target card
        private bool canBecomeAffordable(Card card, int totalCardsReservedByMe, int[] myResources, int totalCoins)
        {
            if (card == null)
                return false;
            int[] cardCost = card.Cost();
            int[] availableCoins = CoinsRemaining();
            int coinsUsed = totalCoins;

            //Gold coins available to get is max(slots for gold coins I have, gold coins in the bank up to slots I have)
            int goldCoinsAvailableToGet = 0;
            if (totalCardsReservedByMe < 3)
            {
                int coinsICanGet = MAX_RESERVED_CARDS - totalCardsReservedByMe;
                goldCoinsAvailableToGet = coinsICanGet >= availableCoins[(int)COINS.Gold] ? availableCoins[(int)COINS.Gold] : coinsICanGet;
            }

            //Integrate the use of my own gold in expense calculation
            int goldCoinsIHave = myResources[(int)COINS.Gold];
            for (int i = 0; i < 5; i++)
            {
                if (myResources[i] + goldCoinsIHave < cardCost[i])
                {
                    int coinsNeeded = cardCost[i] - myResources[i];
                    if ((coinsUsed + coinsNeeded) > COINS_MAX)
                    {
                        return false;
                    }
                    else
                    {
                        if (coinsNeeded > availableCoins[i])
                        {
                            if (coinsNeeded > (availableCoins[i] + goldCoinsAvailableToGet))
                            {
                                return false;
                            }
                            else
                            {
                                coinsUsed += coinsNeeded;
                                goldCoinsAvailableToGet -= coinsNeeded - availableCoins[i];
                            }
                        }
                        else
                        {
                            coinsUsed += coinsNeeded;
                        }
                    }
                }
                else if (myResources[i] < cardCost[i])
                {
                    goldCoinsIHave -= cardCost[i] - myResources[i];
                }
            }

              return true;
        }

        //Finds coin difference between card cost and resources owned
        private int[] findCostVsResourceDifference(int[] cost, int[] resources)
        {
            int[] results = new int[6];

            //Find all coins I'm short one
            for (int i = 0; i < 5; i++)
            {
                if (cost[i] != 0)
                {
                    int tempVal =  resources[i] - cost[i];
                    if (tempVal < 0)
                        results[i] = tempVal;
                }
            }

            //Add Gold as a positive value. It'll be used later to calculate the net positive of coins I'm short on
            results[(int)COINS.Gold] = resources[(int)COINS.Gold];

            return results;
        }

        //Find the ratio of points to cost. If no points, return a negative of cost
        private float getValueVsCost(int[] myCoins, int[] myGems, Card card)
        {
            int tier = card.Tier();
	        float cardValue = card.Points() + (GEM_MODIFIER * (3 - tier));
	        int[] cardCost = card.Cost();

	        cardValue += calculateValueTowardsAchievements(myGems, card);

            return cardValue / sumArray(card.Cost());
        }

        //For each noble the gem helps achieve, add 1 / totalNobleCost value to the gem
        private float calculateValueTowardsAchievements(int[] myGems, Card card)
        {
	        float valueTowardsAchievements = 0;
	        for (int i = 0; i < maxNobles; i++) {
		        int[] nobleCost = CheckNoble(i).Cost();
		        //If this gem's type is necessary for the noble && I don't have enough of this type of gem, then add the fraction of the way this gem will help towards the noble
                if (nobleCost[(int)card.Type()] > 0 && myGems[i] < nobleCost[(int)card.Type()])
                {
			        valueTowardsAchievements += (1.0f / sumArray(nobleCost)) * 3;
		        }
	        }

	        return valueTowardsAchievements;
        }


        //Find at least one stack that has coins that I need. If none found, then, knowing that there must be enough Gold
        //coins to compensate (based on prior calculations), the only option remains to take the Gold
        private bool needToTakeGold(int[] difference, int[] coinsInBank)
        {
            for (int i = 0; i < 5; i++)
            {
                if (difference[i] < 0 && coinsInBank[i] > 0)
                {
                    return false;
                }
            }

            return true;
        }

        //If 2 coins needed and can take 2, take 2
        //Else If, take coin if needed and available
        //Finally, loop again and take coins to sum to 3, UNLESS they would take up space needed by specific coins next round
        private int[] getCoins(Card targetCard, int[] myResources, int[] difference, int totalDifference, int totalCoins)
        {
            int[] cardCost = targetCard.Cost();
            int[] coinsAvailable = CoinsRemaining();
            int[] coinsToGet = new int[5];
            int coinsAbleToTake = totalCoins >= 7 ? 3 - (totalCoins - 7) : 3;
            int coinCounter = totalCoins;


            for (int i = 0; i < 5 && coinsAbleToTake > 0 && coinCounter < COINS_MAX; i++)
            {
                if (totalDifference == -2 && difference[i] == -2 && coinCounter <= 8 && coinsAvailable[i] >= 4)
                {
                    coinsToGet[i] = 2;
                    return coinsToGet;
                }
                else
                {
                    if ((cardCost[i] - myResources[i]) > 0 && coinsAvailable[i] > 0)
                    {
                        coinsToGet[i] = 1;
                        coinsAbleToTake--;
                        coinCounter++;
                    }
                }
            }

            //Loop and take additional coins, so long as they aren't duplicates, or take up space for other necessary coins
            for (int i = 0; i < 5 && coinsAbleToTake > 0 && coinCounter < COINS_MAX; i++)
            {
                if (coinsToGet[i] == 0 && coinsAvailable[i] > 0)
                {
                    coinsToGet[i] = 1;
                    coinsAbleToTake--;
                    coinCounter++;
                }
            }

            return coinsToGet;
        }
    }
}
