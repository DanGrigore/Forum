using CursLab8.Models;
using Lab10IdentityNew;
using Lab10IdentityNew.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CursLab8.Controllers
{
    public class ArticleController : Controller
    {
        private ApplicationDbContext db = ApplicationDbContext.Create();

        [Authorize(Roles = "User,Moderator,Administrator")]
        public ActionResult Index()
        {
            var articles = db.Articles.Include("Category").Include("User");
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }
            ViewBag.Articles = articles;

            return View();
        }

        public ActionResult ArticlesByCategory(int id, int offset, int pageSize)
        {
            var articles = db.Articles
                .Include("Category")
                .Include("User")
                .Where(item => item.CategoryId == id)
                .OrderBy(item => item.Date)
                .Skip(offset)
                .Take(pageSize + 1);
            var categoryName = db.Categories.Where(item => item.CategoryId == id).First();
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }
            ViewBag.CategoryName = categoryName.CategoryName;
            ViewBag.Articles = articles.Take(pageSize);
            ViewBag.CategoryId = id;
            ViewBag.OffsetNext = offset + pageSize;
            ViewBag.OffsetPrev = offset - pageSize;
            ViewBag.HasNextPage = articles.Count() > pageSize ? true : false;

            return View();
        }

        public ActionResult Show(int id)
        {
            var messages = db.Messages.Include("Article").Include("User").Where(item => item.ArticleId == id);
            Article article = db.Articles.Find(id);
            ViewBag.Article = article;
            ViewBag.Category = article.Category;
            ViewBag.Messages = messages;
            ViewBag.MessagesNo = messages.Count();
            ViewBag.afisareButoane1 = false;
            ViewBag.afisareButoane2 = false;
            if (User.IsInRole("Moderator") || User.IsInRole("Administrator") || article.UserId == User.Identity.GetUserId())
            {
                ViewBag.afisareButoane1 = true;
            }
            if (User.IsInRole("Moderator") || User.IsInRole("Administrator"))
            {
                ViewBag.afisareButoane2 = true;
            }
            ViewBag.esteAdmin = User.IsInRole("Administrator");
            ViewBag.utilizatorCurent = User.Identity.GetUserId();
            return View(article);

        }

        [Authorize(Roles = "User,Moderator,Administrator")]
        public ActionResult New(int id = 1)
        {
            Article article = new Article();
            article.CategoryId = id;
            // preluam lista de categorii din metoda GetAllCategories()
            article.Categories = GetAllCategories();
            // Preluam ID-ul utilizatorului curent
            article.UserId = User.Identity.GetUserId();
            return View(article);
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllCategories()
        {
            // generam o lista goala
            var selectList = new List<SelectListItem>();
            // Extragem toate categoriile din baza de date
            var categories = from cat in db.Categories select cat;
            // iteram prin categorii
            foreach (var category in categories)
            {
                // Adaugam in lista elementele necesare pentru dropdown
                selectList.Add(new SelectListItem
                {
                    Value = category.CategoryId.ToString(),
                    Text = category.CategoryName.ToString()
                });
            }
            // returnam lista de categorii
            return selectList;
        }


        [HttpPost]
        [Authorize(Roles = "User,Moderator,Administrator")]
        public ActionResult New(Article article)
        {
            article.Categories = GetAllCategories();
            try
            {
                if (ModelState.IsValid)
                {
                    db.Articles.Add(article);
                    db.SaveChanges();
                    TempData["message"] = "New topic added!";
                    return RedirectToRoute(new
                    {
                        controller = "Article",
                        action = "ArticlesByCategory",
                        id = article.CategoryId,
                        offset = 0,
                        pageSize = 5
                    });
                }
                else
                {
                    return View(article);
                }
            }
            catch (Exception e)
            {
                return View(article);
            }
        }

        [Authorize(Roles = "User,Moderator,Administrator")]
        public ActionResult Edit(int id)
        {

            Article article = db.Articles.Find(id);
            ViewBag.Article = article;
            article.Categories = GetAllCategories();

            if (article.UserId == User.Identity.GetUserId() || User.IsInRole("Administrator") || User.IsInRole("Moderator"))
            {
                return View(article);
            }
            else
            {
                TempData["message"] = "You don't have the rights to do that!";
                return RedirectToAction("Index");
            }
        }


        [HttpPut]
        [Authorize(Roles = "User,Moderator,Administrator")]
        public ActionResult Edit(int id, Article requestArticle)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Article article = db.Articles.Find(id);

                    if (article.UserId == User.Identity.GetUserId() || User.IsInRole("Administrator") || User.IsInRole("Moderator"))
                    {
                        if (TryUpdateModel(article))
                        {
                            article.Title = requestArticle.Title;
                            article.Content = requestArticle.Content;
                            article.Date = requestArticle.Date;
                            article.CategoryId = requestArticle.CategoryId;
                            db.SaveChanges();
                            TempData["message"] = "Topic modified successfully!";
                        }
                        return RedirectToRoute(new
                        {
                            controller = "Article",
                            action = "Show",
                            id = article.Id
                        });
                    }
                    else
                    {
                        TempData["message"] = "You don't have the rights to do that!";
                        return RedirectToAction("Index");
                    }


                }
                else
                {
                    return View();
                }

            }
            catch (Exception e)
            {
                return View();
            }
        }

        [HttpDelete]
        [Authorize(Roles = "User,Moderator,Administrator")]
        public ActionResult Delete(int id)
        {

            Article article = db.Articles.Find(id);

            if (article.UserId == User.Identity.GetUserId() || User.IsInRole("Administrator") || User.IsInRole("Moderator"))
            {
                db.Articles.Remove(article);
                db.SaveChanges();
                TempData["message"] = "Topic deleted successfully!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "You don't have the rights to do that!";
                return RedirectToAction("Index");
            }

        }


    }
}