using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using STOPwithUDP.Words;

class Client
{
    public void RunClient()
    {
        UdpClient clientSocket = new UdpClient();
        byte[] buffer;
        string SendResponse;
        string ResponseReceived;
        
        IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Loopback, 4545);

        SendResponse = "CONECTADO";
        buffer = Encoding.UTF8.GetBytes(SendResponse); 
        clientSocket.Send(buffer, buffer.Length, serverEndPoint);  
        
        buffer = clientSocket.Receive(ref serverEndPoint);
        ResponseReceived = Encoding.UTF8.GetString(buffer);
        Console.WriteLine($"Resposta recebida do servidor: {ResponseReceived}");

        Words words = new Words();
        
        foreach (var property in typeof(Words).GetProperties())
        {
            string propertyName = property.Name;
            
            Console.WriteLine($"Enter Value for {propertyName}");

            string propertyValue = Console.ReadLine().Replace(" ", "").ToUpper();
            
            property.SetValue(words, propertyValue);

        }

        string jsonString = JsonSerializer.Serialize(words);
        
        buffer = Encoding.ASCII.GetBytes(jsonString);
        clientSocket.Send(buffer, buffer.Length, serverEndPoint);
        
        buffer = clientSocket.Receive(ref serverEndPoint);
        ResponseReceived = Encoding.UTF8.GetString(buffer);
        Console.WriteLine($"Resposta recebida do servidor: {ResponseReceived}");
    }
}