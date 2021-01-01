using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Linq;

namespace Caravan
{
   enum Suite
   {
      Club,
      Heart,
      Spade,
      Diamond
   }

   enum Rank
   {
      King, Queen, Jack, Ten, Nine, Eight, Seven, Six, Five, Four, Three, Two, Ace
   }
   class Card
   {
      private Rank rank;
      private Suite suite;

      public Card(Rank rank, Suite suite)
      {
         this.rank = rank;
         this.suite = suite;
      }

      public int GetValue()
      {
         switch (this.rank)
         {
            case Rank.Ace:
               return 1;
            case Rank.King:
               return 11;
            case Rank.Ten:
               return 10;
            case Rank.Nine:
               return 9;
            case Rank.Eight:
               return 8;
            case Rank.Seven:
               return 7;
            case Rank.Six:
               return 6;
            case Rank.Five:
               return 5;
            case Rank.Four:
               return 4;
            case Rank.Three:
               return 3;
            case Rank.Two:
               return 2;
            default:
               return 11;
         }
      }
      public Boolean Equals(Card card)
      {
         return (this.rank == card.rank && this.suite == card.suite);
      }
      public Rank GetRank()
      {
         return this.rank;
      }
      public Suite GetSuite()
      {
         return this.suite;
      }

      public string toString()
      {
         string returnValue = "";

         switch (rank)
         {
            case Rank.Ace:
               returnValue = "A";
               break;
            case Rank.King:
               returnValue = "K";
               break;
            case Rank.Queen:
               returnValue = "Q";
               break;
            case Rank.Jack:
               returnValue = "J";
               break;
            case Rank.Ten:
               returnValue = "10";
               break;
            case Rank.Nine:
               returnValue = "9";
               break;
            case Rank.Eight:
               returnValue = "8";
               break;
            case Rank.Seven:
               returnValue = "7";
               break;
            case Rank.Six:
               returnValue = "6";
               break;
            case Rank.Five:
               returnValue = "5";
               break;
            case Rank.Four:
               returnValue = "4";
               break;
            case Rank.Three:
               returnValue = "3";
               break;
            case Rank.Two:
               returnValue = "2";
               break;
         }

         switch (suite)
         {
            case Suite.Club:
               returnValue += "_Club";
               break;
            case Suite.Diamond:
               returnValue += "_Diamond";
               break;
            case Suite.Heart:
               returnValue += "_Heart";
               break;
            case Suite.Spade:
               returnValue += "_Spade";
               break;
         }

         return returnValue;
      }


      public static Card toCard(string s)
      {
         string[] tokens = s.Split('_');
         Rank rank = 0;
         Suite suite = 0;

         switch (tokens[0])
         {
            case "A":
               rank = Rank.Ace;
               break;
            case "K":
               rank = Rank.King;
               break;
            case "Q":
               rank = Rank.Queen;
               break;
            case "J":
               rank = Rank.Jack;
               break;
            case "10":
               rank = Rank.Ten;
               break;
            case "9":
               rank = Rank.Nine;
               break;
            case "8":
               rank = Rank.Eight;
               break;
            case "7":
               rank = Rank.Seven;
               break;
            case "6":
               rank = Rank.Six;
               break;
            case "5":
               rank = Rank.Five;
               break;
            case "4":
               rank = Rank.Four;
               break;
            case "3":
               rank = Rank.Three;
               break;
            case "2":
               rank = Rank.Two;
               break;
         }

         switch (tokens[1])
         {
            case "Club":
               suite = Suite.Club;
               break;
            case "Diamond":
               suite = Suite.Diamond;
               break;
            case "Heart":
               suite = Suite.Heart;
               break;
            case "Spade":
               suite = Suite.Spade;
               break;
         }

         return new Card(rank, suite);
      }
   }

   /* --------------------------------------- Deck ------------------------------------------------------------
    * */
   class Deck
   {
      Stack<Card> cards;
      int size;

      public Deck()
      {
         Card[] shuffleArray = new Card[52];
         cards = new Stack<Card>();

         int i = 0;
         foreach (Rank rank in Enum.GetValues(typeof(Rank)))
         {
            foreach (Suite suite in Enum.GetValues(typeof(Suite)))
            {

               shuffleArray[i++] = new Card(rank, suite);
            }
         }
         
         shuffleArray = Shuffle(shuffleArray);

         for (i = 0; i < 52; i++)
         {
            cards.Push(shuffleArray[i]);
         }
         this.size = 52;
      }

