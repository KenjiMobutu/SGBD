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
        Console.WriteLine(IsRegistrated);

        Vote = participant.VotesList.FirstOrDefault(v => v.Choice.ChoiceId == choice.ChoiceId,
            new Vote() { Choice = choice, User = participant });

        IsVoteYes = Vote.Value == 1;
        IsVoteNo = Vote.Value == -1;
        IsVoteMaybe = Vote.Value == 0.5;

        // Commande (utilisée par le bouton de la vue) qui "bascule" le booléen indiquant si l'user a voté 
        HasVotedCommand = new RelayCommand<object>(HasVoted);

        //ChangeVoteYes = new RelayCommand( ChangesVoteYes);
        //ChangeVoteNo = new RelayCommand( ChangesVoteNo);
        //ChangeVoteMaybe = new RelayCommand(ChangesVoteMaybe);
    }

    public Vote Vote { get; set; }

    public bool IsVoteYes { get; set; }
    public bool IsVoteNo { get; set; }
    public bool IsVoteMaybe { get; set; }

    public VoteChoiceViewModel() { }

    private bool _editMode;

    public bool EditMode {
        get => _editMode;
        set => SetProperty(ref _editMode, value);
    }

    public RelayCommand<object> HasVotedCommand { get; set; }
    public ICommand ChangeVoteYes { get; set; }
    public void ChangesVoteYes() {
        if (!EditMode) {
            return;
        }
        Vote.Type = VoteType.Yes;
    }

    public ICommand ChangeVoteNo { get; set; }
    public void ChangesVoteNo() {
        if (!EditMode) {
            return;
        }
        Vote.Type = VoteType.No;
    }

    public ICommand ChangeVoteMaybe { get; set; }
    public void ChangesVoteMaybe() {
        if (!EditMode) {
            return;
        }
        Vote.Type = VoteType.Maybe;
    }
    public void HasVoted(object parameter) {
        if (!EditMode) {
            return;
        }

        double newVote = Convert.ToDouble(parameter);
        Console.WriteLine(newVote.ToString());
        if (newVote == Vote.Value) {
            return;
        }

        switch (newVote) {
            case 1.0:
                ChangesVoteYes();
                break;
            case -1.0:
                ChangesVoteNo();
                break;
            case 0.5:
                ChangesVoteMaybe();
                break;
            default:
                break;
        }
        IsRegistrated = true;
    }


    public void ChangeVote(object parameter) {
        if (!EditMode) {
            return;
        }

        double newVote = (double)parameter;
        if (newVote == Vote.Value) {
            return;
        }

        Vote.Type = newVote switch {
            1 => VoteType.Yes,
            0.5 => VoteType.Maybe,
            -1 => VoteType.No,
            _ => VoteType.Maybe, // Valeur par défaut pour les votes invalides
        };
    }


    private bool _isRegistrated;
    public bool IsRegistrated {
        get => _isRegistrated;
        set => SetProperty(ref _isRegistrated, value);
    }

    public bool IsRegistratedX {
        get => Vote.Value == -1;
    }

    public bool IsRegistratedQuestion {
        get => Vote.Value == 0.5;
    }

    public EFontAwesomeIcon RegistratedIcon => IsRegistrated && Vote.Value == 1 ? EFontAwesomeIcon.Solid_Check : EFontAwesomeIcon.None;

    public EFontAwesomeIcon RegistratedX => IsRegistrated && Vote.Value == -1 ? EFontAwesomeIcon.Solid_X : EFontAwesomeIcon.None;

    public EFontAwesomeIcon RegistratedQuestion => IsRegistrated && Vote.Value == 0.5 ? EFontAwesomeIcon.Regular_CircleQuestion : EFontAwesomeIcon.None;

    public Brush RegistratedColor => IsRegistrated ? Brushes.Green : Brushes.White;

    public Brush RegistratedColorRed => IsRegistratedX ? Brushes.Red : Brushes.White;

    public Brush RegistratedColorOrange => IsRegistrated ? Brushes.Orange : Brushes.White;

    //public string RegistratedToolTip => IsRegistrated ? "Yes" : "No";
    public string RegistratedToolTip => IsRegistrated ? "Yes" : (Vote.Value == 0.5 ? "Maybe" : "No");

}


