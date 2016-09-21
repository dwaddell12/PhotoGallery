using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PhotoGallery.Models;

namespace PhotoGallery.Controllers
{
    public class GalleryController : Controller
    {
        private UsersContext db = new UsersContext();

        //
        // GET: /Gallery/

        public ActionResult Index()
        {
            return View(db.Galleries.ToList());
        }

        //
        // GET: /Gallery/Details/5

        public ActionResult Details(int id = 0)
        {
            Gallery gallery = db.Galleries.Include(g => g.Photos).SingleOrDefault(g => g.Id == id);
            if (gallery == null)
            {
                return HttpNotFound();
            }
            return View(gallery);
        }

        //
        // GET: /Gallery/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Gallery/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create(Gallery gallery)
        {
            if (ModelState.IsValid)
            {
                if(gallery.Name != null)
                {
                    string valueOfName = gallery.Name;
                    IQueryable<Gallery> query = db.Galleries.Where(g => g.Name == valueOfName);
                    if(query.FirstOrDefault() == null)
                    {
                        //Gallery did not previously exist
                        gallery.DateCreated = DateTime.Now;
                        gallery.DateModified = DateTime.Now;
                        db.Galleries.Add(gallery);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        //Gallery already existed
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    //Gallery name was null
                    return RedirectToAction("Create");
                }
            }
            return RedirectToAction("Index");
        }

        //
        // GET: /Gallery/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Gallery gallery = db.Galleries.Find(id);
            if (gallery == null)
            {
                return HttpNotFound();
            }
            return View(gallery);
        }

        //
        // POST: /Gallery/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            Gallery gallery = db.Galleries.Find(id);
            IQueryable<Photo> query = db.Photos.Where(p => p.Album.Id == gallery.Id);
            if(query.Any())
            {
                //The gallery has photos
                foreach(Photo photo in query)
                {
                    string relativePath = photo.Path + photo.FileName;
                    string absolutePath = Server.MapPath(relativePath);
                    System.IO.File.Delete(absolutePath);
                    db.Photos.Remove(photo);
                }
            }
            db.Galleries.Remove(gallery);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}