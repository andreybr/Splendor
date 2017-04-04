using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using XNACS1Lib;

namespace Splendor
{
    public class TurnInfo
    {
        public enum ActionType { NONE, COINS, BUY_BOARD, BUY_HAND, RESERVE_BOARD, RESERVE_DECK };
        public ActionType eActionType = ActionType.NONE;
        public string strAction;
        public int[] nSelectedCoins = new int[5];
        public Vector2 oTarget;
        public bool bNobleGained = false;
    }

    class GameMenu
    {
        KeyboardState oKeyState = new KeyboardState();

        // white blue green red black
        List<Card>[] oCardStack = new List<Card>[3];
        List<Card>[] oPickedUpCards = new List<Card>[4];
        List<Noble> oNobleStack = new List<Noble>();
        XNACS1Rectangle[] oCardBacks = new XNACS1Rectangle[3];
        XNACS1Rectangle[] oCardCounts = new XNACS1Rectangle[3];
        XNACS1Rectangle[] oCoinStack = new XNACS1Rectangle[6];
        ActionLog oActionLog = new ActionLog();
        const int nLogMaxLenght = 17;
        int[] nCoinCount = new int[6];
        public Player[] oPlayers = new Player[4];
        enum TURN_PHASE { NONE, PLAYER_MOVE, PLAYER_REACTION, PLAYER_ANIMATION, BOARD_ANIMATION, RESOLVE_ACTION, GAME_OVER };
        TURN_PHASE ePhase = TURN_PHASE.PLAYER_MOVE;
        XNACS1Rectangle oTurnTimer;
        int nRealPlayerIndex = -1;

        List<BotBase> oBots = new List<BotBase>();
        int nKeyPressTimer = 0;

        int nTurnTimer = 0;
        bool bCanPressButton = true;

        Stats oStats;
        VictoryMenu oVictoryMenu;

        bool bRandomizeBotOrder = true;
        XNACS1Rectangle oCoinActionMenu;

        public TurnInfo oTurnInfo = new TurnInfo();
        int nAnimationTimer = 0;
        int nAnimationTimerMax = 15;
        bool bInitializePhase = false;
        XNACS1Rectangle oCardHighlight;
        XNACS1Rectangle[] oAnimationCoins = new XNACS1Rectangle[6];

