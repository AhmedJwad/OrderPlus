using Microsoft.AspNetCore.Components;
using MudBlazor;
using OrderPlus.Fronend.Repositories;
using OrderPlus.Fronend.Shared;
using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Entites;
using System.Data;

namespace OrderPlus.Fronend.Pages.Purchases
{
    public partial class PurchaseCreate
    {
        private TemporalPurchaseDTO temporalPurchaseDTO = new() { Date = DateTime.Now};
        private List<Supplier>? suppliers;
        private Supplier selectedSupplier = new();
        private List<Product>? products;
        private Product selectedProduct = new();
        private MudTable<TemporalPurchase> table = new();
        private List<TemporalPurchase> temporalPurchases = new();
        private float sumQuantity;
        private decimal sumValue;
        private const string baseUrl = "api/temporalPurchases";
        private readonly int[] pageSizeOptions = [5, 10, 20, int.MaxValue];
        private int totalRecords = 0;
        private bool loading;
        private string infoFormat = "{first_item}-{last_item} of {all_items}";

        [Inject] private IRepository repository { get; set; } = null!;
        [Inject] private ISnackbar snackbar { get; set; } = null!;
        [Inject] private IDialogService dialogService { get; set; } = null!;
        [Inject] private NavigationManager navigationManager { get; set; } = null!;
        [CascadingParameter] private MudDialogInstance dialogInstance { get; set; } = null!;
        public List<TemporalPurchase>? TemporalPurchases { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadSupplierAsync();
            await LoadProductAsync();
            await LoadTemporalPurchasesAsync();
        }

        

        private async Task LoadSupplierAsync()
        {
            var responseHttp = await repository.GetAsync<List<Supplier>>($"/api/suppliers/combo");
            if(responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                snackbar.Add(message, Severity.Error);
                return;
            }
            suppliers = responseHttp.Response;
        }
        private async Task LoadProductAsync()
        {
            var responseHttp = await repository.GetAsync<List<Product>>($"/api/products/combo");
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                snackbar.Add(message!, Severity.Error);
                return;
            }
            products = responseHttp.Response;
        }
        private async Task LoadTemporalPurchasesAsync()
        {
            var responseHttp = await repository.GetAsync<List<TemporalPurchase>>($"/api/TemporalPurchases/my");
            if(responseHttp.Error)
            {
                var message=await responseHttp.GetErrorMessageAsync();
                snackbar.Add(message!, Severity.Error);
                return;
            }
            foreach (var item in responseHttp.Response!)
            {
                temporalPurchases.Add(new TemporalPurchase
                {
                    Cost= item.Cost,                    
                    Quantity=item.Quantity,
                    ProductId=item.ProductId,
                    Remarks=item.Remarks,                     
                    
                });
            }
        }
        private async Task<TableData<TemporalPurchase>> LoadListAsync(TableState state)
        {
            var url = $"{baseUrl}/my";
            var responseHttp = await repository.GetAsync<List<TemporalPurchase>>(url);

            if(responseHttp.Error)
            {
                var message=await responseHttp.GetErrorMessageAsync();
                snackbar.Add(message, Severity.Error);
                return new TableData<TemporalPurchase> { Items = [], TotalItems=0 };
            }

            if (responseHttp.Response == null)
            {
                return new TableData<TemporalPurchase> { Items = [], TotalItems = 0 };
            }
            sumQuantity=responseHttp.Response.Sum(x => x.Quantity);
            sumValue=responseHttp.Response.Sum(x => x.Value);
            await InvokeAsync(StateHasChanged);
            return new TableData<TemporalPurchase>
            {
                Items=responseHttp.Response,
                TotalItems=responseHttp.Response.Count(),
            };
        }
        private async Task DeleteAsync(int temporalPurchaseId)
        {
            var parameters = new DialogParameters
            {
                { "Message", "Are you sure you want to delete the record?" }
            };
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };
            var dialog = dialogService.Show<ConfirmDialog>("Confirmation", parameters, options);
            var result = await dialog.Result;
            if (result.Canceled)
            {
                return;
            }

