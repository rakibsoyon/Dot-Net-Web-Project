using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class ItemUploadModel
    {
        public int CategoryId { get; set; }

        public IFormFile File { get; set; }
    }
}
