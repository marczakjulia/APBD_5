using System.Text.RegularExpressions;

namespace APBD5
{
    /// <summary>
    /// Interface for assessing the validity of an IP address via the use of regex.
    /// </summary>
    public static class IpAddressValidator
    {
        private static readonly Regex IpRegex = new Regex("^((25[0-5]|(2[0-4]|1\\d|[1-9]|)\\d)\\.?\\b){4}$");

        /// <summary>
        /// Function for checking if the IP address is valid.
        /// </summary>
        /// <param name="ipAddress">IP address which is later compared to regex.</param>
        public static bool IsValid(string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
                return false;

            return IpRegex.IsMatch(ipAddress);
        }
    }
}