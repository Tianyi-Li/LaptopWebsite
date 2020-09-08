using Microsoft.AspNetCore.Mvc;
using Casestudy.Models;
namespace Casestudy.Controllers
{
    public class ProductItemController : Controller
    {
        AppDbContext _db;
        public ProductItemController(AppDbContext context)
        {
            _db = context;
        }
        public IActionResult Index(BrandViewModel brand)
        {
            ProductItemModel model = new ProductItemModel(_db);
            ProductItemViewModel viewModel = new ProductItemViewModel();
            viewModel.ProductItems = model.GetAllByBrand(brand.Id);
            viewModel.BrandName = model.GetBrand(brand.Id).Name;
            return View(viewModel);
        }
    }
}
