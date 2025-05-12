namespace Neinei.Models;

public class VisitReturnDto
{
    public DateTime Date { get; set; }
    public ClientDto Client { get; set; }
    public MechanicDto Mechanic { get; set; }
    public List<ServiceDto> Services { get; set; }
}

public class ServiceDto
{
    public string ServiceName { get; set; }
    public decimal serviceFee { get; set; }
}

public class VisitDto
{
    public int VisitId { get; set; }
    public int ClientId { get; set; }
    public string MechanicLicenceNumber { get; set; }
    public List<ServiceDto> Services { get; set; }
}