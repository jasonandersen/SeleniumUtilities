using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System.Collections.Generic;

namespace SeleniumUtilities.Web
{
    /// <summary>
    /// Utility class to house convenience methods specifically around IWebElement objects.
    /// </summary>
    public class WebElementWrapper : SearchContextWrapper
    {
        // This subclass of PageHelper deals specifically with convenience methods that require
        // an IWebElement instead of an ISearchContext.
        private IWebElement element;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="element"></param>
        public WebElementWrapper(IWebElement element) : base(element)
        {
            this.element = element;
        }
        /// <summary>
        /// Retrieves the original driver from the WebElement.
        /// </summary>
        public IWebDriver Driver
        {
            get
            {
                RemoteWebElement re = (RemoteWebElement)element;
                return re.WrappedDriver;
            }
        }

        /// <summary>
        /// Tagname for this element
        /// </summary>
        public string TagName
        {
            get
            {
                return element.TagName;
            }
        }

        /// <summary>
        /// Determines if the text of the element matches the candidate text.
        /// </summary>
        /// <param name="elementText"></param>
        /// <returns></returns>
        public bool TextMatches(string candidate)
        {
            string text = element.Text.Trim();
            return text.Equals(candidate.Trim());
        }

        /// <summary>
        /// Looks for a 'value' attribute. If found will determine if the value of the value attribute matches
        /// the candidate text
        /// </summary>
        /// <param name="elementText"></param>
        /// <returns></returns>
        public bool ValueAttributeMatches(string candidate)
        {
            string value = element.GetAttribute("value");
            return value.Equals(candidate);
        }

        /// <summary>
        /// Will iterate through all direct children to see if it has a label child element with matching text.
        /// </summary>
        /// <param name="elementText"></param>
        /// <returns></returns>
        public bool HasMatchingLabelChild(string elementText)
        {
            // Note - I know this possible with some schmancy one line XPath but I was getting very
            // inconsistent results from XPath for reasons I could not track down.
            List<WebElementWrapper> children = GetChildren();
            foreach (WebElementWrapper child in children)
            {
                if (child.TagName.Equals("label"))
                {
                    if (child.TextMatches(elementText))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Fetches all the direct descendants of this node.
        /// </summary>
        /// <returns>Will always return an object, never null. Might be empty though</returns>
        public List<WebElementWrapper> GetChildren()
        {
            IReadOnlyCollection<IWebElement> children = element.FindElements(By.XPath("*"));
            List<WebElementWrapper> output = new List<WebElementWrapper>();
            foreach (IWebElement child in children)
            {
                output.Add(new WebElementWrapper(child));
            }
            return output;
        }

        /// <summary>
        /// Return the associated web driver.
        /// </summary>
        /// <returns></returns>
        protected override IWebDriver GetDriver()
        {
            return Driver;
        }

    }
}
