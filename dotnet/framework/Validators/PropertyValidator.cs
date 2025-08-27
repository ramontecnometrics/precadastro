using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace framework.Validators
{
    public static class PropertyValidator
    {
        public static AggregateException ValidateProperties(object obj)
        {
            var exceptions = new List<Exception>();
            var validGroups = new HashSet<string>();
            var groupsWithValue = new HashSet<string>();
            var exceptionsForAtLeastOneValidation = new Dictionary<string, Exception>();
            var exceptionsForValidateAllIfNotNull = new Dictionary<string, Exception>();
            var props = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props)
            {
                var mget = prop.GetGetMethod(false);
                if (mget != null)
                {
                    var value = prop.GetValue(obj);
                    var attributes = prop.GetCustomAttributes(false);
                    foreach (var attribute in attributes)
                    {
                        if (typeof(IGroupValidationAtribute).IsAssignableFrom(attribute.GetType()) &&
                            (((IGroupValidationAtribute)attribute).GroupId != null))
                        {
                            var validationAttribute = (IGroupValidationAtribute)attribute;
                            var hasValue = (value is string) ? !string.IsNullOrWhiteSpace((string)value) : (value != null);

                            if ((hasValue) && (!groupsWithValue.Contains(validationAttribute.GroupId)))
                            {
                                groupsWithValue.Add(validationAttribute.GroupId);
                            }

                            try
                            {
                                if (!validationAttribute.IsValid(value))
                                {
                                    throw new Exception(validationAttribute.ErrorMessage);
                                }
                                if (validationAttribute.GroupValidationType == GroupValidationType.AtLeastOneValid)
                                {
                                    if (!validGroups.Contains(validationAttribute.GroupId))
                                    {
                                        //Se uma propriedade do grupo estiver válida então removemos os erros anteriores e
                                        //marcamos o grupo como válido.
                                        exceptionsForAtLeastOneValidation = exceptionsForAtLeastOneValidation.Where(i => i.Key != validationAttribute.GroupId)
                                            .ToDictionary(i => i.Key, i => i.Value);
                                        validGroups.Add(validationAttribute.GroupId);
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                e.Data["property"] = prop;
                                if (validationAttribute.GroupValidationType == GroupValidationType.AtLeastOneValid)
                                {
                                    if ((!validGroups.Contains(validationAttribute.GroupId)) &&
                                        (!exceptionsForAtLeastOneValidation.ContainsKey(validationAttribute.GroupId)))
                                    {
                                        exceptionsForAtLeastOneValidation.Add(validationAttribute.GroupId, e);
                                    }
                                }
                                if (validationAttribute.GroupValidationType == GroupValidationType.ValidateAllIfNotNull)
                                {
                                    if (!exceptionsForValidateAllIfNotNull.ContainsKey(validationAttribute.GroupId))
                                    {
                                        exceptionsForValidateAllIfNotNull.Add(validationAttribute.GroupId, e);
                                    }
                                }
                            }
                        }
                        else
                        if (typeof(IValidationAttribute).IsAssignableFrom(attribute.GetType()))
                        {
                            var validationAttribute = (IValidationAttribute)attribute;
                            try
                            {
                                if (!validationAttribute.IsValid(value))
                                {
                                    throw new Exception(validationAttribute.ErrorMessage);
                                }
                            }
                            catch (Exception e)
                            {
                                e.Data["property"] = prop;
                                exceptions.Add(e);
                            }
                        }
                    }
                }
            }
            exceptions.AddRange(exceptionsForAtLeastOneValidation.Select(i => i.Value));
            //Considerar somente os erros onde pelo uma propriedade do grupo tem valor
            exceptions.AddRange(exceptionsForValidateAllIfNotNull
                .Where(i => groupsWithValue.Contains(i.Key))
                .Select(i => i.Value).ToArray());
            var result = default(AggregateException);
            if (exceptions.Any())
            {
                result = new AggregateException(exceptions);
            }
            return result;
        }

        private static MemberExpression GetMemberExpression<TObject, TPropertyType>(
            Expression<Func<TObject, TPropertyType>> sourceProperty)
        {
            if (!(sourceProperty.Body is MemberExpression memberExpression))
            {
                memberExpression = ((UnaryExpression)sourceProperty.Body).Operand as MemberExpression;
                if (memberExpression == null)
                {
                    throw new ArgumentException(
                        "O parâmetro informado não é um MemberExpression.");
                }
            }
            return memberExpression;
        }
    }
}
