using org.mao.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;
using System.Linq;

namespace org.mao.Dao
{
	public abstract class AbstractPagedBaseDao<TEntity, TDbContext>: AbstractBaseDao<TEntity, TDbContext>
		where TEntity : BaseModel, new()
		where TDbContext : DbContext, new()
	{

		private int _pageSize = 16;

		public int PageSize { get { return _pageSize; } set { _pageSize = value; } }

		public override List<TEntity> List()
		{
			return List(1);
		}

		public List<TEntity> List(int page)
		{ 
			using (TDbContext repository = new TDbContext())
			{
				if (page < 1)
					return null;

                IQueryable<TEntity> rawQuery = Include(repository.Set<TEntity>());

				int rawCount = rawQuery.Count();
				int start = (page - 1) * PageSize; 

                return rawQuery
					.OrderBy((TEntity entity) => entity.Id)
					.Skip(start)
					.Take(rawCount < start + PageSize ? rawCount - start : PageSize)
					.ToList<TEntity>();
			}
		}

		public override List<TEntity> Read(TEntity model)
		{
			return Read(model, 1);
		}

		public List<TEntity> Read(TEntity model, int page)
		{
			if (page < 1)
				return null;
			
			using (TDbContext repository = new TDbContext())
			{
                IQueryable<TEntity> rawQuery = Include(repository.Set<TEntity>());

				IQueryable<TEntity> filteredQuery = Filter(rawQuery, model);

				int filteredCount = filteredQuery.Count();
				int start = (page - 1) * PageSize;

				if (filteredCount < start)
					return null;

				return filteredQuery
					.OrderBy((TEntity entity) => entity.Id)
					.Skip(start)
					.Take(filteredCount < start + PageSize ? filteredCount - start : PageSize)
					.ToList();
			}

		}

	}
}
