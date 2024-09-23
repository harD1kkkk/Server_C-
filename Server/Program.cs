using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using MySql.Data.MySqlClient;

class ChatServer
{
    private static Dictionary<TcpClient, string> clients = new Dictionary<TcpClient, string>();
    private static MySqlConnection? dbConnection;

    static void Main(string[] args)
    {
        // Set up MySQL connection
        string connectionString = "Server=34.116.232.242;Database=mydatabase;User ID=myuser;Password=mypassword;";
        dbConnection = new MySqlConnection(connectionString);

        try
        {
            dbConnection.Open();
            Console.WriteLine("Database connected!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Database connection failed: " + ex.Message);
            return;
        }

        // Start listening for clients
        TcpListener server = new TcpListener(IPAddress.Any, 8080);
        server.Start();
        Console.WriteLine("Server started. Waiting for connections...");

        while (true)
        {
            TcpClient client = server.AcceptTcpClient();
            Thread clientThread = new Thread(HandleClient);
            clientThread.Start(client);
        }
    }

    private static void HandleClient(object obj)
    {
        TcpClient client = (TcpClient)obj;
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[1024];
        int bytesRead;

        // 1. Receive credentials (username and password)
        bytesRead = stream.Read(buffer, 0, buffer.Length);
        string credentials = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        string[] parts = credentials.Split(':');
        string username = parts[0];
        string password = parts[1];

        // 2. Authenticate the user against MySQL
        if (AuthenticateUser(username, password))
        {
            // Log the connection details
            DateTime connectionTime = DateTime.Now;
            clients.Add(client, username);
            LogConnection(username, connectionTime);

            // Send success response to client
            byte[] successMessage = Encoding.UTF8.GetBytes("OK");
            stream.Write(successMessage, 0, successMessage.Length);
            Console.WriteLine($"{username} connected.");

            // Start chat handling
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                BroadcastMessage($"{username}: {message}", client);
            }

            // Log disconnection
            DateTime disconnectionTime = DateTime.Now;
            LogDisconnection(username, disconnectionTime);
            Console.WriteLine($"{username} disconnected.");
            clients.Remove(client);
        }
        else
        {
            // Send error message
            byte[] errorMessage = Encoding.UTF8.GetBytes("ERROR");
            stream.Write(errorMessage, 0, errorMessage.Length);
            client.Close();
        }
    }

    private static bool AuthenticateUser(string username, string password)
    {
        string query = "SELECT COUNT(*) FROM users WHERE username = @username AND password = @password";
        MySqlCommand cmd = new MySqlCommand(query, dbConnection);
        cmd.Parameters.AddWithValue("@username", username);
        cmd.Parameters.AddWithValue("@password", password);

        int count = Convert.ToInt32(cmd.ExecuteScalar());
        return count > 0;
    }

    private static void LogConnection(string username, DateTime connectionTime)
    {
        string query = "INSERT INTO connections (username, connection_time) VALUES (@username, @connection_time)";
        MySqlCommand cmd = new MySqlCommand(query, dbConnection);
        cmd.Parameters.AddWithValue("@username", username);
        cmd.Parameters.AddWithValue("@connection_time", connectionTime);
        cmd.ExecuteNonQuery();
    }

    private static void LogDisconnection(string username, DateTime disconnectionTime)
    {
        string query = "UPDATE connections SET disconnection_time = @disconnection_time WHERE username = @username AND disconnection_time IS NULL";
        MySqlCommand cmd = new MySqlCommand(query, dbConnection);
        cmd.Parameters.AddWithValue("@username", username);
        cmd.Parameters.AddWithValue("@disconnection_time", disconnectionTime);
        cmd.ExecuteNonQuery();
    }

    private static void BroadcastMessage(string message, TcpClient excludeClient)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(message);
        foreach (var client in clients)
        {
            if (client.Key != excludeClient)
            {
                NetworkStream stream = client.Key.GetStream();
                stream.Write(buffer, 0, buffer.Length);
            }
        }
    }
}
