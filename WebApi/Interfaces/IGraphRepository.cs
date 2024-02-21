using WebApi.Dtos;

namespace WebApi.Interfaces
{
    public interface IGraphRepository
    {
        Task<ICollection<DateValueDto>> GetDailyAsync(Guid meterId, DateTime date);
        Task<ICollection<DateValueDto>> GetDailyAsyncFromRoomId(Guid roomId, DateTime date);
        Task<ICollection<DateValueDto>> GetMonthlyAsync(Guid meterId, DateTime date);
        Task<ICollection<DateValueDto>> GetMonthlyAsyncFromRoomId(Guid roomId, DateTime date);
        Task<ICollection<DateValueDto>> GetYearlyAsync(Guid meterId);
        Task<ICollection<DateValueDto>> GetYearlyAsyncFromRoomId(Guid roomId);
        Task<bool> IsRoomId(Guid id);
    }
}
