using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyPoll.View;
using MyPoll.ViewModel;
//using Castle.Components.DictionaryAdapter;
using PRBD_Framework;

namespace MyPoll.Model;
public enum PollType { Multiple, Single }

public class Poll : EntityBase<MyPollContext> {

    [Key]
    public int PollId { get; set; }
    [Required]
    public string Title { get; set; }
    public PollType Type  { get; set; }
    public Boolean IsClosed { get; set; }


    [Required,ForeignKey(nameof(Creator))]
    public int  CreatorId { get; set; }
    public virtual User Creator { get; set; }

    [NotMapped]
    public bool IsOpen => !IsClosed;

    public Poll() { }

    public virtual ICollection<User> Participants { get; set; } = new HashSet<User>().OrderBy(u => u.Name).ToList();
    public virtual ICollection<Comment> Comments{ get; set; } = new HashSet<Comment>();
    public virtual ICollection<Choice> Choices{ get; set; } = new HashSet<Choice>();
    public virtual ICollection<Vote> Votes { get; set; } = new HashSet<Vote>();

    public static IQueryable<Poll> GetPolls(User CurrentUser) {
            var polls = Context.Polls.Where(poll =>
            poll.Creator.Mail == CurrentUser.Mail || poll.Participants.Contains(CurrentUser));
            return polls;
    }

    public static IQueryable<Poll> GetAllPolls() {
        return Context.Polls;
    }

    [NotMapped]
    public IEnumerable<Choice> BestChoices {
        get {
            if (Choices.Count == 0) return new List<Choice>();
            var maxScore = Choices.Select(c => c.VotesList.Sum(v => v.Value)).Max();
            if (maxScore == 0) return new List<Choice>();
            var choices = Choices.Where(c => c.VotesList.Sum(v => v.Value) == maxScore).ToList();

            return choices;
        }
    }

    public int GetVotesCount() {
        int count = 0;
        foreach (var choice in Choices) {
            count += choice.VotesList.Count;
        }
        return count;
    }

    public void Delete() {
      
        Context.Polls.Remove(this);
        Context.SaveChanges();
    }

}
