using BWYouCore.Web.M.Attributes;
using BWYouCore.Web.M.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;
using System.Collections;
using BWYouCore.Web.MVC.ViewModels;
using BWYouCore.Web.MVC.BindingModels;
using BWYouCore.Web.MVC.Etc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Data;

namespace BWYouCore.Web.MVC.Extensions
{
    public static class CustomExtensions
    {
        public static ILog logger = LogManager.GetLogger(typeof(CustomExtensions));

        /// <summary>
        /// Deep 복사 본 만들기. DB 저장을 위해 관계 처리
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="source"></param>
        /// <param name="seen"></param>
        /// <param name="bCopykey"></param>
        /// <param name="bCascade"></param>
        /// <param name="direction"></param>
        /// <param name="bClone"></param>
        /// <returns></returns>
        public static TEntity Clone<TEntity>(this TEntity source, Dictionary<object, object> seen, bool bCopykey, bool bCascade, CascadeRelationAttribute.CascadeDirection direction, bool bClone)
            where TEntity : IKeyModel
        {
            if (source == null)
            {
                logger.Warn(string.Format("Clone Source is null return default(T): type={0}",
                                                typeof(TEntity).FullName));
                return default(TEntity);
            }
            if (seen.ContainsKey(source) == true)
            {
                return (TEntity)seen[source];
            }

            //부모자식 관계 일 때는 자식이 부모를 만들지 않고 원본을 그대로 돌려주도록 해야 함
            if (direction == CascadeRelationAttribute.CascadeDirection.Up || bClone == false)
            {
                logger.Warn(string.Format("Clone Source as it is : type={0}, id={1}",
                                                source.GetType().FullName, source.ToString()));
                seen.Add(source, source);
                return source;
            }


            TEntity clone = (TEntity)Activator.CreateInstance(source.GetType());
            seen.Add(source, clone);

            //NonCopyable 하지 않고, CascadeRelation에 속하지 않은 프로퍼티 모두 복사
            var props = source.GetType().GetProperties().Where(p => p.GetCustomAttributes(typeof(NonCopyableAttribute), true).Length == 0
                                                                    && p.GetCustomAttributes(typeof(NonCopyableAttribute)).Count() == 0
                                                                    && p.GetCustomAttributes(typeof(CascadeRelationAttribute), true).Length == 0
                                                                    && p.GetCustomAttributes(typeof(CascadeRelationAttribute)).Count() == 0);

            var keyName = source.GetKeyName();  //Key의 이름은 동적으로 받아서 사용.
            foreach (var prop in props)
            {
                if (prop.Name == keyName)
                {
                    if (bCopykey == false)
                    {
                        continue;
                    }
                }
                prop.SetValue(clone, prop.GetValue(source, null), null);
            }

            //CascadeRelation에 속한 프로퍼티는 Cascade 복사
            if (bCascade == true)
            {
                props = source.GetType().GetProperties().Where(p => (p.GetCustomAttributes(typeof(CascadeRelationAttribute), true).Length != 0
                                                                    || p.GetCustomAttributes(typeof(CascadeRelationAttribute)).Count() != 0)
                                                                    && p.GetCustomAttributes(typeof(NonCopyableAttribute), true).Length == 0
                                                                    && p.GetCustomAttributes(typeof(NonCopyableAttribute)).Count() == 0);

                foreach (var prop in props)
                {
                    if (typeof(IKeyModel).IsAssignableFrom(prop.PropertyType))
                    {
                        var t = (IKeyModel)prop.GetValue(source, null);

                        CascadeRelationAttribute attr = (CascadeRelationAttribute)prop.GetCustomAttribute(typeof(CascadeRelationAttribute));
                        prop.SetValue(clone, t == null ? null : t.Clone(seen, bCopykey, bCascade, attr.Direction, attr.Clonable), null);
                    }
                    else if (prop.PropertyType.IsGenericType && typeof(ICollection<>).IsAssignableFrom(prop.PropertyType.GetGenericTypeDefinition()))
                    {
                        var listType = typeof(List<>);
                        var constructedListType = listType.MakeGenericType(prop.PropertyType.GetGenericArguments()[0]);
                        var instance = (IList)Activator.CreateInstance(constructedListType);

                        var t = (IEnumerable)prop.GetValue(source, null);
                        if (t != null)
                        {
                            foreach (var item in t)
                            {
                                if (typeof(IKeyModel).IsAssignableFrom(item.GetType()))
                                {
                                    CascadeRelationAttribute attr = (CascadeRelationAttribute)prop.GetCustomAttribute(typeof(CascadeRelationAttribute));
                                    instance.Add(item == null ? null : ((IKeyModel)item).Clone(seen, bCopykey, bCascade, attr.Direction, attr.Clonable));
                                }
                                else
                                {
                                    logger.Warn(string.Format("Clone Property ICollection<T> T not IKeyModel type={0}, ToString={1}",
                                                                    item.GetType().FullName,
                                                                    item.GetType().ToString()));
                                }
                            }
                        }
                        prop.SetValue(clone, instance, null);
                    }
                    else
                    {
                        logger.Warn(string.Format("Clone Property not IKeyModel type={0}, ToString={1}",
                                                        prop.PropertyType.FullName,
                                                        prop.PropertyType.ToString()));
                    }
                }
            }

            return clone;
        }

