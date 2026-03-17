# Simple E-Commerce MVP (ASP.NET Core MVC)

This project is a beginner-friendly MVP e-commerce app using:

- ASP.NET Core MVC
- Entity Framework Core (Code First)
- SQL Server (LocalDB)
- Razor Views + Bootstrap
- Session-based cart

## Features

- Home page product listing
- Product details page
- Cart: add, remove, update quantity, total price
- Checkout form with order summary
- Admin panel with hardcoded login
- Admin CRUD for products

## Admin Login

- Username: admin
- Password: admin123

## Run the project

1. Ensure SQL Server LocalDB is available.
1. Update connection string in appsettings.json if needed.
1. Run:

```bash
dotnet restore
dotnet ef database update
dotnet run
```

1. Open the URL shown in terminal.

## Payment Integration (Optional)

### Option A: PayPal Checkout (Sandbox)

1. Create sandbox accounts:
Go to <https://developer.paypal.com/>.
Create one Business sandbox account and one Personal sandbox account.

1. Create an app in PayPal Developer Dashboard:
Copy Client ID and Secret.

1. Add settings in appsettings.json:

```json
"PayPal": {
  "ClientId": "YOUR_SANDBOX_CLIENT_ID",
  "Secret": "YOUR_SANDBOX_SECRET",
  "BaseUrl": "https://api-m.sandbox.paypal.com"
}
```

1. Add a new action (example in OrderController) to create a PayPal order.
Send cart total to PayPal API.
Return PayPal approval link.

1. Success/Cancel URLs:
Success URL example: /Order/PaymentSuccess.
Cancel URL example: /Order/Checkout.
In success action, capture the PayPal order and then save your local order.

1. Frontend:
Add PayPal JS SDK script with client id.
Render PayPal button on checkout page.

### Option B: Stripe Checkout (Sandbox)

1. Create Stripe account: <https://dashboard.stripe.com/register>
1. Use test keys from Developers > API keys.
1. Add settings in appsettings.json:

```json
"Stripe": {
  "SecretKey": "sk_test_...",
  "PublishableKey": "pk_test_..."
}
```

1. Install package:

```bash
dotnet add package Stripe.net
```

1. In checkout action:
Create Stripe Checkout Session with line items from cart.
Set success URL: /Order/PaymentSuccess?session_id={CHECKOUT_SESSION_ID}.
Set cancel URL: /Order/Checkout.

1. Redirect user to Stripe Checkout URL and complete order on success callback.

## Notes

- This is intentionally simple (MVP) and not production-ready.
- Admin authentication is hardcoded for learning/demo purposes only.
