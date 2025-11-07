using MediatR;
using Wallet.Domain.Events;
using Wallet.Domain.Exceptions;

namespace Wallet.Domain.Entities
{
    public class WalletModel : Entity
    {
        public WalletModel(string userName, string userEmail, Guid userId)
        {
            WalletCreatedEvent walletCreatedEvent;

            try
            {
                //set random init balance for testing
                Balance = Random.Shared.Next(1000, 1000000);

                if (string.IsNullOrWhiteSpace(userName))
                {
                    throw new InvalidWalletOperationException("Invalid user name");
                }

                UserName = userName;

                if (string.IsNullOrWhiteSpace(userEmail))
                {
                    throw new InvalidWalletOperationException("Invalid user email");
                }

                UserEmail = userEmail;

                if (userId == Guid.Empty)
                {
                    throw new InvalidWalletOperationException("Invalid user Id");
                }

                UserId = userId;

                IsBocked = false;

                walletCreatedEvent = new WalletCreatedEvent(UserId, true);
            }
            catch (Exception)
            {
                if (userId != Guid.Empty)
                {
                    walletCreatedEvent = new WalletCreatedEvent(UserId, false);
                }

                throw;
            }

            EventList.Add(walletCreatedEvent);
        }

        private WalletModel() { }


        public decimal Balance { get; private set; }

        public string UserName { get; private set; }

        public string UserEmail { get; private set; }

        public bool IsBocked { get; private set; }

        public Guid UserId { get; private set; }


        private List<INotification> EventList = new ();

        public IReadOnlyList<INotification> GetEvents() => EventList;


        public void AddMoneyToBalance(decimal money)
        {
            if (money <= 0)
            {
                throw new InvalidWalletOperationException("Transacted sum is lower than 0");
            }

            Balance += money;
        }

        public void GetMoneyFromBalance(decimal money)
        {
            if (money <= 0)
            {
                throw new InvalidWalletOperationException("Transacted sum is lower than 0");
            }

            if (Balance < money)
            {
                throw new LowerBalanceException($"Your balance is lower than transacted sum on {money - Balance}");
            }

            Balance -= money;
        }

        public void UpdateUserName(string userName)
        {
            NameUpdatedEvent nameUpdatedEvent;

            if (string.IsNullOrWhiteSpace(userName))
            {
                nameUpdatedEvent = new NameUpdatedEvent(UserId, false, UserName);

                throw new InvalidWalletOperationException("Invalid user name to update");
            }

            UserName = userName;

            nameUpdatedEvent = new NameUpdatedEvent(UserId, true, UserName);

            EventList.Add(nameUpdatedEvent);
        }

        public void UpdateUserEmail(string userEmail)
        {
            EmailUpdatedEvent emailUpdatedEvent;

            if (string.IsNullOrWhiteSpace(userEmail))
            {
                emailUpdatedEvent = new EmailUpdatedEvent(UserId, false, UserEmail);

                throw new InvalidWalletOperationException("Invalid user email to update");
            }

            UserEmail = userEmail;

            emailUpdatedEvent = new EmailUpdatedEvent(UserId, true, UserEmail);

            EventList.Add(emailUpdatedEvent);
        }

        public void Block()
        {
            IsBocked = true;
        }

        public void Unblock()
        {
            IsBocked = false;
        }
    }
}
