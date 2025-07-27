# Techperts Solutions API Documentation

## Overview
Techperts Solutions is a comprehensive e-commerce platform for computer hardware and services, supporting four user types: **Customer**, **TechCompany**, **Admin**, and **DeliveryPerson**. The platform provides three core services: **PC Build**, **Maintenance**, and **Delivery**.

## Base URL
- **Development**: `https://localhost:7230` or `https://localhost:5155`
- **Swagger UI**: `https://localhost:7230/swagger`

## Authentication
All endpoints (except authentication) require JWT Bearer token in the Authorization header:
```
Authorization: Bearer {your-jwt-token}
```

## User Types & Permissions

### 1. Customer 👤
- Browse and purchase products
- Manage shopping cart and wishlist
- Request PC assembly services
- Request maintenance services
- Track orders and deliveries
- Manage profile and addresses

### 2. TechCompany 🏢
- Manage product catalog
- Handle maintenance requests
- Process PC assembly orders
- Update order statuses
- Manage company profile

### 3. Admin 👑
- Full system access
- Manage all users and roles
- Oversee all operations
- System configuration
- Analytics and reporting

### 4. DeliveryPerson 🚚
- View assigned deliveries
- Update delivery status
- Manage delivery routes
- Update delivery completion

---

## 🔐 Authentication Endpoints

### Register User
```http
POST /api/Authentication/register
Content-Type: application/x-www-form-urlencoded

fullName: string (required, 2-100 chars, letters and spaces only)
userName: string (required, 3-50 chars, alphanumeric and underscore only)
email: string (required, valid email format, max 100 chars)
password: string (required, 6-100 chars, must contain lowercase, uppercase, and number)
confirmPassword: string (required, must match password)
address: string (required, 10-200 chars)
phoneNumber: string (required, valid phone format, max 20 chars)
role: string (required) - "Customer" | "Admin" | "TechCompany" | "DeliveryPerson"
```

**Validation Rules:**
- **Full Name**: 2-100 characters, letters and spaces only
- **Username**: 3-50 characters, alphanumeric and underscore only
- **Email**: Valid email format, maximum 100 characters
- **Password**: 6-100 characters, must contain at least one lowercase letter, one uppercase letter, and one number
- **Confirm Password**: Must match the password exactly
- **Address**: 10-200 characters
- **Phone Number**: Valid phone number format, maximum 20 characters
- **Role**: Must be a valid role from the predefined list

### Login
```http
POST /api/Authentication/login
Content-Type: application/x-www-form-urlencoded

email: string (required, valid email format, max 100 chars)
password: string (required, 6-100 chars)
rememberMe: boolean (optional, default: false)
```

**Validation Rules:**
- **Email**: Valid email format, maximum 100 characters
- **Password**: 6-100 characters
- **Remember Me**: Optional boolean flag

### Forgot Password
```http
POST /api/Authentication/forgot-password
Content-Type: application/json

{
  "email": "string (required, valid email format, max 100 chars)"
}
```

**Validation Rules:**
- **Email**: Valid email format, maximum 100 characters

### Reset Password
```http
POST /api/Authentication/reset-password
Content-Type: application/json

{
  "email": "string (required, valid email format, max 100 chars)",
  "token": "string (required, max 500 chars)",
  "newPassword": "string (required, 6-100 chars, must contain lowercase, uppercase, and number)"
}
```

**Validation Rules:**
- **Email**: Valid email format, maximum 100 characters
- **Token**: Required reset token, maximum 500 characters
- **New Password**: 6-100 characters, must contain at least one lowercase letter, one uppercase letter, and one number

### Delete Account
```http
DELETE /api/Authentication/delete-account
Authorization: Bearer {token}
Content-Type: application/json

{
  "password": "string (required, 6-100 chars)"
}
```

**Validation Rules:**
- **Password**: 6-100 characters (required to confirm account deletion)

---

## 🛍️ Product Management

### Get All Products
```http
GET /api/Product/all?pageNumber=1&pageSize=10&status=Pending&categoryId={id}&subCategoryId={id}&search={term}&sortBy=name&sortDesc=false
```

### Get Product by ID
```http
GET /api/Product/{id}
```

### Get Pending Products
```http
GET /api/Product/pending?pageNumber=1&pageSize=10&sortBy=name&sortDesc=false
```

### Get Products by Status
```http
GET /api/Product/status/{status}
```

### Get Products by Category
```http
GET /api/Product/category/{categoryId}?pageNumber=1&pageSize=10&sortBy=name&sortDesc=false
```

