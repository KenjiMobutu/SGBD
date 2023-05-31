using System.Windows;
using MyPoll.Model;
using MyPoll.ViewModel;
using PRBD_Framework;

namespace MyPoll; 

public partial class App : ApplicationBase<User, MyPollContext> {
    public enum Messages {
        MSG_NEW_MEMBER,
        MSG_PSEUDO_CHANGED,
        MSG_TITLE_CHANGED,
        MSG_LABEL_CHANGED,
        MSG_MEMBER_CHANGED,
        MSG_DISPLAY_MEMBER,
        MSG_CLOSE_TAB,
        MSG_LOGIN,
        MSG_LOGOUT,
        MSG_NEW_POLL,
        MSG_DISPLAY_POLL,
        MSG_POLL_CHANGED,
        MSG_POLL_SAVED,
        MSG_SIGNUP,
        MSG_DISPLAY_GRID,
        MSG_DISPLAY_CHANGED,
        MSG_EDIT_POLL
    }
    protected override void OnStartup(StartupEventArgs e) {
        base.OnStartup(e);

        // Clear database and seed data
        Context.Database.EnsureDeleted();
        Context.Database.EnsureCreated();

        Register<User>(this, Messages.MSG_LOGIN, user => {
            Login(user);
            NavigateTo<MainViewModel, User, MyPollContext>();
        });

        Register(this, Messages.MSG_LOGOUT, () => {
            Logout();
            NavigateTo<LoginViewModel, User, MyPollContext>();
        });

        Register<User>(this, Messages.MSG_NEW_MEMBER, user => {

            NavigateTo<SignUpViewModel, User, MyPollContext>();
        });

        Register<User>(this, App.Messages.MSG_SIGNUP, user => {
            CurrentUser = user;
            NavigateTo<MainViewModel, User, MyPollContext>();
            
        });

        Register<PollAddViewModel>(this, Messages.MSG_EDIT_POLL, pollAddViewModel => {

            NavigateTo<PollAddViewModel, User, MyPollContext>();

        });

        // Cold start
        //Console.Write("Cold starting database... ");
        //Context.Users.Find(0);
        //Console.WriteLine("done");

        // affichage du pseudo de tous les membres
        foreach (var u in Context.Users) {
            Console.WriteLine(u.Name);
        }

        // affichage du nombre d'instances de l'entité 'Member'
        Console.WriteLine("Nombres d'utilisateurs --> "+Context.Users.Count());
    }

    protected override void OnRefreshData() {
        if (CurrentUser?.Name != null)
            CurrentUser = User.GetByName(CurrentUser.Name);
    }
}
