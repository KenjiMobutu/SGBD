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
    public VoteChoiceViewModel(User participant, Choice choice, bool isRegistered) {
        IsRegistrated = isRegistered;

        Vote = participant.VotesList.FirstOrDefault(v => v.Choice.ChoiceId == choice.ChoiceId) ??
               new Vote() { Choice = choice, User = participant };


        IsVoteYes = Vote.Value == 1;
        IsVoteNo = Vote.Value == -1;
        IsVoteMaybe = Vote.Value == 0.5;

        IsRegistratedYes = Vote.Type == VoteType.Yes && IsRegistrated;
        IsRegistratedNo = Vote.Type == VoteType.No && IsRegistrated;
        IsRegistratedMaybe = Vote.Type == VoteType.Maybe && IsRegistrated;

        // Commande (utilisée par le bouton de la vue) qui "bascule" le booléen indiquant si l'user a voté 
        HasVotedCommand = new RelayCommand<object>(HasVoted);

        ClearChoicesCommand = new RelayCommand(ClearChoices);
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
    public RelayCommand ChangeVoteYes { get; set; }
    public void ChangesVoteYes() {
        if (!EditMode) {
            return;
        }
        Vote.Type = VoteType.Yes;
        
    }


    public RelayCommand ChangeVoteNo { get; set; }
    public void ChangesVoteNo() {
        if (!EditMode) {
            return;
        }
        Vote.Type = VoteType.No;
       
    }

    public RelayCommand ChangeVoteMaybe { get; set; }
    public void ChangesVoteMaybe() {
        if (!EditMode) {
            return;
        }
        Vote.Type = VoteType.Maybe;
    }
    public RelayCommand ClearChoicesCommand { get; set; }
    public void ClearChoices() {
        IsRegistratedYes = false;
        IsRegistratedNo = false;
        IsRegistratedMaybe = false;
        Vote = new Vote();
    }



    public void HasVoted(object parameter) {
        if (!EditMode) {
            return;
        }

        double newVote = Convert.ToDouble(parameter);
        Console.WriteLine(newVote.ToString());
        // Determine the new vote type
        VoteType newVoteType = newVote switch {
            1.0 => VoteType.Yes,
            -1.0 => VoteType.No,
            0.5 => VoteType.Maybe,
            _ => VoteType.Maybe,
        };

        if (newVoteType == Vote.Type) {
            return;
        }

        // Update the vote type
        Vote.Type = newVoteType;

        // Update the IsRegistrated properties
        IsRegistrated = true;
        IsRegistratedNo = Vote.Type == VoteType.No;
        IsRegistratedYes = Vote.Type == VoteType.Yes;
        IsRegistratedMaybe = Vote.Type == VoteType.Maybe;
    }


    private bool _isRegistrated;
    public bool IsRegistrated {
        get => _isRegistrated;
        set => SetProperty(ref _isRegistrated, value);
    }
    private bool _isRegistratedYes;
    public bool IsRegistratedYes {
        get => _isRegistratedYes;
        set => SetProperty(ref _isRegistratedYes, value);
    }

    private bool _isRegistratedNo;
    public bool IsRegistratedNo {
        get => _isRegistratedNo;
        set => SetProperty(ref _isRegistratedNo, value);
    }

    private bool _isRegistratedMaybe;
    public bool IsRegistratedMaybe {
        get => _isRegistratedMaybe;
        set => SetProperty(ref _isRegistratedMaybe, value);
    }
    public double MaybeDoubleValue => 0.5;
        
    

    public EFontAwesomeIcon RegistratedIcon => IsRegistratedYes  ? EFontAwesomeIcon.Solid_Check : EFontAwesomeIcon.None;

    public EFontAwesomeIcon RegistratedX => IsRegistratedNo  ? EFontAwesomeIcon.Solid_X : EFontAwesomeIcon.None;

    public EFontAwesomeIcon RegistratedQuestion => IsRegistratedMaybe ? EFontAwesomeIcon.Regular_CircleQuestion : EFontAwesomeIcon.None;

    public Brush RegistratedColor => IsRegistratedYes ? Brushes.Green : Brushes.White;

    public Brush RegistratedColorRed => IsRegistratedNo ? Brushes.Red : Brushes.White;

    public Brush RegistratedColorOrange => IsRegistratedMaybe ? Brushes.Orange : Brushes.White;

    //public string RegistratedToolTip => IsRegistrated ? "Yes" : "No";
    public string RegistratedToolTipYes => IsRegistratedYes ? "Yes" : "";
    public string RegistratedToolTipNo => IsRegistratedNo ? "No" : "";
    public string RegistratedToolTipMaybe => IsRegistratedMaybe ? "Maybe" : "";

}


