﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;
using MyPoll.Model;
using MyPoll.View;
using System.Collections.Specialized;
using PRBD_Framework;



namespace MyPoll.ViewModel;
public class PollAddViewModel : ViewModelCommon {
    public PollAddViewModel() {
    }
    public ICommand Save { get; set; }
    public ICommand Cancel { get; set; }
    public ICommand CancelCommand { get; set; }
    public ICommand Delete { get; set; }
    public ICommand SaveCommand { get; set; }
    public ICommand EditChoiceCommand { get; set; }
    private PollDetailViewModel _pollDetailViewModel;

    public PollDetailViewModel PollDetailViewModel {
        get { return _pollDetailViewModel; }
        set { SetProperty(ref _pollDetailViewModel, value); }
    }

    private Poll _poll;

    public Poll Poll {
        get => _poll;
        set => SetProperty(ref _poll, value);
    }
    public ChoiceViewModel PollChoice { get; } = new();
    private Choice _choice;
    public Choice Choice {
        get => _choice;
        set =>SetProperty(ref _choice, value,()=> PollChoice.Choice = value); 
    }

   
    public string PollTitle {
        get => Poll?.Title;
        set => SetProperty(Poll.Title, value,Poll, (p,v) => {
            p.Title = v;   
            NotifyColleagues(App.Messages.MSG_TITLE_CHANGED, Poll);
            ValidateTitle();
        });
    }
    public string Label {
        get => Choice.Label;
        set => SetProperty(Choice.Label, value, Choice, (c, v) => {
            c.Label = v;
            Validate();
            
        });
    }

    public bool ValidateTitle() {
        ClearErrors();

        if (string.IsNullOrEmpty(PollTitle)) {
            AddError(nameof(PollTitle), "Title is required");
        } else if (PollTitle.Length < 3) {
            AddError(nameof(PollTitle), "length must be >= 3");
        } else if (PollTitle.TrimStart() != PollTitle) {
            AddError(nameof(PollTitle), "cannot start with a space");
        } 

        return !HasErrors;
    }

    public PollAddViewModel(Poll poll) {
        Poll = poll;
    }
    private bool _isNew;
    public bool IsNew {
        get => _isNew;
        set => SetProperty(ref _isNew, value);
    }
    public bool IsExisting => !_isNew;
    public string Title => Poll.Title;
    public User Creator => Poll.Creator;
    public bool CanAddCurrentUser => !Poll.Participants.Any(p => p.UserId == CurrentUser.UserId);

    public int VoteCount(Choice choice) {
        Console.WriteLine("TotalVotesForPoll 1");
        return choice.VotesList.Count;
    }

    private User _selectedUserToAdd;

    public User SelectedUserToAdd {
        get { return _selectedUserToAdd; }
        set {
            _selectedUserToAdd = value;
            
            RaisePropertyChanged(nameof(SelectedUserToAdd));
        }
    }

    private ICommand _addSelectedUserCommand;
    public ICommand AddSelectedUserCommand {
        get {
            if (_addSelectedUserCommand == null) {
                _addSelectedUserCommand = new RelayCommand(
                    () => this.AddSelectedUser(),
                    () => this.CanAddSelectedUser()
                );
                
            }
            return _addSelectedUserCommand;
        }
       
    }

    private ICommand _addAllUsersCommand;
    public ICommand AddAllUsersCommand {
        get {
            if (_addAllUsersCommand == null) {
                _addAllUsersCommand = new RelayCommand(() => AddAllUsers(),
                    () => true);
            }
            return _addAllUsersCommand;
        }
    }
    
    public ICommand AddCurrentUserCommand { get; set; }

    public ICommand DeleteParticipantCommand { get; set; }

    private void DeleteParticipant(int userId) {
        var participant = Poll.Participants.FirstOrDefault(p => p.UserId == userId);
        Console.WriteLine("User à EFFACER  ===> " + participant.Name);

        if (participant != null) {
            // Vérifier si le participant a déjà voté
            if (NbVotesForUser(participant) > 0) {
                // Afficher une boîte de dialogue de confirmation
                var result = MessageBox.Show("Le participant a voté au moins une fois. Êtes-vous sûr de vouloir le supprimer ?",
                    "Confirmation de suppression", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.No) {
                    // L'utilisateur a annulé la suppression, vous pouvez sortir de la méthode sans supprimer le participant
                    return;
                }
            }

            // Supprimer le participant
            Poll.Participants.Remove(participant);
            Participants.Remove(participant);
        }
        Context.SaveChanges();
        RaisePropertyChanged();
        RaisePropertyChanged(nameof(Participants));
    }

    private bool CanDeleteParticipant(int userId) {
        return Poll.Participants.Any(p => p.UserId == userId);
    }
    
