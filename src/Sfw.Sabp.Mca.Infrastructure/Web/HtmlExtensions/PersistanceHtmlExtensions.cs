using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Sfw.Sabp.Mca.Infrastructure.Web.HtmlExtensions
{
    public static class PersistanceHtmlExtensions
    {
        public static MvcHtmlString HiddenCollectionOf<TModel, TProperty>(this HtmlHelper htmlHelper, TModel model,
            Expression<Func<TModel, IEnumerable<TProperty>>> collectionExpression)
        {
            var collection = collectionExpression.Compile().Invoke(model);
            var enumerable = collection as IList<TProperty> ?? collection.ToList();

            if (!enumerable.Any()) return null;

            var collectionPropertyName = ((MemberExpression)collectionExpression.Body).Member.Name;

            var memberExpression = ((MemberExpression)collectionExpression.Body).Expression.ToString();

            if (NestedCollection(memberExpression))
            {
                collectionPropertyName = CollectionPropertyName(collectionPropertyName, memberExpression);
            }

            var properties = TypeDescriptor.GetProperties(typeof(TProperty));

            var hiddenHtmlCollection = new List<string>();
            var index = 0;
            foreach (var item in enumerable)
            {
                foreach (var property in properties)
                {
                    var propertyValue = ((PropertyDescriptor)property).GetValue(item);
                    var propertyName = ((PropertyDescriptor)property).Name;

                    hiddenHtmlCollection.Add(htmlHelper.Hidden(
                        FormatCollectionName(collectionPropertyName, index, propertyName),
                        propertyValue,
                        new { id = FormatCollectionName(collectionPropertyName, index, propertyName) })
                        .ToHtmlString());
                }

                index++;
            }

            return MvcHtmlString.Create(hiddenHtmlCollection.Aggregate((current, next) => current + next));
        }

        #region private

        private static string CollectionPropertyName(string collectionPropertyName, string memberExpression)
        {
            return string.Format("{0}.{1}", memberExpression.Substring(memberExpression.IndexOf('.'), (memberExpression.Length - 1)).TrimStart('.'), collectionPropertyName);
        }

        private static bool NestedCollection(string memberExpression)
        {
            return memberExpression.Contains('.');
        }

        private static string FormatCollectionName(string collectionPropertyName, int index, string propertyName)
        {
            return string.Format("{0}[{1}].{2}", collectionPropertyName, index, propertyName);
        }
        #endregion
    }
}
