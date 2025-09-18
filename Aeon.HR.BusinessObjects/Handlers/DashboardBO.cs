using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.Infrastructure.Utilities;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.DTOs;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Handlers
{
    public class DashboardBO : IDashboardBO
    {
        private readonly ILogger _logger;
        private readonly IUnitOfWork _uow;
        private static List<EmployeeNodeViewModel> _nodes = null;
        private static object _lock = new object();
        public DashboardBO(IUnitOfWork uow, ILogger logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<ResultDTO> GetMyItems()
        {
            var items = new List<MyItemViewModel>();
            try
            {
                var ass = Assembly.GetAssembly(typeof(WorkflowInstance));
                var models = ass.GetTypes().Where(x => typeof(IWorkflowEntity).IsAssignableFrom(x));
                foreach (var model in models)
                {
                    var repository = DynamicInvoker.InvokeGeneric(_uow, "GetRepository", model);
                    var addedItems = (IEnumerable<MyItemViewModel>)(await DynamicInvoker.InvokeGenericAsync(repository, "FindByAsync", typeof(MyItemViewModel), "modified desc", 1, int.MaxValue, "createdById=@0", new object[] { _uow.UserContext.CurrentUserId }));
                    foreach (var addedItem in addedItems)
                    {
                        addedItem.ItemType = model.Name;
                    }
                    items.AddRange(addedItems);
                }

                //ncao2: add more my items from new modules
                items.AddRange(DashboardHelper.GetMyItems(_uow.UserContext.CurrentUserId));

                items = items.OrderByDescending(x => x.Modified).ToList();
                return new ResultDTO() { Object = items };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw ex;
            }
        }

        public ResultDTO GetEmployeeNodesByDepartment(Guid departmentId, int maxLvl = 3)
        {
            EnsureNodes();
            var result = new ResultDTO();
            var parentNode = _nodes.Where(x => x.DepartmentId == departmentId).FirstOrDefault();
            if (parentNode != null)
            {
                var rootNode = parentNode.DeepClone();
                ExpandAllNodes(rootNode, _nodes, rootNode.Grade - maxLvl);
                result.Object = new List<EmployeeNodeViewModel>() { rootNode };
            }
            return result;
        }

        private void EnsureNodes()
        {
            if (_nodes == null || _nodes.Count() == 0)
            {
                lock (_lock)
                {
                    //Double check
                    if (_nodes == null || _nodes.Count() == 0)
                    {
                        var nodes = new List<EmployeeNodeViewModel>();
                        var departments = _uow.GetRepository<Department>().GetAll("", x => x.UserDepartmentMappings.Select(t => t.User), x => x.JobGrade).ToList();
                        foreach (var department in departments)
                        {
                            var users = department.UserDepartmentMappings.Where(x => x.Role == Infrastructure.Enums.Group.Member && x.IsHeadCount).Select(x => x.User);
                            if (users.Count() > 0)
                            {
                                foreach (var user in users)
                                {
                                    var dataUser = Mapper.Map<UserListViewModel>(user);
                                    nodes.Add(new EmployeeNodeViewModel()
                                    {
                                        DepartmentId = department.Id,
                                        DepartmentCaption = department.PositionName,
                                        DepartmentName = department.Name,
                                        ColorCode = department.Color,
                                        JobGrade = department.JobGrade.Caption,
                                        Grade = department.JobGrade.Grade,
                                        ParentDepartmentId = department.ParentId,
                                        EmployeeName = user.FullName,
                                        EmployeeImage = dataUser.ProfilePicture
                                    });
                                }
                            }
                            else
                            {
                                nodes.Add(new EmployeeNodeViewModel()
                                {
                                    DepartmentId = department.Id,
                                    DepartmentCaption = department.PositionName,
                                    DepartmentName = department.Name,
                                    JobGrade = department.JobGrade.Caption,
                                    Grade = department.JobGrade.Grade,
                                    ColorCode = department.Color,
                                    ParentDepartmentId = department.ParentId
                                });
                            }
                        }
                        _nodes = nodes;
                    }
                }
            }
        }

        //private void ProccessTree(List<EmployeeNodeViewModel> nodes)
        //{
        //    var topNodes = nodes.Where(x => !x.ParentDepartmentId.HasValue).OrderBy(x => x.DepartmentName).ToList();
        //    foreach (var topNode in topNodes)
        //    {
        //        ExpandAllNodes(topNode, nodes);
        //    }
        //}

        private void ExpandAllNodes(EmployeeNodeViewModel parentNode, List<EmployeeNodeViewModel> nodes, int minLvl)
        {
            var childNodes = nodes.Where(x => x.ParentDepartmentId == parentNode.DepartmentId && x.Grade > minLvl).OrderBy(x => x.DepartmentName).Select(x => x.DeepClone()).ToList();
            if (childNodes.Count() > 0)
            {
                var minGrade = childNodes.Select(x => x.Grade).Min();
                EmployeeNodeViewModel lastVirtualNode = null;
                var groupNodes = childNodes.GroupBy(x => x.Grade).OrderBy(x => x.Key);

                foreach (var itemGroup in groupNodes)
                {
                    var deepLevel = parentNode.Grade - itemGroup.Key;
                    if (deepLevel == 1)
                    {
                        parentNode.Items.AddRange(itemGroup);
                    }
                    else if(deepLevel > 1)
                    {
                        foreach(var node in itemGroup)
                        {
                            lastVirtualNode = null;
                            for (var c = 0; c < deepLevel - 1; c++)
                            {
                                if (lastVirtualNode == null)
                                {
                                    lastVirtualNode = parentNode;
                                }
                                var vNode = new EmployeeNodeViewModel() { DepartmentName = "<Virtual>" };
                                lastVirtualNode.Items.Add(vNode);
                                lastVirtualNode = vNode;
                            }
                            lastVirtualNode.Items.Add(node);
                        }
                    }
                }
                foreach (var childNode in childNodes)
                {
                    ExpandAllNodes(childNode, nodes,  minLvl);
                }
            }
        }

        public ResultDTO ClearNode()
        {
            _nodes = null;
            return new ResultDTO() { };
        }
    }
}
