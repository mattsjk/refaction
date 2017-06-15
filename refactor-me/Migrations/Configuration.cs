namespace refactor_me.Migrations
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<refactor_me.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(refactor_me.Models.ApplicationDbContext context)
        {
            var products = new List<Product>
                {
                    new Product { ProductId = Guid.NewGuid(), Name="Samsung S7", Description="Samsung Galaxy S Series 7 with Android 7.0 Nougat", Price=1459.89M, DeliveryPrice=1299.50M },
                    new Product { ProductId = Guid.NewGuid(), Name="Samsung S6", Description="Samsung Galaxy S Series 6 with Android 6.0 Marshmallow", Price=1059.99M, DeliveryPrice=999.99M },
                    new Product { ProductId = Guid.NewGuid(), Name="Samsung J5", Description="Samsung Galaxy J Series 5 with Android 6.0 Marshmallow", Price=459.99M, DeliveryPrice=399.49M }
                };
            products.ForEach(prd => context.Products.AddOrUpdate(p => p.Name, prd));
            context.SaveChanges();

            var productOptions = new List<ProductOption>();
            foreach (Product p in products)
            {
                ProductOption po = new ProductOption();
                po.ProductOptionId = Guid.NewGuid();
                po.ProductId = p.ProductId;
                po.Name = "White";
                po.Description = "White " + p.Name;
                po.product = p;
                productOptions.Add(po);

                po = new ProductOption();
                po.ProductOptionId = Guid.NewGuid();
                po.ProductId = p.ProductId;
                po.Name = "Black";
                po.Description = "Black " + p.Name;
                po.product = p;
                productOptions.Add(po);

                po = new ProductOption();
                po.ProductOptionId = Guid.NewGuid();
                po.ProductId = p.ProductId;
                po.Name = "Grey";
                po.Description = "Grey " + p.Name;
                po.product = p;
                productOptions.Add(po);

                break;
            }
            productOptions.ForEach(prdOpt => context.ProductOptions.Add(prdOpt));
            context.SaveChanges();
        }
    }
}
