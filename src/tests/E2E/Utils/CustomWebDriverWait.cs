using System.Diagnostics.CodeAnalysis;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace tests.E2E.Utils;

public class CustomWebDriverWait(
        IWebDriver driver,
        TimeSpan timeout,
        Func<List<LogEntry>> captureConsoleErrorMessages) : WebDriverWait(driver, timeout)
{
        [return: NotNull]
        public override TResult Until<TResult>(Func<IWebDriver, TResult?> condition) where TResult : default
        {
                try
                {
                        return base.Until(condition);
                }
                catch (WebDriverTimeoutException innerException)
                {
                        List<string> consoleErrors = [.. captureConsoleErrorMessages().Select(m => m.Message)];
                        if (consoleErrors.Count > 0)
                        {
                                string consoleErrorsMessage = $"Unexpected errors found in browser console: {string.Join(", ", consoleErrors)}";
                                WebDriverTimeoutException outer = new(
                                        $"A timeout exception was thrown. {consoleErrorsMessage}",
                                        innerException);
                                throw outer;
                        }
                        else
                        {
                                throw;
                        }
                }
        }
}
