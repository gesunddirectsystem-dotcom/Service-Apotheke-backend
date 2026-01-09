using Microsoft.EntityFrameworkCore;
using Service_Apotheke.Services.Email;
using ServiceApothekeAPI.Data;

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
                .Include(jp => jp.JobApplications)
                .Where(jp => jp.JobApplications.Any(ja => ja.PharmacistId == pharmacistId))
                .Include(jp => jp.Pharmacy)
                .OrderByDescending(jp => jp.ShiftDate)
                .ToListAsync();
        }
        public async Task<JobPost> GetJobPostById(Guid id)
        {
            return await _context.JobPosts
                .Include(jp => jp.Pharmacy) // مهم جداً عشان يعرض اسم الصيدلية وعنوانها
                .FirstOrDefaultAsync(jp => jp.Id == id);
        }
        public async Task<bool> UpdateJobPostStatus(Guid id, string status)
        {
            var jobPost = await _context.JobPosts
                .Include(jp => jp.JobApplications)
                .FirstOrDefaultAsync(jp => jp.Id == id);

            if (jobPost == null) return false;

            // لو عليه متقدمين مينفعش Active
            if (status == "Cancelled" && jobPost.JobApplications.Any(a => a.Status == "Accepted"))
                throw new Exception("لا يمكن إلغاء الشيفت بعد قبول صيدلي");

            jobPost.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<JobPost> CreateJobPost(CreateJobPostDto dto)
        {
            var pharmacy = await _context.Pharmacies.FindAsync(dto.PharmacyId);
            if (pharmacy == null)
                throw new Exception("Pharmacy not found");

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
            // Check if pharmacist exists and profile is completed
            var pharmacist = await _context.Pharmacists.FindAsync(dto.PharmacistId);
            if (pharmacist == null)
                throw new Exception("Pharmacist not found");

            if (!pharmacist.IsProfileCompleted)
                throw new Exception("Please complete your profile first");

            // Check if job post exists
            var jobPost = await _context.JobPosts
                .Include(jp => jp.Pharmacy)
                .FirstOrDefaultAsync(jp => jp.Id == dto.JobPostId);
            if (jobPost == null)
                throw new Exception("Job post not found");

            // Check if already applied
            var existingApplication = await _context.JobApplications
                .FirstOrDefaultAsync(ja => ja.JobPostId == dto.JobPostId && ja.PharmacistId == dto.PharmacistId);
            if (existingApplication != null)
                throw new Exception("Already applied for this shift");

            // Create application
            var application = new JobApplication
            {
                JobPostId = dto.JobPostId,
                PharmacistId = dto.PharmacistId,
                Status = "Pending",
                AppliedAt = DateTime.UtcNow
            };

            await _context.JobApplications.AddAsync(application);
            await _context.SaveChangesAsync();

            // Create notification for pharmacy
            var notification = new Notification
            {
                UserId = jobPost.PharmacyId,
                Title = "New Application",
                Message = $"Pharmacist {pharmacist.FullName} applied for your shift on {jobPost.ShiftDate:yyyy-MM-dd}",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();

            // Send email notification to pharmacy
            await _emailService.SendApplicationNotification(jobPost.Pharmacy.Email, pharmacist.FullName, jobPost.ShiftDate);

            return "Application submitted successfully";
        }

        public async Task<string> RespondToApplication(RespondDto dto)
        {
            var application = await _context.JobApplications
                .Include(ja => ja.JobPost)
                .Include(ja => ja.Pharmacist)
                .FirstOrDefaultAsync(ja => ja.Id == dto.ApplicationId);

            if (application == null)
                throw new Exception("Application not found");

            if (dto.NewStatus != "Accepted" && dto.NewStatus != "Rejected")
                throw new Exception("Invalid status");

            application.Status = dto.NewStatus;

            if (dto.NewStatus == "Accepted")
            {
                // Create shift record
                var shift = new Shift
                {
                    JobPostId = application.JobPostId,
                    PharmacistId = application.PharmacistId,
                    Status = "Scheduled",
                    CreatedAt = DateTime.UtcNow
                };

                await _context.Shifts.AddAsync(shift);
            }

            await _context.SaveChangesAsync();

            // Create notification for pharmacist
            var notification = new Notification
            {
                UserId = application.PharmacistId,
                Title = "Application Update",
                Message = $"Your application for shift on {application.JobPost.ShiftDate:yyyy-MM-dd} has been {dto.NewStatus.ToLower()}",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();

            return $"Application {dto.NewStatus.ToLower()} successfully";
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
                .Where(jp => jp.Status == "Active" && jp.ShiftDate >= DateTime.UtcNow.Date)
                .OrderBy(jp => jp.ShiftDate)
                .ToListAsync();
        }

        public async Task<List<object>> GetPharmacistApplications(Guid pharmacistId)
        {
            var applications = await _context.JobApplications
                .Where(ja => ja.PharmacistId == pharmacistId)
                .Include(ja => ja.JobPost)
                    .ThenInclude(jp => jp.Pharmacy)
                .OrderByDescending(ja => ja.AppliedAt)
                .Select(ja => new
                {
                    ja.Id, // Application ID
                    ja.Status,
                    ja.AppliedAt,
                    JobPostId = ja.JobPostId,
                    ShiftDate = ja.JobPost.ShiftDate,
                    StartTime = ja.JobPost.StartTime,
                    EndTime = ja.JobPost.EndTime,
                    PharmacyId = ja.JobPost.PharmacyId,
                    PharmacyName = ja.JobPost.Pharmacy.PharmacyName
                })
                .ToListAsync();

            return applications.Cast<object>().ToList();
        }


        public async Task<List<object>> GetJobPostApplications(Guid jobPostId)
        {
            var applications = await _context.JobApplications
                .Where(ja => ja.JobPostId == jobPostId)
                .Include(ja => ja.Pharmacist)
                .Select(ja => new {
                    ja.Id,
                    ja.Status,
                    ja.AppliedAt,
                    PharmacistId = ja.PharmacistId,
                    PharmacistName = ja.Pharmacist.FullName,
                    PharmacistPhone = ja.Pharmacist.Phone
                })
                .ToListAsync();

            return applications.Cast<object>().ToList();
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
            if (notification == null)
                return false;

            notification.IsRead = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
