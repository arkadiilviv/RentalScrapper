using ImScoutAtWorker.Models;
using ImScoutAtWorker.Interfaces;
using System.Text.Json;
using RabbitMQ.Client.Events;

namespace ImScoutAtWorker
{
    public class MessageProcessor : IMessageProcessor
    {
        protected RentalContext RentalContext { get; }
        public MessageProcessor(RentalContext rentalContext)
        {
            RentalContext = rentalContext;
        }
        public async Task MessageReceivedHandler(RabbitMQMessage model)
        {
            model.Link = SystemInfo.PARSER_URL + model.Link;
            var flat = await FlatParser.ParseAsync(model);
            RentalContext.Flats.Add(flat);
            try
            {
                await RentalContext.SaveChangesAsync();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                // Skip if duplicate (do nothing)
                if (!ex.InnerException?.Message.Contains("duplicate") == true || 
                    !ex.InnerException?.Message.Contains("UNIQUE") == true)
                {
                    Console.WriteLine($"Error saving flat: {ex.Message}");
                    throw;
                }
            }
        }
    }
}