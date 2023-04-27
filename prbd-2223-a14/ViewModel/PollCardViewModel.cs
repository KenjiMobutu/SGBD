using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel;

public class PollCardViewModel : ViewModelCommon {
    private readonly Poll _poll;

    public Poll Poll {
        get => _poll;
        private init => SetProperty(ref _poll, value);
    }

    public PollCardViewModel(Poll poll, User creator) {
        Poll = poll;
    }

    public string Name => Poll.Title;
    public PollType Type => Poll.Type;
    public int CreatorId => Poll.CreatorId;
    public User Creator => Poll.Creator;
    public int ParticipantsCount => Poll.Participants.Count;
    public IEnumerable<Choice> BestChoices => Poll.BestChoices;
    [NotMapped]
    public double VotesSum => BestChoices.Sum(c => c.VotesList.Sum(v => v.Value));
    public SolidColorBrush BackgroundColor {
        get {
            return Poll.IsClosed ? new SolidColorBrush(Color.FromRgb(255, 230, 220)) : new SolidColorBrush(Colors.White);
        }
    }





    public double VotesCount => Poll.GetVotesCount();  
    public PollCardViewModel(Poll poll) {
        Poll = poll;
    }
}
