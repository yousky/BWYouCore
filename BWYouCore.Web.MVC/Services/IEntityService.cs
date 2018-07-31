using BWYouCore.Web.M.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
    /// EntityService Interface
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public interface IEntityService<TEntity, TId> : IDisposable
        where TEntity : IdModel<TId>
    {
        /// <summary>
        /// Get all filtered lists
        /// </summary>
        /// <param name="model">Search for the same item with a value</param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> GetFilteredListAsync(TEntity model);
        /// <summary>
        /// Get all sorted filtered lists
        /// </summary>
        /// <param name="model">Search for the same item with a value</param>
        /// <param name="sort"></param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> GetFilteredListAsync(TEntity model, string sort);
        /// <summary>
        /// Get sorted filtered paged lists
        /// </summary>
        /// <param name="model">Search for the same item with a value</param>
        /// <param name="sort"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<IPagedList<TEntity>> GetFilteredListAsync(TEntity model, string sort, int pageNumber, int pageSize);
        /// <summary>
        /// Get all sorted filtered lists (for infinite scroll)
        /// </summary>
        /// <param name="model"></param>
        /// <param name="sort"></param>
        /// <param name="limitBaseColName">after, before column name</param>
        /// <param name="after"></param>
        /// <param name="before"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> GetFilteredListAsync(TEntity model, string sort, string limitBaseColName, TId after, TId before, int limit);
        /// <summary>
        /// Get all filtered lists
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> GetFilteredListAsync(Expression<Func<TEntity, bool>> filter);
        /// <summary>
        /// Get all sorted filtered lists
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> GetFilteredListAsync(Expression<Func<TEntity, bool>> filter, string sort);
        /// <summary>
        /// Get sorted filtered paged lists
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<IPagedList<TEntity>> GetFilteredListAsync(Expression<Func<TEntity, bool>> filter, string sort, int pageNumber, int pageSize);
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
        Task<IEnumerable<TEntity>> GetFilteredListAsync(Expression<Func<TEntity, bool>> filter, string sort, string limitBaseColName, TId after, TId before, int limit);
        /// <summary>
        /// Expose query objects
        /// </summary>
        /// <returns></returns>
        IQueryable<TEntity> Query();
        /// <summary>
        /// Expose filtered query objects
        /// </summary>
        /// <param name="model">Search for the same item with a value</param>
        /// <returns></returns>
        IQueryable<TEntity> GetFilteredQuery(TEntity model);
        /// <summary>
        /// Expose sorted filtered query objects
        /// </summary>
        /// <param name="model">Search for the same item with a value</param>
        /// <param name="sort"></param>
        /// <returns></returns>
        IOrderedQueryable<TEntity> GetFilteredQuery(TEntity model, string sort);
        /// <summary>
        /// Expose filtered query objects
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        IQueryable<TEntity> GetFilteredQuery(Expression<Func<TEntity, bool>> filter);
        /// <summary>
        /// Expose sorted filtered query objects
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        IOrderedQueryable<TEntity> GetFilteredQuery(Expression<Func<TEntity, bool>> filter, string sort);
        /// <summary>
        /// Get all lists
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> GetListAsync();
        /// <summary>
        /// Get all sorted lists
        /// </summary>
        /// <param name="sort"></param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> GetListAsync(string sort);
        /// <summary>
        /// Get sorted paged list
        /// </summary>
        /// <param name="sort"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<IPagedList<TEntity>> GetListAsync(string sort, int pageNumber, int pageSize);
        /// <summary>
        /// Get a specific entity
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<TEntity> GetAsync(TId id);
        /// <summary>
        /// Create entity after validation
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="ModelState"></param>
        /// <returns></returns>
        Task<TEntity> ValidAndCreateAsync(TEntity entity, ModelStateDictionary ModelState);
        /// <summary>
        /// Create entities after validation
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="ModelState"></param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> ValidAndCreateAsync(IEnumerable<TEntity> entities, ModelStateDictionary ModelState);
        /// <summary>
        /// Update entity after validation
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="ModelState"></param>
        /// <returns></returns>
        Task<TEntity> ValidAndUpdateAsync(TEntity entity, ModelStateDictionary ModelState);
        /// <summary>
        /// Delete entity after validation
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ModelState"></param>
        /// <returns></returns>
        Task<TEntity> ValidAndDeleteAsync(TId id, ModelStateDictionary ModelState);
        /// <summary>
        /// Delete entities. No validation.
        /// </summary>
        /// <param name="models"></param>
        /// <param name="ModelState"></param>
        /// <returns></returns>
        Task<IEnumerable<TId>> ValidAndDeleteAsync(IEnumerable<TId> models, ModelStateDictionary ModelState);
        /// <summary>
        /// Clone entity after validation
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ModelState"></param>
        /// <returns></returns>
        Task<TEntity> ValidAndCloneAsync(TId id, ModelStateDictionary ModelState);



    }
}