      private Card[] Shuffle(Card[] unshuffled)
      {
         // implement the fisher-yates algorithm
         Random r = new Random();

         for (int i = 51; i > 0; i--)
         {
            // Pick a random index from 0 to i 
            int j = r.Next(0, i + 1);

            // Swap i with the element at random index 
            Card temp = unshuffled[i];
            unshuffled[i] = unshuffled[j];
            unshuffled[j] = temp;
         }

         return unshuffled;
      }

      public Card DealCard()
      {
         if (this.size > 0)
         {
            this.size--;
            return cards.Pop();
         }

         return null;
      }
      public int GetSize()
      {
         return this.size;
      }

      public string toString()
      {
         string returnValue = "";
         foreach (Card card in cards)
         {
            returnValue += (card.toString() + "\n");
         }

         return returnValue;
      }

   }

   // FINSISH AND TEST THIS CLASS FIRST

   /* ------------------------------------- CaravanRow -----------------------------------------------
    * Represents a card placement spot on the game board. The implementation is a slightly modified linked list
    * Numbered cards are played down the board while face cards
    * are played next to numbered cards.
    * 
    * 
   */


   // NOTE: add another type of node that is just for face card. and add the amount of face cards on the normal node since there can only be three and it makes it quicker to check
   class CaravanRow
   {
      class Node
      {
         public Card card;
         public Node nextNumberedCard;
         public Node nextFaceCard;
         public int numFaceCards;


         public Node()
         {
            this.card = null;
            this.nextNumberedCard = null;
            this.nextFaceCard = null;
            this.numFaceCards = 0;
         }
         public Node(Card card)
         {
            this.card = card;
            this.nextNumberedCard = null;
            this.nextFaceCard = null;
            this.numFaceCards = 0;
         }
      }

      private Node front;
      private Node end;
      private bool isIncreasing;
      private int score;
      private int size;

      public CaravanRow()
      {
         this.front = null;
         this.end = null;
         this.isIncreasing = true;
         this.score = 0;
         this.size = 0;
      }

      public int addNumberedCard(Card card)
      {
         Node newCard = new Node(card);
         if (this.front == null)
         {
            this.front = newCard;
            this.end = newCard;
         }
         else
         {
            this.end.nextNumberedCard = newCard;
            this.end = this.end.nextNumberedCard;
         }
         this.size++;
         CalculateScore();
         return this.score;
      }

      public Card GetEnd()
      {
         if (this.end == null)
         {
            return null;
         }
         return this.end.card;
      }

      public int AddFaceCard(Card faceCard, Card numberedCard)
      {
         if (this.size == 0)
         {
            return -1;
         }

         Node targetCard = FindCard(numberedCard);
         PlaceFaceCard(faceCard, targetCard);
         targetCard.numFaceCards++;
         CalculateScore();
         return this.score;
      }

      // helper method for AddFaceCard
      private Node FindCard(Card card)
      {
         Node current = this.front;

         while (current != null)
         {
            if (current.card.Equals(card))
            {
               return current;
            }

            current = current.nextNumberedCard;
         }

         return current;
      }

      // helper method for AddFace Card
      private void PlaceFaceCard(Card card, Node current)
      {
         while (current.nextFaceCard != null)
         {
            current = current.nextFaceCard;
         }
         current.nextFaceCard = new Node(card);
      }

      public int RemoveCard(Card card)
      {
         Node current = this.front;
         Node backOne = null;

         // card being removed is the first card
         if (this.front.card.Equals(card))
         {
            // the card is the only card
            if (this.end.card.Equals(card))
            {
               this.end = null;
               this.front = null;
               this.size = 0;
               this.score = 0;
               return 0;
            }

            // just the first card, not only
            this.front = this.front.nextNumberedCard;
         }

         // card is not the first card
         else
         {
            // finding the target card
            while (current != null)
            {
               if (current.card.Equals(card))
               {
                  backOne.nextNumberedCard = backOne.nextNumberedCard.nextNumberedCard;
               }
               backOne = current;
               current = current.nextNumberedCard;
            }
         }
         this.size--;
         CalculateScore();
         return this.score;
      }

