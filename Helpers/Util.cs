using System.Text;

namespace HTNLShop.Helpers
{
    public class Util
    {
        public string GetRamdomKey(int length = 5)
        {
            var pattern = @"ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
                            "qwertyuiopasdfghjklzxcvbnm!@#$%^&*()-_+=[]{};:,.<>/?";
            var sb = new StringBuilder();
            var rd = new Random();
            for (int i = 0; i < length; i++)
            {
                sb.Append(pattern[rd.Next(0, pattern.Length)]);
            }
            return sb.ToString();
        }
    }
}
