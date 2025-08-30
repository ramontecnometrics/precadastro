using System;
using System.Security.Cryptography;
using System.Text;

namespace data
{
    public static class SearchableHelper
    {
        private static byte[] key = [4, 65, 78, 56, 32, 14, 0, 64];
        private const string ALPH = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public static string Build(string searchableText, string scope)
        {
            var result = default(string);
            if (searchableText == null)
            {
                return null;
            }
            result = Caesar(Normalize(searchableText), GetShift(scope));
            return result;
        }

        public static int GetShift(string scope)
        {
            using var h = new HMACSHA256(key);
            var bytes = h.ComputeHash(Encoding.UTF8.GetBytes(scope));
            return bytes[0] % 36; // 0..35
        }

        private static string Caesar(string text, int shift)
        {
            var buf = new char[text.Length];
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                int idx = ALPH.IndexOf(c);
                buf[i] = idx >= 0 ? ALPH[(idx + shift) % ALPH.Length] : c;
            }
            var s = new string(buf);

            var bytes = Encoding.UTF8.GetBytes(s);
            var result = BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
            return result;
        }

        private static string Normalize(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "";
            s = s.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var c in s.ToUpperInvariant())
            {
                if (c >= 'A' && c <= 'Z') sb.Append(c);
                else if (c >= '0' && c <= '9') sb.Append(c);
                else sb.Append(' ');
            }
            return string.Join(' ', sb.ToString().Split(' ', StringSplitOptions.RemoveEmptyEntries)).Replace(" ", "");
        }

        public static string Build(object searchable, string searchableScope)
        {
            throw new NotImplementedException();
        }
    }
}
