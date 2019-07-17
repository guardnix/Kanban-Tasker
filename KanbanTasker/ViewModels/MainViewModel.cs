﻿using KanbanTasker.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanbanTasker.ViewModels
{
    public class MainViewModel : Observable
    {
        private ObservableCollection<BoardViewModel> _boardList;
        private BoardViewModel _current;

        public MainViewModel()
        {
            // Instantiate the list object.  You bind the XAML list to this in 'ItemSource'
            BoardList = new ObservableCollection<BoardViewModel>();

            // Create board
            BoardViewModel myBoard = new BoardViewModel();
            
            // Load Board

            // Add to list
            BoardList.Add(myBoard);
        }

        public ObservableCollection<BoardViewModel> BoardList
        {
            get
            {
                return _boardList;
            }
            set
            {
                BoardList = value;
                OnPropertyChanged("BoardList");
            }
        }

        public BoardViewModel Current
        {
            get
            {
                return _current;
            }
            set
            {
                _current = value;
                OnPropertyChanged("Current"); // Let the UI know to update this binding.
            }

        }
    }
}