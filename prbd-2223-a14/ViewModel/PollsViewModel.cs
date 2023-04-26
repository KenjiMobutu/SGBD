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

        Register<Poll>(App.Messages.MSG_POLL_CHANGED, member => OnRefreshData());

        //AllSelected = true;
    }
    protected  void GetUserPolls() {
        IQueryable<Poll> polls = Poll.GetPolls(CurrentUser);

        Polls = new ObservableCollection<PollCardViewModel>(polls.Select(p => new PollCardViewModel(p)));
    }
    //public static IQueryable<Poll> GetAll() {
    //    IQueryable<Poll> polls = Poll.GetPolls(CurrentUser);
    //    Polls = new ObservableCollection<PollCardViewModel>(polls.Select(p => new PollCardViewModel(p)));
    //    return Polls;
    //}
    private void Filtering() {
        IQueryable<Poll> query = Context.Polls;
        if (!CurrentUser.isAdmin()) {
            query = query.Where(p => p.Participants.Any(participant => participant.UserId == CurrentUser.UserId));
        }
        query = query.Where(p => p.Title.Contains(Filter));

        var filter = new ObservableCollection<PollCardViewModel>(query.Select(p => new PollCardViewModel(p)));

        Polls = filter;
    }
    protected override void OnRefreshData() {
        //GetUserPolls();
        //Filtering();
        IQueryable<Poll> polls = Poll.GetPolls(CurrentUser);


        Polls = new ObservableCollection<PollCardViewModel>(polls.Select(p => new PollCardViewModel(p)));
    }
}