### Add Product (TechCompany/Admin)
```http
POST /api/Product
Content-Type: multipart/form-data

name: string (required)
description: string (required)
price: decimal (required)
discountPrice: decimal (optional)
stock: int (required)
categoryId: string (required)
subCategoryId: string (optional)
techCompanyId: string (required)
status: string (optional) - "Pending" | "Approved" | "Rejected"
img: file (optional)
```

### Update Product (TechCompany/Admin)
```http
PUT /api/Product/{id}
Content-Type: multipart/form-data

name: string (required)
description: string (required)
price: decimal (required)
discountPrice: decimal (optional)
stock: int (required)
categoryId: string (required)
subCategoryId: string (optional)
techCompanyId: string (required)
status: string (optional)
img: file (optional)
```

### Delete Product (Admin)
```http
DELETE /api/Product/{id}
```

---

## 🛒 Shopping Cart

### Get Customer Cart
```http
GET /api/Cart/{customerId}
```

### Add Item to Cart
```http
POST /api/Cart/{customerId}/items
Content-Type: application/x-www-form-urlencoded

productId: string (required)
quantity: int (required)
```

### Update Cart Item Quantity
```http
PUT /api/Cart/{customerId}/items
Content-Type: application/x-www-form-urlencoded

productId: string (required)
quantity: int (required)
```

### Remove Item from Cart
```http
DELETE /api/Cart/{customerId}/items/{productId}
```

### Clear Cart
```http
DELETE /api/Cart/{customerId}/clear
```

### Checkout Cart
```http
POST /api/Cart/{customerId}/checkout
```

### Advanced Checkout
```http
POST /api/Cart/checkout
Content-Type: application/json

{
  "customerId": "string",
  "shippingAddress": "string",
  "paymentMethod": "string",
  "items": [
    {
      "productId": "string",
      "quantity": 0
    }
  ]
}
```

### Partial Checkout
```http
POST /api/Cart/{customerId}/partial-checkout
Content-Type: application/json

{
  "items": [
    {
      "productId": "string",
      "quantity": 0
    }
  ]
}
```

---

## 📋 Orders

### Get Order by ID
```http
GET /api/Orders/{id}
```

### Get All Orders (Admin)
```http
GET /api/Orders
```

### Get Orders by Customer
```http
GET /api/Orders/by-customer/{customerId}
```

### Get Order History by Customer
```http
GET /api/Orders/customer/{customerId}/history
```

---

## 🏷️ Categories

### Get All Categories
```http
GET /api/Category/All
```

### Get Category by ID
```http
GET /api/Category/GetCategory/{id}
```

### Create Category (Admin)
```http
POST /api/Category/Create
Content-Type: application/x-www-form-urlencoded

name: string (required)
description: string (optional)
image: string (optional)
```

### Update Category (Admin)
```http
PUT /api/Category/Update/{id}
Content-Type: application/x-www-form-urlencoded

id: string (required)
name: string (required)
description: string (optional)
image: string (optional)
```

### Delete Category (Admin)
```http
DELETE /api/Category/Delete/{id}
```

---

## 📂 SubCategories

### Get All SubCategories
```http
GET /api/SubCategory
```

### Get SubCategory by ID
```http
GET /api/SubCategory/{id}
```

### Get SubCategories by Category
```http
GET /api/SubCategory/byCategory/{categoryId}
```

### Create SubCategory (Admin)
```http
POST /api/SubCategory
Content-Type: application/x-www-form-urlencoded

name: string (required)
description: string (optional)
image: string (optional)
categoryId: string (required)
```

### Update SubCategory (Admin)
```http
PUT /api/SubCategory/{id}
Content-Type: application/x-www-form-urlencoded

id: string (required)
name: string (required)
description: string (optional)
image: string (optional)
categoryId: string (required)
```

### Delete SubCategory (Admin)
```http
DELETE /api/SubCategory/{id}
```

---

## 💝 WishList

### Create WishList
```http
POST /api/WishList
Content-Type: application/json

{
  "customerId": "string"
}
```

### Get WishList by ID
```http
GET /api/WishList/{id}
```

### Get Customer WishList
```http
GET /api/WishList/customer/{customerId}
```

### Add Item to WishList
```http
POST /api/WishList/{wishListId}/items
Content-Type: application/json

{
  "productId": "string"
}
```

### Remove Item from WishList
```http
DELETE /api/WishList/{wishListId}/items/{itemId}
```

