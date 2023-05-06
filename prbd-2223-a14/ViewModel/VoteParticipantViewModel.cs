﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.EntityFrameworkCore.Metadata;
using MyPoll.Model;
using MyPoll.ViewModel;
using PRBD_Framework;

namespace MyPoll.ViewModel;
public class VoteParticipantViewModel : ViewModelCommon {
    private List<User> _participants;
    public VoteParticipantViewModel(VoteGridViewModel voteGridViewModel,User participant, List<Choice> choices) {
        _participants = Context.Participants.ToList();
        _voteGridViewModel = voteGridViewModel;
        _choices = choices;
        Participant = participant;
        IsCurrentUser = CurrentUser == participant;
        RefreshVotes();
        UpdateVotes();
            EditCommand = new RelayCommand(() => EditMode = true);
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
            DeleteCommand = new RelayCommand(Delete);
        
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

    private void EditModeChanged() {
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
    public bool Editable => !EditMode && !ParentEditMode && (IsCurrentUser || IsAdmin) ;
  
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
            .Select(c => new VoteChoiceViewModel(Participant, c, Participant.VotesList.Any(v => v.Choice.ChoiceId == c.ChoiceId)))
            .ToList();
    }

    private void Save() {
        EditMode = false;
        // Get the current poll ID
        int pollId = _voteGridViewModel.Poll.PollId;
        // Filter the participant's existing votes to include only the votes for the current poll
        var existingVotes = Participant.VotesList.Where(v => v.Choice.Poll.PollId == pollId).ToList();
        // Replace only the existing votes for the current poll with the new votes from VotesVM
        foreach (var voteVM in VotesVM.Where(v => v.IsRegistrated)) {
            var existingVote = existingVotes.FirstOrDefault(v => v.Choice.ChoiceId == voteVM.Vote.Choice.ChoiceId);
            if (existingVote != null) {
               // existingVote.Value = voteVM.Vote.Value;
                existingVote.Type = voteVM.Vote.Type;
            } else {
                Participant.VotesList.Add(voteVM.Vote);
            }
        }
        Context.SaveChanges();
        // On recrée la liste VotesVM avec les nouvelles données
        RefreshVotes();
        UpdateVotes();
    }


    private void Cancel() {
        EditMode = false;
        // On recrée la liste RegistrationsVM avec les nouvelles données
        RefreshVotes();
    }

    private void Delete() {
        Participant.VotesList.Clear();
        Context.SaveChanges();
        // On recrée la liste RegistrationsVM avec les nouvelles données
        RefreshVotes();
    }
    private void UpdateVotes() {
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
            vote.IsRegistratedYes = vote.Vote.Type == VoteType.Yes && vote.IsRegistrated;
            vote.IsRegistratedNo = vote.Vote.Type == VoteType.No && vote.IsRegistrated;
            vote.IsRegistratedMaybe = vote.Vote.Type == VoteType.Maybe && vote.IsRegistrated;
        }
    }

}
