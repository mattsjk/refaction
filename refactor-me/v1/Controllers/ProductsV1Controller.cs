using log4net;
using refactor_me.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace refactor_me.v1.Controllers
{
    [RoutePrefix("v1/products")]
    public class ProductsV1Controller : ApiController
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ApplicationDbContext dbcontext = new ApplicationDbContext();

        // GET: /v1/products/
        [Route]
        [HttpGet]
        public ProductVO GetAllProducts()
        {
            return GetProductByName("");
        }

        // GET: /v1/products?name={ProductName}
        [Route]
        [HttpGet]
        public ProductVO GetProductByName(string name)
        {
            List<Product> productList;

            if (name == "") // Get All
            {
                logger.Info("GetProductByName() - Retrieving all products.");
                productList = dbcontext.Products.ToList();
            }
            else
            {
                logger.Info("GetProductByName() - Retrieving products with name:[" + name + "].");
                productList = dbcontext.Products.Where(p => p.Name.ToLower() == name.ToLower()).ToList();
                logger.Info("GetProductByName() - Retrieved [" + productList.Count + "] products with name:[" + name + "].");
            }

            ProductVO allproducts = new ProductVO();
            foreach (Product p in productList)
            {
                allproducts.Items.Add(p);
            }
            return allproducts;

        }

        // GET: /v1/products/{id}
        [Route("{id:Guid}")]
        [HttpGet]
        public Product GetProductByGuid(Guid id)
        {
            logger.Info("GetProductByGuid() - Retrieving product with ID:[" + id + "].");
            Product curProduct = dbcontext.Products.FirstOrDefault(p => p.ProductId == id);
            return curProduct;
        }

        // POST: /v1/products
        [Route]
        [HttpPost]
        public IHttpActionResult AddProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                logger.Error("AddProduct() - Invalid ModelState received in the POST request.");
                return BadRequest(ModelState);
            }
            // Verify the data format
            if ((product.DeliveryPrice < 0) ||
                (product.Price < 0) ||
                (product.Name == null) ||
                (product.Description == null))
            {

                logger.Error("AddProduct() - Invalid data format received for the Product.");
                return StatusCode(HttpStatusCode.BadRequest);
            }

            // No need to check for duplicates as similar names are allowed
            product.ProductId = Guid.NewGuid();
            dbcontext.Products.Add(product);

            try
            {
                dbcontext.SaveChanges();
                logger.Info("AddProduct() - Added new product with ID:[" + product.ProductId + "] and Name:[" + product.Name + "].");
            }
            catch (DbUpdateException)
            {
                logger.Error("AddProduct() - DB Error while trying to add new product with ID:[" + product.ProductId + "] and Name:[" + product.Name + "].");
                // Returning 500 as the request could not be fulfilled
                return StatusCode(HttpStatusCode.InternalServerError);
            }

            // Return success with the created product
            return Ok(product);
        }

        // PUT: /v1/products/{id}
        [Route("{id:Guid}")]
        [HttpPut]
        public IHttpActionResult UpdateProduct(Guid id, Product updatedproduct)
        {
            if (!ModelState.IsValid)
            {
                logger.Error("UpdateProduct() - Invalid ModelState received in the POST request.");
                return BadRequest(ModelState);
            }
            Product curproduct = GetProductByGuid(id);
            if (curproduct == null)
            {
                logger.Error("UpdateProduct() - Product with ID:[" + id + "] not found or unable to retrieve this product.");
                return NotFound(); //StatusCode(HttpStatusCode.NotFound); //ErrorCode:404
            }
            if (!updatedproduct.ProductId.Equals(id))
            {
                updatedproduct.ProductId = curproduct.ProductId;
            }
            dbcontext.Entry(curproduct).CurrentValues.SetValues(updatedproduct);

            try
            {
                dbcontext.SaveChanges();
                logger.Info("UpdateProduct() - Updated product with ID:[" + curproduct.ProductId + "] and Name:[" + curproduct.Name + "].");
            }
            catch (DbUpdateException)
            {
                logger.Error("UpdateProduct() - DB Error while trying to update product with ID:[" + curproduct.ProductId + "] and Name:[" + curproduct.Name + "].");
                // Returning 500 as the request could not be fulfilled
                return StatusCode(HttpStatusCode.InternalServerError);
            }
            return Ok(curproduct);
        }

        // DELETE: /v1/products/{id}
        [Route("{id:Guid}")]
        [HttpDelete]
        public IHttpActionResult DeleteProduct(Guid id)
        {
            Product product = GetProductByGuid(id);
            if (product == null)
            {
                logger.Warn("DeleteProduct() - Product with ID:[" + id + "] not found or unable to retrieve this product.");
                return NotFound(); //StatusCode(HttpStatusCode.NotFound); //ErrorCode:404
            }
            dbcontext.Products.Remove(product);
            try
            {
                dbcontext.SaveChanges();
                logger.Info("DeleteProduct() - Product with ID:[" + id + "] removed successfully.");
            }
            catch (DbUpdateException)
            {
                logger.Error("DeleteProduct() - DB error occurred while trying to delete product with ID:[" + id + "].");
                // Returning 500 as the request could not be fulfilled
                return StatusCode(HttpStatusCode.InternalServerError);
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

    }
}
