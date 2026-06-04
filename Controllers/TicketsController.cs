using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using ITDestek.Data;
using ITDestek.Models.Entities;

namespace ITDestek.Controllers;

[Authorize]
public class TicketsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    
    public TicketsController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    
    // GET: Tickets
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");
        
        var userRoles = await _userManager.GetRolesAsync(user);
        
        IQueryable<Ticket> tickets;
        
        if (userRoles.Contains("Admin"))
        {
            // Sadece Admin tüm talepleri görebilir
            tickets = _context.Tickets
                .Include(t => t.User)
                .Include(t => t.Department)
                .Include(t => t.Category)
                .Include(t => t.AssignedTo)
                .OrderByDescending(t => t.CreatedDate);
        }
        else
        {
            // Diğerleri sadece kendi taleplerini veya kendisine atanan talepleri görebilir
            tickets = _context.Tickets
                .Where(t => t.UserId == user.Id || t.AssignedToId == user.Id)
                .Include(t => t.User)
                .Include(t => t.Department)
                .Include(t => t.Category)
                .Include(t => t.AssignedTo)
                .OrderByDescending(t => t.CreatedDate);
        }
        
        return View(await tickets.ToListAsync());
    }
    
    // GET: Tickets/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");
        
        var ticket = await _context.Tickets
            .Include(t => t.User)
            .Include(t => t.Department)
            .Include(t => t.Category)
            .Include(t => t.AssignedTo)
            .Include(t => t.Comments)
                .ThenInclude(c => c.User)
            .Include(t => t.Attachments)
            .FirstOrDefaultAsync(m => m.Id == id);
            
        if (ticket == null) return NotFound();
        
        var userRoles = await _userManager.GetRolesAsync(user);
        
        // Check authorization
        if (!userRoles.Contains("Admin") && !userRoles.Contains("ITStaff") && ticket.UserId != user.Id)
        {
            return RedirectToAction("AccessDenied", "Account");
        }
        
        // Load IT Staff for assignment dropdown (Admin only)
        if (userRoles.Contains("Admin"))
        {
            var itStaffUsers = await _userManager.GetUsersInRoleAsync("ITStaff");
            var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
            var allStaff = itStaffUsers.Concat(adminUsers);
            
            ViewBag.ITStaff = new SelectList(
                allStaff.Select(u => new { Value = u.Id, Text = $"{u.FirstName} {u.LastName}" }),
                "Value",
                "Text",
                ticket.AssignedToId);
            
            ViewBag.IsAdmin = true;
        }
        else
        {
            ViewBag.IsAdmin = false;
        }
        
        return View(ticket);
    }
    
    // GET: Tickets/Create
    public IActionResult Create()
    {
        ViewBag.Departments = new SelectList(_context.Departments.Where(d => d.IsActive), "Id", "Name");
        ViewBag.Categories = new SelectList(_context.Categories.Where(c => c.IsActive), "Id", "Name");
        return View();
    }
    
    // POST: Tickets/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("DepartmentId,CategoryId,Priority,Description,OfficeLocation")] TicketCreateViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");
        
        if (ModelState.IsValid)
        {
            var ticket = new Ticket
            {
                UserId = user.Id,
                DepartmentId = model.DepartmentId,
                CategoryId = model.CategoryId,
                Priority = model.Priority,
                Description = model.Description,
                Status = Status.Yeni,
                Title = $"{model.CategoryId} - {user.FirstName} {user.LastName}",
                OfficeLocation = model.OfficeLocation,
                CreatedDate = DateTime.Now
            };
            
            // Generate ticket number: IT-2026-0152
            var lastTicket = await _context.Tickets.OrderByDescending(t => t.Id).FirstOrDefaultAsync();
            var year = DateTime.Now.Year;
            var nextNumber = lastTicket != null ? 
                int.Parse(lastTicket.TicketNumber.Split('-')[2]) + 1 : 1;
            ticket.TicketNumber = $"IT-{year}-{nextNumber:D4}";
            
            _context.Add(ticket);
            await _context.SaveChangesAsync();
            
            // Create notification for IT staff
            var itStaff = await _userManager.GetUsersInRoleAsync("ITStaff");
            foreach (var staff in itStaff)
            {
                var notification = new Notification
                {
                    UserId = staff.Id,
                    Title = "Yeni Talep Oluşturuldu",
                    Message = $"{user.FirstName} {user.LastName} tarafından yeni bir talep oluşturuldu.",
                    CreatedDate = DateTime.Now
                };
                _context.Notifications.Add(notification);
            }
            await _context.SaveChangesAsync();
            
            TempData["Success"] = "Talebiniz başarıyla oluşturuldu. Talep Numaranız: " + ticket.TicketNumber;
            return RedirectToAction(nameof(Index));
        }
        
        ViewBag.Departments = new SelectList(_context.Departments.Where(d => d.IsActive), "Id", "Name", model.DepartmentId);
        ViewBag.Categories = new SelectList(_context.Categories.Where(c => c.IsActive), "Id", "Name", model.CategoryId);
        return View(model);
    }
    
    // GET: Tickets/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        
        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket == null) return NotFound();
        
        var user = await _userManager.GetUserAsync(User);
        var userRoles = await _userManager.GetRolesAsync(user);
        
        // Only IT Staff and Admin can edit
        if (!userRoles.Contains("Admin") && !userRoles.Contains("ITStaff"))
        {
            return RedirectToAction("AccessDenied", "Account");
        }
        
        ViewBag.Statuses = new SelectList(Enum.GetValues(typeof(Status)), (int)ticket.Status);
        ViewBag.ITStaff = new SelectList(
            (await _userManager.GetUsersInRoleAsync("ITStaff")).Concat(
            await _userManager.GetUsersInRoleAsync("Admin")), "Id", "Email", ticket.AssignedToId);
        
        return View(ticket);
    }
    
    // POST: Tickets/UpdateStatus
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, string newStatus)
    {
        var user = await _userManager.GetUserAsync(User);
        var userRoles = await _userManager.GetRolesAsync(user);
        
        // Only IT Staff and Admin can update status
        if (!userRoles.Contains("Admin") && !userRoles.Contains("ITStaff"))
        {
            return RedirectToAction("AccessDenied", "Account");
        }
        
        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket == null) return NotFound();
        
        // Convert string to Status enum
        var status = newStatus switch
        {
            "Yeni" => Status.Yeni,
            "Atandi" => Status.Atandi,
            "Islemde" => Status.Islemde,
            "Cozüldü" => Status.Cozüldü,
            "Kapali" => Status.Kapali,
            _ => Status.Yeni
        };
        
        ticket.Status = status;
        ticket.UpdatedDate = DateTime.Now;
        
        if (status == Status.Cozüldü)
        {
            ticket.ResolvedDate = DateTime.Now;
        }
        
        try
        {
            await _context.SaveChangesAsync();
            TempData["Success"] = "Talep durumu güncellendi.";
            return RedirectToAction(nameof(Details), new { id = id });
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TicketExists(ticket.Id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
    }
    
    // POST: Tickets/AssignTechnician
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignTechnician(int id, string technicianId)
    {
        var user = await _userManager.GetUserAsync(User);
        var userRoles = await _userManager.GetRolesAsync(user);
        
        // Only IT Staff and Admin can assign
        if (!userRoles.Contains("Admin") && !userRoles.Contains("ITStaff"))
        {
            return RedirectToAction("AccessDenied", "Account");
        }
        
        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket == null) return NotFound();
        
        ticket.AssignedToId = technicianId;
        ticket.Status = Status.Atandi;
        ticket.UpdatedDate = DateTime.Now;
        
        try
        {
            await _context.SaveChangesAsync();
            TempData["Success"] = "Teknik ataması yapıldı.";
            return RedirectToAction(nameof(Details), new { id = id });
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TicketExists(ticket.Id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
    }
    
    // POST: Tickets/AddComment
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddComment(int ticketId, string comment, bool isInternal = false)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");
        
        var ticket = await _context.Tickets.FindAsync(ticketId);
        if (ticket == null) return NotFound();
        
        var ticketComment = new TicketComment
        {
            TicketId = ticketId,
            UserId = user.Id,
            Comment = comment,
            IsInternal = isInternal,
            CreatedDate = DateTime.Now
        };
        
        _context.TicketComments.Add(ticketComment);
        await _context.SaveChangesAsync();
        
        TempData["Success"] = "Yorum eklendi.";
        return RedirectToAction(nameof(Details), new { id = ticketId });
    }
    
    // POST: Tickets/Delete
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");
        
        var userRoles = await _userManager.GetRolesAsync(user);
        
        // Only Admin and ITStaff can delete
        if (!userRoles.Contains("Admin") && !userRoles.Contains("ITStaff"))
        {
            return RedirectToAction("AccessDenied", "Account");
        }
        
        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket == null) return NotFound();
        
        _context.Tickets.Remove(ticket);
        await _context.SaveChangesAsync();
        
        TempData["Success"] = "Talep silindi.";
        return RedirectToAction(nameof(Index));
    }
    
    private bool TicketExists(int id)
    {
        return _context.Tickets.Any(e => e.Id == id);
    }

}

public class TicketCreateViewModel
{
    [Required(ErrorMessage = "Departman seçimi gereklidir.")]
    [Display(Name = "Departman")]
    public int DepartmentId { get; set; }
    
    [Required(ErrorMessage = "Kategori seçimi gereklidir.")]
    [Display(Name = "Kategori")]
    public int CategoryId { get; set; }
    
    [Required(ErrorMessage = "Öncelik seçimi gereklidir.")]
    [Display(Name = "Öncelik")]
    public Priority Priority { get; set; }
    
    [Required(ErrorMessage = "Açıklama gereklidir.")]
    [Display(Name = "Açıklama")]
    public string Description { get; set; } = string.Empty;
    
    [Display(Name = "Ofis Konumu (Bina / Oda)")]
    public string? OfficeLocation { get; set; }
}
