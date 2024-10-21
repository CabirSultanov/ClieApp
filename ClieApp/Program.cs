using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;


namespace ClientApp
{
    public class Program
    {
        public HttpClient Client = new HttpClient();
        public string BaseUrl = "http://localhost:7070/";

        public static async Task Main()
        {
            var program = new Program();
            await program.RunAsync();
        }

        public async Task RunAsync()
        {
            Console.WriteLine("Welcome to the Book Management System!");

            while (true)
            {
                Console.WriteLine("\nMenu:");
                Console.WriteLine("1. View a book by ID");
                Console.WriteLine("2. Add a new book");
                Console.WriteLine("3. Delete a book");
                Console.WriteLine("4. Change a book");
                Console.WriteLine("5. Exit");
                Console.Write("Input your choice (1-5): ");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        await GetBookByIdAsync();
                        break;
                    case "2":
                        await AddBookAsync();
                        break;
                    case "3":
                        await DeleteBookAsync();
                        break;
                    case "4":
                        await UpdateBookAsync();
                        break;
                    case "5":
                        Console.WriteLine("Exit from the application. Goodbye!");
                        return;
                    default:
                        Console.WriteLine("Wrong choice. Please input a number between 1 and 5.");
                        break;
                }
            }
        }

        public async Task GetBookByIdAsync()
        {
            Console.WriteLine("\nRetrieving a book by ID...");
            var idStr = ShortReader("Input the ID of the book: ");

            if (int.TryParse(idStr, out int id))
            {
                try
                {
                    var response = await Client.GetAsync($"{BaseUrl}/books/{id}");

                    if (response.IsSuccessStatusCode)
                    {
                        var book = await response.Content.ReadFromJsonAsync<Book>();
                        if (book != null)
                        {
                            Console.WriteLine("Book Details:");
                            Console.WriteLine($"ID: {book.Id}");
                            Console.WriteLine($"Name: {book.Name}");
                            Console.WriteLine($"Author: {book.Author}");
                            Console.WriteLine($"Description: {book.Description}");
                            Console.WriteLine($"Price: {book.Price}");
                        }
                        else
                            Console.WriteLine("Book not found.");
                    }
                    else
                        Console.WriteLine($"Failed to retrieve book. Status: {response.StatusCode}");
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Request error: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Incorrect input! Book ID must be a number!");
            }
        }

        public async Task AddBookAsync()
        {
            Console.WriteLine("\nAdding a new book...");
            var book = new Book
            {
                Name = ShortReader("Input the name of the book: "),
                Author = ShortReader("Input the author's name: "),
                Description = ShortReader("Input the book's description: "),
                Price = GetBookPrice()
            };

            try
            {
                var response = await Client.PostAsJsonAsync($"{BaseUrl}/books", book);

                if (response.IsSuccessStatusCode)
                    Console.WriteLine("Book added successfully.");

                else
                    Console.WriteLine($"Failed to add book. Status: {response.StatusCode}");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Request error: {ex.Message}");
            }
        }

        public async Task DeleteBookAsync()
        {
            Console.WriteLine("\nDeleting a book...");
            var idStr = ShortReader("Input the ID of the book to delete: ");

            if (int.TryParse(idStr, out int id))
            {
                try
                {
                    var response = await Client.DeleteAsync($"{BaseUrl}/books/{id}");

                    if (response.IsSuccessStatusCode)
                        Console.WriteLine("Book deleted successfully.");
                    else
                        Console.WriteLine($"Failed to delete book. Status: {response.StatusCode}");
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Request error: {ex.Message}");
                }
            }
            else
                Console.WriteLine("Incorrect input! Book ID must be a number!");
        }

        public async Task UpdateBookAsync()
        {
            Console.WriteLine("\nUpdating a book...");
            var idStr = ShortReader("Input the ID of the book to update: ");

            if (int.TryParse(idStr, out int id))
            {
                var book = new Book
                {
                    Id = id,
                    Name = ShortReader("Input the new name of the book: "),
                    Author = ShortReader("Input the new author's name: "),
                    Description = ShortReader("Input the new book's description: "),
                    Price = GetBookPrice()
                };

                try
                {
                    var response = await Client.PutAsJsonAsync($"{BaseUrl}/books/{id}", book);

                    if (response.IsSuccessStatusCode)
                        Console.WriteLine("Book updated successfully.");
                    else
                        Console.WriteLine($"Failed to update book. Status: {response.StatusCode}");
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Request error: {ex.Message}");
                }
            }
            else
                Console.WriteLine("Incorrect input! Book ID must be a number!");
        }

        private double GetBookPrice()
        {
            while (true)
            {
                var priceStr = ShortReader("Enter the book's price: ");
                if (double.TryParse(priceStr, out double price))
                    return price;

                else
                    Console.WriteLine("Incorrect input! Price must be a valid number.");
            }
        }

        private string ShortReader(string message)
        {
            Console.Write(message);
            return Console.ReadLine();
        }
    }

    public class Book
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string? Description { get; set; }
        public double Price { get; set; }
    }
}
