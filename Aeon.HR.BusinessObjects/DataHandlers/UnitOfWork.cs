using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Entities;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using EFSecondLevelCache;
using EFSecondLevelCache.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace Aeon.HR.BusinessObjects.DataHandlers
{
    public class UnitOfWork<TContext> : IRepositoryFactory, IUnitOfWork<TContext>, IUnitOfWork
        where TContext : DbContext
    {
        private Dictionary<Type, object> _repositories;
        public UserContext UserContext { get; set; }
        static UnitOfWork()
        {
            ConfigureAuditLogs();
        }
        public UnitOfWork(TContext context, IAPIContext apiCtx)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            LoadUserContext(context, apiCtx);
        }
        public void RefreshContext(IEntity obj)
        {
            try
            {
                Context.Entry(obj).Reload();
            }
            catch
            {
            }
        }
        private void LoadUserContext(TContext context, IAPIContext apiCtx)
        {
            if (UserContext == null)
            {
                UserContext = new UserContext();
                var loginName = string.Empty;
                if (apiCtx == null || String.IsNullOrEmpty(apiCtx.CurrentUser))
                {
                    loginName = "SAdmin";
                }
                else
                {
                    loginName = apiCtx.CurrentUser;
                }
                var user = Context.Set<User>().Where(x => x.LoginName == loginName && !x.IsDeleted && x.IsActivated).Cacheable().FirstOrDefault();
                if (user != null)
                {
                    UserContext.CurrentUserId = user.Id;
                    UserContext.CurrentUserName = user.LoginName;
                    UserContext.CurrentUserFullName = user.FullName;
                    UserContext.CurrentUserRole = user.Role;
                    UserContext.IsHQ = Context.Set<Department>().Where(x => !x.IsDeleted && x.IsStore == false && x.UserDepartmentMappings.Any(t => t.UserId == user.Id && t.IsHeadCount && t.Role == Group.Member)).Cacheable().Any();

                    //  if (!UserContext.IsHQ && ((UserContext.CurrentUserRole & UserRole.CB) == UserRole.CB || (UserContext.CurrentUserRole & UserRole.HR) == UserRole.HR))
                    //Fixed: AMOAEON-370 Nhân viên thuộc role HR không thấy được danh sách nhân viên nghỉ việc
                    if (((UserContext.CurrentUserRole & UserRole.CB) == UserRole.CB || (UserContext.CurrentUserRole & UserRole.HR) == UserRole.HR))
                    {
                        //If role cb, load dept of store
                        var dept = DbHelper.LoadDept(context, user.Id);
                        if (dept != null)
                        {
                            UserContext.DeptCode = dept.Code;
                            UserContext.DeptId = dept.Id;

                            var jobGradeG5 = context.Set<JobGrade>().Where(x => x.Title.Equals("G5")).FirstOrDefault();
                            if (jobGradeG5 == null)
                            {
                                jobGradeG5 = new JobGrade() { Grade = 5 };
                            }
                            if (dept.JobGrade.Grade >= jobGradeG5.Grade)
                            {
                                UserContext.DeptG5Id = dept.Id;
                            }
                            else
                            {
                                UserContext.DeptG5Id = FindG5DepartmentId(context, dept.ParentId, jobGradeG5.Grade) ?? null;
                            }

                        }
                    }
                }
            }
        }

        private Guid? FindG5DepartmentId(TContext context, Guid? parentId, int g5Grade)
        {
            if (!parentId.HasValue || parentId == null)
                return null;
            var parentDept = context.Set<Department>().Include(x => x.JobGrade).Where(x => x.Id == parentId && !x.IsDeleted).FirstOrDefault();
            if (parentDept == null)
                return null;
            if (parentDept.JobGrade != null && parentDept.JobGrade.Grade >= g5Grade)
            {
                return parentDept.Id;
            }
            return FindG5DepartmentId(context, parentDept.ParentId, g5Grade);
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class, IEntity, new()
        {
            if (_repositories == null) _repositories = new Dictionary<Type, object>();

            var type = typeof(TEntity);
            if (!_repositories.ContainsKey(type))
            {
                if (!(UserContext is null))
                {
                    if ((UserContext.CurrentUserRole & UserRole.SAdmin) == UserRole.SAdmin || (UserContext.CurrentUserRole & UserRole.HRAdmin) == UserRole.HRAdmin)
                    {
                        _repositories[type] = new Repository<TEntity>(Context, true, UserContext);
                    }

                    else if ((UserContext.CurrentUserRole & UserRole.CB) == UserRole.CB && typeof(ICBEntity).IsAssignableFrom(type) && UserContext.IsHQ)
                    {
                        _repositories[type] = new Repository<TEntity>(Context, true, UserContext);

                    }
                    else if ((UserContext.CurrentUserRole & UserRole.Accounting) == UserRole.Accounting && (typeof(BusinessTripApplication).IsAssignableFrom(type) || typeof(BusinessTripOverBudget).IsAssignableFrom(type)))
                    {
                        _repositories[type] = new Repository<TEntity>(Context, true, UserContext);
                    }
                    else if ((UserContext.CurrentUserRole & UserRole.Admin) == UserRole.Admin && typeof(BusinessTripApplication).IsAssignableFrom(type))
                    {
                        //lamnl 07/04/2022
                        _repositories[type] = new Repository<TEntity>(Context, true, UserContext);
                    }
                    else
                    {
                        _repositories[type] = new Repository<TEntity>(Context, false, UserContext);
                    }
                }
                else
                {
                    _repositories[type] = new Repository<TEntity>(Context, false, UserContext);
                }
            }
            return (IRepository<TEntity>)_repositories[type];
        }
        public IRepository<TEntity> GetRepository<TEntity>(bool forceAllItems) where TEntity : class, IEntity, new()
        {
            return new Repository<TEntity>(Context, forceAllItems, UserContext);
        }

        protected TContext Context { get; }

        TContext IUnitOfWork<TContext>.Context => throw new NotImplementedException();

        public DataSet ExecuteQuery(string sql, Dictionary<string, object> parameters)
        {
            DataSet ds = new DataSet();
            DbConnection conn = this.Context.Database.Connection;
            ConnectionState initialState = conn.State;
            try
            {
                if (initialState != ConnectionState.Open)
                    conn.Open();
                using (DbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.CommandTimeout = 120;
                    if (parameters != null && parameters.Count() > 0)
                    {
                        foreach (var item in parameters)
                        {
                            cmd.Parameters.Add(new SqlParameter(item.Key, item.Value));
                        }
                    }
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (DbDataAdapter ad = new SqlDataAdapter())
                    {
                        ad.SelectCommand = cmd;
                        ad.Fill(ds);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (initialState != ConnectionState.Open)
                    conn.Close();
            }
            return ds;
        }

        public DataSet ExecuteQuerySQL(DbConnection conn, DbTransaction transaction, string sql, Dictionary<string, object> parameters)
        {
            DataSet ds = new DataSet();
            // DbConnection conn = this.Context.Database.Connection;
            ConnectionState initialState = conn.State;
            try
            {
                using (DbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.CommandTimeout = 120;
                    cmd.CommandType = CommandType.Text;
                    if (transaction != null)
                        cmd.Transaction = transaction;

                    if (parameters != null && parameters.Count > 0)
                    {
                        foreach (var item in parameters)
                        {
                            var param = cmd.CreateParameter();
                            param.ParameterName = item.Key;
                            param.Value = item.Value ?? DBNull.Value;
                            cmd.Parameters.Add(param);
                        }
                    }

                    using (DbDataAdapter ad = new SqlDataAdapter((SqlCommand)cmd))
                    {
                        ad.Fill(ds);
                    }
                }
            }
            catch (Exception ex)
            {
                // Rollback nếu có transaction
                Console.WriteLine("ExecuteQuery error: " + ex.Message);
                throw new Exception($"ExecuteQuery error: {ex.Message}");
            }

            return ds;
        }

        public DataSet QuerySQL(string sql, Dictionary<string, object> parameters)
        {
            DataSet ds = new DataSet();
            DbConnection conn = this.Context.Database.Connection;
            ConnectionState initialState = conn.State;
            try
            {
                if (initialState != ConnectionState.Open)
                    conn.Open();

                using (DbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.CommandTimeout = 120;
                    cmd.CommandType = CommandType.Text;

                    if (parameters != null && parameters.Count > 0)
                    {
                        foreach (var item in parameters)
                        {
                            var param = cmd.CreateParameter();
                            param.ParameterName = item.Key;
                            param.Value = item.Value ?? DBNull.Value;
                            cmd.Parameters.Add(param);
                        }
                    }

                    using (DbDataAdapter ad = new SqlDataAdapter((SqlCommand)cmd))
                    {
                        ad.Fill(ds);
                    }
                }
            }
            catch (Exception ex)
            {
                // Nên log lỗi ở đây hoặc throw lại nếu cần
                Console.WriteLine("ExecuteQuery error: " + ex.Message);
            }
            finally
            {
                if (initialState != ConnectionState.Open)
                    conn.Close();
            }

            return ds;
        }

        public int Commit()
        {
            var changedEntityNames = getChangedEntityNames();
            //var audit = new Audit();
            //audit.CreatedBy = UserContext?.CurrentUserName;
            //audit.PreSaveChanges(Context);
            var rowAffecteds = Context.SaveChanges();
            //audit.PostSaveChanges();

            //if (audit.Configuration.AutoSavePreAction != null)
            //{
            //    audit.Configuration.AutoSavePreAction(Context, audit);
            //    Context.SaveChanges();
            //}
            new EFCacheServiceProvider().InvalidateCacheDependencies(changedEntityNames);
            return rowAffecteds;
        }
        public async Task<int> CommitAsync()
        {
            var changedEntityNames = getChangedEntityNames();
            var audit = new Audit();
            audit.CreatedBy = UserContext?.CurrentUserName;
            audit.PreSaveChanges(Context);
            var rowAffecteds = await Context.SaveChangesAsync();
            audit.PostSaveChanges();

            if (audit.Configuration.AutoSavePreAction != null)
            {
                audit.Configuration.AutoSavePreAction(Context, audit);
                await Context.SaveChangesAsync();
            }
            new EFCacheServiceProvider().InvalidateCacheDependencies(changedEntityNames);
            return rowAffecteds;
        }

        private static void ConfigureAuditLogs()
        {
            AuditManager.DefaultConfiguration.Exclude(x => true);
            AuditManager.DefaultConfiguration.Include<IAuditableEntity>();
            AuditManager.DefaultConfiguration.AutoSavePreAction = (context, audit) =>
             (context as HRDbContext).AuditEntries.AddRange(audit.Entries);
        }
        private string[] getChangedEntityNames()
        {
            // Updated version of this method: \EFSecondLevelCache\EFSecondLevelCache.Tests\EFSecondLevelCache.TestDataLayer\DataLayer\SampleContext.cs
            return Context.ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Added ||
                            x.State == EntityState.Modified ||
                            x.State == EntityState.Deleted)
                .Select(x => System.Data.Entity.Core.Objects.ObjectContext.GetObjectType(x.Entity.GetType()).FullName)
                .Distinct()
                .ToArray();
        }
        public void Dispose()
        {
            Context?.Dispose();
        }
    }
}
