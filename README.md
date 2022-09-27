# Bookkeeper

# Scenario

We've built an API that lets companies manage their invoices. They can create, update, view and delete invoices, and even report on outstanding balances.
Since this API serves financial data, we need to ensure it's secure and follows secure coding and security best practices.

Your job is to make sure it's secure enough to deploy to production.

## Technical details:
- The system is multi-tenanted, a user belongs to a tenant and should only be able to see/modify/etc data for their own tenant.
- The system uses basic authentication. Test user is Bob (Tenant 1) and Alice (Tenant 2), both with password P@55w0rd.
	- Bob Auth header: "Authorization Basic Qm9iOlBANTV3MHJk"
	- Alice Auth header: "Authorization Basic QWxpY2U6UEA1NXcwcmQ="
- Swagger is available at https://localhost:5001/swagger