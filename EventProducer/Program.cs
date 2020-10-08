namespace EventProducer
{
	using System;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Net.Sockets;
	using System.Security.Cryptography;

	public class Program
	{
		public static void Main(string[] args)
		{
			const string Host = "localhost";
			const int Port = 5050;
			var client = new TcpClient(Host, Port);

			var stream = client.GetStream();

			var aes = Aes.Create();
			var key = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };
			var iv = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };

			var cryptoStream = new CryptoStream(stream, aes.CreateDecryptor(key, iv), CryptoStreamMode.Write);

			var writer = new StreamWriter(cryptoStream)
			{
				AutoFlush = true
			};

			Console.WriteLine($"Client connected to {Host} on port {Port}.");

			Console.WriteLine("Press any key to send an event.");

			while (true)
			{
				Console.ReadKey();

				var message = $"EVENT: {DateTime.Now:T}";

				Console.WriteLine($"Sending {message}");

				writer.Write(message);
				writer.Flush();
			}
		}
	}
}
