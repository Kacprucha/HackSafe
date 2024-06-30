using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

public static class SystemHelper
{
    public static bool IpIsCorrect (string ip)
    {
        bool result = false;

        string pattern = @"^(\d{1,3}\.){3}\d{1,3}$";
        var regex = new Regex (pattern);

        if (regex.IsMatch (ip))
        {
            string[] parts = ip.Split ('.');
            foreach (string part in parts)
            {
                if (int.TryParse (part, out int num))
                {
                    if (num < 0 || num > 255)
                        result = false;
                }
                else
                {
                    result = false;
                }
            }
            result = true;
        }

        return result;
    }

    public static string GenerateValidIp ()
    {
        Random rand = new Random ();
        return $"{rand.Next (0, 256)}.{rand.Next (0, 256)}.{rand.Next (0, 256)}.{rand.Next (0, 256)}";
    }

    public static string GenerateValidIpInNetwork (string networkPart)
    {
        Random rand = new Random ();
        string hostPart = $"{rand.Next (0, 256)}.{rand.Next (0, 256)}";

        return networkPart + "." + hostPart;
    }

    public static string GeneratePassword (int length)
    {
        const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lower = "abcdefghijklmnopqrstuvwxyz";
        const string digits = "0123456789";
        const string special = "!@#$%^&*()_+-=[]{}|;:,.<>?";

        var allChars = upper + lower + digits + special;
        var random = new Random ();
        var password = new StringBuilder ();

        password.Append (upper[random.Next (upper.Length)]);
        password.Append (lower[random.Next (lower.Length)]);
        password.Append (digits[random.Next (digits.Length)]);
        password.Append (special[random.Next (special.Length)]);

        for (int i = 4; i < length; i++)
        {
            password.Append (allChars[random.Next (allChars.Length)]);
        }

        return new string (password.ToString ().OrderBy (_ => random.Next ()).ToArray ());
    }

    public static bool CheckIfPathHasCorrectSyntex (string path)
    {
        bool result = true;

        if (string.IsNullOrWhiteSpace (path) || !path.StartsWith ("/"))
        {
            result = false;
        }
        else
        {
            string[] parts = path.Split ('/');
            foreach (string part in parts)
            {
                if (part == "" || part.Contains (" ") || part.Contains ("\\"))
                {
                    result = false;
                    break;
                }
            }
        }

        return result;
    }
}
