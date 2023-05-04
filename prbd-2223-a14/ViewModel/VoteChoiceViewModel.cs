using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyPoll.Model;
using PRBD_Framework;
using FontAwesome6;
using System.Windows.Media;
using System.Windows.Input;

namespace MyPoll.ViewModel;
public class VoteChoiceViewModel : ViewModelCommon {
    public VoteChoiceViewModel(User participant, Choice choice) {
        IsRegistrated = participant.VotesList.Any(p => p.Choice.ChoiceId == choice.ChoiceId);

        Vote = participant.VotesList.FirstOrDefault(v => v.Choice.ChoiceId == choice.ChoiceId,
            new Vote() { Choice = choice, User = participant });

        // Commande (utilisée par le bouton de la vue) qui "bascule" le booléen indiquant si l'étudiant est inscrit au cours 
        ChangeVote = new RelayCommand(() => IsRegistrated = !IsRegistrated);
    }
    public VoteChoiceViewModel(User participant, Choice choice, bool isRegistered) {
        IsRegistrated = isRegistered;
        Vote = participant.VotesList.FirstOrDefault(v => v.Choice.ChoiceId == choice.ChoiceId,
            new Vote() { Choice = choice, User = participant });
        ChangeVote = new RelayCommand(() => IsRegistrated = !IsRegistrated);
    }


    public Vote Vote { get; private set; }

    public VoteChoiceViewModel() { }

    private bool _editMode;

    public bool EditMode {
        get => _editMode;
        set => SetProperty(ref _editMode, value);
    }
    public ICommand ChangeVote { get; set; }

    private bool _isRegistrated;
    public bool IsRegistrated {
        get => _isRegistrated;
        set => SetProperty(ref _isRegistrated, value);
    }
    public EFontAwesomeIcon RegistratedIcon => IsRegistrated ? EFontAwesomeIcon.Solid_Check : EFontAwesomeIcon.None;
    public Brush RegistratedColor => IsRegistrated ? Brushes.Green : Brushes.White;
    public string RegistratedToolTip => IsRegistrated ? "Yes" : "No";
}

