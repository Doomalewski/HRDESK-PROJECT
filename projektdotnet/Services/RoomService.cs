using projektdotnet.Models;
using projektdotnet.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace projektdotnet.Services
{
    public class RoomService
    {
        private readonly RoomRepository _roomRepository;

        public RoomService(RoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public async Task<List<Room>> GetAllRooms()
        {
            return await _roomRepository.GetAllRooms();
        }

        public async Task<Room> GetRoomById(int? id)
        {
            return await _roomRepository.GetRoomById(id);
        }

        public async Task AddRoom(Room room)
        {
            await _roomRepository.AddRoom(room);
        }

        public async Task UpdateRoom(Room room)
        {
            await _roomRepository.UpdateRoom(room);
        }

        public async Task<bool> RoomExists(int id)
        {
            return await _roomRepository.RoomExists(id);
        }

        public async Task RemoveRoom(int id)
        {
            await _roomRepository.RemoveRoom(id);
        }
    }
}
