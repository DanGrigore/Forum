using CursLab8.Models;
using Lab10IdentityNew.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lab10IdentityNew.Controllers
{
    [Authorize]
    public class SearchController : Controller
    {
        private ApplicationDbContext db = ApplicationDbContext.Create();

        // GET: Search
        public ActionResult ArticlesByTitle(Search search)
        {
            var articles = db.Articles
                .Where(item => item.Title == search.Title)
                .ToList();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }
            ViewBag.Articles = articles;

            return View();
        }
    }
}