using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRBD_Framework;
using MyPoll.Model;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Windows;
using System.Collections.ObjectModel;

namespace MyPoll.ViewModel;

public class ChoiceViewModel : ViewModelCommon{
    public event Action ChoiceChanged;

    public ICommand EditCommand { get; }
    public ICommand DeleteChoiceCommand2 { get; }
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    private ICommand _addChoiceCommand;
    public ICommand AddChoiceCommand {
        get => _addChoiceCommand;
        set => SetProperty(ref _addChoiceCommand, value);
    }
    private int _totalVotes;
    public int TotalVotes => _totalVotes;   

    private ObservableCollectionFast<Choice> _choices;
    public ObservableCollectionFast<Choice> Choices => _choices;
    private readonly Poll _poll;

    public Poll Poll {
        get => _poll;
        private init => SetProperty(ref _poll, value);
    }
    
    private EditChoiceViewModel _editChoiceViewModel;
    public ChoiceViewModel() {}
    public ChoiceViewModel(EditChoiceViewModel editChoiceViewModel, Poll poll, Choice choice) {
        Poll = poll;
        _editChoiceViewModel = editChoiceViewModel;
        Choice = choice;
        _totalVotes = NbVotesForChoice(choice);
        var editChoices = Choice.GetById(Poll.PollId).OrderBy(c => c.Label).ToList();
        _choices = new ObservableCollectionFast<Choice>(Poll.Choices.OrderBy(c => c.Label));
        
        //_choicesVM = editChoices.Select(c => new ChoiceListViewModel( poll, _choices)).ToList();
        EditCommand = new RelayCommand(() => EditMode = true);
        SaveCommand = new RelayCommand(SaveChoiceAction);
        CancelCommand = new RelayCommand(Cancel);
        DeleteChoiceCommand2 = new RelayCommand(DeleteChoice, CanDeleteChoice);
        
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
    private bool LabelExists() {
        return Choices.Any(choice => choice.Label == NewChoiceLabel);
    }
    private void AddChoice() {

        var choice = new Choice { Label = NewChoiceLabel };
        Poll.Choices.Add(choice);
        Choices.Add(choice);
        NewChoiceLabel = ""; // remise à zéro de la propriété pour permettre d'ajouter un nouveau choix

        Context.SaveChanges();
        RaisePropertyChanged();
        RaisePropertyChanged(nameof(Choices));
        ClearErrors();
    }
    /*
   private ICommand _deleteChoiceCommand;
   public ICommand DeleteChoiceCommand {
        get {
            if (_deleteChoiceCommand == null) {
                _deleteChoiceCommand = new RelayCommand<int>(
                    DeleteChoice,
                    CanDeleteChoice
                );
            }
            return _deleteChoiceCommand;
        }
    }*/
    public Choice Choice { get; set; }
    
    public void SaveChoiceAction() {
        
        Context.SaveChanges();
        RaisePropertyChanged();
        EditMode = false;
    }
    private void Cancel() {
        EditMode = false;
        
        //RefreshChoices();
    }
    private void DeleteChoice() {
        Console.WriteLine("DELETE CHOICE");
        var choice = Poll.Choices.FirstOrDefault(c => c.ChoiceId == Choice.ChoiceId);

        if (choice != null) {
            if (NbVotesForChoice(choice) > 0) {
                // Afficher une boîte de dialogue de confirmation
                var result = MessageBox.Show("Le choix contient des votes. Êtes-vous sûr de vouloir le supprimer ?",
                    "Confirmation de suppression", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.No) {
                    return;
                }
            }
            Choices.Remove(choice);
            Poll.Choices.Remove(choice);
        }
        //_editChoiceViewModel.ChoicesVM.Remove(this);
        ChoiceChanged?.Invoke();
        Context.SaveChanges();
        RaisePropertyChanged();
        RaisePropertyChanged(nameof(Choice));
        NotifyColleagues(App.Messages.MSG_POLL_CHANGED, Poll);
        NotifyColleagues(ApplicationBaseMessages.MSG_REFRESH_DATA);
        NotifyColleagues(App.Messages.MSG_POLL_CHANGED, Poll);

        
        EditMode = false;
    }

    private bool CanDeleteChoice() {
        return Poll.Choices.Any(c => c.ChoiceId == Choice.ChoiceId);
    }
    public int NbVotesForChoice(Choice choice) {
        int totalVotes = 0;
        foreach (var c in Poll.Choices) {
            totalVotes += c.VotesList.Count(c => c.ChoiceId == choice.ChoiceId);
        }
        return totalVotes;
    }
    private bool _editMode;

    // La visbilité des boutons de sauvegarde et d'annulation sont bindés sur cette propriété
    public bool EditMode {
        get => _editMode;
        set => SetProperty(ref _editMode, value, EditModeChanged);
    }
    public void EditModeChanged() {
        

        // On informe le parent qu'on change le mode d'édition de la ligne
        _editChoiceViewModel.AskEditMode(EditMode);
    }
    public void Changes() {
        RaisePropertyChanged(nameof(Editable));
    }
     
    public bool Editable => !EditMode;
    

}
