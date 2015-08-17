namespace TechnicalTests
{
    using System;
    using System.Xml;
    using NUnit.Framework;
    using TestProject.UKLocation;
    using TestProject;
    
    [TestFixture]
    public class TechnicalTests
    {
        [Test]
        public void CountArrayOrderedSet()
        {
            var unorderedArraySet = new[] { 1, 4, 1, 4, 2, 1, 3, 5, 6, 2, 3, 7 };
            int countOrderedSet = 0;
            int count = 0;
            for (int index = 0; index < unorderedArraySet.Length; index++)
            {
                var item = unorderedArraySet[index];
                if ((index + 1) == unorderedArraySet.Length)
                {
                    break;
                }

                if (item < unorderedArraySet[index + 1])
                {
                    count = count + 1;
                }
                else
                {
                    countOrderedSet = countOrderedSet > count ? countOrderedSet : count;
                    count = 0;
                }
            }

            if (countOrderedSet == 0)
            {
                Console.WriteLine("No Ordered Set found in the Array Input provided");
            }
            else
            {
                Console.WriteLine("Longest length is: {0}", countOrderedSet + 1);
            }
        }

        [Test]
        public void LocationValidator()
        {
            string county = "Lancashire";
            string location = "Earby";
            ValidateLocationExistsInCounty(county, location);
        }

        [Test]
        public void WikipediaLanguageSearch()
        {
            var helper = new SeleniumHelper();
            SeleniumHelper.CreateWebDriver();
            helper.SearchWikipediaPageAndValidatePageHeader("Test", "English");
            helper.ValidateLanguage("en");
            helper.ChangeSearchResultsLanguage("Deutsch", "Test");
            helper.ValidateLanguage("de");
            helper.ValidateLanguageLinkExists("English");
            helper.CloseBrowser();
        }

        private void ValidateLocationExistsInCounty(string county, string location)
        {
            var soapClient = new UKLocationSoapClient();
            var returnValue = soapClient.GetUKLocationByCounty(county);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(returnValue);
            var outputNode = doc.SelectSingleNode(".//Table/Town[text() = '" + location + "']");
            if (outputNode == null)
            {
                throw new Exception(string.Format("Expected Location : {0} not found in County {1}", location, county));
            }

            var postcode = doc.SelectSingleNode(".//Table/Town[text() = '" + location + "']/following-sibling::PostCode");
            Console.WriteLine("Location {0} with PostCode {1} found in the County {2}", location, postcode.InnerText, county);
        }

    }
}
