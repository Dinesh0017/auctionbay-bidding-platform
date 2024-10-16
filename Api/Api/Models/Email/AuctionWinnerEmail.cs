﻿using Api.Dtos;

namespace Api.Models.Email
{
    public class AuctionWinnerEmail : EmailBuilder
    {
        private Email email;
        private string auctionName;
        private decimal winningBidAmount;
        private string winnerName;
        private string winnerEmail;
        private string claimLink;

        public AuctionWinnerEmail(string auctionName, decimal winningBidAmount, string winnerName, string winnerEmail, string claimLink)
        {
            this.auctionName = auctionName;
            this.winningBidAmount = winningBidAmount/100;
            this.winnerName = winnerName;
            this.winnerEmail = winnerEmail;
            this.claimLink = claimLink;
            this.email = new Email();
        }

        public Email Build()
        {
            return email;
        }

        public void BuildBody()
        {
            email.body = $@"
                <div style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
                    <div style='max-width: 600px; margin: 0 auto; background-color: #ffffff; padding: 20px; border-radius: 8px; box-shadow: 0px 4px 12px rgba(0, 0, 0, 0.1);'>
                        <h2 style='color: #333333; text-align: center;'>Congratulations {winnerName}!</h2>
                        <p style='color: #555555; font-size: 16px; line-height: 1.6;'>
                            We are excited to inform you that you have won the auction for <strong>{auctionName}</strong> with a winning bid of <strong>${winningBidAmount}</strong>!
                        </p>
                        <p style='color: #555555; font-size: 16px; line-height: 1.6;'>
                            To claim your NFT, you must complete the payment process. Please follow the link below to claim your auction item:
                        </p>
                        <div style='text-align: center; margin: 20px 0;'>
                            <a href='{claimLink}' style='background-color: #28a745; color: white; padding: 10px 20px; text-decoration: none; font-size: 16px; border-radius: 5px;'>Claim Your NFT</a>
                        </div>
                        <p style='color: #555555; font-size: 16px; line-height: 1.6;'>
                            Please note that if you do not complete the payment and claim process within the given time, you will forfeit your rights to this NFT.
                        </p>
                        <p style='color: #555555; font-size: 16px; line-height: 1.6;'>
                            Best Regards,<br>
                            <span style='color: #333333; font-weight: bold;'>The NFTFY Team</span>
                        </p>
                    </div>
                    <div style='max-width: 600px; margin: 0 auto; text-align: center; padding: 10px; font-size: 12px; color: #999999;'>
                        <p>
                            Please do not reply to this email. This is an automated message from NFTFY.<br>
                            If you have any questions, please contact our support team at <a href='mailto:support@nftfy.com' style='color: #007BFF;'>support@nftfy.com</a>.
                        </p>
                        <p>
                            © {DateTime.UtcNow.Year} NFTFY, All rights reserved.
                        </p>
                    </div>
                </div>";
        }

        public void BuildSubject()
        {
            email.subject = $"Congratulations {winnerName} – You Won the Auction for {auctionName}!";
        }

        public void BuildTo()
        {
            email.to = winnerEmail;
        }
    }
}