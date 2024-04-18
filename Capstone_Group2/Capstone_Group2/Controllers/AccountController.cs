using Capstone_Group2.Entities;
using Capstone_Group2.Models;
using Capstone_Group2.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using System;
using Capstone_Group2.DataAccess;

namespace Capstone_Group2.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly EmailService _emailService;
        private readonly CapstoneDbContext _taskDbContext;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, EmailService emailService, CapstoneDbContext taskDbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _taskDbContext = taskDbContext;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = new User { UserName = registerViewModel.Username, UserEmailAddress = registerViewModel.Email, Email = registerViewModel.Email };

                var result = await _userManager.CreateAsync(user, registerViewModel.Password);

                if (result.Succeeded)
                {
                    // Create a new timetable for the user
                    var timetable = new Timetable
                    {
                        UserId = user.Id,
                        TimetableId = user.Id
                    };

                    _taskDbContext.timetables.Add(timetable);
                    await _taskDbContext.SaveChangesAsync();

                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(registerViewModel);
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = "")
        {
            var model = new LoginViewModel { ReturnUrl = returnUrl };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, isPersistent: model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    // Get the logged-in user
                    var user = await _userManager.FindByNameAsync(model.Username);
                    if (user != null)
                    {
                        // Get tasks due within the next 24 hours for the user
                        var tasksDueWithin24Hours = _taskDbContext.Tasks
                            .Where(t => t.TimetableId == user.Id && t.End_Date > DateTime.Now && t.End_Date <= DateTime.Now.AddDays(1))
                            .ToList();

                        if (tasksDueWithin24Hours.Any())
                        {
                            // Check if email notifications have already been sent for these tasks
                            var tasksWithNotifications = tasksDueWithin24Hours
                                .Where(task => !_taskDbContext.EmailNotifications.Any(en => en.UserId == user.Id && en.TaskId == task.TaskId && en.NotificationSent == true))
                                .ToList();

                            if (tasksWithNotifications.Any())
                            {
                                // Send email containing tasks due within 24 hours
                                await _emailService.SendTasksDueWithin24HoursEmail(user.Email, tasksWithNotifications);

                                // Update EmailNotifications table to mark checkboxes for sent notifications
                                foreach (var task in tasksWithNotifications)
                                {
                                    var emailNotification = new EmailNotification
                                    {
                                        UserId = user.Id,
                                        TaskId = task.TaskId,
                                        NotificationTimestamp = DateTime.Now,
                                        NotificationSent = true
                                    };

                                    //Change task's status to due soon
                                    task.StatusId = 2;
                                    _taskDbContext.EmailNotifications.Add(emailNotification);
                                }

                                await _taskDbContext.SaveChangesAsync();
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            ModelState.AddModelError("", "Username or Password is invalid");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}