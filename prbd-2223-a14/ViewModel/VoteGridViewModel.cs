using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel;
public class VoteGridViewModel : ViewModelCommon {
    private Poll _poll;
    public Poll Poll {
        get => _poll;
        set => SetProperty(ref _poll, value);
    }

    private readonly Choice _choice;
    public Choice Choice {
        get => _choice;
        private init => SetProperty(ref _choice, value);
    }

    public VoteGridViewModel() {
       
        _choices = Context.Choices.OrderBy(c => c.Label).ToList();
        var participants = Context.Users.OrderBy(p => p.Name).ToList();
        _participantsVM = participants.Select(p => new VoteParticipantViewModel(this, p, _choices)).ToList();
    }

    public VoteGridViewModel(Poll poll) {
        Poll = poll;
        var pollId = poll.PollId;
        var choices = Choice.GetChoicesForGrid(pollId);
        Console.WriteLine("POLL_ID ===> "+pollId);
        foreach (var choice in choices.ToList()) {
            Console.WriteLine("Choice ID: " + choice.ChoiceId + ", Label: " + choice.Label);
        }

        _choices = choices != null ? choices.OrderBy(c => c.Label).ToList() : new List<Choice>();
        //var participants = Context.Participations.OrderBy(p => p.User.Name).ToList();
        var participants = Participation.GetParticipantOfGrid(pollId).OrderBy(p => p.User.Name).ToList();
        foreach (var p in participants.ToList()) {
            Console.WriteLine("NAME PARTICIPANTS ===> : " + p.User.Name );
        }
        _participantsVM = participants.Select(p => new VoteParticipantViewModel(this, p.User, _choices)).ToList();
    }


    private bool _editMode;
    public bool EditMode {
        get => _editMode;
        set => SetProperty(ref _editMode, value);
    }

    private List<Choice> _choices;
    public List<Choice> Choices => _choices;

    private List<VoteParticipantViewModel> _participantsVM;
    public List<VoteParticipantViewModel> ParticipantsVM => _participantsVM;
    public Poll poll => Poll;

    public void AskEditMode(bool editMode) {
        EditMode = editMode;
        foreach (var p in ParticipantsVM)
            p.Changes();
    }
}



