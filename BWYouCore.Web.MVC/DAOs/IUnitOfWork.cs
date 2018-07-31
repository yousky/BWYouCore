using BWYouCore.Web.M.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BWYouCore.Web.MVC.DAOs
{
    /// <summary>
    /// One transaction processing Interface
    /// reference https://github.com/gyuwon/.NET-Data-Access-Layer
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Get a repository
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TId"></typeparam>
        /// <returns></returns>
        IRepository<TEntity, TId> GetRepository<TEntity, TId>()
            where TEntity : IdModel<TId>;
        /// <summary>
        /// Save changes
        /// </summary>
        /// <returns></returns>
        int SaveChanges();
        /// <summary>
        /// Save changes asynchronously
        /// </summary>
        /// <returns></returns>
        Task<int> SaveChangesAsync();
    }
}
