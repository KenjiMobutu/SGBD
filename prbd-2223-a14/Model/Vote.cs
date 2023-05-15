using System.ComponentModel.DataAnnotations;
using System;
using PRBD_Framework;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyPoll.Model;
public enum VoteType { Yes = 1, No = -1, Maybe  }

public class Vote : EntityBase<MyPollContext> {
    public VoteType Type { get; set; }

    [ForeignKey(nameof(User))]
    public int UserId { get; set; }
    public virtual User User { get; set; }

    [ForeignKey(nameof(Choice))]
    public int ChoiceId { get; set; }
    public virtual Choice Choice { get; set; }

    public Vote() { }

    public double Value {
        get {
            switch (Type) {
                case VoteType.Yes:
                    return 1;
                case VoteType.Maybe:
                    return 0.5;
                case VoteType.No:
                    return -1;
                default:
                    throw new Exception("Invalid vote type.");
            }
        }
    }
}


