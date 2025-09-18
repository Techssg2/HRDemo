using System.ComponentModel.DataAnnotations;

namespace Aeon.Academy.API.DTOs
{
    public class TrainingSponsorshipContractDto
    {
        public bool ApplySponsorship { get; set; }
        [Range(0, 100, ErrorMessage = "Sponsorship must be between 0 to 100")]
        public int SponsorshipPercentage { get; set; }
        public decimal? ActualTuitionReimbursementAmount { get; set; }
    }
}