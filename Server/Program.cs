using System.Net.Sockets;
using System.Text;
using System.Net;

namespace VideoTransferSocketServer
{

    class server

    {
        public static void Main(string[] args)
        {

            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            //  IPEndPoint ipEnd = new IPEndPoint(new IPAddress(new byte[] { 172, 16, 0, 189 }), 8080);
            Console.WriteLine("Starting TCP listener...");
            IPEndPoint ipEnd = new IPEndPoint(ipAddress, 1111);
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP); ;
            serverSocket.Bind(ipEnd);
            int counter = 0;
            serverSocket.Listen(3004);
            Console.WriteLine(" >> " + "Server Started");
            while (true)

            {

                counter += 1;


                Socket clientSocket = serverSocket.Accept();

                Console.WriteLine(" >> " + "Client No:" + Convert.ToString(counter) + " started");


                new Thread(delegate ()
                {
                    doChat(clientSocket, Convert.ToString(counter));
                }).Start();




            }


        }

        public static void doChat(Socket clientSocket, string n)
        {
            int maxPacketSize = 1024 * 1024 * 10;


            byte[] buffer = new byte[maxPacketSize];

            int bytesRead = 0;
            long lastPacketSize = 0;
            string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "/recieve";
            int ch = 0;
            //FileStream fileStream = new FileStream(fileName, FileMode.OpenOrCreate);
            int totalBytesRead = 0;

            while (true)
            {

                bytesRead = 0;
                totalBytesRead = 0;
                clientSocket.Receive(buffer, bytesRead, SocketFlags.None);
                // Console.WriteLine(bytesRead);
                while ((bytesRead = clientSocket.Receive(buffer, totalBytesRead,
                             buffer.Length - totalBytesRead, SocketFlags.None)) != 0)
                {
                    totalBytesRead += bytesRead;
                }

                if (totalBytesRead == 0)
                {

                    break;
                }
                int fileNameLen = BitConverter.ToInt32(buffer, 0);
                int fileSize = BitConverter.ToInt32(buffer, 4);
                string fileName = Encoding.ASCII.GetString(buffer, 12, fileNameLen);

                Console.WriteLine("fileSize:" + fileSize);
                if (!Directory.Exists(path))
                {
                    Console.WriteLine("File " + path + " does not exists!");
                    // Ensure that the parent directory exists.
                    Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "/recieve");
                }
                if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "/recieve/" + fileName))
                {
                    ch = 1;
                }
                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "/recieve/" + fileName) && ch == 0)
                {
                    File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "/recieve/" + fileName);
                    ch = 1;
                }
                FileStream fileStream = new FileStream(path + "/" + fileName, FileMode.Append);


                if (fileStream.Length + maxPacketSize - (12 + fileNameLen) >= fileSize)
                {
                    lastPacketSize = fileSize + 12 + fileNameLen - fileStream.Length;

                }

                if (lastPacketSize == 0)
                    fileStream.Write(buffer, 12 + fileNameLen, totalBytesRead - 12 - fileNameLen);
                else
                    fileStream.Write(buffer, 12 + fileNameLen, (int)lastPacketSize - 12 - fileNameLen);
                Console.WriteLine(totalBytesRead + "   " + fileStream.Length);

                Console.WriteLine(fileName);
            }

        }
    }
}
