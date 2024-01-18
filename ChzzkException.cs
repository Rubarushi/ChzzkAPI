using System.Net;

namespace ChzzkAPI
{
    public class ChzzkException : Exception
    {


        public ChzzkException(HttpStatusCode statusCode, string message) : base($"[{statusCode}] : {message}")
        {

        }

        public ChzzkException(string message) : base(message)
        {

        }
    }
}
