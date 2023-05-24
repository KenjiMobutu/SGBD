using System.Collections.ObjectModel;
using System.Linq;
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
        if (CurrentUser == null || string.IsNullOrEmpty(CurrentUser.Mail)) {
            // gestion du cas où CurrentUser est null ou CurrentUser.Mail est null
            return;
        }

        IQueryable<Poll> polls;

        if (CurrentUser.IsAdmin) // Vérifier si l'utilisateur actuel est un administrateur
        {
            polls = Poll.GetAllPolls(); // Récupérer tous les sondages sans filtre
        } else {
            polls = Poll.GetPolls(CurrentUser); // Récupérer les sondages de l'utilisateur actuel
        }
        
        if (!string.IsNullOrEmpty(Filter)) {
            polls = polls.Where(p =>
                p.Title.Contains(Filter) ||
                p.Creator.Name.Contains(Filter) ||
                p.Participants.Any(participant => participant.Name.Contains(Filter)) ||
                p.Choices.Any(choice => choice.Label.Contains(Filter))
            );
        }


        Polls = new ObservableCollection<PollCardViewModel>(polls.Select(p => new PollCardViewModel(p)));
    }

}