    private bool CanAddSelectedUser() {
        // Vérifier si l'utilisateur sélectionné n'est pas déjà dans la liste des participants
        return SelectedUserToAdd != null && !Poll.Participants.Contains(SelectedUserToAdd);
    }

    private void AddSelectedUser() {
        // Ajouter l'utilisateur sélectionné à la liste des participants
        if (SelectedUserToAdd != null) {
            Poll.Participants.Add(SelectedUserToAdd);
            Participants.Add(SelectedUserToAdd);
            Context.SaveChanges();
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(Participants));
            NotifyColleagues(ApplicationBaseMessages.MSG_REFRESH_DATA);
        }
    }

    public List<User> AllUsers {
        get {
            var currentParticipants = Poll.Participants.Select(p => p.UserId);
            return Context.Users
                .Where(u => !currentParticipants.Contains(u.UserId))
                .OrderBy(u => u.Name)
                .ToList();
        }
    }

    private void AddAllUsers() {
        foreach (var user in AllUsers) {
            Poll.Participants.Add( user);
            Participants.Add(user);
        }
        // Mise à jour de la liste des participants
        RaisePropertyChanged();
        RaisePropertyChanged(nameof(Participants));
    }
 
    private void AddCurrentUser() {
        Poll.Participants.Add(CurrentUser);
        Participants.Add(CurrentUser);
        RaisePropertyChanged();
        RaisePropertyChanged(nameof(Participants));
    }
    public void SaveChoiceAction() {
        EditChoiceVisibility = true;
        IsEditingVisibility = false;
        EditingChoice = false;
        Context.SaveChanges();
        RaisePropertyChanged();
      
    }

    public override void SaveAction() {
        if (IsNew) {
            Poll.CreatorId = CurrentUser.UserId;
            Poll.Title = PollTitle;
            Context.Add(Poll);
            IsNew = false;
        } else {
            Context.Update(Poll);
        }
        if (!ValidateTitle()) {
            // Afficher un message d'erreur ou prendre une autre action en cas de validation échouée
            return;
        }
        
        Context.SaveChanges();
        RaisePropertyChanged();
        NotifyColleagues(App.Messages.MSG_POLL_CHANGED, Poll);
        NotifyColleagues(App.Messages.MSG_CLOSE_TAB, Poll);
        NotifyColleagues(App.Messages.MSG_DISPLAY_POLL, Poll);

    }

    private bool CanSaveAction() {
        Console.WriteLine("Can SAVE ACTION ===>" + IsNew);
        if (IsNew)
            return !string.IsNullOrEmpty(PollTitle) && !HasErrors;
        return Poll != null && PollTitle != null && !HasErrors && Poll.IsModified; 
    }
    private bool CanCancelAction() {
            return Poll != null && (IsNew || Poll.IsModified);
    }
    public override void CancelAction() {
        if (IsNew) {
            Console.WriteLine($"New {PollTitle}");
            
            NotifyColleagues(App.Messages.MSG_CLOSE_TAB, Poll);
            IsNew = false;
        } else {
            ClearErrors();
            Poll.Reload();
            RaisePropertyChanged();
        }
    }

    private ICommand _addChoiceCommand;
    public ICommand AddChoiceCommand {
        get => _addChoiceCommand;
        set => SetProperty(ref _addChoiceCommand, value);
    }
    

    public ObservableCollection<EditChoiceView> EditChoiceViews { get; } = new ObservableCollection<EditChoiceView>();
    private ObservableCollection<EditChoiceView> _editChoiceViews;
    public ObservableCollection<EditChoiceView> EditChoice {
        get => _editChoiceViews;
        set => SetProperty(ref _editChoiceViews, value);
    }
    private ObservableCollection<User> _participants;
    public ObservableCollection<User> Participants {
        get => _participants;
        set => SetProperty(ref _participants, value);
    }
    private ObservableCollection<Choice> _choices;
    public ObservableCollection<Choice> Choices {
        get => _choices;
        set => SetProperty(ref _choices, value);
    }

    private bool _isClosed;
    public bool IsClosed {
        get => _isClosed;
        set {
            SetProperty(ref _isClosed, value);
            if (_isClosed) {
                Poll.IsClosed = true;
            }
        }
    }
    private bool _isEditing;
    public bool IsEditingPoll {
        get => _isEditing;
        set => SetProperty(ref _isEditing, value);
    }

    private ObservableCollection<PollType> _pollTypes;
    public ObservableCollection<PollType> PollTypes {
        get => _pollTypes;
        set => SetProperty(ref _pollTypes, value);

    }
    private PollType _selectedType;
    public PollType SelectedType {
        get { return _selectedType; }
        set {
            SetProperty(ref _selectedType, value);
            Poll.Type = value;
        }
    }
    
    private bool _editChoiceVisibility;
    public bool EditChoiceVisibility {
        get => _editChoiceVisibility;
        set => SetProperty(ref _editChoiceVisibility, value);
    }

    private bool _isEditingVisibility;
    public bool IsEditingVisibility {
        get => _isEditingVisibility;
        set => SetProperty(ref _isEditingVisibility, value);
    }
    private bool _isEditingChoice;
    public bool IsEditingChoice {
        get => _isEditingChoice;
        set => SetProperty(ref _isEditingChoice, value);
    }
    private bool _editingChoice;
    public bool EditingChoice {
        get => _editingChoice; 
        set => SetProperty(ref _editingChoice, value); 
    }
    private bool _editMode;
    public bool EditMode {
        get => _editMode;
        set => SetProperty(ref _editMode, value, EditModeChanged);
    }
    public void EditModeChanged() {
        Console.WriteLine("EDIT MODE CHANGED");
        // Lorsqu'on change le mode d'édition de la ligne, on le signale à chaque cellule
        foreach (Choice choice in Choices) {
            choice.IsEditing = EditMode;
        }
    }

    public PollAddViewModel(Poll poll, bool isNew) {
        Poll = poll;
        IsNew = isNew;
        PollTitle = Poll.Title;
        EditChoiceVisibility = true;
        IsEditingVisibility = false;
        EditingChoice = false;
        IsEditingPoll = false;
        Choices = new ObservableCollection<Choice>(Poll.Choices);

        var editChoices = Choice.GetChoicesForGrid(Poll.PollId).OrderBy(c => c.Label).ToList();
        foreach(var c in editChoices.ToList()) {
            Console.WriteLine("EDITCHOICES ===>" + c.Label);
        }
        _editChoices = editChoices.Select(c => new EditChoiceViewModel(poll, IsNew)).ToList();

        var editChoiceView = new EditChoiceView(Poll, IsNew);
        EditChoiceViews.Add(editChoiceView);

        foreach (var c in _editChoices.ToList()) {
            Console.WriteLine("EDITCHOICES 2 ===>" + c);
        }
        //_initialChoices = new List<Choice>(Poll.Choices);
        foreach (Choice choice in Poll.Choices) {
            Choice = choice;
        
            Console.WriteLine("Choice!!! ==>"+Choice.Label);
        }
        
        
        Console.WriteLine("ISNEW===> " +IsNew);
        IsClosed = Poll.IsClosed;
        SelectedType = Poll.Type;
        PollTypes = new ObservableCollection<PollType>(Enum.GetValues(typeof(PollType)).Cast<PollType>());
        Participants = new ObservableCollection<User>(Poll.Participants);
        UpdateParticipantsTotalVotes();
        ;
        Save = new RelayCommand(SaveAction, CanSaveAction);
        SaveCommand = new RelayCommand(SaveChoiceAction);
        Cancel = new RelayCommand(CancelAction,CanCancelAction);
       
        Delete = new RelayCommand(DeleteAction);
        AddCurrentUserCommand = new RelayCommand(AddCurrentUser);
        DeleteParticipantCommand = new RelayCommand<int>(
                    (id) => this.DeleteParticipant(id),
                    (id) => this.CanDeleteParticipant(id)
         );

        RaisePropertyChanged();
        EditChoice = new ObservableCollection<EditChoiceView>(new[] { new EditChoiceView(poll, IsNew) });
    }
    
    public void UpdateParticipantsTotalVotes() {
        foreach (var participant in Participants.ToList()) {
            TotalVotesForUser(participant);
        }
        Context.SaveChanges();
    }

    public void TotalVotesForUser(User user) {
        int totalVotes = 0;
        foreach (var choice in Poll.Choices) {
            totalVotes += choice.VotesList.Count(v => v.UserId == user.UserId);
        }
        user.TotalVotes = totalVotes;
    }
    public int NbVotesForChoice(Choice choice) {
        int totalVotes = 0;
        foreach (var c in Poll.Choices) {
            totalVotes += c.VotesList.Count(c => c.ChoiceId == choice.ChoiceId);
        }
        return totalVotes;
    }
    public int NbVotesForUser(User user) {
        int totalVotes = 0;
        foreach (var choice in Poll.Choices) {
            totalVotes += choice.VotesList.Count(v => v.UserId == user.UserId);
        }
        return totalVotes;
    }
    public void DeleteAction() {
        Console.WriteLine("DELETE !!!!!");
    }
    private List<EditChoiceViewModel> _editChoices;
    public List<EditChoiceViewModel> EditChoices => _editChoices;
    
}

