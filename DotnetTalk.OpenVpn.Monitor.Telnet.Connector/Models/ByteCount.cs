namespace DotnetTalk.OpenVpn.Monitor.Telnet.Connector.Models;
public class ByteCount
{
    public string ClienId { get; set; }

    public int ByteIn { get; set; }

    public int ByteOut { get; set; }

    public override string ToString()
    {
        return $"{nameof(ByteCount)} {nameof(ClienId)}: {ClienId}, {nameof(ByteIn)}: {ByteIn}, {nameof(ByteOut)}: {ByteOut}";
    }
}