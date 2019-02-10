using Microsoft.EntityFrameworkCore;
using SportsStore.Models.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportsStore.Models {
    public class DataRepository : IRepository {

        //private List<Product> data = new List<Product>();

        private DataContext context;

        public DataRepository(DataContext ctx) => context = ctx;

        public IEnumerable<Product> Products //=> context.Products.ToArray();
            => context.Products.Include(p => p.Category).ToArray();

        public void AddProduct(Product product) {
            //context.Add(product);
            //context.SaveChanges();
            context.Products.Add(product);
            context.SaveChanges();
        }

        public Product GetProduct(long key)
            //=> context.Products.Find(key);
            => context.Products.Include(p => p.Category).First(p => p.Id == key);

        public void UpdateProduct(Product product) {
            //context.Products.Update(product);
            //context.SaveChanges();

            /*
            Product p = GetProduct(product.Id);
            p.Name = product.Name;
            p.Category = product.Category;
            p.PurchasePrice = product.PurchasePrice;
            p.RetailPrice = product.RetailPrice;
            context.SaveChanges();*/

            Product p = context.Products.Find(product.Id);
            p.Name = product.Name;
            p.PurchasePrice = product.PurchasePrice;
            p.RetailPrice = product.RetailPrice;
            p.CategoryId = product.CategoryId;
            context.SaveChanges();
        }

        public void UpdateAll(Product[] products) {
            //context.Products.UpdateRange(products);

            Dictionary<long, Product> data = products.ToDictionary(p => p.Id);
            IEnumerable<Product> baseline = context.Products.Where(p => data.Keys.Contains(p.Id));

            foreach(Product databaseProduct in baseline) {
                Product requestProduct = data[databaseProduct.Id];
                databaseProduct.Name = requestProduct.Name;
                databaseProduct.Category = requestProduct.Category;
                databaseProduct.PurchasePrice = requestProduct.PurchasePrice;
                databaseProduct.RetailPrice = requestProduct.RetailPrice;
            }
            context.SaveChanges();
        }

        public void Delete(Product product) {
            context.Products.Remove(product);
            context.SaveChanges();
        }

        public PagedList<Product> GetProducts(QueryOptions options, long category = 0)
        {
            //return new PagedList<Product>(context.Products.Include(p => p.Category), options);
            IQueryable<Product> query = context.Products.Include(p => p.Category);
            if(category != 0 ) {
                query = query.Where(p => p.CategoryId == category);
            }
            return new PagedList<Product>(query, options);
        }
    }
}
