using System.Security.Cryptography;
using System.Text;


public static class Hash
{
    public static string ToHash(string data, string salt)
    {
        SHA256Managed sha = new SHA256Managed();
        byte[] dataToBytes = Encoding.UTF8.GetBytes(data + salt);
        byte[] hash = sha.ComputeHash(dataToBytes);
        return HashBytesToString(hash);
    }

    private static string HashBytesToString(byte[] hash)
    {
        string str = string.Empty;
        foreach (byte b in hash)
            str += b.ToString("x2");
        return str;
    }
}
