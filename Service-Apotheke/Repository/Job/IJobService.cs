namespace Service_Apotheke.Repository.Job
{
    public interface IJobService
    {
        Task<bool> UpdateJobPostStatus(Guid id, string status);

        Task<JobPost> CreateJobPost(CreateJobPostDto dto);
        Task<string> ApplyForShift(ApplyDto dto);
        Task<string> RespondToApplication(RespondDto dto);
        Task<List<JobPost>> GetPharmacyJobPosts(Guid pharmacyId);
        Task<List<JobPost>> GetActiveJobPosts();
        Task<List<JobPost>> GetPharmacistAppliedJobs(Guid pharmacistId);
        Task<JobPost> GetJobPostById(Guid id);
        Task<List<object>> GetPharmacistApplications(Guid pharmacistId);

        Task<List<object>> GetJobPostApplications(Guid jobPostId);
        Task<List<Notification>> GetUserNotifications(Guid userId);
        Task<bool> MarkNotificationAsRead(int notificationId);
    }
}
