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
public class EditChoiceViewModel : ViewModelCommon {
    private ICommand _addChoiceCommand;
    public ICommand AddChoiceCommand {
        get => _addChoiceCommand;
        set => SetProperty(ref _addChoiceCommand, value);
    }
    public EditChoiceViewModel() { }
    public EditChoiceViewModel(Poll poll) {
       
        Poll = poll;
        var pollId = poll.PollId;
        var editChoices = Choice.GetById(Poll.PollId).OrderBy(c => c.Label).ToList();
        _choices = new ObservableCollectionFast<Choice>(Poll.Choices);

        _choicesVM = new ObservableCollectionFast<ChoiceViewModel>(_choices
                        .Select(c => {
                            var vm = new ChoiceViewModel(this, poll, c);
                            vm.ChoiceChanged += () => {
                                Console.WriteLine("ChoiceChanged");
                                ChoicesVM.Remove(vm);
                            };
                            return vm;
                        }).OrderBy(vm => vm.ChoiceLabel));

        AddChoiceCommand = new RelayCommand(AddChoice, CanAddChoice);
        RaisePropertyChanged();

    }
    private ObservableCollectionFast<Choice> _choices;
    public ObservableCollectionFast<Choice> Choices => _choices;
    private Poll _poll;
    public Poll Poll {
        get => _poll;
        set => SetProperty(ref _poll, value);
    }
    private PollAddViewModel _pollAddVM;
    private readonly Choice _choice;
    public Choice Choice {
        get => _choice;
        private init => SetProperty(ref _choice, value);
    }
    // La visbilité des boutons de sauvegarde et d'annulation sont bindés sur cette propriété
    private bool _editMode;
    public bool EditMode {
        get => _editMode;
        set => SetProperty(ref _editMode, value);
    }

    private ObservableCollectionFast<ChoiceViewModel> _choicesVM;
    public ObservableCollectionFast<ChoiceViewModel> ChoicesVM => _choicesVM;
    public void AskEditMode(bool editMode) {
        EditMode = editMode;
        foreach (var c in ChoicesVM)
            c.Changes();
    }
    private bool CanAddChoice() {
        return !string.IsNullOrEmpty(NewChoiceLabel) && !HasErrors;

    }
    private bool LabelExists() {
        return Choices.Any(choice => choice.Label == NewChoiceLabel);
    }
    private void AddChoice() {
        var choice = new Choice { Label = NewChoiceLabel };
        Poll.Choices.Add(choice);
        Choices.Add(choice);

        var vm = new ChoiceViewModel(this, Poll, choice);
        vm.ChoiceChanged += () =>{
            Console.WriteLine("ChoiceChanged");
            ChoicesVM.Remove(vm);
        };

        int insertIndex = 0;
        while (insertIndex < ChoicesVM.Count && string.Compare(vm.ChoiceLabel, ChoicesVM[insertIndex].ChoiceLabel) > 0) {
            insertIndex++;
        }

        ChoicesVM.Insert(insertIndex, vm);
        Context.Choices.Add(choice);
        NewChoiceLabel = ""; // remise à zéro de la propriété pour permettre d'ajouter un nouveau choix
        ClearErrors();
        RaisePropertyChanged();
        RaisePropertyChanged(nameof(Choice));
        NotifyColleagues(App.Messages.MSG_POLL_CHANGED, Poll);
        NotifyColleagues(ApplicationBaseMessages.MSG_REFRESH_DATA);
        //Context.SaveChanges();  
    }

    private string _newChoiceLabel;
    public string NewChoiceLabel {
        get => _newChoiceLabel;
        set => SetProperty(ref _newChoiceLabel, value, () => Validate());
    }
    public override bool Validate() {
        ClearErrors();

        if (string.IsNullOrEmpty(NewChoiceLabel)) {

            AddError(nameof(NewChoiceLabel), "Cannot be empty");

        } else if (NewChoiceLabel.Length < 3) {
            AddError(nameof(NewChoiceLabel), "length must be >= 3");
        } else if (NewChoiceLabel.TrimStart() != NewChoiceLabel) {
            AddError(nameof(NewChoiceLabel), "cannot start with a space");
        } else if (LabelExists()) {
            AddError(nameof(NewChoiceLabel), "Label already in the choice list");
        }

        return !HasErrors;
    }

}

