using System;
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
    private  Poll _poll;

    public Poll Poll {
        get => _poll;
        set => SetProperty(ref _poll, value);
    }
    private Choice _choice;
    public Choice Choice {
        get { return _choice; }
        set { SetProperty(ref _choice, value); }
    }

   
    public string PollTitle {
        get => Poll?.Title;
        set => SetProperty(Poll.Title, value,Poll, (p,v) => {
            p.Title = v;
            ValidateTitle();
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
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(Participants));
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
       
        Context.SaveChanges();
        RaisePropertyChanged();
        NotifyColleagues(App.Messages.MSG_POLL_CHANGED, Poll);
    }

    private bool CanSaveAction() {
        Console.WriteLine("Can SAVE ACTION ===>" + IsNew);
        if (IsNew)
            return !string.IsNullOrEmpty(PollTitle) && !HasErrors;
        return Poll != null && Poll.IsModified && PollTitle != null && !HasErrors; 
    }

    public override void CancelAction() {
        if (IsNew) {
            IsNew = false;
            Console.WriteLine($"New {PollTitle}");
            NotifyColleagues(App.Messages.MSG_CLOSE_TAB, Poll);
        } else {
            ClearErrors();
            Poll.Reload();
            RaisePropertyChanged();
        }
    }
    private Choice _initialChoice;

    private Choice CloneChoice(Choice choice) {
        if (choice == null)
            return null;

        Choice clonedChoice = new Choice {
            ChoiceId = choice.ChoiceId,
            Label = choice.Label,
            VotesList = new List<Vote>(choice.VotesList) // Effectue une copie de la liste de votes
        };

        return clonedChoice;
    }



    private void CancelChoice() {
        EditChoiceVisibility = true;
        IsEditingVisibility = false;

        Choices.Clear(); // Supprime tous les éléments de la liste actuelle

        foreach (var choice in _initialChoices) {
            Console.WriteLine(choice.Label);
            Choices.Add(CloneChoice(choice)); // Ajoute un clone de chaque choix initial
        }

        RaisePropertyChanged(nameof(Choices));
    }

    private bool CanCancelAction() {
        return Poll != null && (IsNew || Poll.IsModified);
    }

    private string _newChoiceLabel;
    public string NewChoiceLabel {
        get => _newChoiceLabel; 
        set => SetProperty(ref _newChoiceLabel, value, () => Validate());
    }
    public override bool Validate() {
        ClearErrors();

        if (string.IsNullOrEmpty(NewChoiceLabel)) {
           
            AddError(nameof(NewChoiceLabel), "Cannot be empty");
            
        } else if (NewChoiceLabel.Length < 3) {
            AddError(nameof(NewChoiceLabel), "length must be >= 3");
        } else if (NewChoiceLabel.TrimStart() != NewChoiceLabel) {
            AddError(nameof(NewChoiceLabel), "cannot start with a space");
        } else if (LabelExists()) {
            AddError(nameof(NewChoiceLabel), "Label already in the choice list");
        }

        return !HasErrors;
    }



    private string _label;
    public string Label {
        get { return _label; }
        set {
            _label = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(Label));
        }
    }

    private ICommand _addChoiceCommand;
    public ICommand AddChoiceCommand {
        get => _addChoiceCommand;
        set => SetProperty(ref _addChoiceCommand, value);
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

        if (choice != null) {
            if (NbVotesForChoice(choice) > 0) {
                // Afficher une boîte de dialogue de confirmation
                var result = MessageBox.Show("Le choix contient des votes. Êtes-vous sûr de vouloir le supprimer ?",
                    "Confirmation de suppression", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.No) {
                    return;
                }
            }
            Choices.Remove(choice);
            Poll.Choices.Remove(choice);
            
        }
        
        RaisePropertyChanged();
        RaisePropertyChanged(nameof(Choices));
        Context.SaveChanges ();
    }

    private bool CanAddChoice() {
        return !string.IsNullOrEmpty(NewChoiceLabel) && !HasErrors;
        
    }

    private bool LabelExists() {
        return Choices.Any(choice => choice.Label == NewChoiceLabel);
    }
    private void AddChoice() {
        
        var choice = new Choice { Label = NewChoiceLabel };
        Poll.Choices.Add(choice);
        Choices.Add(choice);
        NewChoiceLabel = ""; // remise à zéro de la propriété pour permettre d'ajouter un nouveau choix
        Context.SaveChanges();
        RaisePropertyChanged();
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
        set {
            SetProperty(ref _isClosed, value);
            if (_isClosed) {
                Poll.IsClosed = true;
            }
        }
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
    private List<Choice> _initialChoices;

    public PollAddViewModel(Poll poll, bool isNew) {
        Poll = poll;
        IsNew = isNew;
        PollTitle = Poll.Title;
        EditChoiceVisibility = true;
        IsEditingVisibility = false;
        EditChoiceCommand = new RelayCommand(() => {
            Console.WriteLine("EDIT CHOICE COMMAND");
            EditChoiceVisibility = false;
            IsEditingVisibility = true; 
        });
        
        
        _initialChoices = new List<Choice>(Poll.Choices);
        foreach (Choice choice in Poll.Choices) {
            Choice = choice;
            Console.WriteLine("Choice ==>"+Choice.Label);
        }
        AddChoiceCommand = new RelayCommand(AddChoice, CanAddChoice);

        
        Console.WriteLine("ISNEW===> " +IsNew);
        IsClosed = Poll.IsClosed;
        SelectedType = Poll.Type;
        PollTypes = new ObservableCollection<PollType>(Enum.GetValues(typeof(PollType)).Cast<PollType>());
        Participants = new ObservableCollection<User>(Poll.Participants);
        UpdateParticipantsTotalVotes();
        Choices = new ObservableCollection<Choice>(Poll.Choices);
        Save = new RelayCommand(SaveAction, CanSaveAction);
        SaveCommand = new RelayCommand(SaveChoiceAction);
        Cancel = new RelayCommand(CancelAction,CanCancelAction);
       // CancelCommand = new RelayCommand();
        Delete = new RelayCommand(DeleteAction);
        AddCurrentUserCommand = new RelayCommand(AddCurrentUser);
        DeleteParticipantCommand = new RelayCommand<int>(
                    (id) => this.DeleteParticipant(id),
                    (id) => this.CanDeleteParticipant(id)
         );

        RaisePropertyChanged();
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
    
}

