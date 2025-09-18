using Aeon.Academy.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.Academy.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("AcademyDbContext")
        {
            Database.SetInitializer<AppDbContext>(null);
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<TrainingRequest> TrainingRequests { get; set; }
        public DbSet<TrainingDurationItem> TrainingDurationItems { get; set; }
        public DbSet<TrainingRequestHistory> TrainingRequestHistories { get; set; }
        public DbSet<TrainingRequestParticipant> TrainingRequestParticipants { get; set; }
        public DbSet<TrainingInvitation> TrainingInvitations { get; set; }
        public DbSet<TrainingInvitationParticipant> TrainingInvitationParticipants { get; set; }
        public DbSet<TrainingReport> TrainingReports { get; set; }
        public DbSet<TrainingReportHistory> TrainingReportHistories { get; set; }
        public DbSet<TrainingActionPlan> TrainingActionPlans { get; set; }
        public DbSet<TrainingSurveyQuestion> TrainingSurveyQuestions { get; set; }
        public DbSet<ReasonOfTrainingRequest> ReasonOfTrainingRequests { get; set; }
        public DbSet<ServiceQueue> ServiceQueues { get; set; }
        public DbSet<TrainingRequestCostCenter> TrainingRequestCostCenters { get; set; }
    }
}
