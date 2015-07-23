﻿using PerformanceMonitor.ViewModels;

namespace PerformanceMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            DataContext = new MainViewModel();
            
            InitializeComponent();
        }
    }
}
