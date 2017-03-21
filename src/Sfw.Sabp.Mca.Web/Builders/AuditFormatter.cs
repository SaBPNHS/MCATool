using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Sfw.Sabp.Mca.Web.Builders
{
    public class AuditFormatter : IAuditFormatter
    {
        public string AuditValues(IValueProvider valueProvider, string auditProperties)
        {
            var values = string.Empty;

            var auditPropertiyItems = GetAuditProperties(auditProperties);

            foreach (var auditProperty in auditPropertiyItems)
            {
                var value = valueProvider.GetValue(auditProperty).AttemptedValue;
                var oldProperty = "Current" + auditProperty;
                var oldValue = GetOldValue(valueProvider, oldProperty);

                if (!string.IsNullOrEmpty(oldValue))
                {
                    values += string.Format("{0}[{1}:{2}],", auditProperty, value, oldValue);
                }
                else
                {
                    values += string.Format("{0}[{1}],", auditProperty, value);
                }                
            }

            if (values.Length > 0) values = values.TrimEnd(new[] { ',' });

            return values;
        }

        #region private

        private IEnumerable<string> GetAuditProperties(string auditProperties)
        {
            return auditProperties.Split(new[] { ',' }).ToList();
        }

        private string GetOldValue(IValueProvider valueProvider, string oldProperty)
        {
            var oldValue = string.Empty;

            try
            {
                oldValue = valueProvider.GetValue(oldProperty).AttemptedValue;
            }
            catch (NullReferenceException)
            {
            }

            return oldValue;
        }

        #endregion
    }
}