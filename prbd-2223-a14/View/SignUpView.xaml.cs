using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MyPoll.ViewModel;
using MyPoll.Model;
using PRBD_Framework;


namespace MyPoll.View;

public partial class SignUpView : WindowBase {
    //private readonly SignUpViewModel _vm;
    public SignUpView() {
        InitializeComponent();
        //DataContext = _vm = new SignUpViewModel();
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e) {
        Close();
    }
}
