namespace IdentityServer4.MongoDB.Utilities
{
    /// <summary>
    /// this class holds the extensions methods
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// check if the given string is not null or empty or white space
        /// </summary>
        /// <param name="value">the string value to be checked</param>
        /// <returns>true if valid, false if not</returns>
        public static bool IsValid(this string value)
            => !(string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value));
    }
}
