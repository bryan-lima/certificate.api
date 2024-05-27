using System.Collections.Generic;
using System.Linq.Expressions;

namespace Certificate.Domain.Core.Extensions
{
    internal class SubstituteExpressionVisitor : ExpressionVisitor
    {
        #region Public Fields

        public Dictionary<Expression, Expression> Substitute = new();

        #endregion Public Fields

        #region Protected Methods

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return Substitute.TryGetValue(node, out Expression? _newValue)
                       ? _newValue!
                       : node;
        }

        #endregion Protected Methods
    }
}
