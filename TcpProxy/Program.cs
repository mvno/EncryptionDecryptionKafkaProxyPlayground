namespace TcpProxy
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
		private static TcpListener server;

		public static async Task Main(string[] args)
		{
			var host = await Dns.GetHostEntryAsync("localhost");
			var ip = new IPAddress(host.AddressList.First().GetAddressBytes());
			const int Port = 5050;

			server = new TcpListener(ip, Port);
			server.Start();

			Console.WriteLine($"Server started on port {Port}.");

			var socket = await server.AcceptSocketAsync();

			Console.WriteLine($"Connected {socket.RemoteEndPoint}");

			var stream = new NetworkStream(socket);
			var aes = Aes.Create();
			var key = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };
			var iv = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };

			var cryptoStream = new CryptoStream(stream, aes.CreateDecryptor(key, iv), CryptoStreamMode.Read);
			var reader = new StreamReader(cryptoStream);

			while (true)
			{
				var message = await reader.ReadToEndAsync();

				Console.WriteLine($"Received: {message}");
			}
		}

		private static async Task SetupNewSocket()
		{
			var socket = await server.AcceptSocketAsync();

			Console.WriteLine($"Connected {socket.RemoteEndPoint}");

			var stream = new NetworkStream(socket);
			var aes = Aes.Create();
			var key = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };
			var iv = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };

			var cryptoStream = new CryptoStream(stream, aes.CreateDecryptor(key, iv), CryptoStreamMode.Read);
			var reader = new StreamReader(cryptoStream);

			while (true)
			{
				var message = await reader.ReadToEndAsync();

				Console.WriteLine($"Received: {message}");
			}
		}
	}
}
