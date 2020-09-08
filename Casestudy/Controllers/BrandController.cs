using Microsoft.AspNetCore.Mvc;
using Casestudy.Models;
using System.Collections.Generic;
using Casestudy.Utils;
using System;
namespace Casestudy.Controllers
{
    public class BrandController : Controller
    {
        AppDbContext _db;
        public BrandController(AppDbContext context)
        {
            _db = context;
        }
        public IActionResult Index(BrandViewModel vm)
        {
            // only build the catalogue once
            if (HttpContext.Session.Get<List<Brand>>(SessionVariables.Brands) == null)
            {
                // no session information so let's go to the database
                try
                {
                    BrandModel brandModel = new BrandModel(_db);
                    // now load the categories
                    List<Brand> brands = brandModel.GetAll();
                    HttpContext.Session.Set<List<Brand>>(SessionVariables.Brands, brands);
                    vm.SetBrands(brands);
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "Catalogue Problem - " + ex.Message;
                }
            }
            else
            {
                // no need to go back to the database as information is already in the session
                vm.SetBrands(HttpContext.Session.Get<List<Brand>>(SessionVariables.Brands));
            }
            return View(vm);
        }

        public IActionResult SelectBrand(BrandViewModel vm)
        {
            BrandModel brandModel = new BrandModel(_db);
            ProductItemModel productModel = new ProductItemModel(_db);
            List<ProductItem> items = productModel.GetAllByBrand(vm.BrandId);
            List<ProductItemViewModel> vms = new List<ProductItemViewModel>();
            if (items.Count > 0)
            {
                foreach (ProductItem item in items)
                {
                    ProductItemViewModel mvm = new ProductItemViewModel();
                    mvm.Qty = 0;
                    mvm.BrandId = item.BrandId;
                    mvm.BrandName = brandModel.GetName(item.BrandId);
                    mvm.Description = item.Description;
                    mvm.Id = item.Id;
                    mvm.PRODUCTNAME = item.ProductName;
                    mvm.GRAPHICNAME = item.GraphicName;
                    mvm.COST = item.CostPrice;
                    mvm.MSRP = item.MSRP;
                    mvm.QTYONHAND = item.QtyOnHand;
                    mvm.QTYONBACKORDER = item.QtyOnBackOrder;
                    
                    
                    
                    vms.Add(mvm);
                }
                ProductItemViewModel[] myProduct = vms.ToArray();
                HttpContext.Session.Set<ProductItemViewModel[]>(SessionVariables.Product, myProduct);
            }
            vm.SetBrands(HttpContext.Session.Get<List<Brand>>(SessionVariables.Brands));
            return View("Index", vm); // need the original Index View here
        }

        public ActionResult SelectItem(BrandViewModel vm)
        {
            Dictionary<string, object> cart;
            if (HttpContext.Session.Get<Dictionary<string, Object>>(SessionVariables.Cart) == null)
            {
                cart = new Dictionary<string, object>();
            }
            else
            {
                cart = HttpContext.Session.Get<Dictionary<string, object>>(SessionVariables.Cart);
            }
            ProductItemViewModel[] product = HttpContext.Session.Get<ProductItemViewModel[]>(SessionVariables.Product);
            String retMsg = "";
            foreach (ProductItemViewModel item in product)
            {
                if (item.Id == vm.ProductId)
                {
                    if (vm.Qty > 0) // update only selected item
                    {
                        item.Qty = vm.Qty;
                        retMsg = vm.Qty + " - item(s) Added!";
                        cart[item.Id] = item;
                       
                    }
                    else
                    {
                        item.Qty = 0;
                        cart.Remove(item.Id);
                        retMsg = "item(s) Removed!";
                    }
                    vm.BrandId = item.BrandId;
                    break;
                }
            }
            ViewBag.AddMessage = retMsg;
            HttpContext.Session.Set<Dictionary<string, Object>>(SessionVariables.Cart, cart);
            vm.SetBrands(HttpContext.Session.Get<List<Brand>>(SessionVariables.Brands));
            return View("Index", vm);
        }
    }
}
