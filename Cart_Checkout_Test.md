# Cart Checkout Functionality Test Guide

## Overview
This guide tests the improved cart checkout functionality that includes:
- Full cart checkout
- Partial cart checkout
- Advanced checkout with delivery/service options
- Proper validation and error handling
- Transaction safety

## Test Scenarios

### 1. Full Cart Checkout

#### Test Case 1.1: Successful Full Checkout
```http
POST /api/cart/{customerId}/checkout
```

**Prerequisites:**
- Customer exists with ID
- Cart has items
- All items have sufficient stock

**Expected Result:**
- Order created successfully
- Cart cleared
- Product stock updated
- Returns order details with total amount

#### Test Case 1.2: Empty Cart Checkout
```http
POST /api/cart/{customerId}/checkout
```

**Prerequisites:**
- Customer exists
- Cart is empty

**Expected Result:**
- Returns 404 with "Cart is empty" message

#### Test Case 1.3: Insufficient Stock
```http
POST /api/cart/{customerId}/checkout
```

**Prerequisites:**
- Cart has items
- One or more items have insufficient stock

**Expected Result:**
- Returns 409 with detailed stock validation errors
- Cart remains unchanged
- No order created

### 2. Advanced Checkout with Details

#### Test Case 2.1: Checkout with Delivery and Service
```http
POST /api/cart/checkout
Content-Type: application/json

{
  "customerId": "valid-guid",
  "deliveryId": "valid-delivery-guid",
  "serviceUsageId": "valid-service-guid"
}
```

**Expected Result:**
- Order created with delivery and service references
- All other functionality same as full checkout

#### Test Case 2.2: Invalid GUID Format
```http
POST /api/cart/checkout
Content-Type: application/json

{
  "customerId": "invalid-guid-format",
  "deliveryId": "invalid-delivery-guid"
}
```

**Expected Result:**
- Returns 400 with "Invalid Customer ID format" message

### 3. Partial Checkout

#### Test Case 3.1: Successful Partial Checkout
```http
POST /api/cart/{customerId}/partial-checkout
Content-Type: application/json

{
  "productIds": ["valid-product-guid-1", "valid-product-guid-2"],
  "promoCode": "SAVE10"
}
```

**Prerequisites:**
- Cart has multiple items
- Selected products exist in cart
- Sufficient stock for selected items

**Expected Result:**
- Order created with only selected items
- Selected items removed from cart
- Other items remain in cart
- Product stock updated for selected items only

#### Test Case 3.2: Partial Checkout with Invalid Product IDs
```http
POST /api/cart/{customerId}/partial-checkout
Content-Type: application/json

{
  "productIds": ["invalid-guid", "another-invalid-guid"]
}
```

**Expected Result:**
- Returns 400 with "Invalid Product ID format" message

#### Test Case 3.3: Partial Checkout with Non-existent Products
```http
POST /api/cart/{customerId}/partial-checkout
Content-Type: application/json

{
  "productIds": ["valid-guid-but-not-in-cart"]
}
```

**Expected Result:**
- Returns 400 with "None of the selected products are in the cart" message

### 4. Error Handling Tests

#### Test Case 4.1: Invalid Customer ID
```http
POST /api/cart/invalid-customer-id/checkout
```

**Expected Result:**
- Returns 400 with "Invalid Customer ID format" message

#### Test Case 4.2: Customer Not Found
```http
POST /api/cart/{non-existent-customer-guid}/checkout
```

**Expected Result:**
- Returns 404 with "Cart not found for this customer" message

#### Test Case 4.3: Database Transaction Rollback
**Test Scenario:** Simulate database error during checkout

**Expected Result:**
- Transaction rolls back
- Cart remains unchanged
- No order created
- Product stock unchanged
- Returns 500 with error message

## API Endpoints Summary

### Full Cart Checkout
- `POST /api/cart/{customerId}/checkout` - Checkout entire cart

### Advanced Checkout
- `POST /api/cart/checkout` - Checkout with delivery/service options

### Partial Checkout
- `POST /api/cart/{customerId}/partial-checkout` - Checkout selected items only

## Response Format

### Success Response
```json
{
  "success": true,
  "message": "✅ full order placed successfully! Order ID: {orderId}, Total Amount: $123.45",
  "data": {
    "id": "order-guid",
    "customerId": "customer-guid",
    "orderDate": "2024-01-01T00:00:00Z",
    "totalAmount": 123.45,
    "status": "Pending",
    "orderItems": [
      {
        "id": "order-item-guid",
        "productId": "product-guid",
        "productName": "Product Name",
        "quantity": 2,
        "unitPrice": 50.00,
        "imageUrl": "product-image-url",
        "itemTotal": 100.00
      }
    ]
  }
}
```

### Error Response
```json
{
  "success": false,
  "message": "❌ Error description",
  "data": null
}
```

## Key Improvements Made

1. **Unified Checkout Logic**: Single `CheckoutCartAsync` method handles both full and partial checkout
2. **Transaction Safety**: Database transactions ensure atomic operations
3. **Better Validation**: Comprehensive input validation with clear error messages
4. **Consistent Error Handling**: Standardized error responses across all endpoints
5. **Removed Redundancy**: Eliminated duplicate checkout endpoints
6. **Improved Stock Management**: Proper stock validation and updates
7. **Enhanced Partial Checkout**: No more hardcoded GUIDs for delivery/service
8. **Better Documentation**: Clear API documentation and test cases

## Testing Checklist

- [ ] Full cart checkout with valid data
- [ ] Full cart checkout with empty cart
- [ ] Full cart checkout with insufficient stock
- [ ] Advanced checkout with delivery/service options
- [ ] Advanced checkout with invalid GUIDs
- [ ] Partial checkout with valid product selection
- [ ] Partial checkout with invalid product IDs
- [ ] Partial checkout with products not in cart
- [ ] Error handling for invalid customer ID
- [ ] Error handling for non-existent customer
- [ ] Transaction rollback on database errors
- [ ] Stock validation and updates
- [ ] Cart item removal after successful checkout
- [ ] Order creation with proper total calculation 