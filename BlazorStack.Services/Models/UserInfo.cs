using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorStack.Services.Models
{
    public class UserInfo
    {
        public string Email { get; set; } = string.Empty;

        public bool IsEmailConfirmed { get; set; }

        public Dictionary<string, string> Claims { get; set; } = [];
    }
}
