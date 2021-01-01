using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

public class Server
{
   static Dictionary<string, Player> playerList = new Dictionary<string, Player>();
   static Dictionary<string, Player> gamesList = new Dictionary<string, Player>();
   static List<Player> leaderBoard = new List<Player>();
   static int PORT = 3074;
   
   // handle client's requests
   public static void HandleClientSession(Object obj)
   {
      Socket clientSocket = (Socket)obj;
      Player client = Register(clientSocket);

      if (client == null)
      {
         clientSocket.Shutdown(SocketShutdown.Both);
         clientSocket.Close();
         return;
      }

      string u = client.Username;

      while (!client.Exit)
      { 
         string command = ReceiveMessage(clientSocket);
         HandleCommand(command, client);
      }

      Console.WriteLine("Ending " + u + "'s thread");
   }

   public static Player Register(Socket socket)
   {
      Player client = null;

      while (client == null)
      {
         string username = ReceiveMessage(socket);
         if (username == "exit")
         {
            return null;
         }

         if (username.Contains(" ") || username.Length == 0)
         {
            SendMessage(socket, "invalid");
            continue;
         }

         if (playerList.ContainsKey(username))
         {
            SendMessage(socket, "0");
            continue;
         }

         client = new Player(username, socket);
         playerList.Add(username, client);
         leaderBoard.Add(client);
         int sent = SendMessage(socket, "1");
         if (sent == -1)
         {
            Error(client);
         }
         Console.WriteLine("Registered " + username);
      }

      return client;
   }

   public static void HandleCommand(string command, Player player)
   {
      Console.WriteLine(command);
      string[] commands = command.Split(' ');

      switch(commands[0])
      {
         case "create":
            CreateGame(player);
            break;
         case "list":
            ListGames(player);
            break;
         case "join":
            JoinGame(player, commands[1]);
            break;
         case "leaderboard":
            LeaderBoard(player);
            break;
         case "unregister":
            Unregister(player);
            break;
         case "exit":
            Exit(player);
            break;
         case "error":
            Error(player);
            break;
         case "rank":
            Rank(player);
            break;
         default:
            SendMessage(player.Socket, "unknown");
            break;
      }
   }

   public static void Rank(Player host)
   {
      string retVal = "Rank: ";
      int count = 1;

      for (int i = 0; i < leaderBoard.Count; i++)
      {
         if (leaderBoard[i].Username == host.Username)
         {
            break;
         }
         count++;
      }
      retVal += count;
      retVal += "   Score: " + host.Score;

      SendMessage(host.Socket, retVal);
   }

   public static void CreateGame(Player host)
   {
      gamesList.Add(host.Username, host);
      Console.WriteLine("Created game for " + host.Username);

      if (!host.Socket.Poll(60000000, SelectMode.SelectRead))
      {
         gamesList.Remove(host.Username);
         SendMessage(host.Socket, "error");
         Console.WriteLine("No one joined " + host.Username);
         return;
      }

      string player2 = ReceiveMessage(host.Socket);
      if (player2 == "error")
      {
         gamesList.Remove(host.Username);
         return;
      }

      Player other = playerList[player2];

      if (other == null)
      {
         Console.WriteLine(player2 + " failed to join " + host.Username + "'s game");
         SendMessage(host.Socket, "0");
         return;
      }

      Console.WriteLine(player2 + " has joined " + host.Username + "'s game");
      int s = SendMessage(host.Socket, "1");

      if (s == -1)
      {
         return;
      }

      bool win = HandleGame(host, other);
      host.UpdateScore(win);
   }

   public static void ListGames(Player player)
   {
      if (gamesList.Count == 0)
      {
         int sent = SendMessage(player.Socket, "No one is playing");
         if (sent == -1)
         {
            Error(player);
         }
         return;
      }

      String games = "";
      foreach(System.Collections.Generic.KeyValuePair<string, Server.Player> pair in gamesList)
      {
         Player p = pair.Value;
         games += p.Username + "\n";
      }

      SendMessage(player.Socket, games);
   }

   public static void JoinGame(Player player, string p2)
   {
      if(!gamesList.ContainsKey(p2))
      {
         int sent = SendMessage(player.Socket, "0");
         return;
      }

      Player host = gamesList[p2];
      gamesList.Remove(p2);
      int s = SendMessage(player.Socket, "1");
      if (s == -1)
      {
         SendMessage(host.Socket, "error");
         return;
      }

      SendMessage(host.Socket, player.Username);
      bool win = HandleGame(player, host);
      player.UpdateScore(win);
   }

