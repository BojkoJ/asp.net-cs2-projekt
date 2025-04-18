using System;

namespace BOJ0043_App.Models
{
    public class WorkspaceStatusHistory
    {
        public DateTime ChangedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public string StatusText => Status switch
        {
            "Available" => "Dostupné",
            "Occupied" => "Obsazené",
            "Maintenance" => "V údržbě",
            _ => Status
        };
    }
}
