using System.Text;
namespace Identity.Settings
{
    public class TokenSettings
    {
        public string SecretKey { get; set; } = "";

        public byte[] GetSecret() => Encoding.ASCII.GetBytes(SecretKey);
    }
}