using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using ITDestek.Models.Entities;

namespace ITDestek.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<AccountController> _logger;
    
    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }
    
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        
        // Check if email domain is @tuzla.bel.tr
        if (!model.Email.EndsWith("@tuzla.bel.tr", StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(string.Empty, "Sadece @tuzla.bel.tr uzantılı kurumsal e-posta adresleri ile giriş yapılabilir.");
            return View(model);
        }
        
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
        {
            ModelState.AddModelError(string.Empty, "E-posta adresi veya şifre hatalı.");
            return View(model);
        }
        
        if (!user.IsActive)
        {
            ModelState.AddModelError(string.Empty, "Hesabınız pasif durumda. Lütfen sistem yöneticisi ile iletişime geçin.");
            return View(model);
        }
        
        var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: true);
        
        if (result.Succeeded)
        {
            _logger.LogInformation("User logged in: {Email}", user.Email);
            
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            
            return RedirectToAction("Index", "Dashboard");
        }
        
        if (result.RequiresTwoFactor)
        {
            return RedirectToAction(nameof(LoginWith2fa), new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
        }
        
        if (result.IsLockedOut)
        {
            _logger.LogWarning("User account locked out: {Email}", user.Email);
            ModelState.AddModelError(string.Empty, "Hesabınız çok fazla başarısız giriş denemesi nedeniyle kilitlendi. Lütfen daha sonra tekrar deneyin.");
            return View(model);
        }
        
        ModelState.AddModelError(string.Empty, "Giriş başarısız. Lütfen bilgilerinizi kontrol edin.");
        return View(model);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("User logged out");
        return RedirectToAction(nameof(Login));
    }
    
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
    
    [HttpGet]
    public IActionResult LoginWith2fa(bool rememberMe, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        ViewData["RememberMe"] = rememberMe;
        return View();
    }
}

public class LoginViewModel
{
    [Required(ErrorMessage = "E-posta adresi gereklidir.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    [Display(Name = "Kurumsal E-posta")]
    public string Email { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Şifre gereklidir.")]
    [DataType(DataType.Password)]
    [Display(Name = "Şifre")]
    public string Password { get; set; } = string.Empty;
    
    [Display(Name = "Beni Hatırla")]
    public bool RememberMe { get; set; }
}
