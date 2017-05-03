using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.ObjectModel;

namespace SeleniumUtilities.Web
{
    /// <summary>
    /// Utility class to house common methods around interacting with ISearchContext (IWebElement or
    /// IWebDriver) objects.
    /// </summary>
    public abstract class SearchContextWrapper
    {
        // Search context to find page elements. Can be either an IWebDriver (the browser) or
        // an IWebElement (an HTML element on the page). If this is an IWebElement, then it will
        // only search for child elements of the specified element.
        private ISearchContext searchContext;

        // Default timeout when waiting for an element to appear (in milliseconds).
        private int defaultTimeout;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="searchContext">WebDriver or WebElement to search for elements</param>
        public SearchContextWrapper(ISearchContext searchContext)
        {
            this.searchContext = searchContext;
            if (searchContext == null)
            {
                throw new NullReferenceException();
            }
            defaultTimeout = 3000;
        }

        /// <summary>
        /// Constructor that sets the default timeout.
        /// </summary>
        /// <param name="searchContext"></param>
        /// <param name="defaultTimeout"></param>
        public SearchContextWrapper(ISearchContext searchContext, int defaultTimeout) : this(searchContext)
        {
            this.defaultTimeout = defaultTimeout;
        }

        /// <summary>
        /// Finds an element within this search context (either the web driver or the parent web element).
        /// </summary>
        /// <param name="by"></param>
        /// <returns>the first element that matches the By argument</returns>
        /// <exception cref="NoSuchElementException">when the element cannot be found</exception>
        public IWebElement FindElement(By by)
        {
            return searchContext.FindElement(by);
        }

        /// <summary>
        /// Finds all elements within this search context that match the search strategy.
        /// </summary>
        /// <param name="by"></param>
        /// <returns></returns>
        public ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            return searchContext.FindElements(by);
        }

