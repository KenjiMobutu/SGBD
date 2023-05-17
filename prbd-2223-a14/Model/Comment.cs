using System.ComponentModel.DataAnnotations;
using System;
using PRBD_Framework;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyPoll.Model;


public class Comment : EntityBase<MyPollContext> {
    [Key]
    public int CommentId { get; set; }
    public string Text { get; set; }
    public DateTime CreationDate { get; set; } = DateTime.Now;

    [ForeignKey(nameof(Poll))]
    public int PollId { get; set; }
    public virtual Poll Poll { get; set; }


    [ForeignKey(nameof(User))]
    public int UserId { get; set; }
    public virtual User User { get; set; }

    public Comment() { }
    
    public bool IsCreatedByUser(User currentUser) {
        return User == currentUser;
    }
    public bool IsDeletable { get; set; }
}