### Move All Items to Cart
```http
POST /api/WishList/{wishListId}/move-to-cart?customerId={customerId}
```

### Move Selected Items to Cart
```http
POST /api/WishList/{customerId}/move-selected-to-cart
Content-Type: application/json

[
  "wishListItemId1",
  "wishListItemId2"
]
```

### Move Single Item to Cart
```http
POST /api/WishList/{customerId}/move-item-to-cart/{wishListItemId}
```

---

## 🔧 PC Assembly Service

### Create PC Assembly Request
```http
POST /api/PCAssembly
Content-Type: application/json

{
  "customerId": "string",
  "description": "string",
  "budget": 0,
  "items": [
    {
      "productId": "string",
      "quantity": 0
    }
  ]
}
```

### Get PC Assembly by ID
```http
GET /api/PCAssembly/{id}
```

### Get All PC Assemblies (Admin/TechCompany)
```http
GET /api/PCAssembly
```

### Get PC Assemblies by Customer
```http
GET /api/PCAssembly/customer/{customerId}
```

### Update PC Assembly (TechCompany)
```http
PUT /api/PCAssembly/{id}
Content-Type: application/json

{
  "customerId": "string",
  "description": "string",
  "budget": 0,
  "status": "string",
  "items": [
    {
      "productId": "string",
      "quantity": 0
    }
  ]
}
```

---

## 🛠️ Maintenance Service

### Get All Maintenance Requests
```http
GET /api/Maintenance
```

### Get Maintenance Request by ID
```http
GET /api/Maintenance/{id}
```

### Create Maintenance Request
```http
POST /api/Maintenance
Content-Type: application/json

{
  "customerId": "string",
  "techCompanyId": "string",
  "description": "string",
  "deviceType": "string",
  "issue": "string",
  "priority": "string"
}
```

### Update Maintenance Request
```http
PUT /api/Maintenance/{id}
Content-Type: application/json

{
  "customerId": "string",
  "techCompanyId": "string",
  "description": "string",
  "deviceType": "string",
  "issue": "string",
  "priority": "string",
  "status": "string",
  "notes": "string",
  "completedDate": "datetime"
}
```

### Delete Maintenance Request
```http
DELETE /api/Maintenance/{id}
```

---

## 🚚 Delivery Service

### Get All Deliveries
```http
GET /api/Delivery
```

### Get Delivery by ID
```http
GET /api/Delivery/{id}
```

### Get Delivery Details
```http
GET /api/Delivery/details/{id}
```

### Create Delivery
```http
POST /api/Delivery
Content-Type: application/json

{
  "orderId": "string",
  "deliveryPersonId": "string",
  "pickupAddress": "string",
  "deliveryAddress": "string",
  "estimatedDeliveryDate": "datetime",
  "deliveryFee": 0
}
```

### Update Delivery
```http
PUT /api/Delivery/{id}
Content-Type: application/json

{
  "orderId": "string",
  "deliveryPersonId": "string",
  "pickupAddress": "string",
  "deliveryAddress": "string",
  "estimatedDeliveryDate": "datetime",
  "actualDeliveryDate": "datetime",
  "deliveryFee": 0,
  "status": "string"
}
```

### Delete Delivery
```http
DELETE /api/Delivery/{id}
```

### Get Deliveries by Delivery Person
```http
GET /api/Delivery/deliveryperson/{deliveryPersonId}
```

### Get Deliveries by Status
```http
GET /api/Delivery/status/{status}
```

---

## 👥 User Management

### Get All Customers (Admin)
```http
GET /api/Customer/All
```

### Get Customer by ID
```http
GET /api/Customer/get/{id}
```

### Update Customer Profile
```http
PUT /api/Customer/update/{id}
Content-Type: application/json

{
  "fullName": "string",
  "email": "string",
  "phoneNumber": "string",
  "address": "string",
  "city": "string",
  "country": "string"
}
```

### Get All Admins (Admin)
```http
GET /api/Admins/all
```

### Get Admin by ID
```http
GET /api/Admins/{id}
```

### Get Pending Products (Admin)
```http
GET /api/Admins/products/pending
```

### Approve Product (Admin)
```http
POST /api/Admins/products/{productId}/approve
```

### Reject Product (Admin)
```http
POST /api/Admins/products/{productId}/reject
Content-Type: application/json

"reason: string"
```

### Get All Orders (Admin)
```http
GET /api/Admins/orders
```

### Get Orders by Status (Admin)
```http
GET /api/Admins/orders/status/{status}
```

