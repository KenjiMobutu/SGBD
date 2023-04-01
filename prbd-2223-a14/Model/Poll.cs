using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using Castle.Components.DictionaryAdapter;
using PRBD_Framework;

namespace MyPoll.Model;
public enum PollType { Multiple, Single }

public class Poll : EntityBase<MyPollContext> {

    [Key]
    public int PollId { get; set; }
    [Required]
    public string Title { get; set; }
    public PollType Type  { get; set; }
    public Boolean IsClosed { get; set; }


    [ForeignKey(nameof(User))]
    public int  CreatorId { get; set; }
    //public virtual User Creator { get; set; }

    public Poll() { }

    public virtual ICollection<User> Participants { get; set; } = new HashSet<User>();
    public virtual ICollection<Participation> Participations{ get; set; } = new HashSet<Participation>();
    public virtual ICollection<Comment> Comments{ get; set; } = new HashSet<Comment>();
    public virtual ICollection<Choice> Choices{ get; set; } = new HashSet<Choice>();
    public virtual ICollection<Vote> Vote { get; set; } = new HashSet<Vote>();


}
