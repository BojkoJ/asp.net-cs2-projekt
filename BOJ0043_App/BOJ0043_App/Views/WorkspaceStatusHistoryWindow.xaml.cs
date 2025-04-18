using System.Windows;
using BOJ0043_App.Models;
using BOJ0043_App.ViewModels;

namespace BOJ0043_App.Views
{
    public partial class WorkspaceStatusHistoryWindow : Window
    {
        public WorkspaceStatusHistoryWindow(Workspace workspace)
        {
            InitializeComponent();
            DataContext = new WorkspaceStatusHistoryViewModel(workspace);
        }
    }
}
