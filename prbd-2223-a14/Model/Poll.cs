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

    //public virtual ICollection<User> Participants { get; set; } = new HashSet<User>();
    public virtual ICollection<User> Participants { get; set; } = new HashSet<User>().OrderBy(u => u.Name).ToList();

    //public virtual ICollection<Participation> Participations{ get; set; } = new HashSet<Participation>();
    public virtual ICollection<Comment> Comments{ get; set; } = new HashSet<Comment>();
    public virtual ICollection<Choice> Choices{ get; set; } = new HashSet<Choice>();
    // public virtual ICollection<Vote> Vote { get; set; } = new HashSet<Vote>();
    public virtual ICollection<Vote> Votes { get; set; } = new HashSet<Vote>();

    public static IQueryable<Poll> GetPolls(User CurrentUser) {
            var polls = Context.Polls.Where(poll =>
            poll.Creator.Mail == CurrentUser.Mail || poll.Participants.Contains(CurrentUser));
            return polls;
    }

    public static IQueryable<Poll> GetAllPolls() {
        return Context.Polls;
    }
    public static IQueryable<Poll> GetById(Poll PollId) {
        var poll = Context.Polls.Where(poll =>
        poll.PollId == PollId.PollId);
        return poll;
    }
    public static Dictionary<int, User> GetCreator(IEnumerable<Poll> polls) {
        var creatorIds = polls.Select(p => p.CreatorId).Distinct();
        var creators = Context.Users.Where(u => creatorIds.Contains(u.UserId)).ToDictionary(u => u.UserId);
        return creators;
    }

    public static IQueryable<Poll> GetFiltered(string Filter) {
        var filtered = from p in Context.Polls
                       where p.Title.Contains(Filter)
                       orderby p.Title
                       select p;
        return filtered;
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

    public double GetVotesCount(Choice choice) {
        return choice.VotesList.Sum(v => v.Value);
    }
    public int GetVotesCount() {
        int count = 0;
        foreach (var choice in Choices) {
            count += choice.VotesList.Count;
        }
        return count;
    }

    public int TotalVotesForUser(User user) {
        int totalVotes = 0;
        foreach (var choice in Choices) {
            totalVotes += choice.VotesList.Count(v => v.UserId == user.UserId);
        }
        return totalVotes;
    }

    public int VoteCount(Choice choice) {
        return choice.VotesList.Count;
    }
    public void Delete() {
      
        // Supprime le membre lui-même
        Context.Polls.Remove(this);
        Context.SaveChanges();
    }

}
