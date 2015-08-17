namespace TestProject
{
    using System;
    using FluentAssertions;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using OpenQA.Selenium.Support.UI;
 
    class SeleniumHelper
    {
        public static ChromeDriver driver;
        public static int Timeout = 240;
        public static string URL = "http://www.wikipedia.org";

        public static void CreateWebDriver()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            driver = new ChromeDriver("../../Drivers", options, TimeSpan.FromSeconds(Timeout));
            driver.Manage().Timeouts().SetScriptTimeout(TimeSpan.FromSeconds(Timeout));
            driver.Manage().Cookies.DeleteAllCookies();
            driver.Url = URL;
        }

        public void SearchWikipediaPageAndValidatePageHeader(string searchString, string language)
        {
            var searchField = driver.FindElement(By.Id("searchInput"));
            var languageDropdown = new SelectElement(driver.FindElement(By.Id("searchLanguage")));
            var searchButton = driver.FindElement(By.ClassName("formBtn"));

            if (string.IsNullOrEmpty(searchString))
            {
                throw new Exception("Invalid input for Search String");
            }

            searchField.SendKeys(searchString);
            
            if (string.IsNullOrEmpty(language))
            {
                throw new Exception("Invalid input for Language");
            }

            languageDropdown.SelectByText(language);
            searchButton.Click();
            WaitForPageToLoad();

            ValidatePageHeader(searchString);
        }

        public void ValidatePageHeader(string pageHeader)
        {
            if (string.IsNullOrEmpty(pageHeader))
            {
                throw new Exception("Invalid input for Page Header");
            }

            var heading = driver.FindElementByClassName("firstHeading").GetAttribute("textContent");
            heading.ToLower().Should().Contain(pageHeader.ToLower());
        }

        public void WaitForPageToLoad()
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(Timeout));
            var scriptExecutor = driver as IJavaScriptExecutor;
            wait.Until(d =>
            {
                string state = (string)scriptExecutor.ExecuteScript("return document.readyState");
                return state.ToLower() == "complete";
            });
        }

        internal void ValidateLanguage(string language)
        {
            var scriptExecutor = driver as IJavaScriptExecutor;
            var displayedValue = (string) scriptExecutor.ExecuteScript("return document.documentElement.lang");
            displayedValue.Should().Be(language);
        }

        internal void ChangeSearchResultsLanguage(string language, string pageHeader)
        {
            var languageLink = driver.FindElement(By.LinkText(language));
            languageLink.Click();
            WaitForPageToLoad();
            ValidatePageHeader(pageHeader);
        }

        public void ValidateLanguageLinkExists(string language)
        {
            try
            {
                driver.FindElement(By.LinkText(language));
            }
            catch (Exception)
            {
                throw new Exception("Unable to find a link with language : " + language + " on search results");
            }
            
        }

        internal void CloseBrowser()
        {
           driver.Close();
           driver.Quit();
        }
    }
}
