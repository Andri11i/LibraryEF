using LibraryEF.Data;
using LibraryEF.Models;
using Microsoft.EntityFrameworkCore;

public class Program
{
    static void Main()
    {
        bool doing = true;
        while (doing)
        {
            Console.WriteLine("Library \n 1 - Rent a book \n 2 - Cancel rental \n 3 - Search for books \n 0 - Exit");
            int choice = int.Parse(Console.ReadLine()!);
            var rentalFunctions = new RentalFuncs();

            switch (choice)
            {
                case 1:
                    var newRental = new Rental();
                    Console.WriteLine("Enter the ID of the user:");
                    newRental.UserId = int.Parse(Console.ReadLine()!);
                    Console.WriteLine("Enter the ID of the book:");
                    newRental.BookId = int.Parse(Console.ReadLine()!);
                    Console.WriteLine("Enter the start date:");
                    newRental.StartDate = DateTime.Parse(Console.ReadLine()!);
                    Console.WriteLine("Enter the end date:");
                    newRental.EndDate = DateTime.Parse(Console.ReadLine()!);
                    rentalFunctions.CreateRental(newRental);
                    break;

                case 2:
                    Console.WriteLine("Enter the rental ID to cancel:");
                    int rentalId = int.Parse(Console.ReadLine()!);
                    rentalFunctions.CancelRental(rentalId);
                    break;
                case 3:
                    Console.WriteLine("Enter a keyword to search for books (by title or author):");
                    string query = Console.ReadLine()!;
                    var results = rentalFunctions.SearchBooks(query);
                    foreach (var book in results)
                    {
                        Console.WriteLine($"ID: {book.Id}, Title: {book.Title}, Author: {book.Author}, ISBN: {book.ISBN}");
                    }
                    break;
                case 0:
                    doing = false;
                    break;
            }
        }
    }
}



public class RentalFuncs
{
    public LibraryEfContext context = new LibraryEfContext();

  

    public bool IsBookAvailable(int bookId, DateTime startDate, DateTime endDate)
    {
        return !context.Rentals.Any(r => r.BookId == bookId &&
            ((startDate >= r.StartDate && startDate < r.EndDate) ||
             (endDate > r.StartDate && endDate <= r.EndDate) ||
             (startDate <= r.StartDate && endDate >= r.EndDate)));
    }
    public Rental CreateRental(Rental rental)
    {
        if (!IsBookAvailable(rental.BookId, rental.StartDate, rental.EndDate))
        {
            Console.WriteLine("Book is not available for the selected dates.");
        }

        context.Rentals.Add(rental);
        context.SaveChanges();
        return rental;
    }
    public Rental GetRental(int id)
    {
        return context.Rentals.FirstOrDefault(r => r.Id == id);
    }

    public Rental UpdateRental(int id, DateTime startDate, DateTime endDate)
    {
        var rental = context.Rentals.FirstOrDefault(r => r.Id == id);

        if (rental == null)
            Console.WriteLine("Rental not found.");

        if (!IsBookAvailable(rental.BookId, startDate, endDate))
        {
            Console.WriteLine("Book is not available for the selected dates.");
        }

        rental.StartDate = startDate;
        rental.EndDate = endDate;

        context.SaveChanges();
        return rental;
    }

    public void CancelRental(int id)
    {
        var rental = context.Rentals.FirstOrDefault(r => r.Id == id);

        if (rental == null)
            Console.WriteLine("Rental not found.");
        context.Rentals.Remove(rental);
        context.SaveChanges();
    }

    public List<Book> SearchBooks(string query)
    {
        return context.Books.Where(b => b.Title.Contains(query) || b.Author.Contains(query)).ToList();
    }
}
