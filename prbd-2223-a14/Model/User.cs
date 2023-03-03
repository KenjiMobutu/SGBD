using PRBD_Framework;

namespace MyPoll.Model;

public class User : EntityBase<MyPollContext> {
    public int UserId { get; set; }
    public string Name { get; set; }
    public string Mail { get; set; }
    public string Password { get; set; }
    public Boolean IsAdmin { get; set; }
}
