using AutomationPermitPros.AutomationBlocks;
using AutomationPermitPros.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationPermitPros.Flows
{
    public class LocationFlow
    {

        private readonly LocationBlocks _block;

        public LocationFlow(Microsoft.Playwright.IPage page)
        {
            _block = new LocationBlocks(page);
        }

        public async Task ExecuteAsync(Dictionary<string, string> data)
        {


            if (ExcelHelper.IsTrue(data, "Search"))
            {
                Console.WriteLine("Flow: SEARCH");

                await _block.SearchAsync(data);

                await _block.ReloadAsync();


                //string expectedSearchResult = data.GetValueOrDefault("ExpectedOutcome");

                //if (!string.IsNullOrWhiteSpace(expectedSearchResult))
                //{
                //    bool exists = await _block.BUSLIC_VerifySearchResultExists(
                //        data["Search_LicenseNumber"]
                //    );

                //    if (expectedSearchResult.Equals("FOUND", StringComparison.OrdinalIgnoreCase))
                //    {
                //        Assert.IsTrue(
                //            exists,
                //            $"Expected record '{data["Search_LicenseNumber"]}' to be found, but it was NOT found"
                //        );
                //    }
                //    else if (expectedSearchResult.Equals("NOTFOUND", StringComparison.OrdinalIgnoreCase))
                //    {
                //        Assert.IsFalse(
                //            exists,
                //            $"Expected record '{data["Search_LicenseNumber"]}' to NOT be found, but it EXISTS"
                //        );
                //    }
                //    else
                //    {
                //        Assert.Fail($"Unknown SearchExpectedResult '{expectedSearchResult}' in Excel");
                //    }
                //}
            }
            if (ExcelHelper.IsTrue(data, "Create"))
            {
                Console.WriteLine("Flow: CREATE");
                await _block.CreateAsync(data);

                Console.WriteLine("Flow: VALIDATE CREATE");

                string expectedOutcome = data["ExpectedOutcome"];
                string expectedMessage = data["ExpectedMessage"];

                string actualMessage = await _block.GetToastMessageAsync();
                Console.WriteLine($"Actual Toast Message: {actualMessage}");

                Assert.IsNotNull(actualMessage, "Expected a toast message, but none appeared");

                if (expectedOutcome.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase))
                {
                    Assert.That(
                        actualMessage,
                        Does.Contain(expectedMessage).IgnoreCase,
                        $"Expected SUCCESS message '{expectedMessage}' but got '{actualMessage}'"
                    );

                    // 3️⃣ Verify result
                    //bool exists = await _block.BUSLIC_VerifySearchResultExists(
                    //    data["LicenseNumber"]
                    //);

                    //Assert.IsTrue(
                    //    exists,
                    //    "Record was created but not found in search results"
                    //);
                }
                else if (expectedOutcome.Equals("ERROR", StringComparison.OrdinalIgnoreCase))
                {
                    Assert.That(
                        actualMessage,
                        Does.Contain(expectedMessage).IgnoreCase,
                        $"Expected ERROR message '{expectedMessage}' but got '{actualMessage}'"
                    );
                }
                else
                {
                    Assert.Fail($"Unknown ExpectedOutcome '{expectedOutcome}' in Excel");
                }
            }
            }
        }
}
