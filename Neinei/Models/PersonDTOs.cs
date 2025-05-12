namespace Neinei.Models;

public class ClientDto
{
    public string firstName { get; set; }
    public string lastName { get; set; }
    public DateTime dateOfBirth { get; set; }
}

public class MechanicDto
{
    public int mechanicId { get; set; }
    public string licenceNumber { get; set; }
}