### Update Order Status (Admin)
```http
PUT /api/Admins/orders/{orderId}/status
Content-Type: application/json

"newStatus: string"
```

### Get Dashboard Statistics (Admin)
```http
GET /api/Admins/dashboard/stats
```

### Get All Tech Companies (Admin)
```http
GET /api/TechCompany
```

### Get Tech Company by ID
```http
GET /api/TechCompany/{id}
```

### Create Tech Company (Admin)
```http
POST /api/TechCompany
Content-Type: application/json

{
  "name": "string",
  "description": "string",
  "address": "string",
  "phoneNumber": "string",
  "email": "string"
}
```

### Update Tech Company (Admin)
```http
PUT /api/TechCompany/{id}
Content-Type: application/json

{
  "name": "string",
  "description": "string",
  "address": "string",
  "phoneNumber": "string",
  "email": "string"
}
```

### Delete Tech Company (Admin)
```http
DELETE /api/TechCompany/{id}
```

### Get All Delivery Persons (Admin)
```http
GET /api/DeliveryPerson
```

### Get Delivery Person by ID
```http
GET /api/DeliveryPerson/{id}
```

### Get Available Delivery Persons
```http
GET /api/DeliveryPerson/available
```

### Create Delivery Person (Admin)
```http
POST /api/DeliveryPerson
Content-Type: application/json

{
  "fullName": "string",
  "email": "string",
  "phoneNumber": "string",
  "vehicleNumber": "string",
  "vehicleType": "string"
}
```

### Update Delivery Person (Admin)
```http
PUT /api/DeliveryPerson/{id}
Content-Type: application/json

{
  "fullName": "string",
  "email": "string",
  "phoneNumber": "string",
  "vehicleNumber": "string",
  "vehicleType": "string"
}
```

### Delete Delivery Person (Admin)
```http
DELETE /api/DeliveryPerson/{id}
```

---

## 🔧 Service Usage

### Get All Service Usages
```http
GET /api/ServiceUsage
```

### Get Service Usage by ID
```http
GET /api/ServiceUsage/{id}
```

### Create Service Usage
```http
POST /api/ServiceUsage
Content-Type: application/json

{
  "maintenanceId": "string",
  "serviceType": "string",
  "description": "string",
  "cost": 0
}
```

### Update Service Usage
```http
PUT /api/ServiceUsage/{id}
Content-Type: application/json

{
  "maintenanceId": "string",
  "serviceType": "string",
  "description": "string",
  "cost": 0
}
```

### Delete Service Usage
```http
DELETE /api/ServiceUsage/{id}
```

---

## 📋 Specifications

### Get All Specifications
```http
GET /api/Specification
```

### Get Specification by ID
```http
GET /api/Specification/{id}
```

### Get Specifications by Product ID
```http
GET /api/Specification/product/{productId}
```

### Create Specification (Admin)
```http
POST /api/Specification
Content-Type: application/json

{
  "productId": "string",
  "name": "string",
  "value": "string"
}
```

### Update Specification (Admin)
```http
PUT /api/Specification
Content-Type: application/json

{
  "id": "string",
  "productId": "string",
  "name": "string",
  "value": "string"
}
```

### Delete Specification (Admin)
```http
DELETE /api/Specification/{id}
```

### Get Products by Specification
```http
GET /api/Specification/products-by-specification?key={key}&value={value}
```

### Get Products by Specification ID
```http
GET /api/Specification/products-by-specification-id/{specificationId}
```

---

## 🛡️ Warranty

### Get All Warranties
```http
GET /api/Warranty
```

### Get Warranty by ID
```http
GET /api/Warranty/{id}
```

### Create Warranty (Admin)
```http
POST /api/Warranty
Content-Type: application/json

{
  "productId": "string",
  "duration": "string",
  "description": "string",
  "terms": "string"
}
```

### Update Warranty (Admin)
```http
PUT /api/Warranty/{id}
Content-Type: application/json

{
  "productId": "string",
  "duration": "string",
  "description": "string",
  "terms": "string"
}
```

### Delete Warranty (Admin)
```http
DELETE /api/Warranty/{id}
```

---

## 🏷️ Order History

### Get All Order Histories
```http
GET /api/OrderHistories
```

### Get Order History by ID
```http
GET /api/OrderHistories/{id}
```

### Get Order History by Customer
```http
GET /api/OrderHistories/customer/{customerId}
```

---

## 🔐 Roles Management (Admin Only)

### Check if Role Exists
```http
POST /api/Roles/check-role
Content-Type: application/x-www-form-urlencoded

roleName: string (required)
```

