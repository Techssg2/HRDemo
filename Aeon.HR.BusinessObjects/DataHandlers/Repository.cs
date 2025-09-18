using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Entities;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using AutoMapper.QueryableExtensions;
using DocumentFormat.OpenXml.Presentation;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Unity;

namespace Aeon.HR.BusinessObjects.DataHandlers
{
    public class Repository<T> : IRepository<T>
            where T : class, IEntity, new()
    {
        private static Dictionary<string, ReferenceNumber> _refs;
        private UserContext _userContext;
        protected readonly DbContext _context;
        private readonly bool _forceAllItems;
        #region Properties
        public Repository(DbContext context, bool forceAllItems, UserContext userContext)
        {
            _context = context;
            _forceAllItems = forceAllItems;
            _userContext = userContext;
            LoadRefs();
        }

        private void LoadRefs()
        {
            if (GlobalData.Instance.Refs == null || GlobalData.Instance.Refs.Count == 0)
            {
                lock (GlobalData.Instance.GlobalLock)
                {
                    if (GlobalData.Instance.Refs == null || GlobalData.Instance.Refs.Count == 0)
                    {
                        GlobalData.Instance.Refs = _context.Set<ReferenceNumber>().AsNoTracking().ToList().ToDictionary(x => x.ModuleType, x => x);
                    }
                }
            }
            _refs = GlobalData.Instance.Refs;
        }

        public Guid CurrentUserId { get { return _userContext.CurrentUserId; } }
        #endregion

        #region Query without project
        public IEnumerable<T> GetAll(string order = "", params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = Query;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return query.OrderBy(order).AsEnumerable();
        }


        public IEnumerable<T> GetAll(string order, int pageIndex, int limit, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = Query;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query.OrderBy(order).Skip((pageIndex - 1) * limit).Take(limit).AsEnumerable();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            IQueryable<T> query = Query;
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync(string order = "", params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = Query;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return await query.OrderBy(order).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync(string order, int pageIndex, int limit, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = Query;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return await query.OrderBy(order).Skip((pageIndex - 1) * limit).Take(limit).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync<TKey>(Expression<Func<T, TKey>> order, int pageIndex, int limit, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = Query;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return await query.OrderBy(order).Skip((pageIndex - 1) * limit).Take(limit).ToListAsync();
        }

        public T FindById(Guid id, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = Query;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query.FirstOrDefault(x => x.Id == id);
        }

        public T FindById(Guid id)
        {
            IQueryable<T> query = Query;
            return query.FirstOrDefault(x => x.Id == id);
        }
        public async Task<T> FindByIdAsync(Guid id)
        {
            IQueryable<T> query = Query;
            return await query.FirstOrDefaultAsync(x => x.Id == id);
        }


        public async Task<T> FindByIdAsync(Guid id, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = Query;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return await query.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<T> ITFindByIdAsync(Guid id, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = ITQuery;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return await query.FirstOrDefaultAsync(e => e.Id == id);
        }

        public T GetSingle(Expression<Func<T, bool>> predicate, string order = "", params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = Query;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            query = query.OrderBy(order);
            if (predicate == null)
            {
                return query.FirstOrDefault();
            }
            return query.Where(predicate).FirstOrDefault();
        }

        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate, string order = "", params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = Query;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            query = query.OrderBy(order);
            if (predicate == null)
            {
                return await query.FirstOrDefaultAsync();
            }
            return await query.Where(predicate).FirstOrDefaultAsync();
        }
        public async Task<T> GetSingleAsync(string predicate, object[] parameters, string order = "")
        {
            return await Query.Where(predicate, parameters).OrderBy(order).FirstOrDefaultAsync();
        }

        public IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate, string order = "", params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = Query;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query.Where(predicate).OrderBy(order).AsEnumerable();
        }
        public IEnumerable<T> ITFindBy(Expression<Func<T, bool>> predicate, string order = "", params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = ITQuery;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query.Where(predicate).OrderBy(order).AsEnumerable();
        }

        public IEnumerable<T> FindBy(string order, int pageIndex, int limit, Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = Query;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query.Where(predicate).OrderBy(order).Skip((pageIndex - 1) * limit).Take(limit).AsEnumerable();
        }
        public IEnumerable<T> FindBy(string predicate, object[] parameters, string order = "", params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = Query;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query.Where<T>(predicate, parameters).OrderBy(order).AsEnumerable();
        }
        public IEnumerable<T> FindBy(string order, int pageIndex, int limit, string predicate, object[] parameters, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = Query;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query.Where(predicate, parameters).OrderBy(order).Skip((pageIndex - 1) * limit).Take(limit).AsEnumerable();
        }
        public async Task<IEnumerable<T>> FindByAsync(Expression<Func<T, bool>> predicate, string order = "", params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = Query;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return await query.Where(predicate).OrderBy(order).ToListAsync();
        }
        public async Task<IEnumerable<T>> FindByAsync(string order, int pageIndex, int limit, Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = Query;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return await query.Where(predicate).OrderBy(order).Skip((pageIndex - 1) * limit).Take(limit).ToListAsync();
        }
        public async Task<IEnumerable<T>> FindByAsync(string predicate, object[] parameters, string order = "", params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = Query;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return await query.Where(predicate, parameters).OrderBy(order).ToListAsync();
        }
        public async Task<IEnumerable<T>> FindByAsync(string order, int pageIndex, int limit, string predicate, object[] parameters, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = Query;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return await query.Where(predicate, parameters).OrderBy(order).Skip((pageIndex - 1) * limit).Take(limit).ToListAsync();
        }
        public async Task<IEnumerable<H>> FindByAsync<H>(string order, int pageIndex, int limit, string predicate, object[] parameters, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = Query;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return await query.Where(predicate, parameters).OrderBy(order).Skip((pageIndex - 1) * limit).Take(limit).ProjectTo<H>().ToListAsync();
        }

        #endregion
        #region query with project
        public IEnumerable<H> GetAll<H>(string order = "")
        {
            return Query.OrderBy(order).ProjectTo<H>().AsEnumerable();
        }
        public IEnumerable<H> GetAll<H>(string order, int pageIndex, int limit)
        {
            return Query.OrderBy(order).Skip((pageIndex - 1) * limit).Take(limit).ProjectTo<H>().AsEnumerable();
        }
        public async Task<IEnumerable<H>> GetAllAsync<H>(string order = "")
        {
            return await Query.OrderBy(order).ProjectTo<H>().ToListAsync();
        }
        public async Task<IEnumerable<H>> GetAllAsync<H>(string order, int pageIndex, int limit)
        {
            return await Query.OrderBy(order).Skip((pageIndex - 1) * limit).Take(limit).ProjectTo<H>().ToListAsync();
        }
        public async Task<IEnumerable<H>> GetAllAsync<H, TKey>(Expression<Func<T, TKey>> order, int pageIndex, int limit)
        {
            return await Query.OrderBy(order).Skip((pageIndex - 1) * limit).Take(limit).ProjectTo<H>().ToListAsync();
        }
        public H FindById<H>(Guid id)
        {
            return Query.Where(x => x.Id == id).ProjectTo<H>().FirstOrDefault();
        }
        public async Task<H> FindByIdAsync<H>(Guid id)
        {

            return await Query.Where(x => x.Id == id).ProjectTo<H>().FirstOrDefaultAsync();
        }
        public async Task<H> ITFindByIdAsync<H>(Guid id)
        {

            return await ITQuery.Where(x => x.Id == id).ProjectTo<H>().FirstOrDefaultAsync();
        }
        public H GetSingle<H>(Expression<Func<T, bool>> predicate, string order = "")
        {
            if (predicate == null)
            {
                return Query.OrderBy(order).ProjectTo<H>().FirstOrDefault();
            }
            return Query.Where(predicate).OrderBy(order).ProjectTo<H>().FirstOrDefault();
        }
        public async Task<H> GetSingleAsync<H>(Expression<Func<T, bool>> predicate, string order = "")
        {
            IQueryable<T> query = Query;
            if (predicate == null)
            {
                return await Query.OrderBy(order).ProjectTo<H>().FirstOrDefaultAsync();
            }
            return await Query.Where(predicate).OrderBy(order).ProjectTo<H>().FirstOrDefaultAsync();
        }

        public IEnumerable<H> FindBy<H>(Expression<Func<T, bool>> predicate, string order = "")
        {
            return Query.Where(predicate).OrderBy(order).ProjectTo<H>().AsEnumerable();
        }
        public IEnumerable<H> FindBy<H>(string order, int pageIndex, int limit, Expression<Func<T, bool>> predicate)
        {
            return Query.Where(predicate).OrderBy(order).Skip((pageIndex - 1) * limit).Take(limit).ProjectTo<H>().AsEnumerable();
        }

        public IEnumerable<H> FindBy<H>(string predicate, object[] parameters, string order = "")
        {
            return Query.Where<T>(predicate, parameters).OrderBy(order).ProjectTo<H>().AsEnumerable();
        }
        public IEnumerable<H> FindBy<H>(string order, int pageIndex, int limit, string predicate, object[] parameters)
        {
            return Query.Where<T>(predicate, parameters).OrderBy(order).Skip((pageIndex - 1) * limit).Take(limit).ProjectTo<H>().AsEnumerable();
        }
        public async Task<IEnumerable<H>> FindByAsync<H>(string predicate, object[] parameters, string order = "")
        {
            return await Query.Where<T>(predicate, parameters).OrderBy(order).ProjectTo<H>().ToListAsync();
        }

        public async Task<IEnumerable<H>> FindByAsync<H>(string order, int pageIndex, int limit, string predicate, object[] parameters)
        {
            return await Query.Where<T>(predicate, parameters).OrderBy(order).Skip((pageIndex - 1) * limit).Take(limit).ProjectTo<H>().ToListAsync();
        }
        public async Task<IEnumerable<H>> FindByAsync<H>(Expression<Func<T, bool>> predicate, string order = "")
        {
            return await Query.Where(predicate).OrderBy(order).ProjectTo<H>().ToListAsync();
        }
        public async Task<IEnumerable<H>> FindByAsync<H>(string order, int pageIndex, int limit, Expression<Func<T, bool>> predicate)
        {
            return await Query.Where(predicate).OrderBy(order).Skip((pageIndex - 1) * limit).Take(limit).ProjectTo<H>().ToListAsync();
        }

        public async Task<IEnumerable<H>> FindByAsync<H, TKey>(Expression<Func<T, TKey>> order, int pageIndex, int limit, Expression<Func<T, bool>> predicate)
        {
            return await Query.Where(predicate).OrderBy(order).Skip((pageIndex - 1) * limit).Take(limit).ProjectTo<H>().ToListAsync();
        }
        #endregion

        public int CountAll()
        {
            return Query.Count();
        }

        public async Task<int> CountAllAsync()
        {
            return await Query.CountAsync();
        }
        public int Count(Expression<Func<T, bool>> predicate)
        {
            return Query.Where(predicate).Count();
        }
        public int Count(string predicate, object[] parameters)
        {
            return Query.Where(predicate, parameters).Count();
        }
        public bool Any(Expression<Func<T, bool>> predicate)
        {
            return Query.Where(predicate).Any();
        }
        public bool Any(string predicate, object[] parameters)
        {
            return Query.Where(predicate, parameters).Any();
        }
        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await Query.Where(predicate).AnyAsync();
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return await Query.Where(predicate).CountAsync();
        }
        public async Task<bool> AnyAsync(string predicate, object[] parameters)
        {
            return await Query.Where(predicate, parameters).AnyAsync();
        }

        public async Task<int> CountAsync(string predicate, object[] parameters)
        {
            return await Query.Where(predicate, parameters).CountAsync();
        }

        public void Add(T entity)
        {
            entity.Id = Guid.NewGuid();
            entity.Created = DateTime.Now;
            entity.Modified = DateTime.Now;
            if (entity is IAutoNumber autoNamingEntity)
            {
                autoNamingEntity.ReferenceNumber = GenerateReferenceNumber(typeof(T).Name);
            }
            if (entity is AuditableEntity auditable)
            {
                auditable.CreatedById = _userContext.CurrentUserId;
                auditable.ModifiedById = _userContext.CurrentUserId;
                auditable.CreatedBy = _userContext.CurrentUserName;
                auditable.CreatedByFullName = _userContext.CurrentUserFullName;
                auditable.ModifiedBy = _userContext.CurrentUserName;
                auditable.ModifiedByFullName = _userContext.CurrentUserFullName;
            }
            if (entity is IPermission)
            {
                AddPerm(entity);
            }
            if (entity is WorkflowEntity wfEntity)
            {
                wfEntity.Status = "Draft"; //add draft for work flow entity by default
            }
            _context.Set<T>().Add(entity);
        }
        /// <summary>
        /// Adds RECORD AND KEEP identifier.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void AddWithId(T entity)
        {
            entity.Created = DateTime.Now;
            entity.Modified = DateTime.Now;
            if (entity is IAutoNumber autoNamingEntity)
            {
                autoNamingEntity.ReferenceNumber = GenerateReferenceNumber(typeof(T).Name);
            }
            if (entity is AuditableEntity auditable)
            {
                auditable.CreatedById = _userContext?.CurrentUserId;
                auditable.ModifiedById = !(_userContext is null) ? _userContext.CurrentUserId : new Guid();
                auditable.CreatedBy = _userContext?.CurrentUserName;
                auditable.CreatedByFullName = _userContext?.CurrentUserFullName;
                auditable.ModifiedBy = _userContext?.CurrentUserName;
                auditable.ModifiedByFullName = _userContext?.CurrentUserFullName;
            }
            if (entity is IPermission)
            {
                AddPerm(entity);
            }
            if (entity is WorkflowEntity wfEntity)
            {
                wfEntity.Status = "Draft"; //add draft for work flow entity by default
            }
            _context.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            entity.Modified = DateTime.Now;
            try
            {
                var dbEntityEntry = _context.Entry(entity);
                dbEntityEntry.State = EntityState.Modified;
                dbEntityEntry.Property(x => x.Created).IsModified = false;
                if (entity is AuditableEntity auditEntity && !(_userContext is null))
                {
                    auditEntity.Modified = DateTime.Now;
                    auditEntity.ModifiedById = _userContext.CurrentUserId;
                    auditEntity.ModifiedBy = _userContext.CurrentUserName;
                    auditEntity.ModifiedByFullName = _userContext.CurrentUserFullName;
                    var auditableEntityEntry = _context.Entry(auditEntity);
                    auditableEntityEntry.Property(x => x.CreatedBy).IsModified = false;
                }

            }
            catch (Exception ex)
            {
                var e = ex;
            }


        }
        public void Delete(T entity)
        {
            if (entity is ISoftDeleteEntity softDeleteEntity)
            {
                softDeleteEntity.IsDeleted = true;
                softDeleteEntity.Modified = DateTime.Now;
                var dbEntityEntry = _context.Entry(softDeleteEntity);
                dbEntityEntry.Property(x => x.IsDeleted).IsModified = true;
                dbEntityEntry.Property(x => x.Modified).IsModified = true;
            }
            else
            {
                var dbEntityEntry = _context.Entry(entity);
                dbEntityEntry.State = EntityState.Deleted;
            }
        }

        public void Add(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                entity.Id = Guid.NewGuid();
                entity.Created = DateTime.Now;
                entity.Modified = DateTime.Now;
                if (entity is AuditableEntity auditable)
                {
                    auditable.CreatedById = _userContext.CurrentUserId;
                    auditable.ModifiedById = _userContext.CurrentUserId;
                    auditable.CreatedBy = _userContext.CurrentUserName;
                    auditable.CreatedByFullName = _userContext.CurrentUserFullName;
                    auditable.ModifiedBy = _userContext.CurrentUserName;
                    auditable.ModifiedByFullName = _userContext.CurrentUserFullName;
                }
                if (entity is IAutoNumber autoNamingEntity)
                {
                    autoNamingEntity.ReferenceNumber = GenerateReferenceNumber(typeof(T).Name);
                }
                if (entity is IPermission)
                {
                    AddPerm(entity);
                }
                if (entity is WorkflowEntity wfEntity)
                {
                    wfEntity.Status = "Draft"; //add draft for work flow entity by default
                }
            }
            _context.Set<T>().AddRange(entities);
        }

        public void Update(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                Update(entity);
            }
        }

