using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KeycloakDemo.Pages
{
    [Authorize(Policy = "users")] 
    public class AuthenticationModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
