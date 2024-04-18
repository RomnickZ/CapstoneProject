using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Capstone_Group2.Entities;

namespace Capstone_Group2.Services
{
    public class EmailService
    {
        private readonly string _fromMail = "timewizecapstone@gmail.com";
        private readonly string _fromPassword = "kbhyloycdzpjkcdh";

        public async Task SendTasksDueWithin24HoursEmail(string userEmail, List<TimetableTask> tasks)
        {
            var message = new MailMessage();
            message.From = new MailAddress(_fromMail);
            message.To.Add(new MailAddress(userEmail));
            message.Subject = "Tasks Due Within 24 Hours";

            // Construct email body with task details
            string body = "Task due within the next 24 hours:\n\n";
            foreach (var task in tasks)
            {
                body += $"Task Name: {task.TaskName}\n";
                body += $"Due Date: {task.End_Date}\n\n";
            }
            message.Body = body;

            using (var smtpClient = new SmtpClient("smtp.gmail.com"))
            {
                smtpClient.Port = 587;
                smtpClient.Credentials = new NetworkCredential(_fromMail, _fromPassword);
                smtpClient.EnableSsl = true;

                await smtpClient.SendMailAsync(message);
            }
        }
    }
}