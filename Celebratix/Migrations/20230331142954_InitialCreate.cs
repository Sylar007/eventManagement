using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Celebratix.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(450)", nullable: false),
                    Name = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "varchar", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Businesses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "varchar", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Businesses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(450)", nullable: false),
                    ISO3 = table.Column<string>(type: "varchar", nullable: false),
                    Name = table.Column<string>(type: "varchar", nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    StorageIdentifier = table.Column<Guid>(type: "uuid", nullable: false),
                    PermaLink = table.Column<string>(type: "varchar", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    OriginalFileName = table.Column<string>(type: "varchar", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.StorageIdentifier);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "varchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "varchar", nullable: true),
                    ClaimValue = table.Column<string>(type: "varchar", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(450)", nullable: false),
                    LastLoggedIn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    FirstName = table.Column<string>(type: "varchar", nullable: true),
                    LastName = table.Column<string>(type: "varchar", nullable: true),
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: true),
                    BusinessOwner = table.Column<bool>(type: "boolean", nullable: true),
                    UserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "varchar", nullable: true),
                    SecurityStamp = table.Column<string>(type: "varchar", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "varchar", nullable: true),
                    PhoneNumber = table.Column<string>(type: "varchar", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "varchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "varchar", nullable: true),
                    ClaimValue = table.Column<string>(type: "varchar", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "varchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "varchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "varchar", nullable: true),
                    UserId = table.Column<string>(type: "varchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "varchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "varchar(450)", nullable: false),
                    Name = table.Column<string>(type: "varchar(450)", nullable: false),
                    Value = table.Column<string>(type: "varchar", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "Enabled", "ISO3", "Name" },
                values: new object[,]
                {
                    { "AE", false, "ARE", "United Arab Emirates" },
                    { "AF", false, "AFG", "Afghanistan" },
                    { "AL", false, "ALB", "Albania" },
                    { "AM", false, "ARM", "Armenia" },
                    { "AR", false, "ARG", "Argentina" },
                    { "AT", false, "AUT", "Austria" },
                    { "AU", false, "AUS", "Australia" },
                    { "AZ", false, "AZE", "Azerbaijan" },
                    { "BA", false, "BIH", "Bosnia & Herzegovina" },
                    { "BD", false, "BGD", "Bangladesh" },
                    { "BE", false, "BEL", "Belgium" },
                    { "BG", false, "BGR", "Bulgaria" },
                    { "BH", false, "BHR", "Bahrain" },
                    { "BN", false, "BRN", "Brunei" },
                    { "BO", false, "BOL", "Bolivia" },
                    { "BR", false, "BRA", "Brazil" },
                    { "BT", false, "BTN", "Bhutan" },
                    { "BW", false, "BWA", "Botswana" },
                    { "BY", false, "BLR", "Belarus" },
                    { "BZ", false, "BLZ", "Belize" },
                    { "CA", false, "CAN", "Canada" },
                    { "CD", false, "COD", "Congo (DRC)" },
                    { "CH", false, "CHE", "Switzerland" },
                    { "CI", false, "CIV", "Côte d’Ivoire" },
                    { "CL", false, "CHL", "Chile" },
                    { "CM", false, "CMR", "Cameroon" },
                    { "CN", false, "CHN", "China" },
                    { "CO", false, "COL", "Colombia" },
                    { "CR", false, "CRI", "Costa Rica" },
                    { "CU", false, "CUB", "Cuba" },
                    { "CZ", false, "CZE", "Czechia" },
                    { "DE", false, "DEU", "Germany" },
                    { "DK", false, "DNK", "Denmark" },
                    { "DO", false, "DOM", "Dominican Republic" },
                    { "DZ", false, "DZA", "Algeria" },
                    { "EC", false, "ECU", "Ecuador" },
                    { "EE", false, "EST", "Estonia" },
                    { "EG", false, "EGY", "Egypt" },
                    { "ER", false, "ERI", "Eritrea" },
                    { "ES", false, "ESP", "Spain" },
                    { "ET", false, "ETH", "Ethiopia" },
                    { "FI", false, "FIN", "Finland" },
                    { "FO", false, "FRO", "Faroe Islands" },
                    { "FR", false, "FRA", "France" },
                    { "GB", false, "GBR", "United Kingdom" },
                    { "GE", false, "GEO", "Georgia" },
                    { "GL", false, "GRL", "Greenland" },
                    { "GR", false, "GRC", "Greece" },
                    { "GT", false, "GTM", "Guatemala" },
                    { "HK", false, "HKG", "Hong Kong SAR" },
                    { "HN", false, "HND", "Honduras" },
                    { "HR", false, "HRV", "Croatia" },
                    { "HT", false, "HTI", "Haiti" },
                    { "HU", false, "HUN", "Hungary" },
                    { "ID", false, "IDN", "Indonesia" },
                    { "IE", false, "IRL", "Ireland" },
                    { "IL", false, "ISR", "Israel" },
                    { "IN", false, "IND", "India" },
                    { "IQ", false, "IRQ", "Iraq" },
                    { "IR", false, "IRN", "Iran" },
                    { "IS", false, "ISL", "Iceland" },
                    { "IT", false, "ITA", "Italy" },
                    { "JM", false, "JAM", "Jamaica" },
                    { "JO", false, "JOR", "Jordan" },
                    { "JP", false, "JPN", "Japan" },
                    { "KE", false, "KEN", "Kenya" },
                    { "KG", false, "KGZ", "Kyrgyzstan" },
                    { "KH", false, "KHM", "Cambodia" },
                    { "KR", false, "KOR", "Korea" },
                    { "KW", false, "KWT", "Kuwait" },
                    { "KZ", false, "KAZ", "Kazakhstan" },
                    { "LA", false, "LAO", "Laos" },
                    { "LB", false, "LBN", "Lebanon" },
                    { "LI", false, "LIE", "Liechtenstein" },
                    { "LK", false, "LKA", "Sri Lanka" },
                    { "LT", false, "LTU", "Lithuania" },
                    { "LU", false, "LUX", "Luxembourg" },
                    { "LV", false, "LVA", "Latvia" },
                    { "LY", false, "LBY", "Libya" },
                    { "MA", false, "MAR", "Morocco" },
                    { "MC", false, "MCO", "Monaco" },
                    { "MD", false, "MDA", "Moldova" },
                    { "ME", false, "MNE", "Montenegro" },
                    { "MK", false, "MKD", "North Macedonia" },
                    { "ML", false, "MLI", "Mali" },
                    { "MM", false, "MMR", "Myanmar" },
                    { "MN", false, "MNG", "Mongolia" },
                    { "MT", false, "MLT", "Malta" },
                    { "MV", false, "MDV", "Maldives" },
                    { "MX", false, "MEX", "Mexico" },
                    { "MY", false, "MYS", "Malaysia" },
                    { "NG", false, "NGA", "Nigeria" },
                    { "NI", false, "NIC", "Nicaragua" },
                    { "NL", true, "NLD", "Netherlands" },
                    { "NO", false, "NOR", "Norway" },
                    { "NP", false, "NPL", "Nepal" },
                    { "NZ", false, "NZL", "New Zealand" },
                    { "OM", false, "OMN", "Oman" },
                    { "PA", false, "PAN", "Panama" },
                    { "PE", false, "PER", "Peru" },
                    { "PH", false, "PHL", "Philippines" },
                    { "PK", false, "PAK", "Pakistan" },
                    { "PL", false, "POL", "Poland" },
                    { "PR", false, "PRI", "Puerto Rico" },
                    { "PT", false, "PRT", "Portugal" },
                    { "PY", false, "PRY", "Paraguay" },
                    { "QA", false, "QAT", "Qatar" },
                    { "RE", false, "REU", "Réunion" },
                    { "RO", false, "ROU", "Romania" },
                    { "RS", false, "SRB", "Serbia" },
                    { "RU", false, "RUS", "Russia" },
                    { "RW", false, "RWA", "Rwanda" },
                    { "SA", false, "SAU", "Saudi Arabia" },
                    { "SE", true, "SWE", "Sweden" },
                    { "SG", false, "SGP", "Singapore" },
                    { "SI", false, "SVN", "Slovenia" },
                    { "SK", false, "SVK", "Slovakia" },
                    { "SN", false, "SEN", "Senegal" },
                    { "SO", false, "SOM", "Somalia" },
                    { "SV", false, "SLV", "El Salvador" },
                    { "SY", false, "SYR", "Syria" },
                    { "TH", false, "THA", "Thailand" },
                    { "TM", false, "TKM", "Turkmenistan" },
                    { "TN", false, "TUN", "Tunisia" },
                    { "TR", false, "TUR", "Turkey" },
                    { "TT", false, "TTO", "Trinidad & Tobago" },
                    { "UA", false, "UKR", "Ukraine" },
                    { "US", false, "USA", "United States" },
                    { "UY", false, "URY", "Uruguay" },
                    { "UZ", false, "UZB", "Uzbekistan" },
                    { "VE", false, "VEN", "Venezuela" },
                    { "VN", false, "VNM", "Vietnam" },
                    { "YE", false, "YEM", "Yemen" },
                    { "ZA", false, "ZAF", "South Africa" },
                    { "ZW", false, "ZWE", "Zimbabwe" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "'NormalizedName' IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_BusinessId",
                table: "AspNetUsers",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CreatedAt",
                table: "AspNetUsers",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "'NormalizedUserName' IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Businesses");
        }
    }
}
