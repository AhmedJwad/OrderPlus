

using Microsoft.AspNetCore.Components;
using MudBlazor;
using OrderPlus.Fronend.Repositories;
using OrderPlus.Shared.Entites;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;
using OrderPlus.Shared.DTOs;

namespace OrderPlus.Fronend.Pages.Charts
{
    public partial class ProductChart
    {
        private int Index = -1; //default value cannot be 0 -> first selectedindex is 0.

        private Position LegendPosition = Position.Bottom;
        // Chart series to hold the number of products per category
        public List<ChartSeries> Series = new List<ChartSeries>();

        // Labels for the X axis, which will be the category names
        public string[] XAxisLabels = null!;

        // Lists to hold fetched products and categories
        public List<Product>? Products { get; set; }
        public List<Category>? Categories { get; set; }
        [Inject] private ISnackbar Snackbar { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;
        private List<CategoryProductDTO> results;
        protected override async Task OnInitializedAsync()
        {
           
            await getResults();
            
        }

        private async Task<bool> getResults()
        {
            var responseHttpCategories = await Repository.GetAsync<List<CategoryProductDTO>>("api/Products/getProductbyCategory");
            if (responseHttpCategories.Error)
            {
                var message = await responseHttpCategories.GetErrorMessageAsync();
                Snackbar.Add(message, Severity.Error);
               
            }

            // Count products and store in dictionary
            results = responseHttpCategories!.Response!;
             PrepareChartData(results);
          
            return true;
        }

       

        private void PrepareChartData(List<CategoryProductDTO> results)
        {
            Series.Clear(); // Clear existing data
            double[] productCounts = results.Select(r => (double)r.ProductCount).ToArray(); // Get product counts
            XAxisLabels = results.Select(r => r.CategoryName!).ToArray(); // Get category names
            foreach (var item in results)
            {
                Series.Add(new ChartSeries()
                {
                    Name = item.CategoryName,               // Set the category name
                    Data= productCounts
               
                });
            }

            

           

            //Series.Add(new ChartSeries
            //{
            //    Name = "Product Count",
            //    Data = productCounts

            //});
        }
        // Method to randomize product counts for demonstration
        public void RandomizeData()
        {
            var random = new Random();
            foreach (var series in Series)
            {
                // Randomize the product count for each series (category)
                series.Data[0] = random.NextDouble();  // Random value between 1 and 100
            }
            StateHasChanged(); // Refresh the UI
        }
    }
    }

