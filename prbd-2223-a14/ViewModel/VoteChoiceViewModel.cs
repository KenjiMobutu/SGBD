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

        // A la construction, s'il existe une inscription de l'étudiant pour le cours, on la récupère,
        // sinon, on en crée une nouvelle avec les deux attributs correctement initialisés
        // Si l'utilisateur valide cette inscription, elle sera ajoutée à la liste des inscriptions 
        // de l'étudiant lors de la sauvegarde par le VM parent (RegistrationStudentViewModel)
        Vote = participant.VotesList.FirstOrDefault(v => v.Choice.ChoiceId == choice.ChoiceId,
            new Vote() { Choice = choice, User = participant });

        // Commande (utilisée par le bouton de la vue) qui "bascule" le booléen indiquant si l'étudiant est inscrit au cours 
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

