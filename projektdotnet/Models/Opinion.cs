using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace projektdotnet.Models
{
    public class Opinion
    {
        [Key] 
        public int OpinionId { get; set; }

        [ForeignKey("Sender")]
        public int? EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }

        [Required(ErrorMessage = "Please enter title"),
            Display(Name = "Title:"),
            MinLength(1, ErrorMessage = "Title has to be longer than 1 character"),
            MaxLength(50, ErrorMessage = "Title has to be shorter than 50 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Please enter description"),
            MinLength(5,ErrorMessage = "Description has to be longer than 5 characters"),
            MaxLength(500,ErrorMessage = "Description has to be shorter than 500 characters")]
        public string Description { get; set; }
    }
}
