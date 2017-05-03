using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace SeleniumUtilities.Web
{
    /// <summary>
    /// Encapsulates the IWebDriver instance and provides utility methods for page objects. Should abstract
    /// the messier bits of the Selenium API.
    /// </summary>
    public class Browser : SearchContextWrapper
    {
        // This subclass of SearchContextWrapper deals specifically with convenience methods that require
        // an IWebDriver instead of an ISearchContext.
        private IWebDriver driver;

        // The host helps construct host-specific URLs. Can be null.
        private Host host;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="searchContext"></param>
        /// <param name="host"></param>
        public Browser(IWebDriver driver, Host host, int defaultTimeout) : base(driver, defaultTimeout)
        {
            this.driver = driver;
            this.host = host;
        }

        /// <summary>
        /// Allows browser to be created with a specific host.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="defaultTimeout"></param>
        public Browser(IWebDriver driver, int defaultTimeout) : this(driver, null, defaultTimeout)
        {

        }

        /// <summary>
        /// Switches the driver over to a specified iframe.
        /// </summary>
        /// <param name="iFrameId"></param>
        /// <exception cref="NoSuchFrameException" />
        public void SwitchToFrame(string iFrameId)
        {
            driver.SwitchTo().Frame(iFrameId);
        }

        /// <summary>
        /// Navigates to a URL on the specified host based on the path provided. The hostname will be specified
        /// by the host, the path will be specified by the path argument.
        /// </summary>
        /// <param name="path"></param>
        /// <exception cref="InvalidOperationException">Thrown when the browser was created without a host.</exception>
        public void NavigateToPath(string path)
        {
            if (host == null)
            {
                throw new InvalidOperationException(
                    "The specified host is null. Cannot navigate to a relative path.");
            }
            string url = host.ConstructUrl(path);
            driver.Navigate().GoToUrl(url);
        }

        /// <summary>
        /// Navigates to an absolute URL.
        /// </summary>
        /// <param name="url"></param>
        public void NavigateToUrl(string url)
        {
            driver.Navigate().GoToUrl(url);
        }

        /// <summary>
        /// Moves the mouse pointer over the top of an element without clicking the element.
        /// </summary>
        /// <exception cref="NoSuchElementException">When the element cannot be found.</exception>
        /// <param name="by"></param>
        public void HoverElement(By by)
        {
            IWebElement target = FindElement(by);
            Actions builder = new Actions(driver);
            builder.MoveToElement(target).Build().Perform();
        }

        /// <summary>
        /// Will switch to the alert dialog and click OK.
        /// </summary>
        public void AcceptAlertDialog()
        {
            driver.SwitchTo().Alert().Accept();
        }

        /// <summary>
        /// Shuts all browser windows opened during the session.
        /// </summary>
        public void Shutdown()
        {
            if (driver != null)
            {
                driver.Quit();
            }
        }

        /// <summary>
        /// Switches back to the default frame in case the driver has switched over to another frame.
        /// </summary>
        public void SwitchToDefaultFrame()
        {
            driver.SwitchTo().DefaultContent();
        }

        /// <summary>
        /// Will switch to the most recent page opened.
        /// </summary>
        public void SwitchToMostRecentPage()
        {
            driver.SwitchTo().Window(driver.WindowHandles[driver.WindowHandles.Count - 1]);
        }

        /// <summary>
        /// Return the web driver object being used.
        /// </summary>
        /// <returns></returns>
        protected override IWebDriver GetDriver()
        {
            return driver;
        }
    }
}
