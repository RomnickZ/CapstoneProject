using Capstone_Group2.DataAccess;
using Capstone_Group2.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Capstone_Group2.Controllers
{
    public class CalendarController : Controller
    {
        private readonly CapstoneDbContext _dbContext;
        private readonly UserManager<User> _userManager;

        public CalendarController(CapstoneDbContext dbContext, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public IActionResult Calendar()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound(); // Handle the case where user is not found
            }

            var tasks = _dbContext.Tasks
                .Include(t => t.Priority)
                .Include(t => t.Category)
                .Include(t => t.Status)
                .Where(t => t.TimetableId == currentUser.Id)
                .ToList();

            var events = tasks.Select(task => new
            {
                id = task.TaskId,
                title = task.TaskName,
                start = task.Start_Date.HasValue ? task.Start_Date.Value.ToString("yyyy-MM-ddTHH:mm:ss") : null,
                end = task.End_Date.HasValue ? task.End_Date.Value.ToString("yyyy-MM-ddTHH:mm:ss") : null,
                category = task.Category?.CategoryName,
                status = task.Status?.StatusName,
                priorityId = task.Priority?.PriorityId
            });

            return Json(events);
        }

    }
}