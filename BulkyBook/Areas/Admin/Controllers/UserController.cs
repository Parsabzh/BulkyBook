using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetail.Role_Admin + "," + StaticDetail.Role_Employee)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;

        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult UserIndex()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {
            var model = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if (model==null)
            {
                return Json(new {success = false, message = "Error while Locking/Unlocking"});
            }

            if (model.LockoutEnd!=null && model.LockoutEnd > DateTime.Now)
            {
                //user is currently locked, we will unlock

                model.LockoutEnd = DateTime.Now;
            }
            else
            {
                model.LockoutEnd = DateTime.Now.AddYears(1000);
            }

            _db.SaveChanges();
            return Json(new {success = true, message = "Operation Successful"});
        }



        #region API CallS

        [HttpGet]
        public IActionResult GetAll()
        {
            var userList = _db.ApplicationUsers.Include(u => u.Company).ToList();
            var userRole = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();
            foreach (var user in userList)
            {
                var roleId = userRole.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;
                if (user.Company == null)
                {
                    user.Company=new Company()
                    {
                        Name = "",
                    };
                }
            }
            return Json(new {data = userList});
        }

        
        #endregion
    }
}