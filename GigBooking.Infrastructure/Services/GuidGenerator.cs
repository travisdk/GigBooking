
using GigBooking.Application.Common.Interfaces;
namespace GigBooking.Infrastructure.Services
{
    public class GuidGenerator : IGuidGenerator
    {
        public Guid GenerateGuid()
        {
           return Guid.CreateVersion7();
        }
    }
}
