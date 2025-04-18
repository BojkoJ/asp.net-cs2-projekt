using System.Collections.ObjectModel;
using System.Threading.Tasks;
using BOJ0043_App.Models;
using BOJ0043_App.Services;

namespace BOJ0043_App.ViewModels
{
    public class WorkspaceStatusHistoryViewModel
    {
        public ObservableCollection<WorkspaceStatusHistory> StatusHistory { get; set; } = new();
        public string WorkspaceName { get; set; }

        public WorkspaceStatusHistoryViewModel(Workspace workspace)
        {
            WorkspaceName = workspace.Name;
            LoadStatusHistoryAsync(workspace.Id);
        }

        private async void LoadStatusHistoryAsync(int workspaceId)
        {
            var service = new WorkspaceService();
            var history = await service.GetStatusHistoryAsync(workspaceId);
            StatusHistory.Clear();
            if (history != null)
            {
                foreach (var item in history)
                    StatusHistory.Add(item);
            }
        }
    }
}
