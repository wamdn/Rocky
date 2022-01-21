using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Range(0.01, 99_999_999.99)]
        // [Column(TypeName = "decimal(10,2)")]      // set in ApplicationDbContext.OnModelCreating() 
        public decimal Price { get; set; }

        [MaxLength(50)]
        [Display(Name = "Short description")]
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }

        // OneToMany Category
        [Display(Name = "Category type")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        // OneToMany Application Type
        [Display(Name = "Application Type")]
        public int ApplicationTypeId { get; set; }

        [ForeignKey("ApplicationTypeId")]
        public virtual ApplicationType? ApplicationType { get; set; }
    }
}