        /// <summary>
        /// Determines if an element exists in the current search context.
        /// </summary>
        /// <param name="by">search criteria</param>
        /// <returns>true if the element exists, false if cannot be found - will not throw an exception</returns>
        public bool ElementExists(By by)
        {
            try
            {
                IWebElement element = searchContext.FindElement(by);
                return element != null;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        /// <summary>
        /// Determines if an element is visible.
        /// </summary>
        /// <param name="by"></param>
        /// <returns>true if the element is visible</returns>
        public bool ElementIsVisible(By by)
        {
            IWebElement element = FindElement(by);
            return element.Displayed;
        }

        /// <summary>
        /// Retrieve the text of the element.
        /// </summary>
        /// <param name="by">search criteria</param>
        /// <returns>Text of the element.</returns>
        /// <exception cref="NoSuchElementException">when element not found</exception>
        public string GetElementText(By by)
        {
            IWebElement element = searchContext.FindElement(by);
            return element.Text;
        }
        /// <summary>
        /// Ensures selected value of a checkbox matches the selected param.
        /// </summary>
        /// <param name="checkbox">checkbox to operate against</param>
        /// <param name="selected">If true, ensure checkbox is selected. If false, ensure checkbox is not selected.</param>
        public void ToggleCheckBox(IWebElement checkbox, bool selected)
        {
            if (checkbox.Selected && !selected)
            {
                checkbox.Click();
            }
            else if (!checkbox.Selected && selected)
            {
                checkbox.Click();
            }
        }
        /// <summary>
        /// Clicks the first element found.
        /// </summary>
        /// <param name="by">search criteria</param>
        /// <exception cref="NoSuchElementException">when element not found</exception>
        public void ClickElement(By by)
        {
            IWebElement element = searchContext.FindElement(by);
            element.Click();
        }
        /// <summary>
        /// Retrieves the value of the first element found.
        /// </summary>
        /// <param name="by">search criteria</param>
        /// <returns>the value of the first element found.</returns>
        /// <exception cref="NoSuchElementException">when element not found</exception>
        public string GetElementValue(By by)
        {
            IWebElement element = searchContext.FindElement(by);
            return element.GetAttribute("value");
        }
        /// <summary>
        /// Sets the value of the first element found.
        /// </summary>
        /// <param name="by">search criteria</param>
        /// <param name="newValue">value of the element to set</param>
        /// <exception cref="NoSuchElementException">when element not found</exception>
        public void SetElementValue(By by, string newValue)
        {
            IWebElement element = searchContext.FindElement(by);
            if (IsSelect(element))
            {
                SetSelectValue(element, newValue);
            }
            else
            {
                SetFocus(element);
                element.SendKeys(newValue);
            }
        }

        /// <summary>
        /// Sets the value of a group of radio buttons to the radio button that has the inner text
        /// or 'value' attribute that matches newValue.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newValue"></param>
        /// <exception cref="NoSuchElementException" />
        public void SetRadioValue(string id, string newValue)
        {
            // Grab all the radio elements
            ReadOnlyCollection<IWebElement> elements = FindElementsByIdOrName(id);
            foreach (IWebElement element in elements)
            {
                string text = element.Text;
                if (ElementTextOrValueMatches(element, newValue))
                {
                    element.Click();
                    return;
                }
            }
            throw new InvalidOperationException(string.Format("Could not find the value '{0}' in the radio button '{1}'", newValue, id));
        }

        /// <summary>
        /// Will find elements either by ID first or if unsuccessful, by name.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>collection of elements with a matching ID or name</returns>
        /// <exception cref="NoSuchElementException">when elements cannot be found by either name or ID</exception>
        private ReadOnlyCollection<IWebElement> FindElementsByIdOrName(string id)
        {
            // first look by ID
            ReadOnlyCollection<IWebElement> elements = searchContext.FindElements(By.Id(id));
            if (elements == null || elements.Count == 0)
            {
                // then look by name
                elements = searchContext.FindElements(By.Name(id));
            }
            return elements;
        }

        /// <summary>
        /// Will determine if an element's text matches or it's 'value' attribute matches.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="elementText"></param>
        /// <returns></returns>
        private bool ElementTextOrValueMatches(IWebElement element, string elementText)
        {
            WebElementWrapper wrapper = new WebElementWrapper(element);
            if (wrapper.TextMatches(elementText))
            {
                return true;
            }
            if (wrapper.ValueAttributeMatches(elementText))
            {
                return true;
            }
            return false;

        }

        /// <summary>
        /// Waits for an element to appear.
        /// </summary>
        /// <param name="by">The search algorithm to find the element</param>
        /// <param name="pollingFrequency">How often (in milliseconds) to check for it's existence</param>
        /// <param name="timeoutDuration">How long (in milliseconds) to wait before timing out</param>
        /// <exception cref="NoSuchElementException">when timeout is reached</exception>
        public void WaitForElement(By by, int pollingFrequency = 500, int timeoutDuration = -1)
        {
            // Use the default timeout set in the constructor if not passed in
            timeoutDuration = timeoutDuration < 0 ? defaultTimeout : timeoutDuration;
            DateTime timeout = DateTime.Now.AddMilliseconds(timeoutDuration);
            while (DateTime.Now.CompareTo(timeout) < 0)
            {
                if (ElementExists(by))
                {
                    return;
                }
                System.Threading.Thread.Sleep(pollingFrequency);
            }
            throw new NoSuchElementException("Element could not be found! " + by.ToString());
        }

        /// <summary>
        /// Halts the current thread's execution for the specified time span.
        /// </summary>
        /// <param name="timeSpan"></param>
        public void Wait(int milliseconds)
        {
            System.Threading.Thread.Sleep(milliseconds);
        }

        /// <summary>
        /// Retrieve the related web driver
        /// </summary>
        /// <returns></returns>
        protected abstract IWebDriver GetDriver();

        /// <summary>
        /// Sets the focus on the particular web element.
        /// </summary>
        /// <param name="element"></param>
        private void SetFocus(IWebElement element)
        {
            new Actions(GetDriver()).MoveToElement(element).Perform();
        }

        /// <summary>
        /// Sets the value of a select by iterating through it's options and matching by text value
        /// </summary>
        /// <param name="element"></param>
        /// <param name="newValue"></param>
        private void SetSelectValue(IWebElement element, string newValue)
        {
            SelectElement select = new SelectElement(element);
            select.SelectByText(newValue);
        }

        /// <summary>
        /// Determines if an element is a select element.
        /// </summary>
        /// <param name="element"></param>
        /// <returns>true if the element's tag name is 'select'</returns>
        private bool IsSelect(IWebElement element)
        {
            return "select".Equals(element.TagName.ToLower());
        }


    }
}
