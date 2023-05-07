﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MyPoll.Model;
using MyPoll.View;
using PRBD_Framework;
using static MyPoll.App;

namespace MyPoll.ViewModel;

public class PollDetailViewModel : ViewModelCommon {
    private readonly Poll _poll;

    public Poll Poll {
        get => _poll;
        private init => SetProperty(ref _poll, value);
    }

    private ObservableCollection<Choice> _choices;
    public ObservableCollection<Choice> Choices {
        get => _choices;
        private set => SetProperty(ref _choices, value);
    }

    private bool _isNew;
    public bool IsNew {
        get => _isNew;
        set => SetProperty(ref _isNew, value);
    }
    public bool IsExisting => !_isNew;

    private ObservableCollection<VoteGridView> _voteGridViews;
    public ObservableCollection<VoteGridView> VoteGrid {
        get => _voteGridViews;
        set => SetProperty(ref _voteGridViews, value);
    }

    public ICommand DisplayEdit { get; set; }

    public ObservableCollection<VoteGridView> VoteGridViews { get; } = new ObservableCollection<VoteGridView>();

    private UserControl _editView;
    public UserControl EditView {
        get => _editView;
        set => SetProperty(ref _editView, value);
    }

    public PollDetailViewModel(Poll poll, bool isNew) : base() {
        IsNew = isNew;
        Poll = poll;

        // Ajouter seulement le VoteGridView du Poll sélectionné
        var voteGridView = new VoteGridView(Poll);
        VoteGridViews.Add(voteGridView);

        DisplayEdit = new RelayCommand(() => {
            EditView = new PollAddView(Poll);
        });


        // Assigner la liste VoteGrid à une nouvelle collection créée à partir de VoteGridViews
        VoteGrid = new ObservableCollection<VoteGridView>(VoteGridViews.Select(vg => new VoteGridView(poll)));
    }

    public string Title => Poll.Title;

    public User Creator => Poll.Creator;

    protected override void OnRefreshData() {
 
    }
}








