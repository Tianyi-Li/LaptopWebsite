using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Http;
using System.Text;
using Casestudy.Models;
using Newtonsoft.Json;
using Casestudy.Utils;

namespace Casestudy.TagHelpers
{
    // You may need to install the Microsoft.AspNetCore.Razor.Runtime package into your project
    [HtmlTargetElement("catalogue", Attributes = BrandIdAttribute)]
    public class CatalogueHelper : TagHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        public CatalogueHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private const string BrandIdAttribute = "brand";
        [HtmlAttributeName(BrandIdAttribute)]
        public string BrandId { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (_session.Get<ProductItemViewModel[]>(SessionVariables.Product) != null && Convert.ToInt32(BrandId) > 0)
            {
                var innerHtml = new StringBuilder();
                ProductItemViewModel[] product = _session.Get<ProductItemViewModel[]>(SessionVariables.Product);
                innerHtml.Append("<h5>Items</h5>");
                innerHtml.Append("<div class=\"row w-100 m-1\" style=\"overflow-y:scroll;height:60vh;\">");
                foreach (ProductItemViewModel item in product)
                {
                    if (item.BrandId == Convert.ToInt32(BrandId))
                    {
                        // remove apostrophe from JSON
                        item.Description = item.Description.Contains("'") ? item.Description.Replace("'", "") : item.Description;
                        var itemJson = JsonConvert.SerializeObject(item);
                        var btn = "brandbtn" + item.Id;
                        innerHtml.Append("<div class=\"col-sm-3 col-xs-12 text-center\" style =\"border:solid;\">");
                        //innerHtml.Append("<img src =\"/images/icron.jpg\" /><br />");
                        innerHtml.Append("<img src =\"/images/"+item.GRAPHICNAME+"\""+ " style=\"height: 140px; width: 85px; \" /><br />");
                        if (item.Description.Length > 25)
                        {
                            innerHtml.Append("<span class=\"m-0\" style=\"font-size:large;font-weight:bold;\">" +
                           item.Description.Substring(0, 25) + "...</span>");
                        }
                        else
                        {
                            innerHtml.Append("<span class=\"m-0\" style=\"font-size:large;font-weight:bold;\">" +
                           item.Description + "...</span>");
                        }
                        innerHtml.Append("<p><span style=\"font-size:medium\">Products info. in details</span >");
                        innerHtml.Append("<p><a id=\"" + btn + "\" href=\"#details_popup\" data-toggle=\"modal\"");
                        innerHtml.Append(" class=\"btn btn-outline-dark pt-2 pb-2\" data-id=" + item.Id);
                        innerHtml.Append(" data-details='" + itemJson + "'>Details</a></div>");
                    }
                }
                output.Content.SetHtmlContent(innerHtml.ToString());
            }
        }
    }
}
