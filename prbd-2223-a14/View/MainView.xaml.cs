using PRBD_Framework;
using MyPoll.Model;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Controls;

namespace MyPoll.View;

public partial class MainView : WindowBase {
    public MainView() {
        InitializeComponent();

        Register<Poll>(App.Messages.MSG_NEW_POLL,
            poll => DoDisplayNewPoll(poll, true));

        Register<Poll>(App.Messages.MSG_DISPLAY_POLL,
            poll => DoDisplayPoll(poll, false));

        Register<Poll>(App.Messages.MSG_POLL_CHANGED,
            poll => DoRenameTab(string.IsNullOrEmpty(poll.Title) ? "<New Poll>" : poll.Title));

        Register<Poll>(App.Messages.MSG_TITLE_CHANGED,
            poll => DoRenameTab(string.IsNullOrEmpty(poll.Title) ? "<New Poll>" : poll.Title));

        Register<Poll>(App.Messages.MSG_CLOSE_TAB,
            poll => DoCloseTab(poll));

    }

    private void DoDisplayPoll(Poll poll, bool isNew) {
        if (poll != null)
            OpenTab(isNew ? "<New Poll>" : poll.Title, poll.Title, () => new PollDetailView(poll, isNew));
    }
    private void DoDisplayNewPoll(Poll poll, bool isNew) {
        Console.WriteLine("DO DISPLAY NEW POLL ===>" + isNew);
        if (poll != null)
            OpenTab(isNew ? "<New Poll>" : poll.Title, poll.Title, () => new PollAddView(poll, isNew));
    }
    private void OpenTab(string header, string tag, Func<UserControlBase> createView) {
        var tab = tabControl.FindByTag(tag);
        if (tab == null)
            tabControl.Add(createView(), header, tag);
        else
            tabControl.SetFocus(tab);
    }
    private void DoRenameTab(string header) {
        if (tabControl.SelectedItem is TabItem tab) {
            MyTabControl.RenameTab(tab, header);
            tab.Tag = header;
        }
    }

    private void DoCloseTab(Poll poll) {
        tabControl.CloseByTag(string.IsNullOrEmpty(poll.Title) ? "<New Poll>" : poll.Title);
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
