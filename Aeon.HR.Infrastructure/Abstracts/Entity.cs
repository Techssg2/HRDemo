using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;

namespace Aeon.HR.Infrastructure.Abstracts
{
    public abstract class Entity : IEntity
    {
        [Key]
        public virtual Guid Id { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }

        

    }
}