using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using PRBD_Framework;

namespace MyPoll.ViewModel;
public  class SignUpViewModel : ViewModelCommon {
    public ICommand SignUpCommand { get; set; }

    protected override void OnRefreshData() {

    }
    public void SignUpAction() {
            NotifyColleagues(App.Messages.MSG_SIGNUP);
        
    }
  



}
