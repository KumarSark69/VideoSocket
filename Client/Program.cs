using System.Net.Sockets;
using System.Text;
using System.Net;
namespace TestClients
{
    class testClient
    {
        public static void Main(string[] args)
        {
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEnd = new IPEndPoint(ipAddress, 1111);
            // IPEndPoint ipEnd = new IPEndPoint(new IPAddress(new byte[] { 172, 16, 0, 189 }), 8080);
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            clientSocket.Connect(ipEnd);
            Console.WriteLine("Connected Sucessfully");
            int maxPacketSize = 1024 * 1024 * 10;
            byte[] packetData = new byte[maxPacketSize];
            long totalsize = 0;
            string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "/test/";
            string[] fileEntries = Directory.GetFiles(path, args[0]);
            for (int i = 0; i < fileEntries.Length; i++)
            {
                Console.WriteLine(Path.GetFileName(fileEntries[i]));
                string fileName = Path.GetFileName(fileEntries[i]);
                byte[] fileNameByte = Encoding.ASCII.GetBytes(fileName);
                byte[] fileNameLen = BitConverter.GetBytes(fileNameByte.Length);

                Console.WriteLine(Encoding.ASCII.GetString(fileNameByte));
                using (FileStream fileStream = new(path + fileName, FileMode.Open))
                {
                    byte[] fileSize = BitConverter.GetBytes(fileStream.Length); ;
                    fileNameLen.CopyTo(packetData, 0);
                    fileSize.CopyTo(packetData, 4);
                    fileNameByte.CopyTo(packetData, 12);
                    Console.WriteLine(fileNameLen.Length + "  " + fileNameByte.Length + "  " + fileSize.Length);
                    // Thread.Sleep(5000);
                    int readCount = fileStream.Read(packetData, 12 + fileNameByte.Length, packetData.Length - (12 + fileNameByte.Length));

                    while (readCount > 12 + fileNameByte.Length)
                    {
                        // if (readCount <= 4 + fileNameByte.Length + fileStream.Length)
                        // {

                        clientSocket.Send(packetData);
                        Console.WriteLine(packetData.Length);
                        totalsize += packetData.Length;
                        fileNameLen.CopyTo(packetData, 0);
                        fileSize.CopyTo(packetData, 4);
                        fileNameByte.CopyTo(packetData, 12);
                        readCount = fileStream.Read(packetData, 12 + fileNameByte.Length, packetData.Length - (12 + fileNameByte.Length));
                        Console.WriteLine(readCount);
                        // }

                    }
                    totalsize = 0;
                }

            }
            // Close the clientSocket object once the file has been sent
            clientSocket.Close();
        }

    }
}