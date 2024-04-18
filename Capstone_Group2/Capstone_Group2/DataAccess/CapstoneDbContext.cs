using Microsoft.AspNetCore.Identity;

using Microsoft.EntityFrameworkCore;
using Capstone_Group2.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using Microsoft.Build.Framework;

namespace Capstone_Group2.DataAccess
{
    public class CapstoneDbContext : IdentityDbContext<User>
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Priority> Priorities { get; set; }
        public DbSet<Reminder> Reminders { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<TimetableTask> Tasks { get; set; }
        public DbSet<Timetable> timetables { get; set; }
        public DbSet<EmailNotification> EmailNotifications { get; set; }

        public CapstoneDbContext(DbContextOptions<CapstoneDbContext> options) : base(options) { }

        public static async Task CreateAdminUser(IServiceProvider serviceProvider)
        {
            UserManager<User> userManager =
                serviceProvider.GetRequiredService<UserManager<User>>();
            RoleManager<IdentityRole> roleManager = serviceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();

            string username = "admin";
            string password = "Admin123#";
            string roleName = "Admin";

            // if role doesn't exist, create it
            if (await roleManager.FindByNameAsync(roleName) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
            // if username doesn't exist, create it and add it to role
            if (await userManager.FindByNameAsync(username) == null)
            {
                User user = new User { UserName = username };
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, roleName);
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Category>().HasData(
                new Category() { CategoryId = 1, CategoryName = "School" },
                new Category() { CategoryId = 2, CategoryName = "Work"},
                new Category() { CategoryId = 3, CategoryName = "Personal" }
            );

            builder.Entity<Priority>().HasData(
                new Priority() { PriorityId = 1, PriorityType = "Low" },
                new Priority() { PriorityId = 2, PriorityType = "Medium" },
                new Priority() { PriorityId = 3, PriorityType = "High" }
            );

            builder.Entity<Status>().HasData(
                new Status() { StatusId = 1, StatusName = "Not Done" },
                new Status() { StatusId = 2, StatusName = "Due Soon" },
                new Status() { StatusId = 3, StatusName = "Finished" }
            );
            builder.Entity<Timetable>().HasData(
                new Timetable() { TimetableId = "1", UserId = "1" }
            );

            builder.Entity<TimetableTask>().HasData(
                new TimetableTask() { TaskId = 1, TaskName="First Task", TaskDescription = "First Task", Start_Date = new DateTime(2024,2,10), End_Date = new DateTime(2024,2,20), TimetableId = "1", CategoryId =1, PriorityId = 1, StatusId = 1 }
                );
        }


    }
}
