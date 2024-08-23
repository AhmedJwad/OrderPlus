using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using OrderPlus.Backend.Helpers;
using OrderPlus.Backend.UnitsOfWork.Interfaces;
using OrderPlus.Shared.Entites;
using OrderPlus.Shared.Enums;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;

namespace OrderPlus.Backend.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUsersUnitOfWork _usersUnitOfWork;
        private readonly IFileStorage _fileStorage;
        private readonly IRuntimeInformationWrapper _runtimeInformationWrapper;

        public SeedDb(DataContext context, IUsersUnitOfWork usersUnitOfWork, IFileStorage fileStorage,
            IRuntimeInformationWrapper runtimeInformationWrapper)
        {
           _context = context;
           _usersUnitOfWork = usersUnitOfWork;
           _fileStorage = fileStorage;
           _runtimeInformationWrapper = runtimeInformationWrapper;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckCountriesAsync();
            await CheckRolesAsync();            
            await CheckUsersAsync();
            await CheckCategoriesAsync();
            await CheckProductsAsync();
        }

        private async Task CheckProductsAsync()
        {
           if(!_context.Products.Any())
            {
                await AddProductAsync("Adidas Barracuda", 270000M, ["Shoes", "Sports"], ["adidas_barracuda.jpg"], 230000M, 0.3F);
                await AddProductAsync("Adidas Superstar", 250000M, ["Shoes", "Sports"], ["Adidas_superstar.jpg"], 200000M, 0.3F);
                await AddProductAsync("Avocado", 5000M, ["Food"], ["Aguacate1.jpg", "Aguacate2.jpg", "Aguacate3.jpg"], 4000M, 0.3F);
                await AddProductAsync("AirPods", 1300000M, ["Technology", "Apple"], ["airpos.jpg", "airpos2.jpg"], 1000000M, 0.3F);
                await AddProductAsync("Akai APC40 MKII", 2650000M, ["Technology"], ["Akai1.jpg", "Akai2.jpg", "Akai3.jpg"], 2150000M, 0.3F);
                await AddProductAsync("Apple Watch Ultra", 4500000M, ["Apple", "Technology"], ["AppleWatchUltra1.jpg", "AppleWatchUltra2.jpg"], 4000000M, 0.3F);
                await AddProductAsync("Bose Headphones", 870000M, ["Technology"], ["audifonos_bose.jpg"], 800000M, 0.3F);
                await AddProductAsync("Ribble Bicycle", 12000000M, ["Sports"], ["bicicleta_ribble.jpg"], 10000000M, 0.3F);
                await AddProductAsync("Plaid Shirt", 56000M, ["Clothing"], ["camisa_cuadros.jpg"], 50000M, 0.3F);
                await AddProductAsync("Bicycle Helmet", 820000M, ["Sports"], ["casco_bicicleta.jpg", "casco.jpg"], 750000M, 0.3F);
                await AddProductAsync("Sports Glasses", 160000M, ["Sports"], ["Gafas1.jpg", "Gafas2.jpg", "Gafas3.jpg"], 130000M, 0.3F);
                await AddProductAsync("Triple Meat Burger", 25500M, ["Food"], ["Hamburguesa1.jpg", "Hamburguesa2.jpg", "Hamburguesa3.jpg"], 16500M, 0.3F);
                await AddProductAsync("iPad", 2300000M, ["Technology", "Apple"], ["ipad.jpg"], 200000M, 0.3F);
                await AddProductAsync("iPhone 13", 5200000M, ["Technology", "Apple"], ["iphone13.jpg", "iphone13b.jpg", "iphone13c.jpg", "iphone13d.jpg"], 4900000M, 0.3F);          
                await AddProductAsync("MacBook Pro", 12100000M, ["Technology", "Apple"], ["mac_book_pro.jpg"], 11500000M, 0.3F);
                await AddProductAsync("Dumbbells", 370000M, ["Sports"], ["mancuernas.jpg"], 300000M, 0.3F);
                await AddProductAsync("Face Mask", 26000M, ["Beauty"], ["mascarilla_cara.jpg"], 20000M, 0.3F);
                await AddProductAsync("New Balance 530", 180000M, ["Shoes", "Sports"], ["newbalance530.jpg"], 140000M, 0.3F);
                await AddProductAsync("New Balance 565", 179000M, ["Shoes", "Sports"], ["newbalance565.jpg"], 155000M, 0.3F);
                await AddProductAsync("Nike Air", 233000M, ["Shoes", "Sports"], ["nike_air.jpg"], 200000M, 0.3F);
                await AddProductAsync("Nike Zoom", 249900M, ["Shoes", "Sports"], ["nike_zoom.jpg"], 200000M, 0.3F);
                await AddProductAsync("Adidas Women's Sweatshirt", 134000M, ["Clothing", "Sports"], ["buso_adidas.jpg"], 100000M, 0.3F);
                await AddProductAsync("Boost Original Supplement", 15600M, ["Nutrition"], ["Boost_Original.jpg"], 150000M, 0.3F);
                await AddProductAsync("Whey Protein", 252000M, ["Nutrition"], ["whey_protein.jpg"], 200000M, 0.3F);
                await AddProductAsync("Pet Harness", 25000M, ["Pets"], ["arnes_mascota.jpg"], 20000M, 0.3F);
                await AddProductAsync("Pet Bed", 99000M, ["Pets"], ["cama_mascota.jpg"], 78000M, 0.3F);
                await AddProductAsync("Gamer Keyboard", 67000M, ["Gamer", "Technology"], ["teclado_gamer.jpg"], 53000M, 0.3F);
                await AddProductAsync("Luxury Ring 17", 1600000M, ["Cars"], ["Ring1.jpg", "Ring2.jpg"], 1350000M, 0.3F);
                await AddProductAsync("Gamer Chair", 980000M, ["Gamer", "Technology"], ["silla_gamer.jpg"], 715000M, 0.3F);
                await AddProductAsync("Gamer Mouse", 132000M, ["Gamer", "Technology"], ["mouse_gamer.jpg"], 99900M, 0.3F);

                await _context.SaveChangesAsync();
            }
        }
        private async Task AddProductAsync(string name, decimal price, List<string> categories, List<string> images, decimal cost, float desiredProfit)
        {
            Product prodcut = new()
            {
                Description = name,
                Name = name,
                Price = price,
                Cost = cost,
                DesiredProfit = desiredProfit,
                ProductCategories = new List<ProductCategory>(),
                ProductImages = new List<ProductImage>(),
            };

            foreach (var categoryName in categories)
            {
                var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == categoryName);
                if (category != null)
                {
                    prodcut.ProductCategories.Add(new ProductCategory { Category = category });
                }
            }

            foreach (string? image in images)
            {
                string filePath;
                if (_runtimeInformationWrapper.IsOSPlatform(OSPlatform.Windows))
                {
                    filePath = $"{Environment.CurrentDirectory}\\Images\\products\\{image}";
                }
                else
                {
                    filePath = $"{Environment.CurrentDirectory}/Images/products/{image}";
                }

                var fileBytes = File.ReadAllBytes(filePath);
                var imagePath = await _fileStorage.SaveFileAsync(fileBytes, ".jpg", "products");
                prodcut.ProductImages.Add(new ProductImage { Image = imagePath });
            }

            _context.Products.Add(prodcut);
        }
        private async Task CheckUsersAsync()
        {
            await CheckUserAsync( "Ahmed", "Almershady", "Ahmed@yopmail.com","+964", "322 311 4620", "babil 40 strret hilla", "AhmedAlmershady.jpg", UserType.Admin);
            await CheckUserAsync( "Ledys", "Bedoya", "ledys@yopmail.com", "+964", "322 311 4620", "babil 40 strret hilla", "LedysBedoya.jpg", UserType.User);
            await CheckUserAsync( "Brad", "Pitt", "brad@yopmail.com", "+964", "322 311 4620", "babil 40 strret hilla", "Brad.jpg", UserType.User);
            await CheckUserAsync( "Angelina", "Jolie", "angelina@yopmail.com", "+964", "322 311 4620", "babil 40 strret hilla", "Angelina.jpg", UserType.User);
            await CheckUserAsync( "Bob", "Marley", "bob@yopmail.com", "+964", "322 311 4620", "babil 40 strret hilla", "bob.jpg", UserType.User);
            await CheckUserAsync( "Celia", "Cruz", "celia@yopmail.com", "+964", "322 311 4620", "babil 40 strret hilla", "celia.jpg", UserType.Admin);
            await CheckUserAsync( "Fredy", "Mercury", "fredy@yopmail.com", "+964", "322 311 4620", "babil 40 strret hilla", "fredy.jpg", UserType.User);
            await CheckUserAsync( "Hector", "Lavoe", "hector@yopmail.com", "+964", "322 311 4620", "babil 40 strret hilla", "hector.jpg", UserType.User);
            await CheckUserAsync( "Liv", "Taylor", "liv@yopmail.com", "+964", "322 311 4620", "babil 40 strret hilla", "liv.jpg", UserType.User);
            await CheckUserAsync( "Otep", "Shamaya", "otep@yopmail.com", "+964", "322 311 4620", "babil 40 strret hilla", "otep.jpg", UserType.User);
            await CheckUserAsync( "Ozzy", "Osbourne", "ozzy@yopmail.com", "+964", "322 311 4620", "babil 40 strret hilla", "ozzy.jpg", UserType.User);
            await CheckUserAsync( "Selena", "Quintanilla", "selenba@yopmail.com", "+964", "322 311 4620", "babil 40 strret hilla", "selena.jpg", UserType.User);
        }

        private async Task<User> CheckUserAsync(string firstName, string lastName, string email, string countryCode,
            string phoneNumber, string address, string image, UserType userType)
        {
            var user = await _usersUnitOfWork.GetUserAsync(email);
            if (user == null)
            {
                var city = await GetCityAsync();
                string filePath;
                if (_runtimeInformationWrapper.IsOSPlatform(OSPlatform.Windows))
                {
                    filePath = $"{Environment.CurrentDirectory}\\images\\users\\{image}";
                }
                else
                {
                    filePath = $"{Environment.CurrentDirectory}/images/users/{image}";
                }

                var fileBytes = File.ReadAllBytes(filePath);
                var imagePath = await _fileStorage.SaveFileAsync(fileBytes, ".jpg", "users");

                user = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    UserName = email,
                    PhoneNumber = phoneNumber,
                    Address = address,
                    City = city,
                    UserType = userType,
                    Photo = imagePath,
                    CountryCode="+964"
                };

                await _usersUnitOfWork.AddUserAsync(user, "123456");
                await _usersUnitOfWork.AddUserToRoleAsync(user, userType.ToString());

                var token = await _usersUnitOfWork.GenerateEmailConfirmationTokenAsync(user);
                await _usersUnitOfWork.ConfirmEmailAsync(user, token);
            }

            return user;
           
        }

        private async Task<City> GetCityAsync()
        {
            var city = await _context.Cities.FirstOrDefaultAsync(x => x.Name == "40 street");
            city ??= await _context.Cities.FirstOrDefaultAsync();
            return city;
        }

        private async Task CheckRolesAsync()
        {
            await _usersUnitOfWork.CheckRoleAsync(UserType.Admin.ToString());
            await _usersUnitOfWork.CheckRoleAsync(UserType.User.ToString());
        }

        private async Task CheckCountriesAsync()
        {
            if (!_context.Countries.Any())
            {
                _context.Countries.Add(new Country
                {
                    Name = "Iraq",
                    States =
                  [
                      new State()
                        {
                            Name = "Baghdad",
                            Cities = [
                                new City { Name = "Al-Adel" },
                                new City { Name = "Al-Amal" },
                                new City { Name = "Al-Amin" },
                                new City { Name = "Al-Baiyaa" },
                                new City { Name = "Al-Baladiyat" },
                                new City { Name = "Al-Bayaa" },
                                new City { Name = "Al-Binook" },
                                new City { Name = "Al-Dora" },
                                new City { Name = "Al-Furat" },
                                new City { Name = "Al-Furat Al-Awsat" },
                                new City { Name = "Al-Ghazaliyah" },
                                new City { Name = "Al-Habibiyah" },
                                new City { Name = "Al-Hurriya" },
                                new City { Name = "Al-Ilam" },
                                new City { Name = "Al-Jadida" },
                                new City { Name = "Al-Kadhimiya" },
                                new City { Name = "Al-Karrada" },
                                new City { Name = "Al-Khudra" },
                                new City { Name = "Al-Kifah" },
                                new City { Name = "Al-Mada'in" },
                                new City { Name = "Al-Mansour" },
                                new City { Name = "Al-Mashtal" },
                                new City { Name = "Al-Ma'moun" },
                                new City { Name = "Al-Muthanna" },
                                new City { Name = "Al-Nahda" },
                                new City { Name = "Al-Nidhal" },
                                new City { Name = "Al-Rahmaniya" },
                                new City { Name = "Al-Rasheed" },
                                new City { Name = "Al-Risala" },
                                new City { Name = "Al-Saadoun" },
                                new City { Name = "Al-Sadr City" },
                                new City { Name = "Al-Salhiyah" },
                                new City { Name = "Al-Saydiyah" },
                                new City { Name = "Al-Shaab" },
                                new City { Name = "Al-Sho'ala" },
                                new City { Name = "Al-Sulaikh" },
                                new City { Name = "Al-Thawra" },
                                new City { Name = "Al-Waziriya" },
                                new City { Name = "Bab Al-Muadham" },
                                new City { Name = "Bab Al-Sharqi" },
                                new City { Name = "Hayy Al-Andalus" },
                                new City { Name = "Hayy Al-Amel" },
                                new City { Name = "Hayy Al-Amin" },
                                new City { Name = "Hayy Al-Bakr" },
                                new City { Name = "Hayy Al-Dhahir" },
                                new City { Name = "Hayy Al-Furat" },
                                new City { Name = "Hayy Al-Ghadeer" },
                                new City { Name = "Hayy Al-Hartha" },
                                new City { Name = "Hayy Al-Hurriya" },
                                new City { Name = "Hayy Al-Jihad" },
                                new City { Name = "Hayy Al-Jihad Al-Islami" },
                                new City { Name = "Hayy Al-Karakh" },
                                new City { Name = "Hayy Al-Karkh" },
                                new City { Name = "Hayy Al-Maalif" },
                                new City { Name = "Hayy Al-Mahdi" },
                                new City { Name = "Hayy Al-Mashtal" },
                                new City { Name = "Hayy Al-Mua'alimin" },
                                new City { Name = "Hayy Al-Muallimin" },
                                new City { Name = "Hayy Al-Muhandisin" },
                                new City { Name = "Hayy Al-Mujahidin" },
                                new City { Name = "Hayy Al-Murur" },
                                new City { Name = "Hayy Al-Nahdha" },
                                new City { Name = "Hayy Al-Qadiriya" },
                                new City { Name = "Hayy Al-Qadisiya" },
                                new City { Name = "Hayy Al-Qanat" },
                                new City { Name = "Hayy Al-Rashid" },
                                new City { Name = "Hayy Al-Sadeer" },
                                new City { Name = "Hayy Al-Sarai" },
                                new City { Name = "Hayy Al-Saydiyah" },
                                new City { Name = "Hayy Al-Shoula" },
                                new City { Name = "Hayy Al-Suwaib" },
                                new City { Name = "Hayy Al-Taji" },
                                new City { Name = "Hayy Al-Tayaran" },
                                new City { Name = "Hayy Al-Yarmouk" },
                                new City { Name = "Hayy Al-Zayuna" },
                                new City { Name = "Hayy Al-Zuhur" },
                                new City { Name = "Hayy Amin" },
                                new City { Name = "Hayy Arab Jabour" },
                                new City { Name = "Hayy As-Salam" },
                                new City { Name = "Hayy Attar" },
                                new City { Name = "Hayy Babil" },
                                new City { Name = "Hayy Fadl" },
                                new City { Name = "Hayy Jameela" },
                                new City { Name = "Hayy Karrada" },
                                new City { Name = "Hayy Malikiya" },
                                new City { Name = "Hayy Salman" },
                                new City { Name = "Hayy Shorja" },
                                new City { Name = "Hayy Ur" },
                                new City { Name = "Kadhimiya" },
                                new City { Name = "Madinat Al-Sadr" },
                                new City { Name = "Masbah" },
                                new City { Name = "New Baghdad" },
                                new City { Name = "Old Baghdad" },
                                new City { Name = "Sadr City" },
                                new City { Name = "Tayaran" }
                            ]
                        },
                       new State()
                        {
                            Name = "Al Anbar",
                           Cities = new List<City>
                            {
                                new City { Name = "Ramadi" },
                                new City { Name = "Fallujah" },
                                new City { Name = "Haditha" },
                                new City { Name = "Hit" },
                                new City { Name = "Rawa" },
                                // Add more cities as needed
                            }
                        },
                        new State()
                        {
                            Name = "Al Basrah",
                            Cities = new List<City>
                            {
                                new City { Name = "Basra" },
                                new City { Name = "Umm Qasr" },
                                new City { Name = "Al-Zubair" },
                                new City { Name = "Shatt al-Arab" },
                                new City { Name = "Al-Faw" },
                                // Add more cities as needed
                            }
                        },
                        new State()
                        {
                            Name = "Al Muthanna",
                            Cities = new List<City>
                            {
                                new City { Name = "Samawah" },
                                new City { Name = "Al-Rumaytha" },
                                new City { Name = "Al-Khidhir" },
                                new City { Name = "Al-Qasim" },
                                new City { Name = "Al-Diwaniyah" },
                                // Add more cities as needed
                            }
                        },
                        // Add more states and their cities here
                        new State()
                        {
                            Name = "Babil",
                            Cities = new List<City>
                            {
                                new City { Name = "Alkaram" },
                                new City { Name = "hey Alhussein" },
                                new City { Name = "Aljameaa" },
                                new City { Name = "40 street" },
                                new City { Name = "60 street" },
                                // Add more cities as needed
                            }
                        },
                    ]
                });
                _context.Countries.Add(new Country
                {
                    Name = "Oman",
                    States =
                    [
                        new State()
                        {
                            Name = "Ad Dakhiliyah",
                           Cities = [
                                new() { Name = "Nizwa" },
                                new() { Name = "Samail" },
                                new() { Name = "Bahla" },
                                new() { Name = "Adam" },
                                new() { Name = "Al Hamra" },
                            ]
                        },
                        new State()
                        {
                            Name = "Al Buraimi",
                            Cities = [
                                new() { Name = "Houston" },
                                new() { Name = "San Antonio" },
                                new() { Name = "Dallas" },
                                new() { Name = "Austin" },
                                new() { Name = "El Paso" },
                            ]
                        },
                    ]
                });
                await _context.SaveChangesAsync();
            }
              
        }

        private async Task CheckCategoriesAsync()
        {
            if(!_context.Categories.Any())
            {
                _context.Categories.Add(new Category { Name = "Apple" });
                _context.Categories.Add(new Category { Name = "Cars" });
                _context.Categories.Add(new Category { Name = "Beauty" });
                _context.Categories.Add(new Category { Name = "Footwear" });
                _context.Categories.Add(new Category { Name = "Food" });
                _context.Categories.Add(new Category { Name = "Cosmetics" });
                _context.Categories.Add(new Category { Name = "Sports" });
                _context.Categories.Add(new Category { Name = "Gaming" });
                _context.Categories.Add(new Category { Name = "Toys" });
                _context.Categories.Add(new Category { Name = "Pets" });
                _context.Categories.Add(new Category { Name = "Nutrition" });
                _context.Categories.Add(new Category { Name = "Clothing" });
                _context.Categories.Add(new Category { Name = "Technology" });
            }
            await _context.SaveChangesAsync();
        }

    }
}

