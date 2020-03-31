namespace FLUtils
{
    using System;
    using System.Globalization;
    using System.Text;
    using System.Text.RegularExpressions;

    public static class StringUtils
    {
        public static string SplitPascalCase(this string convert) => Regex.Replace(Regex.Replace(convert, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");

        public static int CountOccurrences(this string val, string match) => Regex.Matches(val, match, RegexOptions.IgnoreCase).Count;

        public static string Truncate(this string s, int length) => (string.IsNullOrEmpty(s) || length <= 0) ? string.Empty : s.Length > length ? s.Substring(0, length) + "..." : s;

        public static string TruncateAtWord(this string s, int length) => 
            (s == null || s.Length < length || s.IndexOf(" ", length, StringComparison.Ordinal) == -1) ? s : s.Substring(0, s.IndexOf(" ", length, StringComparison.Ordinal));

        public static bool IsEmailAddress(this string email) => Regex.Match(email, "^[a-zA-Z][\\w\\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\\w\\.-]*[a-zA-Z0-9]\\.[a-zA-Z][a-zA-Z\\.]*[a-zA-Z]$").Success;

        /// <summary>
        /// Detect whether a datetime string is in a valid format or not
        /// </summary>
        /// <param name="data">The time string.</param>
        /// <param name="format">The format you are trying to match against.</param>
        /// <returns>Boolean</returns>
        public static bool IsDateTime(this string data, string format) => DateTime.TryParseExact(data, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateVal);

        private static uint[] createIDTable = null;

        /// <summary>
        /// Function for calculating the Freelancer data nickname hash.
        /// </summary>
        /// <param name="nickname">The nickname you want to get a hash from.</param>
        /// <returns>Unsigned Integer</returns>
        public static uint CreateId(string nickname)
        {
            const uint FLHASH_POLYNOMIAL = 0xA001;
            const int LOGICAL_BITS = 30;
            const int PHYSICAL_BITS = 32;

            // Build the crc lookup table if it hasn't been created
            if (createIDTable == null)
            {
                createIDTable = new uint[256];
                for (uint i = 0; i < 256; i++)
                {
                    uint x = i;
                    for (uint bit = 0; bit < 8; bit++)
                        x = ((x & 1) == 1) ? (x >> 1) ^ (FLHASH_POLYNOMIAL << (LOGICAL_BITS - 16)) : x >> 1;
                    createIDTable[i] = x;
                }
            }

            byte[] name = Encoding.ASCII.GetBytes(nickname.ToLowerInvariant());

            // Calculate the hash.
            uint hash = 0;
            for (int i = 0; i < name.Length; i++)
                hash = (hash >> 8) ^ createIDTable[(byte)hash ^ name[i]];

            // broken because byte swapping is not the same as bit reversing, but that's just the way it is; two hash bits are shifted out and lost
            hash = (hash >> 24) | ((hash >> 8) & 0x0000FF00) | ((hash << 8) & 0x00FF0000) | (hash << 24);
            hash = (hash >> (PHYSICAL_BITS - LOGICAL_BITS)) | 0x80000000;

            return hash;
        }

        private static uint[] createFactionIDTable = null;

        /// <summary>
        /// Function for calculating the Freelancer data nickname hash.
        /// </summary>
        /// <param name="nickname">The faction nickname to convert into a faction hash</param>
        /// <returns>Unsigned Integer</returns>
        public static uint CreateFactionId(string nickname)
        {
            const uint FLFACHASH_POLYNOMIAL = 0x1021;
            const uint NUM_BITS = 8;
            const int HASH_TABLE_SIZE = 256;

            if (createFactionIDTable == null)
            {
                // The hash table used is the standard CRC-16-CCITT Lookup table 
                // using the standard big-endian polynomial of 0x1021.
                createFactionIDTable = new uint[HASH_TABLE_SIZE];
                for (uint i = 0; i < HASH_TABLE_SIZE; i++)
                {
                    uint x = i << (16 - (int)NUM_BITS);
                    for (uint j = 0; j < NUM_BITS; j++)
                    {
                        x = ((x & 0x8000) == 0x8000) ? (x << 1) ^ FLFACHASH_POLYNOMIAL : (x << 1);
                        x &= 0xFFFF;
                    }
                    createFactionIDTable[i] = x;
                }
            }

            byte[] tNickName = Encoding.ASCII.GetBytes(nickname.ToLowerInvariant());

            uint hash = 0xFFFF;
            for (uint i = 0; i < tNickName.Length; i++)
            {
                uint y = (hash & 0xFF00) >> 8;
                hash = y ^ (createFactionIDTable[(hash & 0x00FF) ^ tNickName[i]]);
            }

            return hash;
        }


    }
}
