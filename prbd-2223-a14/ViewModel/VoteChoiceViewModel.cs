using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyPoll.Model;
using PRBD_Framework;
using FontAwesome6;
using System.Windows.Media;

namespace MyPoll.ViewModel;
public class VoteChoiceViewModel : ViewModelCommon {


    private bool _isRegistrated;
    public bool IsRegistrated {
        get => _isRegistrated;
        set => SetProperty(ref _isRegistrated, value);
    }
    public EFontAwesomeIcon RegistratedIcon => IsRegistrated ? EFontAwesomeIcon.Solid_Check : EFontAwesomeIcon.None;
    public Brush RegistratedColor => IsRegistrated ? Brushes.Green : Brushes.White;
    public string RegistratedToolTip => IsRegistrated ? "Yes" : "No";
}

