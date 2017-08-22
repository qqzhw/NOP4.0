using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Nop.Web.Models.Directory
{ 
	public class NullException : Exception
	{
		private const string MESSAGE = "{0} is null.";

		public NullException(Expression<Func<object>> expression)
			: base(string.Format(MESSAGE, ExpressionHelper.GetExpressionText(expression)))
		{ }
	}
}
