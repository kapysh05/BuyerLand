using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuyersMarket.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameTablesToPascalCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_buyer_profiles_users_UserId",
                table: "buyer_profiles");

            migrationBuilder.DropForeignKey(
                name: "FK_messages_conversations_ConversationId",
                table: "messages");

            migrationBuilder.DropForeignKey(
                name: "FK_offers_tenders_TenderId",
                table: "offers");

            migrationBuilder.DropForeignKey(
                name: "FK_refresh_tokens_users_UserId",
                table: "refresh_tokens");

            migrationBuilder.DropForeignKey(
                name: "FK_tender_images_tenders_TenderId",
                table: "tender_images");

            migrationBuilder.DropPrimaryKey(
                name: "PK_users",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tenders",
                table: "tenders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_offers",
                table: "offers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_messages",
                table: "messages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_deals",
                table: "deals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_conversations",
                table: "conversations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tender_images",
                table: "tender_images");

            migrationBuilder.DropPrimaryKey(
                name: "PK_refresh_tokens",
                table: "refresh_tokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_buyer_profiles",
                table: "buyer_profiles");

            migrationBuilder.RenameTable(
                name: "users",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "tenders",
                newName: "Tenders");

            migrationBuilder.RenameTable(
                name: "offers",
                newName: "Offers");

            migrationBuilder.RenameTable(
                name: "messages",
                newName: "Messages");

            migrationBuilder.RenameTable(
                name: "deals",
                newName: "Deals");

            migrationBuilder.RenameTable(
                name: "conversations",
                newName: "Conversations");

            migrationBuilder.RenameTable(
                name: "tender_images",
                newName: "TenderImages");

            migrationBuilder.RenameTable(
                name: "refresh_tokens",
                newName: "RefreshTokens");

            migrationBuilder.RenameTable(
                name: "buyer_profiles",
                newName: "BuyerProfiles");

            migrationBuilder.RenameIndex(
                name: "IX_users_Email",
                table: "Users",
                newName: "IX_Users_Email");

            migrationBuilder.RenameIndex(
                name: "IX_tenders_CustomerId",
                table: "Tenders",
                newName: "IX_Tenders_CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_offers_TenderId",
                table: "Offers",
                newName: "IX_Offers_TenderId");

            migrationBuilder.RenameIndex(
                name: "IX_offers_BuyerId",
                table: "Offers",
                newName: "IX_Offers_BuyerId");

            migrationBuilder.RenameIndex(
                name: "IX_messages_ConversationId",
                table: "Messages",
                newName: "IX_Messages_ConversationId");

            migrationBuilder.RenameIndex(
                name: "IX_deals_TenderId",
                table: "Deals",
                newName: "IX_Deals_TenderId");

            migrationBuilder.RenameIndex(
                name: "IX_deals_OfferId",
                table: "Deals",
                newName: "IX_Deals_OfferId");

            migrationBuilder.RenameIndex(
                name: "IX_deals_CustomerId",
                table: "Deals",
                newName: "IX_Deals_CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_deals_BuyerId",
                table: "Deals",
                newName: "IX_Deals_BuyerId");

            migrationBuilder.RenameIndex(
                name: "IX_conversations_DealId",
                table: "Conversations",
                newName: "IX_Conversations_DealId");

            migrationBuilder.RenameIndex(
                name: "IX_conversations_CustomerId",
                table: "Conversations",
                newName: "IX_Conversations_CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_conversations_BuyerId",
                table: "Conversations",
                newName: "IX_Conversations_BuyerId");

            migrationBuilder.RenameIndex(
                name: "IX_tender_images_TenderId",
                table: "TenderImages",
                newName: "IX_TenderImages_TenderId");

            migrationBuilder.RenameIndex(
                name: "IX_refresh_tokens_UserId",
                table: "RefreshTokens",
                newName: "IX_RefreshTokens_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_refresh_tokens_Token",
                table: "RefreshTokens",
                newName: "IX_RefreshTokens_Token");

            migrationBuilder.RenameIndex(
                name: "IX_buyer_profiles_UserId",
                table: "BuyerProfiles",
                newName: "IX_BuyerProfiles_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tenders",
                table: "Tenders",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Offers",
                table: "Offers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Messages",
                table: "Messages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Deals",
                table: "Deals",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Conversations",
                table: "Conversations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TenderImages",
                table: "TenderImages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RefreshTokens",
                table: "RefreshTokens",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BuyerProfiles",
                table: "BuyerProfiles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BuyerProfiles_Users_UserId",
                table: "BuyerProfiles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Conversations_ConversationId",
                table: "Messages",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_Tenders_TenderId",
                table: "Offers",
                column: "TenderId",
                principalTable: "Tenders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_Users_UserId",
                table: "RefreshTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TenderImages_Tenders_TenderId",
                table: "TenderImages",
                column: "TenderId",
                principalTable: "Tenders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BuyerProfiles_Users_UserId",
                table: "BuyerProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Conversations_ConversationId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Offers_Tenders_TenderId",
                table: "Offers");

            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_Users_UserId",
                table: "RefreshTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_TenderImages_Tenders_TenderId",
                table: "TenderImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tenders",
                table: "Tenders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Offers",
                table: "Offers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Messages",
                table: "Messages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Deals",
                table: "Deals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Conversations",
                table: "Conversations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TenderImages",
                table: "TenderImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RefreshTokens",
                table: "RefreshTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BuyerProfiles",
                table: "BuyerProfiles");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "users");

            migrationBuilder.RenameTable(
                name: "Tenders",
                newName: "tenders");

            migrationBuilder.RenameTable(
                name: "Offers",
                newName: "offers");

            migrationBuilder.RenameTable(
                name: "Messages",
                newName: "messages");

            migrationBuilder.RenameTable(
                name: "Deals",
                newName: "deals");

            migrationBuilder.RenameTable(
                name: "Conversations",
                newName: "conversations");

            migrationBuilder.RenameTable(
                name: "TenderImages",
                newName: "tender_images");

            migrationBuilder.RenameTable(
                name: "RefreshTokens",
                newName: "refresh_tokens");

            migrationBuilder.RenameTable(
                name: "BuyerProfiles",
                newName: "buyer_profiles");

            migrationBuilder.RenameIndex(
                name: "IX_Users_Email",
                table: "users",
                newName: "IX_users_Email");

            migrationBuilder.RenameIndex(
                name: "IX_Tenders_CustomerId",
                table: "tenders",
                newName: "IX_tenders_CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_Offers_TenderId",
                table: "offers",
                newName: "IX_offers_TenderId");

            migrationBuilder.RenameIndex(
                name: "IX_Offers_BuyerId",
                table: "offers",
                newName: "IX_offers_BuyerId");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_ConversationId",
                table: "messages",
                newName: "IX_messages_ConversationId");

            migrationBuilder.RenameIndex(
                name: "IX_Deals_TenderId",
                table: "deals",
                newName: "IX_deals_TenderId");

            migrationBuilder.RenameIndex(
                name: "IX_Deals_OfferId",
                table: "deals",
                newName: "IX_deals_OfferId");

            migrationBuilder.RenameIndex(
                name: "IX_Deals_CustomerId",
                table: "deals",
                newName: "IX_deals_CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_Deals_BuyerId",
                table: "deals",
                newName: "IX_deals_BuyerId");

            migrationBuilder.RenameIndex(
                name: "IX_Conversations_DealId",
                table: "conversations",
                newName: "IX_conversations_DealId");

            migrationBuilder.RenameIndex(
                name: "IX_Conversations_CustomerId",
                table: "conversations",
                newName: "IX_conversations_CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_Conversations_BuyerId",
                table: "conversations",
                newName: "IX_conversations_BuyerId");

            migrationBuilder.RenameIndex(
                name: "IX_TenderImages_TenderId",
                table: "tender_images",
                newName: "IX_tender_images_TenderId");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshTokens_UserId",
                table: "refresh_tokens",
                newName: "IX_refresh_tokens_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshTokens_Token",
                table: "refresh_tokens",
                newName: "IX_refresh_tokens_Token");

            migrationBuilder.RenameIndex(
                name: "IX_BuyerProfiles_UserId",
                table: "buyer_profiles",
                newName: "IX_buyer_profiles_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_users",
                table: "users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tenders",
                table: "tenders",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_offers",
                table: "offers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_messages",
                table: "messages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_deals",
                table: "deals",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_conversations",
                table: "conversations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tender_images",
                table: "tender_images",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_refresh_tokens",
                table: "refresh_tokens",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_buyer_profiles",
                table: "buyer_profiles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_buyer_profiles_users_UserId",
                table: "buyer_profiles",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_messages_conversations_ConversationId",
                table: "messages",
                column: "ConversationId",
                principalTable: "conversations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_offers_tenders_TenderId",
                table: "offers",
                column: "TenderId",
                principalTable: "tenders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_refresh_tokens_users_UserId",
                table: "refresh_tokens",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tender_images_tenders_TenderId",
                table: "tender_images",
                column: "TenderId",
                principalTable: "tenders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
