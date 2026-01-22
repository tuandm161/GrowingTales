using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Subject = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Message = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    AdminNote = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Response = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: true),
                    AssignedTo = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RespondedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ClosedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IpAddress = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Key = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Group = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, defaultValue: "General"),
                    IsPublic = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsEditable = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    PhoneNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    AvatarUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Role = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastLoginAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastLoginIp = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    CurrentPlan = table.Column<int>(type: "INTEGER", nullable: false),
                    StoriesCreatedTotal = table.Column<int>(type: "INTEGER", nullable: false),
                    StoriesCreatedThisMonth = table.Column<int>(type: "INTEGER", nullable: false),
                    LastStoryCreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    MonthResetDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    EmailConfirmToken = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    PasswordResetToken = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    PasswordResetExpiry = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ActivityLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Action = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    EntityType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    EntityId = table.Column<Guid>(type: "TEXT", nullable: true),
                    EntityName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    OldValues = table.Column<string>(type: "TEXT", nullable: true),
                    NewValues = table.Column<string>(type: "TEXT", nullable: true),
                    AdditionalData = table.Column<string>(type: "TEXT", nullable: true),
                    IpAddress = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Referer = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    RequestPath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    RequestMethod = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    IsSuccess = table.Column<bool>(type: "INTEGER", nullable: false),
                    ErrorMessage = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Link = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    EntityType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    EntityId = table.Column<Guid>(type: "TEXT", nullable: true),
                    IsRead = table.Column<bool>(type: "INTEGER", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ImageUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Stories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ChildName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ChildAge = table.Column<int>(type: "INTEGER", nullable: false),
                    Theme = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    CoverImageUrl = table.Column<string>(type: "TEXT", maxLength: 5000000, nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    IsFavorite = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShareToken = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsPublic = table.Column<bool>(type: "INTEGER", nullable: false),
                    ViewCount = table.Column<int>(type: "INTEGER", nullable: false),
                    ShareCount = table.Column<int>(type: "INTEGER", nullable: false),
                    DownloadCount = table.Column<int>(type: "INTEGER", nullable: false),
                    Language = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false, defaultValue: "vi"),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stories_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Plan = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TransactionId = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    AmountPaid = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    PaymentMethod = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryPages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PageNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    ImagePrompt = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", maxLength: 5000000, nullable: false),
                    StoryId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryPages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryPages_Stories_StoryId",
                        column: x => x.StoryId,
                        principalTable: "Stories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SubscriptionId = table.Column<Guid>(type: "TEXT", nullable: true),
                    TransactionId = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    OrderId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Method = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Discount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    FinalAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false, defaultValue: "VND"),
                    PlanName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    DurationMonths = table.Column<int>(type: "INTEGER", nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Note = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    BankCode = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    CardType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    PayerInfo = table.Column<string>(type: "TEXT", nullable: true),
                    GatewayResponse = table.Column<string>(type: "TEXT", nullable: true),
                    ResponseCode = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    ResponseMessage = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ExpiredAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RefundedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IpAddress = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Subscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "Subscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Payments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "SystemSettings",
                columns: new[] { "Id", "CreatedAt", "Description", "Group", "IsEditable", "IsPublic", "Key", "Type", "UpdatedAt", "UpdatedBy", "Value" },
                values: new object[,]
                {
                    { new Guid("00000001-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 4, 0, 38, 862, DateTimeKind.Utc).AddTicks(4427), "Tên website", "General", true, true, "site_name", 0, null, null, "GrowingTales" },
                    { new Guid("00000001-0000-0000-0000-000000000002"), new DateTime(2026, 1, 20, 4, 0, 38, 862, DateTimeKind.Utc).AddTicks(5929), "Mô tả website", "General", true, true, "site_description", 0, null, null, "Tạo truyện cho bé yêu với AI" },
                    { new Guid("00000001-0000-0000-0000-000000000003"), new DateTime(2026, 1, 20, 4, 0, 38, 862, DateTimeKind.Utc).AddTicks(5935), "Email liên hệ", "Contact", true, true, "contact_email", 0, null, null, "support@growingtales.vn" },
                    { new Guid("00000001-0000-0000-0000-000000000004"), new DateTime(2026, 1, 20, 4, 0, 38, 862, DateTimeKind.Utc).AddTicks(5937), "Số điện thoại liên hệ", "Contact", true, true, "contact_phone", 0, null, null, "0123 456 789" },
                    { new Guid("00000001-0000-0000-0000-000000000010"), new DateTime(2026, 1, 20, 4, 0, 38, 862, DateTimeKind.Utc).AddTicks(5940), "Giá gói Premium (VND)", "Pricing", true, false, "premium_price", 1, null, null, "150000" },
                    { new Guid("00000001-0000-0000-0000-000000000011"), new DateTime(2026, 1, 20, 4, 0, 38, 862, DateTimeKind.Utc).AddTicks(5942), "Số truyện miễn phí", "Pricing", true, false, "free_stories_limit", 1, null, null, "3" },
                    { new Guid("00000001-0000-0000-0000-000000000012"), new DateTime(2026, 1, 20, 4, 0, 38, 862, DateTimeKind.Utc).AddTicks(5944), "Số trang tối đa cho gói Free", "Pricing", true, false, "free_pages_per_story", 1, null, null, "5" },
                    { new Guid("00000001-0000-0000-0000-000000000013"), new DateTime(2026, 1, 20, 4, 0, 38, 862, DateTimeKind.Utc).AddTicks(5968), "Số trang tối đa cho gói Premium", "Pricing", true, false, "premium_pages_per_story", 1, null, null, "15" },
                    { new Guid("00000001-0000-0000-0000-000000000020"), new DateTime(2026, 1, 20, 4, 0, 38, 862, DateTimeKind.Utc).AddTicks(5974), "Cho phép đăng ký tài khoản mới", "Features", true, false, "enable_registration", 2, null, null, "true" },
                    { new Guid("00000001-0000-0000-0000-000000000021"), new DateTime(2026, 1, 20, 4, 0, 38, 862, DateTimeKind.Utc).AddTicks(5976), "Cho phép khách tạo truyện không cần đăng ký", "Features", true, false, "enable_guest_stories", 2, null, null, "true" },
                    { new Guid("00000001-0000-0000-0000-000000000022"), new DateTime(2026, 1, 20, 4, 0, 38, 862, DateTimeKind.Utc).AddTicks(5977), "Chế độ bảo trì", "System", true, false, "maintenance_mode", 2, null, null, "false" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLogs_Action",
                table: "ActivityLogs",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLogs_CreatedAt",
                table: "ActivityLogs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLogs_EntityType",
                table: "ActivityLogs",
                column: "EntityType");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLogs_IsSuccess",
                table: "ActivityLogs",
                column: "IsSuccess");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLogs_Type",
                table: "ActivityLogs",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLogs_UserId",
                table: "ActivityLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_CreatedAt",
                table: "Contacts",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_Email",
                table: "Contacts",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_Status",
                table: "Contacts",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_Type",
                table: "Contacts",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_UserId",
                table: "Contacts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CreatedAt",
                table: "Notifications",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_IsRead",
                table: "Notifications",
                column: "IsRead");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Type",
                table: "Notifications",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CreatedAt",
                table: "Payments",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_Method",
                table: "Payments",
                column: "Method");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrderId",
                table: "Payments",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaidAt",
                table: "Payments",
                column: "PaidAt");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_Status",
                table: "Payments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_SubscriptionId",
                table: "Payments",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_TransactionId",
                table: "Payments",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_UserId",
                table: "Payments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Stories_ChildName",
                table: "Stories",
                column: "ChildName");

            migrationBuilder.CreateIndex(
                name: "IX_Stories_CreatedAt",
                table: "Stories",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Stories_ShareToken",
                table: "Stories",
                column: "ShareToken",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stories_Status",
                table: "Stories",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Stories_Theme",
                table: "Stories",
                column: "Theme");

            migrationBuilder.CreateIndex(
                name: "IX_Stories_UserId",
                table: "Stories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Stories_ViewCount",
                table: "Stories",
                column: "ViewCount");

            migrationBuilder.CreateIndex(
                name: "IX_StoryPages_StoryId_PageNumber",
                table: "StoryPages",
                columns: new[] { "StoryId", "PageNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_EndDate",
                table: "Subscriptions",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_StartDate",
                table: "Subscriptions",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_Status",
                table: "Subscriptions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_TransactionId",
                table: "Subscriptions",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_UserId",
                table: "Subscriptions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemSettings_Group",
                table: "SystemSettings",
                column: "Group");

            migrationBuilder.CreateIndex(
                name: "IX_SystemSettings_Key",
                table: "SystemSettings",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatedAt",
                table: "Users",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_PhoneNumber",
                table: "Users",
                column: "PhoneNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Role",
                table: "Users",
                column: "Role");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Status",
                table: "Users",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityLogs");

            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "StoryPages");

            migrationBuilder.DropTable(
                name: "SystemSettings");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "Stories");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
