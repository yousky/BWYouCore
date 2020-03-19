using BWYouCore.Web.M.Attributes;
using BWYouCore.Web.M.Models;
using BWYouCore.Web.MVC.Extensions;
using log4net;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BWYouCore.Web.MVC.DAOs
{
    /// <summary>
    /// Common IRepository Implementation
    /// reference https://github.com/gyuwon/.NET-Data-Access-Layer
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public class DbContextRepository<TEntity, TId> : IRepository<TEntity, TId>
        where TEntity : IdModel<TId>
    {
        readonly ILog logger = LogManager.GetLogger(typeof(DbContextRepository<TEntity, TId>));

        /// <summary>
        /// DbContext
        /// </summary>
        public DbContext DbContext { get; protected set; }
        /// <summary>
        /// DbSet
        /// </summary>
        public DbSet<TEntity> DbSet { get; protected set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public DbContextRepository()
        {

        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbContext"></param>
        public DbContextRepository(DbContext dbContext)
        {
            this.DbContext = dbContext;
            this.DbSet = dbContext.Set<TEntity>();
        }
        /// <summary>
        /// Find a specific entity
        /// </summary>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        public TEntity Find(params object[] keyValues)
        {
            return this.DbSet.Find(keyValues);
        }
        /// <summary>
        /// Find a specific entity asynchronously
        /// </summary>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        public Task<TEntity> FindAsync(params object[] keyValues)
        {
            return this.DbSet.FindAsync(keyValues);
        }
        /// <summary>
        /// Expose query objects
        /// </summary>
        public IQueryable<TEntity> Query
        {
            get
            {
                return this.DbSet;
            }
        }
        /// <summary>
        /// Create entity
        /// </summary>
        /// <param name="entity"></param>
        public void Create(TEntity entity)
        {
            this.DbSet.Add(entity);
        }
        /// <summary>
        /// Create entities
        /// </summary>
        /// <param name="entities"></param>
        public void Create(IEnumerable<TEntity> entities)
        {
            this.DbSet.AddRange(entities);
        }
        /// <summary>
        /// Update all columns except PK
        /// </summary>
        /// <param name="entity"></param>
        public void Update(TEntity entity)
        {
            this.DbContext.Entry(entity).State = EntityState.Modified;
        }
        /// <summary>
        /// Update only certain columns of unmanaged TEntity.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="updateProperties">원하는 칼럼 배열</param>
        public void Update(TEntity entity, params string[] updateProperties)
        {
            if (this.DbContext.Entry(entity).State != EntityState.Detached)
            {
                logger.Info(string.Format("Manually Updating the Entity Managed(Skip)  : type={0}, id={1}", entity.GetType().FullName, entity.Id));
                return;
            }

            this.DbContext.Entry(entity).State = EntityState.Unchanged;

            foreach (string name in updateProperties)
            {
                this.DbContext.Entry(entity).Property(name).IsModified = true;
            }
        }

        /// <summary>
        /// Update columns of unmanaged TEntity where values exist.
        /// </summary>
        /// <param name="entity"></param>
        public void UpdateExceptNullValue(TEntity entity)
        {
            if (this.DbContext.Entry(entity).State != EntityState.Detached)
            {
                logger.Info(string.Format("Manually Updating the Entity Managed(Skip) : type={0}, id={1}", entity.GetType().FullName, entity.Id));
                return;
            }

            this.DbContext.Entry(entity).State = EntityState.Modified;

            foreach (var p in this.DbContext.Entry(entity).CurrentValues.Properties)
            {
                if (this.DbContext.Entry(entity).Property(p.Name).CurrentValue == null)
                {
                    this.DbContext.Entry(entity).Property(p.Name).IsModified = false;
                }
            }
        }

        /// <summary>
        /// Update only certain columns of unmanaged TEntity where values exist.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="updateProperties"></param>
        public void UpdateExceptNullValue(TEntity entity, params string[] updateProperties)
        {
            if (this.DbContext.Entry(entity).State != EntityState.Detached)
            {
                logger.Info(string.Format("Manually Updating the Entity Managed(Skip) : type={0}, id={1}", entity.GetType().FullName, entity.Id));
                return;
            }

            this.DbContext.Entry(entity).State = EntityState.Unchanged;

            foreach (string name in updateProperties)
            {
                if (this.DbContext.Entry(entity).Property(name).CurrentValue != null)
                {
                    this.DbContext.Entry(entity).Property(name).IsModified = true;
                }
            }
        }
        /// <summary>
        /// Remove.
        /// Activation processing is included to delete related data.
        /// </summary>
        /// <param name="entity"></param>
        public void Remove(TEntity entity)
        {
            entity.ActivateRelation4Cascade(new HashSet<object>()); //Relational Activation Required to Manually Handle Unusual Relationship Structures or Lazy loading
            this.DbSet.Remove(entity);
        }
        /// <summary>
        /// Remove entities.
        /// Activation processing is included to delete related data.
        /// </summary>
        /// <param name="entities"></param>
        public void Remove(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                entity.ActivateRelation4Cascade(new HashSet<object>()); //Relational Activation Required to Manually Handle Unusual Relationship Structures or Lazy loading
            }
            this.DbSet.RemoveRange(entities);
        }
        /// <summary>
        /// Reload a entity
        /// </summary>
        /// <param name="entity"></param>
        public void Reload(TEntity entity)
        {
            this.DbContext.Entry(entity).Reload();
        }
        /// <summary>
        /// Clone(Deep Copy)
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public TEntity Clone(TEntity source)
        {
            logger.Info(string.Format("Clone Entity : type={0}, id={1}", source.GetType().FullName, source.Id));
            var clone = source.Clone(new Dictionary<object, object>(), false, true, CascadeRelationAttribute.CascadeDirection.Down, true);
            this.DbContext.Entry(source).State = EntityState.Detached;
            this.DbSet.Add(clone);
            return clone;
        }
    }
}
