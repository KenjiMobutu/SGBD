using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.EntityFrameworkCore.Metadata;
using MyPoll.Model;
using MyPoll.ViewModel;
using PRBD_Framework;

namespace MyPoll.ViewModel;
public class VoteParticipantViewModel : ViewModelCommon {

    public VoteParticipantViewModel(VoteGridViewModel voteGridViewModel,User participant, List<Choice> choices) {

        _voteGridViewModel = voteGridViewModel;
        _choices = choices;
        Participant = participant;
        RefreshVotes();

        EditCommand = new RelayCommand(() => EditMode = true);
        SaveCommand = new RelayCommand(Save);
        CancelCommand = new RelayCommand(Cancel);
        DeleteCommand = new RelayCommand(Delete);
    }

    private VoteGridViewModel _voteGridViewModel;
    public ICommand EditCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand DeleteCommand { get; }
    private List<Choice> _choices;

    public User Participant { get; }

    private bool _editMode;

    // La visbilité des boutons de sauvegarde et d'annulation sont bindés sur cette propriété
    public bool EditMode {
        get => _editMode;
        set => SetProperty(ref _editMode, value, EditModeChanged);
    }

    private void EditModeChanged() {
        // Lorsqu'on change le mode d'édition de la ligne, on le signale à chaque cellule
        foreach (VoteChoiceViewModel vcVM in _choicesVM) {
            vcVM.EditMode = EditMode;
        }

        // On informe le parent qu'on change le mode d'édition de la ligne
        _voteGridViewModel.AskEditMode(EditMode);
    }
    public void Changes() {
        RaisePropertyChanged(nameof(Editable));
    }
    public bool Editable => !EditMode && !ParentEditMode;

    public bool ParentEditMode => _voteGridViewModel.EditMode;

    private List<VoteChoiceViewModel> _choicesVM = new();
    public List<VoteChoiceViewModel> VotesVM {
        get => _choicesVM;
        private set => SetProperty(ref _choicesVM, value);
    }

    /*private void RefreshVotes() {
        // On crée, pour chaque inscription de l'étudiant, un RegistrationStudentCourseViewModel
        // qui sera utilisé par le RegistrationStudentCourseView
        // RegistrationsVM est la liste qui servira de source pour la balise <ItemsControl>
        VotesVM = _choices
            .Select(c => new VoteChoiceViewModel(Participant, c))
            .ToList();
    }*/

    private void RefreshVotes() {
        // On crée, pour chaque choix du sondage, un VoteChoiceViewModel qui sera utilisé par le VoteParticipantView
        // VotesVM est la liste qui servira de source pour la balise <ItemsControl>
        VotesVM = _choices
            .Select(c => new VoteChoiceViewModel(Participant, c, Participant.VotesList.Any(v => v.Choice.ChoiceId == c.ChoiceId)))
            .ToList();
    }


    private void Save() {
        EditMode = false;
        // On remplace la collection de Registrations de l'étudiant avec la liste des éléments des différents 
        // RegistrationStudentCourseViewModel pour lesquels IsRegistrated est true
        Participant.VotesList = VotesVM.Where(v => v.IsRegistrated).Select(v => v.Vote).ToList();
        Context.SaveChanges();
        // On recrée la liste RegistrationsVM avec les nouvelles données
        RefreshVotes();
    }


    private void Cancel() {
        EditMode = false;
        // On recrée la liste RegistrationsVM avec les nouvelles données
        RefreshVotes();
    }

    private void Delete() {
        Participant.VotesList.Clear();
        Context.SaveChanges();
        // On recrée la liste RegistrationsVM avec les nouvelles données
        RefreshVotes();
    }
}
