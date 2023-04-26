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

        
    }

    //private void ApplyFilterAction() {
    //    IQueryable<Poll> query = Context.Polls;
    //    if (!CurrentUser.isAdmin()) {
    //        query = query.Where(p => p.Participants.Any(p.PollId == CurrentUser.UserId));
    //    }
    //    query = query.Where(p => p.Title.Contains(Filter));

        //    var filter = new ObservableCollection<PollCardViewModel>(query.Select(p => new PollCardViewModel(p)));

        //    Polls = filter;
        //}
        //private void ApplyFilterAction() {
        //    IQueryable<Poll> query = Context.Polls;
        //    if (!CurrentUser.isAdmin() {
        //        query = query.Where(p => p.Participants.Any(p.PollId == CurrentUser.UserId));
        //    }
        //    query = query.Where(p => p.Title.Contains(Filter));

        //    var filter = new ObservableCollection<PollCardViewModel>(query.Select(p => new PollCardViewModel(p)));

        //    Polls = filter;
        //}

        //protected override void OnRefreshData() {
        //    /* ApplyFilterAction est appelée deux fois quand on clique sur un radiobutton : une fois
        //     * pour mettre celui qui est sélectionné à true et une autre fois pour mettre celui qui
        //     * était sélectionné à false. Du coup, pour éviter de faire deux fois la requête, on retourne
        //     * sans rien faire quand deux flags sont vrais en même temps.
        //     */

        //    IQueryable<Poll> polls = string.IsNullOrEmpty(Filter) ? Member.GetAll() : Member.GetFiltered(Filter);
        //    var filteredPolls = from p in polls
        //                          where
        //                              // on veut les followees de l'utilisateur courant => on prend tous ceux qui ont 
        //                              // le pseudo courant dans leurs followers 
        //                              FolloweesSelected && m.Followers.Any(f => CurrentUser != null && f.Pseudo == CurrentUser.Pseudo) ||
        //                              // on veut les followers de l'utilisateur courant => on prend tous ceux qui ont 
        //                              // le pseudo courant dans leurs followees 
        //                              FollowersSelected && m.Followees.Any(f => CurrentUser != null && f.Pseudo == CurrentUser.Pseudo) ||
        //                              // on veut tous les membres
        //                              AllSelected
        //                          select m;
        //    Polls = new ObservableCollection<PollCardViewModel>(filteredPolls.Select(p => new PollCardViewModel(p)));
        //}
    public string Title { get; } = "prbd-2223-a14";
}
