using System.Security.Claims;
using Application.interfaces;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Security
{
    public class UserAccessor : IuserAccessor
    {
        private readonly IHttpContextAccessor httpContextAccessor;
       public UserAccessor(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
            
        }
        public string GetUsername()
        {
             return httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
        }
    }
}