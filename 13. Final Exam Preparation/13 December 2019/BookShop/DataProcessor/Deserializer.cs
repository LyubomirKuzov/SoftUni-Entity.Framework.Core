namespace BookShop.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using BookShop.Data.Models;
    using BookShop.Data.Models.Enums;
    using BookShop.DataProcessor.ImportDto;
    using Data;
    using Newtonsoft.Json;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedBook
            = "Successfully imported book {0} for {1:F2}.";

        private const string SuccessfullyImportedAuthor
            = "Successfully imported author - {0} with {1} books.";

        public static string ImportBooks(BookShopContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            var serializer = new XmlSerializer(typeof(List<ImportBooks>), new XmlRootAttribute("Books"));

            var booksDTO = (List<ImportBooks>)serializer.Deserialize(new StringReader(xmlString));

            var books = new List<Book>();

            foreach (var bookDTO in booksDTO)
            {
                if (!IsValid(bookDTO))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                DateTime publishedOn;

                var publishedOnValid = DateTime.TryParseExact(bookDTO.PublishedOn,
                    "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out publishedOn);

                if (!publishedOnValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var book = new Book()
                {
                    Name = bookDTO.Name,
                    Genre = (Genre)bookDTO.Genre,
                    Price = bookDTO.Price,
                    Pages = bookDTO.Pages,
                    PublishedOn = publishedOn
                };

                books.Add(book);

                sb.AppendLine(string.Format(SuccessfullyImportedBook, book.Name, book.Price));
            }

            context.Books.AddRange(books);

            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportAuthors(BookShopContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            var authorsDTO = JsonConvert.DeserializeObject<List<ImportAuthor>>(jsonString);

            var authors = new List<Author>();
            var emails = new List<string>();

            foreach (var authorDTO in authorsDTO)
            {
                if (!IsValid(authorDTO))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (emails.Contains(authorDTO.Email))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                emails.Add(authorDTO.Email);

                var author = new Author()
                {
                    FirstName = authorDTO.FirstName,
                    LastName = authorDTO.LastName,
                    Phone = authorDTO.Phone,
                    Email = authorDTO.Email
                };

                foreach (var bookDTO in authorDTO.Books)
                {
                    if (!bookDTO.BookId.HasValue)
                    {
                        continue;
                    }

                    var book = context.Books.FirstOrDefault(x => x.Id == bookDTO.BookId);

                    if (book == null)
                    {
                        continue;
                    }

                    author.AuthorsBooks.Add(new AuthorBook()
                    {
                        Author = author,
                        Book = book
                    });
                }

                if (author.AuthorsBooks.Count == 0)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                authors.Add(author);

                sb.AppendLine(string.Format(SuccessfullyImportedAuthor, author.FirstName + " " + author.LastName,
                    author.AuthorsBooks.Count));
            }

            context.Authors.AddRange(authors);

            context.SaveChanges();

            return sb.ToString().Trim();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}