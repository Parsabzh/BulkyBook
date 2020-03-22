using System;
using System.Collections.Generic;
using System.Text;
using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;

namespace BulkyBook.DataAccess.Repository
{
   public class ProductRepository:Repository<Product>,IProductRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }
        public void Update(Product product)
        {
            var model = _db.Products.Find(product.Id);
            if (model == null)
            {
                return;
            }

            if (product.ImageUrl!=null)
            {
                model.ImageUrl = product.ImageUrl;
            }
            model.ISBN = product.ISBN;
            model.Price = product.Price;
            model.Price50 = product.Price50;
            model.ListPrice = product.ListPrice;
            model.Price100 = product.Price100;
            model.Title = product.Title;
            model.Description = product.Description;
            model.CategoryId = product.CategoryId;
            model.Author = product.Author;
            model.CoverTypeId = product.CoverTypeId;
 
        }

       
    }
}
