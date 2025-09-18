using Aeon.Academy.Data.Entities;
using System;
using System.Collections.Generic;

namespace Aeon.Academy.Services
{
    public interface IQueueService
    {
        bool Delete(ServiceQueue item);
        ServiceQueue Get(Guid id);
        IList<ServiceQueue> ListAll(int disabled = 0);
        Guid Save(ServiceQueue item);
        IList<ServiceQueue> ListByInstanceType(string type, int disabled = 0);
        ServiceQueue GetBySapCode(string sapCode, Guid invtationId);
    }
}