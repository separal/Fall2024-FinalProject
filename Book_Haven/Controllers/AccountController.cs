using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Fall2024_Assignment3_separal.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

public class AccountController : Controller
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;

    public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    // Login Action
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }







    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Fetch the user from the database
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null)
            {
                // Check the password manually
                var hasher = new PasswordHasher<IdentityUser>();
                var verificationResult = hasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);

                if (verificationResult == PasswordVerificationResult.Success)
                {
                    // Proceed to sign in if the password matches
                    await _signInManager.SignInAsync(user, model.RememberMe);
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Invalid password.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid username.");
            }
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt. Please check your username and password.");
        return View(model);
    }



    // Register Action
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Set UserName to the email address
            var user = new IdentityUser
            {
                UserName = model.Email, // Use Email as the UserName
                Email = model.Email     // Set the Email property
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        return View(model);
    }


    // Logout Action
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}

