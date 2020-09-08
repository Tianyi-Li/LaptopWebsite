using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Casestudy.Models
{
    public class UtilityModel
    {
        private AppDbContext _db;
        public UtilityModel(AppDbContext context) // will be sent by controller
        {
            _db = context;
        }


        public bool loadLaptopTables(string stringJson)
        {
            bool brandsLoaded = false;
            bool productItemsLoaded = false;
            try
            {
                dynamic objectJson = Newtonsoft.Json.JsonConvert.DeserializeObject(stringJson);
                brandsLoaded = loadBrands(objectJson);
                productItemsLoaded = loadProducts(objectJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return brandsLoaded && productItemsLoaded;
        }

        private bool loadBrands(dynamic objectJson)
        {
            bool loadBrands = false;
            try
            {
                // clear out the old rows
                _db.Brands.RemoveRange(_db.Brands);
                _db.SaveChanges();
                List<String> allBrands = new List<String>();
                foreach (var node in objectJson)
                {
                    allBrands.Add(Convert.ToString(node["BRAND"]));
                }
                // distinct will remove duplicates before we insert them into the db
                IEnumerable<String> brands = allBrands.Distinct<String>();
                foreach (string brandname in brands)
                {
                    Brand brand = new Brand();
                    brand.Name = brandname;
                    _db.Brands.Add(brand);
                    _db.SaveChanges();
                }
                loadBrands = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - " + ex.Message);
            }
            return loadBrands;
        }

        private bool loadProducts(dynamic objectJson)
        {
            bool loadedItems = false;
            try
            {
                List<Brand> Brands = _db.Brands.ToList();
                // clear out the old
                _db.ProductItems.RemoveRange(_db.ProductItems);
                _db.SaveChanges();
                foreach (var node in objectJson)
                {
                    ProductItem item = new ProductItem();
                    item.Id = Convert.ToString(node["PRODUCTID"]);
                    item.ProductName = Convert.ToString(node["PRODUCTNAME"]);
                    item.GraphicName = Convert.ToString(node["GRAPHICNAME"]);
                    item.CostPrice = Convert.ToDecimal(node["COST"].Value);
                    item.MSRP = Convert.ToDecimal(node["MSRP"].Value);
                    item.QtyOnHand = Convert.ToInt32(node["QTYONHAND"].Value);
                    item.QtyOnBackOrder = Convert.ToInt32(node["QTYONBACKORDER"].Value);
                    item.Description = Convert.ToString(node["DESCRIPTION"]);
                    string bran = Convert.ToString(node["BRAND"]);
                    // add the FK here
                    foreach (Brand brand in Brands)
                    {
                        if (brand.Name == bran)
                            item.Brand = brand;
                    }
                    _db.ProductItems.Add(item);
                    _db.SaveChanges();
                }
                loadedItems = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - " + ex.Message);
            }
            return loadedItems;
        }
    }
}
