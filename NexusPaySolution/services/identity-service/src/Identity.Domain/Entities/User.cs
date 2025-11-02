using Identity.Domain.Events;
using Identity.Domain.Exceptions;
using Identity.Domain.ValueObjects;

namespace Identity.Domain.Entities
{
    public class User : Entity
    {
        private User() { }

        public User(string name, Email email, Password password, RoleName roleName)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidUserException("Invalid user name");
            }

            Id = Guid.NewGuid();
            UserName = name;
            UserEmail = email;
            Password = password;
            IsActive = false;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            RoleName = roleName;

            _domainEvents.Add(new UserRegisteredEvent(name, email.Value, Id));
        }

        public string UserName { get; private set; }

        public Email UserEmail { get; private set; }

        public Password Password { get; private set; }

        public bool IsActive { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public DateTime UpdatedAt { get; private set; }

        public RoleName RoleName { get; set; }

        public Guid RoleId { get; private set; }


        private List<DomainEvent> _domainEvents = new();

        public IReadOnlyList<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public void ClearEventList()
        {
            _domainEvents.Clear();
        }

        public void ChangePassword(Password password)
        {
            Password = password;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateUserName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidUserException("Invalid user name");
            }

            UserName = name;
            UpdatedAt = DateTime.UtcNow;

            _domainEvents.Add(new UserNameUpdatedEvent(Id, UserName));
        }

        public void UpdateUserEmail(Email email)
        {
            UserEmail = email;
            UpdatedAt = DateTime.UtcNow;

            _domainEvents.Add(new UserEmailUpdatedEvent(Id, UserEmail.Value));
        }

        public void ActivateUser()
        {
            if (IsActive == false)
            {
                IsActive = true;
                UpdatedAt = DateTime.UtcNow;

                _domainEvents.Add(new UserActivatedEvent(Id));
            }
        }

        public void DeactivateUser()
        {
            if (IsActive == true)
            {
                IsActive = false;
                UpdatedAt = DateTime.UtcNow;

                _domainEvents.Add(new UserDeactivatedEvent(Id));
            }
        }
    }
}
