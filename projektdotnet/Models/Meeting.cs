using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace projektdotnet.Models
{
    public class Meeting
    {
        [Key] 
        public int MeetingId { get; set; }


        [ForeignKey("Room")]
        public int RoomId { get; set; }
        public Room room { get; set; }
        public ICollection<Employee> Participants { get; set; } = new List<Employee>();


        [Required(ErrorMessage = "Please enter title"), 
            Display(Name = "Title:"),
            MinLength(1, ErrorMessage = "Please enter title"), 
            MaxLength(50,ErrorMessage = "Title has to be shorter than 50 characters")]
        public string Title { get; set; }



        [Required(ErrorMessage = "Please enter description"),
            Display(Name = "Description:"),
            MinLength(10,ErrorMessage = "Description has to be longer than 10 characters"),
            MaxLength(500,ErrorMessage = "Description has to be shorter than 500 characters")]
        public string Description { get; set; }




        [Required(ErrorMessage = "Please enter starting date"),
            Display(Name = "Starting:"),
            DataType(DataType.DateTime),
            DisplayFormat(DataFormatString = "{0:MM/dd/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime StartingTime { get; set; }



        [Required(ErrorMessage = "Please enter ending date"),
            Display(Name = "Ending:"),
            DataType(DataType.DateTime),
            DisplayFormat(DataFormatString = "{0:MM/dd/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime EndingTime { get; set; }
    }
}