        /// <summary>
        /// 관계 정보를 모두 불러와서 활성화 시키기. 지우기용.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="seen"></param>
        public static void ActivateRelation4Cascade<T>(this T source, HashSet<object> seen) where T : IDbModel
        {
            if (source == null)
            {
                logger.Warn(string.Format("ActivateRelation Source is null : type={0}",
                                                typeof(T).FullName));
                return;
            }
            if (seen.Contains(source) == true)
            {
                return;
            }
            else
            {
                seen.Add(source);
            }

            var props = source.GetType().GetProperties().Where(p => p.GetCustomAttributes(typeof(CascadeRelationAttribute), true).Length != 0
                                                                                || p.GetCustomAttributes(typeof(CascadeRelationAttribute)).Count() != 0);

            foreach (var prop in props)
            {
                CascadeRelationAttribute attr = (CascadeRelationAttribute)prop.GetCustomAttribute(typeof(CascadeRelationAttribute));
                if (attr.Direction == CascadeRelationAttribute.CascadeDirection.Down)
                {
                    if (typeof(IDbModel).IsAssignableFrom(prop.PropertyType))
                    {
                        var t = prop.GetValue(source, null);
                        ((IDbModel)t).ActivateRelation4Cascade(seen);
                    }
                    else if (typeof(ICollection<>).IsAssignableFrom(prop.PropertyType.GetGenericTypeDefinition()))
                    {
                        var t = (IEnumerable)prop.GetValue(source, null);
                        if (t != null)
                        {
                            foreach (var item in t)
                            {
                                if (typeof(IDbModel).IsAssignableFrom(item.GetType()))
                                {
                                    ((IDbModel)item).ActivateRelation4Cascade(seen);
                                }
                                else
                                {
                                    logger.Warn(string.Format("ActivateRelation Property ICollection<T> T not IDbModel type={0}, ToString={1}",
                                                                    item.GetType().FullName,
                                                                    item.GetType().ToString()));
                                }
                            }
                        }
                    }
                    else
                    {
                        logger.Warn(string.Format("ActivateRelation Property not IDbModel type={0}, ToString={1}",
                                                        prop.PropertyType.FullName,
                                                        prop.PropertyType.ToString()));
                    }
                }
            }
        }

