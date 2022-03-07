using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using orion.web.api;
using orion.web.DataAccess.EF;
using ArcFlashlabelExpenditure = orion.web.DataAccess.EF.ArcFlashlabelExpenditure;

namespace orion.web.DataAccess
{
    public interface IApiEntityRepo<TEntity>
    {
        Task<TEntity> FindByExternalId(Guid externalId);
        Task<List<TEntity>> SearchForEntity(Expression<Func<TEntity, bool>> filter);
        Task<TEntity> UpdateEntity(Action<ArcFlashlabelExpenditure> update, Guid externalId);
        Task<TEntity> SaveEntity(TEntity toSave);
        Task DeleteEntity(Guid externalId);
    }

    public interface IArcFlashLabelExpenditureRepo : IApiEntityRepo<DataAccess.EF.ArcFlashlabelExpenditure> { }
    public class ArcFlashLabelExpenditureRepo : IArcFlashLabelExpenditureRepo
    {
        private readonly ContextFactory _contextFactory;

        public ArcFlashLabelExpenditureRepo(ContextFactory contextFactory)
        {
            this._contextFactory = contextFactory;
        }
        public async Task DeleteEntity(Guid externalId)
        {
            using (var db = _contextFactory.CreateDb())
            {
                var exp = await db.ArcFlashlabelExpenditures.SingleOrDefaultAsync(x => x.ExternalId == externalId);
                if (exp != null)
                    db.ArcFlashlabelExpenditures.Remove(exp);

                await db.SaveChangesAsync();
            }
                
        }

        public async Task<ArcFlashlabelExpenditure> FindByExternalId(Guid externalId)
        {
           using (var db = _contextFactory.CreateDb())
            {
                return await db.ArcFlashlabelExpenditures.SingleOrDefaultAsync(x => x.ExternalId == externalId);
            }
        }
        public async Task<ArcFlashlabelExpenditure> SaveEntity(ArcFlashlabelExpenditure update)
        {
            using (var db = _contextFactory.CreateDb())
            { 
                var exp = await db.ArcFlashlabelExpenditures.SingleOrDefaultAsync(x => x.Id == update.Id);
                if(exp != null)
                {
                     db.Entry(exp).CurrentValues.SetValues(update);
                }
                else
                {
                    db.ArcFlashlabelExpenditures.Add(update);
                }
                await db.SaveChangesAsync();
                return exp;
            }
        }

        public async Task<ArcFlashlabelExpenditure> UpdateEntity(Action<ArcFlashlabelExpenditure> update, Guid externalId)
        {
            using (var db = _contextFactory.CreateDb())
            {

                var exp = await db.ArcFlashlabelExpenditures.SingleOrDefaultAsync(x => x.ExternalId == externalId);

                if (exp == null)
                    return null;

                update(exp);

                await db.SaveChangesAsync();
                return exp;
            }
        }

        public  Task<List<ArcFlashlabelExpenditure>> SearchForEntity(Expression<Func<ArcFlashlabelExpenditure, bool>> filter)
        {
            using (var db = _contextFactory.CreateDb())
            {
                return db.ArcFlashlabelExpenditures.Where(filter).ToListAsync();
            }
        }

      
    }
}