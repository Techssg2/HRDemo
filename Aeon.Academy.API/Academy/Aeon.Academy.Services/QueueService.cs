using Aeon.Academy.Data;
using Aeon.Academy.Data.Entities;
using Aeon.Academy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aeon.Academy.Services
{
    public class QueueService : IQueueService
    {
        private readonly IUnitOfWork<AppDbContext> unitOfWork = null;
        private readonly IGenericRepository<ServiceQueue> repository = null;

        public QueueService(IUnitOfWork<AppDbContext> unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            this.repository = unitOfWork.GetRepository<ServiceQueue>();
        }

        public ServiceQueue Get(Guid id)
        {
            return repository.Get(id);
        }

        public IList<ServiceQueue> ListAll( int disabled = 0)
        {
            return repository.Query(q => q.Disabled == disabled).ToList();
        }
        public IList<ServiceQueue> ListByInstanceType(string type, int disabled = 0)
        {
            return repository.Query(q => q.InstanceType == type && q.Disabled == disabled).ToList();
        }
        public Guid Save(ServiceQueue item)
        {
            if (item.Id == Guid.Empty)
            {
                repository.Add(item);
            }
            else
            {
                repository.Update(item);
            }

            unitOfWork.Complete();

            return item.Id;
        }

        public bool Delete(ServiceQueue item)
        {
            if (item == null || item.Id == Guid.Empty) return false;

            var existing = repository.Get(item.Id);
            if(existing != null)
            {
                repository.Update(existing);
                unitOfWork.Complete();
            }            
            return true;
        }
        public ServiceQueue GetBySapCode(string sapCode, Guid invtationId)
        {
            return repository.Query(x => x.SapCode == sapCode && x.TrainingInvitationId.HasValue && x.TrainingInvitationId.Value == invtationId).FirstOrDefault();
        }

    }
}
