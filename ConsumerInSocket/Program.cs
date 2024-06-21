using System.Net.NetworkInformation;

namespace ConsumerSocket
{
    class Program
    {
        static void Main(string[] args)
    {
        NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
        foreach (NetworkInterface nic in interfaces)
        {
            Console.WriteLine("Interface Name: {0}", nic.Name);
            Console.WriteLine("   Type: {0}", nic.NetworkInterfaceType);
            Console.WriteLine("   Speed: {0}", nic.Speed);
        }
        
    }
    }
}