using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.Infrastructure.Utilities;
using JobApproverNotification.src.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JobApproverNotification.src.Helpers
{
    public static class JobDashboardHelper
    {
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
                Utilities.WriteLogError("Error loading job tasks from sub modules." + error);
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
            if (sourceObj == null || targetObj == null)
                return;

            Type sourceType = sourceObj.GetType();
            Type targetType = targetObj.GetType();

            PropertyInfo[] sourceProperties = sourceType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            PropertyInfo[] targetProperties = targetType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var sourceProp in sourceProperties)
            {
                try
                {
                    object sourceValue = sourceProp.GetValue(sourceObj, null);

                    // Tìm thuộc tính đích có cùng tên
                    var targetProp = targetProperties.FirstOrDefault(p =>
                        string.Equals(p.Name, sourceProp.Name, StringComparison.OrdinalIgnoreCase));

                    if (targetProp == null || !targetProp.CanWrite)
                        continue;

                    // Kiểm tra xem thuộc tính có phải là List không
                    Type sourceValueType = sourceValue?.GetType();
                    if (sourceValueType != null && sourceValueType.IsGenericType &&
                        sourceValueType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        // Xử lý cho List properties
                        if (sourceValue is IEnumerable sourceEnumerable && targetObj is EdocTaskViewModel)
                        {
                            var sourceList = new List<object>();
                            foreach (var item in sourceEnumerable)
                            {
                                sourceList.Add(item);
                            }

                            // Map thành WorkflowTaskViewModel của job project
                            var mappedList = MapObjectList<WorkflowTaskViewModel>(sourceList);
                            targetProp.SetValue(targetObj, mappedList);
                        }
                        else if (sourceValue is IEnumerable && targetProp.PropertyType.IsGenericType)
                        {
                            // Xử lý cho các List khác
                            var targetListType = targetProp.PropertyType.GetGenericArguments()[0];
                            var sourceList = new List<object>();
                            foreach (var item in (IEnumerable)sourceValue)
                            {
                                sourceList.Add(item);
                            }

                            // Sử dụng reflection để gọi MapObjectList với đúng type
                            var method = typeof(JobDashboardHelper)
                                .GetMethod("MapObjectList", BindingFlags.NonPublic | BindingFlags.Static)
                                .MakeGenericMethod(targetListType);

                            var mappedList = method.Invoke(null, new object[] { sourceList });
                            targetProp.SetValue(targetObj, mappedList);
                        }
                    }
                    else
                    {
                        // Xử lý cho các property thường
                        if (sourceValue != null)
                        {
                            // Kiểm tra nếu cần convert type
                            if (targetProp.PropertyType == sourceProp.PropertyType)
                            {
                                targetProp.SetValue(targetObj, sourceValue);
                            }
                            else if (CanConvert(sourceValue, targetProp.PropertyType))
                            {
                                var convertedValue = ConvertValue(sourceValue, targetProp.PropertyType);
                                targetProp.SetValue(targetObj, convertedValue);
                            }
                            else
                            {
                                // Nếu là complex object, thử map object
                                if (!sourceProp.PropertyType.IsPrimitive &&
                                    sourceProp.PropertyType != typeof(string) &&
                                    sourceProp.PropertyType != typeof(DateTime) &&
                                    sourceProp.PropertyType != typeof(Guid))
                                {
                                    var newTargetObj = Activator.CreateInstance(targetProp.PropertyType);
                                    MapObjectProps(sourceValue, newTargetObj);
                                    targetProp.SetValue(targetObj, newTargetObj);
                                }
                            }
                        }
                        else
                        {
                            targetProp.SetValue(targetObj, null);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Utilities.WriteLogError($"Error mapping property {sourceProp.Name}" + ex);
                    continue;
                }
            }
        }

        private static bool CanConvert(object value, Type targetType)
        {
            try
            {
                if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    targetType = Nullable.GetUnderlyingType(targetType);
                }

                Convert.ChangeType(value, targetType);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static object ConvertValue(object value, Type targetType)
        {
            try
            {
                if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    targetType = Nullable.GetUnderlyingType(targetType);
                }

                return Convert.ChangeType(value, targetType);
            }
            catch
            {
                return null;
            }
        }
    }
}