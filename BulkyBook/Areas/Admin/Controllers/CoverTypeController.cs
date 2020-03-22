using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BulkyBook.DataAccess.Repository;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CoverTypeController(IUnitOfWork unitOfWork)

        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            var coverType = new CoverType();
            if (id == null)
            {
                return View(coverType);
            }

            var parameter = new DynamicParameters();
            parameter.Add("@Id", id);
            coverType= _unitOfWork.SP_Call.OneRercord<CoverType>(StaticDetail.Proc_CoverType_Get, parameter);
          
            if (coverType != null)
            {
                return View(coverType);
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType coverType)
        {
            if (!ModelState.IsValid)
            {
                return View(coverType);
            }

            var parameter = new DynamicParameters();
            parameter.Add("@Name", coverType.Name);

            if (coverType.Id == 0)
            {
                _unitOfWork.SP_Call.Execute(StaticDetail.Proc_CoverType_Create, parameter);
            }
            else
            {
                parameter.Add("@Id", coverType.Id);
                _unitOfWork.SP_Call.Execute(StaticDetail.Proc_CoverType_Update, parameter);
            }

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));

        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.SP_Call.List<CoverType>(StaticDetail.Proc_CoverType_GetAll);
            return Json(new { data = allObj });
        }

        public IActionResult Delete(int id)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@Id", id);
            var model = _unitOfWork.SP_Call.OneRercord<CoverType>(StaticDetail.Proc_CoverType_Get, parameter);
            if (model == null)
            {
                return Json(new { success = false, message = "Cover Type  is not exist!" });
            }

            _unitOfWork.SP_Call.Execute(StaticDetail.Proc_CoverType_Delete, parameter);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });

        }
    }
}