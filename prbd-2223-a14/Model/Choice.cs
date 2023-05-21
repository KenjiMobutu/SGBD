using System.ComponentModel.DataAnnotations;
using System;
using PRBD_Framework;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyPoll.Model;


public class Choice : EntityBase<MyPollContext> {
    [Key]
    public int ChoiceId { get; set; }
    public string Label { get; set; }

    public int PollId { get; set; }
    public virtual Poll Poll { get; set; }

    public virtual ICollection<Vote> VotesList { get; set; } = new HashSet<Vote>();

    [NotMapped]
    public double Score => VotesList.Sum(v => v.Value);
    public int TotalVotes => VotesList.Count;

    public static IQueryable<Choice> GetChoicesForGrid(int pollId) {
        var poll = Context.Polls.FirstOrDefault(p => p.PollId == pollId);
        if (poll == null) {
            throw new ArgumentException($"Poll with ID {pollId} does not exist.");
        }
        return Context.Choices.Where(c => c.PollId == pollId);
    }
    public Choice() { }

    public List<string> GetChoiceLabels() {
        List<string> choiceLabels = new List<string>();

        foreach (var choice in Poll.Choices) {
            choiceLabels.Add(choice.Label);
        }

        return choiceLabels;
    }

}
