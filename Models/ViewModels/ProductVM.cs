using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky.Models.ViewModels
{
    public class ProductVM
    {
        [Required]
        public Product Product { get; set; }
        public IEnumerable<SelectListItem>? CategorySelectList { get; set; }
    }
}
