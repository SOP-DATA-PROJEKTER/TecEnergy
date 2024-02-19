using WebApi.Dtos;

namespace WebApi.Interfaces
{
    public interface IGraphRepository
    {
        Task<ICollection<DateValueDto>> GetDailyAsync(Guid meterId, DateTime date);
        Task<ICollection<DateValueDto>> GetMonthlyAsync(Guid meterId, DateTime date);
        Task<ICollection<DateValueDto>> GetYearlyAsync(Guid meterId);
    }
}
