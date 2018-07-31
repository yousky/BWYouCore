using BWYouCore.Web.M.Models;
using BWYouCore.Web.MVC.DAOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BWYouCore.Web.MVC.Services
{
    public class BWEntityService<TEntity, TId> : IdEntityService<TEntity, TId>
        where TEntity : BWModel<TId>
    {
        public BWEntityService(DbContext dbContext)
            : base(dbContext)
        {

        }

        public BWEntityService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {

        }

    }
}