        public void Delete(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                Delete(entity);
            }
        }

        public void Load(T entity, params Expression<Func<T, object>>[] includeProperties)
        {
            foreach (var includeProperty in includeProperties)
            {
                _context.Entry(entity).Reference(includeProperty).Load();
            }

        }

        private IQueryable<T> Query
        {
            get
            {
                var query = _context.Set<T>().AsQueryable();

                //AEONCR112-776 - view phieu theo _userContext.DeptG5Id
                var deptLineIds = new HashSet<Guid>();
                var deptLineCodes = new HashSet<string>();

                var allowViewAll = false;
                List<Type> allowViewAllType = new List<Type>();
                allowViewAllType.Add(typeof(ResignationApplication));
                allowViewAllType.Add(typeof(PromoteAndTransfer));
                allowViewAllType.Add(typeof(Acting));
                allowViewAllType.Add(typeof(RequestToHire));
                allowViewAllType.Add(typeof(BusinessTripOverBudget));
                if (allowViewAllType.Contains(typeof(T)) && !(_userContext is null) && (_userContext.CurrentUserRole & UserRole.HR) == UserRole.HR)
                {
                    //Fix for special case, promotion required two hr store to check
                    if (_userContext.IsHQ)
                    {
                        allowViewAll = true;
                    }
                }
                // Allow Accounting role to view all BusinessTripOverBudget (BOB) forms
                if (typeof(T) == typeof(BusinessTripOverBudget) && !(_userContext is null) && (_userContext.CurrentUserRole & UserRole.Accounting) == UserRole.Accounting)
                {
                    allowViewAll = true;
                }
                if (!_forceAllItems && !allowViewAll)
                {
                    #region Phan Quyen User Duoc Xem
                    try
                    {
                        var canView = _context.Set<CanViewDepartment>().AsQueryable().Where(x => x.UserId == _userContext.CurrentUserId);
                        if (canView != null)
                        {
                            deptLineIds = new HashSet<Guid>(canView.Select(x => x.DeptLineId).ToList());
                            deptLineCodes = new HashSet<string>(canView.Select(x => x.DeptLineCode).ToList());
                        }
                    }
                    catch (Exception ex) { }
                    #endregion
                    if (typeof(IPermission).IsAssignableFrom(typeof(T)))
                    {
                        IQueryable<T> cbQuery = null;
                        if (!string.IsNullOrEmpty(_userContext.DeptCode) && !_userContext.IsHQ && ((_userContext.CurrentUserRole & UserRole.CB) == UserRole.CB && typeof(ICBEntity).IsAssignableFrom(typeof(T)) && typeof(WorkflowEntity).IsAssignableFrom(typeof(T))))
                        {
                            //Fix for special case, shift exchange doesn't store deptcode and deptname, store deptid instead
                            if (typeof(T) == typeof(ShiftExchangeApplication))
                            {
                                try
                                {
                                    string user = ConfigurationManager.AppSettings["FixUserForHR_U2"] != null ? ConfigurationManager.AppSettings["FixUserForHR_U2"] : "413759";
                                    var userNewHardCode_HR1392_CB = ConfigurationManager.AppSettings["HR1392_USER_CB"] != null ? ConfigurationManager.AppSettings["HR1392_USER_CB"] : "55213E4F-A5AA-4064-A044-91A0D8DD1B0F";
                                    if (!string.IsNullOrEmpty(_userContext.CurrentUserName) && (user.Equals(_userContext.CurrentUserName) || userNewHardCode_HR1392_CB.Equals(_userContext.CurrentUserId.ToString().ToUpper())))
                                    {
                                        if (user.Equals(_userContext.CurrentUserName))
                                        {
                                            string departmentId = ConfigurationManager.AppSettings["FixUserForHR_U1_DI1"] != null ? ConfigurationManager.AppSettings["FixUserForHR_U1_DI1"] : "40004566";
                                            cbQuery = query.Where("deptlineid=@0 || deptlineid=@1 || deptlineid=@2", _userContext.DeptId, new Guid(departmentId), _userContext.DeptG5Id);
                                        }
                                        else
                                        {
                                            string HaiPhongG5Id = ConfigurationManager.AppSettings["HR1392_DEPT_HAI_PHONG_G5_ID"] != null ? ConfigurationManager.AppSettings["HR1392_DEPT_HAI_PHONG_G5_ID"] : "9F2DB1C6-7365-4976-ACE0-954F9D329027";
                                            string HaiPhongDsmApproverId = ConfigurationManager.AppSettings["HR1392_DEPT_HAI_PHONG_DSM_APPROVER_ID"] != null ? ConfigurationManager.AppSettings["HR1392_DEPT_HAI_PHONG_DSM_APPROVER_ID"] : "D9ABE4FC-D6DD-4768-A0ED-CD666E4A4A93";
                                            cbQuery = query.Where("deptlineid=@0 || deptlineid=@1 || deptlineid=@2 || deptlineid=@3", _userContext.DeptId, new Guid(HaiPhongG5Id), new Guid(HaiPhongDsmApproverId), _userContext.DeptG5Id);
                                        }
                                    }
                                    else if (_userContext.CurrentUserId.ToString().Equals("01dc17f9-a2ab-4ed8-8526-fcf34338a5fd"))
                                    {
                                        cbQuery = query.Where("deptlineid=@0 || deptlineid=@1 || deptlineid=@2 || deptlineid=@3", _userContext.DeptId, new Guid("48857b53-8dbb-439d-84cf-e39ea42bc8cd"), new Guid("0ce0585a-8ec9-4f44-a6ef-ecb237e7c315"), _userContext.DeptG5Id);
                                    }
                                    else if (_userContext.CurrentUserId.ToString().Equals("f1d82902-b8c5-4f19-90e8-40916c972f96"))
                                    {
                                        cbQuery = query.Where("deptlineid=@0 || deptlineid=@1 || deptlineid=@2", _userContext.DeptId, new Guid("a9e262d7-1217-4703-aa8e-43ee50549206"), _userContext.DeptG5Id);
                                    }
                                    else
                                    {
                                        cbQuery = query.Where("deptlineid=@0 || deptlineid=@1", _userContext.DeptId, _userContext.DeptG5Id);
                                    }
                                }
                                catch (Exception e)
                                {
                                    cbQuery = query.Where("deptlineid=@0 || deptlineid=@1", _userContext.DeptId, _userContext.DeptG5Id);
                                }
                            }
                            else
                            {
                                // fix 2 case for HR Task #9913
                                try
                                {
                                    string user = ConfigurationManager.AppSettings["FixUserForHR_U2"] != null ? ConfigurationManager.AppSettings["FixUserForHR_U2"] : "413759";
                                    var userNewHardCode_HR1392_CB = ConfigurationManager.AppSettings["HR1392_USER_CB"] != null ? ConfigurationManager.AppSettings["HR1392_USER_CB"] : "55213E4F-A5AA-4064-A044-91A0D8DD1B0F";
                                    if ((typeof(LeaveApplication).IsAssignableFrom(typeof(T)) || typeof(MissingTimeClock).IsAssignableFrom(typeof(T)) || typeof(ResignationApplication).IsAssignableFrom(typeof(T))
                                    /*|| typeof(OvertimeApplication).IsAssignableFrom(typeof(T)) */ || typeof(TargetPlan).IsAssignableFrom(typeof(T))) && !string.IsNullOrEmpty(_userContext.CurrentUserName)
                                    && (user.Equals(_userContext.CurrentUserName) || userNewHardCode_HR1392_CB.Equals(_userContext.CurrentUserId.ToString().ToUpper())))
                                    {
                                        if (user.Equals(_userContext.CurrentUserName))
                                        {
                                            string departmentCode = ConfigurationManager.AppSettings["FixUserForHR_U1_DC1"] != null ? ConfigurationManager.AppSettings["FixUserForHR_U1_DC1"] : "40004566";
                                            // cbQuery = query.Where("deptcode=@0 || deptcode=@1", _userContext.DeptCode, departmentCode);
                                            #region
                                            if (typeof(ResignationApplication).IsAssignableFrom(typeof(T)) ||
                                                typeof(MissingTimeClock).IsAssignableFrom(typeof(T)) ||
                                                typeof(TargetPlan).IsAssignableFrom(typeof(T))
                                                )
                                            {
                                                // Logic mới - sử dụng deptId
                                                deptLineIds.Add(_userContext.DeptId);
                                                deptLineIds.Add(_userContext.DeptG5Id.Value);
                                                deptLineIds.Add(new Guid("C5B6EC5E-8918-4F4A-B9FC-1E6C6BE381E2"));//deptId của 40004566

                                                deptLineCodes.Add(_userContext.DeptCode);
                                                deptLineCodes.Add(departmentCode);

                                                string deptIdCondition = string.Join(" || ", deptLineIds.Select((_, index) => $"deptid = @{index}"));
                                                string deptCodeCondition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index + deptLineIds.Count}"));

                                                var parameters = new List<object>();
                                                parameters.AddRange(deptLineIds.Cast<object>());
                                                parameters.AddRange(deptLineCodes.Cast<object>());

                                                string combinedCondition = $"({deptIdCondition}) || ({deptCodeCondition})";
                                                cbQuery = query.Where(combinedCondition, parameters.ToArray());
                                            }
                                            else
                                            {
                                                deptLineCodes.Add(_userContext.DeptCode);
                                                deptLineCodes.Add(departmentCode);
                                                deptLineIds.Add(_userContext.DeptG5Id.Value);

                                                string deptCodeCondition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index}"));
                                                string deptIdCondition = $"deptid = @{deptLineCodes.Count}";
                                                string combinedCondition = $"({deptCodeCondition}) || ({deptIdCondition})";

                                                //string condition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index}"));
                                                //cbQuery = query.Where(condition, deptLineCodes.ToArray());

                                                var parameters = new List<object>();
                                                parameters.AddRange(deptLineCodes.Cast<object>());
                                                parameters.Add(_userContext.DeptG5Id.Value);

                                                cbQuery = query.Where(combinedCondition, parameters.ToArray());
                                            }
                                            #endregion
                                        }
                                        else
                                        {
                                            string HaiPhongG5Code = ConfigurationManager.AppSettings["HR1392_DEPT_HAI_PHONG_G5_CODE"] != null ? ConfigurationManager.AppSettings["HR1392_DEPT_HAI_PHONG_G5_CODE"] : "50021419";
                                            string HaiPhongDsmApproverCode = ConfigurationManager.AppSettings["HR1392_DEPT_HAI_PHONG_DSM_APPROVER_CODE"] != null ? ConfigurationManager.AppSettings["HR1392_DEPT_HAI_PHONG_DSM_APPROVER_CODE"] : "HP_DSM";
                                            //cbQuery = query.Where("deptcode=@0 || deptcode=@1 || deptcode=@2", _userContext.DeptCode, HaiPhongG5Code, HaiPhongDsmApproverCode);
                                            #region
                                            if (typeof(ResignationApplication).IsAssignableFrom(typeof(T)) ||
                                                typeof(MissingTimeClock).IsAssignableFrom(typeof(T)) ||
                                                typeof(TargetPlan).IsAssignableFrom(typeof(T))
                                                )
                                            {
                                                // Logic mới - sử dụng deptId
                                                deptLineIds.Add(_userContext.DeptId);
                                                deptLineIds.Add(_userContext.DeptG5Id.Value);
                                                deptLineIds.Add(new Guid("9F2DB1C6-7365-4976-ACE0-954F9D329027"));//deptId của 50021419
                                                deptLineIds.Add(new Guid("D9ABE4FC-D6DD-4768-A0ED-CD666E4A4A93"));//deptId của HP_DSM

                                                deptLineCodes.Add(_userContext.DeptCode);
                                                deptLineCodes.Add(HaiPhongG5Code);
                                                deptLineCodes.Add(HaiPhongDsmApproverCode);

                                                string deptIdCondition = string.Join(" || ", deptLineIds.Select((_, index) => $"deptid = @{index}"));
                                                string deptCodeCondition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index + deptLineIds.Count}"));

                                                var parameters = new List<object>();
                                                parameters.AddRange(deptLineIds.Cast<object>());
                                                parameters.AddRange(deptLineCodes.Cast<object>());

                                                string combinedCondition = $"({deptIdCondition}) || ({deptCodeCondition})";
                                                cbQuery = query.Where(combinedCondition, parameters.ToArray());
                                            }
                                            else
                                            {
                                                deptLineCodes.Add(_userContext.DeptCode);
                                                deptLineCodes.Add(HaiPhongG5Code);
                                                deptLineCodes.Add(HaiPhongDsmApproverCode);
                                                deptLineIds.Add(_userContext.DeptG5Id.Value);

                                                //string condition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index}"));
                                                //cbQuery = query.Where(condition, deptLineCodes.ToArray());

                                                string deptCodeCondition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index}"));
                                                string deptIdCondition = $"deptid = @{deptLineCodes.Count}";
                                                string combinedCondition = $"({deptCodeCondition}) || ({deptIdCondition})";

                                                var parameters = new List<object>();
                                                parameters.AddRange(deptLineCodes.Cast<object>());
                                                parameters.Add(_userContext.DeptG5Id.Value);

                                                cbQuery = query.Where(combinedCondition, parameters.ToArray());
                                            }
                                            #endregion
                                        }
                                    }
                                    else if ((typeof(LeaveApplication).IsAssignableFrom(typeof(T)) || typeof(MissingTimeClock).IsAssignableFrom(typeof(T)) || typeof(ResignationApplication).IsAssignableFrom(typeof(T))
                                    /*|| typeof(OvertimeApplication).IsAssignableFrom(typeof(T)) */ || typeof(TargetPlan).IsAssignableFrom(typeof(T))) && _userContext.CurrentUserId.ToString().Equals((("01dc17f9-a2ab-4ed8-8526-fcf34338a5fd")).ToString().ToLower()))
                                    {
                                        //cbQuery = query.Where("deptcode=@0 || deptcode=@1 || deptcode=@2", _userContext.DeptCode, "DSM_LB", "40000068");
                                        #region
                                        if (typeof(ResignationApplication).IsAssignableFrom(typeof(T)) ||
                                                typeof(MissingTimeClock).IsAssignableFrom(typeof(T)) ||
                                                typeof(TargetPlan).IsAssignableFrom(typeof(T))
                                                )
                                        {
                                            // Logic mới - sử dụng deptId
                                            deptLineIds.Add(_userContext.DeptId);
                                            deptLineIds.Add(_userContext.DeptG5Id.Value);
                                            deptLineIds.Add(new Guid("0CE0585A-8EC9-4F44-A6EF-ECB237E7C315"));//deptId của DSM_LB
                                            deptLineIds.Add(new Guid("48857B53-8DBB-439D-84CF-E39EA42BC8CD"));//deptId của 40000068

                                            deptLineCodes.Add(_userContext.DeptCode);
                                            deptLineCodes.Add("DSM_LB");
                                            deptLineCodes.Add("40000068");

                                            string deptIdCondition = string.Join(" || ", deptLineIds.Select((_, index) => $"deptid = @{index}"));
                                            string deptCodeCondition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index + deptLineIds.Count}"));

                                            var parameters = new List<object>();
                                            parameters.AddRange(deptLineIds.Cast<object>());
                                            parameters.AddRange(deptLineCodes.Cast<object>());

                                            string combinedCondition = $"({deptIdCondition}) || ({deptCodeCondition})";
                                            cbQuery = query.Where(combinedCondition, parameters.ToArray());
                                        }
                                        else
                                        {
                                            deptLineCodes.Add(_userContext.DeptCode);
                                            deptLineCodes.Add("DSM_LB");
                                            deptLineCodes.Add("40000068");
                                            deptLineIds.Add(_userContext.DeptG5Id.Value);

                                            //string condition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index}"));
                                            //cbQuery = query.Where(condition, deptLineCodes.ToArray());

                                            string deptCodeCondition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index}"));
                                            string deptIdCondition = $"deptid = @{deptLineCodes.Count}";
                                            string combinedCondition = $"({deptCodeCondition}) || ({deptIdCondition})";

                                            var parameters = new List<object>();
                                            parameters.AddRange(deptLineCodes.Cast<object>());
                                            parameters.Add(_userContext.DeptG5Id.Value);

                                            cbQuery = query.Where(combinedCondition, parameters.ToArray());
                                        }
                                        #endregion
                                    }
                                    else if ((typeof(LeaveApplication).IsAssignableFrom(typeof(T)) || typeof(MissingTimeClock).IsAssignableFrom(typeof(T)) || typeof(ResignationApplication).IsAssignableFrom(typeof(T))
                                    /*|| typeof(OvertimeApplication).IsAssignableFrom(typeof(T)) */ || typeof(TargetPlan).IsAssignableFrom(typeof(T))) && _userContext.CurrentUserId.ToString().Equals((("f1d82902-b8c5-4f19-90e8-40916c972f96")).ToString().ToLower()))
                                    {
                                        // cbQuery = query.Where("deptcode=@0 || deptcode=@1", _userContext.DeptCode, "DEP5000018");
                                        #region
                                        if (typeof(ResignationApplication).IsAssignableFrom(typeof(T)) ||
                                                typeof(MissingTimeClock).IsAssignableFrom(typeof(T)) ||
                                                typeof(TargetPlan).IsAssignableFrom(typeof(T))
                                                )
                                        {
                                            // Logic mới - sử dụng deptId
                                            deptLineIds.Add(_userContext.DeptId);
                                            deptLineIds.Add(_userContext.DeptG5Id.Value);
                                            deptLineIds.Add(new Guid("A9E262D7-1217-4703-AA8E-43EE50549206"));//deptId của DEP5000018

                                            deptLineCodes.Add(_userContext.DeptCode);
                                            deptLineCodes.Add("DEP5000018");

                                            string deptIdCondition = string.Join(" || ", deptLineIds.Select((_, index) => $"deptid = @{index}"));
                                            string deptCodeCondition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index + deptLineIds.Count}"));

                                            var parameters = new List<object>();
                                            parameters.AddRange(deptLineIds.Cast<object>());
                                            parameters.AddRange(deptLineCodes.Cast<object>());

                                            string combinedCondition = $"({deptIdCondition}) || ({deptCodeCondition})";
                                            cbQuery = query.Where(combinedCondition, parameters.ToArray());
                                        }
                                        else
                                        {
                                            deptLineCodes.Add(_userContext.DeptCode);
                                            deptLineCodes.Add("DEP5000018");
                                            deptLineIds.Add(_userContext.DeptG5Id.Value);

                                            //string condition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index}"));
                                            //cbQuery = query.Where(condition, deptLineCodes.ToArray());

                                            string deptCodeCondition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index}"));
                                            string deptIdCondition = $"deptid = @{deptLineCodes.Count}";
                                            string combinedCondition = $"({deptCodeCondition}) || ({deptIdCondition})";

                                            var parameters = new List<object>();
                                            parameters.AddRange(deptLineCodes.Cast<object>());
                                            parameters.Add(_userContext.DeptG5Id.Value);

                                            cbQuery = query.Where(combinedCondition, parameters.ToArray());
                                        }
                                        #endregion
                                    }

                                    // Fix OvertimeApplication --- query with deptlineid
                                    else if (typeof(OvertimeApplication).IsAssignableFrom(typeof(T)))
                                    {
                                        if (!string.IsNullOrEmpty(_userContext.CurrentUserName) && (user.Equals(_userContext.CurrentUserName) || userNewHardCode_HR1392_CB.Equals(_userContext.CurrentUserId.ToString().ToUpper())))
                                        {
                                            if (user.Equals(_userContext.CurrentUserName))
                                            {
                                                string departmentCode = ConfigurationManager.AppSettings["FixUserForHR_U1_DC1"] != null ? ConfigurationManager.AppSettings["FixUserForHR_U1_DC1"] : "40004566";
                                                #region
                                                deptLineCodes.Add(_userContext.DeptCode);
                                                deptLineCodes.Add(departmentCode);

                                                //string condition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index}")) + $" || deptid = @{deptLineCodes.Count}";
                                                //cbQuery = query.Where(condition, deptLineCodes.Cast<object>().Concat(new object[] { _userContext.DeptId }).ToArray());

                                                string condition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index}")) + $" || deptid = @{deptLineCodes.Count} || deptid = @{deptLineCodes.Count + 1}";
                                                cbQuery = query.Where(condition, deptLineCodes.Cast<object>().Concat(new object[] { _userContext.DeptId, _userContext.DeptG5Id.Value }).ToArray());
                                                #endregion
                                            }
                                            else
                                            {
                                                string HaiPhongG5Code = ConfigurationManager.AppSettings["HR1392_DEPT_HAI_PHONG_G5_CODE"] != null ? ConfigurationManager.AppSettings["HR1392_DEPT_HAI_PHONG_G5_CODE"] : "50021419";
                                                string HaiPhongDsmApproverCode = ConfigurationManager.AppSettings["HR1392_DEPT_HAI_PHONG_DSM_APPROVER_CODE"] != null ? ConfigurationManager.AppSettings["HR1392_DEPT_HAI_PHONG_DSM_APPROVER_CODE"] : "HP_DSM";
                                                //cbQuery = query.Where("deptcode=@0 || deptcode=@1 || deptcode=@2", _userContext.DeptCode, HaiPhongG5Code, HaiPhongDsmApproverCode);
                                                #region
                                                deptLineCodes.Add(_userContext.DeptCode);
                                                deptLineCodes.Add(HaiPhongG5Code);
                                                deptLineCodes.Add(HaiPhongDsmApproverCode);

                                                //string condition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index}")) + $" || deptid = @{deptLineCodes.Count}";
                                                //cbQuery = query.Where(condition, deptLineCodes.Cast<object>().Concat(new object[] { _userContext.DeptId }).ToArray());

                                                string condition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index}")) + $" || deptid = @{deptLineCodes.Count} || deptid = @{deptLineCodes.Count + 1}";
                                                cbQuery = query.Where(condition, deptLineCodes.Cast<object>().Concat(new object[] { _userContext.DeptId, _userContext.DeptG5Id.Value }).ToArray());
                                                #endregion
                                            }
                                        }
                                        else if (_userContext.CurrentUserId.ToString().Equals((("01dc17f9-a2ab-4ed8-8526-fcf34338a5fd")).ToString().ToLower()))
                                        {
                                            #region
                                            deptLineCodes.Add(_userContext.DeptCode);
                                            deptLineCodes.Add("DSM_LB");
                                            deptLineCodes.Add("40000068");

                                            //string condition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index}")) + $" || deptid = @{deptLineCodes.Count}";
                                            //cbQuery = query.Where(condition, deptLineCodes.Cast<object>().Concat(new object[] { _userContext.DeptId }).ToArray());

                                            string condition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index}")) + $" || deptid = @{deptLineCodes.Count} || deptid = @{deptLineCodes.Count + 1}";
                                            cbQuery = query.Where(condition, deptLineCodes.Cast<object>().Concat(new object[] { _userContext.DeptId, _userContext.DeptG5Id.Value }).ToArray());
                                            #endregion
                                        }
                                        else if (_userContext.CurrentUserId.ToString().Equals((("f1d82902-b8c5-4f19-90e8-40916c972f96")).ToString().ToLower()))
                                        {
                                            #region
                                            deptLineCodes.Add(_userContext.DeptCode);
                                            deptLineCodes.Add("DEP5000018");

                                            //string condition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index}")) + $" || deptid = @{deptLineCodes.Count}";
                                            //cbQuery = query.Where(condition, deptLineCodes.Cast<object>().Concat(new object[] { _userContext.DeptId }).ToArray());

                                            string condition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index}")) + $" || deptid = @{deptLineCodes.Count} || deptid = @{deptLineCodes.Count + 1}";
                                            cbQuery = query.Where(condition, deptLineCodes.Cast<object>().Concat(new object[] { _userContext.DeptId, _userContext.DeptG5Id.Value }).ToArray());
                                            #endregion
                                        }
                                        else
                                        {
                                            deptLineCodes.Add(_userContext.DeptCode);

                                            //string condition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index}")) + $" || deptid = @{deptLineCodes.Count}";
                                            //cbQuery = query.Where(condition, deptLineCodes.Cast<object>().Concat(new object[] { _userContext.DeptId }).ToArray());

                                            string condition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index}")) + $" || deptid = @{deptLineCodes.Count} || deptid = @{deptLineCodes.Count + 1}";
                                            cbQuery = query.Where(condition, deptLineCodes.Cast<object>().Concat(new object[] { _userContext.DeptId, _userContext.DeptG5Id.Value }).ToArray());
                                        }
                                    }
                                    else
                                    {
                                        //cbQuery = query.Where("deptcode=@0", _userContext.DeptCode);
                                        if (typeof(ResignationApplication).IsAssignableFrom(typeof(T)) ||
                                                typeof(MissingTimeClock).IsAssignableFrom(typeof(T)) ||
                                                typeof(TargetPlan).IsAssignableFrom(typeof(T))
                                                )
                                        {
                                            // Logic mới - sử dụng deptId
                                            deptLineIds.Add(_userContext.DeptId);
                                            deptLineIds.Add(_userContext.DeptG5Id.Value);
                                            deptLineIds.Add(new Guid("A9E262D7-1217-4703-AA8E-43EE50549206"));//deptId của DEP5000018

                                            deptLineCodes.Add(_userContext.DeptCode);

                                            string deptIdCondition = string.Join(" || ", deptLineIds.Select((_, index) => $"deptid = @{index}"));
                                            string deptCodeCondition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index + deptLineIds.Count}"));

                                            var parameters = new List<object>();
                                            parameters.AddRange(deptLineIds.Cast<object>());
                                            parameters.AddRange(deptLineCodes.Cast<object>());

                                            string combinedCondition = $"({deptIdCondition}) || ({deptCodeCondition})";
                                            cbQuery = query.Where(combinedCondition, parameters.ToArray());
                                        }
                                        else
                                        {
                                            //AEONCR112-808 - BTA & BOB - fix query with deptlineid
                                            if (typeof(T) == typeof(BusinessTripOverBudget) || typeof(T) == typeof(BusinessTripApplication))
                                            {
                                                deptLineCodes.Add(_userContext.DeptCode);
                                                deptLineIds.Add(_userContext.DeptId);
                                                deptLineIds.Add(_userContext.DeptG5Id.Value);

                                                string deptCodeCondition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index}"));
                                                string deptLineIdCondition = string.Join(" || ", deptLineIds.Select((_, index) => $"deptlineid = @{index + deptLineCodes.Count}"));
                                                string combinedCondition = $"({deptCodeCondition}) || ({deptLineIdCondition})";

                                                var parameters = new List<object>();
                                                parameters.AddRange(deptLineCodes.Cast<object>());
                                                parameters.AddRange(deptLineIds.Cast<object>());

                                                cbQuery = query.Where(combinedCondition, parameters.ToArray());
                                            }
                                            else
                                            {
                                                #region LEA
                                                deptLineCodes.Add(_userContext.DeptCode);
                                                deptLineIds.Add(_userContext.DeptG5Id.Value);

                                                //string condition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index}"));
                                                //cbQuery = query.Where(condition, deptLineCodes.ToArray());

                                                string deptCodeCondition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index}"));
                                                string deptIdCondition = $"deptid = @{deptLineCodes.Count}";
                                                string combinedCondition = $"({deptCodeCondition}) || ({deptIdCondition})";

                                                var parameters = new List<object>();
                                                parameters.AddRange(deptLineCodes.Cast<object>());
                                                parameters.Add(_userContext.DeptG5Id.Value);

                                                cbQuery = query.Where(combinedCondition, parameters.ToArray());
                                            }
                                            #endregion
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    if (typeof(ResignationApplication).IsAssignableFrom(typeof(T)) || typeof(MissingTimeClock).IsAssignableFrom(typeof(T)) || typeof(TargetPlan).IsAssignableFrom(typeof(T)))
                                    {
                                        // Logic mới - sử dụng deptId
                                        deptLineIds.Add(_userContext.DeptId);
                                        deptLineIds.Add(_userContext.DeptG5Id.Value);
                                        deptLineCodes.Add(_userContext.DeptCode);

                                        string deptIdCondition = string.Join(" || ", deptLineIds.Select((_, index) => $"deptid = @{index}"));
                                        string deptCodeCondition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index + deptLineIds.Count}"));

                                        var parameters = new List<object>();
                                        parameters.AddRange(deptLineIds.Cast<object>());
                                        parameters.AddRange(deptLineCodes.Cast<object>());

                                        string combinedCondition = $"({deptIdCondition}) || ({deptCodeCondition})";
                                        cbQuery = query.Where(combinedCondition, parameters.ToArray());
                                    }
                                    else
                                    {
                                        if (typeof(T) == typeof(BusinessTripOverBudget) || typeof(T) == typeof(BusinessTripApplication))
                                        {
                                            deptLineCodes.Add(_userContext.DeptCode);
                                            deptLineIds.Add(_userContext.DeptId);
                                            deptLineIds.Add(_userContext.DeptG5Id.Value);

                                            string deptCodeCondition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index}"));
                                            string deptLineIdCondition = string.Join(" || ", deptLineIds.Select((_, index) => $"deptlineid = @{index + deptLineCodes.Count}"));
                                            string combinedCondition = $"({deptCodeCondition}) || ({deptLineIdCondition})";

                                            var parameters = new List<object>();
                                            parameters.AddRange(deptLineCodes.Cast<object>());
                                            parameters.AddRange(deptLineIds.Cast<object>());

                                            cbQuery = query.Where(combinedCondition, parameters.ToArray());
                                        }
                                        else
                                        {

                                            //cbQuery = query.Where("deptcode=@0", _userContext.DeptCode);
                                            //deptLineCodes.Add(_userContext.DeptCode);
                                            //string condition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index}"));
                                            //cbQuery = query.Where(condition, deptLineCodes.ToArray());

                                            deptLineCodes.Add(_userContext.DeptCode);
                                            deptLineIds.Add(_userContext.DeptG5Id.Value);

                                            string deptCodeCondition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index}"));
                                            string deptIdCondition = $"deptid = @{deptLineCodes.Count}";
                                            string combinedCondition = $"({deptCodeCondition}) || ({deptIdCondition})";

                                            var parameters = new List<object>();
                                            parameters.AddRange(deptLineCodes.Cast<object>());
                                            parameters.Add(_userContext.DeptG5Id.Value);

                                            cbQuery = query.Where(combinedCondition, parameters.ToArray());
                                        }
                                    }
                                }
                            }
                        }

                        IQueryable<T> hrQuery = null;
                        if (!string.IsNullOrEmpty(_userContext.DeptCode) && !_userContext.IsHQ && ((_userContext.CurrentUserRole & UserRole.HR) == UserRole.HR && typeof(IRecruimentEntity).IsAssignableFrom(typeof(T)) && typeof(WorkflowEntity).IsAssignableFrom(typeof(T))))
                        {
                            //Fix for special case, promotion required two hr store to check
                            if (typeof(T) == typeof(PromoteAndTransfer))
                            {
                                // hrQuery = query.Where("deptcode=@0 || targetDeptCode=@0", _userContext.DeptCode);
                                deptLineCodes.Add(_userContext.DeptCode);
                                deptLineIds.Add(_userContext.DeptG5Id.Value);

                                //string condition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index} || targetDeptCode = @{index}"));
                                //cbQuery = query.Where(condition, deptLineCodes.ToArray());

                                string deptCodeCondition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index} || targetDeptCode = @{index}"));
                                string deptIdCondition = $"deptid = @{deptLineCodes.Count}";
                                string combinedCondition = $"({deptCodeCondition}) || ({deptIdCondition})";

                                var parameters = new List<object>();
                                parameters.AddRange(deptLineCodes.Cast<object>());
                                parameters.Add(_userContext.DeptG5Id.Value);

                                cbQuery = query.Where(combinedCondition, parameters.ToArray());
                            }
                            else
                            {
                                //hrQuery = query.Where("deptcode=@0", _userContext.DeptCode);
                                #region REQ
                                deptLineCodes.Add(_userContext.DeptCode);
                                deptLineIds.Add(_userContext.DeptG5Id.Value);

                                //string condition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index}"));
                                //hrQuery = query.Where(condition, deptLineCodes.ToArray());

                                string deptCodeCondition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index}"));
                                string deptIdCondition = $"deptid = @{deptLineCodes.Count}";
                                string combinedCondition = $"({deptCodeCondition}) || ({deptIdCondition})";

                                var parameters = new List<object>();
                                parameters.AddRange(deptLineCodes.Cast<object>());
                                parameters.Add(_userContext.DeptG5Id.Value);

                                hrQuery = query.Where(combinedCondition, parameters.ToArray());
                                #endregion
                            }
                        }

                        // fix 2 case for HR Task #9913
                        //Fix for special case, promotion required two hr store to check
                        if (!string.IsNullOrEmpty(_userContext.DeptCode) && !_userContext.IsHQ && ((_userContext.CurrentUserRole & UserRole.HR) == UserRole.HR && typeof(WorkflowEntity).IsAssignableFrom(typeof(T))))
                        {
                            if ((typeof(LeaveApplication).IsAssignableFrom(typeof(T)) || typeof(MissingTimeClock).IsAssignableFrom(typeof(T)) || typeof(ResignationApplication).IsAssignableFrom(typeof(T))
                                /*|| typeof(OvertimeApplication).IsAssignableFrom(typeof(T))*/ || typeof(TargetPlan).IsAssignableFrom(typeof(T))) && !string.IsNullOrEmpty(_userContext.CurrentUserName))
                            {
                                try
                                {
                                    string user = ConfigurationManager.AppSettings["FixUserForHR_U1"] != null ? ConfigurationManager.AppSettings["FixUserForHR_U1"] : "ngoan.tranthi";
                                    if (user.Equals(_userContext.CurrentUserName))
                                    {
                                        string departmentCode = ConfigurationManager.AppSettings["FixUserForHR_U1_DC1"] != null ? ConfigurationManager.AppSettings["FixUserForHR_U1_DC1"] : "40004566";
                                        //hrQuery = query.Where("deptcode=@0 || deptcode=@1", _userContext.DeptCode, departmentCode);
                                        if (typeof(ResignationApplication).IsAssignableFrom(typeof(T)) ||
                                            typeof(MissingTimeClock).IsAssignableFrom(typeof(T)) ||
                                            typeof(TargetPlan).IsAssignableFrom(typeof(T))
                                            )
                                        {
                                            // Logic mới - sử dụng deptId
                                            deptLineIds.Add(_userContext.DeptId);
                                            deptLineIds.Add(_userContext.DeptG5Id.Value);
                                            deptLineIds.Add(new Guid("C5B6EC5E-8918-4F4A-B9FC-1E6C6BE381E2"));//deptId của 40004566

                                            deptLineCodes.Add(_userContext.DeptCode);
                                            deptLineCodes.Add(departmentCode);

                                            string deptIdCondition = string.Join(" || ", deptLineIds.Select((_, index) => $"deptid = @{index}"));
                                            string deptCodeCondition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index + deptLineIds.Count}"));

                                            var parameters = new List<object>();
                                            parameters.AddRange(deptLineIds.Cast<object>());
                                            parameters.AddRange(deptLineCodes.Cast<object>());

                                            string combinedCondition = $"({deptIdCondition}) || ({deptCodeCondition})";
                                            hrQuery = query.Where(combinedCondition, parameters.ToArray());
                                        }
                                        else
                                        {
                                            deptLineCodes.Add(_userContext.DeptCode);
                                            deptLineCodes.Add(departmentCode);
                                            deptLineIds.Add(_userContext.DeptG5Id.Value);

                                            //string condition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index}"));
                                            //hrQuery = query.Where(condition, deptLineCodes.ToArray());

                                            string deptCodeCondition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index}"));
                                            string deptIdCondition = $"deptid = @{deptLineCodes.Count}";
                                            string combinedCondition = $"({deptCodeCondition}) || ({deptIdCondition})";

                                            var parameters = new List<object>();
                                            parameters.AddRange(deptLineCodes.Cast<object>());
                                            parameters.Add(_userContext.DeptG5Id.Value);

                                            hrQuery = query.Where(combinedCondition, parameters.ToArray());
                                        }
                                    }
                                    else if (_userContext.CurrentUserId.ToString().Equals("50c4c218-0175-49b2-a704-6c2ea05f7e16") || _userContext.CurrentUserId.ToString().Equals("20c82afd-bd1e-41db-8ed0-db8bba2ecdca"))
                                    {
                                        //hrQuery = query.Where("deptcode=@0 || deptcode=@1 || deptcode=@2", _userContext.DeptCode, "DSM_LB", "40000068");
                                        if (typeof(ResignationApplication).IsAssignableFrom(typeof(T)) ||
                                            typeof(MissingTimeClock).IsAssignableFrom(typeof(T)) ||
                                            typeof(TargetPlan).IsAssignableFrom(typeof(T))
                                            )
                                        {
                                            // Logic mới - sử dụng deptId
                                            deptLineIds.Add(_userContext.DeptId);
                                            deptLineIds.Add(_userContext.DeptG5Id.Value);
                                            deptLineIds.Add(new Guid("0CE0585A-8EC9-4F44-A6EF-ECB237E7C315"));//deptId của DSM_LB
                                            deptLineIds.Add(new Guid("48857B53-8DBB-439D-84CF-E39EA42BC8CD"));//deptId của 40000068

                                            deptLineCodes.Add(_userContext.DeptCode);
                                            deptLineCodes.Add("DSM_LB");
                                            deptLineCodes.Add("40000068");

                                            string deptIdCondition = string.Join(" || ", deptLineIds.Select((_, index) => $"deptid = @{index}"));
                                            string deptCodeCondition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index + deptLineIds.Count}"));

                                            var parameters = new List<object>();
                                            parameters.AddRange(deptLineIds.Cast<object>());
                                            parameters.AddRange(deptLineCodes.Cast<object>());

                                            string combinedCondition = $"({deptIdCondition}) || ({deptCodeCondition})";
                                            hrQuery = query.Where(combinedCondition, parameters.ToArray());
                                        }
                                        else
                                        {
                                            deptLineCodes.Add(_userContext.DeptCode);
                                            deptLineCodes.Add("DSM_LB");
                                            deptLineCodes.Add("40000068");
                                            deptLineIds.Add(_userContext.DeptG5Id.Value);

                                            //string condition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index}"));
                                            //hrQuery = query.Where(condition, deptLineCodes.ToArray());

                                            string deptCodeCondition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index}"));
                                            string deptIdCondition = $"deptid = @{deptLineCodes.Count}";
                                            string combinedCondition = $"({deptCodeCondition}) || ({deptIdCondition})";

                                            var parameters = new List<object>();
                                            parameters.AddRange(deptLineCodes.Cast<object>());
                                            parameters.Add(_userContext.DeptG5Id.Value);

                                            hrQuery = query.Where(combinedCondition, parameters.ToArray());
                                        }
                                    }
                                    else
                                    {
                                        var userNewHardCode_HR1392 = ConfigurationManager.AppSettings["HR1392_USER_HR"] != null ? ConfigurationManager.AppSettings["HR1392_USER_HR"].Split(',').ToList() : "9DAAF7CC-B535-4E2F-BB9B-B1BEEBD0E8BE,82075A0F-6E69-4DFE-A4B6-F2383E7F3DAC".Split(',').ToList();
                                        if (userNewHardCode_HR1392.Contains(_userContext.CurrentUserId.ToString().ToUpper()))
                                        {
                                            // HR 1932 // 50021419	HAI PHONG (G5)  HP_DSM	HAI PHONG DSM (APPROVER)
                                            string HaiPhongG5Code = ConfigurationManager.AppSettings["HR1392_DEPT_HAI_PHONG_G5_CODE"] != null ? ConfigurationManager.AppSettings["HR1392_DEPT_HAI_PHONG_G5_CODE"] : "50021419";
                                            string HaiPhongDsmApproverCode = ConfigurationManager.AppSettings["HR1392_DEPT_HAI_PHONG_DSM_APPROVER_CODE"] != null ? ConfigurationManager.AppSettings["HR1392_DEPT_HAI_PHONG_DSM_APPROVER_CODE"] : "HP_DSM";
                                            //hrQuery = query.Where("deptcode=@0 || deptcode=@1 || deptcode=@2", _userContext.DeptCode, HaiPhongG5Code, HaiPhongDsmApproverCode);

                                            if (typeof(ResignationApplication).IsAssignableFrom(typeof(T)) ||
                                                typeof(MissingTimeClock).IsAssignableFrom(typeof(T)) ||
                                                typeof(TargetPlan).IsAssignableFrom(typeof(T))
                                                )
                                            {
                                                // Logic mới - sử dụng deptId
                                                deptLineIds.Add(_userContext.DeptId);
                                                deptLineIds.Add(_userContext.DeptG5Id.Value);
                                                deptLineIds.Add(new Guid("9F2DB1C6-7365-4976-ACE0-954F9D329027"));//deptId của 50021419
                                                deptLineIds.Add(new Guid("D9ABE4FC-D6DD-4768-A0ED-CD666E4A4A93"));//deptId của HP_DSM

                                                deptLineCodes.Add(_userContext.DeptCode);
                                                deptLineCodes.Add(HaiPhongG5Code);
                                                deptLineCodes.Add(HaiPhongDsmApproverCode);

                                                string deptIdCondition = string.Join(" || ", deptLineIds.Select((_, index) => $"deptid = @{index}"));
                                                string deptCodeCondition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index + deptLineIds.Count}"));

                                                var parameters = new List<object>();
                                                parameters.AddRange(deptLineIds.Cast<object>());
                                                parameters.AddRange(deptLineCodes.Cast<object>());

                                                string combinedCondition = $"({deptIdCondition}) || ({deptCodeCondition})";
                                                hrQuery = query.Where(combinedCondition, parameters.ToArray());
                                            }
                                            else
                                            {
                                                deptLineCodes.Add(_userContext.DeptCode);
                                                deptLineCodes.Add(HaiPhongG5Code);
                                                deptLineCodes.Add(HaiPhongDsmApproverCode);
                                                deptLineIds.Add(_userContext.DeptG5Id.Value);

                                                //string condition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index}"));
                                                //hrQuery = query.Where(condition, deptLineCodes.ToArray());

                                                string deptCodeCondition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index}"));
                                                string deptIdCondition = $"deptid = @{deptLineCodes.Count}";
                                                string combinedCondition = $"({deptCodeCondition}) || ({deptIdCondition})";

                                                var parameters = new List<object>();
                                                parameters.AddRange(deptLineCodes.Cast<object>());
                                                parameters.Add(_userContext.DeptG5Id.Value);

                                                hrQuery = query.Where(combinedCondition, parameters.ToArray());
                                            }
                                        }
                                        else if (_userContext.CurrentUserId.ToString().Equals("50c4c218-0175-49b2-a704-6c2ea05f7e16") || _userContext.CurrentUserId.ToString().Equals("20c82afd-bd1e-41db-8ed0-db8bba2ecdca"))
                                        {
                                            // hrQuery = query.Where("deptlineid=@0 || deptlineid=@1 || deptlineid=@2", _userContext.DeptId, new Guid("48857b53-8dbb-439d-84cf-e39ea42bc8cd"), new Guid("0ce0585a-8ec9-4f44-a6ef-ecb237e7c315"));

                                            if (typeof(ResignationApplication).IsAssignableFrom(typeof(T)) ||
                                                typeof(MissingTimeClock).IsAssignableFrom(typeof(T)) ||
                                                typeof(TargetPlan).IsAssignableFrom(typeof(T))
                                                )
                                            {
                                                // Logic mới - sử dụng deptId
                                                deptLineIds.Add(_userContext.DeptId);
                                                deptLineIds.Add(_userContext.DeptG5Id.Value);
                                                deptLineIds.Add(new Guid("48857b53-8dbb-439d-84cf-e39ea42bc8cd"));
                                                deptLineIds.Add(new Guid("0ce0585a-8ec9-4f44-a6ef-ecb237e7c315"));

                                                deptLineCodes.Add(_userContext.DeptCode);

                                                string deptIdCondition = string.Join(" || ", deptLineIds.Select((_, index) => $"deptid = @{index}"));
                                                string deptCodeCondition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index + deptLineIds.Count}"));

                                                var parameters = new List<object>();
                                                parameters.AddRange(deptLineIds.Cast<object>());
                                                parameters.AddRange(deptLineCodes.Cast<object>());

                                                string combinedCondition = $"({deptIdCondition}) || ({deptCodeCondition})";
                                                hrQuery = query.Where(combinedCondition, parameters.ToArray());
                                            }
                                            else
                                            {
                                                deptLineIds.Add(_userContext.DeptId);
                                                deptLineIds.Add(_userContext.DeptG5Id.Value);
                                                deptLineIds.Add(new Guid("48857b53-8dbb-439d-84cf-e39ea42bc8cd"));
                                                deptLineIds.Add(new Guid("0ce0585a-8ec9-4f44-a6ef-ecb237e7c315"));

                                                //string condition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptlineid = @{index}"));
                                                //hrQuery = query.Where(condition, deptLineCodes.ToArray());

                                                string condition = string.Join(" || ", deptLineIds.Select((_, index) => $"deptlineid = @{index}"));
                                                hrQuery = query.Where(condition, deptLineIds.ToArray());
                                            }
                                        }
                                    }
                                }
                                catch (Exception e) { }
                            }
                            else if ((typeof(ShiftExchangeApplication).IsAssignableFrom(typeof(T)) && !string.IsNullOrEmpty(_userContext.CurrentUserName)))
                            {
                                try
                                {
                                    string user = ConfigurationManager.AppSettings["FixUserForHR_U1"] != null ? ConfigurationManager.AppSettings["FixUserForHR_U1"] : "ngoan.tranthi";
                                    if (user.Equals(_userContext.CurrentUserName))
                                    {
                                        string departmentId = ConfigurationManager.AppSettings["FixUserForHR_U1_DI1"] != null ? ConfigurationManager.AppSettings["FixUserForHR_U1_DI1"] : "40004566";
                                        // hrQuery = query.Where("deptlineid=@0 || deptlineid=@1", _userContext.DeptId, new Guid(departmentId));
                                        deptLineIds.Add(_userContext.DeptId);
                                        deptLineIds.Add(new Guid(departmentId));
                                        deptLineIds.Add(_userContext.DeptG5Id.Value);
                                        //string condition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptlineid = @{index}"));
                                        //hrQuery = query.Where(condition, deptLineCodes.ToArray());
                                        string condition = string.Join(" || ", deptLineIds.Select((_, index) => $"deptlineid = @{index}"));
                                        hrQuery = query.Where(condition, deptLineIds.ToArray());

                                    }
                                    else if (_userContext.CurrentUserId.ToString().Equals("50c4c218-0175-49b2-a704-6c2ea05f7e16") || _userContext.CurrentUserId.ToString().Equals("20c82afd-bd1e-41db-8ed0-db8bba2ecdca"))
                                    {
                                        // hrQuery = query.Where("deptlineid=@0 || deptlineid=@1 || deptlineid=@2", _userContext.DeptId, new Guid("48857b53-8dbb-439d-84cf-e39ea42bc8cd"), new Guid("0ce0585a-8ec9-4f44-a6ef-ecb237e7c315"));
                                        deptLineIds.Add(_userContext.DeptId);
                                        deptLineIds.Add(new Guid("48857b53-8dbb-439d-84cf-e39ea42bc8cd"));
                                        deptLineIds.Add(new Guid("0ce0585a-8ec9-4f44-a6ef-ecb237e7c315"));
                                        deptLineIds.Add(_userContext.DeptG5Id.Value);
                                        //string condition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptlineid = @{index}"));
                                        //hrQuery = query.Where(condition, deptLineCodes.ToArray());
                                        string condition = string.Join(" || ", deptLineIds.Select((_, index) => $"deptlineid = @{index}"));
                                        hrQuery = query.Where(condition, deptLineIds.ToArray());
                                    }
                                    else
                                    {
                                        var userNewHardCode_HR1392 = ConfigurationManager.AppSettings["HR1392_USER_HR"] != null ? ConfigurationManager.AppSettings["HR1392_USER_HR"].Split(',').ToList() : "9DAAF7CC-B535-4E2F-BB9B-B1BEEBD0E8BE,82075A0F-6E69-4DFE-A4B6-F2383E7F3DAC".Split(',').ToList();
                                        if (userNewHardCode_HR1392.Contains(_userContext.CurrentUserId.ToString().ToUpper()))
                                        {
                                            // HR 1932 // 50021419	HAI PHONG (G5)  HP_DSM	HAI PHONG DSM (APPROVER)
                                            string HaiPhongG5Id = ConfigurationManager.AppSettings["HR1392_DEPT_HAI_PHONG_G5_ID"] != null ? ConfigurationManager.AppSettings["HR1392_DEPT_HAI_PHONG_G5_ID"] : "9F2DB1C6-7365-4976-ACE0-954F9D329027";
                                            string HaiPhongDsmApproverId = ConfigurationManager.AppSettings["HR1392_DEPT_HAI_PHONG_DSM_APPROVER_ID"] != null ? ConfigurationManager.AppSettings["HR1392_DEPT_HAI_PHONG_DSM_APPROVER_ID"] : "D9ABE4FC-D6DD-4768-A0ED-CD666E4A4A93";
                                            // hrQuery = query.Where("deptlineid=@0 || deptlineid=@1 || deptlineid=@2", _userContext.DeptId, new Guid(HaiPhongG5Id), new Guid(HaiPhongDsmApproverId));
                                            deptLineIds.Add(_userContext.DeptId);
                                            deptLineIds.Add(new Guid(HaiPhongG5Id));
                                            deptLineIds.Add(new Guid(HaiPhongDsmApproverId));
                                            deptLineIds.Add(_userContext.DeptG5Id.Value);
                                            //string condition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptlineid = @{index}"));
                                            //hrQuery = query.Where(condition, deptLineCodes.ToArray());
                                            string condition = string.Join(" || ", deptLineIds.Select((_, index) => $"deptlineid = @{index}"));
                                            hrQuery = query.Where(condition, deptLineIds.ToArray());
                                        }
                                    }
                                }
                                catch (Exception e) { }
                            }
                            else if (typeof(OvertimeApplication).IsAssignableFrom(typeof(T)) && !string.IsNullOrEmpty(_userContext.CurrentUserName))
                            {
                                try
                                {
                                    string user = ConfigurationManager.AppSettings["FixUserForHR_U1"] != null ? ConfigurationManager.AppSettings["FixUserForHR_U1"] : "ngoan.tranthi";
                                    if (user.Equals(_userContext.CurrentUserName))
                                    {
                                        string departmentCode = ConfigurationManager.AppSettings["FixUserForHR_U1_DC1"] != null ? ConfigurationManager.AppSettings["FixUserForHR_U1_DC1"] : "40004566";
                                        deptLineCodes.Add(_userContext.DeptCode);
                                        deptLineCodes.Add(departmentCode);
                                        /*string condition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index}")) + $" || deptid = @{deptLineCodes.Count}";
                                        //hrQuery = query.Where(condition, deptLineCodes.ToArray());
                                        hrQuery = query.Where(condition, deptLineCodes.Cast<object>().Concat(new object[] { _userContext.DeptId }).ToArray());*/

                                        string condition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index}")) + $" || deptid = @{deptLineCodes.Count} || deptid = @{deptLineCodes.Count + 1}";
                                        hrQuery = query.Where(condition, deptLineCodes.Cast<object>().Concat(new object[] { _userContext.DeptId, _userContext.DeptG5Id.Value }).ToArray());

                                    }
                                    else if (_userContext.CurrentUserId.ToString().Equals("50c4c218-0175-49b2-a704-6c2ea05f7e16") || _userContext.CurrentUserId.ToString().Equals("20c82afd-bd1e-41db-8ed0-db8bba2ecdca"))
                                    {
                                        //hrQuery = query.Where("deptcode=@0 || deptcode=@1 || deptcode=@2", _userContext.DeptCode, "DSM_LB", "40000068");
                                        deptLineCodes.Add(_userContext.DeptCode);
                                        deptLineCodes.Add("DSM_LB");
                                        deptLineCodes.Add("40000068");
                                        //string condition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index}")) + $" || deptid = @{deptLineCodes.Count}";
                                        //hrQuery = query.Where(condition, deptLineCodes.Cast<object>().Concat(new object[] { _userContext.DeptId }).ToArray());

                                        string condition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index}")) + $" || deptid = @{deptLineCodes.Count} || deptid = @{deptLineCodes.Count + 1}";
                                        hrQuery = query.Where(condition, deptLineCodes.Cast<object>().Concat(new object[] { _userContext.DeptId, _userContext.DeptG5Id.Value }).ToArray());
                                    }
                                    else
                                    {
                                        var userNewHardCode_HR1392 = ConfigurationManager.AppSettings["HR1392_USER_HR"] != null ? ConfigurationManager.AppSettings["HR1392_USER_HR"].Split(',').ToList() : "9DAAF7CC-B535-4E2F-BB9B-B1BEEBD0E8BE,82075A0F-6E69-4DFE-A4B6-F2383E7F3DAC".Split(',').ToList();
                                        if (userNewHardCode_HR1392.Contains(_userContext.CurrentUserId.ToString().ToUpper()))
                                        {
                                            // HR 1932 // 50021419	HAI PHONG (G5)  HP_DSM	HAI PHONG DSM (APPROVER)
                                            string HaiPhongG5Code = ConfigurationManager.AppSettings["HR1392_DEPT_HAI_PHONG_G5_CODE"] != null ? ConfigurationManager.AppSettings["HR1392_DEPT_HAI_PHONG_G5_CODE"] : "50021419";
                                            string HaiPhongDsmApproverCode = ConfigurationManager.AppSettings["HR1392_DEPT_HAI_PHONG_DSM_APPROVER_CODE"] != null ? ConfigurationManager.AppSettings["HR1392_DEPT_HAI_PHONG_DSM_APPROVER_CODE"] : "HP_DSM";
                                            deptLineCodes.Add(_userContext.DeptCode);
                                            deptLineCodes.Add(HaiPhongG5Code);
                                            deptLineCodes.Add(HaiPhongDsmApproverCode);
                                            //string condition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index}")) + $" || deptid = @{deptLineCodes.Count}";
                                            //hrQuery = query.Where(condition, deptLineCodes.Cast<object>().Concat(new object[] { _userContext.DeptId }).ToArray());

                                            string condition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index}")) + $" || deptid = @{deptLineCodes.Count} || deptid = @{deptLineCodes.Count + 1}";
                                            hrQuery = query.Where(condition, deptLineCodes.Cast<object>().Concat(new object[] { _userContext.DeptId, _userContext.DeptG5Id.Value }).ToArray());
                                        }

                                        //else if này chắc chắn ko vào
                                        /*
                                        else if (_userContext.CurrentUserId.ToString().Equals("50c4c218-0175-49b2-a704-6c2ea05f7e16") || _userContext.CurrentUserId.ToString().Equals("20c82afd-bd1e-41db-8ed0-db8bba2ecdca"))
                                        {
                                            deptLineIds.Add(_userContext.DeptId);
                                            deptLineIds.Add(new Guid("48857b53-8dbb-439d-84cf-e39ea42bc8cd"));
                                            deptLineIds.Add(new Guid("0ce0585a-8ec9-4f44-a6ef-ecb237e7c315"));
                                            string condition = string.Join(" || ", deptLineIds.Select((_, index) => $"deptlineid = @{index}")) + $" || deptid = @{deptLineIds.Count}";
                                            hrQuery = query.Where(condition, deptLineCodes.Cast<object>().Concat(new object[] { _userContext.DeptId }).ToArray());
                                        }
                                        */
                                    }
                                }
                                catch (Exception e) { }
                            }
                        }

                        // Special Case
                        if (!(_userContext is null) && (_userContext.CurrentUserRole & UserRole.HR) == UserRole.HR && !_userContext.IsHQ && typeof(ResignationApplication).IsAssignableFrom(typeof(T)))
                        {
                            //query = query.Where("deptcode=@0", _userContext.DeptCode);

                            //deptLineCodes.Add(_userContext.DeptCode);
                            //string condition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index}"));
                            //query = query.Where(condition, deptLineCodes.ToArray());

                            // Logic mới - sử dụng deptId
                            deptLineIds.Add(_userContext.DeptId);
                            deptLineIds.Add(_userContext.DeptG5Id.Value);
                            deptLineCodes.Add(_userContext.DeptCode);

                            string deptIdCondition = string.Join(" || ", deptLineIds.Select((_, index) => $"deptid = @{index}"));
                            string deptCodeCondition = string.Join(" || ", deptLineCodes.Select((_, index) => $"deptcode = @{index + deptLineIds.Count}"));

                            var parameters = new List<object>();
                            parameters.AddRange(deptLineIds.Cast<object>());
                            parameters.AddRange(deptLineCodes.Cast<object>());

                            string combinedCondition = $"({deptIdCondition}) || ({deptCodeCondition})";
                            query = query.Where(combinedCondition, parameters.ToArray());
                        }

                        else
                        {
                            //Check user based on the permission
                            query = query
                            .GroupJoin(_context.Set<Permission>().AsQueryable(), t => t.Id, p => p.ItemId, (t, p) => new { t, p })
                            .Where(x => x.p.Any(u => u.UserId == _userContext.CurrentUserId) || x.p.Any(d => d.Department.UserDepartmentMappings.Any(u => u.UserId == _userContext.CurrentUserId && u.Role == d.DepartmentType && !u.IsDeleted)))
                            .Select(x => x.t);
                        }

                        //Check user based on the permission

                        if (cbQuery != null)
                        {
                            query = query.Union(cbQuery);
                        }
                        if (hrQuery != null)
                        {
                            query = query.Union(hrQuery);
                        }
                    }
                }

                if (typeof(User).IsAssignableFrom(typeof(T)) || typeof(Department).IsAssignableFrom(typeof(T)) || typeof(UserDepartmentMapping).IsAssignableFrom(typeof(T)))
                {
                    query = query.Where("IsFromIT=@0", new object[] { false });
                }

                if (typeof(ISoftDeleteEntity).IsAssignableFrom(typeof(T)))
                {
                    query = query.Where("IsDeleted=@0", new object[] { false });
                }

                return query;
            }
        }

        private IQueryable<T> ITQuery
        {
            get
            {
                var query = _context.Set<T>().AsQueryable();

                var allowViewAll = false;
                List<Type> allowViewAllType = new List<Type>();
                allowViewAllType.Add(typeof(ResignationApplication));
                allowViewAllType.Add(typeof(PromoteAndTransfer));
                allowViewAllType.Add(typeof(Acting));
                allowViewAllType.Add(typeof(RequestToHire));
                allowViewAllType.Add(typeof(BusinessTripOverBudget));

                if (allowViewAllType.Contains(typeof(T)) && !(_userContext is null) && (_userContext.CurrentUserRole & UserRole.HR) == UserRole.HR)
                {
                    //Fix for special case, promotion required two hr store to check
                    if (_userContext.IsHQ)
                    {
                        allowViewAll = true;
                    }
                }
                // Allow Accounting role to view all BusinessTripOverBudget (BOB) forms
                if (typeof(T) == typeof(BusinessTripOverBudget) && !(_userContext is null) && (_userContext.CurrentUserRole & UserRole.Accounting) == UserRole.Accounting)
                {
                    allowViewAll = true;
                }
                if (!_forceAllItems && !allowViewAll)
                {
                    if (typeof(IPermission).IsAssignableFrom(typeof(T)))
                    {
                        IQueryable<T> cbQuery = null;
                        if (!string.IsNullOrEmpty(_userContext.DeptCode) && !_userContext.IsHQ && ((_userContext.CurrentUserRole & UserRole.CB) == UserRole.CB && typeof(ICBEntity).IsAssignableFrom(typeof(T)) && typeof(WorkflowEntity).IsAssignableFrom(typeof(T))))
                        {
                            //Fix for special case, shift exchange doesn't store deptcode and deptname, store deptid instead
                            if (typeof(T) == typeof(ShiftExchangeApplication))
                            {
                                try
                                {
                                    string user = ConfigurationManager.AppSettings["FixUserForHR_U2"] != null ? ConfigurationManager.AppSettings["FixUserForHR_U2"] : "413759";
                                    if (!string.IsNullOrEmpty(_userContext.CurrentUserName) && user.Equals(_userContext.CurrentUserName))
                                    {
                                        string departmentId = ConfigurationManager.AppSettings["FixUserForHR_U1_DI1"] != null ? ConfigurationManager.AppSettings["FixUserForHR_U1_DI1"] : "40004566";
                                        cbQuery = query.Where("deptlineid=@0 || deptlineid=@1", _userContext.DeptId, new Guid(departmentId));
                                    }
                                    else
                                    {
                                        cbQuery = query.Where("deptlineid=@0", _userContext.DeptId);
                                    }
                                }
                                catch (Exception e)
                                {
                                    cbQuery = query.Where("deptlineid=@0", _userContext.DeptId);
                                }
                            }
                            else
                            {
                                // fix 2 case for HR Task #9913
                                try
                                {
                                    string user = ConfigurationManager.AppSettings["FixUserForHR_U2"] != null ? ConfigurationManager.AppSettings["FixUserForHR_U2"] : "413759";
                                    if ((typeof(LeaveApplication).IsAssignableFrom(typeof(T)) || typeof(MissingTimeClock).IsAssignableFrom(typeof(T)) || typeof(ResignationApplication).IsAssignableFrom(typeof(T))
                                    || typeof(OvertimeApplication).IsAssignableFrom(typeof(T)) || typeof(TargetPlan).IsAssignableFrom(typeof(T))) && !string.IsNullOrEmpty(_userContext.CurrentUserName) && user.Equals(_userContext.CurrentUserName))
                                    {
                                        string departmentCode = ConfigurationManager.AppSettings["FixUserForHR_U1_DC1"] != null ? ConfigurationManager.AppSettings["FixUserForHR_U1_DC1"] : "40004566";
                                        cbQuery = query.Where("deptcode=@0 || deptcode=@1", _userContext.DeptCode, departmentCode);
                                    }
                                    else if ((typeof(LeaveApplication).IsAssignableFrom(typeof(T)) || typeof(MissingTimeClock).IsAssignableFrom(typeof(T)) || typeof(ResignationApplication).IsAssignableFrom(typeof(T))
                                    || typeof(OvertimeApplication).IsAssignableFrom(typeof(T)) || typeof(TargetPlan).IsAssignableFrom(typeof(T))) && _userContext.CurrentUserId.ToString().Equals((("01dc17f9-a2ab-4ed8-8526-fcf34338a5fd")).ToString().ToLower()))
                                    {
                                        cbQuery = query.Where("deptcode=@0 || deptcode=@1 || deptcode=@2", _userContext.DeptCode, "DSM_LB", "40000068");
                                    }
                                    else
                                    {
                                        cbQuery = query.Where("deptcode=@0", _userContext.DeptCode);
                                    }
                                }
                                catch (Exception e)
                                {
                                    cbQuery = query.Where("deptcode=@0", _userContext.DeptCode);
                                }
                            }
                        }

                        IQueryable<T> hrQuery = null;
                        if (!string.IsNullOrEmpty(_userContext.DeptCode) && !_userContext.IsHQ && ((_userContext.CurrentUserRole & UserRole.HR) == UserRole.HR && typeof(IRecruimentEntity).IsAssignableFrom(typeof(T)) && typeof(WorkflowEntity).IsAssignableFrom(typeof(T))))
                        {
                            //Fix for special case, promotion required two hr store to check
                            if (typeof(T) == typeof(PromoteAndTransfer))
                            {
                                hrQuery = query.Where("deptcode=@0 || targetDeptCode=@0", _userContext.DeptCode);
                            }
                            else
                            {
                                hrQuery = query.Where("deptcode=@0", _userContext.DeptCode);
                            }
                        }

                        // fix 2 case for HR Task #9913
                        //Fix for special case, promotion required two hr store to check
                        if (!string.IsNullOrEmpty(_userContext.DeptCode) && !_userContext.IsHQ && ((_userContext.CurrentUserRole & UserRole.HR) == UserRole.HR && typeof(WorkflowEntity).IsAssignableFrom(typeof(T))))
                        {
                            if ((typeof(LeaveApplication).IsAssignableFrom(typeof(T)) || typeof(MissingTimeClock).IsAssignableFrom(typeof(T)) || typeof(ResignationApplication).IsAssignableFrom(typeof(T))
                                || typeof(OvertimeApplication).IsAssignableFrom(typeof(T)) || typeof(TargetPlan).IsAssignableFrom(typeof(T))) && !string.IsNullOrEmpty(_userContext.CurrentUserName))
                            {
                                try
                                {
                                    string user = ConfigurationManager.AppSettings["FixUserForHR_U1"] != null ? ConfigurationManager.AppSettings["FixUserForHR_U1"] : "ngoan.tranthi";
                                    if (user.Equals(_userContext.CurrentUserName))
                                    {
                                        string departmentCode = ConfigurationManager.AppSettings["FixUserForHR_U1_DC1"] != null ? ConfigurationManager.AppSettings["FixUserForHR_U1_DC1"] : "40004566";
                                        hrQuery = query.Where("deptcode=@0 || deptcode=@1", _userContext.DeptCode, departmentCode);
                                    }
                                    else if (_userContext.CurrentUserId.ToString().Equals("50c4c218-0175-49b2-a704-6c2ea05f7e16") || _userContext.CurrentUserId.ToString().Equals("20c82afd-bd1e-41db-8ed0-db8bba2ecdca"))
                                    {
                                        hrQuery = query.Where("deptcode=@0 || deptcode=@1 || deptcode=@2", _userContext.DeptCode, "DSM_LB", "40000068");
                                    }
                                }
                                catch (Exception e) { }
                            }
                            else if ((typeof(ShiftExchangeApplication).IsAssignableFrom(typeof(T)) && !string.IsNullOrEmpty(_userContext.CurrentUserName)))
                            {
                                try
                                {
                                    string user = ConfigurationManager.AppSettings["FixUserForHR_U1"] != null ? ConfigurationManager.AppSettings["FixUserForHR_U1"] : "ngoan.tranthi";
                                    if (user.Equals(_userContext.CurrentUserName))
                                    {
                                        string departmentId = ConfigurationManager.AppSettings["FixUserForHR_U1_DI1"] != null ? ConfigurationManager.AppSettings["FixUserForHR_U1_DI1"] : "40004566";
                                        hrQuery = query.Where("deptlineid=@0 || deptlineid=@1", _userContext.DeptId, new Guid(departmentId));
                                    }
                                    else if (_userContext.CurrentUserId.ToString().Equals("50c4c218-0175-49b2-a704-6c2ea05f7e16") || _userContext.CurrentUserId.ToString().Equals("20c82afd-bd1e-41db-8ed0-db8bba2ecdca"))
                                    {
                                        hrQuery = query.Where("deptlineid=@0 || deptlineid=@1 || deptlineid=@2", _userContext.DeptId, new Guid("48857b53-8dbb-439d-84cf-e39ea42bc8cd"), new Guid("0ce0585a-8ec9-4f44-a6ef-ecb237e7c315"));
                                    }
                                }
                                catch (Exception e) { }
                            }
                        }

                        // Special Case
                        if (!(_userContext is null) && (_userContext.CurrentUserRole & UserRole.HR) == UserRole.HR && !_userContext.IsHQ && typeof(ResignationApplication).IsAssignableFrom(typeof(T)))
                        {
                            /*query = query
                            .GroupJoin(_context.Set<Permission>().AsQueryable(), t => t.Id, p => p.ItemId, (t, p) => new { t, p })
                            .Where(x => x.p.Any(u => u.UserId == _userContext.CurrentUserId) || x.p.Any(d => d.Department.UserDepartmentMappings.Any(u => u.UserId == _userContext.CurrentUserId && (u.Role == d.DepartmentType || (Infrastructure.Enums.Group)u.Role == Infrastructure.Enums.Group.Member || (Infrastructure.Enums.Group)u.Role == Infrastructure.Enums.Group.Checker))))
                            .Select(x => x.t);*/
                            query = query.Where("deptcode=@0", _userContext.DeptCode);
                        }
                        else
                        {
                            //Check user based on the permission
                            query = query
                            .GroupJoin(_context.Set<Permission>().AsQueryable(), t => t.Id, p => p.ItemId, (t, p) => new { t, p })
                            .Where(x => x.p.Any(u => u.UserId == _userContext.CurrentUserId) || x.p.Any(d => d.Department.UserDepartmentMappings.Any(u => u.UserId == _userContext.CurrentUserId && u.Role == d.DepartmentType)))
                            .Select(x => x.t);
                        }

                        //Check user based on the permission

                        if (cbQuery != null)
                        {
                            query = query.Union(cbQuery);
                        }
                        if (hrQuery != null)
                        {
                            query = query.Union(hrQuery);
                        }
                    }
                }

                if (typeof(ISoftDeleteEntity).IsAssignableFrom(typeof(T)))
                {
                    query = query.Where("IsDeleted=@0", new object[] { false });
                }
                return query;
            }
        }

        private void AddPerm(T entity)
        {
            if (!(entity is WorkflowTask))
            {
                _context.Set<Permission>().Add(new Permission
                {
                    Created = DateTime.Now,
                    UserId = _userContext.CurrentUserId,
                    ItemId = entity.Id,
                    Modified = DateTime.Now,
                    Perm = Right.Full,
                    Id = Guid.NewGuid()
                });
            }
        }

        // Ham dung cho get cac record isdeleted = true
        public async Task<T> GetSingleAsyncIsNotDeleted(Expression<Func<T, bool>> predicate, string order = "", params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>().AsQueryable();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            query = query.OrderBy(order);
            if (predicate == null)
            {
                return await query.FirstOrDefaultAsync();
            }
            return await query.Where(predicate).FirstOrDefaultAsync();
        }

        #region Auto Number

        private string GenerateReferenceNumber(string name)
        {
            var refNumber = string.Empty;
            lock (GlobalData.Instance.GlobalLock)
            {
                if (_refs.ContainsKey(name))
                {
                    var refEnitty = _refs[name];


                    if (refEnitty.IsNewYearReset && refEnitty.CurrentYear != DateTime.Now.Year || refEnitty.CurrentYear == 0)
                    {
                        refEnitty.CurrentNumber = 1;
                        refEnitty.CurrentYear = DateTime.Now.Year;
                    }
                    else
                    {
                        try
                        {
                            int? maxCurrentNumber = _context.Database.SqlQuery<int?>("select MAX(CurrentNumber) from ReferenceNumbers where ModuleType = @p0 and CurrentYear = @p1", refEnitty.ModuleType, refEnitty.CurrentYear).FirstOrDefault();
                            if (maxCurrentNumber != null && maxCurrentNumber.HasValue)
                            {
                                refEnitty.CurrentNumber = (maxCurrentNumber.Value);
                                refEnitty.CurrentNumber++;
                            }
                        }
                        catch (Exception ex)
                        {
                            refEnitty.CurrentNumber++;
                        }
                    }
                    //Update via sql
                    _context.Database.ExecuteSqlCommand("Update [ReferenceNumbers] set CurrentNumber = @p0, CurrentYear =@p1 where Id= @p2", refEnitty.CurrentNumber, refEnitty.CurrentYear, refEnitty.Id);
                    refNumber = refEnitty.Formula;
                    var tokens = FindFieldTokens(refNumber);
                    foreach (var token in tokens)
                    {
                        switch (token.ToLower())
                        {
                            case "{year}":
                                refNumber = refNumber.Replace(token, refEnitty.CurrentYear.ToString());
                                break;
                            //For Autonumber field
                            default:
                                var tokenParts = token.Trim(new char[] { '{', '}' }).Split(new char[] { ':' });
                                if (tokenParts.Length > 1)
                                {
                                    refNumber = refNumber.Replace(token, refEnitty.CurrentNumber.ToString($"D{tokenParts[1]}"));
                                }
                                break;
                        }
                    }
                }
            }
            return refNumber;
        }

        public IEnumerable<string> FindTokens(string str, string pattern)
        {
            var regex = new Regex(pattern);
            var matches = regex.Matches(str);
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    yield return match.Value;
                }
            }
        }
        public IEnumerable<string> FindFieldTokens(string str)
        {
            var tokens = FindTokens(str, @"\{[\d\w\s\:]*\}");
            foreach (var token in tokens)
            {
                yield return token;
            }
        }


        #endregion

        public async Task<List<TResult>> ExecuteRawSqlQueryAsync<TResult>(string sql, params SqlParameter[] parameters) where TResult : class, new()
        {
            var connection = _context.Database.Connection;
            var result = new List<TResult>();

            try
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;

                    if (parameters != null && parameters.Length > 0)
                    {
                        foreach (var p in parameters)
                        {
                            command.Parameters.Add(p);
                        }
                    }

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var props = typeof(TResult).GetProperties();
                        var columnNames = new HashSet<string>(
                                    Enumerable.Range(0, reader.FieldCount).Select(reader.GetName),
                                    StringComparer.InvariantCultureIgnoreCase
                                );

                        while (await reader.ReadAsync())
                        {
                            var item = new TResult();

                            foreach (var prop in props)
                            {
                                if (!columnNames.Contains(prop.Name)) continue;
                                var value = reader[prop.Name];
                                if (value is DBNull) continue;

                                prop.SetValue(item, value);
                            }

                            result.Add(item);
                        }
                    }
                }
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return result;
        }
    }
}
