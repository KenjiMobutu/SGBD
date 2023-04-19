using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel;

public class PollsViewModel : ViewModelBase<User, MyPollContext> {
    public string Title { get; } = "prbd-2223-a14";
}
