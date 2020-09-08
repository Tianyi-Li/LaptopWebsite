using System.Collections.Generic;
namespace Casestudy.Models
{
    public class ProductItemViewModel
    {
        public string BrandName { get; set; }
        public int BrandId { get; set; }
        public IEnumerable<ProductItem> ProductItems { get; set; }
        public string Id { get; set; }
        public string Description { get; set; }
        public int Qty { get; set; }
        public string BRAND { get; set; }
        public string PRODUCTID { get; set; }
        public string PRODUCTNAME { get; set; }
        public string GRAPHICNAME { get; set; }
        public decimal COST { get; set; }
        public decimal MSRP { get; set; }
        public int QTYONHAND { get; set; }
        public int QTYONBACKORDER { get; set; }
    }
}
