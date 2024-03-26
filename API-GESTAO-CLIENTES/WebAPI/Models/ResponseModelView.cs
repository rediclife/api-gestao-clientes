using System.Security.Cryptography.Xml;

namespace WebAPI.Models
{
    public class ResponseModelView<T> where T : class
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<T> Data { get; set; }

        public ResponseModelView(bool success, string message, List<T> data = null)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        public ResponseModelView(bool success, string message, T data = null)
        {
            Success = success;
            Message = message;
            Data = new List<T> { data };
        }
    }

    public class ResponseModelView
    {
        public bool Success { get; set; }
        public string Message { get; set; }
       
        public ResponseModelView(bool success, string message)
        {
            Success = success;
            Message = message;
        }

    }
}
