using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Caravan
{
   public class Client
   {
      //static string SERVER_NAME = "TST-SurfaceBook";
      static int PORT = 3074;
      static bool exit = false;

      public static void HandleSession(Socket server)
      {
         exit = false;
         exit = Register(server);
         if (exit)
         {
            Console.WriteLine("Exiting...");
            server.Shutdown(SocketShutdown.Both);
            server.Close();
            return;
         }

         while (!exit)
         {
            Console.WriteLine("Please enter a command or 'help' to see a list of commands");
            string command = Console.ReadLine();
            HandleCommand(command, server);
         }

         Console.WriteLine("Exiting...");
      }

      public static bool Register(Socket server)
      {
         bool registered = false;
         Console.WriteLine("Please enter a username!");
         string user = Console.ReadLine();
         SendMessage(user, server);
         if (user == "exit")
         {
            return true;
         }

         string response = ReceiveMessage(server);

         while (!registered)
         {
            if (response == "0")
            {
               Console.WriteLine("Username is taken. Please choose a different name.");
               user = Console.ReadLine();
               SendMessage(user, server);
               if (user == "exit")
               {
                  return true;
               }

               response = ReceiveMessage(server);
            }
            else if (response == "invalid")
            {
               Console.WriteLine("Username is invalid. Cannot be empty or contain spaces");
               user = Console.ReadLine();
               SendMessage(user, server);
               if (user == "exit")
               {
                  return true;
               }

               response = ReceiveMessage(server);
            }
            else
            {
               registered = true;
            }
         }

         Console.WriteLine("Registration Complete");
         return false;
      }

      public static void HandleCommand(String command, Socket server)
      {
         string[] commands = command.Split(' ');
         if (commands[0] == "join" && commands.Length != 2)
         {
            Console.WriteLine("Invalid number of arguments");
            return;
         }

         switch (commands[0])
         {
            case "create":
               SendMessage(command, server);
               Create(server);
               break;
            case "list":
               SendMessage(command, server);
               List(server);
               break;
            case "join":
               SendMessage(command, server);
               Join(server);
               break;
            case "leaderboard":
               SendMessage(command, server);
               LeaderBoard(server);
               break;
            case "unregister":
               SendMessage(command, server);
               Unregister(server);
               break;
            case "exit":
               SendMessage(command, server);
               Exit(server);
               break;
            case "help":
               Help();
               break;
            case "rank":
               SendMessage(command, server);
               Rank(server);
               break;
            default:
               Console.WriteLine("Invalid command");
               break;
         }
      }

      public static void Help()
      {
         Console.WriteLine("'unregister' will remove your info from the server and you will be taken back to registration. Warning: You will be removed from the leaderboard as well.");
         Console.WriteLine("'exit' will unregister you and terminate the program");
         Console.WriteLine("'create' allows you to host a game. The game will wait 60 seconds for someone to join before timing out.");
         Console.WriteLine("'list' lets you see who is hosting a game.");
         Console.WriteLine("'join' allows you to join someone else's game. Just enter their username as a second argument.");
         Console.WriteLine("'leaderboard' lets you see who has the greatest net wins.");
         Console.WriteLine("'rank' gives your your rank and score on the leaderboard.");
      }

      public static void Rank(Socket server)
      {
         string r = ReceiveMessage(server);
         Console.WriteLine(r);
      }

      public static void Create(Socket server)
      {
         Console.WriteLine("Waiting...");
         string p2 = ReceiveMessage(server);

         if (p2 == "error" || p2 == "give up")
         {
            Console.WriteLine("No one joined...");
            return;
         }

         Console.WriteLine(p2 + " is trying to join your game");
         SendMessage(p2, server);

         string response = ReceiveMessage(server);
         if (response == "0")
         {
            Console.WriteLine(p2 + " could not connect");
            return;
         }

         else
         {
            Console.WriteLine(p2 + " has connected");
         }

         Game game = new Game(server);
         game.Start(0);
      }

      public static void List(Socket server)
      {
         string response = ReceiveMessage(server);
         Console.WriteLine("Available games:");
         Console.WriteLine(response);
      }

      public static void Join(Socket server)
      {
         string response = ReceiveMessage(server);

         if (response == "0")
         {
            Console.WriteLine("That game is unavailable");
            return;
         }

         else
         {
            Console.WriteLine("Joining game...");
         }

         Game game = new Game(server);
         game.Start(1);
      }

      public static void LeaderBoard(Socket server)
      {
         string response = ReceiveMessage(server);
         Console.WriteLine("Leaderboard: ");
         Console.WriteLine(response);
      }

      public static void Unregister(Socket server)
      {
         Console.Clear();
         HandleSession(server);
      }

      public static void Exit(Socket server)
      {
         server.Shutdown(SocketShutdown.Both);
         server.Close();
         exit = true;
      }

      public static void ExecuteClient()
      {
         try
         {
            //IPHostEntry ipHost = Dns.GetHostEntry(SERVER_NAME);
            //IPAddress ipAddr = ipHost.AddressList[0];
            Console.WriteLine("Please enter the server's ip address");
            string sip = Console.ReadLine();
            
            IPAddress ipAddr = IPAddress.Parse(sip);
            IPEndPoint endPoint = new IPEndPoint(ipAddr, PORT);

            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
               sender.Connect(endPoint);
               HandleSession(sender);
            }
            catch (Exception e)
            {
               Console.Clear();
               Console.WriteLine(e.Message);
               ClearLastLine();
               Console.WriteLine("Cannot connect to the server.");
               Console.WriteLine("Exiting...");
               System.Threading.Thread.Sleep(3000);
            }
         }
         catch (Exception e)
         {
            Console.Clear();
            Console.WriteLine(e.Message);
            ClearLastLine();
            Console.WriteLine("Cannot connect to the server.");
            Console.WriteLine("Exiting...");
            System.Threading.Thread.Sleep(3000);
         }
      }

      public static int SendMessage(string msg, Socket sender)
      {
         msg += "\r\n";
         byte[] message = Encoding.ASCII.GetBytes(msg);
         try
         {
            return sender.Send(message);
         }
         catch (Exception e)
         {
            Console.Clear();
            Console.WriteLine(e.Message);
            ClearLastLine();
            //Console.WriteLine("Connection to server lost.");
            return -1;
         }
      }

      public static string ReceiveMessage(Socket sender)
      {
         string msg = "";
         byte[] buffer = new byte[1024];

         while (true)
         {
            int numByte = sender.Receive(buffer);
            msg += Encoding.ASCII.GetString(buffer, 0, numByte);
            if (msg.IndexOf("\r\n") > -1) break;
         }

         msg = msg.Remove(msg.Length - 2, 2);
         return msg;
      }

      public static void ClearLastLine()
      {
         Console.SetCursorPosition(0, Console.CursorTop - 1);
         Console.Write(new string(' ', Console.BufferWidth));
         Console.SetCursorPosition(0, Console.CursorTop - 1);
      }

      public static void Main(String[] args)
      {
         Client.ExecuteClient();
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
   }
}