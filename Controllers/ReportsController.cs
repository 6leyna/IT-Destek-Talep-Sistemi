using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ITDestek.Data;
using ITDestek.Models.Entities;
using ITDestek.Models.ViewModels;

namespace ITDestek.Controllers;

[Authorize]
public class ReportsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ReportsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");
        
        var userRoles = await _userManager.GetRolesAsync(user);
        
        var model = new ReportsViewModel();
        
        // Sadece Admin tüm talepleri görebilir
        IQueryable<Ticket> ticketsQuery;
        if (userRoles.Contains("Admin"))
        {
            ticketsQuery = _context.Tickets
                .Include(t => t.Category)
                .Include(t => t.Department);
        }
        else
        {
            // Diğerleri sadece kendi taleplerini görebilir
            ticketsQuery = _context.Tickets
                .Where(t => t.UserId == user.Id)
                .Include(t => t.Category)
                .Include(t => t.Department);
        }
        
        var tickets = await ticketsQuery.ToListAsync();
        
        // Genel İstatistikler
        model.TotalTickets = tickets.Count;
        model.OpenTickets = tickets.Count(t => t.Status == Status.Yeni || t.Status == Status.Atandi);
        model.InProgressTickets = tickets.Count(t => t.Status == Status.Islemde);
        model.ResolvedTickets = tickets.Count(t => t.Status == Status.Cozüldü);
        model.ClosedTickets = tickets.Count(t => t.Status == Status.Kapali);
        
        // Aylık talep verileri (son 6 ay)
        var sixMonthsAgo = DateTime.Now.AddMonths(-6);
        var monthlyData = tickets
            .Where(t => t.CreatedDate >= sixMonthsAgo)
            .GroupBy(t => new { t.CreatedDate.Year, t.CreatedDate.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .Select(g => new 
            {
                Month = $"{g.Key.Month}/{g.Key.Year}",
                Count = g.Count()
            })
            .ToList();
        
        model.Months = monthlyData.Select(x => x.Month).ToList();
        model.TicketCounts = monthlyData.Select(x => x.Count).ToList();
        
        // Kategori bazlı dağılım
        model.CategoryStats = tickets
            .GroupBy(t => t.Category)
            .Select(g => new CategoryStat
            {
                CategoryName = g.Key.Name,
                Count = g.Count(),
                Icon = g.Key.Icon ?? "fa-circle"
            })
            .OrderByDescending(x => x.Count)
            .Take(5)
            .ToList();
        
        // Departman bazlı dağılım
        model.DepartmentStats = tickets
            .GroupBy(t => t.Department)
            .Select(g => new DepartmentStat
            {
                DepartmentName = g.Key.Name,
                TicketCount = g.Count(),
                ResolvedCount = g.Count(t => t.Status == Status.Cozüldü || t.Status == Status.Kapali)
            })
            .OrderByDescending(x => x.TicketCount)
            .ToList();
        
        // Öncelik dağılımı
        model.LowPriority = tickets.Count(t => t.Priority == Priority.Düsük);
        model.MediumPriority = tickets.Count(t => t.Priority == Priority.Orta);
        model.HighPriority = tickets.Count(t => t.Priority == Priority.Yüksek);
        model.CriticalPriority = tickets.Count(t => t.Priority == Priority.Kritik);
        
        // Çözüm süreleri (ortalama)
        var resolvedTickets = tickets.Where(t => t.ResolvedDate.HasValue).ToList();
        if (resolvedTickets.Any())
        {
            var resolutionTimes = resolvedTickets
                .Where(t => t.ResolvedDate.HasValue)
                .Select(t => (t.ResolvedDate.Value - t.CreatedDate).TotalHours)
                .Where(h => h > 0)
                .ToList();
            
            if (resolutionTimes.Any())
            {
                model.AverageResolutionTime = resolutionTimes.Average();
                model.FastestResolution = resolutionTimes.Min();
                model.SlowestResolution = resolutionTimes.Max();
            }
        }
        
        return View(model);
    }
}
