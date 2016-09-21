using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace PhotoGallery.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public int Size { get; set; }
        public byte[] ByteContent { get; set; }
        public virtual IPrincipal UserName { get; set; }
        public UserProfile User { get; set; }
        public Gallery Album { get; set; }
        public IEnumerable<SelectListItem> AlbumSelect { get; set; }
    }
}