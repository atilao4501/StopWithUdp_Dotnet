using System.Threading;

class Program
{

    static void Main(string[] args)
    {
        // Server Server = new();
        // Client Client = new();
        //
        // Thread RodarServidor = new Thread(Server.RunServer);
        // Thread RodarClient1 = new Thread(Client.RunClient);
        // Thread RodarClient2 = new Thread(Client.RunClient);
        //
        // RodarServidor.Start();
        // RodarClient1.Start();
        // RodarClient2.Start();
        
        Console.WriteLine("Client = 1, Server = 2");
        var option = Console.ReadLine();

        if (option == "2")
        {
            Server Server = new();
            Server.RunServer();
        }
        else
        {
            Client Client = new();
            Client.RunClient();
        }
        
        while(true);
        
    }
}