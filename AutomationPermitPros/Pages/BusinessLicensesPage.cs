using Microsoft.Playwright;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace AutomationPermitPros.Pages
{
    public class BusinessLicensesPage
    {

        private readonly IPage _page;

        public BusinessLicensesPage(IPage page)
        {
            _page = page;
        }

        private ILocator LocationNumberInput => _page.GetByRole(AriaRole.Textbox, new() { Name = "Enter Location Number" });
        private ILocator LocationNameInput => _page.GetByRole(AriaRole.Textbox, new() { Name = "Enter Location Name" });
        private ILocator LicenseNumberInput => _page.GetByRole(AriaRole.Textbox, new() { Name = "Enter License Number" });

        // License Type <select> as combobox
        private ILocator LicenseTypeDropdown => _page.GetByRole(AriaRole.Combobox, new() { Name = "License Type" });

        // State <select> as combobox
        private ILocator StateDropdown => _page.GetByRole(AriaRole.Combobox, new() { Name = "State" });

        // Async helpers
        public async Task FillLocationNumberAsync(string value)
        {
            await LocationNumberInput.FillAsync(value);
        }

        public async Task FillLocationNameAsync(string value)
        {
            await LocationNameInput.FillAsync(value);
        }

        public async Task FillLicenseNumberAsync(string value)
        {
            await LicenseNumberInput.FillAsync(value);
        }

        public async Task SelectLicenseTypeAsync(string optionLabel)
        {
            await LicenseTypeDropdown.SelectOptionAsync(new SelectOptionValue { Label = optionLabel });
        }

        public async Task SelectStateAsync(string stateLabel)
        {
            await StateDropdown.SelectOptionAsync(new SelectOptionValue { Label = stateLabel });
        }

    }
}
