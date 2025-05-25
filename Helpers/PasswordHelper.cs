
using System;
using System.Security.Cryptography;
using System.Text;

public static class PasswordHelper
{
    public static byte[] HashPasswordAsBytes(string password)
    {
        using (SHA256 sha = SHA256.Create())
        {
            return sha.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }

    public static string HashPassword(string password)
    {
        var hashedBytes = HashPasswordAsBytes(password);
        return Convert.ToBase64String(hashedBytes); // Or store raw bytes if needed
    }

    public static bool VerifyPassword(string plainPassword, string storedBase64Password)
    {
        string hashedInput = HashPassword(plainPassword);
        return hashedInput == storedBase64Password;
    }
}
