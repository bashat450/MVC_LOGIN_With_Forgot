using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CrudMap.Models
{
    public class AllModel
    {
        [Required(ErrorMessage = "ID is required")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Image Path is required")]
        public string ImagePath { get; set; } // For storing image file name/path


        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public HttpPostedFileBase ImageFile { get; set; } // For file upload


        [Required(ErrorMessage = "PDF Path is required")]
        public string PdfPath { get; set; } // For storing PDF path


        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public HttpPostedFileBase PdfFile { get; set; } // For file upload


    }
}
