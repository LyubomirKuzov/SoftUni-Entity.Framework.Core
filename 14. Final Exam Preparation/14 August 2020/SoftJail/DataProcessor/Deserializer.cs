namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using SoftJail.Data.Models;
    using SoftJail.Data.Models.Enums;
    using SoftJail.DataProcessor.ImportDto;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;

    public class Deserializer
    {
        private const string ERROR_MESSAGE = "Invalid Data";

        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            var departmentsDTO = JsonConvert.DeserializeObject<List<ImportDepartmentsAndCellsDepartmentDTO>>(jsonString);

            var departments = new List<Department>();

            foreach (var departmentDTO in departmentsDTO)
            {
                if (!IsValid(departmentDTO))
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                var department = new Department()
                {
                    Name = departmentDTO.Name
                };

                foreach (var cellDTO in departmentDTO.Cells)
                {
                    if (!IsValid(cellDTO))
                    {
                        sb.AppendLine(ERROR_MESSAGE);

                        break;
                    }

                    var cell = new Cell()
                    {
                        CellNumber = cellDTO.CellNumber,
                        HasWindow = cellDTO.HasWindow,
                        Department = department
                    };

                    department.Cells.Add(cell);
                }

                if (department.Cells.Count != 0)
                {
                    departments.Add(department);

                    sb.AppendLine($"Imported {department.Name} with {department.Cells.Count} cells");
                }
            }

            context.Departments.AddRange(departments);

            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportPrisonersMails(SoftJailDbContext context, string jsonString)
        {
            var prisonersDTO = JsonConvert.DeserializeObject<List<ImportPrisonersAndMailsPrisonerDTO>>(jsonString);

            var prisoners = new List<Prisoner>();

            StringBuilder sb = new StringBuilder();

            foreach (var prisonerDTO in prisonersDTO)
            {
                if (!IsValid(prisonerDTO))
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                DateTime incarcerationDate;

                var incarcerationDateValid = DateTime.TryParseExact(prisonerDTO.IncarcerationDate, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None ,out incarcerationDate);

                if (!incarcerationDateValid)
                {
                    continue;
                }

                DateTime? releaseDate = null;

                if (prisonerDTO.ReleaseDate != null)
                {
                    DateTime releaseDateValue;

                    var releaseDateValid = DateTime.TryParseExact(prisonerDTO.ReleaseDate, "dd/MM/yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out releaseDateValue);

                    if (!releaseDateValid)
                    {
                        continue;
                    }
                }

                var prisoner = new Prisoner()
                {
                    FullName = prisonerDTO.FullName,
                    Nickname = prisonerDTO.Nickname,
                    Age = prisonerDTO.Age,
                    IncarcerationDate = incarcerationDate,
                    ReleaseDate = releaseDate,
                    CellId = prisonerDTO.CellId,
                    Bail = prisonerDTO.Bail
                };

                foreach (var mailDTO in prisonerDTO.Mails)
                {
                    if (!IsValid(mailDTO))
                    {
                        sb.AppendLine(ERROR_MESSAGE);
                        break;
                    }

                    var mail = new Mail()
                    {
                        Prisoner = prisoner,
                        Address = mailDTO.Address,
                        Description = mailDTO.Description,
                        Sender = mailDTO.Sender
                    };

                    prisoner.Mails.Add(mail);
                }

                prisoners.Add(prisoner);

                sb.AppendLine($"Imported {prisoner.FullName} {prisoner.Age} years old");
            }

            context.AddRange(prisoners);

            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ImportOfficersAndPrisonersOfficer[]), new XmlRootAttribute("Officers"));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            StringBuilder sb = new StringBuilder();

            var officersDTO = (ImportOfficersAndPrisonersOfficer[])serializer.Deserialize(new StringReader(xmlString));

            var officers = new List<Officer>();

            foreach (var officerDTO in officersDTO)
            {
                if (!IsValid(officerDTO))
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                Position position;

                var isPositionValid = Enum.TryParse(officerDTO.Position, out position);

                if (!isPositionValid)
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                Weapon weapon;

                var isWeaponValid = Enum.TryParse(officerDTO.Weapon, out weapon);

                if (!isWeaponValid)
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                var officer = new Officer()
                {
                    FullName = officerDTO.Name,
                    DepartmentId = officerDTO.DepartmentId,
                    Position = position,
                    Weapon = weapon,
                    Salary = officerDTO.Money
                };

                foreach (var prisonerDTO in officerDTO.Prisoners)
                {
                    var prisoner = new OfficerPrisoner()
                    {
                        Officer = officer,
                        PrisonerId = prisonerDTO.id
                    };

                    officer.OfficerPrisoners.Add(prisoner);
                }

                officers.Add(officer);

                sb.AppendLine($"Imported {officer.FullName} ({officer.OfficerPrisoners.Count} prisoners)");
            }

            context.Officers.AddRange(officers);

            context.SaveChanges();

            return sb.ToString().Trim();
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
            return isValid;
        }
    }
}