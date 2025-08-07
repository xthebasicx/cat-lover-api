using Microsoft.AspNetCore.Identity;

namespace cat_lover_api.Models.Entities
{
    public class User : IdentityUser
    {
        public ICollection<Comment>? Comments { get; set; }
    }
}
