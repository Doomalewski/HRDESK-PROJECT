
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace projektdotnet.Models
{
    public class Ticket
    {
        [Key]
        public int TicketId { get; set; }
        [ForeignKey("Employee")]
        public int? SenderId { get; set; }
        [JsonIgnore]
        public virtual Employee Sender { get; set; }
        [ForeignKey("Employee")]
        public int? ReceiverId { get; set; }
        [JsonIgnore]
        public virtual Employee Receiver { get; set; }
        [Required(ErrorMessage = "Please choose category"),
            Display(Name = "Category:")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TicketCategory Category { get; set; }
        [Required(ErrorMessage = "Please choose ticket status"),
            Display(Name = "Status:")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TicketStatus Status { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TicketPriority Priority { get; set; }

        public DateTime CreationDate { get; set; }
        [Required(ErrorMessage = "Please enter description"),
            Display(Name = "Description:"),
            MinLength(1, ErrorMessage = "Description should be longer than 5 characters"),
            MaxLength(500, ErrorMessage = "Description should be shorter than 500 characters")]
        public string Description { get; set; }
        public ICollection<TicketComment> Comments { get; set; } = new List<TicketComment>();
        public Ticket()
        {
            CreationDate = DateTime.Now;
            Status = 0;
        }
    }
}
