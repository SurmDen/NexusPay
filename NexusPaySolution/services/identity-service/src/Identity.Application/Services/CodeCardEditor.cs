using Identity.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Services
{
    public class CodeCardEditor : ICodeCardEditor
    {
        public string EditCode(string code)
        {
            string formattedCode = string.Join(" ", code.ToCharArray());

            var htmlBuilder = new StringBuilder();

            htmlBuilder.AppendLine($@"<div style=""
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    border-radius: 20px;
    padding: 32px;
    text-align: center;
    box-shadow: 0 15px 35px rgba(0, 0, 0, 0.1);
    border: 1px solid rgba(255, 255, 255, 0.2);
    max-width: 400px;
    margin: 20px auto;
    color: white;
"">
    <!-- Logo -->
    <div style=""margin-bottom: 24px;"">
        <div style=""
            background: rgba(255, 255, 255, 0.2);
            width: 60px;
            height: 60px;
            border-radius: 16px;
            margin: 0 auto;
            display: flex;
            align-items: center;
            justify-content: center;
            backdrop-filter: blur(10px);
        "">
            <svg width=""30"" height=""30"" viewBox=""0 0 24 24"" fill=""none"">
                <path d=""M13 2L3 14H12L11 22L21 10H12L13 2Z"" fill=""white""/>
            </svg>
        </div>
        <h2 style=""margin: 16px 0 8px 0; font-size: 24px; font-weight: 700;"">Nexus Pay</h2>
    </div>

    <!-- Title -->
    <h3 style=""margin: 0 0 16px 0; font-size: 20px; font-weight: 600;"">
        Your Verification Code
    </h3>
    
    <p style=""margin: 0 0 24px 0; opacity: 0.9; line-height: 1.5; font-size: 14px;"">
        Use this code to complete your email verification
    </p>

    <!-- Code Display -->
    <div style=""
        background: rgba(255, 255, 255, 0.15);
        backdrop-filter: blur(10px);
        border-radius: 16px;
        padding: 20px;
        margin-bottom: 24px;
        border: 1px solid rgba(255, 255, 255, 0.3);
    "">
        <div style=""
            font-size: 32px;
            font-weight: 700;
            letter-spacing: 8px;
            color: white;
            text-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
        "">
            {formattedCode}
        </div>
    </div>

    <!-- Expiry -->
    <div style=""
        background: rgba(255, 255, 255, 0.1);
        border-radius: 12px;
        padding: 12px;
        display: inline-block;
    "">
        <p style=""margin: 0; font-size: 13px; opacity: 0.9;"">
            ⏰ Expires in: <strong>15 minutes</strong>
        </p>
    </div>

    <!-- Footer -->
    <div style=""margin-top: 24px; padding-top: 20px; border-top: 1px solid rgba(255, 255, 255, 0.2);"">
        <p style=""margin: 0; font-size: 12px; opacity: 0.8;"">
            If you didn't request this code, please ignore this email.
        </p>
    </div>
</div>");

            return htmlBuilder.ToString();
        }
    }
}
