using BWYouCore.Web.M.Models;
using log4net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BWYouCore.Web.MVC.DAOs
{
    public class BWIdentityDbContext<TUser> : IdentityDbContext<TUser> where TUser : IdentityUser
    {
        public ILog logger = LogManager.GetLogger(typeof(BWIdentityDbContext<TUser>));

        public BWIdentityDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            ChangeDefaultValue();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            ChangeDefaultValue();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            ChangeDefaultValue();
            return base.SaveChangesAsync(cancellationToken);
        }

        protected void ChangeDefaultValue()
        {
            int cntAdded = 0;
            int cntDeleted = 0;
            int cntModified = 0;
            foreach (var entry in ChangeTracker.Entries().Where(p => (p.State == EntityState.Added || p.State == EntityState.Deleted || p.State == EntityState.Modified)))
            {
                logger.Debug(string.Format("ChangeEntity : {0} {1}", entry.Entity.ToString(), entry.State.ToString()));
                if (entry.State == EntityState.Modified)
                {
                    cntModified++;
                }
                else if (entry.State == EntityState.Deleted)
                {
                    cntDeleted++;
                }
                else
                {
                    cntAdded++;
                }

                if (typeof(ICUModel).IsAssignableFrom(entry.Entity.GetType()))
                {
                    DateTime dtCur = DateTime.UtcNow;
                    if (entry.State == EntityState.Added)
                    {
                        ((ICUModel)entry.Entity).CreateDT = dtCur;
                    }
                    if (entry.State != EntityState.Deleted)
                    {
                        ((ICUModel)entry.Entity).UpdateDT = dtCur;
                    }
                }
            }

            logger.Debug(string.Format("ChangeEntity Count : Added={0}, Deleted={1}, Modified={2}", cntAdded, cntDeleted, cntModified));
        }

    }
}
