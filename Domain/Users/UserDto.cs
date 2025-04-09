namespace ShopTex.Domain.Users;

public class UserDto
{
    public Guid Id { get; set; }
    
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    public string Phone { get; set; }
    
    public string Email { get; set; }
    
    public string? Role { get; set; }
    
    public string? Nif { get; set; }
    
    public string Status { get; set; }

    public UserDto(Guid id, string firstName, string lastName, string phone, UserEmail email, UserRole role, UserNif nif, UserStatus status)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Phone = phone;
        Email = email.Value;
        Role =  role != null ? role.RoleName : null;
        Nif = nif !=null ? nif.Value:null;
        Status = status.ToString();
    }
}