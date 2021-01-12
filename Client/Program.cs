using System;
using System.Text;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static bool connect = false;
        static void Main(string[] args)
        {
            Console.Write("Введите имя пользователя: ");
            string name = Console.ReadLine();
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            connect = true;
            s.Connect("127.0.0.1", 8086);
            s.Send(Encoding.UTF8.GetBytes(name));
            Task t = new Task(() => ReadSocket(s));
            t.Start();
            while(connect)
                s.Send(Encoding.UTF8.GetBytes(Console.ReadLine()));
        }

        static void ReadSocket(Socket socket)
        {
            byte[] buffer = new byte[2048];
            int count;
            try
            {
                while(true)
                {
                    count = socket.Receive(buffer);
                    Console.WriteLine(Encoding.UTF8.GetString(buffer, 0, count));
                }
            }
            catch(SocketException)
            {
                connect = false;
                Console.WriteLine("Отключение от сервера");
            }
        }
    }
}
