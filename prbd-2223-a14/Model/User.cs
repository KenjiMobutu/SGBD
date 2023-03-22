using System.ComponentModel.DataAnnotations;
//using Castle.Components.DictionaryAdapter;
using PRBD_Framework;

namespace MyPoll.Model;

public class User : EntityBase<MyPollContext> {

    [Key]
    public int UserId { get; set; }
    [Required]
    public string Name { get; set; }
    public string Mail { get; set; }
    
    public string Password { get; set; }
   
    //public Boolean IsAdmin { get; set; }

}
