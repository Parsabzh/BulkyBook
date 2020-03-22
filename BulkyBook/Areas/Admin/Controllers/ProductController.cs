using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult ProductIndex()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            var productViewModel = new ProductViewModel()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem()
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                CoverTypeList = _unitOfWork.CoverType.GetAll().Select(i => new SelectListItem()
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };
            if (id == null)
            {
                //create
                return View(productViewModel);
            }

            productViewModel.Product = _unitOfWork.Product.Get(id.GetValueOrDefault());
            if (productViewModel.Product == null)
            {
                return NotFound();
            }

            return View(productViewModel);


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductViewModel productVM)
        {
            if (ModelState.IsValid)
            {
                var webRootPath = _hostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;
                if (files.Any())
                {
                    var fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(webRootPath, @"images\products");
                    var extenstion = Path.GetExtension(files[0].FileName);
                    if (productVM.Product.ImageUrl != null)
                    {
                        // this is for edit and we need to remove old image

                        var imagePath = Path.Combine(webRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }

                    using (var filesStream= new FileStream(Path.Combine(uploads,fileName+extenstion),FileMode.Create))
                    {
                        files[0].CopyTo(filesStream);
                    }

                    productVM.Product.ImageUrl = @"\images\products\" + fileName + extenstion;
                }
                else
                {
                    //update when they do not change the image
                    if (productVM.Product.Id !=0)
                    {
                        var objFromDb = _unitOfWork.Product.Get(productVM.Product.Id);
                        productVM.Product.ImageUrl = objFromDb.ImageUrl;

                    }

               
                }
              
                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);

                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(ProductIndex));
            }
            else
            {
                productVM.CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem()
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
                productVM.CoverTypeList = _unitOfWork.CoverType.GetAll().Select(i => new SelectListItem()
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
                if (productVM.Product.Id !=0)
                {
                    productVM.Product = _unitOfWork.Product.Get(productVM.Product.Id);
                }
            }
            return View(productVM);
        }



        #region API CallS

        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.Product.GetAll(includeProperties:"Category,CoverType");
            return allObj==null ? Json(new {message = "No data has been found"}) : Json(new { data = allObj });
        }

        public IActionResult Delete(int id)
        {
            var model = _unitOfWork.Product.Get(id);
            if (model == null)
            {
                return Json(new { success = false, message = "Product is not exist!" });
            }
            var webRootPath = _hostEnvironment.WebRootPath;
            var imagePath = Path.Combine(webRootPath, model.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
            _unitOfWork.Product.Remove(model);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });

        }

        #endregion
    }
}