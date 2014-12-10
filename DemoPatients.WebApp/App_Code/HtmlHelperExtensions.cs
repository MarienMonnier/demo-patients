using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace DemoPatients.WebApp
{
    public static class HtmlHelperExtensions
    {
        public static IHtmlString EnumDropDownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);

            var values = Enum.GetValues(metadata.Model.GetType()).Cast<Enum>();
            var items = values.Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = v.ToString(),
                Selected = v.Equals(metadata.Model)
            });

            TagBuilder builder = new TagBuilder("div");
            builder.MergeAttribute("class", "form-group");
            builder.InnerHtml = html.LabelFor(expression).ToString();
            builder.InnerHtml += html.DropDownListFor(expression, items, new { @class = "form-control" });
            return MvcHtmlString.Create(builder.ToString());
        }
    }
}