        /// <summary>
        /// CascadeRelationAttribute 가 아닌 동일 이름의 property의 값을 소스에서 타겟으로 복사한다.
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="target"></param>
        /// <param name="source"></param>
        /// <param name="ignoreNull">null 값은 무시할 지 여부</param>
        public static void MapFrom<TTarget, TSource>(this TTarget target, TSource source, bool ignoreNull = true)
            where TSource : IDbModel
            where TTarget : IModelLoader<TSource>
        {
            if (source == null)
            {
                logger.Warn(string.Format("MapFrom Source is null : type={0}",
                                                typeof(TSource).FullName));
                return;
            }

            var srcFields = (from PropertyInfo prop in source.GetType().GetProperties().Where(p => p.GetCustomAttributes(typeof(CascadeRelationAttribute), true).Length == 0
                                                                    && p.GetCustomAttributes(typeof(CascadeRelationAttribute)).Count() == 0
                                                                    && p.CanRead == true)
                             select new
                             {
                                 Name = prop.Name,
                                 Type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType
                             }).ToList();

            var targetFields = (from PropertyInfo prop in target.GetType().GetProperties().Where(p => p.GetCustomAttributes(typeof(CascadeRelationAttribute), true).Length == 0
                                                                    && p.GetCustomAttributes(typeof(CascadeRelationAttribute)).Count() == 0
                                                                    && p.CanWrite == true)
                                select new
                                {
                                    Name = prop.Name,
                                    Type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType
                                }).ToList();

            var commonFields = srcFields.Intersect(targetFields);

            foreach (var field in commonFields)
            {
                var value = source.GetType().GetProperty(field.Name).GetValue(source, null);
                if (ignoreNull == false || value != null)
                {
                    target.GetType().GetProperty(field.Name).SetValue(target, value, null); 
                }
            }

        }
        /// <summary>
        /// 동일 이름의 property의 값을 소스에서 타겟으로 복사한다.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="ignoreNull">null 값은 무시할 지 여부</param>
        public static void MapFromBindingModelToBaseModel<TSource, TTarget>(this TSource source, TTarget target, bool ignoreNull = true)
            where TSource : IBindingModel<TTarget>
            where TTarget : IDbModel
        {
            if (source == null)
            {
                logger.Warn(string.Format("MapFromBindingModelToBaseModel Source is null : type={0}",
                                                typeof(TSource).FullName));
                return;
            }

            var srcFields = (from PropertyInfo prop in source.GetType().GetProperties().Where(p => p.CanRead == true)
                             select new
                             {
                                 Name = prop.Name,
                                 Type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType
                             }).ToList();

            var targetFields = (from PropertyInfo prop in target.GetType().GetProperties().Where(p => p.CanWrite == true)
                                select new
                                {
                                    Name = prop.Name,
                                    Type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType
                                }).ToList();

            var commonFields = srcFields.Intersect(targetFields);

            foreach (var field in commonFields)
            {
                var value = source.GetType().GetProperty(field.Name).GetValue(source, null);
                if (ignoreNull == false || value != null)
                {
                    target.GetType().GetProperty(field.Name).SetValue(target, value, null); 
                }
            }

        }
        /// <summary>
        /// 동적으로 Where 조건 만들기.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="ignoreNull">null 값은 무시할 지 여부</param>
        /// <param name="filterableAttributeRequired">FilterableAttribute인 것만 검색 할지 여부</param>
        /// <param name="necessaryAddFields">꼭 추가 되어야 하는 조건 필드명들. FilterableAttribute 상관 없이 추가 됨</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetWhereClause<T>(this T source, bool ignoreNull = true, bool filterableAttributeRequired = true, IEnumerable<string> necessaryAddFields = null) where T : IDbModel
        {
            List<ExpressionFilter> filters = new List<ExpressionFilter>();
            var props = source.GetType().GetProperties().Where(p =>
            {
                if (necessaryAddFields != null && necessaryAddFields.Contains(p.Name))
                {
                    return true;
                }
                var attr = p.GetCustomAttributes(typeof(FilterableAttribute), true).FirstOrDefault();
                if (attr == null)
                {
                    if (filterableAttributeRequired == true)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                if (((FilterableAttribute)attr).IsFilterable == true)
                {
                    return true;
                }
                return false;
            }
                                                            );
            foreach (var prop in props)
            {
                var value = prop.GetValue(source);
                if (ignoreNull == false || value != null)
                {
                    //Type t = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                    //Convert.ChangeType(value, prop.PropertyType);
                    filters.Add(new ExpressionFilter() { PropertyName = prop.Name, Operation = Op.Equals, Value = value });
                }
            }
            if (filters.Count <= 0)
            {
                throw new Exception("Not Exist Filter");
            }
            return ExpressionBuilder.GetExpression<T>(filters);
            //var deleg = ExpressionBuilder.GetExpression<TEntity>(filter).Compile();   //Compile 하면 DB쪽으로 검색 안 하는 듯 함.. 뭐여.. ~_~;
            //return deleg;
        }
        /// <summary>
        /// 동적으로 Where 조건 만들기.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="filters">조건 필터들. T의 prop명으로 된 필터들이어야 함</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetWhereClause<T>(this T source, IList<ExpressionFilter> filters) where T : IDbModel
        {
            if (filters.Count <= 0)
            {
                throw new Exception("Not Exist Filter");
            }
            return ExpressionBuilder.GetExpression<T>(filters);
            //var deleg = ExpressionBuilder.GetExpression<TEntity>(filter).Compile();   //Compile 하면 DB쪽으로 검색 안 하는 듯 함.. 뭐여.. ~_~;
            //return deleg;
        }

        public static void AddModelErrorFromModelValidResult(this ModelStateDictionary mvcModelStateDictionary, List<ValidationObjectError> modelValidResults)
        {
            foreach (var item in modelValidResults)
            {
                mvcModelStateDictionary.AddModelError(item.ObjectName, item.ErrorMessage);
            }
        }

        public static bool IsSubclassOfRawGeneric(this Object source, Type generic)
        {
            Type toCheck = source.GetType();
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }

        public static async Task ConvertVMAsync<TEntity, TVM>(this TEntity baseModel, TVM vm, string sort = "Id", int depth = 0)
        where TEntity : IDbModel
        where TVM : IModelLoader<TEntity>, new()
        {
            vm = new TVM();
            await vm.LoadModelAsync(baseModel, 0, depth, sort);
        }

        public static Task ConvertVMAsync<TEntity, TVM>(this IEnumerable<TEntity> baseModels, List<TVM> vms, string sort = "Id", int depth = 0)
        where TEntity : IDbModel
        where TVM : IModelLoader<TEntity>, new()
        {
            return Task.Run(async () =>
            {
                vms = new List<TVM>();
                foreach (var baseModel in baseModels)
                {
                    TVM model = new TVM();
                    await model.LoadModelAsync(baseModel, 0, depth, sort);
                    vms.Add(model);
                }
                return vms;
            });
        }

        public static DbCommand LoadSqlQuery(this DbContext context, string sql)
        {
            var cmd = context.Database.GetDbConnection().CreateCommand();
            cmd.CommandText = sql;
            cmd.CommandType = System.Data.CommandType.Text;
            return cmd;
        }

        public static DbCommand WithSqlParam(this DbCommand cmd, string paramName, object paramValue)
        {
            if (string.IsNullOrEmpty(cmd.CommandText))
                throw new InvalidOperationException("Call LoadStoredProc before using this method");

            var param = cmd.CreateParameter();
            param.ParameterName = paramName;
            param.Value = paramValue;
            cmd.Parameters.Add(param);
            return cmd;
        }

        private static IList<T> MapToList<T>(this DbDataReader dr)
        {
            var objList = new List<T>();
            var props = typeof(T).GetRuntimeProperties();

            var colMapping = dr.GetColumnSchema()
                .Where(x => props.Any(y => y.Name.ToLower() == x.ColumnName.ToLower()))
                .ToDictionary(key => key.ColumnName.ToLower());

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    T obj = Activator.CreateInstance<T>();
                    foreach (var prop in props)
                    {
                        var val = dr.GetValue(colMapping[prop.Name.ToLower()].ColumnOrdinal.Value);
                        prop.SetValue(obj, val == DBNull.Value ? null : val);
                    }
                    objList.Add(obj);
                }
            }
            return objList;
        }

        public static IList<T> ExecuteSqlQuery<T>(this DbCommand command)
        {
            using (command)
            {
                if (command.Connection.State == System.Data.ConnectionState.Closed)
                    command.Connection.Open();
                try
                {
                    using (var reader = command.ExecuteReader())
                    {
                        return reader.MapToList<T>();
                    }
                }
                finally
                {
                    command.Connection.Close();
                }
            }
        }
    }
}
