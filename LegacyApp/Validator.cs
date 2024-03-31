namespace LegacyApp;

public class Validator
{
    public static bool ValidatorAddUser(string firstName, string lastName)
    {
        if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
        {
            return false;
        }

        return true;
    }
    
    public static bool ValidatorAddUserMail(string email)
    {
        if (!email.Contains("@") && !email.Contains("."))
        {
            return false;
        }

        return true;
    }
}