using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MyPoll.Model;
using PRBD_Framework;
using System.ComponentModel.DataAnnotations;

namespace MyPoll.ViewModel;
public  class SignUpViewModel : ViewModelCommon {
    private const string MailPropertyName = nameof(Mail);
    public ICommand SaveCommand { get; set; }
    public ICommand Cancel { get; set; }

    private User _user = new User();
    public User User {
        get => _user;
        set => SetProperty(ref _user, value);
    }

    private bool _isNew = true;
    public bool IsNew {
        get => _isNew;
        set => SetProperty(ref _isNew, value);
    }


    private string _mail;
    public string Mail {
        get => _mail;
        set => SetProperty(ref _mail, value, () => Validate());
    }

    private string _password;

    public string Password {
        get => _password;
        set => SetProperty(ref _password, value, () => Validate());
    }

    private string _passwordConfirm;

    public string PasswordConfirm {
        get => _passwordConfirm;
        set => SetProperty(ref _passwordConfirm, value, () => Validate());
    }

    private string _name;
    public string Name {
        get => _name;
        set => SetProperty(ref _name, value, () => Validate());
    }

    protected override void OnRefreshData() {
        if (IsNew || User == null) return;
        User = User.GetByName(User.Name);
        RaisePropertyChanged();
    }
  
    public SignUpViewModel()  {
      
        SaveCommand = new RelayCommand(SaveAction, CanSaveAction);

        RaisePropertyChanged();
    }

    public override void SaveAction() {

        if (IsNew) {
            User = new User {
                Mail = _mail,
                Name = _name,
                Password = SecretHasher.Hash(_password) 
            };
            Context.Add(User);
            IsNew = false;
        }
        
        Context.SaveChanges();
        RaisePropertyChanged();
        NotifyColleagues(App.Messages.MSG_SIGNUP, User);
        
    }

    private bool CanSaveAction() {
        if (IsNew)
            return !string.IsNullOrEmpty(Name);
        return User != null && User.IsModified;

        // return Validate();
    }

    /*public bool ValidateEmail() {

        Regex regex = new Regex(@"^([\w.-]+)@([\w-]+)((.(\w){2,3})+)$");
        Match match = regex.Match(Mail);
        if (string.IsNullOrEmpty(Mail))
            AddError(nameof(Mail), "required");
        else if (Mail.Length < 3)
            AddError(nameof(Mail), "length must be >= 3");
        else if (!match.Success)
            AddError(nameof(Mail), "Incorrect format of Mail");
        return !HasErrors;
    }*/
    public bool ValidateEmail() {
        var emailAttribute = new EmailAddressAttribute();
        if (string.IsNullOrEmpty(Mail)) {
            AddError(MailPropertyName, "required");
        } else if (!emailAttribute.IsValid(Mail)) {
            AddError(MailPropertyName, "Invalid email format");
        }
        return !HasErrors;
    }




    public bool ValidatePassword() {
        if (string.IsNullOrEmpty(Password))
            AddError(nameof(Password), "required");
        else if (Password.Length < 3)
            AddError(nameof(Password), "length must be >= 3");

        return !HasErrors;
    }

    public bool ValidatePasswordConfirm() {
       
        if (string.IsNullOrEmpty(PasswordConfirm))
            AddError(nameof(PasswordConfirm), "required");
        else if (PasswordConfirm.Length < 3)
            AddError(nameof(PasswordConfirm), "length must be >= 3");
        else if (!string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(PasswordConfirm) && Password != PasswordConfirm)
            AddError(nameof(PasswordConfirm), "passwords do not match");

        return !HasErrors;
    }

    public bool ValidateName() {
        if (string.IsNullOrEmpty(Name))
            AddError(nameof(Name), "required");
        else if (Name.Length < 3)
            AddError(nameof(Name), "length must be >= 3");

        return !HasErrors;
    }

    public override bool Validate() {
        ClearErrors();
        ValidateEmail();
        ValidatePassword();
        ValidatePasswordConfirm();
        ValidateName();
        return !HasErrors;
    }



}
