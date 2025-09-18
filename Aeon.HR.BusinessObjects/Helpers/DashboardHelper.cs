using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.Infrastructure.Utilities;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Helpers
{
    public static class DashboardHelper
    {
        private static readonly ILogger logger = ServiceLocator.Resolve<ILogger>();

        public static List<MyItemViewModel> GetMyItems(Guid userId)
        {
            var myItems = new List<MyItemViewModel>();

            try
            {
                var moduleInstances = GetDashboardnModules();
                if (moduleInstances == null || moduleInstances.Count() == 0) return myItems;

                foreach (var mi in moduleInstances)
                {
                    var items = mi.GetMyItems(userId);
                    var itemsFromModule = MapObjectList<MyItemViewModel>(items);
                    myItems.AddRange(itemsFromModule);
                }
            }
            catch (Exception error)
            {
                logger.LogError(error, "Error loading my items from sub modules.");
            }

            return myItems;
        }

        public static List<WorkflowTaskViewModel> GetMyTasks(Guid userId, QueryArgs args)
        {
            var myTasks = new List<WorkflowTaskViewModel>();

            try
            {
                var moduleInstances = GetDashboardnModules();
                if (moduleInstances == null || moduleInstances.Count() == 0) return myTasks;
                logger.LogError("GetMyTasks: " + moduleInstances.Count());
                foreach (var mi in moduleInstances)
                {
                    var @params = new Infrastructure.QueryArgs();
                    MapObjectPropsV2(args, @params);
                    var items = mi.GetMyTasks(userId, @params);
                    logger.LogError("items: " + JsonConvert.SerializeObject(items));
                    var itemsFromModule = MapObjectList<WorkflowTaskViewModel>(items);
                    myTasks.AddRange(itemsFromModule);
                }
            }
            catch (Exception error)
            {
                logger.LogError(error, "Error loading my tasks from sub modules.");
            }

            return myTasks;
        }

        public static List<EdocTaskViewModel> GetJobTasks()
        {
            var jobTasks = new List<EdocTaskViewModel>();

            try
            {
                var moduleInstances = GetDashboardnModules();
                if (moduleInstances == null || moduleInstances.Count() == 0) return jobTasks;

                foreach (var mi in moduleInstances)
                {
                    var items = mi.GetJobTasks();
                    var itemsFromModule = MapObjectList<EdocTaskViewModel>(items);
                    jobTasks.AddRange(itemsFromModule);
                }
            }
            catch (Exception error)
            {
                logger.LogError(error, "Error loading job tasks from sub modules.");
            }

            return jobTasks;
        }

        private static IEnumerable<IModuleDashboard> GetDashboardnModules()
        {
            var instances = ServiceLocator.ResolveAll<IModuleDashboard>();
            return instances;
        }

        private static List<T> MapObjectList<T>(IList<object> items) where T : class, new()
        {
            var returnItems = new List<T>();

            foreach (var item in items)
            {
                var newItem = new T();
                MapObjectProps(item, newItem);
                returnItems.Add(newItem);
            }

            return returnItems;
        }

        private static void MapObjectProps(object sourceObj, object targetObj)
        {
            Type T1 = sourceObj.GetType();
            Type T2 = targetObj.GetType();

            PropertyInfo[] sourceProprties = T1.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            PropertyInfo[] targetProprties = T2.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var sourceProp in sourceProprties)
            {
                object osourceVal = sourceProp.GetValue(sourceObj, null);
                var targetProp = targetProprties.First(p => p.Name == sourceProp.Name);
                if (targetProp != null)
                {

                    Type type = osourceVal?.GetType();
                    if (type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                    {

                        if (osourceVal is IEnumerable && targetObj is EdocTaskViewModel)
                        {
                            List<object> list = new List<object>();
                            var enumerator = ((IEnumerable)osourceVal).GetEnumerator();
                            while (enumerator.MoveNext())
                            {
                                list.Add(enumerator.Current);
                            }
                            var edoc = MapObjectList<WorkflowTaskViewModel>(list);
                            targetProp.SetValue(targetObj, edoc);
                        }
                    }
                    else
                        targetProp.SetValue(targetObj, osourceVal);
                }
            }
        }

        private static void MapObjectPropsV2(object sourceObj, object targetObj)
        {
            if (sourceObj == null || targetObj == null)
                throw new ArgumentNullException("Source or target object cannot be null.");

            // Lấy kiểu dữ liệu của đối tượng nguồn và đối tượng đích
            Type sourceType = sourceObj.GetType();
            Type targetType = targetObj.GetType();

            // Lấy danh sách các thuộc tính công khai từ đối tượng nguồn và đích
            PropertyInfo[] sourceProperties = sourceType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            PropertyInfo[] targetProperties = targetType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var sourceProp in sourceProperties)
            {
                // Lấy giá trị từ thuộc tính của đối tượng nguồn
                object sourceValue = sourceProp.GetValue(sourceObj, null);

                // Tìm thuộc tính đích có cùng tên (nếu không tìm thấy thì bỏ qua)
                var targetProp = targetProperties.FirstOrDefault(p => string.Equals(p.Name, sourceProp.Name, StringComparison.OrdinalIgnoreCase));
                if (targetProp == null || !targetProp.CanWrite)
                    continue;

                // Kiểm tra xem thuộc tính là danh sách (List<T>)
                Type sourceValueType = sourceValue?.GetType();
                if (sourceValueType != null && sourceValueType.IsGenericType && sourceValueType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    // Xử lý riêng cho kiểu danh sách
                    if (sourceValue is IEnumerable sourceEnumerable && targetObj is EdocTaskViewModel)
                    {
                        // Chuyển đổi danh sách thành đối tượng đích cụ thể
                        var sourceList = new List<object>();
                        foreach (var item in sourceEnumerable)
                        {
                            sourceList.Add(item);
                        }

                        // Gọi phương thức MapObjectList để ánh xạ danh sách
                        var mappedList = MapObjectList<WorkflowTaskViewModel>(sourceList);
                        targetProp.SetValue(targetObj, mappedList);
                    }
                }
                else
                {
                    // Gán giá trị cho thuộc tính đích nếu không phải danh sách
                    targetProp.SetValue(targetObj, sourceValue);
                }
            }
        }


    }
}
