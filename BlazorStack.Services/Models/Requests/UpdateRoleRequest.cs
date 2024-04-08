using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorStack.Services.Models.Requests
{
    public class UpdateRoleRequest
    {
        public string NewRole { get; set; } = string.Empty;
    }
}
