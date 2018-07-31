using BWYouCore.Web.M.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BWYouCore.Web.MVC.DAOs
{
    /// <summary>
    /// Repository Interface
    /// When saving in IUnitOfWork, it is applied within one transaction.
    /// reference https://github.com/gyuwon/.NET-Data-Access-Layer
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public interface IRepository<TEntity, TId>
        where TEntity : IdModel<TId>
    {
        /// <summary>
        /// DbContext
        /// </summary>
        DbContext DbContext { get; }
        /// <summary>
        /// DbSet
        /// </summary>
        DbSet<TEntity> DbSet { get; }

        /// <summary>
        /// Find a specific entity
        /// </summary>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        TEntity Find(params object[] keyValues);
        /// <summary>
        /// Find a specific entity asynchronously
        /// </summary>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        Task<TEntity> FindAsync(params object[] keyValues);
        /// <summary>
        /// Expose query objects
        /// </summary>
        IQueryable<TEntity> Query { get; }
        /// <summary>
        /// Create entity
        /// </summary>
        /// <param name="entity"></param>
        void Create(TEntity entity);
        /// <summary>
        /// Create entities
        /// </summary>
        /// <param name="entities"></param>
        void Create(IEnumerable<TEntity> entities);
        /// <summary>
        /// Update all columns except PK
        /// </summary>
        /// <param name="entity"></param>
        void Update(TEntity entity);
        /// <summary>
        /// Update only certain columns of unmanaged TEntity.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="updateProperties"></param>
        void Update(TEntity entity, params string[] updateProperties);
        /// <summary>
        /// Update columns of unmanaged TEntity where values exist.
        /// </summary>
        /// <param name="entity"></param>
        void UpdateExceptNullValue(TEntity entity);
        /// <summary>
        /// Update only certain columns of unmanaged TEntity where values exist.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="updateProperties"></param>
        void UpdateExceptNullValue(TEntity entity, params string[] updateProperties);
        /// <summary>
        /// Remove
        /// Activation processing is included to delete related data.
        /// </summary>
        /// <param name="entity"></param>
        void Remove(TEntity entity);
        /// <summary>
        /// Remove entities.
        /// Activation processing is included to delete related data.
        /// </summary>
        /// <param name="entities"></param>
        void Remove(IEnumerable<TEntity> entities);
        /// <summary>
        /// Reload a entity
        /// </summary>
        /// <param name="entity"></param>
        void Reload(TEntity entity);
        /// <summary>
        /// Clone(Deep Copy)
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        TEntity Clone(TEntity source);
    }
}
