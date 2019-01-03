using Lab10IdentityNew.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lab10IdentityNew.Controllers
{
    public class MessageController : Controller
    {
        private ApplicationDbContext db = ApplicationDbContext.Create();

        // GET: Messages
        public ActionResult Index(int id)
        {
            var messages = db.Messages.Include("Article").Include("User").Where(item => item.ArticleId == id);
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }
            ViewBag.Messages = messages;

            return View();
 
        }

        public ActionResult New(int id)
        {
            Message message = new Message();
            message.ArticleId = id;
            message.UserId = User.Identity.GetUserId();
            return View(message);
        }

        [HttpPost]
        public ActionResult New(Message message)
        {
            try
            {
                db.Messages.Add(message);
                db.SaveChanges();
                TempData["message"] = "Comentariu adaugat!";
                return RedirectToRoute(new
                {
                    controller = "Article",
                    action = "Show",
                    id = message.ArticleId
                });
            }
            catch (Exception e)
            {
                return View();
            }
        }

        public ActionResult Edit(int id)
        {

            Message message = db.Messages.Find(id);
            ViewBag.Message = message;

            if (message.UserId == User.Identity.GetUserId() || User.IsInRole("Administrator")
                || User.IsInRole("Moderator"))
            {
                return View(message);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui articol care nu va apartine!";
                return ReturnToShow(message.ArticleId);
            }
        }


        [HttpPut]
        [Authorize(Roles = "User,Moderator,Administrator")]
        public ActionResult Edit(int id, Message requestMessage)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Message message = db.Messages.Find(id);

                    if (message.UserId == User.Identity.GetUserId() || User.IsInRole("Administrator")
                        || User.IsInRole("Moderator"))
                    {
                        if (TryUpdateModel(message))
                        {
                            message.Comment = requestMessage.Comment;
                            message.Date = requestMessage.Date;
                            db.SaveChanges();
                            TempData["message"] = "Commentul a fost modificat!";
                        }
                        return RedirectToRoute(new
                        {
                            controller = "Article",
                            action = "Show",
                            id = message.ArticleId
                        });
                    }
                    else
                    {
                        TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui comment care nu va apartine!";
                        return ReturnToShow(message.ArticleId);
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
        [Authorize(Roles = "Moderator,Administrator")]
        public ActionResult Delete(int id)
        {

            Message message = db.Messages.Find(id);

            if (message.UserId == User.Identity.GetUserId() || User.IsInRole("Administrator")
                || User.IsInRole("Moderator"))
            {
                db.Messages.Remove(message);
                db.SaveChanges();
                TempData["message"] = "Comentariul a fost sters!";
                return ReturnToShow(message.ArticleId);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti un comment care nu va apartine!";
                return ReturnToShow(message.ArticleId);
            }

        }

        [NonAction]
        private ActionResult ReturnToShow(int articleId)
        {
            return RedirectToRoute(new
            {
                controller = "Article",
                action = "Show",
                id = articleId
            });
        }

    }
}