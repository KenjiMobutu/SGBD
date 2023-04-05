using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq; 
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PRBD_Framework;

namespace MyPoll.Model;

public class Participation : EntityBase<MyPollContext> {
    [ForeignKey(nameof(Poll))]
    public int PollId { get; set; }
    public virtual Poll Poll { get; set; }


    [ForeignKey(nameof(User))]
    public int UserId { get; set; }
    public virtual User User { get; set; }

    //public virtual ICollection<User> Participants { get; set; } = new HashSet<User>();
    //public virtual ICollection<Poll> FollowedBy { get; set; } = new HashSet<Poll>();

    public Participation() { }

}
