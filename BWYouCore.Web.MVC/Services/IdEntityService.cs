using BWYouCore.Web.M.Attributes;
using BWYouCore.Web.M.Models;
using BWYouCore.Web.MVC.DAOs;
using BWYouCore.Web.MVC.Etc;
using BWYouCore.Web.MVC.Extensions;
using LinqKit;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace BWYouCore.Web.MVC.Services
{
    /// <summary>
    /// IEntityService Implementation for TId
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public class IdEntityService<TEntity, TId> : IEntityService<TEntity, TId>
        where TEntity : IdModel<TId>
    {
        //TODO 각 라이브러의 테스트 코드 필요.
        /// <summary>
        /// IUnitOfWork
        /// </summary>
        protected IUnitOfWork _unitOfWork;
        /// <summary>
        /// IRepository
        /// </summary>
        protected IRepository<TEntity, TId> _repo;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbContext"></param>
        public IdEntityService(DbContext dbContext)
            : this(new DbContextUnitOfWork<DbContext>(dbContext))
        {

        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public IdEntityService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
            this._repo = unitOfWork.GetRepository<TEntity, TId>();
        }
        /// <summary>
        /// Get all filtered lists
        /// </summary>
        /// <param name="model">Search for the same item with a value</param>
        /// <returns></returns>
        public virtual Task<IEnumerable<TEntity>> GetFilteredListAsync(TEntity model)
        {
            var predicate = model.GetWhereClause();
            return GetFilteredListAsync(predicate);
        }
        /// <summary>
        /// Get all sorted filtered lists
        /// </summary>
        /// <param name="model">Search for the same item with a value</param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public virtual Task<IEnumerable<TEntity>> GetFilteredListAsync(TEntity model, string sort)
        {
            var predicate = model.GetWhereClause();
            return GetFilteredListAsync(predicate, sort);
        }
        /// <summary>
        /// Get sorted filtered paged lists
        /// </summary>
        /// <param name="model">Search for the same item with a value</param>
        /// <param name="sort"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public virtual Task<IPagedList<TEntity>> GetFilteredListAsync(TEntity model, string sort, int pageNumber, int pageSize)
        {
            var predicate = model.GetWhereClause();
            return GetFilteredListAsync(predicate, sort, pageNumber, pageSize);
        }
        /// <summary>
        /// Get sorted filtered paged lists (for infinite scroll)
        /// </summary>
        /// <param name="model">Search for the same item with a value</param>
        /// <param name="sort"></param>
        /// <param name="limitBaseColName">after, before column name</param>
        /// <param name="after"></param>
        /// <param name="before"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public virtual Task<IEnumerable<TEntity>> GetFilteredListAsync(TEntity model, string sort, string limitBaseColName, TId after, TId before, int limit)
        {
            var predicate = model.GetWhereClause();
            return GetFilteredListAsync(predicate, sort, limitBaseColName, after, before, limit);
        }
        /// <summary>
        /// Get all filtered lists
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<TEntity>> GetFilteredListAsync(Expression<Func<TEntity, bool>> filter)
        {
            return await this._repo.Query.AsExpandable().Where(filter).ToListAsync();
        }
        /// <summary>
        /// Get all sorted filtered lists
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<TEntity>> GetFilteredListAsync(Expression<Func<TEntity, bool>> filter, string sort)
        {
            return await this._repo.Query.AsExpandable().Where(filter).SortBy(sort).ToListAsync();
        }
        /// <summary>
        /// Get sorted filtered paged lists
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public virtual async Task<IPagedList<TEntity>> GetFilteredListAsync(Expression<Func<TEntity, bool>> filter, string sort, int pageNumber, int pageSize)
        {
            return await Task<IPagedList<TEntity>>.Run(() =>
            {
                IOrderedQueryable<TEntity> orderedQueryable = this._repo.Query.AsExpandable().Where(filter).SortBy(sort);
                return orderedQueryable.ToPagedList(pageNumber, pageSize);
            });
        }
        /// <summary>
        /// Get all sorted filtered lists (for infinite scroll)
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <param name="limitBaseColName">after, before column name</param>
        /// <param name="after"></param>
        /// <param name="before"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<TEntity>> GetFilteredListAsync(Expression<Func<TEntity, bool>> filter, string sort, string limitBaseColName, TId after, TId before, int limit)
        {
            List<ExpressionFilter> filters = new List<ExpressionFilter>();
            if (after != null)
            {
                filters.Add(new ExpressionFilter(limitBaseColName, Op.GreaterThan, after));
            }
            if (before != null)
            {
                filters.Add(new ExpressionFilter(limitBaseColName, Op.LessThanOrEqual, before));
            }

            var q = this._repo.Query.AsExpandable().Where(filter);
            if (filters.Count > 0)
            {
                var expr = ExpressionBuilder.GetExpression<TEntity>(filters);
                q = q.Where(expr);
            }
            return await q.SortBy(sort).Take(limit).ToListAsync();
        }
        /// <summary>
        /// Expose query objects
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<TEntity> Query()
        {
            return this._repo.Query;
        }
        /// <summary>
        /// Expose filtered query objects
        /// </summary>
        /// <param name="model">Search for the same item with a value</param>
        /// <returns></returns>
        public virtual IQueryable<TEntity> GetFilteredQuery(TEntity model)
        {
            var predicate = model.GetWhereClause();
            return GetFilteredQuery(predicate);
        }
        /// <summary>
        /// Expose sorted filtered query objects
        /// </summary>
        /// <param name="model">Search for the same item with a value</param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public virtual IOrderedQueryable<TEntity> GetFilteredQuery(TEntity model, string sort)
        {
            var predicate = model.GetWhereClause();
            return GetFilteredQuery(predicate, sort);
        }
        /// <summary>
        /// Expose filtered query objects
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public virtual IQueryable<TEntity> GetFilteredQuery(Expression<Func<TEntity, bool>> filter)
        {
            return this._repo.Query.AsExpandable().Where(filter);
        }
        /// <summary>
        /// Expose sorted filtered query objects
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public virtual IOrderedQueryable<TEntity> GetFilteredQuery(Expression<Func<TEntity, bool>> filter, string sort)
        {
            return this._repo.Query.AsExpandable().Where(filter).SortBy(sort);
        }
        /// <summary>
        /// Get all lists
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IEnumerable<TEntity>> GetListAsync()
        {
            return await this._repo.Query.ToListAsync();
        }
        /// <summary>
        /// Get all sorted lists
        /// </summary>
        /// <param name="sort"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<TEntity>> GetListAsync(string sort)
        {
            return await this._repo.Query.AsExpandable().SortBy(sort).ToListAsync();
        }
        /// <summary>
        /// Get sorted paged list
        /// </summary>
        /// <param name="sort"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public virtual async Task<IPagedList<TEntity>> GetListAsync(string sort, int pageNumber, int pageSize)
        {
            return await Task<IPagedList<TEntity>>.Run(() =>
            {
                IOrderedQueryable<TEntity> orderedQueryable = this._repo.Query.SortBy(sort);
                return orderedQueryable.ToPagedList(pageNumber, pageSize);
            });
        }
        /// <summary>
        /// Get a specific entity
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<TEntity> GetAsync(TId id)
        {
            TEntity model = await this._repo.FindAsync(id);
            return model;
        }
        /// <summary>
        /// Create entity after validation
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ModelState"></param>
        /// <returns></returns>
        public virtual async Task<TEntity> ValidAndCreateAsync(TEntity model, ModelStateDictionary ModelState)
        {
            return await CreateAsync(model);
        }
        /// <summary>
        /// Create entities after validation
        /// </summary>
        /// <param name="models"></param>
        /// <param name="ModelState"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<TEntity>> ValidAndCreateAsync(IEnumerable<TEntity> models, ModelStateDictionary ModelState)
        {
            return await CreateAsync(models);
        }
        /// <summary>
        /// Create entity
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected virtual async Task<TEntity> CreateAsync(TEntity model)
        {
            this._repo.Create(model);
            await this._unitOfWork.SaveChangesAsync();
            return model;
        }
        /// <summary>
        /// Create entities
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        protected virtual async Task<IEnumerable<TEntity>> CreateAsync(IEnumerable<TEntity> models)
        {
            this._repo.Create(models);
            await this._unitOfWork.SaveChangesAsync();
            return models;
        }
        /// <summary>
        /// Update entity after validation
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ModelState"></param>
        /// <returns></returns>
        public virtual async Task<TEntity> ValidAndUpdateAsync(TEntity model, ModelStateDictionary ModelState)
        {
            return await UpdateAsync(model);
        }
        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected virtual async Task<TEntity> UpdateAsync(TEntity model)
        {
            this._repo.Update(model, GetUpdatablePropertiesNameArray(model));
            await this._unitOfWork.SaveChangesAsync();
            this._repo.Reload(model);   //Only [Updatable] property is updated, so reload it to match the DB value.
            return model;
        }
        /// <summary>
        /// Delete entity after validation
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ModelState"></param>
        /// <returns></returns>
        public virtual async Task<TEntity> ValidAndDeleteAsync(TId id, ModelStateDictionary ModelState)
        {
            TEntity model = await this._repo.FindAsync(id);
            if (model == null)
            {
                ModelState.AddModelError("Id", "Id Not Exist");
                return null;
            }
            return await DeleteAsync(model);
        }
        /// <summary>
        /// Delete entities. No validation.
        /// </summary>
        /// <param name="models"></param>
        /// <param name="ModelState"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<TId>> ValidAndDeleteAsync(IEnumerable<TId> ids, ModelStateDictionary ModelState)
        {
            //No validation.
            return await DeleteAsync(ids);
        }
        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected virtual async Task<TEntity> DeleteAsync(TEntity model)
        {
            this._repo.Remove(model);
            await this._unitOfWork.SaveChangesAsync();
            return model;
        }
        protected virtual async Task<IEnumerable<TId>> DeleteAsync(IEnumerable<TId> ids)
        {
            this._repo.DbSet.RemoveRange(this._repo.DbSet.Where(x => ids.Contains(x.Id)));
            //this._repo.Remove(models);
            await this._unitOfWork.SaveChangesAsync();
            return ids;
        }
        /// <summary>
        /// Clone entity after validation
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ModelState"></param>
        /// <returns></returns>
        public virtual async Task<TEntity> ValidAndCloneAsync(TId id, ModelStateDictionary ModelState)
        {
            TEntity model = await this._repo.FindAsync(id);
            if (model == null)
            {
                ModelState.AddModelError("Id", "Id Not Exist");
                return null;
            }
            return await CloneAsync(model);
        }
        /// <summary>
        /// Clone entity
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected virtual async Task<TEntity> CloneAsync(TEntity model)
        {
            var clone = this._repo.Clone(model);
            await this._unitOfWork.SaveChangesAsync();
            return clone;
        }

        /// <summary>
        /// Make sure you are alive and try a DB query to keep the DB connection
        /// </summary>
        /// <returns></returns>
        public virtual async Task<TEntity> BeatAsync()
        {
            return await this._repo.Query.FirstOrDefaultAsync();
        }
        /// <summary>
        /// Get an array of [Updatable] property names
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ignoreNull">Whether null values are ignored.</param>
        /// <param name="updatableAttributeRequired">Whether to retrieve only [Updatable] Attribute</param>
        /// <param name="necessaryAddFields">Field names that must be added. All other conditions are ignored</param>
        /// <returns></returns>
        public string[] GetUpdatablePropertiesNameArray(TEntity model, bool ignoreNull = false, bool updatableAttributeRequired = true, IEnumerable<string> necessaryAddFields = null)
        {
            var props = model.GetType().GetProperties().Where(p =>
            {
                if (necessaryAddFields != null && necessaryAddFields.Contains(p.Name))
                {
                    return true;
                }
                var attr = p.GetCustomAttributes(typeof(UpdatableAttribute), true).FirstOrDefault();
                if (attr == null)
                {
                    if (updatableAttributeRequired == true)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                if (((UpdatableAttribute)attr).IsUpdatable == true)
                {
                    return true;
                }
                return false;
            }
                                                            );
            List<string> updateProperties = new List<string>();
            foreach (var prop in props)
            {
                var value = prop.GetValue(model);
                if (ignoreNull == false || value != null)
                {
                    updateProperties.Add(prop.Name);
                }
            }
            if (updateProperties.Count <= 0)
            {
                throw new Exception("Not Exist Updatable Properties");
            }
            return updateProperties.ToArray();
        }
        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            this._unitOfWork.Dispose();
        }
    }
}
