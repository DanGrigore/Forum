using Lab10IdentityNew.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CursLab8.Models
{
    public class Article
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        public DateTime Date { get; set; }

        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }


        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

       
        // Se aduaga acest atribut pentru a putea prelua toate categoriile unui model in helper
        public IEnumerable<SelectListItem> Categories { get; set; }

    }

    
}