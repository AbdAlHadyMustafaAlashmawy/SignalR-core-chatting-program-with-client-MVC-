using System.ComponentModel.DataAnnotations;

namespace Chat_SignalR_Demo.DTOs
{
    public class UserLoginDTO
    {
        public string Name { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool Remember_me { get; set; } = false;
    }
}
