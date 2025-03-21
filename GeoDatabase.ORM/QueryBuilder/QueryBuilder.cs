﻿using GeoDatabase.ORM.Mapper.Mappings;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace GeoDatabase.ORM.QueryBuilder
{
    internal class QueryBuilder
    {
        private readonly MappingConfig _config;

        public QueryBuilder(MappingConfig config)
        {
            _config = config;
        }

        internal string Compile<T>(Expression<Func<T, bool>> expression)
        {
            if (expression == null)
            {
                return "1 = 1";
            }
            var whereVisitor = new WhereVisitor<T>(_config, expression.Parameters.First());
            var res = (ConstantExpression)whereVisitor.Visit(expression.Body);

            return (string)res.Value;
        }
    }

    internal class WhereVisitor<T> : ExpressionVisitor
    {
        private readonly MappingConfig _config;
        private readonly ParameterExpression _parameter;
        
        private static readonly MethodInfo _stringIsNullOrEmptyMethod =
            typeof(string).GetMethod("IsNullOrEmpty", BindingFlags.Public | BindingFlags.Static);

        private static readonly MethodInfo _enumerableContainsMethod =
            typeof(Enumerable).GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(m => m.Name == "Contains" && m.GetParameters().Length == 2);
        private static readonly MethodInfo _objectsEquals = typeof(object).GetMethod("Equals", BindingFlags.Instance | BindingFlags.Public);

        private static readonly Dictionary<Type, MethodInfo> _listContainsMethod = new Dictionary<Type, MethodInfo>();
        private static readonly Dictionary<Type, MethodInfo> _hashSetContainsMethod = new Dictionary<Type, MethodInfo>();

        public WhereVisitor(MappingConfig config, ParameterExpression parameter)
        {
            _config = config;
            _parameter = parameter;
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            var binaryOperator = getBinaryOperator(node.NodeType);

            var left = convertExpression(node.Left);
            var right = convertExpression(node.Right);

            return Expression.Constant($"({left} {binaryOperator} {right})");
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (node.NodeType == ExpressionType.Not)
            {
                return Expression.Constant($"NOT {convertExpression(node.Operand)}");
            }
            if (node.NodeType == ExpressionType.Convert)
            {
                return Expression.Constant(convertExpression(node.Operand));
            }

            throw new NotImplementedException();
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method == _stringIsNullOrEmptyMethod)
            {
                var argument = node.Arguments[0];
                var convertedArgument = convertExpression(argument);

                // TODO: null check
                //return Expression.Constant($"({convertedArgument} IS NULL OR {convertedArgument} = \"\")");
                return Expression.Constant($"{convertedArgument}=\"\"");
            }
            if (node.Method.IsGenericMethod && node.Method.GetGenericMethodDefinition() == _enumerableContainsMethod)
            {
                var collectionExpression = node.Arguments[0];
                if (collectionExpression is MemberExpression memberExpression)
                {
                    var member = memberExpression.Member;

                    var collectionField = member.DeclaringType.GetField(member.Name,
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                    var instance = ((ConstantExpression)memberExpression.Expression).Value;

                    var collection = (IEnumerable)collectionField.GetValue(instance);

                    var value = node.Arguments[1];

                    return createInCollectionExpression(collection, value);
                }
            }

            if (node.Method.DeclaringType.IsGenericType)
            {
                var genericArgument = node.Method.DeclaringType.GetGenericArguments()[0];
                if (getListContainsMethod(genericArgument) == node.Method || 
                    getHashSetContainsMethod(genericArgument) == node.Method)
                {
                    var value = node.Arguments[0];

                    if (node.Object is MemberExpression memberExpression)
                    {
                        var member = memberExpression.Member;

                        var collectionField = member.DeclaringType.GetField(member.Name,
                            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                        var instance = ((ConstantExpression)memberExpression.Expression).Value;

                        var collection = (IEnumerable)collectionField.GetValue(instance);
                        return createInCollectionExpression(collection, value);
                    }
                }
            }

            if (node.Method == _objectsEquals)
            {
                var argument = node.Arguments[0];
                var convertedArgument = convertExpression(argument);

                var convertedCaller = convertExpression(node.Object);

                return Expression.Constant($"{convertedArgument}={convertedCaller}");
            }

            throw new NotImplementedException();
        }

        private Expression createInCollectionExpression(IEnumerable collection, Expression value)
        {
            var collectionCount = collection.Cast<object>().Count();
            if (collectionCount == 0)
            {
                return Expression.Constant("1 = 0");
            }

            var column = convertConstantValue(convertExpression(value));
            return Expression.Constant(string.Join(" or ", collection.Cast<object>().Select(s => $"{column} = {convertConstantValue(s)}")));
        }

        private MethodInfo getListContainsMethod(Type type)
        {
            if (!_listContainsMethod.TryGetValue(type, out var containsMethod))
            {
                containsMethod = typeof(List<>).MakeGenericType(type)
                    .GetMethod("Contains", BindingFlags.Instance | BindingFlags.Public);
                _listContainsMethod.Add(type, containsMethod);
            }

            return containsMethod;
        }

        private MethodInfo getHashSetContainsMethod(Type type)
        {
            if (!_hashSetContainsMethod.TryGetValue(type, out var containsMethod))
            {
                containsMethod = typeof(List<>).MakeGenericType(type)
                    .GetMethod("Contains", BindingFlags.Instance | BindingFlags.Public);
                _hashSetContainsMethod.Add(type, containsMethod);
            }

            return containsMethod;
        }

        private object convertExpression(Expression expression)
        {
            switch (expression)
            {
                case ConstantExpression constantExpression:
                    return convertConstantValue(constantExpression.Value);
                case MemberExpression memberExpression:
                    if (memberExpression.Expression is ParameterExpression parameter && parameter == _parameter &&
                        _config.ColumnNames.TryGetValue(memberExpression.Member.Name, out var columnName))
                    {
                        return $"[{columnName}]";
                    }
                    var convertedExpression = convertExpression(memberExpression.Expression);

                    if (memberExpression.Member is PropertyInfo propertyInfo)
                    {
                        return propertyInfo.GetValue(convertedExpression);
                    }
                    else if (memberExpression.Member is FieldInfo fieldInfo)
                    {
                        return fieldInfo.GetValue(convertedExpression);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                case UnaryExpression unaryExpression:
                    return ((ConstantExpression)VisitUnary(unaryExpression)).Value;
                case BinaryExpression binaryExpression:
                    return ((ConstantExpression)VisitBinary(binaryExpression)).Value;
                case MethodCallExpression methodCallExpression:
                    return ((ConstantExpression)VisitMethodCall(methodCallExpression)).Value;
                default:
                    throw new NotImplementedException();
            }
        }

        private static object convertConstantValue(object value)
        {
            return value is string str
                ? str.StartsWith("[") && str.EndsWith("]") ? str : $"\"{value}\""
                : value is bool boolValue
                    ? boolValue.ToString().ToUpperInvariant()
                    : value;
        }

        private static string getBinaryOperator(ExpressionType type)
        {
            switch (type)
            {
                case ExpressionType.Equal:
                    return "=";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.NotEqual:
                    return ">=";
                case ExpressionType.AndAlso:
                    return "and";
                case ExpressionType.OrElse:
                    return "or";
                default:
                    throw new InvalidEnumArgumentException();
            }
        } 
    }
}
