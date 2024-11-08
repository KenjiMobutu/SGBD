﻿using System;
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
public partial class PollAddView : UserControlBase {
   public PollAddView() {
        InitializeComponent();
   }
    public PollAddView(Poll poll, bool IsNew) {
        InitializeComponent();
        DataContext = new PollAddViewModel(poll, IsNew);
        DoDisplayChoice(poll,IsNew);
    }
   private void DoDisplayChoice(Poll poll, bool IsNew) {
        if (poll != null)
            new EditChoiceView(poll,IsNew);
    }
}

