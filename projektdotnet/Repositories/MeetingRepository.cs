using Microsoft.EntityFrameworkCore;
using projektdotnet.Data;
using projektdotnet.Models;

namespace projektdotnet.Repositories
{
    public class MeetingRepository
    {
        private readonly NewDbContext _context;
        public MeetingRepository(NewDbContext context)
        {
            _context = context;
        }
        public async Task<List<Meeting>> GetTodaysMeetingsForEmployee(int employeeId)
        {
            return await _context.Meetings
                .Include(r => r.room)
                .Include(p => p.Participants)
                .Where(e => e.StartingTime.Date == DateTime.Today && e.Participants.Any(p => p.EmployeeId == employeeId))
                .ToListAsync();
        }
        public async Task<Meeting> GetMeetingById(int? meetingId)
        {
            if (meetingId == null)
            {
                return null;
            }
            return await _context.Meetings
               .Include(r => r.room)
               .Include(p => p.Participants)
               .Where(e => e.MeetingId == meetingId).FirstOrDefaultAsync();
        }
        public async Task<List<Meeting>> GetMeetingsWithParticipant(int employeeId)
        {
            return await _context.Meetings.Include(m => m.room)
                .Where(e => e.Participants.Any(e => e.EmployeeId == employeeId))
                .OrderBy(e => e.StartingTime).ToListAsync();
        }
        public async Task UpdateMeeting(Meeting meeting)
        {
            _context.Meetings.Update(meeting);
            await _context.SaveChangesAsync();
        }
        public async Task AddMeeting(Meeting meeting)
        {
            _context.Add(meeting);
            await _context.SaveChangesAsync();
        }
        public async Task RemoveMeeting(Meeting meeting)
        {
            _context.Remove(meeting);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> MeetingExists(int id)
        {
            return await _context.Meetings.AnyAsync(e => e.MeetingId == id);
        }
    }
}
