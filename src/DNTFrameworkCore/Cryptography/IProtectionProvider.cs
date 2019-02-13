namespace DNTFrameworkCore.Cryptography
{
    /// <summary>
    /// Add it as services.AddSingleton(IProtectionProvider, ProtectionProvider)
    /// More info: http://www.dotnettips.info/post/2519
    ///  تولید توکن‌ها، رمزنگاری کوئری استرینگ‌ها و یا کوکی‌های کوتاه مدت
    /// </summary>
    public interface IProtectionProvider
    {
        /// <summary>
        /// Decrypts the message
        /// </summary>
        string Decrypt(string inputText);

        /// <summary>
        /// Encrypts the message
        /// </summary>
        string Encrypt(string inputText);
    }
}