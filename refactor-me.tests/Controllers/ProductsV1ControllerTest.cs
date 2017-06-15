using Microsoft.VisualStudio.TestTools.UnitTesting;
using refactor_me.Models;
using refactor_me.v1.Controllers;

namespace refactor_me.tests.Controllers
{
    [TestClass]
    public class ProductsV1ControllerTest
    {
        // Arrange/Define
        ProductsV1Controller controller = new ProductsV1Controller();

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

        // Add more test cases here
    }
}
