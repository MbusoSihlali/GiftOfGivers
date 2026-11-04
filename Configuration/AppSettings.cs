namespace GiftOfGivers.Configuration
{
    public class AppSettings
    {
        public ConnectionStrings ConnectionStrings { get; set; } = new();
        public EmailSettings Email { get; set; } = new();
        public SecuritySettings Security { get; set; } = new();
        public ApplicationSettings Application { get; set; } = new();
    }

    public class ConnectionStrings
    {
        public string DefaultConnection { get; set; } = string.Empty;
    }

    public class EmailSettings
    {
        public string SmtpServer { get; set; } = string.Empty;
        public int SmtpPort { get; set; } = 587;
        public string SmtpUsername { get; set; } = string.Empty;
        public string SmtpPassword { get; set; } = string.Empty;
        public bool EnableSsl { get; set; } = true;
        public string FromEmail { get; set; } = string.Empty;
        public string FromName { get; set; } = "Gift of the Givers";
    }

    public class SecuritySettings
    {
        public int PasswordMinLength { get; set; } = 8;
        public bool PasswordRequireDigit { get; set; } = true;
        public bool PasswordRequireLowercase { get; set; } = true;
        public bool PasswordRequireUppercase { get; set; } = true;
        public bool PasswordRequireNonAlphanumeric { get; set; } = true;
        public int MaxFailedAccessAttempts { get; set; } = 5;
        public int DefaultLockoutTimeSpanMinutes { get; set; } = 5;
        public int CookieExpirationHours { get; set; } = 2;
    }

    public class ApplicationSettings
    {
        public string SiteName { get; set; } = "Gift of the Givers Foundation";
        public string SiteUrl { get; set; } = string.Empty;
        public string AdminEmail { get; set; } = "admin@giftofthegivers.org";
        public bool EnableRegistration { get; set; } = true;
        public bool EnableVolunteerApplications { get; set; } = true;
        public int RecentDonationsCount { get; set; } = 5;
        public int RecentVolunteersCount { get; set; } = 10;
    }
}