using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel;

public class PollsViewModel : ViewModelCommon {
    private ObservableCollection<PollCardViewModel> _polls;
    public ObservableCollection<PollCardViewModel> Polls {
        get => _polls;
        set => SetProperty(ref _polls, value);
    }

    private string _filter;
    public string Filter {
        get => _filter;
        set => SetProperty(ref _filter, value, OnRefreshData);

    }
    public ICommand ClearFilter { get; set; }
    public ICommand NewPoll { get; set; }
    public ICommand DisplayPollDetails { get; set; }

    public PollsViewModel() : base() {
        OnRefreshData();
        
        ClearFilter = new RelayCommand(() => Filter = "");

        NewPoll = new RelayCommand(() => {
            NotifyColleagues(App.Messages.MSG_NEW_POLL, new Poll());
        });

        DisplayPollDetails = new RelayCommand<PollCardViewModel>(vm => {
            NotifyColleagues(App.Messages.MSG_DISPLAY_POLL, vm.Poll);
        });

        Register<Poll>(App.Messages.MSG_POLL_CHANGED, poll => OnRefreshData());
    }
    

    protected override void OnRefreshData() {
        IQueryable<Poll> polls = Poll.GetPolls(CurrentUser);

        if (!string.IsNullOrEmpty(Filter)) {
            polls = polls.Where(p => p.Title.Contains(Filter) || p.Creator.Name.Contains(Filter));
        }

        Polls = new ObservableCollection<PollCardViewModel>(polls.Select(p => new PollCardViewModel(p)));
    }


}
