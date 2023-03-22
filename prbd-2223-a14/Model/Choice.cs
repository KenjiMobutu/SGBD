using System.ComponentModel.DataAnnotations;
using System;
using PRBD_Framework;

namespace MyPoll.Model;


public class Choice : EntityBase<MyPollContext> {
    [Key]
    public int ChoiceId { get; set; }
    public string Label { get; set; }
}
