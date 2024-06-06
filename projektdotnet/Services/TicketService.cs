using Microsoft.EntityFrameworkCore;
using projektdotnet.Models;
using projektdotnet.Repositories;

namespace projektdotnet.Services
{
    public class TicketService
    {
        private readonly TicketRepository _ticketRepository;
        private readonly EmployeeRepository _employeeRepository;

        public TicketService(TicketRepository ticketRepository, EmployeeRepository employeeRepository)
        {
            _ticketRepository = ticketRepository;
            _employeeRepository = employeeRepository;
        }
        //returns true if any ticket was sent and nulled bcs of this,otherwise false;
        public async Task<bool> NullifySenderTickets(Employee employee)
        {
            //check if any tickets were sent
            var SentTickets = await _ticketRepository.GetTicketsWithSenderById(employee.EmployeeId);
            if (SentTickets != null)
            {
                foreach (Ticket ticket in SentTickets)
                {
                    if (ticket.SenderId == employee.EmployeeId)
                    {
                        ticket.SenderId = null;
                        await _ticketRepository.UpdateTicket(ticket);
                    }
                }
                return true;
            }
            return false;
        }
        //returns 1 if moved succesfully,0 if arent any tickets to move, -1 if cant move because there is not any valid reciever 
        public async Task<int> MoveReceivedTickets(Employee employee)
        {
            var ticketsToMove = await _ticketRepository.GetTicketsWithReceiverById(employee.EmployeeId);
            if (ticketsToMove.Any())
            {
                var employeeWithLeastTickets = await _employeeRepository.GetEmployeeWithLeastTicketsExId(employee.EmployeeId);

                if (employeeWithLeastTickets != null)
                {
                    foreach (var ticket in ticketsToMove)
                    {
                        ticket.ReceiverId = employeeWithLeastTickets.EmployeeId;

                        await _ticketRepository.UpdateTicket(ticket);
                        return 1;
                    }
                }
                return -1;
            }
            return 0;
        }
        public async Task<List<Ticket>> GetTicketsWithReceiverById(int id)
        {
            return await _ticketRepository.GetTicketsWithReceiverById(id);
        }
        public async Task<List<Ticket>> GetTicketsWithSenderById(int id)
        {
            return await _ticketRepository.GetTicketsWithSenderById(id);
        }
        public async Task<Ticket> GetTicketById(int? id)
        {
            if (id == null)
            {
                return null;
            }
            return await _ticketRepository.GetTicketById(id);
        }
        public async Task UpdateTicket(Ticket ticket)
        {
            await _ticketRepository.UpdateTicket(ticket);
        }
        public async Task AddTicket(Ticket ticket)
        {
            await _ticketRepository.AddTicket(ticket);
        }
        public async Task<bool> TicketExists(int id)
        {
            return await _ticketRepository.TicketExists(id);
        }
        public async Task RemoveTicket(Ticket ticket)
        {
            await _ticketRepository.RemoveTicket(ticket);
        }
        public async Task<List<Ticket>> GetAllTickets()
        {
            return await _ticketRepository.GetAllTickets();
        }
        public async Task SaveDbChanges()
        {
            await _ticketRepository.SaveDbChanges();
        }
        public Ticket MakeReadyTicket(int id)
        {
            switch (id)
            {
                case 1:
                    return new Ticket
                    {
                        Category = TicketCategory.Payroll,
                        Status = TicketStatus.Sent,
                        Priority = TicketPriority.Emergency,
                        Description = "My payroll is being delayed"
                    };
                case 2:
                    return new Ticket
                    {
                        Category = TicketCategory.Benefits,
                        Status = TicketStatus.Sent,
                        Priority = TicketPriority.Normal,
                        Description = "My food discount card is clogged"
                    };
                case 3:
                    return new Ticket
                    {
                        Category = TicketCategory.General,
                        Status = TicketStatus.Sent,
                        Priority = TicketPriority.High,
                        Description = "Toilet isn't working"
                    };
                case 4:
                    return new Ticket
                    {
                        Category = TicketCategory.Conflicts,
                        Status = TicketStatus.Sent,
                        Priority = TicketPriority.Low,
                        Description = "Other Employees are rude"
                    };
                default:
                    throw new ArgumentException("Invalid ticket id");
            }
        }
        public async Task<List<Ticket>> GetAllResolvedTickets()
        {
            return await _ticketRepository.GetAllResolvedTickets();
        }
    }
}
