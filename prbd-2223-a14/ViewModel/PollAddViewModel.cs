using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MyPoll.Model;
using MyPoll.View;
using PRBD_Framework;

namespace MyPoll.ViewModel;
public class PollAddViewModel : ViewModelCommon {

    public ICommand Save { get; set; }
    public ICommand Cancel { get; set; }

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

    private bool CanAddSelectedUser() {
        // Vérifier si l'utilisateur sélectionné n'est pas déjà dans la liste des participants
        return SelectedUserToAdd != null && !Poll.Participants.Contains(SelectedUserToAdd);
    }

    private void AddSelectedUser() {
        // Ajouter l'utilisateur sélectionné à la liste des participants
        if (SelectedUserToAdd != null) {
            Poll.Participants.Add(SelectedUserToAdd);
            RaisePropertyChanged(nameof(Poll.Participants));
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

    public override void SaveAction() {
        if (IsNew) {
            // Un petit raccourci ;-)
            //Member.Password = Member.Pseudo;
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
        return Poll != null && Poll.IsModified;
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

    public PollAddViewModel(Poll poll, bool isNew) {
        Poll = poll;
        IsNew = isNew;

        Save = new RelayCommand(SaveAction, CanSaveAction);
        Cancel = new RelayCommand(CancelAction, CanCancelAction);

        RaisePropertyChanged();
    }
}

