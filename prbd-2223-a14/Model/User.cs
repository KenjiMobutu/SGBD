using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using Castle.Components.DictionaryAdapter;
using PRBD_Framework;

namespace MyPoll.Model;

public enum Role {
    Member = 1, Admin = 2
}
public class User : EntityBase<MyPollContext> {

    [Key]
    public int UserId { get; set; }
    [Required]
    public string Name { get; set; }
    public string Mail { get; set; }
    public string Password { get; set; }
    public Role Role { get; protected set; } = Role.Member;

    public User( int userId, string name, string mail, string password) {
            UserId = userId;
            Name = name;
            Mail = mail;
            Password = password;

        }
    public User() { }

    public virtual ICollection<Poll> Participations { get; set; } = new HashSet<Poll>();
    //public virtual ICollection<Poll> UserPollList { get; set; } = new HashSet<Poll>();
    //public virtual ICollection<Participation> PartcipantsList { get; set; } = new HashSet<Participation>();
    //public virtual ICollection<Choice> ChoicesList { get; set; } = new HashSet<Choice>();

    [InverseProperty(nameof(Poll.Creator))]
    public virtual ICollection<Poll> Polls { get; set; } = new HashSet<Poll>();
    public virtual ICollection<Comment> CommentsList { get; set; } = new HashSet<Comment>();
    public virtual ICollection<Vote> VotesList { get; set; } = new HashSet<Vote>();
    
    

}
