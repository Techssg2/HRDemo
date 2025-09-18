using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using SyncOrgchartJob.Enums;
using SyncOrgchartJob.Models;
using SyncOrgchartJob.Models.eDocHR;

namespace SyncOrgchartJob
{
    public static class Helpers
    {
        public static List<StagingDetailDto> BuildTreeWithPersons(this List<StagingDetailDto> flatList)
        {
            // Dictionary tạm để truy xuất nhanh

            // Danh sách gốc
            List<StagingDetailDto> roots = new List<StagingDetailDto>();
            Dictionary<string, StagingDetailDto> lookup = null;
            try
            {
                lookup = flatList.ToDictionary(x => x.ObjectId, x => x);
            }
            catch (ArgumentException ex)
            {
                var duplicateKeys = flatList.GroupBy(x => x.ObjectId)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key);
                Logger.Write($"Duplicate keys: {string.Join(",", duplicateKeys)}", true);
                throw;
            }
            foreach (var item in flatList)
            {
                try
                {
                    if (item.ObjectType == "P")
                    {
                        // Đây là người — gán vào node cha có ObjectType = "S"
                        if (lookup.TryGetValue(item.ParentId, out var parent) && parent.ObjectType == "S")
                        {
                            parent.Persons.Add(item);
                        }// 00416984
                    }
                    else
                    {
                        if (item.ObjectId.Equals("50044135"))
                        {
                            var dsa = "";
                        }
                        
                        // Kiêm nhiệm vụ của node S
                        if (item.ObjectType == "S" && item.Concurrently.Equals("1"))
                        {
                            var findEmployee = flatList.FirstOrDefault(x =>
                                x.ObjectId.Equals(item.EmployeeId) && x.ObjectType.Equals(ObjectType.Person));
                            if (findEmployee != null)
                            {
                                // findEmployee.ParentId = item.ObjectId;
                                item.Persons.Add(findEmployee);
                            }
                        }
                        // Xử lý node S như bình thường
                        if (lookup.ContainsKey(item.ParentId))
                        {
                            /*if (item.ObjectType == "S")
                            {
                                var parent = lookup[item.ParentId];
                                parent.Positions.Add(item);
                            }
                            else
                            {
                                var parent = lookup[item.ParentId];
                                parent.Children.Add(item);
                            }*/
                            var parent = lookup[item.ParentId];
                            parent.Children.Add(item);
                        }
                        else
                        {
                            roots.Add(item);
                        }
                    }
                } catch (Exception ex)
                {
                    Console.WriteLine($"Error processing item with ObjectId {item.ObjectId}: {ex.Message}");
                    Logger.Write($"Error processing item with ObjectId {item.ObjectId}: {ex.Message}", true);
                    throw ex;
                }
            }

            return roots;
        }
        
        public static List<Department> BuildTree(this List<Department> flatList)
        {
            if (flatList == null || !flatList.Any())
                return new List<Department>();

            var roots = new List<Department>();

            foreach (var item in flatList)
            {
                try
                {
                    if (item.ParentId.HasValue)
                    {
                        var parent = flatList.FirstOrDefault(x => x.Id == item.ParentId.Value);

                        if (parent != null && parent.IsFromIT != true)
                        {
                            parent.Children.Add(item);
                        }
                        // Nếu parent là IT → không add gì cả (bỏ qua item)
                    }
                    else
                    {
                        // Không có parent (node gốc) → chỉ add nếu nó không phải từ IT
                        if (item.IsFromIT != true)
                        {
                            roots.Add(item);
                        }
                        // Nếu là IT thì bỏ qua luôn
                    }
                } catch (Exception ex)
                {
                    Console.WriteLine($"Error processing item with Id {item.Id}: {ex.Message}");
                    Logger.Write($"Error processing item with Id {item.Id}: {ex.Message}", true);
                    throw ex;
                }
            }

            return roots;
        }
        
        /*public static List<Department> BuildTree(this List<Department> flatList)
        {
            if (flatList == null || !flatList.Any())
                return new List<Department>();

            var lookup = flatList
                .GroupBy(x => x.Id)
                .Select(x => x.First()) // tránh trùng ID
                .ToDictionary(x => x.Id, x => x);

            var roots = new List<Department>();

            foreach (var item in flatList)
            {
                if (item.ParentId.HasValue && lookup.TryGetValue(item.ParentId.Value, out var parent))
                {
                    parent.Children.Add(item);
                }
                else
                {
                    roots.Add(item);
                }
            }

            return roots;
        }*/
        
        public static Department FindTreeHR(string sapCode, List<Department> departments)
        {
            foreach (var dept in departments)
            {
                if (dept.SAPCode == sapCode)
                    return dept;

                if (dept.Children != null)
                {
                    var found = FindTreeHR(sapCode, dept.Children);
                    if (found != null)
                        return found;
                }
            }
            return null;
        }
                    
        public static Department FindTreeHRById(Guid Id, List<Department> departments)
        {
            foreach (var dept in departments)
            {
                if (dept.Id == Id)
                    return dept;

                if (dept.Children != null)
                {
                    var found = FindTreeHRById(Id, dept.Children);
                    if (found != null)
                        return found;
                }
            }
            return null;
        }
        
        public static bool HasColumn(this SqlDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
                if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                    return true;
            return false;
        }
    }
}