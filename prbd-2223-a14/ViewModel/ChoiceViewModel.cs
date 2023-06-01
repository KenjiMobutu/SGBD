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
    public event Action<string, string> ValidationFailed;

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
    private bool _isNew;
    public bool IsNew {
        get => _isNew;
        set => SetProperty(ref _isNew, value);
    }

    private EditChoiceViewModel _editChoiceViewModel;
    public ChoiceViewModel() {}
    public ChoiceViewModel(EditChoiceViewModel editChoiceViewModel, Poll poll, Choice choice, bool isNew) {
        Poll = poll;
        IsNew = isNew;
        _editChoiceViewModel = editChoiceViewModel;
        Choice = choice;
        _totalVotes = NbVotesForChoice(choice);
        var editChoices = Choice.GetById(Poll.PollId).OrderBy(c => c.Label).ToList();
        _choices = new ObservableCollectionFast<Choice>(Poll.Choices.OrderBy(c => c.Label));
        EditCommand = new RelayCommand(() => EditMode = true);
        SaveCommand = new RelayCommand(SaveChoiceAction,CanSaveChoice);
        CancelCommand = new RelayCommand(Cancel, CanCancel);
        DeleteChoiceCommand2 = new RelayCommand(DeleteChoice, CanDeleteChoice);

        RaisePropertyChanged();
    }

    private void NotifyValidationFailed(string propertyName, string errorMessage) {
        ValidationFailed?.Invoke(propertyName, errorMessage);
    }


    public string ChoiceLabel {
        get => Choice?.Label;
        set => SetProperty(Choice.Label, value, Choice, (c,v) => {
            c.Label = v;
            NotifyColleagues(App.Messages.MSG_LABEL_CHANGED, Choice);
            Validate();
        });
    }
    public override bool Validate() {
        ClearErrors();
        Console.WriteLine("VALIDATE CHOICE LABEL ===>" + ChoiceLabel);
       
        if (string.IsNullOrEmpty(ChoiceLabel)) {
            AddError(nameof(ChoiceLabel), "Cannot be empty");
            NotifyColleagues(App.Messages.MSG_CHOICE_HASERROR, Choice);
        } else if (ChoiceLabel.Length < 3) {
            AddError(nameof(ChoiceLabel), "length must be >= 3");
            NotifyColleagues(App.Messages.MSG_CHOICE_HASERROR, Choice);
        } else if (ChoiceLabel.TrimStart() != ChoiceLabel) {
            AddError(nameof(ChoiceLabel), "cannot start with a space");
        } else if (ChoiceLabelExists()) {
            AddError(nameof(ChoiceLabel), "Label already in the choice list");
        }

        

        return !HasErrors;
    }
 
    private bool ChoiceLabelExists() {
        return Context.Choices.Any(choice => choice.Label == ChoiceLabel);
    }
    private Choice _choice;
    public Choice Choice {
        get => _choice;
        set=> SetProperty(ref _choice, value);
    }
    
    public void SaveChoiceAction() {
        
        //Context.SaveChanges();
        RaisePropertyChanged();
        EditMode = false;
    }
    private bool CanSaveChoice() {
        return !string.IsNullOrEmpty(ChoiceLabel) && !HasErrors;
    }
    private void Cancel() {
        EditMode = false;
        Choice.Reload();
        RaisePropertyChanged();
    }
    private bool CanCancel() {
        return !string.IsNullOrEmpty(ChoiceLabel) && !HasErrors;
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
        
        RaisePropertyChanged();
        RaisePropertyChanged(nameof(Choice));
        
        NotifyColleagues(ApplicationBaseMessages.MSG_REFRESH_DATA);
        NotifyColleagues(App.Messages.MSG_CHOICE_ADDED, Choice);
        EditMode = false;
        if (!IsNew) { Context.SaveChanges(); }
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
        _editChoiceViewModel.AskEditMode(EditMode);
    }
    public void Changes() {
        RaisePropertyChanged(nameof(Editable));
    }
     
    public bool Editable => !EditMode;
    

}