        public GameMenu(Cursor oCursor, int[] nSelectedBots = null)
        {         
            XNACS1Base.World.SetBackgroundTexture("Background4");
            //XNACS1Base.World.SetBackgroundTexture(null);
            XNACS1Base.World.SetBackgroundColor(new Color(64, 40, 32));
            //XNACS1Base.World.SetBackgroundTexture("");
            //nSelectedBots = null;

            int nPlayerCount = 1;
            if (nSelectedBots != null)
            {             
                for (int x = 0; x < nSelectedBots.Count(); x++)
                    if (nSelectedBots[x] > 0)
                        nPlayerCount++;

                oStats = new Stats(nPlayerCount);
            }
            else
            {
                oStats = new Stats(4);
            }

            oTurnTimer = new XNACS1Rectangle(new Vector2(70f, 15f), 40f, 3f);
            oTurnTimer.Color = Color.Black;
            oTurnTimer.LabelColor = Color.White;
            oTurnTimer.Visible = false;

            for (int x = 0; x < 5; x++)
                nCoinCount[x] = oStats.nMaxRegularCoins;
            nCoinCount[5] = oStats.nMaxGoldCoins;

            oCoinActionMenu = new XNACS1Rectangle(new Vector2(), MyLibrary.Resize(80), MyLibrary.Resize(405));
            oCoinActionMenu.Visible = false;
            oCoinActionMenu.LabelColor = Color.White;

            oCoinStack[0] = new XNACS1Rectangle(new Vector2(), MyLibrary.Resize(52), MyLibrary.Resize(52), "CoinWhite2");
            oCoinStack[1] = new XNACS1Rectangle(new Vector2(), MyLibrary.Resize(52), MyLibrary.Resize(52), "CoinBlue2");
            oCoinStack[2] = new XNACS1Rectangle(new Vector2(), MyLibrary.Resize(52), MyLibrary.Resize(52), "CoinGreen2");
            oCoinStack[3] = new XNACS1Rectangle(new Vector2(), MyLibrary.Resize(52), MyLibrary.Resize(52), "CoinRed2");
            oCoinStack[4] = new XNACS1Rectangle(new Vector2(), MyLibrary.Resize(52), MyLibrary.Resize(52), "CoinBlack2");
            oCoinStack[5] = new XNACS1Rectangle(new Vector2(), MyLibrary.Resize(52), MyLibrary.Resize(52), "CoinGold");
            //oCoinStack[0].TextureTintColor = new Color(240, 240, 240);
            //oCoinStack[1].TextureTintColor = Color.Blue;
            //oCoinStack[2].TextureTintColor = Color.Green;
            //oCoinStack[3].TextureTintColor = Color.OrangeRed;
            //oCoinStack[4].TextureTintColor = Color.SaddleBrown;
            //oCoinStack[5].TextureTintColor = Color.Gold;
            oCoinStack[0].LabelColor = Color.Black;
            oCoinStack[1].LabelColor = Color.White;
            oCoinStack[2].LabelColor = Color.White;
            oCoinStack[3].LabelColor = Color.White;
            oCoinStack[4].LabelColor = Color.White;
            oCoinStack[5].LabelColor = Color.Black;
            for (int x = 0; x < oCoinStack.Length; x++)
            {
                oCoinStack[x].LabelFont = "CoinCount";
                oCoinStack[x].Center = new Vector2(MyLibrary.Resize(550), MyLibrary.Resize(570) - x * MyLibrary.Resize(61));
            }

            oCardStack[0] = new List<Card>();
            oCardStack[1] = new List<Card>();
            oCardStack[2] = new List<Card>();

            oPickedUpCards[0] = new List<Card>();
            oPickedUpCards[1] = new List<Card>();
            oPickedUpCards[2] = new List<Card>();
            oPickedUpCards[3] = new List<Card>();

            oCardCounts[0] = new XNACS1Rectangle(new Vector2(), MyLibrary.Resize(60), MyLibrary.Resize(100), "CardCountMarker");
            oCardCounts[0].LabelColor = Color.White;
            oCardCounts[0].LabelFont = "CardCount";
            oCardCounts[1] = new XNACS1Rectangle(new Vector2(), MyLibrary.Resize(60), MyLibrary.Resize(100), "CardCountMarker");
            oCardCounts[1].LabelColor = Color.White;
            oCardCounts[1].LabelFont = "CardCount";
            oCardCounts[2] = new XNACS1Rectangle(new Vector2(), MyLibrary.Resize(60), MyLibrary.Resize(100), "CardCountMarker");
            oCardCounts[2].LabelColor = Color.White;
            oCardCounts[2].LabelFont = "CardCount";

            for (int x = 0; x < oCardBacks.Length; x++) // deck counts
            {
                oCardCounts[x].Center = new Vector2(MyLibrary.Resize(37), MyLibrary.Resize(275) + x * MyLibrary.Resize(130));
            }

            oCardBacks[0] = new XNACS1Rectangle(new Vector2(), MyLibrary.Resize(84), MyLibrary.Resize(122), "CardBack1");
            oCardBacks[1] = new XNACS1Rectangle(new Vector2(), MyLibrary.Resize(84), MyLibrary.Resize(122), "CardBack2");
            oCardBacks[2] = new XNACS1Rectangle(new Vector2(), MyLibrary.Resize(84), MyLibrary.Resize(122), "CardBack3");

            for (int x = 0; x < oCardBacks.Length; x++) // decks
                oCardBacks[x].Center = new Vector2(MyLibrary.Resize(95), MyLibrary.Resize(275) + x * MyLibrary.Resize(130));

            #region Green Card Stack (Tier 1)
            oCardStack[0].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.WHITE, new int[] { 0, 3, 0, 0, 0, 0 }, 0));
            oCardStack[0].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.WHITE, new int[] { 0, 0, 0, 2, 1, 0 }, 0));
            oCardStack[0].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.WHITE, new int[] { 0, 1, 1, 1, 1, 0 }, 0));
            oCardStack[0].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.WHITE, new int[] { 0, 2, 0, 0, 2, 0 }, 0));
            oCardStack[0].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.WHITE, new int[] { 0, 0, 4, 0, 0, 0 }, 1));
            oCardStack[0].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.WHITE, new int[] { 0, 1, 2, 1, 1, 0 }, 0));
            oCardStack[0].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.WHITE, new int[] { 0, 2, 2, 0, 1, 0 }, 0));
            oCardStack[0].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.WHITE, new int[] { 3, 1, 0, 0, 1, 0 }, 0));

            oCardStack[0].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.BLUE, new int[] { 1, 0, 0, 0, 2, 0 }, 0));
            oCardStack[0].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.BLUE, new int[] { 0, 0, 0, 0, 3, 0 }, 0));
            oCardStack[0].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.BLUE, new int[] { 1, 0, 1, 1, 1, 0 }, 0));
            oCardStack[0].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.BLUE, new int[] { 0, 0, 2, 0, 2, 0 }, 0));
            oCardStack[0].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.BLUE, new int[] { 0, 0, 0, 4, 0, 0 }, 1));
            oCardStack[0].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.BLUE, new int[] { 1, 0, 1, 2, 1, 0 }, 0));
            oCardStack[0].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.BLUE, new int[] { 1, 0, 2, 2, 0, 0 }, 0));
            oCardStack[0].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.BLUE, new int[] { 0, 1, 3, 1, 0, 0 }, 0));

            oCardStack[0].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.GREEN, new int[] { 2, 1, 0, 0, 0, 0 }, 0));
            oCardStack[0].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.GREEN, new int[] { 0, 0, 0, 3, 0, 0 }, 0));
            oCardStack[0].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.GREEN, new int[] { 1, 1, 0, 1, 1, 0 }, 0));
            oCardStack[0].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.GREEN, new int[] { 0, 2, 0, 2, 0, 0 }, 0));
            oCardStack[0].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.GREEN, new int[] { 0, 0, 0, 0, 4, 0 }, 1));
            oCardStack[0].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.GREEN, new int[] { 1, 1, 0, 1, 2, 0 }, 0));
            oCardStack[0].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.GREEN, new int[] { 0, 1, 0, 2, 2, 0 }, 0));
            oCardStack[0].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.GREEN, new int[] { 1, 3, 1, 0, 0, 0 }, 0));

            oCardStack[0].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.RED, new int[] { 0, 2, 1, 0, 0, 0 }, 0));
            oCardStack[0].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.RED, new int[] { 3, 0, 0, 0, 0, 0 }, 0));
            oCardStack[0].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.RED, new int[] { 1, 1, 1, 0, 1, 0 }, 0));
            oCardStack[0].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.RED, new int[] { 2, 0, 0, 2, 0, 0 }, 0));
            oCardStack[0].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.RED, new int[] { 4, 0, 0, 0, 0, 0 }, 1));
            oCardStack[0].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.RED, new int[] { 2, 1, 1, 0, 1, 0 }, 0));
            oCardStack[0].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.RED, new int[] { 2, 0, 1, 0, 2, 0 }, 0));
            oCardStack[0].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.RED, new int[] { 1, 0, 0, 1, 3, 0 }, 0));

            oCardStack[0].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.BLACK, new int[] { 0, 0, 2, 1, 0, 0 }, 0));
            oCardStack[0].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.BLACK, new int[] { 0, 0, 3, 0, 0, 0 }, 0));
            oCardStack[0].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.BLACK, new int[] { 1, 1, 1, 1, 0, 0 }, 0));
            oCardStack[0].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.BLACK, new int[] { 2, 0, 2, 0, 0, 0 }, 0));
            oCardStack[0].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.BLACK, new int[] { 0, 4, 0, 0, 0, 0 }, 1));
            oCardStack[0].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.BLACK, new int[] { 1, 2, 1, 1, 0, 0 }, 0));
            oCardStack[0].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.BLACK, new int[] { 2, 2, 0, 1, 0, 0 }, 0));
            oCardStack[0].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.BLACK, new int[] { 0, 0, 1, 3, 1, 0 }, 0));
            #endregion

            for (int x = 0; x < oCardStack[0].Count(); x++)
                oCardStack[0][x].nTier = 0;

            #region Green Card Stack (Tier 2)
            oCardStack[1].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.WHITE, new int[] { 0, 0, 0, 5, 0, 0 }, 2));
            oCardStack[1].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.WHITE, new int[] { 6, 0, 0, 0, 0, 0 }, 3));
            oCardStack[1].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.WHITE, new int[] { 0, 0, 3, 2, 2, 0 }, 1));
            oCardStack[1].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.WHITE, new int[] { 0, 0, 1, 4, 2, 0 }, 2));
            oCardStack[1].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.WHITE, new int[] { 2, 3, 0, 3, 0, 0 }, 1));
            oCardStack[1].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.WHITE, new int[] { 0, 0, 0, 5, 3, 0 }, 2));

            oCardStack[1].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.BLUE, new int[] { 0, 5, 0, 0, 0, 0 }, 2));
            oCardStack[1].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.BLUE, new int[] { 0, 6, 0, 0, 0, 0 }, 3));
            oCardStack[1].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.BLUE, new int[] { 0, 2, 2, 3, 0, 0 }, 1));
            oCardStack[1].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.BLUE, new int[] { 2, 0, 0, 1, 4, 0 }, 2));
            oCardStack[1].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.BLUE, new int[] { 0, 2, 3, 0, 3, 0 }, 1));
            oCardStack[1].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.BLUE, new int[] { 5, 3, 0, 0, 0, 0 }, 2));

            oCardStack[1].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.GREEN, new int[] { 0, 0, 5, 0, 0, 0 }, 2));
            oCardStack[1].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.GREEN, new int[] { 0, 0, 6, 0, 0, 0 }, 3));
            oCardStack[1].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.GREEN, new int[] { 2, 3, 0, 0, 2, 0 }, 1));
            oCardStack[1].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.GREEN, new int[] { 3, 0, 2, 3, 0, 0 }, 1));
            oCardStack[1].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.GREEN, new int[] { 4, 2, 0, 0, 1, 0 }, 2));
            oCardStack[1].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.GREEN, new int[] { 0, 5, 3, 0, 0, 0 }, 2));

            oCardStack[1].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.RED, new int[] { 0, 0, 0, 0, 5, 0 }, 2));
            oCardStack[1].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.RED, new int[] { 0, 0, 0, 6, 0, 0 }, 3));
            oCardStack[1].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.RED, new int[] { 2, 0, 0, 2, 3, 0 }, 1));
            oCardStack[1].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.RED, new int[] { 1, 4, 2, 0, 0, 0 }, 2));
            oCardStack[1].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.RED, new int[] { 0, 3, 0, 2, 3, 0 }, 1));
            oCardStack[1].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.RED, new int[] { 3, 0, 0, 0, 5, 0 }, 2));

            oCardStack[1].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.BLACK, new int[] { 5, 0, 0, 0, 0, 0 }, 2));
            oCardStack[1].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.BLACK, new int[] { 0, 0, 0, 0, 6, 0 }, 3));
            oCardStack[1].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.BLACK, new int[] { 3, 2, 2, 0, 0, 0 }, 1));
            oCardStack[1].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.BLACK, new int[] { 0, 1, 4, 2, 0, 0 }, 2));
            oCardStack[1].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.BLACK, new int[] { 3, 0, 3, 0, 2, 0 }, 1));
            oCardStack[1].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.BLACK, new int[] { 0, 0, 5, 3, 0, 0 }, 2));
            #endregion

            for (int x = 0; x < oCardStack[1].Count(); x++)
                oCardStack[1][x].nTier = 1;

            #region Green Card Stack (Tier 3)
            oCardStack[2].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.WHITE, new int[] { 0, 0, 0, 0, 7, 0 }, 4));
            oCardStack[2].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.WHITE, new int[] { 3, 0, 0, 0, 7, 0 }, 5));
            oCardStack[2].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.WHITE, new int[] { 3, 0, 0, 3, 6, 0 }, 4));
            oCardStack[2].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.WHITE, new int[] { 0, 3, 3, 5, 3, 0 }, 5));

            oCardStack[2].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.BLUE, new int[] { 7, 0, 0, 0, 0, 0 }, 4));
            oCardStack[2].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.BLUE, new int[] { 7, 3, 0, 0, 0, 0 }, 5));
            oCardStack[2].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.BLUE, new int[] { 6, 3, 0, 0, 3, 0 }, 4));
            oCardStack[2].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.BLUE, new int[] { 3, 0, 3, 3, 5, 0 }, 3));

            oCardStack[2].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.GREEN, new int[] { 0, 7, 0, 0, 0, 0 }, 4));
            oCardStack[2].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.GREEN, new int[] { 0, 7, 3, 0, 0, 0 }, 5));
            oCardStack[2].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.GREEN, new int[] { 3, 6, 3, 0, 0, 0 }, 4));
            oCardStack[2].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.GREEN, new int[] { 5, 3, 0, 3, 3, 0 }, 3));

            oCardStack[2].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.RED, new int[] { 0, 0, 7, 0, 0, 0 }, 4));
            oCardStack[2].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.RED, new int[] { 0, 0, 7, 3, 0, 0 }, 5));
            oCardStack[2].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.RED, new int[] { 0, 3, 6, 3, 0, 0 }, 4));
            oCardStack[2].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.RED, new int[] { 3, 5, 3, 0, 3, 0 }, 3));

            oCardStack[2].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.BLACK, new int[] { 0, 0, 0, 7, 0, 0 }, 4));
            oCardStack[2].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.BLACK, new int[] { 0, 0, 0, 7, 3, 0 }, 5));
            oCardStack[2].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.BLACK, new int[] { 0, 0, 3, 6, 3, 0 }, 4));
            oCardStack[2].Add(new Card(new Vector2(-50f, -50f), Card.TYPE.BLACK, new int[] { 3, 3, 5, 3, 0, 0 }, 3));
            #endregion

            for (int x = 0; x < oCardStack[2].Count(); x++)
                oCardStack[2][x].nTier = 2;

            #region Nobles
            oNobleStack.Add(new Noble(new Vector2(-50f, -50f), new int[] { 3, 3, 0, 0, 3 }, 3));
            oNobleStack.Add(new Noble(new Vector2(-50f, -50f), new int[] { 0, 3, 3, 3, 0 }, 3));
            oNobleStack.Add(new Noble(new Vector2(-50f, -50f), new int[] { 3, 0, 0, 3, 3 }, 3));
            oNobleStack.Add(new Noble(new Vector2(-50f, -50f), new int[] { 0, 0, 4, 4, 0 }, 3));
            oNobleStack.Add(new Noble(new Vector2(-50f, -50f), new int[] { 0, 4, 4, 0, 0 }, 3));

            oNobleStack.Add(new Noble(new Vector2(-50f, -50f), new int[] { 0, 0, 0, 4, 4 }, 3));
            oNobleStack.Add(new Noble(new Vector2(-50f, -50f), new int[] { 4, 0, 0, 0, 4 }, 3));
            oNobleStack.Add(new Noble(new Vector2(-50f, -50f), new int[] { 3, 3, 3, 0, 0 }, 3));
            oNobleStack.Add(new Noble(new Vector2(-50f, -50f), new int[] { 0, 0, 3, 3, 3 }, 3));
            oNobleStack.Add(new Noble(new Vector2(-50f, -50f), new int[] { 4, 4, 0, 0, 0 }, 3));
            #endregion

            ShuffleCards(oCardStack[0], 100);
            ShuffleCards(oCardStack[1], 100);
            ShuffleCards(oCardStack[2], 100);
            ShuffleNobles(oNobleStack, 20);

            for (int x = 0; x < oStats.nNobleCount; x++) // nobles
                oNobleStack[x].Recenter(new Vector2(MyLibrary.Resize(95) + x * MyLibrary.Resize(90), MyLibrary.Resize(650)));

            if (nSelectedBots != null)
            {
                oBots.Add(new PlayerBot(oCardStack, oPickedUpCards, oNobleStack, oPlayers, oActionLog, nCoinCount, oCardBacks, oCardCounts, oStats, oCoinActionMenu, oCursor, oCoinStack, oTurnInfo));
                for (int x = 0; x < nSelectedBots.Count(); x++)
                {
                    if (nSelectedBots[x] == 1)
                        oBots.Add(new JerryBot(oCardStack, oPickedUpCards, oNobleStack, oPlayers, oActionLog, nCoinCount, oCardBacks, oCardCounts, oStats, oCoinActionMenu, oCursor, oCoinStack, oTurnInfo));
                    else if (nSelectedBots[x] == 2)
                        oBots.Add(new MaxBot(oCardStack, oPickedUpCards, oNobleStack, oPlayers, oActionLog, nCoinCount, oCardBacks, oCardCounts, oStats, oCoinActionMenu, oCursor, oCoinStack, oTurnInfo));
                    else if (nSelectedBots[x] == 3)
                        oBots.Add(new AndreyBot(oCardStack, oPickedUpCards, oNobleStack, oPlayers, oActionLog, nCoinCount, oCardBacks, oCardCounts, oStats, oCoinActionMenu, oCursor, oCoinStack, oTurnInfo));                    
                }
            }
            else
            {
                oBots.Add(new PlayerBot(oCardStack, oPickedUpCards, oNobleStack, oPlayers, oActionLog, nCoinCount, oCardBacks, oCardCounts, oStats, oCoinActionMenu, oCursor, oCoinStack, oTurnInfo));
                oBots.Add(new AndreyBot(oCardStack, oPickedUpCards, oNobleStack, oPlayers, oActionLog, nCoinCount, oCardBacks, oCardCounts, oStats, oCoinActionMenu, oCursor, oCoinStack, oTurnInfo));
                oBots.Add(new JerryBot(oCardStack, oPickedUpCards, oNobleStack, oPlayers, oActionLog, nCoinCount, oCardBacks, oCardCounts, oStats, oCoinActionMenu, oCursor, oCoinStack, oTurnInfo));
                oBots.Add(new MaxBot(oCardStack, oPickedUpCards, oNobleStack, oPlayers, oActionLog, nCoinCount, oCardBacks, oCardCounts, oStats, oCoinActionMenu, oCursor, oCoinStack, oTurnInfo));
            }

            Random rand = new Random(Guid.NewGuid().GetHashCode());
            int nCurRand;
            if (bRandomizeBotOrder == true)
            {
                for (int x = 0; x < oStats.nNumberOfPlayers; x++)
                {
                    nCurRand = rand.Next() % oBots.Count();

                    if (oBots[nCurRand].GetType() == typeof(PlayerBot))
                        nRealPlayerIndex = x;

                    oBots[nCurRand].SetPlayerNumber(x);
                    oPlayers[x] = new Player(oStats.nNobleCount, oBots[nCurRand], oBots[nCurRand].Name());
                    oBots.RemoveAt(nCurRand);
                }
            }
            else
            {
                for (int x = 0; x < oStats.nNumberOfPlayers; x++)
                {
                    if (x == 0)
                        oPlayers[x] = new Player(oStats.nNobleCount, oBots[0], oBots[0].Name());
                    else if (x == 1)
                        oPlayers[x] = new Player(oStats.nNobleCount, oBots[1], oBots[1].Name());
                    else if (x == 2)
                        oPlayers[x] = new Player(oStats.nNobleCount, oBots[2], oBots[2].Name());
                    else if (x == 3)
                        oPlayers[x] = new Player(oStats.nNobleCount, oBots[3], oBots[3].Name());
                }
            }

            oActionLog.AddLog("Round " + (oStats.nCurrentRound + 1), Color.White);
            SetupBoard();
        }

        public void Update(Cursor oCursor)
        {
            oKeyState = Keyboard.GetState();
            //XNACS1Base.EchoToTopStatus("Game:" + ePhase);
            //XNACS1Base.EchoToTopStatus("Game:" + oStats.nCurrentPlayer);
            //XNACS1Base.EchoToBottomStatus("Game:" + nCurrentPlayer + " " + ePhase);
            //XNACS1Base.EchoToTopStatus("Game:" + oStats.nCurrentPlayer + " " + oStats.nCurrentPlayer);
     
            for (int x = 0; x < oStats.nNumberOfPlayers; x++)
                oPlayers[x].Update();

            if (ePhase == TURN_PHASE.NONE)
            {
                oTurnTimer.Label = "Round: " + (oStats.nCurrentRound + 1) + "   Turn: " + oStats.nCurrentPlayer + "    Timer: " + Math.Ceiling((float)nTurnTimer / 40f);

                #region bot take turn
                if (oKeyState.IsKeyDown(Keys.Space))
                {
                    if (bCanPressButton == true)
                    {
                        bCanPressButton = false;
                        //XNACS1Base.PlayACue("ButtonClick.wav");
                        oPlayers[oStats.nCurrentPlayer].TakeTurn();
                        oPlayers[oStats.nCurrentPlayer].ResetTurn();
                        nKeyPressTimer = 0;

                        //XNACS1Base.EchoToTopStatus("GameMenu:" + oTurnInfo.strAction + " " + oTurnInfo.oTarget );

                        for (int x = 0; x < oStats.nNumberOfPlayers; x++)
                            oPlayers[x].Update();

                        #region check if player earned noble
                        for (int x = 0; x < oStats.nNobleCount; x++)
                        {
                            if (oNobleStack[x].IsAvailable() == true)
                            {
                                if (oPlayers[oStats.nCurrentPlayer].nOwnedCards[0] >= oNobleStack[x].Cost()[0] &&
                                    oPlayers[oStats.nCurrentPlayer].nOwnedCards[1] >= oNobleStack[x].Cost()[1] &&
                                    oPlayers[oStats.nCurrentPlayer].nOwnedCards[2] >= oNobleStack[x].Cost()[2] &&
                                    oPlayers[oStats.nCurrentPlayer].nOwnedCards[3] >= oNobleStack[x].Cost()[3] &&
                                    oPlayers[oStats.nCurrentPlayer].nOwnedCards[4] >= oNobleStack[x].Cost()[4])
                                {
                                    oNobleStack[x].SetOwner(oPlayers[oStats.nCurrentPlayer].GetMyID());
                                    oPlayers[oStats.nCurrentPlayer].nPoints += oNobleStack[x].Points();
                                    oActionLog.AddLog("[" + oPlayers[oStats.nCurrentPlayer].strName + "] has earned a noble(" + x + ")", Color.Purple);
                                }
                            }
                        }
                        #endregion

                        // game ends
                        //if (oPlayers[oStats.nCurrentPlayer].Points() >= 15)
                        //{
                        //    int[] nScores = new int[oStats.nNumberOfPlayers];
                        //    string[] strNames = new string[oStats.nNumberOfPlayers];

                        //    for (int x = 0; x < nScores.Count(); x++)
                        //        nScores[x] = oPlayers[x].Points();

                        //    for (int x = 0; x < strNames.Count(); x++)
                        //        strNames[x] = oPlayers[x].strName;

                        //    Clear();
                        //    oVictoryMenu = new VictoryMenu(nScores, strNames);
                        //    ePhase = TURN_PHASE.GAME_OVER;
                        //}
                        //else
                        //{
                            //SetupBoard();
                            //oStats.AdvanceTurn();
                            //SetupBoard();

                            //if (oStats.nCurrentPlayer == 0)
                            //    AddToLog("Round " + oStats.nCurrentRound, Color.White);

                            //if (oStats.nCurrentPlayer == 0)
                            //{
                            //    oPlayers[oStats.nNumberOfPlayers - 1].Texture = "PlayerBox";
                            //    oPlayers[oStats.nCurrentPlayer].Texture = "PlayerBox2";
                            //}
                            //else
                            //{
                            //    oPlayers[oStats.nCurrentPlayer - 1].Texture = "PlayerBox";
                            //    oPlayers[oStats.nCurrentPlayer].Texture = "PlayerBox2";
                            //}


                        //nAnimationTimer = 60;
                        bInitializePhase = false;
                        ePhase = TURN_PHASE.PLAYER_ANIMATION;

                        //if (oTurnInfo.oTarget.Y > -1)
                        //    if (oTurnInfo.oTarget.X > -1)
                        //        oCardHighlight = new XNACS1Rectangle(oCardStack[(int)oTurnInfo.oTarget.Y][(int)oTurnInfo.oTarget.X].oCardImage.Center, MyLibrary.Resize(88), MyLibrary.Resize(126), "CardHighlight");
                        //    else
                        //        oCardHighlight = new XNACS1Rectangle(oCardBacks[(int)oTurnInfo.oTarget.Y].Center, MyLibrary.Resize(88), MyLibrary.Resize(126), "CardHighlight");
                        //else if (oTurnInfo.oTarget.Y == -1)
                        //    if (oTurnInfo.oTarget.X > -1)
                        //        oCardHighlight = new XNACS1Rectangle(oPickedUpCards[oStats.nCurrentPlayer][(int)oTurnInfo.oTarget.X].oCardImage.Center, MyLibrary.Resize(88), MyLibrary.Resize(126), "CardHighlight");
                        //}

                        for (int x = 0; x < 6; x++)
                        {
                            if (nCoinCount[x] == 0)
                                oCoinStack[x].TextureTintColor = new Color(0, 0, 0, 100);
                            else
                                oCoinStack[x].TextureTintColor = new Color(255, 255, 255, 255);
                        }
                    }
                }
                else
                {
                    bCanPressButton = true;
                }
                #endregion
            }
            else if (ePhase == TURN_PHASE.PLAYER_MOVE)
            {
                #region player take turn
                oTurnTimer.Label = "Round: " + oStats.nCurrentRound + "   Turn: " + oStats.nCurrentPlayer + "    Timer: " + Math.Ceiling((float)nTurnTimer / 40f);

                if (oPlayers[oStats.nCurrentPlayer].oBot.GetType() == typeof(PlayerBot) || oKeyState.IsKeyDown(Keys.Space))
                {
                    //if(oPlayers[oStats.nCurrentPlayer].oBot.GetType() == typeof(PlayerBot))
                    //    oPlayers[oStats.nCurrentPlayer].oBot.bTurnTaken = true;

                    if (bCanPressButton == true)
                    {
                        for(int x = 0; x < oStats.nNumberOfPlayers; x++)
                            if(oStats.nCurrentPlayer != x)
                                oPlayers[x].ResetTurn();

                        oPlayers[oStats.nCurrentPlayer].TakeTurn();                  
                        if (oPlayers[oStats.nCurrentPlayer].HasTakenTurn() == true)
                        {
                            for (int x = 0; x < oStats.nNumberOfPlayers; x++)
                                oPlayers[x].Update();

                            //oPlayers[oStats.nCurrentPlayer].ResetTurn();
                            SetupBoard();

                            bInitializePhase = false;
                            SetupBoard();
                            ePhase = TURN_PHASE.PLAYER_REACTION;
                        }
                    }
                }
                else
                {
                    bCanPressButton = true;
                }
                #endregion
            }
            else if (ePhase == TURN_PHASE.PLAYER_REACTION)
            {
                #region player reaction
                Card oCard;

                if (oTurnInfo.eActionType == TurnInfo.ActionType.BUY_BOARD || oTurnInfo.eActionType == TurnInfo.ActionType.RESERVE_BOARD)
                    oCard = oCardStack[(int)oTurnInfo.oTarget.Y][(int)oTurnInfo.oTarget.X].Copy();
                else if (oTurnInfo.eActionType == TurnInfo.ActionType.BUY_HAND)
                    oCard = oPickedUpCards[oStats.nCurrentPlayer][(int)oTurnInfo.oTarget.X].Copy();
                else
                    oCard = null;

                for (int x = 0; x < oStats.nNumberOfPlayers; x++)
                {
                    if(oStats.nCurrentPlayer == x)
                        continue;

                    oPlayers[x].ReactToMove(oPlayers[oStats.nCurrentPlayer].GetName(), x, oTurnInfo.eActionType, oTurnInfo.nSelectedCoins, oCard);
                }
                #endregion

                ePhase = TURN_PHASE.PLAYER_ANIMATION;              
            }
            else if (ePhase == TURN_PHASE.PLAYER_ANIMATION)
            {
                string strAction = oTurnInfo.strAction;
                Vector2 oTarget = oTurnInfo.oTarget;
                int[] nSelectedCoins = oTurnInfo.nSelectedCoins;

                #region animation
                if (strAction == "pick up coins")
                {
                    if (bInitializePhase == false)
                    {
                        //XNACS1Base.PlayBackgroundAudio("rhodesmas__action-02", 1f);
                        //XNACS1Base.PlayACue("rhodesmas__action-02");
                        bInitializePhase = true;
                        nAnimationTimer = nAnimationTimerMax;

                        for (int x = 0; x < nSelectedCoins.Count(); x++)
                        {
                            if (nSelectedCoins[x] > 0)
                            {
                                oAnimationCoins[x] = new XNACS1Rectangle(new Vector2(), MyLibrary.Resize(52), MyLibrary.Resize(52), "CoinWhite2");
                                oAnimationCoins[x].Center = oCoinStack[x].Center;

                                if (x == 0)
                                    oAnimationCoins[x].Texture = "CoinWhite2";
                                else if (x == 1)
                                    oAnimationCoins[x].Texture = "CoinBlue";
                                else if (x == 2)
                                    oAnimationCoins[x].Texture = "CoinGreen2";
                                else if (x == 3)
                                    oAnimationCoins[x].Texture = "CoinRed2";
                                else if (x == 4)
                                    oAnimationCoins[x].Texture = "CoinBlack2";

                                oAnimationCoins[x].ShouldTravel = true;
                                oAnimationCoins[x].Velocity = oPlayers[oStats.nCurrentPlayer].Center() - oAnimationCoins[x].Center;
                                oAnimationCoins[x].Velocity.Normalize();
                                oAnimationCoins[x].Velocity *= 0.02f;
                            }
                        }
                    }
                    else
                    {
                        int nFade = (int)((((float)nAnimationTimer - 5) / (float)nAnimationTimerMax) * 255f);

                        for (int x = 0; x < nSelectedCoins.Count(); x++)
                            if(oAnimationCoins[x] != null)
                                oAnimationCoins[x].TextureTintColor = new Color(nFade, nFade, nFade, nFade);
                        
                    }
                }
                else if (strAction == "buy from board" || strAction == "reserve from board")
                {                
                    if (bInitializePhase == false)
                    {
                        //XNACS1Base.PlayACue("bigmac4029__1cards");
                        //XNACS1Base.PlayACue("Action1");
                        bInitializePhase = true;
                        nAnimationTimer = nAnimationTimerMax;

                        oCardStack[(int)oTarget.Y][(int)oTarget.X].oCardImage.ShouldTravel = true;
                        oCardStack[(int)oTarget.Y][(int)oTarget.X].oCardImage.Velocity = new Vector2(0, -0.25f);
                    }
                    else
                    {
                        int nFade = (int)((((float)nAnimationTimer - 5) / (float)nAnimationTimerMax) * 255f);

                        if (nFade < 0)
                            nFade = 0;

                        oCardStack[(int)oTarget.Y][(int)oTarget.X].Recenter(oCardStack[(int)oTarget.Y][(int)oTarget.X].oCardImage.Center);
                        oCardStack[(int)oTarget.Y][(int)oTarget.X].oCardImage.TextureTintColor = new Color(nFade, nFade, nFade, nFade);
                    }
                }
                else if (strAction == "buy from hand")
                {             
                    if (bInitializePhase == false)
                    {
                        bInitializePhase = true;
                        nAnimationTimer = nAnimationTimerMax;
                    }
                    else
                    {
                        int nFade = (int)(((float)nAnimationTimer / 40f) * 255f);

                        if (nFade < 0)
                            nFade = 0;

                        oPickedUpCards[oStats.nCurrentPlayer][(int)oTarget.X].oCardImage.TextureTintColor = new Color(nFade, nFade, nFade, nFade);
                    }
                }
                else if (strAction == "reserve from deck")
                {
                    if (bInitializePhase == false)
                    {
                        bInitializePhase = true;
                        nAnimationTimer = nAnimationTimerMax;

                        oCardStack[(int)oTarget.Y][4].DrawFront();
                        oCardStack[(int)oTarget.Y][4].oCardImage.Center = oCardBacks[(int)oTarget.Y].Center;
                        oCardStack[(int)oTarget.Y][4].oCardImage.Texture = "CardBack" + (oTarget.Y + 1);
                        oCardStack[(int)oTarget.Y][4].oCardImage.ShouldTravel = true;
                        oCardStack[(int)oTarget.Y][4].oCardImage.Velocity = new Vector2(0, -0.25f);
                    }
                    else
                    {
                        int nFade = (int)(((float)nAnimationTimer / 40f) * 255f);

                        if (nFade < 0)
                            nFade = 0;

                        oCardStack[(int)oTarget.Y][4].oCardImage.TextureTintColor = new Color(nFade, nFade, nFade, nFade);
                    }
                }
                #endregion

                // end phase
                if (nAnimationTimer > 0)
                    nAnimationTimer--;
                else
                {
                    if (strAction == "pick up coins")
                    {
                        for(int x= 0; x < oAnimationCoins.Count(); x++)
                            if (oAnimationCoins[x] != null)
                            {
                                oAnimationCoins[x].RemoveFromAutoDrawSet();
                                oAnimationCoins[x] = null;
                            }
                    }

                    //if (strAction == "reserve from deck")
                    //{
                    //    oCardStack[(int)oTarget.Y][4].Clear();
                    //    oCardStack[(int)oTarget.Y][4].Draw();
                    //    oCardStack[(int)oTarget.Y][4].oCardImage.ShouldTravel = false;
                    //    oCardStack[(int)oTarget.Y][4].oCardImage.TextureTintColor = new Color(255, 255, 255, 255);
                    //    SetupBoard();
                    //}
                    //else if (strAction == "reserve from board" || strAction == "buy from board")
                    //{
                    //    oCardStack[(int)oTarget.Y][(int)oTarget.X].oCardImage.TextureTintColor = new Color(255, 255, 255, 255);
                    //    oCardStack[(int)oTarget.Y][(int)oTarget.X].oCardImage.ShouldTravel = false;
                    //    oCardStack[(int)oTarget.Y][(int)oTarget.X].oCardImage.TextureTintColor = new Color(255, 255, 255, 255);
                    //}
                    //else if (strAction == "buy from hand")
                    //{

                    //}

                    //nAnimationTimer = 20;
                    ePhase = TURN_PHASE.BOARD_ANIMATION;
                    bInitializePhase = false;
                }
            }
            else if (ePhase == TURN_PHASE.BOARD_ANIMATION)
            {
                #region 
                string strAction = oTurnInfo.strAction;
                Vector2 oTarget = oTurnInfo.oTarget;
                int[] nSelectedCoins = oTurnInfo.nSelectedCoins;

                if (bInitializePhase == false)
                {
                    if (strAction == "reserve from deck")
                    {
                        oCardStack[(int)oTarget.Y][4].Clear();
                        oCardStack[(int)oTarget.Y][4].DrawFront();
                        oCardStack[(int)oTarget.Y][4].oCardImage.ShouldTravel = false;
                        oCardStack[(int)oTarget.Y][4].oCardImage.TextureTintColor = new Color(255, 255, 255, 255);
                        //SetupBoard();
                    }
                    else if (strAction == "reserve from board")
                    {
                        oCardStack[(int)oTarget.Y][(int)oTarget.X].oCardImage.ShouldTravel = false;
                        oCardStack[(int)oTarget.Y][(int)oTarget.X].oCardImage.TextureTintColor = new Color(255, 255, 255, 255);
                        //SetupBoard();
                    }

                    else if (strAction == "buy from board")
                    {
                        //oCardStack[(int)oTarget.Y][(int)oTarget.X].oCardImage.TextureTintColor = new Color(255, 255, 255, 255);
                        //oCardStack[(int)oTarget.Y][(int)oTarget.X].oCardImage.ShouldTravel = false;
                        oCardStack[(int)oTarget.Y][(int)oTarget.X].oCardImage.TextureTintColor = new Color(255, 255, 255, 255);
                    }
                    else if (strAction == "buy from hand")
                    {

                    }
                    bInitializePhase = true;
                }

                if (nAnimationTimer > 0)
                    nAnimationTimer--;
                else
                {
                    bInitializePhase = false;
                    ePhase = TURN_PHASE.RESOLVE_ACTION;
                }
                #endregion
            }
            else if (ePhase == TURN_PHASE.RESOLVE_ACTION)
            {
                //XNACS1Base.PlayACue("ButtonClick");

                #region give rewards for action
                string strAction = oTurnInfo.strAction;
                Vector2 oTargetCard = oTurnInfo.oTarget;
                int[] nSelectedCoins = oTurnInfo.nSelectedCoins;

                if (strAction == "reserve from board")
                {
                    if (oPlayers[oStats.nCurrentPlayer].CoinTotal() < 10 && nCoinCount[5] > 0)
                    {
                        oPlayers[oStats.nCurrentPlayer].GetCoins(5, 1);
                        nCoinCount[5]--;
                    }

                    // add card to player hand and remove from board
                    oPickedUpCards[oStats.nCurrentPlayer].Add(oCardStack[(int)oTargetCard.Y][(int)oTargetCard.X]);
                    if (oPlayers[oStats.nCurrentPlayer].oBot.GetType() != typeof(PlayerBot))
                    {
                        oPickedUpCards[oStats.nCurrentPlayer].Last().Clear();
                        oPickedUpCards[oStats.nCurrentPlayer].Last().DrawBack();
                        oPickedUpCards[oStats.nCurrentPlayer].Last().ScaleSize(0.4f);
                    }
                    else
                    {
                        oPickedUpCards[oStats.nCurrentPlayer].Last().ScaleSize(1.0f);
                    }
                    //oPickedUpCards[oStats.nCurrentPlayer].Last().Draw();
                    //oCardStack[(int)oTargetCard.Y][(int)oTargetCard.X].Clear();
                    oCardStack[(int)oTargetCard.Y].RemoveAt((int)oTargetCard.X);

                    // show that deck is empty
                    if (oCardStack[(int)oTargetCard.Y].Count == 4)
                    {
                        oCardBacks[(int)oTargetCard.Y].TextureTintColor = new Color(50, 50, 50, 50);
                        oCardCounts[(int)oTargetCard.Y].RemoveFromAutoDrawSet();
                        oCardCounts[(int)oTargetCard.Y] = null;
                    }
                }
                else if (strAction == "reserve from deck")
                {
                    if (oPlayers[oStats.nCurrentPlayer].CoinTotal() < 10 && nCoinCount[5] > 0)
                    {
                        oPlayers[oStats.nCurrentPlayer].GetCoins(5, 1);
                        nCoinCount[5]--;
                    }

                    oPickedUpCards[oStats.nCurrentPlayer].Add(oCardStack[(int)oTargetCard.Y][4]);
                    if (oPlayers[oStats.nCurrentPlayer].oBot.GetType() != typeof(PlayerBot))
                    {
                        oPickedUpCards[oStats.nCurrentPlayer].Last().Clear();
                        oPickedUpCards[oStats.nCurrentPlayer].Last().DrawBack();
                        oPickedUpCards[oStats.nCurrentPlayer].Last().ScaleSize(0.4f);
                    }
                    else
                    {
                        oPickedUpCards[oStats.nCurrentPlayer].Last().ScaleSize(1.0f);
                    }
                    oCardStack[(int)oTargetCard.Y].RemoveAt(4);

                    if (oCardStack[(int)oTargetCard.Y].Count == 4)
                    {
                        oCardBacks[(int)oTargetCard.Y].TextureTintColor = new Color(50, 50, 50, 50);
                        oCardCounts[(int)oTargetCard.Y].RemoveFromAutoDrawSet();
                        oCardCounts[(int)oTargetCard.Y] = null;
                    }
                }
                else if (strAction == "buy from board")
                {
                    int nCost;
                    for (int x = 0; x < 5; x++)
                    {
                        nCost = oCardStack[(int)oTargetCard.Y][(int)oTargetCard.X].Cost()[x] - oPlayers[oStats.nCurrentPlayer].nOwnedCards[x];

                        if (nCost <= 0)
                            continue;

                        if (nCost > oPlayers[oStats.nCurrentPlayer].nOwnedCoins[x])
                        {
                            nCoinCount[x] += oPlayers[oStats.nCurrentPlayer].nOwnedCoins[x];
                            nCoinCount[5] += (nCost - oPlayers[oStats.nCurrentPlayer].nOwnedCoins[x]);
                            //oPlayers[oStats.nCurrentPlayer].nOwnedCoins[5] -= (nCost - oPlayers[oStats.nCurrentPlayer].nOwnedCoins[x]);
                            //oPlayers[oStats.nCurrentPlayer].nOwnedCoins[x] = 0;
                            oPlayers[oStats.nCurrentPlayer].GetCoins(5, -(nCost - oPlayers[oStats.nCurrentPlayer].nOwnedCoins[x]));
                            oPlayers[oStats.nCurrentPlayer].GetCoins(x, -oPlayers[oStats.nCurrentPlayer].nOwnedCoins[x]);
                        }
                        else if (nCost <= oPlayers[oStats.nCurrentPlayer].nOwnedCoins[x])
                        {
                            nCoinCount[x] += nCost;
                            //oPlayers[oStats.nCurrentPlayer].nOwnedCoins[x] -= nCost;
                            oPlayers[oStats.nCurrentPlayer].GetCoins(x, -nCost);
                        }
                    }

                    oPlayers[oStats.nCurrentPlayer].nBoughtCardTiers[oCardStack[(int)oTargetCard.Y][(int)oTargetCard.X].nTier]++;
                    oPlayers[oStats.nCurrentPlayer].nPoints += oCardStack[(int)oTargetCard.Y][(int)oTargetCard.X].Points();
                    //oPlayers[oStats.nCurrentPlayer].nOwnedCards[(int)oCardStack[(int)oTargetCard.Y][(int)oTargetCard.X].Type()]++;
                    oPlayers[oStats.nCurrentPlayer].GetCards((int)oCardStack[(int)oTargetCard.Y][(int)oTargetCard.X].Type(), oCardStack[(int)oTargetCard.Y][(int)oTargetCard.X].Points());
                    oCardStack[(int)oTargetCard.Y][(int)oTargetCard.X].Clear();
                    oCardStack[(int)oTargetCard.Y].RemoveAt((int)oTargetCard.X);

                    if (oCardStack[(int)oTargetCard.Y].Count == 4)
                    {
                        oCardBacks[(int)oTargetCard.Y].TextureTintColor = new Color(50, 50, 50, 50);
                        oCardCounts[(int)oTargetCard.Y].RemoveFromAutoDrawSet();
                        oCardCounts[(int)oTargetCard.Y] = null;
                    }
                }
                else if (strAction == "buy from hand")
                {
                    int nCost;
                    for (int x = 0; x < 5; x++)
                    {
                        nCost = oPickedUpCards[oStats.nCurrentPlayer][(int)oTargetCard.X].Cost()[x] - oPlayers[oStats.nCurrentPlayer].nOwnedCards[x];

                        if (nCost <= 0)
                            continue;

                        if (nCost > oPlayers[oStats.nCurrentPlayer].nOwnedCoins[x])
                        {
                            nCoinCount[x] += oPlayers[oStats.nCurrentPlayer].nOwnedCoins[x];
                            nCoinCount[5] += (nCost - oPlayers[oStats.nCurrentPlayer].nOwnedCoins[x]);
                            //oPlayers[oStats.nCurrentPlayer].nOwnedCoins[5] -= (nCost - oPlayers[oStats.nCurrentPlayer].nOwnedCoins[x]);
                            //oPlayers[oStats.nCurrentPlayer].nOwnedCoins[x] = 0;
                            oPlayers[oStats.nCurrentPlayer].GetCoins(5, -(nCost - oPlayers[oStats.nCurrentPlayer].nOwnedCoins[x]));
                            oPlayers[oStats.nCurrentPlayer].GetCoins(x, -oPlayers[oStats.nCurrentPlayer].nOwnedCoins[x]);
                        }
                        else if (nCost <= oPlayers[oStats.nCurrentPlayer].nOwnedCoins[x])
                        {
                            nCoinCount[x] += nCost;
                            //oPlayers[oStats.nCurrentPlayer].nOwnedCoins[x] -= nCost;
                            oPlayers[oStats.nCurrentPlayer].GetCoins(x, -nCost);
                        }

                    }

                    oPlayers[oStats.nCurrentPlayer].nBoughtCardTiers[oPickedUpCards[oStats.nCurrentPlayer][(int)oTargetCard.X].nTier]++;
                    oPlayers[oStats.nCurrentPlayer].nPoints += oPickedUpCards[oStats.nCurrentPlayer][(int)oTargetCard.X].Points();
                    //oPlayers[oStats.nCurrentPlayer].nOwnedCards[(int)oPickedUpCards[oStats.nCurrentPlayer][(int)oTargetCard.X].Type()]++;
                    oPlayers[oStats.nCurrentPlayer].GetCards((int)oPickedUpCards[oStats.nCurrentPlayer][(int)oTargetCard.X].Type(), oPickedUpCards[oStats.nCurrentPlayer][(int)oTargetCard.X].Points());
                    oPickedUpCards[oStats.nCurrentPlayer][(int)oTargetCard.X].Clear();
                    oPickedUpCards[oStats.nCurrentPlayer].RemoveAt((int)oTargetCard.X);
                }
                else if (strAction == "pick up coins")
                {
                    for (int x = 0; x < 5; x++)
                    {
                        if (nSelectedCoins[x] > 0)
                        {
                            //strLog += "" + nSelectedCoins[x] + " " + (Card.TYPE)x + " ";
                            //oPlayers[oStats.nCurrentPlayer].nOwnedCoins[x] += nSelectedCoins[x];
                            oPlayers[oStats.nCurrentPlayer].GetCoins(x, nSelectedCoins[x]);
                            nCoinCount[x] -= nSelectedCoins[x];
                            nSelectedCoins[x] = 0;
                        }
                    }
                }

                #region check if player earned noble
                for (int x = 0; x < oStats.nNobleCount; x++)
                {
                    if (oNobleStack[x].IsAvailable() == true)
                    {
                        if (oPlayers[oStats.nCurrentPlayer].nOwnedCards[0] >= oNobleStack[x].Cost()[0] &&
                            oPlayers[oStats.nCurrentPlayer].nOwnedCards[1] >= oNobleStack[x].Cost()[1] &&
                            oPlayers[oStats.nCurrentPlayer].nOwnedCards[2] >= oNobleStack[x].Cost()[2] &&
                            oPlayers[oStats.nCurrentPlayer].nOwnedCards[3] >= oNobleStack[x].Cost()[3] &&
                            oPlayers[oStats.nCurrentPlayer].nOwnedCards[4] >= oNobleStack[x].Cost()[4])
                        {
                            //XNACS1Base.PlayACue("Action1");
                            oNobleStack[x].SetOwner(oPlayers[oStats.nCurrentPlayer].GetMyID());
                            oPlayers[oStats.nCurrentPlayer].nPoints += oNobleStack[x].Points();
                            oPlayers[oStats.nCurrentPlayer].oNoblesAcquired[x].TextureTintColor = new Color(255, 255, 255);
                            //oPlayers[oStats.nCurrentPlayer].oNoblesAcquired[x].Color = new Color(147, 62, 190);
                            oTurnInfo.bNobleGained = true;
                            oActionLog.AddLog("[" + oPlayers[oStats.nCurrentPlayer].strName + "] has earned a noble(" + x + ")", Color.Purple);
                        }
                    }
                }
                #endregion

                for (int x = 0; x < oStats.nNumberOfPlayers; x++)
                    oPlayers[x].Update();

                SetupBoard();
                oStats.AdvanceTurn();
                SetupBoard();

                if (oStats.nCurrentPlayer == 0)
                    oActionLog.AddLog("Round " + (oStats.nCurrentRound + 1), Color.White);


                //if (oStats.nCurrentPlayer == 0)
                //{
                //    if (nRealPlayerIndex != oStats.nNumberOfPlayers - 1)
                //        oPlayers[oStats.nNumberOfPlayers - 1].oBox.Texture = "PlayerBoxv4-1";

                //    if (nRealPlayerIndex != oStats.nCurrentPlayer)
                //        oPlayers[oStats.nCurrentPlayer].oBox.Texture = "PlayerBoxv4-1";
                //}
                //else
                //{
                //    if (nRealPlayerIndex != oStats.nCurrentPlayer - 1)
                //        oPlayers[oStats.nCurrentPlayer - 1].oBox.Texture = "PlayerBoxv4-1";

                //    if (nRealPlayerIndex != oStats.nCurrentPlayer)
                //        oPlayers[oStats.nCurrentPlayer].oBox.Texture = "PlayerBoxv4-1";
                //}
                

                oTurnInfo.eActionType = TurnInfo.ActionType.NONE;
                oTurnInfo.oTarget = new Vector2();
                oTurnInfo.strAction = "";
                oTurnInfo.nSelectedCoins = new int[5];
                oTurnInfo.bNobleGained = false;

                //ePhase = TURN_PHASE.NONE;
                ePhase = TURN_PHASE.PLAYER_MOVE;
                #endregion

                #region update coin when stack empty
                for (int x = 0; x < 6; x++)
                {
                    if (nCoinCount[x] == 0) // empty stack
                    {
                        oCoinStack[x].TextureTintColor = new Color(0, 0, 0, 100);

                        if (x == 0)
                            oCoinStack[x].LabelColor = Color.White;
                        else if (x == 1)
                            oCoinStack[x].LabelColor = new Color(62, 127, 190);
                        else if (x == 2)
                            oCoinStack[x].LabelColor = new Color(62, 190, 86);
                        else if (x == 3)
                            oCoinStack[x].LabelColor = new Color(190, 62, 62);
                        else if (x == 4)
                            oCoinStack[x].LabelColor = new Color(70, 50, 28);
                        else if (x == 5)
                            oCoinStack[x].LabelColor = Color.Gold;
                    }
                    else // non empty stack
                    {
                        oCoinStack[x].TextureTintColor = new Color(255, 255, 255, 255);

                        if (x == 0 || x == 5)
                            oCoinStack[x].LabelColor = Color.Black;
                        else
                            oCoinStack[x].LabelColor = Color.White;
                    }
                }
                #endregion

                #region check if game is over
                for (int x = 0; x < oStats.nNumberOfPlayers; x++)
                    if (oPlayers[x].Points() >= 15)
                        ePhase = TURN_PHASE.GAME_OVER;
                #endregion         

                if (oCardHighlight != null)
                    oCardHighlight.RemoveFromAutoDrawSet();
            }

            else if (ePhase == TURN_PHASE.GAME_OVER)
            {
                if (oVictoryMenu == null)
                {
                    List<Player> oPlayerList = new List<Player>();

                    for (int x = 0; x < oPlayers.Count(); x++)
                        if (oPlayers[x] != null)
                            oPlayerList.Add(oPlayers[x]);

                    oVictoryMenu = new VictoryMenu(oPlayerList);
                    //Clear();
                }
            }
        }

        public void Clear()
        {
            for (int x = 0; x < oCoinStack.Length; x++) // coins
            {
                oCoinStack[x].RemoveFromAutoDrawSet();
                oCoinStack[x] = null;
            }

            for (int x = 0; x < oCardBacks.Length; x++) // decks
            {
                oCardBacks[x].RemoveFromAutoDrawSet();
                oCardBacks[x] = null;
            }

            for (int x = 0; x < oCardBacks.Length; x++) // deck counts
            {
                if (oCardCounts[x] != null)
                {
                    oCardCounts[x].RemoveFromAutoDrawSet();
                    oCardCounts[x] = null;
                }
            }

            for (int x = 0; x < oCardStack.Count(); x++) // cards
            {
                while (oCardStack[x].Count > 0)
                {
                    oCardStack[x][0].Clear();
                    oCardStack[x].RemoveAt(0);
                }
            }

            for (int x = 0; x < oStats.nNobleCount; x++) // nobles
            {
                oNobleStack[x].Clear();
                oNobleStack[x].RemoveFromAutoDrawSet();
                oNobleStack[x] = null;
            }

            for (int x = 0; x < oStats.nNumberOfPlayers; x++) // player hud
            {
                //oPlayers[x].RemoveFromAutoDrawSet();
                //oPlayers[x] = null;
            }
        }

        private void SetupBoard()
        {
            //oPlayers[nRealPlayerIndex].Recenter(new Vector2(MyLibrary.Resize(180), (MyLibrary.Resize(80))));

            for (int x = 0; x < oStats.nNumberOfPlayers; x++) // player hud
            {
                if(x == nRealPlayerIndex)
                    continue;

                if (x > nRealPlayerIndex)
                {
                    if (x - nRealPlayerIndex == 1)
                        oPlayers[x].Recenter(new Vector2(MyLibrary.Resize(168), MyLibrary.Resize(698)));
                    else if (x - nRealPlayerIndex == 2)
                        oPlayers[x].Recenter(new Vector2(MyLibrary.Resize(527), MyLibrary.Resize(698)));
                    else if (x - nRealPlayerIndex == 3)
                        oPlayers[x].Recenter(new Vector2(MyLibrary.Resize(887), MyLibrary.Resize(698)));
                }
                else
                {
                    int nOrder = (oStats.nNumberOfPlayers) - nRealPlayerIndex + x;

                    if (nOrder == 1)
                        oPlayers[x].Recenter(new Vector2(MyLibrary.Resize(168), MyLibrary.Resize(698)));
                    else if (nOrder == 2)
                        oPlayers[x].Recenter(new Vector2(MyLibrary.Resize(527), MyLibrary.Resize(698)));
                    else if (nOrder == 3)
                        oPlayers[x].Recenter(new Vector2(MyLibrary.Resize(887), MyLibrary.Resize(698)));
                }

                //if (x == 0)
                //    oPlayers[x].Recenter(new Vector2(MyLibrary.Resize(170), MyLibrary.Resize(700)));
                //else if (x == 1)
                //    oPlayers[x].Recenter(new Vector2(MyLibrary.Resize(770), MyLibrary.Resize(700)));
                //else if (x == 2)
                //    oPlayers[x].Recenter(new Vector2(MyLibrary.Resize(770), MyLibrary.Resize(70)));
                //else if (x == 3)
                //    oPlayers[x].Recenter(new Vector2(MyLibrary.Resize(170), MyLibrary.Resize(70)));
            }

            for (int x = 0; x < oCoinStack.Length; x++) // coins
            {
                oCoinStack[x].Label = "" + nCoinCount[x];
                oCoinStack[x].Center = new Vector2(MyLibrary.Resize(660), MyLibrary.Resize(520) - x * MyLibrary.Resize(55));
                //oCoinStack[x].Label = "     " + nCoinCount[x] + "         \n "; // top left
                //oCoinStack[x].Label = nCoinCount[x] + "    "; // left
                //oCoinStack[x].Label = "    " + nCoinCount[x]; // right
            }

            for (int x = 0; x < oCardBacks.Length; x++) // deck counts
            {
                if (oCardCounts[x] != null)
                {
                    oCardCounts[x].Label = "" + (oCardStack[x].Count() - 4);
                    oCardCounts[x].Center = new Vector2(MyLibrary.Resize(34), MyLibrary.Resize(254) + x * MyLibrary.Resize(130));
                }
            }

            for (int x = 0; x < oCardBacks.Length; x++) // decks
                oCardBacks[x].Center = new Vector2(MyLibrary.Resize(95), MyLibrary.Resize(254) + x * MyLibrary.Resize(130));

            for (int x = 0; x < oStats.nNobleCount; x++) // nobles
            {
                if (x == 0)
                {
                    if(oStats.nNobleCount == 3)
                        oNobleStack[x].Recenter(new Vector2(MyLibrary.Resize(560), MyLibrary.Resize(480)));
                    else if(oStats.nNobleCount == 4)
                        oNobleStack[x].Recenter(new Vector2(MyLibrary.Resize(560), MyLibrary.Resize(520)));
                    else if (oStats.nNobleCount == 5)
                        oNobleStack[x].Recenter(new Vector2(MyLibrary.Resize(560), MyLibrary.Resize(560)));
                }
                else
                    oNobleStack[x].Recenter(new Vector2(oNobleStack[x - 1].CenterX, oNobleStack[x - 1].CenterY - MyLibrary.Resize(90)));

                //oNobleStack[x].Recenter(new Vector2(MyLibrary.Resize(560), MyLibrary.Resize(533) - x * MyLibrary.Resize(90)));
                //oNobleStack[x].Recenter(new Vector2(MyLibrary.Resize(560), MyLibrary.Resize(400)));
            }

            int nMax;
            for (int y = 0; y < 3; y++) // cards on board
            {
                if (oCardStack[y].Count() > 3)
                    nMax = 4;
                else
                    nMax = oCardStack[y].Count();

                for (int x = 0; x < nMax; x++)
                {
                    if (oCardStack[y][x].oCardImage == null)
                        oCardStack[y][x].DrawFront();

                    oCardStack[y][x].Recenter(new Vector2(MyLibrary.Resize(187) + x * MyLibrary.Resize(89), MyLibrary.Resize(254) + y * MyLibrary.Resize(130)));
                }
            }

            for (int y = 0; y < oPlayers.Count(); y++) // reserved cards
            {
                for (int x = 0; x < oPickedUpCards[y].Count(); x++)
                {
                    if (oPickedUpCards[y][x].oCardImage == null)
                        oPickedUpCards[y][x].DrawFront();

                    if (oPlayers[y].oBot.GetType() != typeof(PlayerBot))
                    {
                        oPickedUpCards[y][x].Recenter(new Vector2(oPlayers[y].Center().X + MyLibrary.Resize(167),
                                                                  oPlayers[y].Center().Y + MyLibrary.Resize(48) - x * MyLibrary.Resize(50)));
                    }
                    else
                    {
                        oPickedUpCards[y][x].Recenter(new Vector2(MyLibrary.Resize(950) + x * MyLibrary.Resize(88),
                                                                   6.3f));
                    }
                }
            }
        }

        private void ShuffleCards(List<Card> oCards, int nNumOfSwaps)
        {
            Random rand = new Random();
            int nTemp1;
            int nTemp2;
            Card oCardTemp;

            for (int x = 0; x < nNumOfSwaps; x++)
            {
                nTemp1 = rand.Next() % oCards.Count;
                nTemp2 = rand.Next() % oCards.Count;

                oCardTemp = oCards[nTemp1];
                oCards[nTemp1] = oCards[nTemp2];
                oCards[nTemp2] = oCardTemp;
            }
        }

        private void ShuffleNobles(List<Noble> oNobles, int nNumOfSwaps)
        {
            Random rand = new Random();
            int nTemp1;
            int nTemp2;
            Noble oNobleTemp;

            for (int x = 0; x < nNumOfSwaps; x++)
            {
                nTemp1 = rand.Next() % oNobles.Count;
                nTemp2 = rand.Next() % oNobles.Count;

                oNobleTemp = oNobles[nTemp1];
                oNobles[nTemp1] = oNobles[nTemp2];
                oNobles[nTemp2] = oNobleTemp;
            }
        }
    }
}
