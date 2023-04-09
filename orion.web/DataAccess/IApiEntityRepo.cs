using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using orion.web.api;
using orion.web.DataAccess.EF;
using orion.web.Util.IoC;
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
    public class ArcFlashLabelExpenditureRepo : IArcFlashLabelExpenditureRepo, IAutoRegisterAsSingleton
    {
        private readonly IContextFactory _contextFactory;

        public ArcFlashLabelExpenditureRepo(IContextFactory contextFactory)
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
                if (exp != null)
                {
                    db.Entry(exp).CurrentValues.SetValues(update);
                }
                else
                {
                    db.ArcFlashlabelExpenditures.Add(update);
                    exp = update;
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

        public async Task<List<ArcFlashLabelExpenditure>> SearchForEntity(Expression<Func<ArcFlashLabelExpenditure, bool>> filter)
        {
            using (var db = _contextFactory.CreateDb())
            {
                return await db.ArcFlashlabelExpenditures.Where(filter).ToListAsync();
            }
        }


    }


    public interface ICompanyVehicleExpenditureRepo : IApiEntityRepo<DataAccess.EF.CompanyVehicleExpenditure> { }
    public class CompanyVehicleExpenditureRepo : ICompanyVehicleExpenditureRepo, IAutoRegisterAsSingleton
    {
        private readonly IContextFactory _contextFactory;

        public CompanyVehicleExpenditureRepo(IContextFactory contextFactory)
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
                if (exp != null)
                {
                    db.Entry(exp).CurrentValues.SetValues(toSave);
                }
                else
                {
                    db.CompanyVehicleExpenditures.Add(toSave);
                }
                await db.SaveChangesAsync();
                return exp ?? toSave;
            }
        }

        public async Task<List<CompanyVehicleExpenditure>> SearchForEntity(Expression<Func<CompanyVehicleExpenditure, bool>> filter)
        {
            using (var db = _contextFactory.CreateDb())
            {
                return await db.CompanyVehicleExpenditures.Where(filter).ToListAsync();
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


    public interface IMiscExpenditureRepo : IApiEntityRepo<DataAccess.EF.MiscExpenditure> { }
    public class MiscExpenditureRepo : IMiscExpenditureRepo, IAutoRegisterAsSingleton
    {
        private readonly IContextFactory _contextFactory;

        public MiscExpenditureRepo(IContextFactory contextFactory)
        {
            this._contextFactory = contextFactory;
        }

        public async Task DeleteEntity(Guid externalId)
        {
            using (var db = _contextFactory.CreateDb())
            {
                var exp = await db.MiscExpenditures.SingleOrDefaultAsync(x => x.ExternalId == externalId);
                if (exp != null)
                    db.MiscExpenditures.Remove(exp);

                await db.SaveChangesAsync();
            }

        }

        public async Task<MiscExpenditure> FindByExternalId(Guid externalId)
        {
            using (var db = _contextFactory.CreateDb())
            {
                return await db.MiscExpenditures.SingleOrDefaultAsync(x => x.ExternalId == externalId);
            }
        }

        public async Task<MiscExpenditure> SaveEntity(MiscExpenditure toSave)
        {
            using (var db = _contextFactory.CreateDb())
            {
                var exp = await db.MiscExpenditures.SingleOrDefaultAsync(x => x.Id == toSave.Id);
                if (exp != null)
                {
                    db.Entry(exp).CurrentValues.SetValues(toSave);
                }
                else
                {
                    db.MiscExpenditures.Add(toSave);
                }
                await db.SaveChangesAsync();
                return exp ?? toSave;
            }
        }

        public async Task<List<MiscExpenditure>> SearchForEntity(Expression<Func<MiscExpenditure, bool>> filter)
        {
            using (var db = _contextFactory.CreateDb())
            {
                return await db.MiscExpenditures.Where(filter).ToListAsync();
            }
        }

        public async Task<MiscExpenditure> UpdateEntity(Action<MiscExpenditure> update, Guid externalId)
        {
            using (var db = _contextFactory.CreateDb())
            {

                var exp = await db.MiscExpenditures.SingleOrDefaultAsync(x => x.ExternalId == externalId);

                if (exp == null)
                    return null;

                update(exp);

                await db.SaveChangesAsync();
                return exp;
            }
        }
    }


    public interface IContractorExpenditureRepo : IApiEntityRepo<DataAccess.EF.ContractorExpenditure> { }
    public class ContractorExpenditureRepo : IContractorExpenditureRepo, IAutoRegisterAsSingleton
    {
        private readonly IContextFactory _contextFactory;

        public ContractorExpenditureRepo(IContextFactory contextFactory)
        {
            this._contextFactory = contextFactory;
        }

        public async Task DeleteEntity(Guid externalId)
        {
            using (var db = _contextFactory.CreateDb())
            {
                var exp = await db.ContractorExpenditures.SingleOrDefaultAsync(x => x.ExternalId == externalId);
                if (exp != null)
                    db.ContractorExpenditures.Remove(exp);

                await db.SaveChangesAsync();
            }

        }

        public async Task<ContractorExpenditure> FindByExternalId(Guid externalId)
        {
            using (var db = _contextFactory.CreateDb())
            {
                return await db.ContractorExpenditures.SingleOrDefaultAsync(x => x.ExternalId == externalId);
            }
        }

        public async Task<ContractorExpenditure> SaveEntity(ContractorExpenditure toSave)
        {
            using (var db = _contextFactory.CreateDb())
            {
                var exp = await db.ContractorExpenditures.SingleOrDefaultAsync(x => x.Id == toSave.Id);
                if (exp != null)
                {
                    db.Entry(exp).CurrentValues.SetValues(toSave);
                }
                else
                {
                    db.ContractorExpenditures.Add(toSave);
                }
                await db.SaveChangesAsync();
                return exp ?? toSave;
            }
        }

        public async Task<List<ContractorExpenditure>> SearchForEntity(Expression<Func<ContractorExpenditure, bool>> filter)
        {
            using (var db = _contextFactory.CreateDb())
            {
                return await db.ContractorExpenditures.Where(filter).ToListAsync();
            }
        }

        public async Task<ContractorExpenditure> UpdateEntity(Action<ContractorExpenditure> update, Guid externalId)
        {
            using (var db = _contextFactory.CreateDb())
            {

                var exp = await db.ContractorExpenditures.SingleOrDefaultAsync(x => x.ExternalId == externalId);

                if (exp == null)
                    return null;

                update(exp);

                await db.SaveChangesAsync();
                return exp;
            }
        }
    }
    public interface ITimeAndExpenceExpenditureRepo : IApiEntityRepo<DataAccess.EF.TimeAndExpenceExpenditure> { }
    public class TimeAndExpenceExpenditureRepo : ITimeAndExpenceExpenditureRepo, IAutoRegisterAsSingleton
    {
        private readonly IContextFactory _contextFactory;

        public TimeAndExpenceExpenditureRepo(IContextFactory contextFactory)
        {
            this._contextFactory = contextFactory;
        }

        public async Task DeleteEntity(Guid externalId)
        {
            using (var db = _contextFactory.CreateDb())
            {
                var exp = await db.TimeAndExpenceExpenditures.SingleOrDefaultAsync(x => x.ExternalId == externalId);
                if (exp != null)
                    db.TimeAndExpenceExpenditures.Remove(exp);

                await db.SaveChangesAsync();
            }

        }

        public async Task<TimeAndExpenceExpenditure> FindByExternalId(Guid externalId)
        {
            using (var db = _contextFactory.CreateDb())
            {
                return await db.TimeAndExpenceExpenditures.SingleOrDefaultAsync(x => x.ExternalId == externalId);
            }
        }

        public async Task<TimeAndExpenceExpenditure> SaveEntity(TimeAndExpenceExpenditure toSave)
        {
            using (var db = _contextFactory.CreateDb())
            {
                var exp = await db.TimeAndExpenceExpenditures.SingleOrDefaultAsync(x => x.Id == toSave.Id);
                if (exp != null)
                {
                    db.Entry(exp).CurrentValues.SetValues(toSave);
                }
                else
                {
                    db.TimeAndExpenceExpenditures.Add(toSave);
                }
                await db.SaveChangesAsync();
                return exp ?? toSave;
            }
        }

        public async Task<List<TimeAndExpenceExpenditure>> SearchForEntity(Expression<Func<TimeAndExpenceExpenditure, bool>> filter)
        {
            using (var db = _contextFactory.CreateDb())
            {
                return await db.TimeAndExpenceExpenditures.Where(filter).ToListAsync();
            }
        }

        public async Task<TimeAndExpenceExpenditure> UpdateEntity(Action<TimeAndExpenceExpenditure> update, Guid externalId)
        {
            using (var db = _contextFactory.CreateDb())
            {

                var exp = await db.TimeAndExpenceExpenditures.SingleOrDefaultAsync(x => x.ExternalId == externalId);

                if (exp == null)
                    return null;

                update(exp);

                await db.SaveChangesAsync();
                return exp;
            }
        }
    }
}