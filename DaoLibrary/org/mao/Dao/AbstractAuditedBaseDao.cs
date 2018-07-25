using org.mao.Model;
using Microsoft.EntityFrameworkCore;
using System;

namespace org.mao.Dao
{
	public abstract class AbstractAuditedBaseDao<TEntity, TDbContext>: AbstractBaseDao<TEntity, TDbContext>
		where TEntity : AuditBaseModel, new()
		where TDbContext : DbContext, new()

	{
		
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
			model.CreateDateTime = DateTime.Now;
			model.LastUpdateDateTime = DateTime.Now;

			return base.Create(model);
		}


		public override TEntity Update(TEntity model)
		{
			model.LastUpdateDateTime = DateTime.Now;

			return base.Update(model);
		}
	}
}
