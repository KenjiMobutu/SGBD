using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel;
public class LoginViewModel : ViewModelCommon{
    public ICommand LoginCommand { get; set; }

    private string _mail;

    public string Mail {
        get => _mail;
        set => SetProperty(ref _mail, value, () => Validate() );
    }

    private string _password;

    public string Password {
        get => _password;
        set => SetProperty(ref _password, value, () => Validate());
    }
    public LoginViewModel() : base(){
        LoginCommand = new RelayCommand(LoginAction,
            () => { return _mail != null && _password != null && !HasErrors; });
    }
    private void LoginAction() {
        if (Validate()) {
            var user = Context.Users.SingleOrDefault(user => user.Mail == Mail);
            NotifyColleagues(App.Messages.MSG_LOGIN, user);
        }
    }
    public override bool Validate() {
        ClearErrors();
        var user = Context.Users.SingleOrDefault(user => user.Mail == Mail);

        if (string.IsNullOrEmpty(Mail))
            AddError(nameof(Mail), "required");
        else if (Mail.Length < 3)
            AddError(nameof(Mail), "length must be >= 3");
        else if (user == null)
            AddError(nameof(Mail), "does not exist");
        else {
            if (string.IsNullOrEmpty(Password))
                AddError(nameof(Password), "required");
            else if (Password.Length < 3)
                AddError(nameof(Password), "length must be >= 3");
            else if (user != null && user.Password != Password)
                AddError(nameof(Password), "wrong password");
        }

        return !HasErrors;
    }
    protected override void OnRefreshData() {

    }
}

