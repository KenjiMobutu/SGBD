using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata;
using MyPoll.Model;
using MyPoll.View;
using PRBD_Framework;

namespace MyPoll.ViewModel;

public class PollDetailViewModel : ViewModelCommon {

    private readonly Poll _poll;

    public Poll Poll {
        get => _poll;
        private init => SetProperty(ref _poll, value);
    }

    private bool _isNew;
    public bool IsNew {
        get => _isNew;
        set => SetProperty(ref _isNew, value);
    }
    public bool IsExisting => !_isNew;

    public PollDetailViewModel(Poll poll, bool isNew) {
        IsNew = isNew;
        Poll = poll;

    }
    public PollDetailViewModel(Poll poll, User creator) {
        Poll = poll;
    }

    public string Title => Poll.Title;
    public User Creator => Poll.Creator;

    private VoteGridView _voteGridView;
    public VoteGridView VoteGridView {
        get {
            if (_voteGridView == null) {
                _voteGridView = new VoteGridView();
            }
            return _voteGridView;
        }
    }

}

