using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorStack.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string PhotoUrl { get; set; } = string.Empty;
    }
}
