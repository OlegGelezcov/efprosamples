using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsStore.Models;

namespace SportsStore.Controllers
{
    public class SeedController : Controller
    {
        private DataContext context;

        public SeedController(DataContext ctx)
            => context = ctx;


        public IActionResult Index()
        {
            ViewBag.Count = context.Products.Count();
            return View(context.Products.Include(p => p.Category).OrderBy(p => p.Id).Take(20));
        }

        [HttpPost]
        public IActionResult CreateSeedData(int count )
        {
            ClearData();
            if(count > 0 )
            {
                context.Database.SetCommandTimeout(TimeSpan.FromMinutes(10));
                context.Database.ExecuteSqlCommand("DROP PROCEDURE IF EXISTS CreateSeedData");
                context.Database.ExecuteSqlCommand($@"create procedure CreateSeedData
		@RowCount decimal
	as
		begin
		set nocount on
		declare @i int = 1;
		declare @catId bigint;
		declare @CatCount int = @RowCount / 10;
		declare @pprice decimal(5,2);
		declare @rprice decimal(5,2);
		begin transaction
			while @i <= @CatCount
				begin 
					insert into Categories(Name, Description)
						values (CONCAT('Category-', @i), 'Test Data Category');
					set @catId = SCOPE_IDENTITY();
					declare @j int = 1;
					while @j <= 10
						begin 
							set @pprice = RAND() * (500 - 5 + 1);
							set @rprice = (RAND() * @pprice) + @pprice;
							insert into Products(Name, CategoryId, PurchasePrice, RetailPrice)
								values(CONCAT('Product', @i, '-', @j), @catId, @pprice, @rprice)
							set @j = @j + 1
						end
						set @i = @i + 1
				end
				commit
		end");

                context.Database.BeginTransaction();
                context.Database.ExecuteSqlCommand($"exec CreateSeedData @RowCount = {count}");
                context.Database.CommitTransaction();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult ClearData()
        {
            context.Database.SetCommandTimeout(TimeSpan.FromMinutes(10));
            context.Database.BeginTransaction();
            context.Database.ExecuteSqlCommand("delete from Orders");
            context.Database.ExecuteSqlCommand("delete from Categories");
            context.Database.CommitTransaction();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult CreateProductionData() {
            ClearData();

            context.Categories.AddRange(new Category[] {
                new Category {
                    Name = "Watersports",
                    Description = "Make a splash",
                    Products = new Product[] {
                        new Product {
                        Name = "Kayak", Description = "A boat for one person",
                        PurchasePrice = 200, RetailPrice = 275
                        },
new Product {
Name = "Lifejacket",
Description = "Protective and fashionable",
PurchasePrice = 40, RetailPrice = 48.95m
},
}
},
new Category {
Name = "Soccer",
Description = "The World's Favorite Game",
Products = new Product[] {
new Product {
Name = "Soccer Ball",
Description = "FIFA-approved size and weight",
PurchasePrice = 18, RetailPrice = 19.50m
},
new Product {
Name = "Corner Flags", Description
= "Give your playing field a professional touch",
PurchasePrice = 32.50m, RetailPrice = 34.95m
},
new Product {
Name = "Stadium",
Description = "Flat-packed 35,000-seat stadium",
PurchasePrice = 75000, RetailPrice = 79500
}
}
},
new Category {
Name = "Chess",
Description = "The Thinky Game",
Products = new Product[] {
new Product {
Name = "Thinking Cap",
Description = "Improve brain efficiency by 75%",
PurchasePrice = 10, RetailPrice = 16
},
new Product {
Name = "Unsteady Chair", Description
= "Secretly give your opponent a disadvantage",
PurchasePrice = 28, RetailPrice = 29.95m
},
new Product {
Name = "Human Chess Board",
Description = "A fun game for the family",
PurchasePrice = 68.50m, RetailPrice = 75
},
new Product {
Name = "Bling-Bling King",
Description = "Gold-plated, diamond-studded King",
PurchasePrice = 800, RetailPrice = 1200
}
}
}
});

            context.SaveChanges();
            return RedirectToAction(nameof(Index));

        }
    }
}