using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using OfficeOpenXml;
using OrderPlus.Fronend.Repositories;
using OrderPlus.Shared.Entites;

namespace OrderPlus.Fronend.Reports
{
    public partial class InventoryAdjustment
    {
        private List<Inventory>? inventories;
        private Inventory selectedInventory = new();
        private Inventory? inventory;
        private bool loading;
        private bool showReport;
        private float totalQuantity = 0;
        private decimal totalValue = 0;

        [Inject] private IDialogService dialogService { get; set; } = null!;
        [Inject] private ISnackbar snackbar { get; set; } = null!;
        [Inject] private IRepository repository { get; set; } = null!;
        [Inject] private IJSRuntime jS { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            await LoadInventoriesAsync();
        }

        private async Task LoadInventoriesAsync()
        {
            loading = true;
            var responseHttp = await repository.GetAsync<List<Inventory>>($"/api/inventories/combo");
            loading = false;

            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                snackbar.Add(message!, Severity.Error);
                return;
            }
            inventories = responseHttp.Response;
        }
        public async Task<IEnumerable<Inventory>> SearchSupplierAsync(string SearchText)
        {
            await Task.Delay(1);
            if (string.IsNullOrEmpty(SearchText))
            {
                return inventories!;
            }
            return inventories!.Where(x => x.Name.Contains(SearchText, StringComparison.InvariantCultureIgnoreCase))
                .ToList();
        }
        public void SuplierChanged(Inventory inventory)
        {
            selectedInventory = inventory;
        }
        private async Task GenerateReportAsync()
        {
            if ((selectedInventory.Id == 0))
            {
                snackbar.Add("You must select an inventory.", Severity.Error);
                return;
            }
            var responseHttp = await repository.GetAsync<Inventory>($"/api/inventories/{selectedInventory.Id}");
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                snackbar.Add(message!, Severity.Error);
                return;
            }
            inventory = responseHttp.Response;
            inventory!.InventoryDetails = inventory.InventoryDetails!.
                Where(x => x.Adjustment != 0).ToList();
            totalQuantity = inventory.InventoryDetails.Sum(x => x.Adjustment);
            totalValue = inventory.InventoryDetails.Sum(x => x.AdjustmentValue);
            showReport = true;
        }
        private void ExportToExcel()
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Inventory Adjustment");

            worksheet.Cells[1, 1].Value = "Product";
            worksheet.Cells[1, 2].Value = "Cost";
            worksheet.Cells[1, 3].Value = "Stock";
            worksheet.Cells[1, 4].Value = "Setting";
            worksheet.Cells[1, 5].Value = "Setting value";

            for (int i = 0; i < inventory!.InventoryDetails!.Count; i++)
            {
                worksheet.Cells[i + 2, 1].Value = inventory.InventoryDetails.ElementAt(i).Product!.Name;
                worksheet.Cells[i + 2, 2].Value = inventory.InventoryDetails.ElementAt(i).Cost;
                worksheet.Cells[i + 2, 3].Value = inventory.InventoryDetails.ElementAt(i).Stock;
                worksheet.Cells[i + 2, 4].Value = inventory.InventoryDetails.ElementAt(i).Adjustment;
                worksheet.Cells[i + 2, 5].Value = inventory.InventoryDetails.ElementAt(i).AdjustmentValue;
            }

            var stream = new MemoryStream(package.GetAsByteArray());
            jS.InvokeVoidAsync("BlazorDownloadFile", "Inventory adjustment.xlsx", Convert.ToBase64String(stream.ToArray()));
        }
    }
}
