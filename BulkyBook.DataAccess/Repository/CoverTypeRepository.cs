using System;
using System.Collections.Generic;
using System.Text;
using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;

namespace BulkyBook.DataAccess.Repository
{
    public class CoverTypeRepository : Repository<CoverType>, ICoverTypeRepository
    {
        private readonly ApplicationDbContext _db;

        public CoverTypeRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }
        public void Update(CoverType coverType)
        {
            var model = _db.CoverTypes.Find(coverType.Id);
            if (model == null)
            {
                return;
            }
            model.Name = coverType.Name;
            _db.SaveChanges();
        }
    }
}
