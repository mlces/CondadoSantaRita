#pragma warning disable CS8603 // Posible tipo de valor devuelto de referencia nulo
using Mylces.ControlCenter;

namespace Infrastructure
{
    public static class Configuration
    {
        public const byte AplicationId = 1;

        private static Control Control { get; set; }

        static Configuration()
        {
            Control = new Control(AplicationId);
        }

        public static int LogAction(string message) => Control.LogAction(message);

        public static int LogError(string message) => Control.LogError(message);

        public static string TokenSigningKey => Control.GetConfiguration<string>(nameof(TokenSigningKey));
        public static string TokenIssuer => Control.GetConfiguration<string>(nameof(TokenIssuer));
        public static string TokenAudience => Control.GetConfiguration<string>(nameof(TokenAudience));
        public static string ConnectionString => Control.GetConfiguration<string>(nameof(ConnectionString));
        public static int TokenValidityMinutesAccess => Control.GetConfiguration<int>(nameof(TokenValidityMinutesAccess));
        public static bool Swagger => Control.GetConfiguration<bool>(nameof(Swagger));
        public static int TokenValidityMinutesReset => Control.GetConfiguration<int>(nameof(TokenValidityMinutesReset));
    }
}