using System.ComponentModel.DataAnnotations;
using System;
using PRBD_Framework;

namespace MyPoll.Model;

public class Vote : EntityBase<MyPollContext> {
    public int VoteId { get; set; }
    enum Value { Yes, No, Maybe }
}
