# Get Available TechCompanies

To fix the Foreign Key constraint error, you need to use a valid `techCompanyId` that exists in the database.

## Step 1: Get All TechCompanies

Make a GET request to:
```
GET /api/TechCompany
```

This will return all available TechCompanies with their IDs.

## Step 2: Use a Valid TechCompanyId

Once you get the response, use one of the valid `techCompanyId` values in your product creation request.

## Example Response Expected

```json
{
  "success": true,
  "message": "TechCompanies retrieved successfully.",
  "data": [
    {
      "id": "valid-guid-here",
      "mapLocation": "Some Location",
      "latitude": 0.0,
      "longitude": 0.0,
      "address": "Some Address",
      "city": "Some City",
      "country": "Some Country",
      "postalCode": "12345",
      "phoneNumber": "+1234567890",
      "website": "https://example.com",
      "description": "Some description",
      "isActive": true,
      "userId": "user-guid-here",
      "user": {
        "id": "user-guid-here",
        "fullName": "Company Name",
        "email": "company@example.com"
      }
    }
  ]
}
```

## Step 3: Updated Product Creation Request

Use the valid `techCompanyId` in your product creation request:

```json
{
  "name": "Gaming Laptop",
  "price": 1299.99,
  "description": "High-performance gaming laptop with RTX graphics",
  "stock": 50,
  "subCategoryName": "Laptops",
  "discountPrice": 1199.99,
  "techCompanyId": "valid-guid-from-step-1",  // Use the actual ID from step 1
  "image1Url": "https://example.com/laptop1.jpg",
  "image2Url": "https://example.com/laptop2.jpg",
  "image3Url": "https://example.com/laptop3.jpg",
  "image4Url": "https://example.com/laptop4.jpg",
  "specifications": [
    {
      "key": "Processor",
      "value": "Intel Core i7-12700H"
    },
    {
      "key": "Graphics",
      "value": "NVIDIA RTX 3070 8GB"
    }
  ],
  "warranties": [
    {
      "type": "Manufacturer Warranty",
      "duration": "2 Years",
      "description": "Standard manufacturer warranty",
      "startDate": "2024-01-01T00:00:00Z",
      "endDate": "2026-01-01T00:00:00Z"
    }
  ]
}
```

## Alternative: Create a TechCompany First

If no TechCompanies exist, you can create one first:

```json
POST /api/TechCompany

{
  "mapLocation": "Test Location",
  "latitude": 0.0,
  "longitude": 0.0,
  "address": "Test Address",
  "city": "Test City",
  "country": "Test Country",
  "postalCode": "12345",
  "phoneNumber": "+1234567890",
  "website": "https://test.com",
  "description": "Test Tech Company",
  "isActive": true,
  "userId": "existing-user-guid"  // You need a valid user ID
}
```

## Common Issues

1. **Invalid TechCompanyId**: Make sure the ID exists in the database
2. **Invalid UserId**: If creating a TechCompany, make sure the userId exists
3. **GUID Format**: Ensure the ID is in proper GUID format (e.g., "12345678-1234-1234-1234-123456789012") 