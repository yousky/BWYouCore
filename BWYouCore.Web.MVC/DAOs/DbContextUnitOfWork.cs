using BWYouCore.Web.M.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BWYouCore.Web.MVC.DAOs
{
    /// <summary>
    /// Common IUnitOfWork Implementation
    /// reference https://github.com/gyuwon/.NET-Data-Access-Layer
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    public class DbContextUnitOfWork<TDbContext> : IUnitOfWork
        where TDbContext : DbContext
    {
        private TDbContext _dbContext;
        private Dictionary<Type, object> _repositories;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbContext"></param>
        public DbContextUnitOfWork(TDbContext dbContext)
        {
            this._dbContext = dbContext;
        }
        /// <summary>
        /// Get a repository
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TId"></typeparam>
        /// <returns></returns>
        public IRepository<TEntity, TId> GetRepository<TEntity, TId>()
            where TEntity : IdModel<TId>
        {
            if (this._repositories == null)
            {
                this._repositories = new Dictionary<Type, object>();
            }
            object repository;
            if (this._repositories.TryGetValue(typeof(TEntity), out repository) == false)
            {
                repository = new DbContextRepository<TEntity, TId>(this._dbContext);
                this._repositories[typeof(TEntity)] = repository;
            }
            return (IRepository<TEntity, TId>)repository;
        }
        /// <summary>
        /// Save changes
        /// </summary>
        /// <returns></returns>
        public int SaveChanges()
        {
            return this._dbContext.SaveChanges();
        }
        /// <summary>
        /// Save changes asynchronously
        /// </summary>
        /// <returns></returns>
        public Task<int> SaveChangesAsync()
        {
            return this._dbContext.SaveChangesAsync();
        }
        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            this._dbContext.Dispose();
        }

    }
}
