using Microsoft.EntityFrameworkCore;
using projektdotnet.Data;
using projektdotnet.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace projektdotnet.Repositories
{
    public class RoomRepository
    {
        private readonly NewDbContext _context;

        public RoomRepository(NewDbContext context)
        {
            _context = context;
        }

        public async Task<List<Room>> GetAllRooms()
        {
            return await _context.Rooms.ToListAsync();
        }

        public async Task<Room> GetRoomById(int? id)
        {
            return await _context.Rooms.FirstOrDefaultAsync(r => r.RoomId == id);
        }

        public async Task AddRoom(Room room)
        {
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRoom(Room room)
        {
            _context.Entry(room).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> RoomExists(int id)
        {
            return await _context.Rooms.AnyAsync(r => r.RoomId == id);
        }

        public async Task RemoveRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room != null)
            {
                _context.Rooms.Remove(room);
                await _context.SaveChangesAsync();
            }
        }
    }
}
