using System.ComponentModel.DataAnnotations;

namespace Tutorial8.Models.DTOs;

public class ClientDTO
{
    [Range(1, 120)]
    public String  FirstName { get; set; }
    
    [Range(1, 120)]
    public String LastName { get; set; }
    
    [Range(1, 120)]
    public String Email { get; set; }
    
    [Range(1, 120)]
    public String Telephone { get; set; }
    
    [Range(1, 120)]
    public String Pesel { get; set; }
}