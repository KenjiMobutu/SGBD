using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyPoll.Model;
using PRBD_Framework;
using FontAwesome6;
using System.Windows.Media;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MyPoll.ViewModel;
public class VoteChoiceViewModel : ViewModelCommon {
    public VoteChoiceViewModel(User participant, Choice choice, bool isRegistered, Poll poll){
        IsRegistrated = isRegistered;
        Participant = participant;
        Choice = choice;
        Poll = poll;
        Vote = participant.VotesList.FirstOrDefault(v => v.Choice.ChoiceId == choice.ChoiceId) ??
               new Vote { Choice = choice, User = participant };

        IsRegistratedYes = Vote.Type == VoteType.Yes && IsRegistrated;
        IsRegistratedNo = Vote.Type == VoteType.No && IsRegistrated;
        IsRegistratedMaybe = Vote.Type == VoteType.Maybe && IsRegistrated;
        IsRegistratedNone = Vote.Type == VoteType.None && IsRegistrated;

        if(Poll.Type == PollType.Single) {
            HasVotedCommand = new RelayCommand<object>(HasVotedSingle);
        } else {
            HasVotedCommand = new RelayCommand<object>(HasVoted);
        }
    }

    public Poll Poll { get; set; }
    public Choice Choice { get; set; }

    public Vote Vote { get; set; }

    public bool IsVoteYes { get; set; }
    public bool IsVoteNo { get; set; }
    public bool IsVoteMaybe { get; set; }

    public VoteChoiceViewModel() { }

    private bool _editMode;

    public bool EditMode {
        get => _editMode;
        set => SetProperty(ref _editMode, value);
    }
    private User _participant;

    public User Participant {
        get => _participant;
        set => SetProperty(ref _participant, value);
    }
    public RelayCommand<object> HasVotedCommand { get; set; }
    
    public RelayCommand<object> ClearChoicesCommand { get; set; }
  
    public void HasVoted(object parameter) {
        Console.WriteLine("RENTRE DANS Multiple");
        if (!EditMode) {
            return;
        }
        Console.WriteLine("Poll TYPE ==> " + Poll.Type);
        double newVote = Convert.ToDouble(parameter);
        Console.WriteLine(newVote);

        // Determine the new vote type
        VoteType newVoteType = newVote switch {
            1.0 => VoteType.Yes,
            -1.0 => VoteType.No,
            0.5 => VoteType.Maybe,
            0.0 => VoteType.None,
            _ => VoteType.Maybe
        };

        // Update the vote type
        Vote.Type = newVoteType;
        Console.WriteLine(Vote.Type);

        // Update the IsRegistrated properties
        IsRegistratedNo = Vote.Type == VoteType.No;
        IsRegistratedYes = Vote.Type == VoteType.Yes;
        IsRegistratedMaybe = Vote.Type == VoteType.Maybe;
        Console.WriteLine("IS Maybe ===> " + IsRegistratedMaybe);

        // Set IsRegistrated to true
        IsRegistrated = true;
    }

    public void HasVotedSingle(object parameter) {
        if (!EditMode) {
            return;
        }

        Console.WriteLine("RENTRE DANS SINGLE");
        Console.WriteLine("SINGLE ===>:" + Poll.Type);
        double newVote = Convert.ToDouble(parameter);

        // Determine the new vote type
        VoteType newVoteType = newVote switch {
            1.0 => VoteType.Yes,
            -1.0 => VoteType.No,
            0.5 => VoteType.Maybe,
            0.0 => VoteType.None,
            _ => VoteType.Maybe
        };

        // Remove all existing votes of the participant for the current poll
        var existingVotesForPoll = Participant.VotesList.Where(v => v.Choice.Poll == Poll).ToList();
        foreach (var existingVote in existingVotesForPoll) {
            Participant.VotesList.Remove(existingVote);
            existingVote.Choice.VotesList.Remove(existingVote);
        }

        // Create a new vote for the selected choice in the current poll
        Vote = new Vote { Choice = Choice, User = Participant, Type = newVoteType };
        Participant.VotesList.Add(Vote);
        Choice.VotesList.Add(Vote);

        // Update the IsRegistrated properties
        IsRegistratedNo = Vote.Type == VoteType.No;
        IsRegistratedYes = Vote.Type == VoteType.Yes;
        IsRegistratedMaybe = Vote.Type == VoteType.Maybe;

        // Set IsRegistrated to true
        IsRegistrated = true;

        // Disable the ability to vote for other choices
        
             
    }

    private bool _isRegistrated;
    public bool IsRegistrated {
        get => _isRegistrated;
        set => SetProperty(ref _isRegistrated, value);
    }

    private bool _isRegistratedYes;
    public bool IsRegistratedYes {
        get => _isRegistratedYes;
        set => SetProperty(ref _isRegistratedYes, value);
    }

    private bool _isRegistratedNo;
    public bool IsRegistratedNo {
        get => _isRegistratedNo;
        set => SetProperty(ref _isRegistratedNo, value);
    }

    private bool _isRegistratedMaybe;
    public bool IsRegistratedMaybe {
        get => _isRegistratedMaybe;
        set => SetProperty(ref _isRegistratedMaybe, value);
    }

    private bool _isRegistratedNone;
    public bool IsRegistratedNone {
        get => _isRegistratedNone;
        set => SetProperty(ref _isRegistratedNone, value);
    }
    
    public EFontAwesomeIcon RegistratedIcon => IsRegistratedYes  ? EFontAwesomeIcon.Solid_Check : EFontAwesomeIcon.None;

    public EFontAwesomeIcon RegistratedX => IsRegistratedNo  ? EFontAwesomeIcon.Solid_X : EFontAwesomeIcon.None;

    public EFontAwesomeIcon RegistratedQuestion => IsRegistratedMaybe ? EFontAwesomeIcon.Regular_CircleQuestion : EFontAwesomeIcon.None;

    public EFontAwesomeIcon RegistratedBan => IsRegistratedNone ? EFontAwesomeIcon.Solid_Ban : EFontAwesomeIcon.None;

    public Brush RegistratedColor => IsRegistratedYes ? Brushes.Green : Brushes.White;

    public Brush RegistratedColorRed => IsRegistratedNo ? Brushes.Red : Brushes.White;

    public Brush RegistratedColorOrange => IsRegistratedMaybe ? Brushes.Orange : Brushes.White;

    public string RegistratedToolTipYes => "Yes" ;
    public string RegistratedToolTipNo =>  "No" ;
    public string RegistratedToolTipMaybe => "Maybe" ;
    public string RegistratedToolTipReset => "Reset" ;

}


