﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using System.Linq.Expressions;
using System.Reflection;
using EasyMongo.Expressions;

namespace EasyMongo
{
    internal class BinaryPredicate : IPropertyPredicate
    {
        public BinaryPredicate(BinaryExpression expr)
        {
            this.Property = GetProperty(expr.Left);
            this.Constant = expr.Right.Eval();
            this.OpType = GetSupportedOpType(expr.NodeType);
        }

        private static PropertyInfo GetProperty(Expression expr)
        {
            var memberExpr = expr as MemberExpression;
            if (memberExpr == null) throw new ArgumentException(expr + " is not a property.");

            var property = memberExpr.Member as PropertyInfo;
            if (property == null) throw new ArgumentException(expr + " is not a property.");

            return property;
        }

        private static ExpressionType GetSupportedOpType(ExpressionType type)
        {
            switch (type)
            {
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.LessThan:
                    return type;
                default:
                    throw new NotSupportedException(type + "is not supported");
            }
        }

        public PropertyInfo Property { get; private set; }
        public object Constant { get; private set; }
        public ExpressionType OpType { get; private set; }
        
        public void Fill(PropertyMapper mapper, Document doc)
        {
            switch (this.OpType)
            {
                case ExpressionType.Equal:
                    mapper.FillEqualPredicate(doc, this.Constant);
                    break;
                case ExpressionType.GreaterThan:
                    mapper.FillGreaterThanPredicate(doc, this.Constant);
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    mapper.FillGreaterThanOrEqualPredicate(doc, this.Constant);
                    break;
                case ExpressionType.LessThan:
                    mapper.FillLessThanPredicate(doc, this.Constant);
                    break;
                case ExpressionType.LessThanOrEqual:
                    mapper.FillLessThanOrEqualPredicate(doc, this.Constant);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}