using NuGet.Packaging.Signing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace Chat_SignalR_Demo.Models
{
    public class Conversation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string Name { get; set; }
        
        public string Message { get; set; }
        public DateTime time { get; set; } = DateTime.Now;
        public kind Kind { get; set; } = kind.AllGroups;
        public string? Gname { get; set; } = null;
        public string? IsSender { get; set; }
        public bool Deleted { get; set; } = false;
    }
}