            var responseHttp = await repository.DeleteAsync<TemporalPurchase>($"api/temporalPurchases/{temporalPurchaseId}");
            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    navigationManager.NavigateTo("/");
                    return;
                }

                var message = await responseHttp.GetErrorMessageAsync();
                snackbar.Add(message!, Severity.Error);
                return;
            }

            await table.ReloadServerData();
            snackbar.Add("Product removed from purchase.", Severity.Success);
        }
        private async Task OnDateChange(DateTime? date)
        {
            await Task.Delay(1);
            if(date == null)
            {
                return;
            }
            temporalPurchaseDTO.Date=(DateTime)date;
        }
        private async Task AddProductAsync()
        {
            if (selectedProduct.Id == 0)
            {
                snackbar.Add("You must select a product.", Severity.Error);
                return;
            }
            if (temporalPurchaseDTO.Quantity <= 0)
            {
                snackbar.Add("You must enter an amount greater than zero", Severity.Error);
                return;
            }
            if (temporalPurchaseDTO.Cost <= 0)
            {
                snackbar.Add("You must enter a cost greater than zero.", Severity.Error);
            }
            var responseHttp = await repository.PostAsync<TemporalPurchaseDTO>("/api/TemporalPurchases/full", temporalPurchaseDTO);
            if(responseHttp.Error)
            {
                var messsage=await responseHttp.GetErrorMessageAsync();
                snackbar.Add(messsage, Severity.Error);
                return;                   
            }
            temporalPurchases.Add(new TemporalPurchase
            {
                Cost=temporalPurchaseDTO.Cost,
                ProductId=temporalPurchaseDTO.ProductId,
                Quantity=temporalPurchaseDTO.Quantity,
                Remarks=temporalPurchaseDTO.RemarksGeneral,
                
            });
            selectedProduct = new Product();
            temporalPurchaseDTO.Quantity = 1;
            temporalPurchaseDTO.Cost = 0;
            await table.ReloadServerData();
            snackbar.Add("Product added to cart.", Severity.Success);
        }
        private async Task SavePurchaseAsync()
        {
            if(selectedSupplier.Id==0)
            {
                snackbar.Add("You must select a provider.", Severity.Error);
            }
            if (sumQuantity <= 0)
            {
                snackbar.Add("You must add at least one product to the purchase.", Severity.Error);
                return;
            }

            var parameters = new DialogParameters
            {
                { "Message", "Are you sure you want to register this purchase?" }
            };
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };
            var dialog = dialogService.Show<ConfirmDialog>("Confirmation", parameters, options);
            var result = await dialog.Result;
            if (result.Canceled)
            {
                return;
            }
            var purchaseDTO = new PurchaseDTO
            {
                Date=temporalPurchaseDTO.Date,
                Remarks= temporalPurchaseDTO.RemarksGeneral,
                SupplierId= temporalPurchaseDTO.SupplierId,
                PurchaseDetails=new List<PurchaseDetailDTO>(),
               
            };
            foreach (var item in temporalPurchases)
            {
                purchaseDTO.PurchaseDetails.Add(new PurchaseDetailDTO
                {
                    Cost = item.Cost,
                    ProductId= item.ProductId,
                    Quantity = item.Quantity,
                    Remarks = item.Remarks,
                });
            }
            var responseHttp = await repository.PostAsync<PurchaseDTO>("/api/Purchases/full", purchaseDTO);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                snackbar.Add(message!, Severity.Error);
                return;
            }

            selectedSupplier = new Supplier();
            temporalPurchases.Clear();
            temporalPurchaseDTO.RemarksGeneral = string.Empty;
            dialogInstance.Close(DialogResult.Ok(true));
            navigationManager.NavigateTo("/purchases");
            snackbar.Add("Purchase added successfully.", Severity.Success);
        }
        private void SuplierChanged(Supplier supplier)
        {
            selectedSupplier = supplier;
            temporalPurchaseDTO.SupplierId = supplier.Id;
        }

        private void ProductChanged(Product product)
        {
            selectedProduct = product;
            temporalPurchaseDTO.ProductId = product.Id;
            temporalPurchaseDTO.Cost = product.Cost;
            temporalPurchaseDTO.Quantity = 1;
        }
        private async Task<IEnumerable<Supplier>> SearchSupplierAsync(string searchText)
        {
            await Task.Delay(5);
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return suppliers!;
            }

            return suppliers!
                .Where(x => x.SupplierName.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
                .ToList();
        }

        private async Task<IEnumerable<Product>> SearchProductAsync(string searchText)
        {
            await Task.Delay(5);
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return products!;
            }

            return products!
                .Where(x => x.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
                .ToList();
        }
    }
}
