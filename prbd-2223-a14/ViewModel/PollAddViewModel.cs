using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyPoll.Model;
using MyPoll.View;
using PRBD_Framework;

namespace MyPoll.ViewModel;
public class PollAddViewModel : ViewModelCommon {
    private readonly Poll _poll;

    public Poll Poll {
        get => _poll;
        private init => SetProperty(ref _poll, value);
    }
    public PollAddViewModel(Poll poll) {
        Poll = poll;
    }
    
    public string Title => Poll.Title;
    public User Creator => Poll.Creator;
}

