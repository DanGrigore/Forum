using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lab10IdentityNew.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Comment { get; set; }
        [Required]
        public DateTime Date { get; set; }

        public int ArticleId { get; set; }
        public virtual CursLab8.Models.Article Article { get; set; }


        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}