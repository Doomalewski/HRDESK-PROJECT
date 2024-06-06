using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace projektdotnet.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        public ICollection<Meeting> Meetings { get; set; } = new List<Meeting>();

        public ICollection<Ticket> SentTickets { get; set; } = new List<Ticket>();

        public ICollection<Ticket> ReceivedTickets { get; set; } = new List<Ticket>();

        public ICollection<Role> Roles { get; set; } = new List<Role>();

        [Required(ErrorMessage = "Please enter login"),
            MinLength(5,ErrorMessage ="Login must be at least 5 characters"),
            MaxLength(20,ErrorMessage ="login has to be shorter than 20 characters")]
        public string Login {  get; set; }


        [Required(ErrorMessage ="Please enter password"),
            DataType(DataType.Password),
            MinLength(5, ErrorMessage = "Password must be at least 5 characters"),
            MaxLength(200, ErrorMessage = "Password has to be shorter than 200 characters")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please enter employee's name"),
            Display(Name = "Name:"),
            MinLength(1, ErrorMessage = "Name has to be longer than 1 character"),
            MaxLength(20, ErrorMessage = "Name has to be shorter than 20 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter employee's surname"),
            Display(Name = "Surname:"),
            MinLength(1, ErrorMessage = "Please enter surname"),
            MaxLength(35, ErrorMessage = "Name has to be shorter than 35 characters")]
        public string Surname { get; set; }

    }
}
