namespace VaporStore.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.Data.Models;
    using VaporStore.DataProcessor.Dto.Import;

    public static class Deserializer
    {
        private const string ERROR_MESSAGE = "Invalid Data";

        public static string ImportGames(VaporStoreDbContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            var gamesDTO = JsonConvert.DeserializeObject<List<ImportGamesGame>>(jsonString);

            var games = new List<Game>();
            var developers = new List<Developer>();
            var genres = new List<Genre>();
            var tags = new List<Tag>();

            foreach (var gameDTO in gamesDTO)
            {
                if (!IsValid(gameDTO))
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                DateTime releaseDate;

                var isReleaseDateValid = DateTime.TryParseExact(gameDTO.ReleaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out releaseDate);

                if (!isReleaseDateValid)
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                var game = new Game()
                {
                    Name = gameDTO.Name,
                    Price = gameDTO.Price,
                    ReleaseDate = releaseDate
                };

                var developer = developers.FirstOrDefault(x => x.Name == gameDTO.Developer);

                if (developer == null)
                {
                    developer = new Developer()
                    {
                        Name = gameDTO.Developer
                    };
                }

                developers.Add(developer);
                game.Developer = developer;

                var genre = genres.FirstOrDefault(x => x.Name == gameDTO.Genre);

                if (genre == null)
                {
                    genre = new Genre()
                    {
                        Name = gameDTO.Genre
                    };
                }

                genres.Add(genre);
                game.Genre = genre;

                foreach (var tagDTO in gameDTO.Tags)
                {
                    Tag tag = tags.FirstOrDefault(x => x.Name == tagDTO);

                    if (tag == null)
                    {
                        tag = new Tag()
                        {
                            Name = tagDTO
                        };
                    }

                    tags.Add(tag);

                    game.GameTags.Add(new GameTag()
                    {
                        Game = game,
                        Tag = tag
                    });
                }

                if (game.GameTags.Count == 0)
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                games.Add(game);

                sb.AppendLine($"Added {game.Name} ({game.Genre.Name}) with {game.GameTags.Count} tags");
            }

            context.Games.AddRange(games);

            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportUsers(VaporStoreDbContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            var usersDTO = JsonConvert.DeserializeObject<List<ImportUsersUser>>(jsonString);

            var users = new List<User>();

            foreach (var userDTO in usersDTO)
            {
                if (!IsValid(userDTO))
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                var user = new User()
                {
                    FullName = userDTO.FullName,
                    Username = userDTO.Username,
                    Age = userDTO.Age,
                    Email = userDTO.Email
                };

                foreach (var cardDTO in userDTO.Cards)
                {
                    if (!IsValid(cardDTO))
                    {
                        sb.AppendLine(ERROR_MESSAGE);
                        continue;
                    }

                    var card = new Card()
                    {
                        Number = cardDTO.Number,
                        Cvc = cardDTO.Cvc,
                        Type = cardDTO.Type,
                        User = user
                    };

                    user.Cards.Add(card);
                }

                users.Add(user);

                sb.AppendLine($"Imported {user.Username} with {user.Cards.Count} cards");
            }

            context.Users.AddRange(users);

            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            var serializer = new XmlSerializer(typeof(List<ImportPurchase>), new XmlRootAttribute("Purchases"));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            var reader = new StringReader(xmlString);

            var purchasesDTO = (List<ImportPurchase>)serializer.Deserialize(reader);

            var purchases = new List<Purchase>();

            foreach (var purchaseDTO in purchasesDTO)
            {
                if (!IsValid(purchasesDTO))
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                DateTime purchaseDate;

                var purchaseDateIsValid = DateTime.TryParseExact(purchaseDTO.Date, "dd/MM/yyyy HH:mm",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out purchaseDate);

                if (!purchaseDateIsValid)
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                var game = context.Games.FirstOrDefault(x => x.Name == purchaseDTO.Title);

                if (game == null)
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue; // ???
                }

                var card = context.Cards.FirstOrDefault(x => x.Number == purchaseDTO.Card);

                if (card == null)
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue; // ???
                }

                var purchase = new Purchase()
                {
                    Game = game,
                    Type = purchaseDTO.Type,
                    Date = purchaseDate,
                    ProductKey = purchaseDTO.Key,
                    Card = card
                };

                purchases.Add(purchase);

                sb.AppendLine($"Imported {game.Name} for {purchase.Card.User.Username}");
            }

            context.Purchases.AddRange(purchases);

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