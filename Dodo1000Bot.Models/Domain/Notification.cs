using System.Security.Cryptography;
using System.Text;

namespace Dodo1000Bot.Models.Domain
{
    public class Notification
    {
        public int Id { get; set; }

        public NotificationPayload Payload { get; set; }

        public string Distinction 
        {
            get
            {
                using var md5 = MD5.Create();
                var bytes = Encoding.Default.GetBytes(Payload.ToString());
                {
                    var hashBytes = md5.ComputeHash(bytes);
                    var hash = Encoding.Default.GetString(md5.ComputeHash(hashBytes));

                    return hash;
                }
            }
        }
    }
}