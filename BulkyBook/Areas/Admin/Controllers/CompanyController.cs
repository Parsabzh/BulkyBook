using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.Areas.Admin.Controllers
{
   [Area("Admin")]
   [Authorize(Roles = StaticDetail.Role_Admin + "," + StaticDetail.Role_Employee)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult CompanyIndex()
        {
            return View();
        }

        public IActionResult CompanyUpsert(int? id)
        {
            var company = new Company();
            if (id==null)
            {
                //create
                return View(company);
            }

            company = _unitOfWork.Company.Get(id.GetValueOrDefault());
            if (company==null)
            {
                return NotFound();
            }

            return View(company);

         
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CompanyUpsert(Company company)
        {
            if (ModelState.IsValid)
            {
                if (company.Id == 0)
                {
                    _unitOfWork.Company.Add(company);
                }
                else
                {
                    _unitOfWork.Company.Update(company);
                    
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(CompanyIndex));
            }

            return View(company);
        }



        #region API CallS

        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.Company.GetAll();
            return Json(new {data = allObj});
        }

        public IActionResult Delete(int id)
        {
            var model = _unitOfWork.Company.Get(id);
            if (model==null)
            {
                return Json(new {success = false, message = "Company is not exist!"});
            }

            _unitOfWork.Company.Remove(model);
            _unitOfWork.Save();
            return Json(new {success = true, message = "Delete Successful"});

        }
        
        #endregion
    }
}