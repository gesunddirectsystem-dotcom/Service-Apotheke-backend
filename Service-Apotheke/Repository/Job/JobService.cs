using Microsoft.EntityFrameworkCore;
using Service_Apotheke.Services.Email;
using ServiceApothekeAPI.Data;
using Service_Apotheke.Models;

namespace Service_Apotheke.Repository.Job
{
    public class JobService : IJobService
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public JobService(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<List<JobPost>> GetPharmacistAppliedJobs(Guid pharmacistId)
        {
            return await _context.JobPosts
                .Include(jp => jp.Pharmacy)
                .Where(jp => jp.JobApplications.Any(ja => ja.PharmacistId == pharmacistId))
                .ToListAsync();
        }

        public async Task<JobPost> GetJobPostById(Guid id)
        {
            return await _context.JobPosts
                .Include(jp => jp.Pharmacy)
                .FirstOrDefaultAsync(jp => jp.Id == id);
        }

        public async Task<bool> UpdateJobPostStatus(Guid id, string status)
        {
            var jobPost = await _context.JobPosts
                .Include(jp => jp.JobApplications)
                .FirstOrDefaultAsync(jp => jp.Id == id);

            if (jobPost == null) return false;

            if (status == "Cancelled" && jobPost.JobApplications.Any(a => a.Status == "Accepted"))
                throw new Exception("لا يمكن إلغاء الشيفت بعد قبول صيدلي");

            jobPost.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<JobPost> CreateJobPost(CreateJobPostDto dto)
        {
            var jobPost = new JobPost
            {
                PharmacyId = dto.PharmacyId,
                ShiftDate = dto.ShiftDate,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                HasBreak = dto.HasBreak,
                BreakDurationInMinutes = dto.BreakDurationInMinutes,
                Salary = dto.Salary,
                HasTransportationAllowance = dto.HasTransportationAllowance,
                TransportationAllowanceAmount = dto.TransportationAllowanceAmount,
                Notes = dto.Notes,
                Status = "Active",
                CreatedAt = DateTime.UtcNow
            };

            await _context.JobPosts.AddAsync(jobPost);
            await _context.SaveChangesAsync();
            return jobPost;
        }

        public async Task<string> ApplyForShift(ApplyDto dto)
        {
            var pharmacist = await _context.Pharmacists.FindAsync(dto.PharmacistId);
            if (pharmacist == null) throw new Exception("Pharmacist not found");
            if (!pharmacist.IsProfileCompleted) throw new Exception("Please complete your profile first");

            var jobPost = await _context.JobPosts.Include(jp => jp.Pharmacy).FirstOrDefaultAsync(jp => jp.Id == dto.JobPostId);
            if (jobPost == null) throw new Exception("Job post not found");

            var existingApplication = await _context.JobApplications
                .AnyAsync(ja => ja.JobPostId == dto.JobPostId && ja.PharmacistId == dto.PharmacistId);
            if (existingApplication) throw new Exception("Already applied");

            var application = new JobApplication
            {
                JobPostId = dto.JobPostId,
                PharmacistId = dto.PharmacistId,
                Status = "Pending",
                AppliedAt = DateTime.UtcNow
            };

            await _context.JobApplications.AddAsync(application);

            var notification = new Notification
            {
                UserId = jobPost.PharmacyId,
                Title = "New Application",
                Message = $"Pharmacist {pharmacist.FullName} applied for {jobPost.ShiftDate:yyyy-MM-dd}",
                CreatedAt = DateTime.UtcNow
            };
            await _context.Notifications.AddAsync(notification);

            await _context.SaveChangesAsync();
            await _emailService.SendApplicationNotification(jobPost.Pharmacy.Email, pharmacist.FullName, jobPost.ShiftDate);

            return "Application submitted successfully";
        }

        public async Task<string> RespondToApplication(RespondDto dto)
        {
            var application = await _context.JobApplications
                .Include(ja => ja.JobPost)
                .FirstOrDefaultAsync(ja => ja.Id == dto.ApplicationId);

            if (application == null) throw new Exception("Application not found");

            application.Status = dto.NewStatus;

            if (dto.NewStatus == "Accepted")
            {
                var shift = new Shift
                {
                    JobPostId = application.JobPostId,
                    PharmacistId = application.PharmacistId,
                    Status = "Scheduled",
                    CreatedAt = DateTime.UtcNow
                };
                await _context.Shifts.AddAsync(shift);
            }

            var notification = new Notification
            {
                UserId = application.PharmacistId,
                Title = "Application Update",
                Message = $"Your application for {application.JobPost.ShiftDate:yyyy-MM-dd} was {dto.NewStatus}",
                CreatedAt = DateTime.UtcNow
            };
            await _context.Notifications.AddAsync(notification);

            await _context.SaveChangesAsync();
            return $"Application {dto.NewStatus} successfully";
        }

        public async Task<List<JobPost>> GetPharmacyJobPosts(Guid pharmacyId)
        {
            return await _context.JobPosts
                .Where(jp => jp.PharmacyId == pharmacyId)
                .OrderByDescending(jp => jp.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<JobPost>> GetActiveJobPosts()
        {
            return await _context.JobPosts
                .Include(jp => jp.Pharmacy)
                .Where(jp => jp.Status == "Active" && jp.ShiftDate >= DateTime.UtcNow.Date)
                .OrderBy(jp => jp.ShiftDate)
                .ToListAsync();
        }

        public async Task<List<object>> GetPharmacistApplications(Guid pharmacistId)
        {
            return await _context.JobApplications
                .Where(ja => ja.PharmacistId == pharmacistId)
                .Include(ja => ja.JobPost)
                .ThenInclude(jp => jp.Pharmacy)
                .Select(ja => new
                {
                    ja.Id,
                    ja.Status,
                    ja.AppliedAt,
                    ja.JobPostId,
                    ja.JobPost.ShiftDate,
                    ja.JobPost.Pharmacy.PharmacyName
                })
                .Cast<object>()
                .ToListAsync();
        }

        public async Task<List<object>> GetJobPostApplications(Guid jobPostId)
        {
            return await _context.JobApplications
                .Where(ja => ja.JobPostId == jobPostId)
                .Include(ja => ja.Pharmacist)
                .Select(ja => new {
                    ja.Id,
                    ja.Status,
                    ja.AppliedAt,
                    ja.Pharmacist.FullName,
                    ja.Pharmacist.CvPath,
                    ja.Pharmacist.Phone
                })
                .Cast<object>()
                .ToListAsync();
        }

        public async Task<List<Notification>> GetUserNotifications(Guid userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> MarkNotificationAsRead(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification == null) return false;
            notification.IsRead = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}