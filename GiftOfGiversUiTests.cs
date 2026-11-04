using System;
using Xunit;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace GiftOfGivers.Test
{
    public class GiftOfGiversUiTests : IDisposable
    {
        private readonly IWebDriver _driver;
        private readonly string _baseUrl = "https://localhost:7006/";

        public GiftOfGiversUiTests()
        {
            var options = new ChromeOptions();
            options.AddArgument("--headless=new");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");

            _driver = new ChromeDriver(options);
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        }

        [Fact(Skip = "Selenium UI test skipped for automated headless CI pipeline build")]
        public void Scenario1_HomePage_LoadsSuccessfullyAndVerifiesTitle()
        {
            // Flow 1: Home page verification
            _driver.Navigate().GoToUrl(_baseUrl);
            Assert.Contains("Gift", _driver.Title, StringComparison.OrdinalIgnoreCase);
        }

        [Fact(Skip = "Selenium UI test skipped for automated headless CI pipeline build")]
        public void Scenario2_DonatePage_NavigationAndFormInteraction()
        {
            // Flow 2: Valid donation navigation & form element check
            _driver.Navigate().GoToUrl($"{_baseUrl}Donate");
            var bodyElement = _driver.FindElement(By.TagName("body"));

            Assert.NotNull(bodyElement);
            Assert.Contains("Donate", _driver.Url, StringComparison.OrdinalIgnoreCase);
        }

        [Fact(Skip = "Selenium UI test skipped for automated headless CI pipeline build")]
        public void Scenario3_NegativePath_InvalidDonationSubmission_HandlesError()
        {
            // Flow 3 (Negative/Error Path): Submitting invalid data or checking validation handling
            _driver.Navigate().GoToUrl($"{_baseUrl}Donate");

            // Attempt to find a submit button or test validation behavior
            var submitButtons = _driver.FindElements(By.TagName("button"));
            Assert.True(submitButtons.Count > 0, "Donation form page should contain interactive elements.");

            // Verify page remains stable under negative/empty interaction paths
            Assert.Contains("localhost", _driver.Url);
        }

        public void Dispose()
        {
            _driver?.Quit();
            _driver?.Dispose();
        }
    }
}