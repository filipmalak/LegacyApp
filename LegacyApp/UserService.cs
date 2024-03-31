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
        private IClientRepository _clientRepository;
        private ICreditLimitService _creditLimit;
        
        [Obsolete]
        public UserService()
        {
            _clientRepository = new ClientRepository();
            _creditLimit = new UserCreditService();
       }

        public UserService(IClientRepository clientRepository, ICreditLimitService creditLimit)
        {
            _clientRepository = clientRepository;
            _creditLimit = creditLimit;
        }
        public bool AddUser(string firstName, string lastName, string email, DateTime dateOfBirth, int clientId)
        {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                return false;
            }

            if (!email.Contains("@") && !email.Contains("."))
            {
                return false;
            }

            var now = DateTime.Now;
            int age = now.Year - dateOfBirth.Year;
            if (now.Month < dateOfBirth.Month || (now.Month == dateOfBirth.Month && now.Day < dateOfBirth.Day)) age--;

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
            
            if (user.HasCreditLimit && user.CreditLimit < 500)
            {
                return false;
            }

            UserDataAccess.AddUser(user);
            return true;
        }
    }
}
