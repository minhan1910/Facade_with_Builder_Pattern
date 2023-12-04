using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;

namespace FacetedBuilderPatternDemo
{
    class Person
    {
        // address
        public string? StreetAddress { get; set; }
        public string? PostalCode { get; set; }
        public string? City { get; set; }
        // employment
        public string? Position { get; set; }
        public string? ComponayName { get; set; }
        public int AnnualIncome { get; set; }
        public int Age { get; set; }

        public override string ToString()
        {
            // applying for properties not using for fields
            return JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }); 
        }

        // can be used by formatting json below .net 6-7
        private string FormatJsonText(string jsonString)
        {
            using var doc = JsonDocument.Parse(
                jsonString,
                new JsonDocumentOptions
                {
                    AllowTrailingCommas = true
                }
            );
            
            MemoryStream memoryStream = new MemoryStream();

            using (
                var utf8JsonWriter = new Utf8JsonWriter(
                    memoryStream,
                    new JsonWriterOptions
                    {
                        Indented = true
                    }
                )
            )
            {
                doc.WriteTo(utf8JsonWriter);
            }
            return new System.Text.UTF8Encoding()
                .GetString(memoryStream.ToArray());
        }
    }

    class PersonBuilder // facade
    {
        // reference not using for struct - this is reaaly important 
        protected Person person = new Person();
        public PersonJobBuilder Works => new PersonJobBuilder(person);
        public PersonAddressBuilder Lives => new PersonAddressBuilder(person);
        public CommonInfoPersonBuilder CommonInfors => new CommonInfoPersonBuilder(person);

        public static implicit operator Person(PersonBuilder pb)
        {
            return pb.person;
        } // like Build method

        internal sealed class CommonInfoPersonBuilder : PersonBuilder
        {
            public CommonInfoPersonBuilder(Person person)
            {
                this.person = person;
            }

            public CommonInfoPersonBuilder WithAge(int age)
            {
                person.Age = age;
                return this;
            }
        }
    }

    // can move it on to inner class of PersonBuilder
    class PersonJobBuilder : PersonBuilder
    {
        // might not work with a value type
        public PersonJobBuilder(Person person)
        {
            this.person = person;
        }

        public PersonJobBuilder At(string companyName)
        {
            person.ComponayName = companyName;
            return this;
        }

        public PersonJobBuilder WorkAsA(string position)
        {
            person.Position = position;
            return this;
        }

        public PersonJobBuilder Earning(int amount)
        {
            person.AnnualIncome = amount;
            return this;
        }        
    }

    class PersonAddressBuilder : PersonBuilder
    {
        public PersonAddressBuilder(Person person)
        {
            this.person = person;
        }

        public PersonAddressBuilder At(string streetAddress)
        {
            person.StreetAddress = streetAddress;
            return this;
        }

        public PersonAddressBuilder withPostalCode(string postalCode)
        {
            person.PostalCode = postalCode;
            return this;
        }

        public PersonAddressBuilder In(string city)
        {
            person.City = city;
            return this;
        }
    }

    internal class Program
    {        
        static void Main(string[] args)
        {
            var person = new PersonBuilder()
                .CommonInfors
                    .WithAge(21)
                .Lives
                    .At("Street Address")
                    .withPostalCode("050822")
                    .In("HCM City")
                .Works
                    .At("MoMo")
                    .WorkAsA("Backend Developer")
                    .Earning(0);

            Console.WriteLine(person);
        }
    }
}