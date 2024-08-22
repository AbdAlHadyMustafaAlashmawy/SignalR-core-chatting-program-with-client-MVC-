using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Chat_SignalR_Demo.SignalR_Database
{
    public class UserConnection
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string UserId { get; set; }
        public string ConnectionId { get; set; }

    }
}
