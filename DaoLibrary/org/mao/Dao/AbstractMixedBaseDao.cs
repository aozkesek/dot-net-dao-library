using org.mao.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System;

namespace org.mao.Dao
{
	public abstract class AbstractMixedBaseDao<TEntity, TDbContext> : AbstractBaseDao<TEntity, TDbContext>
		where TEntity : AuditBaseModel, new()
		where TDbContext : DbContext, new()
	{

		private int _pageSize = 16;

		public int PageSize { get { return _pageSize; } set { _pageSize = value; } }

		protected override void cloneModel(TEntity Source, ref TEntity Target)
		{
			//save the original Id & create date
			long orgId = Target.Id;
            int orgLastUpdateCount = Target.LastUpdateCount;
			DateTime orgCreated = Target.CreateDateTime;

			CopyModel(Source, ref Target);

			Target.Id = orgId;
            Target.LastUpdateCount = orgLastUpdateCount;
			Target.CreateDateTime = orgCreated;

		}

		public override TEntity Create(TEntity model)
		{
			model.CreateDateTime = DateTime.UtcNow;
			model.LastUpdateDateTime = DateTime.UtcNow;

			return base.Create(model);
		}


		public override TEntity Update(TEntity model)
		{
			model.LastUpdateDateTime = DateTime.UtcNow;

			return base.Update(model);
		}

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
