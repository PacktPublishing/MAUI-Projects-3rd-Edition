﻿using MauiMigration.Models;
using MauiMigration.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;



namespace MauiMigration.Views
{
    public partial class NewItemPage : ContentPage
    {
        public Item Item { get; set; }

        public NewItemPage()
        {
            InitializeComponent();
            BindingContext = new NewItemViewModel();
        }
    }
}