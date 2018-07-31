using BWYouCore.Web.M.Models;
using BWYouCore.Web.MVC.DAOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BWYouCore.Web.MVC.Services
{
    public class BWLongEntityService<TEntity> : BWEntityService<TEntity, long>
        where TEntity : BWModel<long>
    {
        public BWLongEntityService(DbContext dbContext)
            : base(dbContext)
        {

        }

        public BWLongEntityService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {

        }

    }
}
