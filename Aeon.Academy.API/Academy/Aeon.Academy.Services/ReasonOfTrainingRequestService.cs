using Aeon.Academy.Data;
using Aeon.Academy.Data.Entities;
using Aeon.Academy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aeon.Academy.Services
{
    public class ReasonOfTrainingRequestService : IReasonOfTrainingRequestService
    {
        private readonly IUnitOfWork<AppDbContext> unitOfWork = null;
        private readonly IGenericRepository<ReasonOfTrainingRequest> repository = null;

        public ReasonOfTrainingRequestService(IUnitOfWork<AppDbContext> unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            this.repository = unitOfWork.GetRepository<ReasonOfTrainingRequest>();
        }

        public ReasonOfTrainingRequest Get(Guid id)
        {
            return repository.Get(id);
        }

        public IList<ReasonOfTrainingRequest> List()
        {
            return repository.GetAll().OrderBy(x => x.CreatedDate).ToList();
        }

        public Guid Save(ReasonOfTrainingRequest reason)
        {
            if (reason.Id == Guid.Empty)
            {
                repository.Add(reason);
            }
            else
            {
                repository.Update(reason);
            }
            unitOfWork.Complete();

            return reason.Id;
        }

        public bool Delete(Guid id)
        {
            var existing = repository.Get(id);
            if (existing == null) return false;
            repository.Delete(existing);
            unitOfWork.Complete();
            return true;
        }
    }
}