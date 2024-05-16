using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class ItemViewModel
    {

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Range(minimum:1,maximum:100)]
        public int Unit { get; set; } = 0;

        [Range(minimum: 1, maximum: 100)]
        public int Quantity { get; set; } = 0;

        [BindRequired]
        [Required(ErrorMessage = "The Category field is required")]
        public int CategoryId { get; set; }

        public string? CategoryName { get; set; }

        //public CategoryViewModel Categories { get; set; }
    }
}
