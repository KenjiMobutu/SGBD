using System.ComponentModel.DataAnnotations;
//using Castle.Components.DictionaryAdapter;
using PRBD_Framework;

namespace MyPoll.Model;

public class Poll : EntityBase<MyPollContext> {

    [Key]
    public int PollId { get; set; }
    public string Title { get; set; }
    public enum Type {Simple,Multiple }
    public Boolean IsClosed { get; set; }

}
