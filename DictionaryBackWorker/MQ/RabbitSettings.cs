namespace DictionaryBackWorker.MQ
{
    public class RabbitSettings
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public static readonly string SectionName = "RabbitSettings";
    }
}
