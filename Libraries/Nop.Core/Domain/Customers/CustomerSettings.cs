
using Nop.Core.Configuration;

namespace Nop.Core.Domain.Customers
{
    public class CustomerSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether usernames are used instead of emails
        /// </summary>
        public bool UsernamesEnabled { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether users can check the availability of usernames (when registering or changing in 'My Account')
        /// </summary>
        public bool CheckUsernameAvailabilityEnabled { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether users are allowed to change their usernames
        /// </summary>
        public bool AllowUsersToChangeUsernames { get; set; }

        /// <summary>
        /// Default password format for customers
        /// </summary>
        public PasswordFormat DefaultPasswordFormat { get; set; }
        /// <summary>
        /// Gets or sets a customer password format (SHA1, MD5) when passwords are hashed (DO NOT edit in production environment)
        /// </summary>
        public string HashedPasswordFormat { get; set; }
        /// <summary>
        /// Gets or sets a minimum password length
        /// </summary>
        public int PasswordMinLength { get; set; }
        /// <summary>
        /// Gets or sets a number of passwords that should not be the same as the previous one; 0 if the customer can use the same password time after time
        /// </summary>
        public int UnduplicatedPasswordsNumber { get; set; }
        /// <summary>
        /// Gets or sets a number of days for password recovery link. Set to 0 if it doesn't expire.
        /// </summary>
        public int PasswordRecoveryLinkDaysValid { get; set; }
        /// <summary>
        /// Gets or sets a number of days for password expiration
        /// </summary>
        public int PasswordLifetime { get; set; }

        /// <summary>
        /// Gets or sets maximum login failures to lockout account. Set 0 to disable this feature
        /// </summary>
        public int FailedPasswordAllowedAttempts { get; set; }
        /// <summary>
        /// Gets or sets a number of minutes to lockout users (for login failures).
        /// </summary>
        public int FailedPasswordLockoutMinutes { get; set; }

        /// <summary>
        /// User registration type
        /// </summary>
        public UserRegistrationType UserRegistrationType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether customers are allowed to upload avatars.
        /// </summary>
        public bool AllowCustomersToUploadAvatars { get; set; }
        /// <summary>
        /// Gets or sets a maximum avatar size (in bytes)
        /// </summary>
        public int AvatarMaximumSizeBytes { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to display default user avatar.
        /// </summary>
        public bool DefaultAvatarEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether customers location is shown
        /// </summary>
        public bool ShowCustomersLocation { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to show customers join date
        /// </summary>
        public bool ShowCustomersJoinDate { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether customers are allowed to view profiles of other customers
        /// </summary>
        public bool AllowViewingProfiles { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'New customer' notification message should be sent to a store owner
        /// </summary>
        public bool NotifyNewCustomerRegistration { get; set; }
		 
 
        /// <summary>
        /// Gets or sets a value indicating whether to validate user when downloading products
        /// </summary>
        public bool DownloadableProductsValidateUser { get; set; }

        /// <summary>
        /// Customer name formatting
        /// </summary>
        public CustomerNameFormat CustomerNameFormat { get; set; }

       
        /// <summary>
        /// Gets or sets a value indicating the number of minutes for 'online customers' module
        /// </summary>
        public int OnlineCustomerMinutes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating we should store last visited page URL for each customer
        /// </summary>
        public bool StoreLastVisitedPage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating we should store IP addresses of customers
        /// </summary>
        public bool StoreIpAddresses { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether deleted customer records should be prefixed suffixed with "-DELETED"
        /// </summary>
        public bool SuffixDeletedCustomers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to force entering email twice
        /// </summary>
        public bool EnteringEmailTwice { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether registration is required for downloadable products
        /// </summary>
        public bool RequireRegistrationForDownloadableProducts { get; set; }

        /// <summary>
        /// Gets or sets interval (in minutes) with which the Delete Guest Task runs
        /// </summary>
        public int DeleteGuestTaskOlderThanMinutes { get; set; }

        #region Form fields
		 
        /// <summary>
        /// Gets or sets a value indicating whether 'Phone number' is enabled
        /// </summary>
        public bool PhoneEnabled { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether 'Phone number' is required
        /// </summary>
        public bool PhoneRequired { get; set; }
		 

        #endregion
    }
}