using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky.Models.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<Product> Products { get; init; }
        public IEnumerable<Category> Categories { get; init; }
    }
}
