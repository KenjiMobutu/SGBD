using System.ComponentModel.DataAnnotations;
using System;
using PRBD_Framework;

namespace MyPoll.Model;


public class Comment : EntityBase<MyPollContext> {
    public int CommentId { get; set; }
    public string Text { get; set; }
    public DateTime CreationDate { get; set; } = DateTime.Now;

    public virtual User Author { get; set; }
}
