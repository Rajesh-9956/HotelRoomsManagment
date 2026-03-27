using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel_Rooms_MVC;

public class BedNumber
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int bedNo { get; set; }
    [ForeignKey("Room")]
    public int RoomId { get; set; }
    public DateOnly CreatedDate { get; set; }
    public string specialDetails { get; set; }
    public DateOnly UpdatedDate { get; set; }
    
    public virtual Room Room { get; set; }
}