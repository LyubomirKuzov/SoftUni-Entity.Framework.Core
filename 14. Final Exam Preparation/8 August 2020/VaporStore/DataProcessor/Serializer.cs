namespace VaporStore.DataProcessor
{
	using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.Data.Models.Enums;
    using VaporStore.DataProcessor.Dto.Export;

    public static class Serializer
	{
		public static string ExportGamesByGenres(VaporStoreDbContext context, string[] genreNames)
		{
			var genres = context
				.Genres
				.ToList()
				.Where(x => genreNames.Contains(x.Name))
				.Select(g => new ExportGenresGenre()
				{
					Id = g.Id,
					Genre = g.Name,
					Games = g.Games
					.Where(ga => ga.Purchases.Count() >= 1)
					.Select(ga => new ExportGenresGame()
					{
						Id = ga.Id,
						Title = ga.Name,
						Developer = ga.Developer.Name,
						Tags = string.Join(", ", ga.GameTags.Select(gt => gt.Tag.Name)),
						Players = ga.Purchases.Count
					})
					.OrderByDescending(x => x.Players)
					.ThenBy(x => x.Id)
					.ToArray(),
					TotalPlayers = g.Games.Sum(z => z.Purchases.Count)
				})
				.OrderByDescending(x => x.TotalPlayers)
				.ThenBy(x => x.Id)
				.ToList();

			var json = JsonConvert.SerializeObject(genres, Formatting.Indented);

			return json;
		}

		public static string ExportUserPurchasesByType(VaporStoreDbContext context, string storeType)
		{
			StringBuilder sb = new StringBuilder();

			var users = context
				.Users
				.ToList()
				.Where(u => u.Cards.Any(c => c.Purchases.Any()))
				.Select(u => new ExportPurchasesUser()
				{
					Username = u.Username,
					Purchases = context
						.Purchases
						.ToList()
						.Where(p => p.Card.User.Username == u.Username
								&& p.Type == Enum.Parse<PurchaseType>(storeType))
						.Select(p => new ExportPurchasesPurchase()
						{
							Card = p.Card.Number,
							Date = p.Date.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
							Cvc = p.Card.Cvc,
							Game = new ExportPurchaseGame()
							{
								Genre = p.Game.Genre.Name,
								Price = p.Game.Price,
								Title = p.Game.Name
							}
						})
						.OrderBy(p => p.Date)
						.ToList(),
					TotalSpent = context.Purchases
						.ToList()
						.Where(p => p.Card.User.Username == u.Username &&
								p.Type == Enum.Parse<PurchaseType>(storeType))
						.Sum(p => p.Game.Price)
				})
				.ToList()
				.Where(u => u.Purchases.Count >= 1)
				.OrderByDescending(u => u.TotalSpent)
				.ThenBy(u => u.Username)
				.ToList();

			var serializer = new XmlSerializer(typeof(List<ExportPurchasesUser>), new XmlRootAttribute("Users"));

			var namespaces = new XmlSerializerNamespaces();
			namespaces.Add(string.Empty, string.Empty);

			var writer = new StringWriter(sb);

			serializer.Serialize(writer, users, namespaces);

			return sb.ToString().Trim();
		}
	}
}