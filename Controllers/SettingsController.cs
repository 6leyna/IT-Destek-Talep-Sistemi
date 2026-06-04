using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ITDestek.Data;
using ITDestek.Models.Entities;

namespace ITDestek.Controllers;

[Authorize(Roles = "Admin")]
public class SettingsController : Controller
{
    private readonly ApplicationDbContext _context;
    
    public SettingsController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<IActionResult> Index(string tab = "departmanlar")
    {
        ViewBag.ActiveTab = tab;
        
        var departments = await _context.Departments.OrderBy(d => d.Name).ToListAsync();
        ViewBag.Departments = departments;
        
        var categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
        ViewBag.Categories = categories;
        
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddDepartment(string name, string code)
    {
        if (string.IsNullOrEmpty(name))
        {
            TempData["Error"] = "Departman adı gereklidir.";
            return RedirectToAction(nameof(Index), new { tab = "departmanlar" });
        }
        
        var dept = new Department { Name = name, Code = code };
        _context.Departments.Add(dept);
        await _context.SaveChangesAsync();
        
        TempData["Success"] = $"{name} departmanı eklendi.";
        return RedirectToAction(nameof(Index), new { tab = "departmanlar" });
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteDepartment(int id)
    {
        var dept = await _context.Departments.FindAsync(id);
        if (dept != null)
        {
            _context.Departments.Remove(dept);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"{dept.Name} departmanı silindi.";
        }
        return RedirectToAction(nameof(Index), new { tab = "departmanlar" });
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddCategory(string name, string icon)
    {
        if (string.IsNullOrEmpty(name))
        {
            TempData["Error"] = "Kategori adı gereklidir.";
            return RedirectToAction(nameof(Index), new { tab = "kategoriler" });
        }
        
        var cat = new Category { Name = name, Icon = icon ?? "fa-circle" };
        _context.Categories.Add(cat);
        await _context.SaveChangesAsync();
        
        TempData["Success"] = $"{name} kategorisi eklendi.";
        return RedirectToAction(nameof(Index), new { tab = "kategoriler" });
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var cat = await _context.Categories.FindAsync(id);
        if (cat != null)
        {
            _context.Categories.Remove(cat);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"{cat.Name} kategorisi silindi.";
        }
        return RedirectToAction(nameof(Index), new { tab = "kategoriler" });
    }
}
