namespace ITDestek.Models.ViewModels;

public class ReportsViewModel
{
    // Genel İstatistikler
    public int TotalTickets { get; set; }
    public int OpenTickets { get; set; }
    public int InProgressTickets { get; set; }
    public int ResolvedTickets { get; set; }
    public int ClosedTickets { get; set; }
    
    // Aylık Talep Verileri (Chart.js için)
    public List<string> Months { get; set; } = new();
    public List<int> TicketCounts { get; set; } = new();
    
    // Kategori Bazlı Dağılım
    public List<CategoryStat> CategoryStats { get; set; } = new();
    
    // Departman Bazlı Dağılım
    public List<DepartmentStat> DepartmentStats { get; set; } = new();
    
    // Öncelik Dağılımı
    public int LowPriority { get; set; }
    public int MediumPriority { get; set; }
    public int HighPriority { get; set; }
    public int CriticalPriority { get; set; }
    
    // Çözüm Süreleri (gün cinsinden)
    public double AverageResolutionTime { get; set; }
    public double FastestResolution { get; set; }
    public double SlowestResolution { get; set; }
}

public class CategoryStat
{
    public string CategoryName { get; set; } = "";
    public int Count { get; set; }
    public string Icon { get; set; } = "";
}

public class DepartmentStat
{
    public string DepartmentName { get; set; } = "";
    public int TicketCount { get; set; }
    public int ResolvedCount { get; set; }
}
