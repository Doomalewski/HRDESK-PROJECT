namespace projektdotnet.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class TicketComment
    {
        [Key]
        public int TicketCommentId { get; set; }
        [Required]
        [ForeignKey("TicketId")]
        public int TicketId { get; set; }
        public Ticket Ticket { get; set; }
        [Required]
        [ForeignKey("EmployeeId")]
        public int EmployeeId { get; set; }
        [JsonIgnore]
    public Employee Employee { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }

        [Required(ErrorMessage = "Please enter description"), Display(Name = "Description:"),
                MinLength(10, ErrorMessage = "Description has to be longer than 10 characters"),
                MaxLength(500, ErrorMessage = "Description has to be shorter than 500 characters")]
        public string Description { get; set; }
    }
