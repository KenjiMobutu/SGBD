using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel;

public class PollDetailViewModel : ViewModelCommon {

    private Poll _poll;
    public Poll Poll {
        get => _poll;
        set => SetProperty(ref _poll, value);
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

}

