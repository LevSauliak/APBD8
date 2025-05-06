using System.ComponentModel.DataAnnotations;

namespace Tutorial8.Models.DTOs;

public class CreateClientDTO
{
    public String  FirstName { get; set; }
    
    public String LastName { get; set; }
    
    public String Email { get; set; }
    
    public String Telephone { get; set; }
    
    public String Pesel { get; set; }
    
    
    public bool IsValid()
    {
        if (FirstName.Length == 0 || LastName.Length == 0 || Email.Length == 0 || Telephone.Length == 0 || Pesel.Length == 0)
        {
            return false;
        }

        if (!Telephone.StartsWith("+"))
        {
            return false;
        }

        if (!Email.Contains("@") || Email.Split("@").Length != 2)
        {
            return false;
        }
        
        return true;
    }
}