using log4net;
using refactor_me.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace refactor_me.v2.Controllers
{
    [RoutePrefix("v2/products")]
    public class ProductsV2Controller : ApiController
    {

        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ApplicationDbContext dbcontext = new ApplicationDbContext();

        // GET: /v2/products/
        [Route]
        [HttpGet]
        public ProductVO GetAllProducts()
        {
            return GetProductByName("");
        }

        // GET: /v2/products?name={ProductName}
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

        // GET: /v2/products/{id}
        [Route("{id:Guid}")]
        [HttpGet]
        public Product GetProductByGuid(Guid id)
        {
            logger.Info("GetProductByGuid() - Retrieving product with ID:[" + id + "].");
            Product curProduct = dbcontext.Products.FirstOrDefault(p => p.ProductId == id);
            return curProduct;
        }

        // POST: /v2/products
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

        // PUT: /v2/products/{id}
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

        // DELETE: /v2/products/{id}
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

        // GET: /v2/products/{id}/options
        [Route("{id:Guid}/options")]
        [HttpGet]
        public ProductOptionVO GetAllProductOptions(Guid id)
        {
            logger.Info("GetAllProductOptions() - Received request to retrieve all product options for product ID:[" + id + "].");
            ProductOptionVO allproductoptions = new ProductOptionVO();
            Product curproduct = GetProductByGuid(id);
            if (curproduct != null)
            {
                logger.Info("GetAllProductOptions() - Retrieving all product options for product ID:[" + curproduct.ProductId + "] and Name:[" + curproduct.Name + "].");
                List<ProductOption> productoptions;
                productoptions = dbcontext.ProductOptions.Where(po => po.ProductId == id).ToList();
                productoptions.ForEach(po => allproductoptions.Items.Add(po));
            }
            else
            {
                logger.Warn("GetAllProductOptions() - Product with ID:[" + id + "] not found or unable to retrieve this product.");
                return null;
            }
            return allproductoptions;
        }

        // GET: /v2/products/{id}/options/{optionId}
        [Route("{id:Guid}/options/{oid:Guid}")]
        [HttpGet]
        public ProductOption GetProductOptionByOptionId(Guid id, Guid oid)
        {
            logger.Info("GetProductOptionByOptionId() - Retrieving product options for ProductID:[" + id + "] and ProductOptionID:[" + oid + "].");
            ProductOption curProductOption = dbcontext.ProductOptions.FirstOrDefault(po => po.ProductId == id && po.ProductOptionId == oid);
            if (curProductOption == null)
            {
                logger.Warn("GetProductOptionByOptionId() - No product options found for ProductID:[" + id + "] and ProductOptionID:[" + oid + "].");
            }
            return curProductOption;
        }

        // POST: /v2/products/{id}/options
        [Route("{id:Guid}/options")]
        [HttpPost]
        public IHttpActionResult AddProductOption(Guid id, ProductOption newproductoption)
        {
            if (!ModelState.IsValid)
            {
                logger.Error("AddProductOption() - Invalid ModelState received in the POST request.");
                return BadRequest(ModelState);
            }
            // Verify the data format
            if ((newproductoption.Name == null) || (newproductoption.Description == null))
            {
                logger.Error("AddProductOption() - Invalid data format received for the ProductOption.");
                return StatusCode(HttpStatusCode.BadRequest);
            }
            // Check if the product with the product ID exists and retrieve it
            Product curproduct = GetProductByGuid(id);
            if (curproduct == null)
            {
                logger.Error("AddProductOption() - No product with ProductID:[" + id + "] found.");
                return StatusCode(HttpStatusCode.BadRequest);
            }

            // No need to check for duplicates as similar names are allowed
            newproductoption.ProductOptionId = Guid.NewGuid();
            newproductoption.ProductId = curproduct.ProductId;
            newproductoption.product = curproduct;
            dbcontext.ProductOptions.Add(newproductoption);
            try
            {
                dbcontext.SaveChanges();
                logger.Info("AddProductOption() - Added new product option with ID:[" + newproductoption.ProductOptionId + "] and Name:[" + newproductoption.Name + "].");
            }
            catch (DbUpdateException)
            {
                logger.Error("AddProductOption() - DB Error while trying to add new product option with ID:[" + newproductoption.ProductOptionId + "] and Name:[" + newproductoption.Name + "].");
                // Returning 500 as the request could not be fulfilled
                return StatusCode(HttpStatusCode.InternalServerError);
            }

            // Return success with the created product
            return Ok(newproductoption);
        }

        // PUT: /v2/products/{id}/options/{optionId}
        [Route("{id:Guid}/options/{oid:Guid}")]
        [HttpPut]
        public IHttpActionResult UpdateProductOption(Guid id, Guid oid, ProductOption updatedproductoption)
        {
            logger.Debug("UpdateProductOption() - Start of method.");
            if (!ModelState.IsValid)
            {
                logger.Error("UpdateProductOption() - Invalid ModelState received in the POST request.");
                return BadRequest(ModelState);
            }
            ProductOption curproductoption = GetProductOptionByOptionId(id, oid);
            if (curproductoption == null)
            {
                logger.Error("UpdateProductOption() - ProductOption with ID:[" + oid + "] not found or unable to retrieve this product.");
                return NotFound(); //StatusCode(HttpStatusCode.NotFound); //ErrorCode:404
            }
            // Verify if the Product ID matches the updatedProductOption Product ID
            if (!curproductoption.ProductId.Equals(id))
            {
                logger.Error("UpdateProductOption() - The product with product ID in the URL:[" + id + "] does not match the product ID for the ProductOption:[" + curproductoption.ProductId + "].");
                return StatusCode(HttpStatusCode.BadRequest);
            }
            // Assign the updated ProductOption to the database context for update
            curproductoption.product = GetProductByGuid(id);
            curproductoption.Name = updatedproductoption.Name;
            curproductoption.Description = updatedproductoption.Description;
            dbcontext.ProductOptions.Attach(curproductoption);
            var entry = dbcontext.Entry(curproductoption);
            entry.State = EntityState.Modified;
            try
            {
                dbcontext.SaveChanges();
                logger.Info("UpdateProductOption() - Updated ProductOption with ID:[" + curproductoption.ProductOptionId + "] and Name:[" + curproductoption.Name + "].");
            }
            catch (DbUpdateException)
            {
                logger.Error("UpdateProductOption() - DB Error while trying to update productoption with ID:[" + curproductoption.ProductOptionId + "] and Name:[" + curproductoption.Name + "].");
                // Returning 500 as the request could not be fulfilled
                return StatusCode(HttpStatusCode.InternalServerError);
            }
            logger.Debug("UpdateProductOption() - End of method.");
            return Ok(curproductoption);
        }

        // DELETE: /v2/products/{id}/options/{optionId}
        [Route("{id:Guid}/options/{oid:Guid}")]
        public IHttpActionResult DeleteProductOption(Guid id, Guid oid)
        {
            ProductOption productoption = GetProductOptionByOptionId(id, oid);
            if (productoption == null)
            {
                logger.Warn("DeleteProductOption() - ProductOptioin with ID:[" + oid + "] not found or unable to retrieve this product.");
                return NotFound(); //StatusCode(HttpStatusCode.NotFound); //ErrorCode:404
            }
            productoption.product = GetProductByGuid(id);
            dbcontext.ProductOptions.Remove(productoption);
            try
            {
                dbcontext.SaveChanges();
                logger.Info("DeleteProductOption() - ProductOption with ID:[" + oid + "] removed successfully.");
            }
            catch (DbUpdateException)
            {
                logger.Error("DeleteProductOption() - DB error occurred while trying to delete ProductOption with ID:[" + oid + "].");
                // Returning 500 as the request could not be fulfilled
                return StatusCode(HttpStatusCode.InternalServerError);
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

    }
}
