using SalesTrackingSystem.ViewModels;
using System;
using System.Windows;
namespace SalesTrackingSystem.Views
{ /// <summary> /// Interaction logic for ExpenseView.xaml 
    /// </summary>
    
    public partial class ExpenseView : Window 
    { public ExpenseView() 
        { 
            InitializeComponent(); 
            DataContext = new ExpenseViewModel(); 
        } 
    } 
}