      public int RemoveRow()
      {
         RemoveCard(this.front.card);
         this.score = 0;
         this.size = 0;
         return this.score;
      }

      public void Empty()
      {
         front = null;
         end = null;
         isIncreasing = true;
         score = 0;
         size = 0;
      }
      private void CalculateScore()
      {
         // walk down list, checking for kings to multiple
         // add up each card value
         Node current = front;
         int sum = 0;
         int kings = 1;

         while (current != null)
         {
            kings = 1;
            Node face = current.nextFaceCard;
            while (face != null)
            {
               if (face.card.GetRank() == Rank.King)
               {
                  kings++;
               }
               face = face.nextFaceCard;
            }

            int s = current.card.GetValue();
            sum += s * kings;

            current = current.nextNumberedCard;
         }

         this.score = sum;
      }

      public bool IsIncreasing()
      {
         return this.isIncreasing;
      }

      public void FlipIncreasing()
      {
         isIncreasing = !isIncreasing;
      }
      public int GetScore()
      {
         return this.score;
      }

      public int GetSize()
      {
         return this.size;
      }

      public int GetNumFaceCardsAt(int n)
      {
         int count = 1;
         Node current = this.front;
         while (count < n)
         {
            current = current.nextNumberedCard;
            count++;
         }

         return current.numFaceCards;
      }

      public Card GetCardAt(int n)
      {
         Card c = null;
         int count = 1;
         Node current = front;

         while (current != null && count < n)
         {
            current = current.nextNumberedCard;
            count++;
         }

         if (current != null)
         {
            c = current.card;
         }
         return c;
      }

      public string DisplayCard(int n)
      {
         if (n > this.size)
         {
            return "(empty)";
         }

         int count = 0;
         Node current = front;
         Node face = null;

         while (current != null && count < n)
         {
            current = current.nextNumberedCard;
            count++;
         }

         if (current == null)
         {
            return "(empty)";
         }

         string retVal = current.card.toString();
         face = current.nextFaceCard;

         while (face != null)
         {
            retVal += "," + face.card.toString();
            face = face.nextFaceCard;
         }

         return retVal;
      }

      public static bool isSold(CaravanRow caravan)
      {
         return caravan.score >= 21 && caravan.score <= 26;
      }

      // IMPLEMENT
      public string toString()
      {
         return "";
      }
   }





   /* ---------------------------------------- Player --------------------------------------------------------------------
    * */
   class Player
   {
      private List<Card> hand;
      private Deck deck;

      public Player()
      {
         hand = new List<Card>();
         deck = new Deck();


         for (int i = 0; i < 8; i++)
         {
            hand.Add(deck.DealCard());
         }
      }

      public void DrawCard()
      {
         hand.Add(deck.DealCard());
         if (deck.GetSize() == 0)
         {
            deck = new Deck();
         }
      }
      public List<Card> GetHand()
      {
         return this.hand;
      }

      public void Discard(int n)
      {
         hand.RemoveAt(n - 1);
      }

      public Card CardAt(int n)
      {
         return hand[n - 1];
      }

      public void DisplayHand()
      {
         int count = 1;
         foreach (Card c in hand)
         {
            Console.Write(count + ": " + c.toString() + "  ");
            count++;
         }

         Console.WriteLine();
      }
   }

   class GameBoard
   {
      private CaravanRow[] myCaravans;
      //private int[] myScore;

      private CaravanRow[] theirCaravans;
      //private int[] theirScore;

      public GameBoard()
      {
         this.myCaravans = new CaravanRow[3];
         this.theirCaravans = new CaravanRow[3];


         for (int i = 0; i < 3; i++)
         {
            this.myCaravans[i] = new CaravanRow();
            this.theirCaravans[i] = new CaravanRow();
         }
      }

