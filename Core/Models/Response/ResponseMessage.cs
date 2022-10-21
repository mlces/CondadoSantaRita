namespace Core.Models.Response
{
    public class ResponseMessage
    {
        public static readonly string AnErrorHasOccurred = "Se ha producido un error, inténtalo de nuevo.";
        public static readonly string DisabledUser = "Usuario deshabilitado, contacte a un administrador.";
        public static readonly string WrongCredentials = "Las credenciales son incorrectas.";
        public static readonly string EnterANewPassword = "Introduzca una nueva contraseña para obtener acceso.";
        public static readonly string TimeLimitExceeded = "Límite de tiempo excedido, inténtalo de nuevo.";

        public static string AnErrorHasOccurredAndId(int errorId) => $"Se ha producido un error, inténtalo de nuevo o reporta el error {errorId}.";
    }
}