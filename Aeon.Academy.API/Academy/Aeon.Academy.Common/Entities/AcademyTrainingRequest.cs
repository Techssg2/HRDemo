using System.ComponentModel.DataAnnotations;
namespace Aeon.Academy.Common.Entities
{
    public class AcademyTrainingRequest
    {
        [Required]
        [StringLength(8)]
        public string Pernr { get; set; }

        [Required]
        public string Begda { get; set; }

        [Required]
        public string Endda { get; set; }

        [StringLength(30)]
        public string Ztrain_loc { get; set; }

        [Required]
        [StringLength(3)]
        public string Zprg_code { get; set; }

        [Required]
        [StringLength(50)]
        public string Zprogram { get; set; }

        [StringLength(5)]
        public string Zhours_day { get; set; }

        [StringLength(3)]
        public string Znumofday { get; set; }

        [StringLength(20)]
        public string Ztotal_hours { get; set; }

        [Required]
        [StringLength(8)]
        public string Zin_ex { get; set; }

        [StringLength(40)]
        public string Ztrainer { get; set; }

        [StringLength(50)]
        public string Zagency { get; set; }

        [StringLength(13)]
        public string Ztrain_cost { get; set; }

        [StringLength(30)]
        public string Ztrain_cont { get; set; }

        public string Zstart { get; set; }

        public string Zend { get; set; }

    }
}
