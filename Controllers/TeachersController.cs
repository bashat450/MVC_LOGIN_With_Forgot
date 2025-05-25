using CrudMap.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CrudMap.Controllers
{
    public class TeachersController : Controller
    {
        Logic _logic = new Logic();
        // GET: Teachers
        public ActionResult Lists()
        {
            if (Session["UserEmail"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            return View(_logic.GetAllDetails());
        }

        // GET: Teachers/Details/5
        public ActionResult Details(int id)
        {
            var details = _logic.GetAllDetails().Find(de => de.Id == id);
            return View(details);
        }

        // GET: Teachers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Teachers/Create
        [HttpPost]
        public ActionResult Create(AllModel createModel, HttpPostedFileBase imageFile, HttpPostedFileBase pdfFile)
        {
            try
            {
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(imageFile.FileName);
                    string imagePath = Path.Combine(Server.MapPath("~/Images/"), fileName);
                    imageFile.SaveAs(imagePath);
                    createModel.ImagePath = "~/Images/" + fileName;
                }

                if (pdfFile != null && pdfFile.ContentLength > 0)
                {
                    string pdfName = Path.GetFileName(pdfFile.FileName);
                    string pdfPath = Path.Combine(Server.MapPath("~/PDFs/"), pdfName);
                    pdfFile.SaveAs(pdfPath);
                    createModel.PdfPath = "~/PDFs/" + pdfName;
                }

                _logic.CreateDetails(createModel);
                return RedirectToAction("Lists");
            }
            catch
            {
                return View(createModel);
            }
        }



        // GET: Teachers/Edit/5
        public ActionResult Edit(int id)
        {
            var edit = _logic.GetAllDetails().Find(ed => ed.Id == id);
            return View(edit);
        }

        // POST: Teachers/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, AllModel editModel, HttpPostedFileBase imageFile, HttpPostedFileBase PdfFile)
        {
            try
            {
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(imageFile.FileName);
                    string imagePath = Path.Combine(Server.MapPath("~/Images/"), fileName);
                    imageFile.SaveAs(imagePath);
                    editModel.ImagePath = "~/Images/" + fileName;
                }
                else
                {
                    // Keep existing image path if new image is not uploaded
                    var existing = new Logic().GetAllDetails().FirstOrDefault(x => x.Id == id);
                    if (existing != null)
                    {
                        editModel.ImagePath = existing.ImagePath;
                    }
                }
                if (PdfFile != null && PdfFile.ContentLength > 0)
                {
                    string pdfName = Path.GetFileName(PdfFile.FileName);
                    string pdfPath = Path.Combine(Server.MapPath("~/PDFs/"), pdfName);
                    PdfFile.SaveAs(pdfPath);
                    editModel.PdfPath = "~/PDFs/" + pdfName;
                }
                else
                {
                    var existing = _logic.GetAllDetails().FirstOrDefault(x => x.Id == id);
                    if (existing != null)
                    {
                        editModel.PdfPath = existing.PdfPath;
                    }
                }



                _logic.EditDetails(editModel);
                return RedirectToAction("Lists");
            }
            catch
            {
                return View(editModel);
            }
        }

        // GET: Teachers/Delete/5
        public ActionResult Delete(int id)
        {
            var delete = _logic.GetAllDetails().Find(del => del.Id == id);
            return View(delete);
        }

        // POST: Teachers/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                _logic.DeleteDetails(id);
                return RedirectToAction("Lists");
            }
            catch
            {
                return View();
            }
        }
    }
}
