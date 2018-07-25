using org.mao.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;
using System.Linq;

namespace org.mao.Dao
{
	public abstract class AbstractBaseDao<TEntity, TDbContext>
		where TEntity : BaseModel, new()
		where TDbContext : DbContext, new()
	{

		public abstract void CopyModel(TEntity Source, ref TEntity Target);

		public abstract IQueryable<TEntity> Filter(IQueryable<TEntity> query, TEntity model);

		protected virtual void cloneModel(TEntity Source, ref TEntity Target)
		{
			//save the original Id
			long orgId = Target.Id;
            int orgLastUpdCnt = Target.LastUpdateCount;

			CopyModel(Source, ref Target);

			Target.Id = orgId;
            Target.LastUpdateCount = orgLastUpdCnt;

		}

        protected virtual IQueryable<TEntity> Include(DbSet<TEntity> dbset)
        {
            return dbset.AsQueryable();   
        }

		public virtual TEntity Create(TEntity model)
		{
			model.LastUpdateCount = 0;

			using (TDbContext repository = new TDbContext())
			{

				DbSet<TEntity> dbSet = repository.Set<TEntity>();
				TEntity createdModel = dbSet.Add(model).Entity;
				repository.SaveChanges();

				return createdModel;

			}

		}

		public TEntity Find(long Id)
		{
			using (TDbContext repository = new TDbContext())
			{
                IQueryable<TEntity> dbSet = Include(repository.Set<TEntity>());
                TEntity model = dbSet.Single((TEntity e)=> e.Id == Id);
				return model;
			}

		}

		public virtual List<TEntity> List()
		{
			using (TDbContext repository = new TDbContext())
			{
                IQueryable<TEntity> dbSet = Include(repository.Set<TEntity>());
                return dbSet.ToList<TEntity>();
			}
		}

		public static int PageNumber(int recordcount, int pagesize)
		{
			double d1 = (double)recordcount / (double)pagesize;
			double d2 = Math.Ceiling(d1);
			return d1 > d2 ? ((int)d2 + 1) : (int)d2;
		}

		public int Count()
		{ 
			using (TDbContext repository = new TDbContext())
			{
				return repository.Set<TEntity>().Count();
			}
		
		}

		public int Count(TEntity model)
		{ 
			using (TDbContext repository = new TDbContext())
			{
				IQueryable<TEntity> rawQuery = repository.Set<TEntity>().AsQueryable();

				IQueryable<TEntity> filteredQuery = Filter(rawQuery, model);

				return filteredQuery.Count();
			}
		}

		public virtual List<TEntity> Read(TEntity model)
		{
			using (TDbContext repository = new TDbContext())
			{
                IQueryable<TEntity> rawQuery = Include(repository.Set<TEntity>());

				IQueryable<TEntity> filteredQuery = Filter(rawQuery, model);

				return filteredQuery.ToList();
			}

		}

		public virtual TEntity Update(TEntity model)
		{

			using (TDbContext repository = new TDbContext())
			{
                IQueryable<TEntity> dbSet = Include(repository.Set<TEntity>());
				TEntity current = dbSet.Single((TEntity e) => e.Id == model.Id);

				if (current.LastUpdateCount != model.LastUpdateCount)
					throw new Exception(string.Format("The Record that you want to update is already updated by someone else.  Re-read and try again.\n{0}\n{1}",
					                                  model, current));

				cloneModel(model, ref current);

				current.LastUpdateCount += 1;

				repository.SaveChanges();
				return current;
			}

		}

		public TEntity Delete(long Id)
		{
			using (TDbContext repository = new TDbContext())
			{
                IQueryable<TEntity> dbSet = Include(repository.Set<TEntity>());
				TEntity model = dbSet.Single((TEntity e) => e.Id == Id);
				repository.Set<TEntity>().Remove(model);
				repository.SaveChanges();
				return model;
			}

		}

		public List<TEntity> Delete(TEntity model)
		{
			using (TDbContext repository = new TDbContext())
			{
                IQueryable<TEntity> dbSet = Include(repository.Set<TEntity>());
				IQueryable<TEntity> filteredQuery = Filter(dbSet, model);
				List<TEntity> deleteds = filteredQuery.ToList<TEntity>();
				repository.Set<TEntity>().RemoveRange(filteredQuery);
				repository.SaveChanges();
				return deleteds;

			}

		}

	}
}
