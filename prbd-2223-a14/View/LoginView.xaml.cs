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
using MyPoll.Model;
using MyPoll.ViewModel;
using PRBD_Framework;

namespace MyPoll.View;
public partial class LoginView : WindowBase {
        public LoginView() {
            InitializeComponent();
        }

    private void btnCancel_Click(object sender, RoutedEventArgs e) {
        Close();
    }

    private void btnHarry_Click(object sender, RoutedEventArgs e) {
        var viewModel = (LoginViewModel)DataContext; // Récupérer l'instance de la classe LoginViewModel
        viewModel.Mail = "harry@test.com"; // Mettre à jour les informations d'identification de l'utilisateur
        viewModel.Password = "harry";
        viewModel.LoginCommand.Execute("harry@test.com"); // Appeler la méthode de connexion pour l'utilisateur normal
    }
   


    private void btnJohn_Click(object sender, RoutedEventArgs e) {
        var viewModel = (LoginViewModel)DataContext; // Récupérer l'instance de la classe LoginViewModel
        viewModel.Mail = "john@test.com"; // Mettre à jour les informations d'identification de l'utilisateur
        viewModel.Password = "john";
        viewModel.LoginCommand.Execute("john@test.com"); // Appeler la méthode de connexion pour l'utilisateur normal

    }

    private void btnAdmin_Click(object sender, RoutedEventArgs e) {
        var viewModel = (LoginViewModel)DataContext; // Récupérer l'instance de la classe LoginViewModel
        viewModel.Mail = "admin@test.com"; // Mettre à jour les informations d'identification de l'utilisateur
        viewModel.Password = "admin";
        viewModel.LoginCommand.Execute("admin@test.com");
    }

    






}

