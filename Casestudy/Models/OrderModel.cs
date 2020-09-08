using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Casestudy.Models
{
    public class OrderModel
    {
        private AppDbContext _db;
        public OrderModel(AppDbContext ctx)
        {
            _db = ctx;
        }
        private bool isBack = false;
        public int AddOrder(Dictionary<string, object> items, ApplicationUser user)
        {
            int orderId = -1;
            using (_db)
            {
                // we need a transaction as multiple entities involved
                using (var _trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        Order order = new Order();
                        order.OrderDate = System.DateTime.Now;
                        order.OrderAmount = 0;
                        order.UserId = user.Id;
                        foreach(var key in items.Keys)
                        {
                            ProductItemViewModel item =
                           JsonConvert.DeserializeObject<ProductItemViewModel>(Convert.ToString(items[key]));
                            if(item.Qty > 0)
                            {
                                order.OrderAmount += item.MSRP * item.Qty;
                            }
                        }
                        order.OrderAmount = order.OrderAmount * (decimal)1.13;
                        
                        _db.Orders.Add(order);
                        _db.SaveChanges();

                        //then change each item data in productitem table
                        foreach (var key in items.Keys)
                        {
                            ProductItemViewModel item =
                           JsonConvert.DeserializeObject<ProductItemViewModel>(Convert.ToString(items[key]));
                            if (item.Qty > 0 && item.Qty <= item.QTYONHAND)
                            {
                                ProductItem _item = _db.ProductItems.FirstOrDefault(x => x.Id == item.Id);
                                
                                _item.QtyOnHand = item.QTYONHAND - item.Qty;
                                
                                _db.ProductItems.Update(_item);
                                _db.SaveChanges();
                            }
                            else if (item.Qty > 0 && item.Qty > item.QTYONHAND)
                            {

                                isBack = true; // set this boolean variable to true
                                ProductItem _item = _db.ProductItems.FirstOrDefault(x => x.Id == item.Id);
                                _item.QtyOnHand = 0;
                                _item.QtyOnBackOrder = item.Qty - item.QTYONHAND;
                                _db.ProductItems.Update(_item);
                                _db.SaveChanges();
                            }
                        }
                           
                        // then add each item to the orderLineItem table
                        foreach (var key in items.Keys)
                        {
                            ProductItemViewModel item =
                           JsonConvert.DeserializeObject<ProductItemViewModel>(Convert.ToString(items[key]));
                            if (item.Qty > 0 && item.Qty <= item.QTYONHAND)
                            {
                                OrderLineItem oLItem = new OrderLineItem();
                                oLItem.SellingPrice = item.MSRP;
                                oLItem.ProductId = item.Id;
                                oLItem.OrderId = order.Id;
                                oLItem.QtySold = item.Qty;
                                oLItem.QtyOrdered = item.Qty;
                                oLItem.QtyBackOrdered = 0;
                                _db.OrderLineItems.Add(oLItem);
                                _db.SaveChanges();
                            }
                            else if(item.Qty > 0 && item.Qty > item.QTYONHAND)
                            {
                                OrderLineItem oLItem = new OrderLineItem();
                                oLItem.SellingPrice = item.MSRP;
                                oLItem.ProductId = item.Id;
                                oLItem.OrderId = order.Id;
                                oLItem.QtySold = item.QTYONHAND;
                                oLItem.QtyOrdered = item.Qty;
                                oLItem.QtyBackOrdered = item.Qty-item.QTYONHAND;
                                _db.OrderLineItems.Add(oLItem);
                                _db.SaveChanges();
                            }
                        }
                        

                        // test trans by uncommenting out these 3 lines
                        //int x = 1;
                        //int y = 0;
                        //x = x / y; 
                        _trans.Commit();
                        orderId = order.Id;
                    }
                    catch (Exception ex)
                    {
                        orderId = -1;
                        Console.WriteLine(ex.Message);
                        _trans.Rollback();
                    }
                }
            }
            return orderId;
        }// end AddOrder

        public List<Order> GetOrdersByUserId(string userid)
        {

            return _db.Orders.Where(order => order.UserId == userid).ToList<Order>();
        }

        public List<OrderViewModel> GetOrderDetails(int oid, string uid)
        {
            List<OrderViewModel> allDetails = new List<OrderViewModel>();
            // LINQ way of doing INNER JOINS
            var results = from o in _db.Set<Order>()
                          join oi in _db.Set<OrderLineItem>() on o.Id equals oi.OrderId
                          join pi in _db.Set<ProductItem>() on oi.ProductId equals pi.Id
                          where (o.UserId == uid && o.Id == oid)
                          select new OrderViewModel
                          {
                              OrderId = o.Id,
                              UserId = uid,
                              OrderAmount = o.OrderAmount,
                              Description = pi.Description,
                              ProductName = pi.ProductName,
                              QtyOrdered = oi.QtyOrdered,
                              QtySold = oi.QtySold,
                              QtyBackOrdered = oi.QtyBackOrdered,
                              ProductItemId = pi.Id,
                              MSRP = pi.MSRP,
                              Qty = oi.QtyOrdered,
                              OrderDate = o.OrderDate.ToString("yyyy/MM/dd - hh:mm tt")
                          };
            allDetails = results.ToList<OrderViewModel>();
            return allDetails;
        }

        public bool getIsBack()
        {
            return isBack;
        }
    }// end class
}

