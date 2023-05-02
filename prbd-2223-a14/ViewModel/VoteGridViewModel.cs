using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel;
public class VoteGridViewModel : ViewModelCommon {

    public VoteGridViewModel() {
        _choices = Context.Choices.OrderBy(c => c.Label).ToList();

        var participants = Context.Users.OrderBy(p => p.Name).ToList();

        _participantsVM = participants.Select(p => new VoteParticipantViewModel(this, p, _choices)).ToList();
    }

    private bool _editMode;
    public bool EditMode {
        get => _editMode;
        set => SetProperty(ref _editMode, value);
    }

    private List<Choice> _choices;
    public List<Choice> Choices => _choices;

    private List<VoteParticipantViewModel> _participantsVM;
    public List<VoteParticipantViewModel> ParticipantsVM => _participantsVM;

    public void AskEditMode(bool editMode) {
        EditMode = editMode;

        // Change la visibilité des boutons de chacune des lignes
        // (voir la logique dans RegistrationStudentViewModel)
        foreach (var p in ParticipantsVM)
            p.Changes();
    }
}
