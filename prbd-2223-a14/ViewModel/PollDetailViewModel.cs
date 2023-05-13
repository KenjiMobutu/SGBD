using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    private readonly Poll _poll;

    public Poll Poll {
        get => _poll;
        private init => SetProperty(ref _poll, value);
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
    public ICommand DisplayEdit { get; set; }
    public ICommand Delete { get; set; }
    public ICommand Reopen { get; set; }
    public ICommand ToggleCommentingCommand { get; set; }
    public ICommand AddCommentCommand { get; set; }
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
    
    public PollDetailViewModel(Poll poll, bool isNew) : base() {
        IsNew = isNew;
        Poll = poll;
        var pollId = poll.PollId;

        IsEditing = false;
        IsClosed = !poll.IsClosed;

        if (!IsClosed) {
            IsVisibleLink = true;
            IsCommenting = true;
            Console.WriteLine(" rentre dans le if :ISCLOSED===>" + IsClosed);
        } else {
            IsCommenting = true;
            IsVisibleLink = false;
        }

        AddCommentCommand =  new RelayCommand(AddCommentAction);
        ToggleCommentingCommand = new RelayCommand(() => {
            IsCommenting = false;
            IsVisibleLink = true;
        });
        Reopen = new RelayCommand(() => {
            IsClosed = true;
        });

        // Ajouter seulement le VoteGridView du Poll sélectionné
        var voteGridView = new VoteGridView(Poll);
        VoteGridViews.Add(voteGridView);

        DisplayEdit = new RelayCommand(() => {
            EditView = new PollAddView(Poll,IsNew);
            IsEditing = true;
            IsClosed = true;
        });
        Delete = new RelayCommand(DeleteAction, () => !IsNew);
       
        Comments = new ObservableCollection<Comment>(Poll.Comments);
        foreach (var c in _comments.ToList()) {
            Console.WriteLine("Comments ===> : " + c.Text.ToString());
        }
       
        // Assigner la liste VoteGrid à une nouvelle collection créée à partir de VoteGridViews
        VoteGrid = new ObservableCollection<VoteGridView>(VoteGridViews.Select(vg => new VoteGridView(poll)));
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
    }

    private void DeleteAction() {
        CancelAction();
        Poll.Delete();
        NotifyColleagues(App.Messages.MSG_MEMBER_CHANGED, Poll);
        NotifyColleagues(App.Messages.MSG_CLOSE_TAB, Poll);
    }
    public string Title => Poll.Title;
    public User Creator => Poll.Creator;

    protected override void OnRefreshData() {
 
    }
}








