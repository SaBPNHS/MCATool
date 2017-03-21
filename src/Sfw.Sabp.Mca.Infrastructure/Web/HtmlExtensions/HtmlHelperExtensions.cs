using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;

namespace Sfw.Sabp.Mca.Infrastructure.Web.HtmlExtensions
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString UncheckedCheckBoxFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> property, ModelStateDictionary modelStateDictionary)
        {
            var member = property.Body as MemberExpression;
            var propInfo = member.Member as PropertyInfo;

            string html = string.Format(@"<input type=""checkbox"" name=""{0}"" id=""{0}"" value=""true"">", propInfo.Name);
            html += string.Format(@"<input type=""hidden"" name=""{0}"" id=""{0}_hidden"" value=""false"">", propInfo.Name);

            return new MvcHtmlString(html);
        }
    }
}
