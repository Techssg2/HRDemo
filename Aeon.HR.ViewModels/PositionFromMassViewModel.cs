using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class PositionFromMassViewModel
    {
        public Guid Id { get; set; }
        public string AlertQuantity { get; set; }
        public Guid? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int CategoryPriority { get; set; }
        public string content { get; set; }
        public string Description { get; set; }
        public string InternalName { get; set; }
        public bool IsActivated { get; set; }
        public bool IsFromEdoc { get; set; }
        public Guid? LocationId { get; set; }
        public string LocationName { get; set; }
        public string Name { get; set; }
        public string RequiredQuantity { get; set; }
        public Guid? TypeId { get; set; }
        public string TypeName { get; set; }
    }

    public class ItemsFromMassViewModel
    {
        public ItemsFromMassViewModel()
        {
            Items = new List<PositionFromMassViewModel>();
        }
        public List<PositionFromMassViewModel> Items { get; set; }
    }
}
