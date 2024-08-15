using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Celebratix.Migrations
{
    /// <inheritdoc />
    public partial class AddCountryCallingCodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CallingCode",
                table: "Countries",
                type: "varchar",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "AE",
                column: "CallingCode",
                value: "+971");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "AF",
                column: "CallingCode",
                value: "+93");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "AL",
                column: "CallingCode",
                value: "+355");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "AM",
                column: "CallingCode",
                value: "+374");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "AR",
                column: "CallingCode",
                value: "+54");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "AT",
                column: "CallingCode",
                value: "+43");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "AU",
                column: "CallingCode",
                value: "+61");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "AZ",
                column: "CallingCode",
                value: "+994");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "BA",
                column: "CallingCode",
                value: "+387");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "BD",
                column: "CallingCode",
                value: "+880");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "BE",
                column: "CallingCode",
                value: "+32");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "BG",
                column: "CallingCode",
                value: "+359");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "BH",
                column: "CallingCode",
                value: "+973");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "BN",
                column: "CallingCode",
                value: "+673");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "BO",
                column: "CallingCode",
                value: "+591");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "BR",
                column: "CallingCode",
                value: "+55");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "BT",
                column: "CallingCode",
                value: "+975");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "BW",
                column: "CallingCode",
                value: "+267");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "BY",
                column: "CallingCode",
                value: "+375");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "BZ",
                column: "CallingCode",
                value: "+501");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "CA",
                column: "CallingCode",
                value: "+1");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "CD",
                column: "CallingCode",
                value: "+243");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "CH",
                column: "CallingCode",
                value: "+41");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "CI",
                column: "CallingCode",
                value: "+225");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "CL",
                column: "CallingCode",
                value: "+56");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "CM",
                column: "CallingCode",
                value: "+237");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "CN",
                column: "CallingCode",
                value: "+86");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "CO",
                column: "CallingCode",
                value: "+57");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "CR",
                column: "CallingCode",
                value: "+506");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "CU",
                column: "CallingCode",
                value: "+53");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "CZ",
                column: "CallingCode",
                value: "+420");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "DE",
                column: "CallingCode",
                value: "+49");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "DK",
                column: "CallingCode",
                value: "+45");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "DO",
                column: "CallingCode",
                value: "+1");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "DZ",
                column: "CallingCode",
                value: "+213");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "EC",
                column: "CallingCode",
                value: "+593");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "EE",
                column: "CallingCode",
                value: "+372");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "EG",
                column: "CallingCode",
                value: "+20");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "ER",
                column: "CallingCode",
                value: "+291");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "ES",
                column: "CallingCode",
                value: "+34");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "ET",
                column: "CallingCode",
                value: "+251");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "FI",
                column: "CallingCode",
                value: "+358");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "FO",
                column: "CallingCode",
                value: "+298");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "FR",
                column: "CallingCode",
                value: "+33");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "GB",
                column: "CallingCode",
                value: "+44");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "GE",
                column: "CallingCode",
                value: "+995");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "GL",
                column: "CallingCode",
                value: "+299");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "GR",
                column: "CallingCode",
                value: "+30");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "GT",
                column: "CallingCode",
                value: "+502");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "HK",
                column: "CallingCode",
                value: "+852");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "HN",
                column: "CallingCode",
                value: "+504");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "HR",
                column: "CallingCode",
                value: "+385");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "HT",
                column: "CallingCode",
                value: "+509");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "HU",
                column: "CallingCode",
                value: "+36");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "ID",
                column: "CallingCode",
                value: "+62");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "IE",
                column: "CallingCode",
                value: "+353");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "IL",
                column: "CallingCode",
                value: "+972");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "IN",
                column: "CallingCode",
                value: "+91");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "IQ",
                column: "CallingCode",
                value: "+964");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "IR",
                column: "CallingCode",
                value: "+98");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "IS",
                column: "CallingCode",
                value: "+354");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "IT",
                column: "CallingCode",
                value: "+39");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "JM",
                column: "CallingCode",
                value: "+1");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "JO",
                column: "CallingCode",
                value: "+962");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "JP",
                column: "CallingCode",
                value: "+81");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "KE",
                column: "CallingCode",
                value: "+254");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "KG",
                column: "CallingCode",
                value: "+996");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "KH",
                column: "CallingCode",
                value: "+855");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "KR",
                column: "CallingCode",
                value: "+82");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "KW",
                column: "CallingCode",
                value: "+965");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "KZ",
                column: "CallingCode",
                value: "+7");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "LA",
                column: "CallingCode",
                value: "+856");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "LB",
                column: "CallingCode",
                value: "+961");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "LI",
                column: "CallingCode",
                value: "+423");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "LK",
                column: "CallingCode",
                value: "+94");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "LT",
                column: "CallingCode",
                value: "+370");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "LU",
                column: "CallingCode",
                value: "+352");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "LV",
                column: "CallingCode",
                value: "+371");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "LY",
                column: "CallingCode",
                value: "+218");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "MA",
                column: "CallingCode",
                value: "+212");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "MC",
                column: "CallingCode",
                value: "+377");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "MD",
                column: "CallingCode",
                value: "+373");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "ME",
                column: "CallingCode",
                value: "+382");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "MK",
                column: "CallingCode",
                value: "+389");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "ML",
                column: "CallingCode",
                value: "+223");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "MM",
                column: "CallingCode",
                value: "+95");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "MN",
                column: "CallingCode",
                value: "+976");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "MT",
                column: "CallingCode",
                value: "+356");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "MV",
                column: "CallingCode",
                value: "+960");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "MX",
                column: "CallingCode",
                value: "+52");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "MY",
                column: "CallingCode",
                value: "+60");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "NG",
                column: "CallingCode",
                value: "+234");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "NI",
                column: "CallingCode",
                value: "+505");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "NL",
                column: "CallingCode",
                value: "+31");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "NO",
                column: "CallingCode",
                value: "+47");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "NP",
                column: "CallingCode",
                value: "+977");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "NZ",
                column: "CallingCode",
                value: "+64");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "OM",
                column: "CallingCode",
                value: "+968");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "PA",
                column: "CallingCode",
                value: "+507");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "PE",
                column: "CallingCode",
                value: "+51");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "PH",
                column: "CallingCode",
                value: "+63");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "PK",
                column: "CallingCode",
                value: "+92");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "PL",
                column: "CallingCode",
                value: "+48");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "PR",
                column: "CallingCode",
                value: "+1");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "PT",
                column: "CallingCode",
                value: "+351");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "PY",
                column: "CallingCode",
                value: "+595");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "QA",
                column: "CallingCode",
                value: "+974");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "RE",
                column: "CallingCode",
                value: "+262");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "RO",
                column: "CallingCode",
                value: "+40");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "RS",
                column: "CallingCode",
                value: "+381");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "RU",
                column: "CallingCode",
                value: "+7");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "RW",
                column: "CallingCode",
                value: "+250");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "SA",
                column: "CallingCode",
                value: "+966");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "SE",
                column: "CallingCode",
                value: "+46");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "SG",
                column: "CallingCode",
                value: "+65");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "SI",
                column: "CallingCode",
                value: "+386");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "SK",
                column: "CallingCode",
                value: "+421");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "SN",
                column: "CallingCode",
                value: "+221");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "SO",
                column: "CallingCode",
                value: "+252");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "SV",
                column: "CallingCode",
                value: "+503");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "SY",
                column: "CallingCode",
                value: "+963");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "TH",
                column: "CallingCode",
                value: "+66");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "TM",
                column: "CallingCode",
                value: "+993");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "TN",
                column: "CallingCode",
                value: "+216");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "TR",
                column: "CallingCode",
                value: "+90");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "TT",
                column: "CallingCode",
                value: "+1");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "UA",
                column: "CallingCode",
                value: "+380");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "US",
                column: "CallingCode",
                value: "+1");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "UY",
                column: "CallingCode",
                value: "+598");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "UZ",
                column: "CallingCode",
                value: "+998");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "VE",
                column: "CallingCode",
                value: "+58");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "VN",
                column: "CallingCode",
                value: "+84");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "YE",
                column: "CallingCode",
                value: "+967");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "ZA",
                column: "CallingCode",
                value: "+27");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "ZW",
                column: "CallingCode",
                value: "+263");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CallingCode",
                table: "Countries");
        }
    }
}
