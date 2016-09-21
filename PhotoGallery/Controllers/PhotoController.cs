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
    public class PhotoController : Controller
    {
        private UsersContext db = new UsersContext();

        //
        // GET: /Photo/

        public ActionResult Index()
        {
            var photos = db.Photos.Include(p => p.Album).Include(p => p.User).ToList();
            return View(photos);
        }

        //
        // GET: /Photo/Details/5

        public ActionResult Details(int id = 0)
        {
            Photo photo = db.Photos.Include(p => p.Album).Include(p => p.User).SingleOrDefault(p => p.Id == id);

            if (photo == null)
            {
                return HttpNotFound();
            }
            return View(photo);
        }

        //
        // GET: /Photo/Create
        [Authorize]
        public ActionResult Create()
        {
            var model = new Photo();

            List<SelectListItem> listItems = new List<SelectListItem>();
            var database = WebMatrix.Data.Database.Open("DefaultConnection");
            var selectQueryString = "SELECT Name FROM Galleries ORDER BY DateModified";
            foreach (var item in database.Query(selectQueryString))
            {
                listItems.Add(new SelectListItem()
                {
                    Text = item.Name,
                    Value = item.Name
                });
            }

            model.AlbumSelect = listItems;

            return View(model);
        }

        //
        // POST: /Photo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create(FormCollection collection, Photo photo)
        {
            if (ModelState.IsValid)
            {
                db.Configuration.LazyLoadingEnabled = false;
                HttpPostedFileBase file = Request.Files["userFileUpload"];
                if (file != null && file.ContentLength > 0)
                {
                    photo.FileName = file.FileName;
                    photo.FileType = file.ContentType;
                    photo.Size = file.ContentLength;
                    photo.Path = "~/uploads/";
                    string relativePath = photo.Path + photo.FileName;
                    string absolutePath = Server.MapPath(relativePath);
                    photo.UserName = User;
                    photo.User = db.UserProfiles.Where(p => p.UserName == User.Identity.Name).FirstOrDefault();
                    //Replace IPrincipal with UserProfiles perhaps
                    //Or add another column
                    if(collection["AlbumGallery"] == "")
                    {
                        IQueryable<Gallery> query = db.Galleries.Where(g => g.Name == "My Gallery");
                        if (query.FirstOrDefault() == null)
                        {
                            //"My Gallery" does not exist*
                            Gallery myGallery = new Gallery();
                            myGallery.Name = "My Gallery";
                            myGallery.Description = "My default gallery";
                            myGallery.DateCreated = DateTime.Now;
                            myGallery.DateModified = DateTime.Now;
                            photo.Album = myGallery;
                            myGallery.Photos.Add(photo);
                            db.Galleries.Add(myGallery);
                        }
                        else
                        {
                            //"My Gallery" exists
                            int galleryId = query.First().Id;
                            Gallery gallery = db.Galleries.Find(galleryId);
                            photo.Album = gallery;
                            gallery.Photos.Add(photo);
                        }
                    }
                    else
                    {
                        //AlbumSelect is not null
                        string result = collection["AlbumGallery"];
                        IQueryable<Gallery> query = db.Galleries.Where(g => g.Name == result);
                        int galleryId = query.First().Id;
                        Gallery tempGallery = db.Galleries.Find(galleryId);
                        photo.Album = tempGallery;
                        tempGallery.Photos.Add(photo);
                    }
                    file.SaveAs(absolutePath);
                    photo.Path = "/uploads/";
                    db.Photos.Add(photo);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    //No file selected
                    return RedirectToAction("Create");
                }
            }
            else
            {
                /*ModelState is not valid*/
                return RedirectToAction("Create");
            }
        }

        //
        // GET: /Photo/Details/photo.Path + photo.FileName
        public ActionResult Examine(Photo photo)
        {
            string filePath = photo.Path + photo.FileName;
            return RedirectToAction(filePath);
        }

        //
        // GET: /Photo/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Photo photo = db.Photos.Find(id);
            if (photo == null)
            {
                return HttpNotFound();
            }
            return View(photo);
        }

        //
        // POST: /Photo/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            Photo photo = db.Photos.Find(id);
            string relativePath = photo.Path + photo.FileName;
            string absolutePath = Server.MapPath(relativePath);
            System.IO.File.Delete(absolutePath);
            //photo.Album.Photos.RemoveAt(photo.Id);
            db.Photos.Remove(photo);
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