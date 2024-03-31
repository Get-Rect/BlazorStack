using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorStack.Services.Models
{
    public class ChangePasswordRequest
    {
        public string newPassword { get; set; } = string.Empty;
    }
}
