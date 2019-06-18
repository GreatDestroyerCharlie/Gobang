using System;

class Start
{
    private static void Main()
    {
        string ip = NetworkUtils.GetLocalIPv4();
        new Network(ip);

        Console.WriteLine("服务器已启动!");
        Console.WriteLine($"ip地址为:{ip}");

        Console.ReadKey();
    }
}