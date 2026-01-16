using AutomationPermitPros.AutomationBlocks;
using AutomationPermitPros.Utilities;
using Microsoft.Playwright;

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

            //SEARCH (mandatory before actions)
            if (ExcelHelper.IsTrue(data, "Search"))
            {
                Console.WriteLine("Flow: SEARCH");
                await _block.SearchAsync(data);
            }

            //CREATE
            if (ExcelHelper.IsTrue(data, "Create"))
            {
                Console.WriteLine("Flow: CREATE");
                await _block.CreateAsync(data);
            }

            //VIEW
            if (ExcelHelper.IsTrue(data, "View"))
            {
                Console.WriteLine("Flow: VIEW");
                await _block.ViewAsync();
            }

            //EDIT
            if (ExcelHelper.IsTrue(data, "Edit"))
            {
                Console.WriteLine("Flow: EDIT");
                await _block.EditAsync(data);
            }

            //DELETE
            if (ExcelHelper.IsTrue(data, "Delete"))
            {
                Console.WriteLine("Flow: DELETE");
                await _block.DeleteAsync(data);
            }
        }

    }
}
