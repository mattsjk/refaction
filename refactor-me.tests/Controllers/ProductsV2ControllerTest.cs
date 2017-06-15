using Microsoft.VisualStudio.TestTools.UnitTesting;
using refactor_me.Models;
using refactor_me.v2.Controllers;
using System;

namespace refactor_me.tests.Controllers
{
    [TestClass]
    public class ProductsV2ControllerTest
    {
        // Arrange/Define
        ProductsV2Controller controller = new ProductsV2Controller();

        [TestMethod]
        public void TestGetAllProducts()
        {
            // Act/Request
            ProductVO result = controller.GetAllProducts();

            // Assert/Inspect
            Assert.IsNotNull(result);
            foreach (Product p in result.Items)
            {
                Assert.IsNotNull(p.Name);
                Assert.IsNotNull(p.Description);
                Assert.IsNotNull(p.DeliveryPrice);
                Assert.IsNotNull(p.Price);
            }
        }

        [TestMethod]
        public void TestGetProductByName()
        {
            string productName = "Samsung S6";

            ProductVO result = controller.GetProductByName(productName);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestGetAllProductOptions()
        {
            string idstr = "739af7f6-a4a8-4f72-ab86-f5d1816940ae";
            // Act/Request
            ProductOptionVO result = controller.GetAllProductOptions(new Guid(idstr));

            // Assert/Inspect
            Assert.IsNotNull(result);
            foreach (ProductOption po in result.Items)
            {
                Assert.IsNotNull(po.Name);
                Assert.IsNotNull(po.Description);
            }
        }

        // Add more test cases here
    }
}
