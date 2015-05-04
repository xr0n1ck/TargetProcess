using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace Web.BootstrapSupport
{
	public static class DefaultScaffoldingExtensions
	{
		public static string GetControllerName(this Type controllerType)
		{
			return controllerType.Name.Replace("Controller", String.Empty);
		}

		public static string GetActionName(this LambdaExpression actionExpression)
		{
			return ((MethodCallExpression) actionExpression.Body).Method.Name;
		}

		public static PropertyInfo[] VisibleProperties(this IEnumerable model)
		{
			var elementType = model.GetType().GetElementType() ?? model.GetType().GetGenericArguments()[0];
			return elementType.GetProperties().Where(info => info.Name != elementType.IdentifierPropertyName()).ToArray();
		}

		public static PropertyInfo[] VisibleProperties(this Object model)
		{
			return model.GetType().GetProperties().Where(info => info.Name != model.IdentifierPropertyName()).ToArray();
		}

		public static PropertyInfo[] CreationProperties(this Object model)
		{
			return model.GetType().GetProperties().
				Where(info => info.Name != model.IdentifierPropertyName() && !info.IsGeneratedProperty()).
				ToArray();
		}

		public static RouteValueDictionary GetIdValue(this object model)
		{
			var v = new RouteValueDictionary {{model.IdentifierPropertyName(), model.GetId()}};
			return v;
		}

		public static object GetId(this object model)
		{
			return model.GetType().GetProperty(model.IdentifierPropertyName()).GetValue(model, new object[0]);
		}


		public static string IdentifierPropertyName(this Object model)
		{
			return IdentifierPropertyName(model.GetType());
		}

		public static string IdentifierPropertyName(this Type type)
		{
			if (type.GetProperties().Any(info => info.PropertyType.AttributeExists<KeyAttribute>()))
			{
				return
					type.GetProperties().First(
						info => info.PropertyType.AttributeExists<KeyAttribute>())
						.Name;
			}
			if (type.GetProperties().Any(p => p.Name.Equals("id", StringComparison.CurrentCultureIgnoreCase)))
			{
				return
					type.GetProperties().First(
						p => p.Name.Equals("id", StringComparison.CurrentCultureIgnoreCase)).Name;
			}
			return "";
		}

		public static string GetLabel(this PropertyInfo propertyInfo)
		{
			var meta = ModelMetadataProviders.Current.GetMetadataForProperty(null, propertyInfo.DeclaringType, propertyInfo.Name);
			return meta.GetDisplayName();
		}

		public static string ToSeparatedWords(this string value)
		{
			return Regex.Replace(value, "([A-Z][a-z])", " $1").Trim();
		}

		private static bool IsGeneratedProperty(this PropertyInfo propertyInfo)
		{
			return propertyInfo.Name.Equals("CreateDate", StringComparison.CurrentCultureIgnoreCase);
		}
	}

	public static class PropertyInfoExtensions
	{
		public static bool AttributeExists<T>(this PropertyInfo propertyInfo) where T : class
		{
			var attribute = propertyInfo.GetCustomAttributes(typeof (T), false)
			                	.FirstOrDefault() as T;
			if (attribute == null)
			{
				return false;
			}
			return true;
		}

		public static bool AttributeExists<T>(this Type type) where T : class
		{
			var attribute = type.GetCustomAttributes(typeof (T), false).FirstOrDefault() as T;
			return attribute != null;
		}

		public static T GetAttribute<T>(this Type type) where T : class
		{
			return type.GetCustomAttributes(typeof (T), false).FirstOrDefault() as T;
		}

		public static T GetAttribute<T>(this PropertyInfo propertyInfo) where T : class
		{
			return propertyInfo.GetCustomAttributes(typeof (T), false).FirstOrDefault() as T;
		}

		public static string LabelFromType(Type @type)
		{
			var att = GetAttribute<DisplayNameAttribute>(@type);
			return att != null
			       	? att.DisplayName
			       	: @type.Name.ToSeparatedWords();
		}

		public static string GetLabel(this Object model)
		{
			return LabelFromType(model.GetType());
		}

		public static string GetLabel(this IEnumerable model)
		{
			var elementType = model.GetType().GetElementType() ?? model.GetType().GetGenericArguments()[0];
			return LabelFromType(elementType);
		}
	}

	public static class HtmlHelperExtensions
	{
		public static MvcHtmlString TryPartial(this HtmlHelper helper, string viewName, object model)
		{
			try
			{
				return helper.Partial(viewName, model);
			}
			catch
			{
			}
			return MvcHtmlString.Empty;
		}
	}
}