      public int PlayCard(Card card, int row, Card targetCard, bool player)
      {
         CaravanRow[] caravanRows = (row > 3) ? theirCaravans : myCaravans;
         int rowIndex = (row - 1) % 3;
         CaravanRow caravan = caravanRows[rowIndex];

         if (targetCard == null)
         {
            caravan.addNumberedCard(card);
         }

         else
         {
            caravan.AddFaceCard(card, targetCard);
            if (card.GetRank() == Rank.Queen)
            {
               caravan.FlipIncreasing();
            }

            if (card.GetRank() == Rank.Jack)
            {
               caravan.RemoveCard(targetCard);
            }
         }

         return -1;
      }

      public int RemoveCard(Card targetCard, int row, bool player)
      {

         return -1;
      }

      public int RemoveCard(int targetCard, int row, bool player)
      {

         return -1;
      }

      public bool isGameOver()
      {
         int count = 0;

         for (int i = 0; i < 3; i++)
         {
            if (CaravanRow.isSold(theirCaravans[i]) || CaravanRow.isSold(myCaravans[i]))
            {
               count++;
            }
         }

         return count == 3;
      }

      public bool checkWinner()
      {
         int mySold = 0;
         int oppSold = 0;

         for (int i = 0; i < 3; i++)
         {
            CaravanRow mine = myCaravans[i];
            CaravanRow theirs = theirCaravans[i];

            if (CaravanRow.isSold(mine) && !CaravanRow.isSold(theirs))
            {
               mySold++;
            }

            else if (!CaravanRow.isSold(mine) && CaravanRow.isSold(theirs))
            {
               oppSold++;
            }

            else if (CaravanRow.isSold(mine) && CaravanRow.isSold(theirs))
            {
               if (mine.GetScore() > theirs.GetScore())
               {
                  mySold++;
               }
               else if (mine.GetScore() < theirs.GetScore())
               {
                  oppSold++;
               }
               else
               {
                  mySold++;
                  oppSold++;
               }
            }
         }

         return mySold >= oppSold;
      }

      public void DisplayBoard()
      {
         Console.Clear();
         Console.WriteLine("Opponent's side:");
         int oMax = Math.Max(theirCaravans[0].GetSize(), theirCaravans[1].GetSize());
         oMax = Math.Max(oMax, theirCaravans[2].GetSize());

         int myMax = Math.Max(myCaravans[0].GetSize(), myCaravans[1].GetSize());
         myMax = Math.Max(myMax, myCaravans[2].GetSize());

         for (int i = 0; i < oMax; i++)
         {
            string col1 = theirCaravans[0].DisplayCard(i);
            string col2 = theirCaravans[1].DisplayCard(i);
            string col3 = theirCaravans[2].DisplayCard(i);
            Console.Write("{0,-10}{1,-35}{2,-35}{3,-35}", "Row " + (i + 1) + ":", col1, col2, col3);
            Console.WriteLine();
         }
         Console.Write("{0,-10}{1,-35}{2,-35}{3,-35}", "Totals: ",
            4 + ": " + theirCaravans[0].GetScore(),
            5 + ": " + theirCaravans[1].GetScore(),
            6 + ": " + theirCaravans[2].GetScore());

         Console.WriteLine("\n");
         Console.WriteLine("Your side:");

         for (int i = 0; i < myMax; i++)
         {
            string col1 = myCaravans[0].DisplayCard(i);
            string col2 = myCaravans[1].DisplayCard(i);
            string col3 = myCaravans[2].DisplayCard(i);
            Console.Write("{0,-10}{1,-35}{2,-35}{3,-35}", "Row " + (i + 1) + ":", col1, col2, col3);
            Console.WriteLine();
         }
         Console.Write("{0,-10}{1,-35}{2,-35}{3,-35}", "Totals: ",
            1 + ": " + myCaravans[0].GetScore(),
            2 + ": " + myCaravans[1].GetScore(),
            3 + ": " + myCaravans[2].GetScore());

         Console.WriteLine("\n");
      }

      public CaravanRow GetCaravan(int n)
      {
         CaravanRow[] rows = (n > 3) ? theirCaravans : myCaravans;
         CaravanRow row = rows[(n - 1) % 3];

         return row;
      }
   }

   class Game
   {
      Socket server;
      Player myPlayer;
      GameBoard gameBoard;
      bool winner;

      public Game(Socket s)
      {
         this.myPlayer = new Player();
         this.gameBoard = new GameBoard();
         this.server = s;
      }

