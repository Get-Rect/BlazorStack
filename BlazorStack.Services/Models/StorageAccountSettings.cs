using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorStack.Services.Models
{
    public class StorageAccountSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string ProfilePhotosContainer { get; set; } = string.Empty;

        public void Validate()
        {
            if (string.IsNullOrEmpty(ProfilePhotosContainer)) throw new Exception("Missing profile photos container app setting.");
        }
    }
}
