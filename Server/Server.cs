using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.Json;
using STOPwithUDP.Words;

public class Server
{
    private UdpClient serverSocket = new UdpClient(4545, IPAddress.Any.AddressFamily);

    private Dictionary<IPEndPoint, int> Players = new Dictionary<IPEndPoint, int>();

    public void RunServer()
    {
        Console.WriteLine("SERVER ON");

        AddPlayers();

        char[] Alphabet =
        {
            'A',
            'B',
            'C',
            'D',
            'E',
            'F',
            'G',
            'H',
            'I',
            'J',
            'K',
            'L',
            'M',
            'N',
            'O',
            'P',
            'Q',
            'R',
            'S',
            'T',
            'U',
            'V',
            'W',
            'X',
            'Y',
            'Z'
        };

        Random random = new Random();
        char randomLetter = Alphabet[random.Next(0, Alphabet.Length)];
        string sendLetter;

        foreach (var player in Players)
        {
            sendLetter = ($"The letter choosen is: {randomLetter} and you are Player {player}");
            byte[] buffer = Encoding.UTF8.GetBytes(sendLetter);
            serverSocket.Send(buffer, buffer.Length, player.Key);
        }

        Tuple<int, IPEndPoint, int> Winner = new Tuple<int, IPEndPoint, int>(0, new IPEndPoint(0, 0), 0);

        Tuple<IPEndPoint, IPEndPoint> Draw = null;

        foreach (var player in Players)
        {
            Tuple<Words, IPEndPoint> words = ReceiveWords();

            var points = VerifyPoints(words.Item1, randomLetter);

            Console.WriteLine($"Player {Players[words.Item2]} from the ip {words.Item2} has {points} points");

            if (points > Winner.Item1)
            {
                Winner = new Tuple<int, IPEndPoint, int>(points, words.Item2, Players[words.Item2]);
                Draw = null;
            }
            else if (points == Winner.Item1)
            {
                Draw = new Tuple<IPEndPoint, IPEndPoint>(Winner.Item2, words.Item2);
            }
        }

        string result = "";

        if (Draw != null)
        {
            result =
                ($"Draw Between Player {Players[Draw.Item2]} ({Draw.Item2}) and Player {Players[Draw.Item1]} ({Draw.Item1})");
        }
        else
        {
            result = ($"AND THE WINNER IS.... PLAYER {Winner.Item3} ({Winner.Item2}) with {Winner.Item1} points");
        }

        foreach (var player in Players)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(result);
            serverSocket.Send(buffer, buffer.Length, player.Key);
        }
    }

    private int VerifyPoints(Words words, char letter)
    {
        PropertyInfo[] properties = typeof(Words).GetProperties();

        int points = 0;

        foreach (PropertyInfo property in properties)
        {
            string value = (string)property.GetValue(words);

            if (string.IsNullOrEmpty(value))
            {
                continue;
            }

            if (value[0] == letter)
            {
                points++;
            }
        }

        return points;
    }

    private Tuple<string, IPEndPoint> Listen()
    {
        IPEndPoint endpoint = null;

        byte[] bufferResponse = serverSocket.Receive(ref endpoint);
        string ResponseReceived = Encoding.UTF8.GetString(bufferResponse);
        Console.WriteLine($"Mensagem recebida de {endpoint}: {ResponseReceived}");

        return new Tuple<string, IPEndPoint>(ResponseReceived, endpoint);
    }

    private void AddPlayers()
    {
        int playerId = 1;

        while (playerId < 3)
        {
            Tuple<string, IPEndPoint> message = Listen();

            if (message.Item1 == "CONECTADO")
            {
                Players.Add(message.Item2, playerId);
                playerId++;
            }
        }
    }

    private Tuple<Words, IPEndPoint> ReceiveWords()
    {
        while (true)
        {
            Tuple<string, IPEndPoint> responseReceived = Listen();

            try
            {
                var words = JsonSerializer.Deserialize<Words>(responseReceived.Item1);

                if (words != null)
                {
                    return new Tuple<Words, IPEndPoint>(words, responseReceived.Item2);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ;
            }
        }
    }
}