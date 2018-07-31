using BWYouCore.Web.M.Models;
using BWYouCore.Web.MVC.DAOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BWYouCore.Web.MVC.Services
{
    public class BWIntEntityService<TEntity> : BWEntityService<TEntity, int>
        where TEntity : BWModel<int>
    {
        public BWIntEntityService(DbContext dbContext)
            : base(dbContext)
        {

        }

        public BWIntEntityService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {

        }

    }
}
