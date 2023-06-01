using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.EntityFrameworkCore.Metadata;
using MyPoll.Model;
using MyPoll.ViewModel;
using PRBD_Framework;

namespace MyPoll.ViewModel;
public class VoteParticipantViewModel : ViewModelCommon {
    private List<User> _participants;
    public VoteParticipantViewModel(VoteGridViewModel voteGridViewModel,User participant, List<Choice> choices, Poll poll) {
        _participants = Context.Participants.ToList();
        _voteGridViewModel = voteGridViewModel;
        _choices = choices;
        Participant = participant;
        IsCurrentUser = CurrentUser == participant;
        IsClosed = poll.IsClosed;
        Poll = poll;
        RefreshVotes();
        UpdateVotes();
        EditCommand = new RelayCommand(() => EditMode = true);
        SaveCommand = new RelayCommand(Save);
        CancelCommand = new RelayCommand(Cancel);
        DeleteCommand = new RelayCommand(Delete);

        Register<Vote>(App.Messages.MSG_VOTE_CHANGED, vote => RefreshVotes());
        Register<Vote>(App.Messages.MSG_EDITMODE_CHANGED, vote => EditModeChanged());
    }

    private VoteGridViewModel _voteGridViewModel;
    public ICommand EditCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand DeleteCommand { get; }
    private List<Choice> _choices;

    public User Participant { get; }
    public bool IsCurrentUser { get; }

    private bool _editMode;

    // La visbilité des boutons de sauvegarde et d'annulation sont bindés sur cette propriété
    public bool EditMode {
        get => _editMode;
        set => SetProperty(ref _editMode, value, EditModeChanged);
    }
    private readonly Poll _poll;

    public Poll Poll {
        get => _poll;
        private init => SetProperty(ref _poll, value);
    }
    private bool _isClosed;
    public bool IsClosed {
        get => _isClosed;
        set => SetProperty(ref _isClosed, value);
    }

    public void EditModeChanged() {
        // Lorsqu'on change le mode d'édition de la ligne, on le signale à chaque cellule
        foreach (VoteChoiceViewModel vcVM in _choicesVM) {
            vcVM.EditMode = EditMode;
            
        }

        // On informe le parent qu'on change le mode d'édition de la ligne
        _voteGridViewModel.AskEditMode(EditMode);
    }
    public void Changes() {
        RaisePropertyChanged(nameof(Editable));
    }
    public bool Editable => !EditMode && !ParentEditMode && (IsCurrentUser || IsAdmin) && !IsClosed ;
  
    public bool ParentEditMode => _voteGridViewModel.EditMode;

    private List<VoteChoiceViewModel> _choicesVM = new();
    public List<VoteChoiceViewModel> VotesVM {
        get => _choicesVM;
        private set => SetProperty(ref _choicesVM, value);
    }

    private void RefreshVotes() {
        // On crée, pour chaque choix du sondage, un VoteChoiceViewModel qui sera utilisé par le VoteParticipantView
        // VotesVM est la liste qui servira de source pour la balise <ItemsControl>
        VotesVM = _choices
            .Select(c => new VoteChoiceViewModel(Participant, c, Participant.VotesList.Any(v => v.Choice.ChoiceId == c.ChoiceId), Poll))
            .ToList();
    }

    private void Save() {
        EditMode = false;
        // Get the current poll ID
        int pollId = _voteGridViewModel.Poll.PollId;

        // Filter the participant's existing votes to include only the votes for the current poll
        var existingVotes = Participant.VotesList.Where(v => v.Choice.Poll.PollId == pollId).ToList();

        // Create a new list to store the votes that should be added or updated
        var newVotes = new List<Vote>();

        // Replace only the existing votes for the current poll with the new votes from VotesVM
        foreach (var voteVM in VotesVM) {
            var existingVote = existingVotes.FirstOrDefault(v => v.Choice.ChoiceId == voteVM.Vote.Choice.ChoiceId);
            if (voteVM.IsRegistrated) {
                if (existingVote != null) {
                    // Update the existing vote
                    existingVote.Type = voteVM.Vote.Type;
                } else {
                    // Add a new vote
                    newVotes.Add(voteVM.Vote);
                }
            } else {
                if (existingVote != null) {
                    // Remove the existing vote
                    existingVotes.Remove(existingVote);
                    Context.Votes.Remove(existingVote);
                }
            }
        }

        // Add the new votes to the participant's votes list
        foreach (var vote in newVotes) {
            Participant.VotesList.Add(vote);
        }

        Context.SaveChanges();

        // On recrée la liste VotesVM avec les nouvelles données
        RefreshVotes();
        UpdateVotes();
        NotifyColleagues(ApplicationBaseMessages.MSG_REFRESH_DATA);
    }
  
    private void Cancel() {
        EditMode = false;
        // On recrée la liste RegistrationsVM avec les nouvelles données
        RefreshVotes();
    }

    private void Delete() {
        // Filtrer les votes pour le sondage actuel
        int pollId = _voteGridViewModel.Poll.PollId;
        var votesToDelete = Participant.VotesList.Where(v => v.Choice.Poll.PollId == pollId).ToList();

        // Supprimer les votes filtrés
        foreach (var vote in votesToDelete) {
            Context.Votes.Remove(vote);
        }

        Context.SaveChanges();

        // Mettre à jour les vues associées aux votes
        RefreshVotes();
        UpdateVotes();
        NotifyColleagues(ApplicationBaseMessages.MSG_REFRESH_DATA);
    }

    public void UpdateVotes() {
        foreach (var vote in VotesVM) {
            var participant = _participants.FirstOrDefault(p => p.UserId == vote.Vote.User.UserId);
            if (participant == null) {
                continue;
            }
            var choice = _choices.FirstOrDefault(c => c.ChoiceId == vote.Vote.Choice.ChoiceId);
            if (choice == null) {
                continue;
            }
            vote.IsRegistrated = participant.VotesList.Any(v => v.Choice.ChoiceId == choice.ChoiceId);
            vote.Vote = participant.VotesList.FirstOrDefault(v => v.Choice.ChoiceId == choice.ChoiceId) ??
                        new Vote() { Choice = choice, User = participant };
            vote.IsVoteYes = vote.Vote.Value == 1;
            vote.IsVoteNo = vote.Vote.Value == -1;
            vote.IsVoteMaybe = vote.Vote.Value == 0.5;
        }
    }



}
