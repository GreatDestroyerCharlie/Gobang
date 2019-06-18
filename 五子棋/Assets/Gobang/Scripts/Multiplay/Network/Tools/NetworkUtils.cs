using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
/// <summary>
/// 网络工具类 <see langword="static"/>
/// </summary>
public static class NetworkUtils
{
    /// <summary>
    /// obj -> bytes, 如果obj未被标记为 [Serializable] 则返回null
    /// </summary>
    public static byte[] Serialize(object obj)
    {
        //物体不为空且可被序列化
        if (obj == null || !obj.GetType().IsSerializable)
            return null;
        BinaryFormatter formatter = new BinaryFormatter();
        using (MemoryStream stream = new MemoryStream())
        {
            formatter.Serialize(stream, obj);
            byte[] data = stream.ToArray();
            return data;
        }
    }

    /// <summary>
    /// bytes -> obj, 如果obj未被标记为 [Serializable] 则返回null
    /// </summary>
    public static T Deserialize<T>(byte[] data) where T : class
    {
        //数据不为空且T是可序列化的类型
        if (data == null || !typeof(T).IsSerializable)
            return null;
        BinaryFormatter formatter = new BinaryFormatter();
        using (MemoryStream stream = new MemoryStream(data))
        {
            object obj = formatter.Deserialize(stream);
            return obj as T;
        }
    }

    /// <summary>
    /// 获取本机IPv4,获取失败则返回null
    /// </summary>
    public static string GetLocalIPv4()
    {
        string hostName = Dns.GetHostName(); //得到主机名
        IPHostEntry iPEntry = Dns.GetHostEntry(hostName);
        for (int i = 0; i < iPEntry.AddressList.Length; i++)
        {
            //从IP地址列表中筛选出IPv4类型的IP地址
            if (iPEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                return iPEntry.AddressList[i].ToString();
        }
        return null;
    }

    /// <summary>
    /// 比特数组 -> 字符串
    /// </summary>
    public static string Byte2String(byte[] bytes)
    {
        return Encoding.UTF8.GetString(bytes);
    }

    /// <summary>
    /// 字符串 -> 比特数组
    /// </summary>
    public static byte[] String2Byte(string str)
    {
        return Encoding.UTF8.GetBytes(str);
    }
}