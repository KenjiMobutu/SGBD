using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using Castle.Components.DictionaryAdapter;
using PRBD_Framework;

namespace MyPoll.Model;
public enum PollType { Single, Multiple }

public class Poll : EntityBase<MyPollContext> {

    [Key]
    public int PollId { get; set; }
    public string Title { get; set; }
    public PollType Type  { get; set; }
    public Boolean IsClosed { get; set; }


    [ForeignKey(nameof(User))]
    public int  CreatorId { get; set; }
    public virtual User Creator { get; set; }

    //public virtual Comment Comment { get; set; }


    public virtual ICollection<Participation> Participants{ get; set; } = new HashSet<Participation>();
    public virtual ICollection<Comment> Comments{ get; set; } = new HashSet<Comment>();
    public virtual ICollection<Choice> choices{ get; set; } = new HashSet<Choice>();

}
