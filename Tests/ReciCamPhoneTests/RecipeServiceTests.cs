using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using SnapBook.Windows.Phone.Services;

namespace SnapBookPhoneTests
{
    [TestClass]
    public class RecipeServiceTests
    {
        private RecipeService recipeService;

        [TestInitialize]
        public void TestInitialize()
        {
            recipeService = RecipeService.Instance;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            recipeService = null;
        }

        [TestMethod]
        public void RecipeServiceTestsUnitTest()
        {
            Assert.IsTrue(true);
        } 
    }
}