using Neinei.Models;

namespace Neinei.Services;

public interface IVisitService
{
    public Task<VisitReturnDto> GetVisit(int id);
    public Task<int> CreateVisit(VisitDto visitDto);
    public Task<bool> DoesVisitExist(int id);
}