      public void SetUp()
      {
         string first = "";
         string temporary = "";
         int count = 1;

         while (count < 4)
         {
            Console.WriteLine("Choose " + (4-count) + " more number cards or enter 'discard' to draw a new card");
            myPlayer.DisplayHand();
            temporary = Console.ReadLine();
            int numCard = 0;

            if (temporary == "discard")
            {
               ClearLastLine();
               Console.WriteLine("Which card would you like to discard? Enter cancel to cancel");
               temporary = Console.ReadLine();
               bool can = false;
               while (true)
               {
                  if (temporary == "cancel") { ClearLastLine(); can = true; break; }
                  if (!int.TryParse(temporary, out numCard))
                  {
                     ClearLastLine();
                     Console.WriteLine("Invalid Input");
                  }
                  else if (numCard < 1 || numCard > myPlayer.GetHand().Count)
                  {
                     ClearLastLine();
                     Console.WriteLine("Out of range.");
                  }
                  else
                  {
                     break;
                  }

                  temporary = Console.ReadLine();
               }
               if (can) { continue; }
               myPlayer.Discard(numCard);
               myPlayer.DrawCard();
               continue;
            }

            else if (!int.TryParse(temporary, out numCard))
            {
               ClearLastLine();
               Console.WriteLine("Invalid Input");
               continue;
            }

            else if (numCard < 1 || numCard > myPlayer.GetHand().Count)
            {
               ClearLastLine();
               Console.WriteLine("Out of range.");
               continue;
            }

            Card h = myPlayer.CardAt(numCard);

            if (h.GetValue() > 10)
            {
               ClearLastLine();
               Console.WriteLine("Not a number card");
               continue;
            }

            myPlayer.Discard(numCard);
            first += " " + h.toString();
            gameBoard.PlayCard(h, count, null, true);
            count++;
         }

         first = first.Remove(0, 1);
         Client.SendMessage(first, server);
         Console.WriteLine("Waiting for opponent...");
         string opp = Client.ReceiveMessage(server);
         //Console.WriteLine(opp);

         while (count < 7)
         {
            string[] cards = opp.Split(' ');
            Card newCard = Card.toCard(cards[count - 4]);
            gameBoard.PlayCard(newCard, count, null, false);
            count++;
         }
      }

      public void Start(int turn)
      {
         Console.Clear();
         Console.WriteLine("Starting Game!");
         SetUp();

         bool over = false;
         bool disconnect = false;
         bool give = false;

         while (!over)
         {
            gameBoard.DisplayBoard();

            if (turn % 2 == 0)
            {
               Console.WriteLine("Your turn!");
               Console.WriteLine("Your hand:");
               myPlayer.DisplayHand();
               bool v = false;
               string move = "";
               while (!v)
               {
                  move = GetMove();
                  v = ValidateMove(move);
               }

               string flipped = PrepareMove(move);
               Client.SendMessage(flipped, server);

               if (move == "give up")
               {
                  give = true;
                  break;
               }

               ParseMove(move);
            }

            else
            {
               Console.WriteLine("Opponent's turn. Waiting...");
               string move = Client.ReceiveMessage(server);

               if (move == "error")
               {
                  disconnect = true;
                  break;
               }
               if (move == "give up")
               {
                  give = true;
                  break;
               }

               Console.WriteLine("Opponent's action: " + move);
               ParseOpponentMove(move);
            }

            gameBoard.DisplayBoard();
            turn++;
            over = gameBoard.isGameOver();
         }

         if (disconnect)
         {
            Console.WriteLine("Your opponent disconnected");
            Console.WriteLine("You win by default");
            return;
         }

         if (give && turn % 2 == 0)
         {
            Console.WriteLine("You gave up");
            return;
         }

         if (give)
         {
            Console.WriteLine("Your opponent gave up");
            Console.WriteLine("You win");
            Client.SendMessage("win", server);
            return;
         }

         winner = gameBoard.checkWinner();

         if (winner)
         {
            Console.WriteLine("You won");
            Client.SendMessage("win", server);
         }

         else
         {
            Console.WriteLine("You lost");
            Client.SendMessage("lose", server);
         }
      }

