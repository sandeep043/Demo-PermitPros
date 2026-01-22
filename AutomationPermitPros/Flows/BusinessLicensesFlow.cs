using AutomationPermitPros.AutomationBlocks;
using AutomationPermitPros.Utilities;
using Microsoft.Playwright;
using NUnit.Framework.Internal.Commands;

namespace AutomationPermitPros.Flows
{
    public class BusinessLicensesFlow
    {
        private readonly BusinesslicensesBLocks _block;

        public BusinessLicensesFlow(IPage page)
        {
            _block = new BusinesslicensesBLocks(page);
        }

        public async Task ExecuteAsync(Dictionary<string, string> data)
        {
            Console.WriteLine($"Executing TestCase: {data["TestCaseID"]}");
            //await sideBar.NavigateToAsync("Business Licenses");
            //SEARCH (mandatory before actions)
            if (ExcelHelper.IsTrue(data, "Search"))
            {
                Console.WriteLine("Flow: SEARCH");

                await _block.SearchAsync(data);

                await  _block.ReloadAsync();


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


            //CREATE
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
                    bool exists = await _block.BUSLIC_VerifySearchResultExists(
                        data["LicenseNumber"]
                    );

                    Assert.IsTrue(
                        exists,
                        "Record was created but not found in search results"
                    );
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

            //VIEW
            if (ExcelHelper.IsTrue(data, "View"))
            {
                await _block.SearchAsync(data);

                Console.WriteLine("Flow: VIEW");
                await _block.ViewAsync();
            }

            //EDIT
            if (ExcelHelper.IsTrue(data, "Edit"))
            {
                Console.WriteLine("Flow: EDIT");
                await _block.SearchAsync(data);
                await _block.EditAsync(data);
            }

            //DELETE
            if (ExcelHelper.IsTrue(data, "Delete"))
            {
                Console.WriteLine("Flow: DELETE");
                await _block.SearchAsync(data);

                await _block.DeleteAsync(data);

                Console.WriteLine("Flow: VALIDATE DELETE");

                string expectedOutcome = data["ExpectedOutcome"];
                string expectedMessage = data["ExpectedMessage"];

                string actualMessage = await _block.GetToastMessageAsync();

                Assert.IsNotNull(actualMessage, "Expected a toast message after delete, but none appeared");

                if (expectedOutcome.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase))
                {
                    //Validate success message
                    Assert.That(
                        actualMessage,
                        Does.Contain(expectedMessage).IgnoreCase,
                        $"Expected SUCCESS delete message '{expectedMessage}' but got '{actualMessage}'"
                    );

                    ////Validate record is really deleted

                    //bool existsBeforeDelete = await _block.BUSLIC_VerifySearchResultExists(
                    //    data["Search_LicenseNumber"]
                    //);

                    //Assert.IsTrue(
                    //    existsBeforeDelete,
                    //    "Record does not exist before delete operation"
                    //);

                    //bool exists = await _block.BUSLIC_VerifySearchResultExists(
                    //    data["Search_LicenseNumber"]
                    //);

                    //Assert.IsFalse(
                    //    exists,
                    //    "Record still exists in search results after successful delete"
                    //);
                }
                else if (expectedOutcome.Equals("ERROR", StringComparison.OrdinalIgnoreCase))
                {
                    //Validate error message
                    Assert.That(
                        actualMessage,
                        Does.Contain(expectedMessage).IgnoreCase,
                        $"Expected ERROR delete message '{expectedMessage}' but got '{actualMessage}'"
                    );

                    bool exists = await _block.BUSLIC_VerifySearchResultExists(
                        data["Search_LicenseNumber"]
                    );

                    Assert.IsTrue(
                        exists,
                        "Record does NOT exist even though delete failed"
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
