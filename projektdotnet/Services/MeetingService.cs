using projektdotnet.Models;
using projektdotnet.Repositories;

namespace projektdotnet.Services
{
    public class MeetingService
    {
        private readonly MeetingRepository _meetingRepository;
        public MeetingService(MeetingRepository meetingRepository)
        {
            _meetingRepository = meetingRepository;
        }
        public async Task<List<Meeting>> GetTodaysMeetingsForEmployee(int employeeId)
        {
            return await _meetingRepository.GetTodaysMeetingsForEmployee(employeeId);
        }
        public async Task<Meeting> GetMeetingById(int? meetingId)
        {
            if (meetingId == null)
            {
                return null;
            }
            return await _meetingRepository.GetMeetingById(meetingId);
        }
        public async Task<List<Meeting>> GetMeetingsWithParticipant(int employeeId)
        {
            return await _meetingRepository.GetMeetingsWithParticipant(employeeId);
        }
        public async Task UpdateMeeting(Meeting meeting)
        {
            await _meetingRepository.UpdateMeeting(meeting);
        }
        public async Task AddMeeting(Meeting meeting)
        {
            await _meetingRepository.AddMeeting(meeting);
        }
        public async Task RemoveMeeting(Meeting meeting)
        {
            await _meetingRepository.RemoveMeeting(meeting);
        }
        public async Task<bool> MeetingExists(int id)
        {
            return await _meetingRepository.MeetingExists(id);
        }
    }
}
