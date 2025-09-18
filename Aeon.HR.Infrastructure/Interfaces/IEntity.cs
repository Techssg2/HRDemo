using System;

namespace Aeon.HR.Infrastructure.Interfaces
{
    public interface IEntity
    {
        Guid Id { get; set; }
        DateTimeOffset Created { get; set; }
        DateTimeOffset Modified { get; set; }
    }
}
