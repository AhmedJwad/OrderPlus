using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using OfficeOpenXml;
using OrderPlus.Fronend.Helpers;
using OrderPlus.Fronend.Repositories;
using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Entites;

namespace OrderPlus.Fronend.Reports
{
    public partial class GrossProfit
    {
        private bool loading;
        private bool showReport;
        private float totalQuantity=0;
        private decimal totalValue=0;
        private decimal totalProfit = 0;
        private DateTime initialDate = DateTime.Today;
        private DateTime finalDate = DateTime.Today.AddDays(1).AddMilliseconds(-1);
        private List<Order>? orders;
        private List<GrossProfitReportDTO>? reports;
        [Inject] private IDialogService dialogService { get; set; } = null!;
        [Inject] private ISnackbar snackbar { get; set; } = null!;
        [Inject] private IRepository repository { get; set; } = null!;
        [Inject] private IJSRuntime jS { get; set; } = null!;

        private async Task OnInitialDateChange(DateTime? date)
        {
            await Task.Delay(1);
            if(date == null)
            {
                return;
            }
            initialDate=(DateTime)date;
        }
        private async Task OnFinalDateChange(DateTime? date)
        {
            await Task.Delay(1);
            if (date == null)
            {
                return;
            }
            finalDate = (DateTime)date;
        }

        private async Task GenerateReportAsync()
        {
            if(initialDate > finalDate)
            {
                snackbar.Add("The start date cannot be greater than the end date.", Severity.Error);
                return;
            }
            var datesDTO = new DatesDTO
            {
                InitialDate = initialDate,
                FinalDate = finalDate,
            };
            loading = true;
            var responseHttp=await repository.PostAsync<DatesDTO, List<Order>> ($"/api/orders/report", datesDTO);
            loading = false;
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                snackbar.Add(message!, Severity.Error);
                return;
            }
            orders = responseHttp.Response;
            reports = [];
            foreach (var order in orders!)
            {
                foreach (var orderDetails in order.OrderDetails!)
                {
                    reports.Add(new GrossProfitReportDTO
                    {
                        Id=order.Id,
                        Date=order.Date,
                        User=order.User,
                        OrderType=order.OrderType,
                        OrderStatus=order.OrderStatus,
                        Name=orderDetails.Name,
                        Price=orderDetails.Price,
                        Product=orderDetails.Product,
                        Quantity=orderDetails.Quantity,

                    });
                }
            }
            totalQuantity=reports.Sum(r => r.Quantity);
            totalValue=reports.Sum(r => r.Value);
            totalProfit=reports.Sum(r=>r.Profit);
            showReport = true;

        }

        private void ExportToExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; 
            using var package= new ExcelPackage();  
            var worksheet=package.Workbook.Worksheets.Add("Gross profit");
            worksheet.Cells[1, 1].Value = "Date";
            worksheet.Cells[1, 2].Value = "User";
            worksheet.Cells[1, 3].Value = "Status";
            worksheet.Cells[1, 4].Value = "Type";
            worksheet.Cells[1, 5].Value = "Product";
            worksheet.Cells[1, 6].Value = "Cost";
            worksheet.Cells[1, 7].Value = "Quantity";
            worksheet.Cells[1, 8].Value = "Value";
            worksheet.Cells[1, 9].Value = "Utility";
            for (int i = 0; i < reports!.Count; i++)
            {
                worksheet.Cells[i + 2, 1].Value = reports.ElementAt(i).Date.ToString("yyyy-MM-dd HH:mm:ss"); ;
                worksheet.Cells[i + 2, 2].Value = reports.ElementAt(i).User!.FullName;
                worksheet.Cells[i + 2, 3].Value = EnumHelper.GetEnumDescription(reports.ElementAt(i).OrderStatus);
                worksheet.Cells[i + 2, 4].Value = EnumHelper.GetEnumDescription(reports.ElementAt(i).OrderType);
                worksheet.Cells[i + 2, 5].Value = reports.ElementAt(i).Product!.Name;
                worksheet.Cells[i + 2, 6].Value = reports.ElementAt(i).Product!.Cost;
                worksheet.Cells[i + 2, 7].Value = reports.ElementAt(i).Quantity;
                worksheet.Cells[i + 2, 8].Value = reports.ElementAt(i).Value;
                worksheet.Cells[i + 2, 9].Value = reports.ElementAt(i).Profit;
            }
            var stream = new MemoryStream(package.GetAsByteArray());
            jS.InvokeVoidAsync("BlazorDownloadFile", "Gross profit.xlsx", Convert.ToBase64String(stream.ToArray()));
        }
    }
}
