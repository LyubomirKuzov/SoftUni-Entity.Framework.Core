namespace SoftJail.DataProcessor
{
    using Data;
    using Newtonsoft.Json;
    using SoftJail.DataProcessor.ExportDto;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    public class Serializer
    {
        public static string ExportPrisonersByCells(SoftJailDbContext context, int[] ids)
        {
            var prisoners = context.Prisoners
                .ToList()
                .Where(p => ids.Contains(p.Id))
                .Select(p => new PrisonersCellsOfficersPrisoner()
                {
                    Id = p.Id,
                    Name = p.FullName,
                    CellNumber = p.Cell.CellNumber,
                    Officers = p.PrisonerOfficers.Select(po => new PrisonersCellsOfficersOfficer()
                    {
                        FullName = po.Officer.FullName,
                        Department = po.Officer.Department.Name
                    })
                    .ToArray()
                    .OrderBy(o => o.FullName)
                    .ToArray(),
                    TotalOfficerSalary = p.PrisonerOfficers.Sum(o => o.Officer.Salary)
                })
                .ToList()
                .OrderBy(p => p.Name)
                .ThenBy(p => p.Id)
                .ToList();

            var json = JsonConvert.SerializeObject(prisoners, Formatting.Indented);

            return json;
        }

        public static string ExportPrisonersInbox(SoftJailDbContext context, string prisonersNames)
        {
            string[] prisonerNames = prisonersNames
                .Split(new char[] { ',' })
                .ToArray();

            var prisoners = context.Prisoners
                .ToList()
                .Where(p => prisonerNames.Contains(p.FullName))
                .Select(p => new InboxPrisonersPrisoner()
                {
                    Id = p.Id,
                    Name = p.FullName,
                    IncarcerationDate = p.IncarcerationDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    EncryptedMessages = p.Mails
                    .ToArray()
                    .Select(m => new InboxPrisonersMail()
                    {
                        Description = ReverseString(m.Description)
                    })
                    .ToArray()
                })
                .ToList()
                .OrderBy(p => p.Name)
                .ThenBy(p => p.Id)
                .ToList();

            var serializer = new XmlSerializer(typeof(List<InboxPrisonersPrisoner>), new XmlRootAttribute("Prisoners"));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            StringBuilder sb = new StringBuilder();

            var writer = new StringWriter(sb);

            serializer.Serialize(writer, prisoners, namespaces);

            return sb.ToString().Trim();
        }

        private static string ReverseString(string oldString)
        {
            var charArray = oldString.ToCharArray();

            Array.Reverse(charArray);

            return new string(charArray);
        }
    }
}