using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel;

public class PollCardViewModel : ViewModelCommon {
    private readonly Poll _poll;
    public Poll Poll {
        get => _poll;
        private init => SetProperty(ref _poll, value);
    }
    public string Name => Poll.Title;
    public PollType Type => Poll.Type;
    public int CreatorId => Poll.CreatorId;
    public virtual User Creator => Poll.Creator;

    public PollCardViewModel(Poll poll) {
        Poll = poll;
    }
    
  
}