   public static bool HandleGame(Player client, Player other)
   {
      string move;

      while(true)
      {
         move = ReceiveMessage(client.Socket);
         if (move == "win")
         {
            return true;
         }
         if (move == "lose")
         {
            return false;
         }
         if (move == "error" || move == "give up")
         {
            bool l = NotifyPlayer(other, move);
            return l;
         }

         try
         {
            Console.WriteLine(move);
            SendMessage(other.Socket, move);
         }
         catch (Exception e)
         {
            Console.WriteLine(e.Message);
            ClearLastLine();
            return true;
         }
      }
   }

   public static void LeaderBoard(Player player)
   {
      leaderBoard.Sort((x, y) => y.Score.CompareTo(x.Score));
      string lb = "";
      int place = 1;

      foreach (Player p in leaderBoard)
      {
         lb += place;
         lb += ". " + p.Username + ": ";
         lb += p.Score;
         lb += "\n";
         place++;
      }

      SendMessage(player.Socket, lb);
   }

   public static void Unregister(Player player)
   {
      playerList.Remove(player.Username);
      gamesList.Remove(player.Username);
      leaderBoard.Remove(player);
      HandleClientSession(player.Socket);
      player.Exit = true;
   }

   public static void Exit(Player player)
   {
      playerList.Remove(player.Username);
      gamesList.Remove(player.Username);
      leaderBoard.Remove(player);
      player.Socket.Shutdown(SocketShutdown.Both);
      player.Socket.Close();
      player.Exit = true;
   }

   public static void Error(Player player)
   {
      Console.WriteLine(player.Username + " disconnected");
      Exit(player);
   }

   public static bool NotifyPlayer(Player player, string msg)
   {
      try
      {
         SendMessage(player.Socket, msg);
         return false;
      }
      catch (Exception e)
      {
         Console.WriteLine(e.Message);
         ClearLastLine();
         return true;
      }
   }

   public static void ExecuteServer()
   {
      IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
      IPAddress ipAddr = ipHost.AddressList[0];
      Console.WriteLine(ipAddr.ToString());
      IPEndPoint endPoint = new IPEndPoint(ipAddr, PORT);

      Socket listener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

      try
      {
         listener.Bind(endPoint);

         listener.Listen(16);

         while (true)
         {
            Socket clientSocket = listener.Accept();

            Thread thread = new Thread(HandleClientSession);
            thread.Start(clientSocket);
         }
      }
      catch (Exception e)
      {
         Console.WriteLine(e.Message);
      }
   }

   public static int SendMessage(Socket s, string msg)
   {
      msg += "\r\n";
      byte[] message = Encoding.ASCII.GetBytes(msg);
      try
      {
         return s.Send(message);
      }
      catch (SocketException se)
      {
         Console.WriteLine(se.Message);
         ClearLastLine();
         return -1;
      }
   }

   public static string ReceiveMessage(Socket s)
   {
      string msg = "";
      byte[] buffer = new byte[1024];

      try
      {
         while (true)
         {
            int numByte = s.Receive(buffer);
            msg += Encoding.ASCII.GetString(buffer, 0, numByte);
            if (msg.IndexOf("\r\n") > -1) break;
         }
      }
      catch(SocketException se)
      {
         Console.WriteLine(se.Message);
         return "error";
      }
      msg = msg.Remove(msg.Length - 2, 2);
      return msg;
   }

   private static bool IsDisconnected(Socket s)
   {
      try
      {
         return s.Poll(10 * 1000, SelectMode.SelectRead) && (s.Available == 0);
      }
      catch (SocketException se)
      {
         // We got a socket error, assume it's disconnected
         Console.WriteLine(se.Message);
         return true;
      }
   }

   public static void ClearLastLine()
   {
      Console.SetCursorPosition(0, Console.CursorTop - 1);
      Console.Write(new string(' ', Console.BufferWidth));
      Console.SetCursorPosition(0, Console.CursorTop - 1);
   }

   public static void Main(String[] args)
   {
      ExecuteServer();
   }

   // Player class is for registering clients and leaderboard
   public class Player
   {
      public string Username { get; set; }
      public int Score { get; set; } // Score is used for leaderboard
      public Socket Socket { get; set; }
      public bool Exit { get; set; }

      public Player(string user, Socket sock)
      {
         this.Username = user;
         this.Score = 0;
         this.Socket = sock;
      }

      public void UpdateScore(bool win)
      {
         if (win == true)
         {
            Console.WriteLine(Username + " won");
            this.Score += 1;
         }

         else
         {
            Console.WriteLine(Username + " lost");
            this.Score -= 1;
         }
      }
   }
}
