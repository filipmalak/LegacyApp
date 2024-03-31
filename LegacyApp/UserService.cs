using System;

namespace LegacyApp
{

    public interface ICreditLimitService
    {
        int GetCreditLimit(string lastName, DateTime birthdate);
    };

    public interface IClientRepository
    {
        Client GetById(int idClient);
    }

    

    public class UserService
    {
        private ClientRepository _clientRepository;
        private UserCreditService _creditLimit;
        
        [Obsolete]
        public UserService()
        {
            _clientRepository = new ClientRepository();
            _creditLimit = new UserCreditService();
       }

        public UserService(ClientRepository clientRepository, UserCreditService creditLimit)
        {
            _clientRepository = clientRepository;
            _creditLimit = creditLimit;
        }
        public bool AddUser(string firstName, string lastName, string email, DateTime dateOfBirth, int clientId)
        {
            if (!Validator.ValidatorAddUser(firstName, lastName)) return false;

            if (!Validator.ValidatorAddUserMail(email)) return false;


            int age = CalculateAge(dateOfBirth);

            if (age < 21)
            {
                return false;
            }

            var client = _clientRepository.GetById(clientId);

            var user = new User
            {
                Client = client,
                DateOfBirth = dateOfBirth,
                EmailAddress = email,
                FirstName = firstName,
                LastName = lastName
            };
            
            UserValidator(client, user);
            
            if (user.HasCreditLimit && user.CreditLimit < 500)
            {
                return false;
            }

            UserDataAccess.AddUser(user);
            return true;
        }

        public void UserValidator(Client client, User user)
        {
            if (client.Type == "VeryImportantClient")
            {
                user.HasCreditLimit = false;
            }
            else if (client.Type == "ImportantClient")
            {

                int creditLimit = _creditLimit.GetCreditLimit(user.LastName, user.DateOfBirth);
                creditLimit = creditLimit * 2;
                user.CreditLimit = creditLimit;

            }
            else
            {
                user.HasCreditLimit = true;
                int creditLimit = _creditLimit.GetCreditLimit(user.LastName, user.DateOfBirth);
                user.CreditLimit = creditLimit;
            }
        }

        private int CalculateAge(DateTime dateOfBirth)
        {
            var now = DateTime.Now;
            int age = now.Year - dateOfBirth.Year;
            if (now.Month < dateOfBirth.Month || (now.Month == dateOfBirth.Month && now.Day < dateOfBirth.Day)) age--;
            return age;
        }
    }
}
