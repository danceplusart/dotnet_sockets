using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    class Program
    {
        static List<Socket> list = new List<Socket>();
        static void Main(string[] args)
        {
            TcpListener listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 8086);
            listener.Start();
            Console.WriteLine("Ожидание клиентов");
            while(true)
            {
                Socket s = listener.AcceptSocket();
                Task t = new Task(() => ReadSocket(s));
                t.Start();
            }
        }

        static void ReadSocket(Socket socket)
        {
            byte[] buffer = new byte[2048];
            int count = socket.Receive(buffer);
            string name = Encoding.UTF8.GetString(buffer, 0, count);
            list.ForEach(s => s.Send(Encoding.UTF8.GetBytes("Подключился пользователь " + name)));
            Console.WriteLine("Подключился пользователь " + name);
            list.Add(socket);
            
            try
            {
                while(true)
                {
                    count = socket.Receive(buffer);
                    byte[] start = Encoding.UTF8.GetBytes(name + ": ");
                    byte[] send = new byte[start.Length + count];
                    Array.Copy(start, send, start.Length);
                    Array.Copy(buffer, 0, send, start.Length, count);
                    list.ForEach(s => s.Send(send));
                    Console.WriteLine("Сообщение от " + Encoding.UTF8.GetString(send));
                }
            }
            catch(SocketException)
            {
                list.Remove(socket);
                Console.WriteLine("Отключился " + name);
                list.ForEach(s => s.Send(Encoding.UTF8.GetBytes("Отключился " + name)));
            }
        }
    }
}