### Assign Role to User
```http
POST /api/Roles/assign
Content-Type: application/x-www-form-urlencoded

userEmail: string (required)
roleName: string (required) - "Customer" | "Admin" | "TechCompany" | "DeliveryPerson"
```

### Unassign Role from User
```http
POST /api/Roles/unassign
Content-Type: application/x-www-form-urlencoded

userEmail: string (required)
roleName: string (required) - "Customer" | "Admin" | "TechCompany" | "DeliveryPerson"
```

### Get All Roles
```http
GET /api/Roles/all
```

### Get Registration Role Options
```http
GET /api/Roles/registration-options
```

### Get Role Enum Values
```http
GET /api/Roles/enum-values
```

---

## 📊 Enums

### Get All Enum Values
```http
GET /api/Enums/all
```

### Get Product Categories
```http
GET /api/Enums/product-categories
```

### Get Product Pending Statuses
```http
GET /api/Enums/product-pending-status
```

### Get Service Types
```http
GET /api/Enums/service-types
```

---

## 📝 Response Format

All API responses follow this standard format:

```json
{
  "success": boolean,
  "message": "string",
  "data": object | array | null
}
```

### Success Response Example:
```json
{
  "success": true,
  "message": "Product created successfully",
  "data": {
    "id": "123e4567-e89b-12d3-a456-426614174000",
    "name": "Intel Core i7",
    "price": 299.99
  }
}
```

### Pagination Response Example:
```json
{
  "success": true,
  "message": "Products retrieved successfully",
  "data": {
    "pageNumber": 1,
    "pageSize": 10,
    "totalItems": 45,
    "totalPages": 5,
    "hasPreviousPage": false,
    "hasNextPage": true,
    "previousPageNumber": null,
    "nextPageNumber": 2,
    "startItem": 1,
    "endItem": 10,
    "items": [
      {
        "id": "123e4567-e89b-12d3-a456-426614174000",
        "name": "Intel Core i7",
        "price": 299.99,
        "discountPrice": 279.99,
        "imageUrl": "https://example.com/image.jpg",
        "categoryId": "456e7890-e89b-12d3-a456-426614174000",
        "categoryName": "Processors",
        "subCategoryId": "789e0123-e89b-12d3-a456-426614174000",
        "subCategoryName": "Intel",
        "status": "Approved"
      }
    ]
  }
}
```

### Error Response Example:
```json
{
  "success": false,
  "message": "Product not found",
  "data": "Product with ID 123 not found in database"
}
```

---

## 🔧 Development Notes

### Form Data vs JSON
- **Authentication endpoints** use form data for better HTML form compatibility
- **Product endpoints** use multipart/form-data to support file uploads
- **Most other endpoints** use JSON for complex data structures

### File Uploads
- Product images are uploaded using multipart/form-data
- File field name: `img`
- Supported formats: JPG, PNG, GIF

### Pagination
- Use `pageNumber` and `pageSize` query parameters
- Default: page 1, 10 items per page
- Maximum: 100 items per page
- Enhanced pagination includes:
  - `TotalPages`: Total number of pages
  - `HasPreviousPage`: Boolean indicating if previous page exists
  - `HasNextPage`: Boolean indicating if next page exists
  - `PreviousPageNumber`: Previous page number (null if not available)
  - `NextPageNumber`: Next page number (null if not available)
  - `StartItem`: First item number on current page
  - `EndItem`: Last item number on current page

### Filtering & Sorting
- Products support filtering by category, subcategory, status, and search term
- Sorting by name, price, stock, createdDate with ascending/descending options
- Available sort options: `name`, `price`, `stock`, `createddate`

### Status Values
- **Product Status**: Pending, Approved, Rejected
- **Order Status**: Pending, Processing, Shipped, Delivered, Cancelled
- **Delivery Status**: Pending, InTransit, Delivered, Failed
- **Maintenance Status**: Pending, InProgress, Completed, Cancelled

---

## 🚀 Getting Started

1. **Start the API**: `dotnet run --project TechpertsSolutions`
2. **Access Swagger**: Navigate to `https://localhost:7230/swagger`
3. **Register a user**: Use the `/api/Authentication/register` endpoint
4. **Login**: Use the `/api/Authentication/login` endpoint to get JWT token
5. **Test endpoints**: Use the JWT token in Authorization header for protected endpoints

---

## 📞 Support

For API support and questions, contact the backend development team.

**Happy Coding! 🎉**