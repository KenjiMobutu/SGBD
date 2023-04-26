using PRBD_Framework;
using MyPoll.Model;
using System.Windows.Input;
using System.ComponentModel;

namespace MyPoll.View;

public partial class MainView : WindowBase {
    public MainView() {
        InitializeComponent();
    }
    private void MenuLogout_Click(object sender, System.Windows.RoutedEventArgs e) {
        NotifyColleagues(App.Messages.MSG_LOGOUT);
    }
    private void WindowBase_KeyDown(object sender, KeyEventArgs e) {
        if (e.Key == Key.Q && Keyboard.IsKeyDown(Key.LeftCtrl))
            Close();
    }
    protected override void OnClosing(CancelEventArgs e) {
        base.OnClosing(e);
        tabControl.Dispose();
    }
}
