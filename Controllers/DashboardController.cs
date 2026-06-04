using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ITDestek.Data;
using ITDestek.Models.Entities;

namespace ITDestek.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    
    public DashboardController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }
        
        var userRoles = await _userManager.GetRolesAsync(user);
        var currentYear = DateTime.Now.Year;
        
        // Get ticket statistics based on user role
        IQueryable<Ticket> ticketsQuery;
        
        if (userRoles.Contains("Admin"))
        {
            // Sadece Admin tüm talepleri görür
            ticketsQuery = _context.Tickets.Include(t => t.User).Include(t => t.AssignedTo).Include(t => t.Category);
        }
        else
        {
            // Diğer roller (ITStaff, Employee) sadece kendi taleplerini görür
            ticketsQuery = _context.Tickets
                .Where(t => t.UserId == user.Id || t.AssignedToId == user.Id)
                .Include(t => t.User)
                .Include(t => t.AssignedTo)
                .Include(t => t.Category);
        }
        
        var tickets = await ticketsQuery.ToListAsync();
        
        var model = new DashboardViewModel
        {
            OpenTickets = tickets.Count(t => t.Status == Status.Yeni || t.Status == Status.Atandi),
            InProgressTickets = tickets.Count(t => t.Status == Status.Islemde),
            ResolvedTickets = tickets.Count(t => t.Status == Status.Cozüldü || t.Status == Status.Kapali),
            TotalTickets = tickets.Count,
            RecentTickets = tickets.OrderByDescending(t => t.CreatedDate).Take(10).ToList(),
            
            // Calculate average resolution time (for resolved tickets)
            AverageResolutionTime = tickets
                .Where(t => t.ResolvedDate.HasValue)
                .Select(t => (t.ResolvedDate.Value - t.CreatedDate).TotalHours)
                .DefaultIfEmpty(0)
                .Average()
        };
        
        // Format resolution time
        if (model.AverageResolutionTime >= 24)
        {
            model.AverageResolutionTimeFormatted = $"{model.AverageResolutionTime / 24:F1} gün";
        }
        else
        {
            model.AverageResolutionTimeFormatted = $"{model.AverageResolutionTime:F0} saat";
        }
        
        ViewBag.UserRole = userRoles.FirstOrDefault() ?? "Employee";
        ViewBag.UserName = $"{user.FirstName} {user.LastName}";
        
        return View(model);
    }
}

public class DashboardViewModel
{
    public int OpenTickets { get; set; }
    public int InProgressTickets { get; set; }
    public int ResolvedTickets { get; set; }
    public int TotalTickets { get; set; }
    public double AverageResolutionTime { get; set; }
    public string AverageResolutionTimeFormatted { get; set; } = string.Empty;
    public List<Ticket> RecentTickets { get; set; } = new();
}
