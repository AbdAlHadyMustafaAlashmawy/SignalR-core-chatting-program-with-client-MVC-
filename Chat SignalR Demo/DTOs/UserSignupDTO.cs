using System.ComponentModel.DataAnnotations;

namespace Chat_SignalR_Demo.DTOs
{
    public class UserSignupDTO
    {
        public string Name { get; set; }
        public string email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string Confirm_Password { get; set; }
    }
}
