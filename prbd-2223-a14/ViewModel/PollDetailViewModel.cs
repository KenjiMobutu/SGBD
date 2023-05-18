using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MyPoll.Model;
using MyPoll.View;
using PRBD_Framework;
using static MyPoll.App;

namespace MyPoll.ViewModel;

public class PollDetailViewModel : ViewModelCommon {
    public ICommand DisplayEdit { get; set; }
    public ICommand Delete { get; set; }
    public ICommand Reopen { get; set; }
    public ICommand ToggleCommentingCommand { get; set; }
    public ICommand AddCommentCommand { get; set; }

    private ICommand _deleteCommentCommand;
    public ICommand DeleteCommentCommand {
        get {
            if (_deleteCommentCommand == null) {
                _deleteCommentCommand = new RelayCommand<int>(
                    (commentId) => this.DeleteCommentAction(commentId)
                    
                );
            }
            return _deleteCommentCommand;
        }
    }

    private readonly Poll _poll;

    public Poll Poll {
        get => _poll;
        private init => SetProperty(ref _poll, value);
    }
    private readonly Comment _comment;

    public Comment Comment{
        get => _comment;
        private init => SetProperty(ref _comment, value);
    }

    private ObservableCollection<Choice> _choices;
    public ObservableCollection<Choice> Choices {
        get => _choices;
        private set => SetProperty(ref _choices, value);
    }
    
    private bool _isClosed;
    public bool IsClosed {
        get => _isClosed;
        set => SetProperty(ref _isClosed, value);
    }

    private bool _isNew;
    public bool IsNew {
        get => _isNew;
        set => SetProperty(ref _isNew, value);
    }

    private bool _isEditing;
    public bool IsEditing {
        get => _isEditing;
        set => SetProperty(ref _isEditing, value);
    }
    private bool _isCommenting;
    public bool IsCommenting {
        get => _isCommenting;
        set => SetProperty(ref _isCommenting, value);
    }
    private bool _isVisibleLink;
    public bool IsVisibleLink {
        get => _isVisibleLink;
        set => SetProperty(ref _isVisibleLink, value);
    }
    public bool IsExisting => !_isNew;
    
    private ObservableCollection<VoteGridView> _voteGridViews;
    public ObservableCollection<VoteGridView> VoteGrid {
        get => _voteGridViews;
        set => SetProperty(ref _voteGridViews, value);
    }

    private string _newCommentText;
    public string NewCommentText {
        get { return _newCommentText; }
        set {
            _newCommentText = value;
            RaisePropertyChanged(nameof(NewCommentText));
        }
    }
    
    public ObservableCollection<VoteGridView> VoteGridViews { get; } = new ObservableCollection<VoteGridView>();

    private UserControl _editView;
    public UserControl EditView {
        get => _editView;
        set => SetProperty(ref _editView, value);
    }

    private ObservableCollection<Comment> _comments;
    public ObservableCollection<Comment> Comments {
        get => _comments;
        set => SetProperty(ref _comments, value);
    }
 
    private VoteGridViewModel _voteGridVM;
    public VoteGridViewModel VoteGridVM => _voteGridVM;
    public static int Index { get; set; }

    public PollDetailViewModel(Poll poll, bool isNew) : base() {
        //Console.WriteLine("IS CREATOR===> "+IsCreator);
        IsNew = isNew;
        Poll = poll;
        var pollId = poll.PollId;
        var participants = Participation.GetParticipantOfGrid(pollId).OrderBy(p => p.User.Name).ToList();
        bool isParticipant = participants.Any(p => p.User.UserId == CurrentUser.UserId);
        Console.WriteLine("IS PART===> " + isParticipant);

        IsEditing = false  ;
        IsClosed = !poll.IsClosed;
        
        if (!IsClosed && isParticipant) {
            IsVisibleLink = true;
            IsCommenting = true;
            
        } else {
            IsCommenting = true;
            IsVisibleLink = false;
        }

        AddCommentCommand =  new RelayCommand(AddCommentAction);
        
        ToggleCommentingCommand = new RelayCommand(() => {
            IsCommenting = false;
            IsVisibleLink = true;
        });
        _voteGridVM = new VoteGridViewModel(poll);
        Reopen = new RelayCommand(ReopenAction);

        var voteGridView = new VoteGridView(Poll);
        VoteGridViews.Add(voteGridView);

        DisplayEdit = new RelayCommand(() => {
            EditView = new PollAddView(Poll,IsNew);
            IsEditing = true;
            IsClosed = true;
        });
        Delete = new RelayCommand(DeleteAction, () => !IsNew);
       
        Comments = new ObservableCollection<Comment>(Poll.Comments);

        foreach (var comment in Comments) {
            bool isCreatedByCurrentUserOrAdmin = comment.IsCreatedByUser(CurrentUser) || CurrentUser.IsAdmin;
            comment.IsDeletable = isCreatedByCurrentUserOrAdmin;
            Context.SaveChanges();
        }
        

        // Assigner la liste VoteGrid à une nouvelle collection créée à partir de VoteGridViews
        VoteGrid = new ObservableCollection<VoteGridView>(new[] { new VoteGridView(poll) });
        
    }
    private void ReopenAction() {
        Poll.IsClosed = false;
        IsClosed = true;
        _voteGridVM.AskEditMode(false);
        Context.SaveChanges();
        RaisePropertyChanged();
        NotifyColleagues(App.Messages.MSG_POLL_CHANGED, Poll);
        NotifyColleagues(ApplicationBaseMessages.MSG_REFRESH_DATA);
    }
    private void AddCommentAction() {
        if (!string.IsNullOrEmpty(NewCommentText)) {
            Comment newComment = new Comment { Text = NewCommentText, User = CurrentUser };
            Poll.Comments.Add(newComment);
            Comments.Add(newComment);   
            NewCommentText = ""; // Réinitialiser la propriété pour permettre l'ajout d'un nouveau commentaire
        }
        Context.SaveChanges();
        RaisePropertyChanged();
        RaisePropertyChanged(nameof(Comments));
        NotifyColleagues(App.Messages.MSG_POLL_CHANGED, Poll);
        NotifyColleagues(ApplicationBaseMessages.MSG_REFRESH_DATA);
    }
    private void DeleteCommentAction(int commentId) {
        var comment = Poll.Comments.FirstOrDefault(c => c.CommentId == commentId);
        Poll.Comments.Remove(comment);
        Comments.Remove(comment);
        Context.SaveChanges();
        RaisePropertyChanged();
        RaisePropertyChanged(nameof(Comments));
    }
    private void DeleteAction() {
        if (Poll != null) {
            // Afficher une boîte de dialogue de confirmation
            var result = MessageBox.Show(" Êtes-vous sûr de vouloir supprimer le Poll ?",
                "Confirmation de suppression", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.No) {
                return;
            }
            CancelAction();
            Poll.Delete();
        }
        
        NotifyColleagues(App.Messages.MSG_MEMBER_CHANGED, Poll);
        NotifyColleagues(App.Messages.MSG_CLOSE_TAB, Poll);
        NotifyColleagues(ApplicationBaseMessages.MSG_REFRESH_DATA);
    }

    public string Title => Poll.Title;
    public User Creator => Poll.Creator;
    public bool IsCreator => Poll.Creator == CurrentUser || CurrentUser.IsAdmin ;
    protected override void OnRefreshData() {
 
    }
}








