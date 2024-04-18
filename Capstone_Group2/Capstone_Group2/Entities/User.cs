using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace Capstone_Group2.Entities
{
    public class User : IdentityUser
    {
        public string? UserEmailAddress { get; set; }



    }
}
