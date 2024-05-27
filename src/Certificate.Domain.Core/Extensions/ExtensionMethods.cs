using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Certificate.Domain.Core.Extensions
{
    public static class ExtensionMethods
    {
        #region Public Methods

        /// <summary>
        /// Adds the elements of the given collection to the end of this list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="items"></param>
        public static void AddRange<T>(this ICollection<T> source, IEnumerable<T> items)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(items);

            List<T>? _list = source as List<T>;

            if (_list is not null)
                _list.AddRange(items);
            else
                foreach (T? item in items)
                    source.Add(item);
        }

        /// <summary>
        /// Check if a value is present in a list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool IsInList<T>(this T value, IEnumerable<T> list) => list.Contains(value);

        /// <summary>
        /// Check if a value is not present in a list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool IsNotInList<T>(this T value, IEnumerable<T> list) => !list.Contains(value);

        /// <summary>
        /// Check if whether the specified constant name exists within the public constant values of the specified type.
        /// </summary>
        /// <typeparam name="T">The type containing the public constant values.</typeparam>
        /// <param name="type">The instance of the type.</param>
        /// <param name="constantName">The name of the constant to check for.</param>
        /// <returns><see langword="true"/> if the specified constant name exists within the public constant values of the specified type; otherwise, <see langword="false"/>.</returns>
        public static bool ContainsConstant<T>(this T type, string constantName)
        {
            List<string>? _constants = Activator.CreateInstance(typeof(T))?
                                                .GetType()
                                                .GetAllNonNullPublicConstantValues<string>();

            return _constants is not null && _constants.Contains(constantName);
        }

        /// <summary>
        /// Retrieve all non-null public constant values of a specified type.
        /// </summary>
        /// <typeparam name="T">The type of the constant values to retrieve.</typeparam>
        /// <param name="type">The <see cref="Type"/> object representing the type from which to retrieve constant values.</param>
        /// <returns>A <see cref="List{T}"/> containing all non-null public constant values of the specified type.</returns>
        public static List<T> GetAllNonNullPublicConstantValues<T>(this Type type) => type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                                                                                          .Where(fieldInfo => fieldInfo.IsLiteral
                                                                                                           && !fieldInfo.IsInitOnly
                                                                                                           && fieldInfo.FieldType == typeof(T))
                                                                                          .Select(fieldInfo => (T?)fieldInfo.GetRawConstantValue())
                                                                                          .Where(value => value is not null)
                                                                                          .Select(value => value!)
                                                                                          .ToList();

        /// <summary>
        /// Get the implementation of a service from the service provider.
        /// </summary>
        /// <typeparam name="TService">The type of service to retrieve.</typeparam>
        /// <typeparam name="TImplementation">The implementation type of the service.</typeparam>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>The implementation of the specified service, or <see langword="null"/> if no implementation is found.</returns>
        /// <exception cref="NotImplementedException">Thrown when no implementation is found for the specified service type.</exception>
        public static TService GetImplementation<TService, TImplementation>(this IServiceProvider serviceProvider)
            where TImplementation : TService
        {
            IEnumerable<TService> _implementations = serviceProvider.GetServices<TService>();

            return _implementations.FirstOrDefault(tService => tService?.GetType() == typeof(TImplementation))
                    ?? throw new NotImplementedException($"No implementation found for type {typeof(TImplementation).Name}");
        }

        /// <summary>
        /// Combine two lambda expressions using the logical AND operator.
        /// </summary>
        /// <typeparam name="T">The type of the parameter in the lambda expressions.</typeparam>
        /// <param name="expressionA">The first lambda expression.</param>
        /// <param name="expressionB">The second lambda expression.</param>
        /// <returns>A new <see cref="Expression"/>&lt;<see langword="Func"/>&lt;<typeparamref name="T"/>, <see cref="bool"/>&gt;&gt; representing the logical AND operation of the input expressions.</returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expressionA, Expression<Func<T, bool>> expressionB)
        {
            ParameterExpression _parameterExpression = expressionA.Parameters[0];

            SubstituteExpressionVisitor _substituteExpressionVisitor = new()
            {
                Substitute = { [expressionB.Parameters[0]] = _parameterExpression }
            };

            Expression _expressionBody = Expression.AndAlso(left: expressionA.Body,
                                                            right: _substituteExpressionVisitor.Visit(expressionB.Body));

            return Expression.Lambda<Func<T, bool>>(_expressionBody, _parameterExpression);
        }

        /// <summary>
        /// Combine two lambda expressions using the logical OR operator.
        /// </summary>
        /// <typeparam name="T">The type of the parameter in the lambda expressions.</typeparam>
        /// <param name="expressionA">The first lambda expression.</param>
        /// <param name="expressionB">The second lambda expression.</param>
        /// <returns>A new <see cref="Expression"/>&lt;<see langword="Func"/>&lt;<typeparamref name="T"/>, <see cref="bool"/>&gt;&gt; representing the logical OR operation of the input expressions.</returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expressionA, Expression<Func<T, bool>> expressionB)
        {
            ParameterExpression _parameterExpression = expressionA.Parameters[0];

            SubstituteExpressionVisitor _substituteExpressionVisitor = new()
            {
                Substitute = { [expressionB.Parameters[0]] = _parameterExpression }
            };

            Expression _expressionBody = Expression.OrElse(left: expressionA.Body,
                                                           right: _substituteExpressionVisitor.Visit(expressionB.Body));

            return Expression.Lambda<Func<T, bool>>(_expressionBody, _parameterExpression);
        }

        /// <summary>
        /// Check if whether the specified <see cref="Guid"/> instance is equal to <see cref="Guid.Empty"/>.
        /// </summary>
        /// <param name="guid">The <see cref="Guid"/> instance to check for emptiness.</param>
        /// <returns><see langword="true"/> if the specified <see cref="Guid"/> instance is equal to <see cref="Guid.Empty"/>; otherwise, <see langword="false"/>.</returns>
        public static bool IsEmpty(this Guid guid) => guid == Guid.Empty;

        /// <summary>
        /// Deserialize a JSON <see cref="string"/> into an object of the specified type.
        /// </summary>
        /// <typeparam name="TType">The type of object to deserialize into.</typeparam>
        /// <param name="serializedObject">The JSON string to deserialize.</param>
        /// <returns>The deserialized object of the specified type, or <see langword="null"/> if the deserialization fails.</returns>
        public static TType? Deserialize<TType>(string serializedObject) => JsonConvert.DeserializeObject<TType>(serializedObject);

        #endregion Public Methods
    }
}
