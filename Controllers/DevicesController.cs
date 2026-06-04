using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ITDestek.Data;
using ITDestek.Models.Entities;

namespace ITDestek.Controllers;

[Authorize]
public class DevicesController : Controller
{
    private readonly ApplicationDbContext _context;
    
    public DevicesController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<IActionResult> Index()
    {
        var devices = await _context.Devices
            .Include(d => d.AssignedUser)
            .Include(d => d.Department)
            .ToListAsync();
        
        return View(devices);
    }
    
    // GET: Devices/Create
    public IActionResult Create()
    {
        ViewBag.DeviceTypes = new[] { "Desktop", "Laptop", "Yazıcı", "Monitör", "Tablet", "Telefon" };
        ViewBag.Departments = new[] { "Fen İşleri", "Mali Hizmetler", "İnsan Kaynakları", "Yazı İşleri", 
                                     "Emlak ve İstimlak", "Park Bahçeler", "Temizlik İşleri", "Sağlık İşleri" };
        return View();
    }
    
    // POST: Devices/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string deviceName, string deviceType, string serialNumber, string assignedTo, string department)
    {
        if (string.IsNullOrEmpty(deviceName) || string.IsNullOrEmpty(serialNumber))
        {
            ModelState.AddModelError("", "Cihaz adı ve seri numarası gereklidir.");
            ViewBag.DeviceTypes = new[] { "Desktop", "Laptop", "Yazıcı", "Monitör", "Tablet", "Telefon" };
            ViewBag.Departments = new[] { "Fen İşleri", "Mali Hizmetler", "İnsan Kaynakları", "Yazı İşleri", 
                                         "Emlak ve İstimlak", "Park Bahçeler", "Temizlik İşleri", "Sağlık İşleri" };
            return View();
        }
        
        var device = new Device
        {
            Name = deviceName,
            SerialNumber = serialNumber,
            IsActive = true,
            PurchaseDate = DateTime.Now
        };
        
        // DeviceType enum'dan eşleştir
        device.Type = deviceType switch
        {
            "Desktop" => DeviceType.Desktop,
            "Laptop" => DeviceType.Laptop,
            "Yazıcı" => DeviceType.Printer,
            "Monitör" => DeviceType.NetworkDevice,
            "Tablet" => DeviceType.Laptop,
            "Telefon" => DeviceType.IPPhone,
            _ => DeviceType.Desktop
        };
        
        _context.Devices.Add(device);
        await _context.SaveChangesAsync();
        
        TempData["Success"] = $"{deviceName} başarıyla envantere eklendi.";
        return RedirectToAction(nameof(Index));
    }
}
