using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MyPoll.Model;
using MyPoll.View;
using PRBD_Framework;

namespace MyPoll.ViewModel;
public class PollAddViewModel : ViewModelCommon {
    public PollAddViewModel() {
        
    }
    public ICommand Save { get; set; }
    public ICommand Cancel { get; set; }
    public ICommand Delete { get; set; }

    private readonly Poll _poll;

    public Poll Poll {
        get => _poll;
        private init => SetProperty(ref _poll, value);
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
            // OnPropertyChanged(nameof(SelectedUserToAdd));
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
    private ICommand _deleteParticipantCommand;
    public ICommand DeleteParticipantCommand {
        get {
            Console.WriteLine("Delete Command 1");
            if (_deleteParticipantCommand == null) {
                Console.WriteLine("Delete Command");
                _deleteParticipantCommand = new RelayCommand<int>(
                    (id) => this.DeleteParticipant(id),
                    (id) => this.CanDeleteParticipant(id)
                );
            }
            return _deleteParticipantCommand;
        }
    }

    private void DeleteParticipant(int userId) {
        var participant = Poll.Participants.FirstOrDefault(p => p.UserId == userId);
        Console.WriteLine("User à EFFACER  ===> "+participant.Name);
        if (participant != null) {
            Poll.Participants.Remove(participant);
            Participants.Remove(participant);
            RaisePropertyChanged(nameof(Participants));
        }
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
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(Participants));
            RaisePropertyChanged(nameof(CanSaveAction));
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
        RaisePropertyChanged(nameof(Participants));
    }
 
    private void AddCurrentUser() {
        Poll.Participants.Add(CurrentUser);
        Participants.Add(CurrentUser);
        RaisePropertyChanged();
        RaisePropertyChanged(nameof(Participants));
        RaisePropertyChanged(nameof(CanSaveAction));
    }

    public override void SaveAction() {
        if (IsNew) {
            Poll.CreatorId = CurrentUser.UserId;
            Context.Add(Poll);
            IsNew = false;
        }
        Context.SaveChanges();
        RaisePropertyChanged();
        NotifyColleagues(App.Messages.MSG_POLL_CHANGED, Poll);
    }

    private bool CanSaveAction() {
        if (IsNew)
            return !string.IsNullOrEmpty(Poll.Title);
        return Poll != null && (Poll.IsModified || Participants.Count != Poll.Participants.Count);
    }

    public override void CancelAction() {
        if (IsNew) {
            IsNew = false;
            NotifyColleagues(App.Messages.MSG_CLOSE_TAB, Poll);
        } else {
            Poll.Reload();
            RaisePropertyChanged();
        }
    }
    private bool CanCancelAction() {
        return Poll != null && (IsNew || Poll.IsModified);
    }

    private string _newChoiceLabel;
    public string NewChoiceLabel {
        get { return _newChoiceLabel; }
        set {
            _newChoiceLabel = value;
            RaisePropertyChanged(nameof(NewChoiceLabel));
        }
    }

    private ICommand _addChoiceCommand;
    public ICommand AddChoiceCommand {
        get {
            if (_addChoiceCommand == null) {
                _addChoiceCommand = new RelayCommand(() => AddChoice(), () => CanAddChoice());
            }
            return _addChoiceCommand;
        }
    }
    private ICommand _deleteChoiceCommand;
    public ICommand DeleteChoiceCommand {
        get {
            if (_deleteChoiceCommand == null) {
                _deleteChoiceCommand = new RelayCommand<int>(
                    (choiceId) => this.DeleteChoice(choiceId),
                    (choiceId) => this.CanDeleteChoice(choiceId)
                );
            }
            return _deleteChoiceCommand;
        }
    }

    private bool CanDeleteChoice(int choiceId) {
        return Poll.Choices.Any(c => c.ChoiceId == choiceId);
    }

    private void DeleteChoice(int choiceId) {
        var choice = Poll.Choices.FirstOrDefault(c => c.ChoiceId == choiceId);
        Poll.Choices.Remove(choice);
        Choices.Remove(choice);
        RaisePropertyChanged(nameof(Choices));
    }


    private bool CanAddChoice() {
        return !string.IsNullOrEmpty(NewChoiceLabel);
    }

    private void AddChoice() {
        Poll.Choices.Add(new Choice { Label = NewChoiceLabel });
        Choices.Add(new Choice { Label = NewChoiceLabel });
        NewChoiceLabel = ""; // remise à zéro de la propriété pour permettre d'ajouter un nouveau choix
        RaisePropertyChanged(nameof(Choices));
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
        set => SetProperty(ref _isClosed, value);
    }

    public PollAddViewModel(Poll poll, bool isNew) {
        Poll = poll;
        IsNew = isNew;
        IsClosed = Poll.IsClosed;
        Participants = new ObservableCollection<User>(Poll.Participants);
        Choices = new ObservableCollection<Choice>(Poll.Choices);
        Save = new RelayCommand(SaveAction, CanSaveAction);
        Cancel = new RelayCommand(CancelAction, CanCancelAction);
        Delete = new RelayCommand(DeleteAction);
        AddCurrentUserCommand = new RelayCommand(AddCurrentUser);
        RaisePropertyChanged();
        RaisePropertyChanged(nameof(Participants));
    }
    public void DeleteAction() {
        Console.WriteLine("DELETE !!!!!");
    }
    
}