      public void ParseOpponentMove(string move)
      {
         string[] tokens = move.Split(' ');

         if (move.Contains("reset"))
         {
            int n = Int32.Parse(tokens[1]);
            gameBoard.GetCaravan(n).Empty();
            return;
         }

         if (move.Contains("play"))
         {
            Card c = Card.toCard(tokens[1]);
            int r = Int32.Parse(tokens[2]);
            Card o = null;

            if (c.GetValue() > 10)
            {
               int other = int.Parse(tokens[3]);
               o = gameBoard.GetCaravan(r).GetCardAt(other);
            }

            gameBoard.PlayCard(c, r, o, false);
         }
      }

      public void ParseMove(string move)
      {
         string[] tokens = move.Split(' ');
         int n = Int32.Parse(tokens[1]);

         if (tokens[0] == "play")
         {
            Card c = myPlayer.GetHand()[n - 1];
            myPlayer.GetHand().RemoveAt(n - 1);

            int r = Int32.Parse(tokens[2]);
            Card o = null;

            if (c.GetValue() > 10)
            {
               int other = int.Parse(tokens[3]);
               o = gameBoard.GetCaravan(r).GetCardAt(other);
            }

            gameBoard.PlayCard(c, r, o, true);
            myPlayer.DrawCard();
         }

         if (tokens[0] == "discard")
         {
            myPlayer.GetHand().RemoveAt(n - 1);
            myPlayer.DrawCard();
         }

         if (tokens[0] == "reset")
         {
            gameBoard.GetCaravan(n).Empty();
         }
      }

      public string PrepareMove(string move)
      {
         if (move == "give up")
         {
            return move;
         }

         string flipped = "";
         string[] tokens = move.Split(' ');
         flipped += tokens[0];
         int n = Int32.Parse(tokens[1]);

         if (tokens[0] == "play")
         {
            Card c = myPlayer.GetHand()[n - 1];
            flipped += " " + c.toString();
            int r = Int32.Parse(tokens[2]);
            if (r > 3)
            {
               flipped += " " + (r - 3);
            }
            else
            {
               flipped += " " + (r + 3);
            }

            if (c.GetValue() > 10)
            {
               flipped += " " + tokens[3];
            }
         }

         else if (tokens[0] == "discard")
         {
            flipped += " " + n;
         }

         else if (tokens[0] == "reset")
         {
            flipped += " " + (n + 3);
         }

         return flipped;
      }

      public string GetMove()
      {
         string move = "";
         string temp = "";
         bool done = false;

         List<Card> hand = myPlayer.GetHand();

         while (!done)
         {
            move = "";
            Console.WriteLine("Would you like to 'play' a card, 'discard' a card, 'reset' a caravan, or 'give up'?");
            temp = Console.ReadLine();

            if (temp == "give up")
            {
               return temp;
            }

            else if (temp == "play")
            {
               move += temp;
               ClearLastLine();
               Console.WriteLine("Choose a card from your hand to play or enter cancel");
               temp = GetNumber(hand.Count);
               if (temp == "cancel") { ClearLastLine(); continue; }
               move += " " + temp;
               int n = Int32.Parse(temp);
               Card c = hand[n - 1];

               if (c.GetValue() > 10)
               {
                  ClearLastLine();
                  ClearLastLine();
                  Console.WriteLine("Choose a caravan between 1-6 (4-6 are your opponent's) or enter cancel");
                  temp = GetNumber(6);
                  if (temp == "cancel") { ClearLastLine(); continue; }
                  move += " " + temp;

                  ClearLastLine();
                  ClearLastLine();
                  Console.WriteLine("Choose a card in caravan " + temp + " or enter cancel");
                  int r = Int32.Parse(temp);
                  CaravanRow row = gameBoard.GetCaravan(r);
                  temp = GetNumber(row.GetSize());
                  if (temp == "cancel") { ClearLastLine(); continue; }
                  move += " " + temp;
                  return move;
               }

               else
               {
                  ClearLastLine();
                  ClearLastLine();
                  Console.WriteLine("Choose a caravan between 1-3 or enter cancel");
                  temp = GetNumber(3);
                  if (temp == "cancel") { ClearLastLine(); continue; }
                  move += " " + temp;
                  return move;
               }
            }

            else if (temp == "discard")
            {
               move += temp;
               ClearLastLine();
               Console.WriteLine("Choose a card from your hand to discard or enter cancel");
               temp = GetNumber(hand.Count);
               if (temp == "cancel") { ClearLastLine();  continue; }
               move += " " + temp;
               return move;
            }

            else if (temp == "reset")
            {
               move += temp;
               ClearLastLine();
               Console.WriteLine("Which caravan would you like to reset? Cancel action with 'cancel'");
               temp = GetNumber(3);
               if (temp == "cancel") { ClearLastLine(); continue; }
               move += " " + temp;
               return move;
            }

            else
            {
               ClearLastLine();
               Console.WriteLine("Invalid command");
               continue;
            }
         }

         return move;
      }

