using System.Collections.Generic;
using System.Linq;
namespace Casestudy.Models
{
    public class ProductItemModel
    {
        private AppDbContext _db;
        public ProductItemModel(AppDbContext context)
        {
            _db = context;
        }
        public List<ProductItem> GetAll()
        {
            return _db.ProductItems.ToList();
        }
        public List<ProductItem> GetAllByBrand(int id)
        {
            return _db.ProductItems.Where(item => item.Brand.Id == id).ToList();
        }
        public Brand GetBrand(int id)
        {
            return _db.Brands.FirstOrDefault(bran => bran.Id == id);
        }
    }
}
