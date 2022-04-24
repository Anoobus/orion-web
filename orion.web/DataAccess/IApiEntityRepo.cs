using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using orion.web.api;
using orion.web.DataAccess.EF;
using ArcFlashLabelExpenditure = orion.web.DataAccess.EF.ArcFlashLabelExpenditure;

namespace orion.web.DataAccess
{
    public interface IApiEntityRepo<TEntity>
    {
        Task<TEntity> FindByExternalId(Guid externalId);
        Task<List<TEntity>> SearchForEntity(Expression<Func<TEntity, bool>> filter);
        Task<TEntity> UpdateEntity(Action<TEntity> update, Guid externalId);
        Task<TEntity> SaveEntity(TEntity toSave);
        Task DeleteEntity(Guid externalId);
    }

    public interface IArcFlashLabelExpenditureRepo : IApiEntityRepo<DataAccess.EF.ArcFlashLabelExpenditure> { }
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

        public async Task<ArcFlashLabelExpenditure> FindByExternalId(Guid externalId)
        {
           using (var db = _contextFactory.CreateDb())
            {
                return await db.ArcFlashlabelExpenditures.SingleOrDefaultAsync(x => x.ExternalId == externalId);
            }
        }
        public async Task<ArcFlashLabelExpenditure> SaveEntity(ArcFlashLabelExpenditure update)
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

        public async Task<ArcFlashLabelExpenditure> UpdateEntity(Action<ArcFlashLabelExpenditure> update, Guid externalId)
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

        public  Task<List<ArcFlashLabelExpenditure>> SearchForEntity(Expression<Func<ArcFlashLabelExpenditure, bool>> filter)
        {
            using (var db = _contextFactory.CreateDb())
            {
                return db.ArcFlashlabelExpenditures.Where(filter).ToListAsync();
            }
        }

      
    }


     public interface ICompanyVehicleExpenditureRepo : IApiEntityRepo<DataAccess.EF.CompanyVehicleExpenditure> { }
    public class CompanyVehicleExpenditureRepo : ICompanyVehicleExpenditureRepo
    {
          private readonly ContextFactory _contextFactory;

        public CompanyVehicleExpenditureRepo(ContextFactory contextFactory)
        {
            this._contextFactory = contextFactory;
        }

          public async Task DeleteEntity(Guid externalId)
        {
            using (var db = _contextFactory.CreateDb())
            {
                var exp = await db.CompanyVehicleExpenditures.SingleOrDefaultAsync(x => x.ExternalId == externalId);
                if (exp != null)
                    db.CompanyVehicleExpenditures.Remove(exp);

                await db.SaveChangesAsync();
            }
                
        }

        public async Task<CompanyVehicleExpenditure> FindByExternalId(Guid externalId)
        {
           using (var db = _contextFactory.CreateDb())
            {
                return await db.CompanyVehicleExpenditures.SingleOrDefaultAsync(x => x.ExternalId == externalId);
            }
        }

        public async Task<CompanyVehicleExpenditure> SaveEntity(CompanyVehicleExpenditure toSave)
        {
            using (var db = _contextFactory.CreateDb())
            { 
                var exp = await db.CompanyVehicleExpenditures.SingleOrDefaultAsync(x => x.Id == toSave.Id);
                if(exp != null)
                {
                     db.Entry(exp).CurrentValues.SetValues(toSave);
                }
                else
                {
                    db.CompanyVehicleExpenditures.Add(toSave);
                }
                await db.SaveChangesAsync();
                return exp;
            }
        }

        public Task<List<CompanyVehicleExpenditure>> SearchForEntity(Expression<Func<CompanyVehicleExpenditure, bool>> filter)
        {
         using (var db = _contextFactory.CreateDb())
            {
                return db.CompanyVehicleExpenditures.Where(filter).ToListAsync();
            }
        }

        public async Task<CompanyVehicleExpenditure> UpdateEntity(Action<CompanyVehicleExpenditure> update, Guid externalId)
        {
           using (var db = _contextFactory.CreateDb())
            {

                var exp = await db.CompanyVehicleExpenditures.SingleOrDefaultAsync(x => x.ExternalId == externalId);

                if (exp == null)
                    return null;

                update(exp);

                await db.SaveChangesAsync();
                return exp;
            }
        }
    }
}