      public string GetNumber(int size)
      {
         int n;
         string temp;

         while (true)
         {
            temp = Console.ReadLine();

            if (temp == "cancel")
            {
               ClearLastLine();
               return temp;
            }

            if (!int.TryParse(temp, out n))
            {
               Console.WriteLine("Invalid: Not a number. Try again.");
               continue;
            }

            if (n < 1 || n > size)
            {
               Console.WriteLine("Invalid: Must be between 1 and " + size + ". Try again.");
               continue;
            }

            if (temp.Contains(" "))
            {
               Console.WriteLine("Cannot contain spaces");
               continue;
            }

            break;
         }

         return temp;
      }

      public bool ValidateMove(string move)
      {
         if (move.Contains("play"))
         {
            string[] tokens = move.Split(' ');
            int h = Int32.Parse(tokens[1]);
            int n = Int32.Parse(tokens[2]);
            CaravanRow row = gameBoard.GetCaravan(n);
            Card card = myPlayer.CardAt(h);

            if (tokens.Length == 4)
            {
               int c = Int32.Parse(tokens[3]);

               if (card.GetRank() == Rank.Queen)
               {
                  if (row.GetSize() != c)
                  {
                     Console.WriteLine("Must play queen on last card.");
                     return false;
                  }
               }

               if (row.GetNumFaceCardsAt(c) >= 3)
               {
                  Console.WriteLine("Cannot attach more than 3 face cards to a number card");
               }
               return row.GetNumFaceCardsAt(c) < 3;
            }

            else if (tokens.Length == 3)
            {
               if (row.GetSize() == 0)
               {
                  return true;
               }

               if (card.GetRank() != row.GetEnd().GetRank() && card.GetSuite() == row.GetEnd().GetSuite())
               {
                  if (row.IsIncreasing() && card.GetValue() < row.GetEnd().GetValue())
                  {
                     row.FlipIncreasing();
                  }

                  else if (!row.IsIncreasing() && card.GetValue() > row.GetEnd().GetValue())
                  {
                     row.FlipIncreasing();
                  }
                  return true;
               }

               if (row.GetEnd().GetRank() == card.GetRank())
               {
                  Console.WriteLine("Cannot play same card value twice in a row");
                  return false;
               }

               if (row.GetSize() == 1)
               {
                  if (row.IsIncreasing() && card.GetValue() < row.GetEnd().GetValue())
                  {
                     row.FlipIncreasing();
                  }

                  else if (!row.IsIncreasing() && card.GetValue() > row.GetEnd().GetValue())
                  {
                     row.FlipIncreasing();
                  }
                  return true;
               }

               if (row.IsIncreasing())
               {
                  if (card.GetValue() > row.GetEnd().GetValue())
                  {
                     return true;
                  }

                  Console.WriteLine("Cannot play card of lower value");
                  return false;
               }

               else
               {
                  if (card.GetValue() < row.GetEnd().GetValue())
                  {
                     return true;
                  }

                  Console.WriteLine("Cannot play card of higher value");
                  return false;
               }
            }
         }

         return move.Contains("discard") || move.Contains("reset") || move == "give up";
      }

      public static void ClearLastLine()
      {
         Console.SetCursorPosition(0, Console.CursorTop - 1);
         Console.Write(new string(' ', Console.BufferWidth));
         Console.SetCursorPosition(0, Console.CursorTop - 1);
      }
   }
   class CardTest
   {
      //public static void Main(string[] args)
      //{
         //Deck myDeck = new Deck();
         //Console.WriteLine(myDeck.toString());

         //Game g = new Game();
         //g.Start(0);
      //}
   }
}
