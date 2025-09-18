using Aeon.Academy.Data.Entities;
using System;
using System.Collections.Generic;

namespace Aeon.Academy.Services
{
    public interface IWorkflowService<T> where T : BaseWorkflowEntity
    {
        string GetWorkflowTemplate(string itemType, string name="");
        List<string> GetWorkflowActions(User currentUser, T request);

        void Approve(T request, string comment = "", User user = null);

        void Submit(T request, string comment = "", User user = null);

        void RequestToChange(T request, string comment = "", User user = null);

        void Reject(T request, string comment = "", User user = null);
        void Cancel(T request, string comment = "", User user = null);
    }
}