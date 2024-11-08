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
using Microsoft.EntityFrameworkCore.Metadata;
using MyPoll.Model;
using MyPoll.ViewModel;
using PRBD_Framework;

namespace MyPoll.View;

public partial class PollDetailView : UserControlBase {
    public PollDetailView(Poll poll, bool isNew) {
        InitializeComponent();
        DataContext = new PollDetailViewModel(poll, isNew);
        DoDisplayGrid(poll);
        
    }

    private void DoDisplayGrid(Poll poll) {
        if (poll != null)
            new VoteGridView(poll);
    }

}
