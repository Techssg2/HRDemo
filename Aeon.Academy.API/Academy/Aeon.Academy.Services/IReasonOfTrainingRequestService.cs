using Aeon.Academy.Data.Entities;
using System;
using System.Collections.Generic;

namespace Aeon.Academy.Services
{
    public interface IReasonOfTrainingRequestService
    {
        bool Delete(Guid id);
        ReasonOfTrainingRequest Get(Guid id);
        IList<ReasonOfTrainingRequest> List();
        Guid Save(ReasonOfTrainingRequest reason);
    }
}