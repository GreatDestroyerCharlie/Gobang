using System;
using System.Collections.Generic;

/// <summary>
/// 消息封装类
/// </summary>
public class MessagePacker
{
    private List<byte> bytes = new List<byte>();

    public byte[] Package
    {
        get { return bytes.ToArray(); }
    }

    public MessagePacker Add(byte[] data)
    {
        bytes.AddRange(data);
        return this;
    }

    public MessagePacker Add(ushort value)
    {
        byte[] data = BitConverter.GetBytes(value);
        bytes.AddRange(data);
        return this;
    }

    public MessagePacker Add(uint value)
    {
        byte[] data = BitConverter.GetBytes(value);
        bytes.AddRange(data);
        return this;
    }

    public MessagePacker Add(ulong value)
    {
        byte[] data = BitConverter.GetBytes(value);
        bytes.AddRange(data);
        return this;
    }
}