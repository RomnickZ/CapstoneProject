using Capstone_Group2.DataAccess;
using Capstone_Group2.Entities;
using Capstone_Group2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Capstone_Group2.Controllers
{
    public class TaskController : Controller
    {
        private CapstoneDbContext _taskDbContext;
        private readonly UserManager<User> _userManager;

        public TaskController(CapstoneDbContext taskDbContext, UserManager<User> userManager)
        {
            _userManager = userManager;
            _taskDbContext = taskDbContext;
        }

        //public async Task<IActionResult> HomePage()
        //{
        //    // Get the start and end dates for the current week
        //    var startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
        //    var endOfWeek = startOfWeek.AddDays(7).AddSeconds(-1);

        //    var currentUser = await _userManager.GetUserAsync(User);
        //    if (currentUser == null)
        //    {
        //        return NotFound(); // Handle the case where user is not found
        //    }

        //    var tasks = _taskDbContext.Tasks
        //        .Where(t => t.Start_Date >= startOfWeek && t.End_Date <= endOfWeek && t.TimetableId == currentUser.Id) // Filter tasks for the current week
        //        .OrderBy(t => t.End_Date)
        //        .ToList(); // Remove .Take(3) to get all tasks for the current week

        //    var categories = _taskDbContext.Categories.ToList();
        //    var statuses = _taskDbContext.Statuses.ToList();
        //    var priorities = _taskDbContext.Priorities.ToList();
        //    var tm = new List<TaskViewModel>();

        //    //add each task to the List of TaskViewModel
        //    foreach (var task in tasks)
        //    {
        //        TaskViewModel temptm = new TaskViewModel();
        //        temptm.TaskId = task.TaskId;
        //        temptm.TaskName = task.TaskName;
        //        temptm.TaskDescription = task.TaskDescription;
        //        temptm.Start_Date = task.Start_Date;
        //        temptm.End_Date = task.End_Date;
        //        //get the category name
        //        foreach (var category in categories)
        //        {
        //            if (task.CategoryId == category.CategoryId)
        //            {
        //                temptm.Category = category;
        //                break;
        //            }
        //        }
        //        //get the status name
        //        foreach (var status in statuses)
        //        {
        //            if (task.StatusId == status.StatusId)
        //            {
        //                temptm.Status = status;
        //                break;
        //            }
        //        }

        //        //get the priority type
        //        foreach (var priority in priorities)
        //        {
        //            if (task.PriorityId == priority.PriorityId)
        //            {
        //                temptm.Priority = priority;
        //                break;
        //            }
        //        }
        //        tm.Add(temptm);
        //    }

        //    return View("HomePage", tm);
        //}
        public async Task<IActionResult> HomePage()
        {
            // Get the start and end dates for the current week
            var today = DateTime.Today;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
            var endOfWeek = startOfWeek.AddDays(7).AddSeconds(-1);

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound(); // Handle the case where user is not found
            }

            var tasks = _taskDbContext.Tasks
                .Where(t => t.Start_Date >= startOfWeek && t.End_Date <= endOfWeek && t.TimetableId == currentUser.Id)
                .OrderBy(t => t.End_Date)
                .ToList();

            var taskViewModels = tasks.Select(task => new TaskViewModel
            {
                TaskId = task.TaskId,
                TaskName = task.TaskName,
                TaskDescription = task.TaskDescription,
                Start_Date = task.Start_Date,
                End_Date = task.End_Date,
                Category = _taskDbContext.Categories.FirstOrDefault(c => c.CategoryId == task.CategoryId),
                Status = _taskDbContext.Statuses.FirstOrDefault(s => s.StatusId == task.StatusId),
                Priority = _taskDbContext.Priorities.FirstOrDefault(p => p.PriorityId == task.PriorityId)
            }).ToList();

            return View("HomePage", taskViewModels);
        }



        // GET ALL TASKS
        [HttpGet("/tasks")]
        public async Task<IActionResult> GetAllTasks()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound(); // Handle the case where user is not found
            }

            var tasks = _taskDbContext.Tasks
                .Where(t => t.TimetableId.Equals(currentUser.Id))
                .OrderByDescending(t => t.Start_Date)
                .ToList();

            var categories = _taskDbContext.Categories
                .ToList();

            var statuses = _taskDbContext.Statuses
                .ToList();

            var priorities = _taskDbContext.Priorities
                .ToList();

            var tm = new List<TaskViewModel>();
            //add each task to the List of TaskViewModel
            foreach (var task in tasks)
            {
                TaskViewModel temptm = new TaskViewModel();
                temptm.TaskId = task.TaskId;
                temptm.TaskName = task.TaskName;
                temptm.TaskDescription = task.TaskDescription;
                temptm.Start_Date = task.Start_Date;
                temptm.End_Date = task.End_Date;
                //get the category name
                foreach (var category in categories)
                {
                    if (task.CategoryId == category.CategoryId)
                    {
                        temptm.Category = category;
                    }
                }
                //get the status name
                foreach (var status in statuses)
                {
                    if (task.StatusId == status.StatusId)
                    {
                        temptm.Status = status;
                    }
                }

                //get the priority type
                foreach (var priority in priorities)
                {
                    if (task.PriorityId == priority.PriorityId)
                    {
                        temptm.Priority = priority;
                    }
                }

                //add the TaskViewModel to the list
                tm.Add(temptm);

            }


            return View("Tasks", tm);
        }

        // GET TASK BY ID

        [HttpGet("/tasks/{id}/task-details")]
        public async Task<IActionResult> GetTaskById(int Id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound(); // Handle the case where user is not found
            }

            var task = _taskDbContext.Tasks
                .Where(t => t.TaskId == Id && t.TimetableId == currentUser.Id)
                .FirstOrDefault();

            return View("TaskDetails", task); // return need to be changed later
        }

        // GET TASK BY CATEGORY

        [HttpGet("/tasks/{categoryId}")]
        public async Task<IActionResult> GetTaskByCategory(int categoryId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound(); // Handle the case where user is not found
            }

            var tasks = _taskDbContext.Tasks
                .Where(t => t.CategoryId == categoryId && t.TimetableId == currentUser.Id)
                .ToList();

            var categories = _taskDbContext.Categories
                .ToList();

            var statuses = _taskDbContext.Statuses
                .ToList();

            var priorities = _taskDbContext.Priorities
               .ToList();

            var tm = new List<TaskViewModel>();
            //add each task to the List of TaskViewModel
            foreach (var task in tasks)
            {
                TaskViewModel temptm = new TaskViewModel();
                temptm.TaskId = task.TaskId;
                temptm.TaskName = task.TaskName;
                temptm.TaskDescription = task.TaskDescription;
                temptm.Start_Date = task.Start_Date;
                temptm.End_Date = task.End_Date;
                //get the category name
                foreach (var category in categories)
                {
                    if (task.CategoryId == category.CategoryId)
                    {
                        temptm.Category = category;
                    }
                }
                //get the status name
                foreach (var status in statuses)
                {
                    if (task.StatusId == status.StatusId)
                    {
                        temptm.Status = status;
                    }
                }

                //get the priority type
                foreach (var priority in priorities)
                {
                    if (task.PriorityId == priority.PriorityId)
                    {
                        temptm.Priority = priority;
                    }
                }

                //add the TaskViewModel to the list
                tm.Add(temptm);

            }


            return View("Tasks", tm);
        }

        // CREATE NEW TASK

        [HttpGet("/tasks/add-request")]
        [Authorize]
        public async Task<IActionResult> GetAddTaskRequest()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound(); // Handle the case where user is not found
            }

            var priorities = _taskDbContext.Priorities.ToList(); // Retrieve priorities from the database
            ViewBag.Priorities = priorities;
            ViewBag.TimetableId = currentUser.Id;
            return View("Create", new TaskCreateModel());
        }

        [HttpPost("/tasks/add-requests")]
        [Authorize]
        public async Task<IActionResult> CreateTask(TaskCreateModel taskModel)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound(); // Handle the case where user is not found
            }

            if (ModelState.IsValid)
            {
                var newTask = new TimetableTask
                {
                    TaskName = taskModel.TaskName,
                    CategoryId = taskModel.TaskType,
                    TaskDescription = taskModel.TaskDescription,
                    Start_Date = taskModel.StartDate,
                    End_Date = taskModel.DueDate,
                    StatusId = taskModel.StatusType,
                    PriorityId = taskModel.PriorityType, // Assign Priority Type from model
                    TimetableId = currentUser.Id
                };

                _taskDbContext.Tasks.Add(newTask);
                _taskDbContext.SaveChanges();

                return RedirectToAction("GetAllTasks", "Task");
            }
            else
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    // Log or display the error messages
                    Console.WriteLine(error.ErrorMessage);
                }

                var priorities = _taskDbContext.Priorities.ToList(); // Retrieve priorities from the database
                ViewBag.Priorities = priorities;
                return View("Create", taskModel);
            }
        }


        // EDIT TASK BY ID

        [HttpGet("/tasks/{TaskId}/edit-request")]
        [Authorize]
        public IActionResult GetEditRequestById(int TaskId)
        {
            var task = _taskDbContext.Tasks.Find(TaskId);
            if (task == null)
            {
                return NotFound();
            }

            ViewBag.TimetableId = task.TimetableId;
            return View("EditTask", task);
        }

        [HttpPost("/tasks/{TaskId}/edit-requests")]
        [Authorize]
        public IActionResult ProcessEditRequestById(int TaskId, TimetableTask task) //Id wasn't working so i use TaskId
        {
            if (ModelState.IsValid)
            {
                var existingTask = _taskDbContext.Tasks.Find(TaskId);
                if (existingTask == null)
                {
                    return NotFound();
                }
                existingTask.TaskName = task.TaskName;
                existingTask.TaskDescription = task.TaskDescription;
                existingTask.Start_Date = task.Start_Date;
                existingTask.End_Date = task.End_Date;
                existingTask.CategoryId = task.CategoryId;
                existingTask.PriorityId = task.PriorityId;
                existingTask.StatusId = task.StatusId;
                _taskDbContext.SaveChanges();
                return RedirectToAction("GetAllTasks", "Task");
            }
            else
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    // Log or display the error messages
                    Console.WriteLine(error.ErrorMessage);
                }
            }
            return View("EditTask", task);
        }


        // DELETE TASK BY ID

        [HttpGet("/tasks/{id}/delete-request")]
        [Authorize]
        public IActionResult GetDeleteRequestById(int id)
        {
            var task = _taskDbContext.Tasks.Find(id);
            return View("DeleteConfirmation", task);
        }

        [HttpPost("/tasks/{id}/delete-requests")]
        [Authorize]
        public IActionResult ProcessDeleteRequestById(int id)
        {
            var task = _taskDbContext.Tasks.Find(id);

            if (task == null)
            {
                return NotFound();
            }

            // Find email notifications for the task and remove them
            var emailNotifications = _taskDbContext.EmailNotifications
                .Where(en => en.TaskId == id)
                .ToList();

            _taskDbContext.EmailNotifications.RemoveRange(emailNotifications);

            // Remove the task
            _taskDbContext.Tasks.Remove(task);

            // Save changes to the database
            _taskDbContext.SaveChanges();

            return RedirectToAction("GetAllTasks", "Task");
        }
    }
}