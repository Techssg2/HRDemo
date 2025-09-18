using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aeon.Academy.API.DTOs
{
    public class TrainingReportAttachmentDto
    {
        public object Certificate { get; set; }
        public object Material { get; set; }
    }
}