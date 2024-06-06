using System.ComponentModel.DataAnnotations;

namespace projektdotnet.Models
{
    public class Room
    {
        [Key]
        public int RoomId { get; set; }

        [Required(ErrorMessage="Please enter room number"),
            Display(Name = "Number:")]
        public int RoomNumber { get; set; }

        [Required(ErrorMessage = "Please enter room level"),
            Display(Name = "Level:")]
        public int RoomLevel { get; set; }
        [Required,Display(Name = "Seats:")]
        public int NumberOfSeats { get; set; }
        [Required,Display(Name = "Name:")]

        public string? Name { get; set; }
        [Display(Name = "Description:")]

        public string? Description { get; set; }


    }
}
