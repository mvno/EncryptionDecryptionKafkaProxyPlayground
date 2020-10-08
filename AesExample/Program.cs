namespace AesExample
{
	using System;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Net.Sockets;
	using System.Security.Cryptography;
	using System.Threading.Tasks;

	public class Program
	{
		private const int Port = 5050;

		private const string HostName = "localhost";

		public Aes Aes { get; set; }

		public Program()
		{
			Aes = Aes.Create();
		}

		public static Task Main()
			=> new Program().ExecuteAsync();

		public async Task ExecuteAsync()
		{
			var original = "Here is some data to encrypt!";

			var listener = await SetupListenerAsync();
			var client = SetupClient();

			var socket = await SetupListenerClientAsync(listener);

			Console.WriteLine($"Plaintext to send is: {original}");

			// Encrypt and send the string
			EncryptAndSend(original, socket);

			// Decrypt the bytes to a string.
			var roundtrip = ReceiveAndDecrypt(client);

			//Display the original data and the decrypted data.
			Console.WriteLine("Original:   {0}", original);
			Console.WriteLine("Round Trip: {0}", roundtrip);
		}

		private static async Task<Socket> SetupListenerClientAsync(TcpListener listener)
		{
			var socket = await listener.AcceptSocketAsync();

			Console.WriteLine($"Client connected from {socket.RemoteEndPoint}.");

			return socket;
		}

		private static async Task<TcpListener> SetupListenerAsync()
		{
			var host = await Dns.GetHostEntryAsync(HostName);
			var ip = new IPAddress(host.AddressList.First().GetAddressBytes());
			var listener = new TcpListener(ip, Port);
			listener.Start();

			Console.WriteLine($"Listener running on host {HostName} on port {Port}.");

			return listener;
		}

		private static TcpClient SetupClient()
		{
			var client = new TcpClient(HostName, Port);

			Console.WriteLine($"Client started on host {HostName} on port {Port}.");

			return client;
		}

		public void EncryptAndSend(string plainText, Socket socket)
		{
			// Create an encryptor to perform the stream transform.
			var encryptor = Aes.CreateEncryptor();

			// Create the streams used for encryption.
			using var networkStream = new NetworkStream(socket);
			using var csEncrypt = new CryptoStream(networkStream, encryptor, CryptoStreamMode.Write);
			using var swEncrypt = new StreamWriter(csEncrypt);

			//Write all data to the stream.
			swEncrypt.Write(plainText);
			swEncrypt.Flush();
			csEncrypt.Flush();
			networkStream.Flush();

			Console.WriteLine("Message encrypted and flushed.");
		}

		public string ReceiveAndDecrypt(TcpClient client)
		{
			// Create a decryptor to perform the stream transform.
			var decryptor = Aes.CreateDecryptor();

			// Create the streams used for decryption.
			using var csDecrypt = new CryptoStream(client.GetStream(), decryptor, CryptoStreamMode.Read);
			using var srDecrypt = new StreamReader(csDecrypt);

			Console.WriteLine("Reading...");

			while (true)
			{
				Console.Write((char) srDecrypt.Read());
			}

			// Read the decrypted bytes from the decrypting stream
			return srDecrypt.ReadToEnd();
		}
	}
}
