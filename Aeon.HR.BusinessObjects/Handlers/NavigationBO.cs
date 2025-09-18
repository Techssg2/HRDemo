using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Constants;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using static Aeon.HR.Data.Models.Navigation;

namespace Aeon.HR.BusinessObjects.Handlers
{
    public class NavigationBO : INavigationBO
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger _logger;

        public NavigationBO(IUnitOfWork uow, ILogger logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<ArrayResultDTO> GetListNavigation(QueryArgs args)
        {
            try
            {
                var items = await _uow.GetRepository<Navigation>().FindByAsync<NavigationViewModel>(args.Order, args.Page, args.Limit, args.Predicate, args.PredicateParameters);
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        item.CountChild = await _uow.GetRepository<Navigation>().CountAsync(x => x.ParentId == item.Id && !x.IsDeleted);
                    }
                }
                var count = await _uow.GetRepository<Navigation>().CountAsync(args.Predicate, args.PredicateParameters);
                var result = new ArrayResultDTO { Data = items, Count = count };
                return result;
            }
            catch (MembershipCreateUserException e)
            {
                throw e;
            }
        }

        public async Task<ArrayResultDTO> GetAll()
        {
            var result = new ArrayResultDTO { };
            try
            {
                var items = await _uow.GetRepository<Navigation>().FindByAsync<NavigationViewModel>(x => !x.IsDeleted);
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        if (item.ParentId != null)
                        {
                            item.CountChild = await _uow.GetRepository<Navigation>().CountAsync(x => x.ParentId == item.Id && !x.IsDeleted);
                        }
                    }
                }
                var count = await _uow.GetRepository<Navigation>().CountAsync(x => !x.IsDeleted);
                result = new ArrayResultDTO { Data = items, Count = count };
            }
            catch (MembershipCreateUserException e)
            {
                throw e;
            }
            return result;
        }

        public async Task<ArrayResultDTO> GetAllParentAndChildNavigation()
        {
            var result = new ArrayResultDTO { };
            try
            {
                var items = await _uow.GetRepository<Navigation>().FindByAsync<NavigationViewModel>(x => x.ParentId == null && !x.IsDeleted, "Priority asc");
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        var childItem = await _uow.GetRepository<Navigation>().FindByAsync<NavigationViewModel>(x => x.ParentId == item.Id && !x.IsDeleted);
                        if (childItem != null)
                        {
                            item.JsonChild = JsonConvert.SerializeObject(childItem);
                        }
                    }
                }
                var count = await _uow.GetRepository<Navigation>().CountAsync(x => x.ParentId == null && !x.IsDeleted);
                result = new ArrayResultDTO { Data = items, Count = count };
            }
            catch (MembershipCreateUserException e)
            {
                throw e;
            }
            return result;
        }

        public async Task<ArrayResultDTO> GetAllByType(List<NavigationType> types)
        {
            var result = new ArrayResultDTO { };
            try
            {
                var items = await _uow.GetRepository<Navigation>().FindByAsync<NavigationViewModel>(x => types.Contains(x.Type) && !x.IsDeleted);
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        if (item.ParentId != null)
                        {
                            item.CountChild = await _uow.GetRepository<Navigation>().CountAsync(x => x.ParentId == item.Id && !x.IsDeleted);
                        }
                    }
                }
                var count = await _uow.GetRepository<Navigation>().CountAsync(x => !x.IsDeleted);
                result = new ArrayResultDTO { Data = items, Count = count };
            }
            catch (MembershipCreateUserException e)
            {
                throw e;
            }
            return result;
        }

        public async Task<ArrayResultDTO> GetListNavigationByUserIdAndDepartmentId()
        {
            var result = new ArrayResultDTO { };
            List<NavigationViewModel> items = new List<NavigationViewModel>();
            try
            {
                // args.DepartmentId parent (Dept)
                Guid? userId = _uow.UserContext.CurrentUserId;
                var currentUser = await _uow.GetRepository<User>().GetSingleAsync(x => x.Id == userId);
                var currentDepartment = await _uow.GetRepository<Department>().GetSingleAsync(x => x.UserDepartmentMappings.Any(y => y.IsHeadCount && y.UserId == currentUser.Id));

                if (currentUser == null)
                {
                    return result;
                }
                var listAllNavigation = await GetAllParentAndChildNavigation();
                if (listAllNavigation.Data != null)
                {
                    foreach (var f1 in ((List<NavigationViewModel>)listAllNavigation.Data))
                    {
                        try
                        {
                            bool currentUserValid = false;
                            if (f1.Type == NavigationType.Left) // left
                            {
                                bool skip = false;
                                if (!string.IsNullOrEmpty(f1.NonUserGroups))
                                {
                                    List<Information> nonUserGroups = Mapper.Map<List<Information>>(JsonConvert.DeserializeObject<List<Information>>(f1.NonUserGroups));
                                    if (nonUserGroups.Any())
                                    {
                                        foreach (var ug in nonUserGroups)
                                        {
                                            var isGuid = IsGuid(ug.id);
                                            if (isGuid)
                                            {
                                                var navigationNonUserGoups = await _uow.GetRepository<Navigation>().GetSingleAsync(x => x.Id == new Guid(ug.id));
                                                if (navigationNonUserGoups != null)
                                                {
                                                    if (navigationNonUserGoups.IsAD && currentUser.Type == LoginType.ActiveDirectory)
                                                        skip = true;
                                                    if (!skip && navigationNonUserGoups.IsMS && currentUser.Type == LoginType.Membership)
                                                        skip = true;

                                                    // JobGrades
                                                    if (!skip && !string.IsNullOrEmpty(navigationNonUserGoups.JobGrades))
                                                    {
                                                        List<Information> jobGradeInfor = Mapper.Map<List<Information>>(JsonConvert.DeserializeObject<List<Information>>(navigationNonUserGoups.JobGrades));
                                                        if (jobGradeInfor.Any())
                                                        {
                                                            foreach (var grade in jobGradeInfor)
                                                            {
                                                                if (!string.IsNullOrEmpty(grade.code) && currentDepartment.JobGrade != null && grade.code.Equals(currentDepartment.JobGrade.Grade.ToString()))
                                                                {
                                                                    skip = true;
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                    }

                                                    // permission
                                                    if (!string.IsNullOrEmpty(navigationNonUserGoups.Permissions) && !skip)
                                                    {
                                                        List<Information> userPermission = Mapper.Map<List<Information>>(JsonConvert.DeserializeObject<List<Information>>(navigationNonUserGoups.Permissions));
                                                        if (userPermission.Any())
                                                        {
                                                            foreach (var per in userPermission)
                                                            {
                                                                if (!string.IsNullOrEmpty(per.code))
                                                                {
                                                                    UserRole role = (UserRole)per.code.ToString().GetAsInt();
                                                                    if ((_uow.UserContext.CurrentUserRole & UserRole.HR) == role)
                                                                    {
                                                                        skip = true;
                                                                        break;
                                                                    }
                                                                    else if ((_uow.UserContext.CurrentUserRole & UserRole.Accounting) == role)
                                                                    {
                                                                        skip = true;
                                                                        break;
                                                                    }
                                                                    else if ((_uow.UserContext.CurrentUserRole & UserRole.Admin) == role)
                                                                    {
                                                                        skip = true;
                                                                        break;
                                                                    }
                                                                    else if ((_uow.UserContext.CurrentUserRole & UserRole.CB) == role)
                                                                    {
                                                                        skip = true;
                                                                        break;
                                                                    }
                                                                    else if ((_uow.UserContext.CurrentUserRole & UserRole.HR) == role)
                                                                    {
                                                                        skip = true;
                                                                        break;
                                                                    }
                                                                    else if ((_uow.UserContext.CurrentUserRole & UserRole.HRAdmin) == role)
                                                                    {
                                                                        skip = true;
                                                                        break;
                                                                    }
                                                                    else if ((_uow.UserContext.CurrentUserRole & UserRole.Member) == role)
                                                                    {
                                                                        skip = true;
                                                                        break;
                                                                    }
                                                                    else if ((_uow.UserContext.CurrentUserRole & UserRole.SAdmin) == role)
                                                                    {
                                                                        skip = true;
                                                                        break;
                                                                    }
                                                                    else if ((_uow.UserContext.CurrentUserRole & UserRole.HRAdmin) == role)
                                                                    {
                                                                        skip = true;
                                                                        break;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }

                                                    // department
                                                    if (!skip && navigationNonUserGoups.Departments != null && navigationNonUserGoups.Departments.Any())
                                                    {
                                                        List<Information> departments = Mapper.Map<List<Information>>(JsonConvert.DeserializeObject<List<Information>>(navigationNonUserGoups.Departments));
                                                        foreach (var f3 in departments)
                                                        {
                                                            bool containChild = await ContainsDepartmentAsync(new Guid(f3.id), currentUser.Id);
                                                            if (containChild)
                                                            {
                                                                skip = true;
                                                                break;
                                                            }
                                                        }
                                                    }

                                                    if (!skip && navigationNonUserGoups.Users != null && navigationNonUserGoups.Users.Any())
                                                    {
                                                        List<Information> users = Mapper.Map<List<Information>>(JsonConvert.DeserializeObject<List<Information>>(navigationNonUserGoups.Users));
                                                        foreach (var f4 in users)
                                                        {
                                                            if (f4.id.ToUpper() == currentUser.Id.ToString().ToUpper())
                                                            {
                                                                skip = true;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                    // break loop
                                                    if (skip) break;
                                                }
                                            }
                                        }
                                    }
                                }

                                if (skip) continue;

                                if (!string.IsNullOrEmpty(f1.UserGroups))
                                {
                                    List<Information> userGroups = Mapper.Map<List<Information>>(JsonConvert.DeserializeObject<List<Information>>(f1.UserGroups));
                                    if (userGroups.Any())
                                    {
                                        foreach (var ug in userGroups)
                                        {
                                            var isGuid = IsGuid(ug.id);
                                            if (isGuid)
                                            {
                                                var navigationUserGoups = await _uow.GetRepository<Navigation>().GetSingleAsync(x => x.Id == new Guid(ug.id));
                                                if (navigationUserGoups != null)
                                                {
                                                    if (navigationUserGoups.IsAD && currentUser.Type == LoginType.ActiveDirectory)
                                                        currentUserValid = true;
                                                    if (!currentUserValid && navigationUserGoups.IsMS && currentUser.Type == LoginType.Membership)
                                                        currentUserValid = true;

                                                    // permission
                                                    if (!string.IsNullOrEmpty(navigationUserGoups.Permissions) && !currentUserValid)
                                                    {
                                                        List<Information> userPermission = Mapper.Map<List<Information>>(JsonConvert.DeserializeObject<List<Information>>(navigationUserGoups.Permissions));
                                                        if (userPermission.Any())
                                                        {
                                                            foreach (var per in userPermission)
                                                            {
                                                                if (!string.IsNullOrEmpty(per.code))
                                                                {
                                                                    UserRole role = (UserRole)per.code.ToString().GetAsInt();
                                                                    if ((_uow.UserContext.CurrentUserRole & UserRole.HR) == role)
                                                                    {
                                                                        currentUserValid = true;
                                                                        break;
                                                                    }
                                                                    else if ((_uow.UserContext.CurrentUserRole & UserRole.Accounting) == role)
                                                                    {
                                                                        currentUserValid = true;
                                                                        break;
                                                                    }
                                                                    else if ((_uow.UserContext.CurrentUserRole & UserRole.Admin) == role)
                                                                    {
                                                                        currentUserValid = true;
                                                                        break;
                                                                    }
                                                                    else if ((_uow.UserContext.CurrentUserRole & UserRole.CB) == role)
                                                                    {
                                                                        currentUserValid = true;
                                                                        break;
                                                                    }
                                                                    else if ((_uow.UserContext.CurrentUserRole & UserRole.HR) == role)
                                                                    {
                                                                        currentUserValid = true;
                                                                        break;
                                                                    }
                                                                    else if ((_uow.UserContext.CurrentUserRole & UserRole.HRAdmin) == role)
                                                                    {
                                                                        currentUserValid = true;
                                                                        break;
                                                                    }
                                                                    else if ((_uow.UserContext.CurrentUserRole & UserRole.Member) == role)
                                                                    {
                                                                        currentUserValid = true;
                                                                        break;
                                                                    }
                                                                    else if ((_uow.UserContext.CurrentUserRole & UserRole.SAdmin) == role)
                                                                    {
                                                                        currentUserValid = true;
                                                                        break;
                                                                    }
                                                                    else if ((_uow.UserContext.CurrentUserRole & UserRole.HRAdmin) == role)
                                                                    {
                                                                        currentUserValid = true;
                                                                        break;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }

                                                    // department
                                                    if (!currentUserValid && navigationUserGoups.Departments != null && navigationUserGoups.Departments.Any())
                                                    {
                                                        List<Information> departments = Mapper.Map<List<Information>>(JsonConvert.DeserializeObject<List<Information>>(navigationUserGoups.Departments));
                                                        foreach (var f3 in departments)
                                                        {
                                                            bool containChild = await ContainsDepartmentAsync(new Guid(f3.id), currentUser.Id);
                                                            if (containChild)
                                                            {
                                                                currentUserValid = true;
                                                                break;
                                                            }
                                                        }
                                                    }

                                                    if (!currentUserValid && navigationUserGoups.Users != null && navigationUserGoups.Users.Any())
                                                    {
                                                        List<Information> users = Mapper.Map<List<Information>>(JsonConvert.DeserializeObject<List<Information>>(navigationUserGoups.Users));
                                                        foreach (var f4 in users)
                                                        {
                                                            if (f4.id.ToUpper() == currentUser.Id.ToString().ToUpper())
                                                            {
                                                                currentUserValid = true;
                                                                break;
                                                            }
                                                        }
                                                    }

                                                    if (currentUserValid)
                                                    {
                                                        items.Add(f1);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                // center, right
                                if (!string.IsNullOrEmpty(f1.JsonChild))
                                {
                                    List<NavigationViewModel> childInvalid = new List<NavigationViewModel>();
                                    List<NavigationViewModel> allChild = Mapper.Map<List<NavigationViewModel>>(JsonConvert.DeserializeObject<List<NavigationViewModel>>(f1.JsonChild));
                                    if (allChild.Any())
                                    {
                                        foreach (var f2 in allChild)
                                        {
                                            // None User
                                            bool skip = false;
                                            if (!string.IsNullOrEmpty(f2.NonUserGroups))
                                            {
                                                List<Information> nonUserGroups = Mapper.Map<List<Information>>(JsonConvert.DeserializeObject<List<Information>>(f2.NonUserGroups));
                                                if (nonUserGroups.Any())
                                                {
                                                    foreach (var ug in nonUserGroups)
                                                    {
                                                        var isGuid = IsGuid(ug.id);
                                                        if (isGuid)
                                                        {
                                                            var navigationNonUserGoups = await _uow.GetRepository<Navigation>().GetSingleAsync(x => x.Id == new Guid(ug.id));
                                                            if (navigationNonUserGoups != null)
                                                            {
                                                                if (navigationNonUserGoups.IsAD && currentUser.Type == LoginType.ActiveDirectory)
                                                                    skip = true;
                                                                if (!skip && navigationNonUserGoups.IsMS && currentUser.Type == LoginType.Membership)
                                                                    skip = true;

                                                                // JobGrades
                                                                if (!skip && !string.IsNullOrEmpty(navigationNonUserGoups.JobGrades))
                                                                {
                                                                    List<Information> jobGradeInfor = Mapper.Map<List<Information>>(JsonConvert.DeserializeObject<List<Information>>(navigationNonUserGoups.JobGrades));
                                                                    if (jobGradeInfor.Any())
                                                                    {
                                                                        foreach (var grade in jobGradeInfor)
                                                                        {
                                                                            if (!string.IsNullOrEmpty(grade.code) && currentDepartment.JobGrade != null && grade.code.Equals(currentDepartment.JobGrade.Grade.ToString()))
                                                                            {
                                                                                skip = true;
                                                                                break;
                                                                            }
                                                                        }
                                                                    }
                                                                }

                                                                // permission
                                                                if (!skip && !string.IsNullOrEmpty(navigationNonUserGoups.Permissions))
                                                                {
                                                                    List<Information> userPermission = Mapper.Map<List<Information>>(JsonConvert.DeserializeObject<List<Information>>(navigationNonUserGoups.Permissions));
                                                                    if (userPermission.Any())
                                                                    {
                                                                        foreach (var per in userPermission)
                                                                        {
                                                                            if (!string.IsNullOrEmpty(per.code))
                                                                            {
                                                                                UserRole role = (UserRole)per.code.ToString().GetAsInt();
                                                                                if ((_uow.UserContext.CurrentUserRole & UserRole.HR) == role)
                                                                                {
                                                                                    skip = true;
                                                                                    break;
                                                                                }
                                                                                else if ((_uow.UserContext.CurrentUserRole & UserRole.Accounting) == role)
                                                                                {
                                                                                    skip = true;
                                                                                    break;
                                                                                }
                                                                                else if ((_uow.UserContext.CurrentUserRole & UserRole.Admin) == role)
                                                                                {
                                                                                    skip = true;
                                                                                    break;
                                                                                }
                                                                                else if ((_uow.UserContext.CurrentUserRole & UserRole.CB) == role)
                                                                                {
                                                                                    skip = true;
                                                                                    break;
                                                                                }
                                                                                else if ((_uow.UserContext.CurrentUserRole & UserRole.HR) == role)
                                                                                {
                                                                                    skip = true;
                                                                                    break;
                                                                                }
                                                                                else if ((_uow.UserContext.CurrentUserRole & UserRole.HRAdmin) == role)
                                                                                {
                                                                                    skip = true;
                                                                                    break;
                                                                                }
                                                                                else if ((_uow.UserContext.CurrentUserRole & UserRole.Member) == role)
                                                                                {
                                                                                    skip = true;
                                                                                    break;
                                                                                }
                                                                                else if ((_uow.UserContext.CurrentUserRole & UserRole.SAdmin) == role)
                                                                                {
                                                                                    skip = true;
                                                                                    break;
                                                                                }
                                                                                else if ((_uow.UserContext.CurrentUserRole & UserRole.HRAdmin) == role)
                                                                                {
                                                                                    skip = true;
                                                                                    break;
                                                                                }

                                                                            }
                                                                        }
                                                                    }
                                                                }

                                                                // department
                                                                if (!skip && navigationNonUserGoups.Departments != null && !navigationNonUserGoups.Departments.Equals(""))
                                                                {
                                                                    List<Information> departments = Mapper.Map<List<Information>>(JsonConvert.DeserializeObject<List<Information>>(navigationNonUserGoups.Departments));
                                                                    foreach (var f3 in departments)
                                                                    {
                                                                        bool containChild = await ContainsDepartmentAsync(new Guid(f3.id), currentUser.Id);
                                                                        if (containChild)
                                                                        {
                                                                            skip = true;
                                                                            break;
                                                                        }
                                                                    }
                                                                }

                                                                if (!skip && navigationNonUserGoups.Users != null && !navigationNonUserGoups.Users.Equals(""))
                                                                {
                                                                    List<Information> users = Mapper.Map<List<Information>>(JsonConvert.DeserializeObject<List<Information>>(navigationNonUserGoups.Users));
                                                                    foreach (var f4 in users)
                                                                    {
                                                                        if (f4.id.ToUpper() == currentUser.Id.ToString().ToUpper())
                                                                        {
                                                                            skip = true;
                                                                            break;
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        if (skip) break;
                                                    }
                                                }
                                            }

                                            if (skip) continue;

                                            // Group User
                                            if (!string.IsNullOrEmpty(f2.UserGroups))
                                            {
                                                List<Information> userGroups = Mapper.Map<List<Information>>(JsonConvert.DeserializeObject<List<Information>>(f2.UserGroups));
                                                if (userGroups.Any())
                                                {
                                                    foreach (var ug in userGroups)
                                                    {
                                                        var isGuid = IsGuid(ug.id);
                                                        if (isGuid)
                                                        {
                                                            var navigationUserGoups = await _uow.GetRepository<Navigation>().GetSingleAsync(x => x.Id == new Guid(ug.id));
                                                            if (navigationUserGoups != null)
                                                            {
                                                                if (navigationUserGoups.IsAD && currentUser.Type == LoginType.ActiveDirectory)
                                                                    currentUserValid = true;
                                                                if (!currentUserValid && navigationUserGoups.IsMS && currentUser.Type == LoginType.Membership)
                                                                    currentUserValid = true;

                                                                // permission
                                                                if (!currentUserValid && !string.IsNullOrEmpty(navigationUserGoups.Permissions))
                                                                {
                                                                    List<Information> userPermission = Mapper.Map<List<Information>>(JsonConvert.DeserializeObject<List<Information>>(navigationUserGoups.Permissions));
                                                                    if (userPermission.Any())
                                                                    {
                                                                        foreach (var per in userPermission)
                                                                        {
                                                                            if (!string.IsNullOrEmpty(per.code))
                                                                            {
                                                                                UserRole role = (UserRole)per.code.ToString().GetAsInt();
                                                                                if ((_uow.UserContext.CurrentUserRole & UserRole.HR) == role)
                                                                                {
                                                                                    currentUserValid = true;
                                                                                    break;
                                                                                }
                                                                                else if ((_uow.UserContext.CurrentUserRole & UserRole.Accounting) == role)
                                                                                {
                                                                                    currentUserValid = true;
                                                                                    break;
                                                                                }
                                                                                else if ((_uow.UserContext.CurrentUserRole & UserRole.Admin) == role)
                                                                                {
                                                                                    currentUserValid = true;
                                                                                    break;
                                                                                }
                                                                                else if ((_uow.UserContext.CurrentUserRole & UserRole.CB) == role)
                                                                                {
                                                                                    currentUserValid = true;
                                                                                    break;
                                                                                }
                                                                                else if ((_uow.UserContext.CurrentUserRole & UserRole.HR) == role)
                                                                                {
                                                                                    currentUserValid = true;
                                                                                    break;
                                                                                }
                                                                                else if ((_uow.UserContext.CurrentUserRole & UserRole.HRAdmin) == role)
                                                                                {
                                                                                    currentUserValid = true;
                                                                                    break;
                                                                                }
                                                                                else if ((_uow.UserContext.CurrentUserRole & UserRole.Member) == role)
                                                                                {
                                                                                    currentUserValid = true;
                                                                                    break;
                                                                                }
                                                                                else if ((_uow.UserContext.CurrentUserRole & UserRole.SAdmin) == role)
                                                                                {
                                                                                    currentUserValid = true;
                                                                                    break;
                                                                                }
                                                                                else if ((_uow.UserContext.CurrentUserRole & UserRole.HRAdmin) == role)
                                                                                {
                                                                                    currentUserValid = true;
                                                                                    break;
                                                                                }

                                                                            }
                                                                        }
                                                                    }
                                                                }

                                                                // department
                                                                if (!currentUserValid && navigationUserGoups.Departments != null && !navigationUserGoups.Departments.Equals(""))
                                                                {
                                                                    List<Information> departments = Mapper.Map<List<Information>>(JsonConvert.DeserializeObject<List<Information>>(navigationUserGoups.Departments));
                                                                    foreach (var f3 in departments)
                                                                    {
                                                                        bool containChild = await ContainsDepartmentAsync(new Guid(f3.id), currentUser.Id);
                                                                        if (containChild)
                                                                        {
                                                                            currentUserValid = true;
                                                                            break;
                                                                        }
                                                                    }
                                                                }

                                                                if (!currentUserValid && navigationUserGoups.Users != null && !navigationUserGoups.Users.Equals(""))
                                                                {
                                                                    List<Information> users = Mapper.Map<List<Information>>(JsonConvert.DeserializeObject<List<Information>>(navigationUserGoups.Users));
                                                                    foreach (var f4 in users)
                                                                    {
                                                                        if (f4.id.ToUpper() == currentUser.Id.ToString().ToUpper())
                                                                        {
                                                                            currentUserValid = true;
                                                                            break;
                                                                        }
                                                                    }
                                                                }
                                                                if (currentUserValid)
                                                                {
                                                                    childInvalid.Add(f2);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        if (childInvalid.Count() > 0)
                                        {
                                            f1.JsonChild = JsonConvert.SerializeObject(childInvalid.OrderBy(x => x.Priority).ToList());
                                            items.Add(f1);
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            _logger.LogError("Navigation Error: " + e.Message);
                            continue;
                        }
                    }
                }
                result = new ArrayResultDTO { Data = items, Count = items.Count };
            }
            catch (MembershipCreateUserException e)
            {
                throw e;
            }
            return result;
        }

        public async Task<ArrayResultDTO> GetListNavigationByUserIdAndDepartmentIdV2(NavigationArgs.GetListArgs args)
        {
            var result = new ArrayResultDTO { };
            List<NavigationViewModel> items = new List<NavigationViewModel>();
            try
            {
                if (args != null || args.DepartmentId != null)
                {
                    Guid? userId = _uow.UserContext.CurrentUserId;
                    if (args.UserId != null)
                    {
                        // Lay User hien tai
                        userId = args.UserId;
                    }
                    var currentUser = await _uow.GetRepository<User>(true).GetSingleAsync(x => x.Id == userId);
                    var currentDepartment = await _uow.GetRepository<Department>(true).GetSingleAsync(x => x.UserDepartmentMappings.Any(y => y.IsHeadCount && y.UserId == currentUser.Id));

                    if (currentUser == null)
                        return result;

                    var listAllNavigation = await GetAllParentAndChildNavigation();
                    if (listAllNavigation.Data != null)
                    {
                        foreach (var f1 in ((List<NavigationViewModel>)listAllNavigation.Data))
                        {
                            try
                            {
                                bool currentUserValid = false;
                                if (f1.Type == NavigationType.Left) // left
                                {
                                    bool skip = false;
                                    if (!string.IsNullOrEmpty(f1.NonUserGroups))
                                    {
                                        List<Information> nonUserGroups = Mapper.Map<List<Information>>(JsonConvert.DeserializeObject<List<Information>>(f1.NonUserGroups));
                                        if (nonUserGroups.Any())
                                        {
                                            foreach (var ug in nonUserGroups)
                                            {
                                                var isGuid = IsGuid(ug.id);
                                                if (isGuid)
                                                {
                                                    var navigationNonUserGoups = await _uow.GetRepository<Navigation>().GetSingleAsync(x => x.Id == new Guid(ug.id));
                                                    if (navigationNonUserGoups != null)
                                                    {
                                                        if (navigationNonUserGoups.IsAD && currentUser.Type == LoginType.ActiveDirectory)
                                                            skip = true;
                                                        if (!skip && navigationNonUserGoups.IsMS && currentUser.Type == LoginType.Membership)
                                                            skip = true;

                                                        // JobGrades
                                                        if (!skip && !string.IsNullOrEmpty(navigationNonUserGoups.JobGrades))
                                                        {
                                                            List<Information> jobGradeInfor = Mapper.Map<List<Information>>(JsonConvert.DeserializeObject<List<Information>>(navigationNonUserGoups.JobGrades));
                                                            if (jobGradeInfor.Any())
                                                            {
                                                                foreach (var grade in jobGradeInfor)
                                                                {
                                                                    if (!string.IsNullOrEmpty(grade.code) && currentDepartment.JobGrade != null && grade.code.Equals(currentDepartment.JobGrade.Grade.ToString()))
                                                                    {
                                                                        skip = true;
                                                                        break;
                                                                    }
                                                                }
                                                            }
                                                        }

                                                        // permission
                                                        if (!string.IsNullOrEmpty(navigationNonUserGoups.Permissions) && !skip)
                                                        {
                                                            List<Information> userPermission = Mapper.Map<List<Information>>(JsonConvert.DeserializeObject<List<Information>>(navigationNonUserGoups.Permissions));
                                                            if (userPermission.Any())
                                                            {
                                                                foreach (var per in userPermission)
                                                                {
                                                                    if (!string.IsNullOrEmpty(per.code))
                                                                    {
                                                                        UserRole role = (UserRole)per.code.ToString().GetAsInt();
                                                                        if ((_uow.UserContext.CurrentUserRole & UserRole.HR) == role)
                                                                        {
                                                                            skip = true;
                                                                            break;
                                                                        }
                                                                        else if ((_uow.UserContext.CurrentUserRole & UserRole.Accounting) == role)
                                                                        {
                                                                            skip = true;
                                                                            break;
                                                                        }
                                                                        else if ((_uow.UserContext.CurrentUserRole & UserRole.Admin) == role)
                                                                        {
                                                                            skip = true;
                                                                            break;
                                                                        }
                                                                        else if ((_uow.UserContext.CurrentUserRole & UserRole.CB) == role)
                                                                        {
                                                                            skip = true;
                                                                            break;
                                                                        }
                                                                        else if ((_uow.UserContext.CurrentUserRole & UserRole.HR) == role)
                                                                        {
                                                                            skip = true;
                                                                            break;
                                                                        }
                                                                        else if ((_uow.UserContext.CurrentUserRole & UserRole.HRAdmin) == role)
                                                                        {
                                                                            skip = true;
                                                                            break;
                                                                        }
                                                                        else if ((_uow.UserContext.CurrentUserRole & UserRole.Member) == role)
                                                                        {
                                                                            skip = true;
                                                                            break;
                                                                        }
                                                                        else if ((_uow.UserContext.CurrentUserRole & UserRole.SAdmin) == role)
                                                                        {
                                                                            skip = true;
                                                                            break;
                                                                        }
                                                                        else if ((_uow.UserContext.CurrentUserRole & UserRole.HRAdmin) == role)
                                                                        {
                                                                            skip = true;
                                                                            break;
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }

                                                        // department
                                                        if (!skip && navigationNonUserGoups.Departments != null && navigationNonUserGoups.Departments.Any())
                                                        {
                                                            List<Information> departments = Mapper.Map<List<Information>>(JsonConvert.DeserializeObject<List<Information>>(navigationNonUserGoups.Departments));
                                                            foreach (var f3 in departments)
                                                            {
                                                                bool containChild = await ContainsDepartmentAsync(new Guid(f3.id), currentUser.Id);
                                                                if (containChild)
                                                                {
                                                                    skip = true;
                                                                    break;
                                                                }
                                                            }
                                                        }

                                                        if (!skip && navigationNonUserGoups.Users != null && navigationNonUserGoups.Users.Any())
                                                        {
                                                            List<Information> users = Mapper.Map<List<Information>>(JsonConvert.DeserializeObject<List<Information>>(navigationNonUserGoups.Users));
                                                            foreach (var f4 in users)
                                                            {
                                                                if (f4.id.ToUpper() == currentUser.Id.ToString().ToUpper())
                                                                {
                                                                    skip = true;
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                        // break loop
                                                        if (skip) break;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    if (skip) continue;

                                    if (!string.IsNullOrEmpty(f1.UserGroups))
                                    {
                                        List<Information> userGroups = Mapper.Map<List<Information>>(JsonConvert.DeserializeObject<List<Information>>(f1.UserGroups));
                                        if (userGroups.Any())
                                        {
                                            foreach (var ug in userGroups)
                                            {
                                                var isGuid = IsGuid(ug.id);
                                                if (isGuid)
                                                {
                                                    var navigationUserGoups = await _uow.GetRepository<Navigation>().GetSingleAsync(x => x.Id == new Guid(ug.id));
                                                    if (navigationUserGoups != null)
                                                    {
                                                        if (navigationUserGoups.IsAD && currentUser.Type == LoginType.ActiveDirectory)
                                                            currentUserValid = true;
                                                        if (!currentUserValid && navigationUserGoups.IsMS && currentUser.Type == LoginType.Membership)
                                                            currentUserValid = true;

                                                        // permission
                                                        if (!string.IsNullOrEmpty(navigationUserGoups.Permissions) && !currentUserValid)
                                                        {
                                                            List<Information> userPermission = Mapper.Map<List<Information>>(JsonConvert.DeserializeObject<List<Information>>(navigationUserGoups.Permissions));
                                                            if (userPermission.Any())
                                                            {
                                                                foreach (var per in userPermission)
                                                                {
                                                                    if (!string.IsNullOrEmpty(per.code))
                                                                    {
                                                                        UserRole role = (UserRole)per.code.ToString().GetAsInt();
                                                                        if ((_uow.UserContext.CurrentUserRole & UserRole.HR) == role)
                                                                        {
                                                                            currentUserValid = true;
                                                                            break;
                                                                        }
                                                                        else if ((_uow.UserContext.CurrentUserRole & UserRole.Accounting) == role)
                                                                        {
                                                                            currentUserValid = true;
                                                                            break;
                                                                        }
                                                                        else if ((_uow.UserContext.CurrentUserRole & UserRole.Admin) == role)
                                                                        {
                                                                            currentUserValid = true;
                                                                            break;
                                                                        }
                                                                        else if ((_uow.UserContext.CurrentUserRole & UserRole.CB) == role)
                                                                        {
                                                                            currentUserValid = true;
                                                                            break;
                                                                        }
                                                                        else if ((_uow.UserContext.CurrentUserRole & UserRole.HR) == role)
                                                                        {
                                                                            currentUserValid = true;
                                                                            break;
                                                                        }
                                                                        else if ((_uow.UserContext.CurrentUserRole & UserRole.HRAdmin) == role)
                                                                        {
                                                                            currentUserValid = true;
                                                                            break;
                                                                        }
                                                                        else if ((_uow.UserContext.CurrentUserRole & UserRole.Member) == role)
                                                                        {
                                                                            currentUserValid = true;
                                                                            break;
                                                                        }
                                                                        else if ((_uow.UserContext.CurrentUserRole & UserRole.SAdmin) == role)
                                                                        {
                                                                            currentUserValid = true;
                                                                            break;
                                                                        }
                                                                        else if ((_uow.UserContext.CurrentUserRole & UserRole.HRAdmin) == role)
                                                                        {
                                                                            currentUserValid = true;
                                                                            break;
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }

                                                        // department
                                                        if (!currentUserValid && navigationUserGoups.Departments != null && navigationUserGoups.Departments.Any())
                                                        {
                                                            List<Information> departments = Mapper.Map<List<Information>>(JsonConvert.DeserializeObject<List<Information>>(navigationUserGoups.Departments));
                                                            foreach (var f3 in departments)
                                                            {
                                                                bool containChild = await ContainsDepartmentAsync(new Guid(f3.id), currentUser.Id);
                                                                if (containChild)
                                                                {
                                                                    currentUserValid = true;
                                                                    break;
                                                                }
                                                            }
                                                        }

                                                        if (!currentUserValid && navigationUserGoups.Users != null && navigationUserGoups.Users.Any())
                                                        {
                                                            List<Information> users = Mapper.Map<List<Information>>(JsonConvert.DeserializeObject<List<Information>>(navigationUserGoups.Users));
                                                            foreach (var f4 in users)
                                                            {
                                                                if (f4.id.ToUpper() == currentUser.Id.ToString().ToUpper())
                                                                {
                                                                    currentUserValid = true;
                                                                    break;
                                                                }
                                                            }
                                                        }

                                                        if (currentUserValid)
                                                        {
                                                            items.Add(f1);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    // center, right
                                    if (!string.IsNullOrEmpty(f1.JsonChild))
                                    {
                                        List<NavigationViewModel> childInvalid = new List<NavigationViewModel>();
                                        List<NavigationViewModel> allChild = Mapper.Map<List<NavigationViewModel>>(JsonConvert.DeserializeObject<List<NavigationViewModel>>(f1.JsonChild));
                                        if (allChild.Any())
                                        {
                                            foreach (var f2 in allChild)
                                            {
                                                // None User
                                                bool skip = false;
                                                if (!string.IsNullOrEmpty(f2.NonUserGroups))
                                                {
                                                    List<Information> nonUserGroups = Mapper.Map<List<Information>>(JsonConvert.DeserializeObject<List<Information>>(f2.NonUserGroups));
                                                    if (nonUserGroups.Any())
                                                    {
                                                        foreach (var ug in nonUserGroups)
                                                        {
                                                            var isGuid = IsGuid(ug.id);
                                                            if (isGuid)
                                                            {
                                                                var navigationNonUserGoups = await _uow.GetRepository<Navigation>().GetSingleAsync(x => x.Id == new Guid(ug.id));
                                                                if (navigationNonUserGoups != null)
                                                                {
                                                                    if (navigationNonUserGoups.IsAD && currentUser.Type == LoginType.ActiveDirectory)
                                                                        skip = true;
                                                                    if (!skip && navigationNonUserGoups.IsMS && currentUser.Type == LoginType.Membership)
                                                                        skip = true;

                                                                    // JobGrades
                                                                    if (!skip && !string.IsNullOrEmpty(navigationNonUserGoups.JobGrades))
                                                                    {
                                                                        List<Information> jobGradeInfor = Mapper.Map<List<Information>>(JsonConvert.DeserializeObject<List<Information>>(navigationNonUserGoups.JobGrades));
                                                                        if (jobGradeInfor.Any())
                                                                        {
                                                                            foreach (var grade in jobGradeInfor)
                                                                            {
                                                                                if (!string.IsNullOrEmpty(grade.code) && currentDepartment.JobGrade != null && grade.code.Equals(currentDepartment.JobGrade.Grade.ToString()))
                                                                                {
                                                                                    skip = true;
                                                                                    break;
                                                                                }
                                                                            }
                                                                        }
                                                                    }

                                                                    // permission
                                                                    if (!skip && !string.IsNullOrEmpty(navigationNonUserGoups.Permissions))
                                                                    {
                                                                        List<Information> userPermission = Mapper.Map<List<Information>>(JsonConvert.DeserializeObject<List<Information>>(navigationNonUserGoups.Permissions));
                                                                        if (userPermission.Any())
                                                                        {
                                                                            foreach (var per in userPermission)
                                                                            {
                                                                                if (!string.IsNullOrEmpty(per.code))
                                                                                {
                                                                                    UserRole role = (UserRole)per.code.ToString().GetAsInt();
                                                                                    if ((_uow.UserContext.CurrentUserRole & UserRole.HR) == role)
                                                                                    {
                                                                                        skip = true;
                                                                                        break;
                                                                                    }
                                                                                    else if ((_uow.UserContext.CurrentUserRole & UserRole.Accounting) == role)
                                                                                    {
                                                                                        skip = true;
                                                                                        break;
                                                                                    }
                                                                                    else if ((_uow.UserContext.CurrentUserRole & UserRole.Admin) == role)
                                                                                    {
                                                                                        skip = true;
                                                                                        break;
                                                                                    }
                                                                                    else if ((_uow.UserContext.CurrentUserRole & UserRole.CB) == role)
                                                                                    {
                                                                                        skip = true;
                                                                                        break;
                                                                                    }
                                                                                    else if ((_uow.UserContext.CurrentUserRole & UserRole.HR) == role)
                                                                                    {
                                                                                        skip = true;
                                                                                        break;
                                                                                    }
                                                                                    else if ((_uow.UserContext.CurrentUserRole & UserRole.HRAdmin) == role)
                                                                                    {
                                                                                        skip = true;
                                                                                        break;
                                                                                    }
                                                                                    else if ((_uow.UserContext.CurrentUserRole & UserRole.Member) == role)
                                                                                    {
                                                                                        skip = true;
                                                                                        break;
                                                                                    }
                                                                                    else if ((_uow.UserContext.CurrentUserRole & UserRole.SAdmin) == role)
                                                                                    {
                                                                                        skip = true;
                                                                                        break;
                                                                                    }
                                                                                    else if ((_uow.UserContext.CurrentUserRole & UserRole.HRAdmin) == role)
                                                                                    {
                                                                                        skip = true;
                                                                                        break;
                                                                                    }

                                                                                }
                                                                            }
                                                                        }
                                                                    }

                                                                    // department
                                                                    if (!skip && navigationNonUserGoups.Departments != null && !navigationNonUserGoups.Departments.Equals(""))
                                                                    {
                                                                        List<Information> departments = Mapper.Map<List<Information>>(JsonConvert.DeserializeObject<List<Information>>(navigationNonUserGoups.Departments));
                                                                        foreach (var f3 in departments)
                                                                        {
                                                                            bool containChild = await ContainsDepartmentAsync(new Guid(f3.id), currentUser.Id);
                                                                            if (containChild)
                                                                            {
                                                                                skip = true;
                                                                                break;
                                                                            }
                                                                        }
                                                                    }

                                                                    if (!skip && navigationNonUserGoups.Users != null && !navigationNonUserGoups.Users.Equals(""))
                                                                    {
                                                                        List<Information> users = Mapper.Map<List<Information>>(JsonConvert.DeserializeObject<List<Information>>(navigationNonUserGoups.Users));
                                                                        foreach (var f4 in users)
                                                                        {
                                                                            if (f4.id.ToUpper() == currentUser.Id.ToString().ToUpper())
                                                                            {
                                                                                skip = true;
                                                                                break;
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            if (skip) break;
                                                        }
                                                    }
                                                }

                                                if (skip) continue;

                                                // Group User
                                                if (!string.IsNullOrEmpty(f2.UserGroups))
                                                {
                                                    List<Information> userGroups = Mapper.Map<List<Information>>(JsonConvert.DeserializeObject<List<Information>>(f2.UserGroups));
                                                    if (userGroups.Any())
                                                    {
                                                        foreach (var ug in userGroups)
                                                        {
                                                            var isGuid = IsGuid(ug.id);
                                                            if (isGuid)
                                                            {
                                                                var navigationUserGoups = await _uow.GetRepository<Navigation>().GetSingleAsync(x => x.Id == new Guid(ug.id));
                                                                if (navigationUserGoups != null)
                                                                {
                                                                    if (navigationUserGoups.IsAD && currentUser.Type == LoginType.ActiveDirectory)
                                                                        currentUserValid = true;
                                                                    if (!currentUserValid && navigationUserGoups.IsMS && currentUser.Type == LoginType.Membership)
                                                                        currentUserValid = true;

                                                                    // permission
                                                                    if (!currentUserValid && !string.IsNullOrEmpty(navigationUserGoups.Permissions))
                                                                    {
                                                                        List<Information> userPermission = Mapper.Map<List<Information>>(JsonConvert.DeserializeObject<List<Information>>(navigationUserGoups.Permissions));
                                                                        if (userPermission.Any())
                                                                        {
                                                                            foreach (var per in userPermission)
                                                                            {
                                                                                if (!string.IsNullOrEmpty(per.code))
                                                                                {
                                                                                    UserRole role = (UserRole)per.code.ToString().GetAsInt();
                                                                                    if ((_uow.UserContext.CurrentUserRole & UserRole.HR) == role)
                                                                                    {
                                                                                        currentUserValid = true;
                                                                                        break;
                                                                                    }
                                                                                    else if ((_uow.UserContext.CurrentUserRole & UserRole.Accounting) == role)
                                                                                    {
                                                                                        currentUserValid = true;
                                                                                        break;
                                                                                    }
                                                                                    else if ((_uow.UserContext.CurrentUserRole & UserRole.Admin) == role)
                                                                                    {
                                                                                        currentUserValid = true;
                                                                                        break;
                                                                                    }
                                                                                    else if ((_uow.UserContext.CurrentUserRole & UserRole.CB) == role)
                                                                                    {
                                                                                        currentUserValid = true;
                                                                                        break;
                                                                                    }
                                                                                    else if ((_uow.UserContext.CurrentUserRole & UserRole.HR) == role)
                                                                                    {
                                                                                        currentUserValid = true;
                                                                                        break;
                                                                                    }
                                                                                    else if ((_uow.UserContext.CurrentUserRole & UserRole.HRAdmin) == role)
                                                                                    {
                                                                                        currentUserValid = true;
                                                                                        break;
                                                                                    }
                                                                                    else if ((_uow.UserContext.CurrentUserRole & UserRole.Member) == role)
                                                                                    {
                                                                                        currentUserValid = true;
                                                                                        break;
                                                                                    }
                                                                                    else if ((_uow.UserContext.CurrentUserRole & UserRole.SAdmin) == role)
                                                                                    {
                                                                                        currentUserValid = true;
                                                                                        break;
                                                                                    }
                                                                                    else if ((_uow.UserContext.CurrentUserRole & UserRole.HRAdmin) == role)
                                                                                    {
                                                                                        currentUserValid = true;
                                                                                        break;
                                                                                    }

                                                                                }
                                                                            }
                                                                        }
                                                                    }

                                                                    // department
                                                                    if (!currentUserValid && navigationUserGoups.Departments != null && !navigationUserGoups.Departments.Equals(""))
                                                                    {
                                                                        List<Information> departments = Mapper.Map<List<Information>>(JsonConvert.DeserializeObject<List<Information>>(navigationUserGoups.Departments));
                                                                        foreach (var f3 in departments)
                                                                        {
                                                                            bool containChild = await ContainsDepartmentAsync(new Guid(f3.id), currentUser.Id);
                                                                            if (containChild)
                                                                            {
                                                                                currentUserValid = true;
                                                                                break;
                                                                            }
                                                                        }
                                                                    }

                                                                    if (!currentUserValid && navigationUserGoups.Users != null && !navigationUserGoups.Users.Equals(""))
                                                                    {
                                                                        List<Information> users = Mapper.Map<List<Information>>(JsonConvert.DeserializeObject<List<Information>>(navigationUserGoups.Users));
                                                                        foreach (var f4 in users)
                                                                        {
                                                                            if (f4.id.ToUpper() == currentUser.Id.ToString().ToUpper())
                                                                            {
                                                                                currentUserValid = true;
                                                                                break;
                                                                            }
                                                                        }
                                                                    }
                                                                    if (currentUserValid)
                                                                    {
                                                                        childInvalid.Add(f2);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            if (childInvalid.Count() > 0)
                                            {
                                                f1.JsonChild = JsonConvert.SerializeObject(childInvalid);
                                                items.Add(f1);
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                _logger.LogError("Error: " + e.Message);
                                continue;
                            }
                        }
                    }
                }
                result = new ArrayResultDTO { Data = items, Count = items.Count };
            }
            catch (MembershipCreateUserException e)
            {
                throw e;
            }
            return result;
        }


        public async Task<ArrayResultDTO> GetListNavigationByUserIsNotEdocHR()
        {
            var result = new ArrayResultDTO { };
            List<NavigationViewModel> items = new List<NavigationViewModel>();
            try
            {
                var listAllNavigation = await GetAllParentAndChildNavigation();
                if (listAllNavigation.Data != null)
                {
                    foreach (var f1 in ((List<NavigationViewModel>)listAllNavigation.Data))
                    {
                        try
                        {
                            if (f1.Type == NavigationType.Left) // left
                            {
                                if (f1.Module != EdocModule.EDOCHR)
                                {
                                    items.Add(f1);
                                }
                            }
                            else // Right / Center
                            {
                                List<NavigationViewModel> childInvalid = new List<NavigationViewModel>();
                                List<NavigationViewModel> allChild = Mapper.Map<List<NavigationViewModel>>(JsonConvert.DeserializeObject<List<NavigationViewModel>>(f1.JsonChild));
                                if (allChild.Any())
                                {
                                    foreach (var f2 in allChild)
                                    {
                                        {
                                            if (f2.Module != EdocModule.EDOCHR)
                                            {
                                                childInvalid.Add(f2);
                                            }
                                        }
                                        if (childInvalid.Count() > 0)
                                        {
                                            f1.JsonChild = JsonConvert.SerializeObject(childInvalid.OrderBy(x => x.Priority).ToList());
                                            items.Add(f1);
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            _logger.LogError("Navigation Error: " + e.Message);
                            continue;
                        }
                    }
                }
                result = new ArrayResultDTO { Data = items, Count = items.Count };
            }
            catch (MembershipCreateUserException e)
            {
                throw e;
            }
            return result;
        }

        public async Task<ArrayResultDTO> GetChildNavigationByParentId(Guid parentId)
        {
            var result = new ArrayResultDTO { };
            try
            {
                var items = await _uow.GetRepository<Navigation>().FindByAsync<NavigationViewModel>(x => x.ParentId == parentId && !x.IsDeleted, "Modified desc");
                var count = await _uow.GetRepository<Navigation>().CountAsync(x => x.ParentId == parentId && !x.IsDeleted);
                result = new ArrayResultDTO { Data = items, Count = count };
            }
            catch (MembershipCreateUserException e)
            {
                throw e;
            }
            return result;
        }

        public async Task<ArrayResultDTO> GetChildNavigationByType(NavigationType type)
        {
            var result = new ArrayResultDTO { };
            try
            {
                var items = await _uow.GetRepository<Navigation>().FindByAsync<NavigationViewModel>(x => x.Type == type && !x.IsDeleted);
                var count = await _uow.GetRepository<Navigation>().CountAsync(x => x.Type == type && !x.IsDeleted);
                result = new ArrayResultDTO { Data = items, Count = count };
            }
            catch (MembershipCreateUserException e)
            {
                throw e;
            }
            return result;
        }

        // De quy Department
        public async Task<bool> ContainsDepartmentAsync(Guid departmentId, Guid userId)
        {
            bool isContain = false;
            var department = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == departmentId);
            if (department != null)
            {
                var userMapDepartment = await _uow.GetRepository<UserDepartmentMapping>().FindByAsync(x => x.DepartmentId == department.Id);
                if (userMapDepartment != null)
                {
                    foreach (var item in userMapDepartment)
                    {
                        if (item.UserId == userId)
                        {
                            isContain = true;
                            break;
                        }
                    }
                }
            }
            if (!isContain)
            {
                var departmentChild = await _uow.GetRepository<Department>().FindByAsync(x => x.ParentId == departmentId);
                if (departmentChild != null)
                {
                    foreach (var item in departmentChild)
                    {
                        isContain = await ContainsDepartmentAsync(item.Id, userId);
                        if (isContain) break;
                    }
                }
            }
            return isContain;
        }

        public async Task<ResultDTO> CreateNavigation(NavigationDataForCreatingArgs args)
        {
            var result = new ResultDTO();
            try
            {
                if (object.ReferenceEquals(args, null))
                {
                    return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Error !" } };
                }

                if (object.ReferenceEquals(args.Type, null))
                {
                    return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Navigation Type Not Found !" } };
                }

                Navigation addNavigation = new Navigation
                {
                    Modified = DateTimeOffset.Now
                };
                switch (args.Type)
                {
                    case NavigationType.Center:
                    case NavigationType.Right:
                    case NavigationType.Left:
                        if (object.ReferenceEquals(args.Priority, null))
                        {
                            return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Priority Type is required !" } };
                        }
                        addNavigation.Priority = args.Priority;
                        if (string.IsNullOrEmpty(args.Title_VI) || string.IsNullOrEmpty(args.Title_EN))
                        {
                            return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Title is required !" } };
                        }
                        addNavigation.Title_VI = args.Title_VI;
                        addNavigation.Title_EN = args.Title_EN;
                        addNavigation.Module = args.Module;
                        break;
                    case NavigationType.GroupUsers:
                        // Chi co 1 ten nen dung name chu khong dung title_vi
                        if (string.IsNullOrEmpty(args.Name))
                        {
                            return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Name is required !" } };
                        }
                        addNavigation.Name = args.Name;
                        break;
                }

                addNavigation.Type = args.Type;
                if ((args.ParentId != null && string.IsNullOrEmpty(args.Url)) || (args.Type.Equals(NavigationType.Left) && string.IsNullOrEmpty(args.Url)))
                {
                    return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Url is required !" } };
                }

                if (!string.IsNullOrEmpty(args.Url))
                {
                    addNavigation.Url = args.Url;
                }

                if (!string.IsNullOrEmpty(args.Description))
                {
                    addNavigation.Description = args.Description;
                }

                var listDepartments = new List<Information>();
                if (args.Departments != null)
                {
                    string[] deptArr = ((IEnumerable)args.Departments).Cast<object>().Select(x => x.ToString()).Where(x => x != null).ToArray();
                    if (deptArr.Count() > 0)
                    {
                        foreach (var item in deptArr)
                        {
                            try
                            {
                                var isGuid = IsGuid(item);
                                if (isGuid)
                                {
                                    var department = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == new Guid(item));
                                    if (department == null) continue;
                                    Information dept = new Information
                                    {
                                        id = department.Id.ToString().ToUpper(),
                                        name = department.Name,
                                        code = department.Code
                                    };
                                    listDepartments.Add(dept);
                                }
                            }
                            catch (MembershipCreateUserException ex)
                            {
                                _logger.LogError("Error: " + ex.Message);
                                continue;
                            }
                        }
                    }
                }

                var listUsers = new List<Information>();
                if (args.Users != null)
                {
                    string[] usersArr = ((IEnumerable)args.Users).Cast<object>().Select(x => x.ToString()).Where(x => x != null).ToArray();
                    if (usersArr.Count() > 0)
                    {
                        foreach (var item in usersArr)
                        {
                            try
                            {
                                var isGuid = IsGuid(item);
                                if (isGuid)
                                {
                                    var user = await _uow.GetRepository<User>().GetSingleAsync(x => x.Id == new Guid(item));
                                    if (user == null) continue;
                                    Information userInfo = new Information
                                    {
                                        id = user.Id.ToString().ToUpper(),
                                        name = user.FullName,
                                        code = user.SAPCode
                                    };
                                    listUsers.Add(userInfo);
                                }
                            }
                            catch (MembershipCreateUserException ex)
                            {
                                _logger.LogError("Error: " + ex.Message);
                                continue;
                            }
                        }
                    }
                }

                var listPermissions = new List<Information>();
                if (args.Permissions != null)
                {
                    string[] permissionArr = ((IEnumerable)args.Permissions).Cast<object>().Select(x => x.ToString()).Where(x => x != null).ToArray();
                    if (permissionArr.Count() > 0)
                    {
                        foreach (var item in permissionArr)
                        {
                            try
                            {
                                Information userInfo = new Information
                                {
                                    code = item
                                };
                                listPermissions.Add(userInfo);
                            }
                            catch (MembershipCreateUserException ex)
                            {
                                _logger.LogError("Error: " + ex.Message);
                                continue;
                            }
                        }
                    }
                    else
                    {
                        addNavigation.Permissions = null;
                    }
                }

                var listJobgrades = new List<Information>();
                if (args.Jobgrades != null)
                {
                    string[] jobgradeArr = ((IEnumerable)args.Jobgrades).Cast<object>().Select(x => x.ToString()).Where(x => x != null).ToArray();
                    if (jobgradeArr.Count() > 0)
                    {
                        foreach (var item in jobgradeArr)
                        {
                            try
                            {
                                Information userInfo = new Information { code = item };
                                listJobgrades.Add(userInfo);
                            }
                            catch (MembershipCreateUserException ex)
                            {
                                _logger.LogError("Error: " + ex.Message);
                                continue;
                            }
                        }
                    }
                    else
                    {
                        addNavigation.JobGrades = null;
                    }
                }

                var listUserGroups = new List<Information>();
                if (args.UserGroups != null)
                {
                    string[] userGroupsArr = ((IEnumerable)args.UserGroups).Cast<object>().Select(x => x.ToString()).Where(x => x != null).ToArray();
                    if (userGroupsArr.Count() > 0)
                    {
                        foreach (var item in userGroupsArr)
                        {
                            try
                            {
                                var isGuid = IsGuid(item);
                                if (isGuid)
                                {
                                    var userGr = await _uow.GetRepository<Navigation>().GetSingleAsync(x => x.Id == new Guid(item) && x.Type == NavigationType.GroupUsers && !x.IsDeleted);
                                    if (userGr == null) continue;
                                    Information userInfo = new Information
                                    {
                                        id = userGr.Id.ToString().ToUpper(),
                                        name = userGr.Name
                                    };
                                    listUserGroups.Add(userInfo);
                                }
                            }
                            catch (MembershipCreateUserException ex)
                            {
                                _logger.LogError("Error: " + ex.Message);
                                continue;
                            }
                        }
                    }
                    else
                    {
                        addNavigation.UserGroups = null;
                    }
                }

                var listNonUserGroups = new List<Information>();
                if (args.NonUserGroups != null)
                {
                    string[] nonUserGroupsArr = ((IEnumerable)args.NonUserGroups).Cast<object>().Select(x => x.ToString()).Where(x => x != null).ToArray();
                    if (nonUserGroupsArr.Count() > 0)
                    {
                        foreach (var item in nonUserGroupsArr)
                        {
                            try
                            {
                                var isGuid = IsGuid(item);
                                if (isGuid)
                                {
                                    var userGr = await _uow.GetRepository<Navigation>().GetSingleAsync(x => x.Id == new Guid(item) && x.Type == NavigationType.GroupUsers && !x.IsDeleted);
                                    if (userGr == null) continue;
                                    Information userInfo = new Information
                                    {
                                        id = userGr.Id.ToString().ToUpper(),
                                        name = userGr.Name
                                    };
                                    listNonUserGroups.Add(userInfo);
                                }
                            }
                            catch (MembershipCreateUserException ex)
                            {
                                _logger.LogError("Error: " + ex.Message);
                                continue;
                            }
                        }
                    }
                    else
                    {
                        addNavigation.NonUserGroups = null;
                    }
                }

                if (listJobgrades.Count > 0)
                {
                    addNavigation.JobGrades = JsonConvert.SerializeObject(listJobgrades);
                }
                if (listUserGroups.Count > 0)
                {
                    addNavigation.UserGroups = JsonConvert.SerializeObject(listUserGroups);
                }
                if (listNonUserGroups.Count > 0)
                {
                    addNavigation.NonUserGroups = JsonConvert.SerializeObject(listNonUserGroups);
                }
                if (listDepartments.Count > 0)
                {
                    addNavigation.Departments = JsonConvert.SerializeObject(listDepartments);
                }
                if (listUsers.Count > 0)
                {
                    addNavigation.Users = JsonConvert.SerializeObject(listUsers);
                }
                if (listPermissions.Count > 0)
                {
                    addNavigation.Permissions = JsonConvert.SerializeObject(listPermissions);
                }
                // lop con
                if (args.ParentId != null)
                {
                    addNavigation.ParentId = args.ParentId;
                    addNavigation.Module = args.Module;
                }
                // do uu tien
                addNavigation.Priority = args.Priority;
                addNavigation.IsAD = args.IsAD;
                addNavigation.IsMS = args.IsMS;
                _uow.GetRepository<Navigation>().Add(addNavigation);
                result.Object = addNavigation.Id;
                await _uow.CommitAsync();
            }
            catch (MembershipCreateUserException e)
            {
                result.ErrorCodes.Add((int)e.StatusCode);
                result.Messages.Add(e.StatusCode.ToString());
            }
            return result;
        }

        public async Task<ResultDTO> UpdateNavigation(NavigationDataForCreatingArgs args)
        {
            var result = new ResultDTO();
            List<String> InforColumn = new List<String> { "code", "id" };
            try
            {
                if (object.ReferenceEquals(args, null))
                {
                    return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Error !" } };
                }
                var currentNavigation = await _uow.GetRepository<Navigation>().GetSingleAsync(x => x.Id == args.Id && !x.IsDeleted, string.Empty);
                if (currentNavigation == null)
                {
                    return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Navigation is not exist" } };
                }

                if (object.ReferenceEquals(args.Type, null))
                {
                    return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Navigation Type is required !" } };
                }

                switch (args.Type)
                {
                    case NavigationType.Center:
                    case NavigationType.Right:
                    case NavigationType.Left:
                        if (object.ReferenceEquals(args.Priority, null))
                        {
                            return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Priority Type is required !" } };
                        }

                        if (string.IsNullOrEmpty(args.Title_EN) || string.IsNullOrEmpty(args.Title_VI))
                        {
                            return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Title is required !" } };
                        }
                        currentNavigation.Title_VI = args.Title_VI;
                        currentNavigation.Title_EN = args.Title_EN;
                        currentNavigation.Module = args.Module;
                        // do uu tien
                        currentNavigation.Priority = args.Priority;
                        break;
                    case NavigationType.GroupUsers:
                        // Chi co 1 ten nen dung name chu khong dung title_vi
                        if (string.IsNullOrEmpty(args.Name))
                        {
                            return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Name is required !" } };
                        }
                        currentNavigation.Name = args.Name;
                        break;
                }

                if ((args.ParentId != null && string.IsNullOrEmpty(args.Url)) || (args.Type.Equals(NavigationType.Left) && string.IsNullOrEmpty(args.Url)))
                {
                    return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Url is required !" } };
                }
                currentNavigation.Url = args.Url;
                currentNavigation.Type = args.Type;

                List<Information> listDepartments = new List<Information>();
                // validate trung ten
                if (args.Departments != null)
                {
                    string[] deptArr = ((IEnumerable)args.Departments).Cast<object>().Select(x => x.ToString()).Where(x => x != null).ToArray();
                    if (deptArr.Count() > 0)
                    {
                        foreach (var item in deptArr)
                        {
                            try
                            {
                                var isGuid = IsGuid(item);
                                if (isGuid)
                                {
                                    var department = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == new Guid(item));
                                    if (department == null) continue;
                                    Information dept = new Information
                                    {
                                        id = department.Id.ToString().ToUpper(),
                                        name = department.Name,
                                        code = department.Code
                                    };
                                    listDepartments.Add(dept);
                                }
                            }
                            catch (MembershipCreateUserException ex)
                            {
                                _logger.LogError("Error: " + ex.Message);
                                continue;
                            }
                        }
                        if (listDepartments.Count > 0)
                        {
                            currentNavigation.Departments = JsonConvert.SerializeObject(listDepartments);
                        }
                    }
                    else
                    {
                        currentNavigation.Departments = null;
                    }
                }

                List<Information> listUsers = new List<Information>();
                if (args.Users != null)
                {
                    string[] userArr = ((IEnumerable)args.Users).Cast<object>().Select(x => x.ToString()).Where(x => x != null).ToArray();
                    if (userArr.Count() > 0)
                    {
                        foreach (var item in userArr)
                        {
                            try
                            {
                                var isGuid = IsGuid(item);
                                if (isGuid)
                                {
                                    var user = await _uow.GetRepository<User>().GetSingleAsync(x => x.Id == new Guid(item));
                                    if (user == null) continue;
                                    Information userInfo = new Information
                                    {
                                        id = user.Id.ToString().ToUpper(),
                                        name = user.FullName,
                                        code = user.SAPCode
                                    };
                                    listUsers.Add(userInfo);
                                }
                            }
                            catch (MembershipCreateUserException ex)
                            {
                                _logger.LogError("Error: " + ex.Message);
                                continue;
                            }
                        }
                        if (listUsers.Count > 0)
                        {
                            currentNavigation.Users = JsonConvert.SerializeObject(listUsers);
                        }
                    }
                    else
                    {
                        currentNavigation.Users = null;
                    }
                }

                var listPermissions = new List<Information>();
                if (args.Permissions != null)
                {
                    string[] permissionArr = ((IEnumerable)args.Permissions).Cast<object>().Select(x => x.ToString()).Where(x => x != null).ToArray();
                    if (permissionArr.Count() > 0)
                    {
                        foreach (var item in permissionArr)
                        {
                            try
                            {
                                if (item.Contains("code") && item.Contains("name") && item.Contains("id"))
                                {
                                    Information parsePermission = Mapper.Map<Information>(JsonConvert.DeserializeObject<Information>(item));
                                    if (parsePermission != null)
                                    {
                                        listPermissions.Add(parsePermission);
                                    }
                                }
                                else
                                {
                                    Information info = new Information
                                    {
                                        code = item
                                    };
                                    listPermissions.Add(info);
                                }

                            }
                            catch (MembershipCreateUserException ex)
                            {
                                _logger.LogError("Error: " + ex.Message);
                                continue;
                            }
                        }
                    }
                    else
                    {
                        currentNavigation.Permissions = null;
                    }
                }
                if (listPermissions.Count > 0)
                {
                    currentNavigation.Permissions = JsonConvert.SerializeObject(listPermissions);
                }

                var listJobGrades = new List<Information>();
                if (args.Jobgrades != null)
                {
                    string[] jobGradeArr = ((IEnumerable)args.Jobgrades).Cast<object>().Select(x => x.ToString()).Where(x => x != null).ToArray();
                    if (jobGradeArr.Count() > 0)
                    {
                        foreach (var item in jobGradeArr)
                        {
                            try
                            {
                                if (item.Contains("code") && item.Contains("name") && item.Contains("id"))
                                {
                                    Information parseJobGrade = Mapper.Map<Information>(JsonConvert.DeserializeObject<Information>(item));
                                    if (parseJobGrade != null)
                                    {
                                        listJobGrades.Add(parseJobGrade);
                                    }
                                }
                                else
                                {
                                    Information info = new Information
                                    {
                                        code = item
                                    };
                                    listJobGrades.Add(info);
                                }

                            }
                            catch (MembershipCreateUserException ex)
                            {
                                _logger.LogError("Error: " + ex.Message);
                                continue;
                            }
                        }
                    }
                    else
                    {
                        currentNavigation.JobGrades = null;
                    }
                }
                if (listJobGrades.Count > 0)
                {
                    currentNavigation.JobGrades = JsonConvert.SerializeObject(listJobGrades);
                }

                var listUserGroups = new List<Information>();
                if (args.UserGroups != null)
                {
                    string[] userGroupsArr = ((IEnumerable)args.UserGroups).Cast<object>().Select(x => x.ToString()).Where(x => x != null).ToArray();
                    if (userGroupsArr.Count() > 0)
                    {
                        foreach (var item in userGroupsArr)
                        {
                            try
                            {
                                var isGuid = IsGuid(item);
                                if (isGuid)
                                {
                                    var userGr = await _uow.GetRepository<Navigation>().GetSingleAsync(x => x.Id == new Guid(item) && x.Type == NavigationType.GroupUsers && !x.IsDeleted);
                                    if (userGr == null) continue;
                                    Information userInfo = new Information
                                    {
                                        id = userGr.Id.ToString().ToUpper(),
                                        name = userGr.Name
                                    };
                                    listUserGroups.Add(userInfo);
                                }
                            }
                            catch (MembershipCreateUserException ex)
                            {
                                _logger.LogError("Error: " + ex.Message);
                                continue;
                            }
                        }
                    }
                    else
                    {
                        currentNavigation.UserGroups = null;
                    }
                }
                if (listUserGroups.Count > 0)
                {
                    currentNavigation.UserGroups = JsonConvert.SerializeObject(listUserGroups);
                }

                var listNonUserGroups = new List<Information>();
                if (args.NonUserGroups != null)
                {
                    string[] nonUserGroupsArr = ((IEnumerable)args.NonUserGroups).Cast<object>().Select(x => x.ToString()).Where(x => x != null).ToArray();
                    if (nonUserGroupsArr.Count() > 0)
                    {
                        foreach (var item in nonUserGroupsArr)
                        {
                            try
                            {
                                var isGuid = IsGuid(item);
                                if (isGuid)
                                {
                                    var userGr = await _uow.GetRepository<Navigation>().GetSingleAsync(x => x.Id == new Guid(item) && x.Type == NavigationType.GroupUsers && !x.IsDeleted);
                                    if (userGr == null) continue;
                                    Information userInfo = new Information
                                    {
                                        id = userGr.Id.ToString().ToUpper(),
                                        name = userGr.Name
                                    };
                                    listNonUserGroups.Add(userInfo);
                                }
                            }
                            catch (MembershipCreateUserException ex)
                            {
                                _logger.LogError("Error: " + ex.Message);
                                continue;
                            }
                        }
                    }
                    else
                    {
                        currentNavigation.NonUserGroups = null;
                    }
                }
                if (listNonUserGroups.Count > 0)
                {
                    currentNavigation.NonUserGroups = JsonConvert.SerializeObject(listNonUserGroups);
                }

                // lop con
                if (args.ParentId != null)
                {
                    currentNavigation.ParentId = args.ParentId;
                }
                currentNavigation.IsAD = args.IsAD;
                currentNavigation.IsMS = args.IsMS;
                _uow.GetRepository<Navigation>().Update(currentNavigation);
                result.Object = currentNavigation.Id;
                await _uow.CommitAsync();
            }
            catch (MembershipCreateUserException e)
            {
                result.ErrorCodes.Add((int)e.StatusCode);
                result.Messages.Add(e.StatusCode.ToString());
            }
            return result;
        }

        public static bool IsGuid(string value)
        {
            Guid x;
            return Guid.TryParse(value, out x);
        }

        public static bool IsNull(object T)
        {
            return (bool)T ? true : false;
        }
        public async Task<ResultDTO> DeleteNavigationById(Guid Id)
        {
            var result = new ResultDTO();
            try
            {
                var currentNavigation = await _uow.GetRepository<Navigation>().GetSingleAsync(x => x.Id == Id, string.Empty);
                if (currentNavigation == null)
                {
                    return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Navigation is not exist" } };
                }
                currentNavigation.IsDeleted = true;
                _uow.GetRepository<Navigation>().Update(currentNavigation);
                result.Object = currentNavigation.Id;
                await _uow.CommitAsync();
            }
            catch (MembershipCreateUserException e)
            {
                result.ErrorCodes.Add((int)e.StatusCode);
                result.Messages.Add(e.StatusCode.ToString());
            }
            return result;
        }

        public async Task<ResultDTO> UpdateImageNavigation(NavigationDataForCreatingArgs data)
        {
            var result = new ResultDTO();
            try
            {
                var currentNavigation = await _uow.GetRepository<Navigation>().GetSingleAsync(x => x.Id == data.Id);
                if (currentNavigation == null)
                {
                    return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Navigation not exist !" } };
                }
                currentNavigation.ProfilePictureId = data.ProfilePictureId;
                _uow.GetRepository<Navigation>().Update(currentNavigation);
                await _uow.CommitAsync();
                result.Object = Mapper.Map<NavigationViewModel>(currentNavigation);
            }
            catch (MembershipCreateUserException e)
            {
                result.ErrorCodes.Add((int)e.StatusCode);
                result.Messages.Add(e.StatusCode.ToString());
            }
            return result;
        }
    }
}
