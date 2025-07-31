# Product Creation with Specifications and Warranty Test

This document demonstrates how to create a product with specifications and warranties using the updated product creation endpoint.

## Endpoint
```
POST /api/Product?category=Electronics&status=Pending
```

## Request Body Example

```json
{
  "name": "Gaming Laptop",
  "price": 1299.99,
  "description": "High-performance gaming laptop with RTX graphics",
  "stock": 50,
  "subCategoryName": "Laptops",
  "discountPrice": 1199.99,
  "techCompanyId": "your-tech-company-guid",
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
    },
    {
      "key": "RAM",
      "value": "16GB DDR4"
    },
    {
      "key": "Storage",
      "value": "512GB NVMe SSD"
    },
    {
      "key": "Display",
      "value": "15.6-inch 144Hz Full HD"
    }
  ],
  "warranties": [
    {
      "type": "Manufacturer Warranty",
      "duration": "2 Years",
      "description": "Standard manufacturer warranty covering hardware defects",
      "startDate": "2024-01-01T00:00:00Z",
      "endDate": "2026-01-01T00:00:00Z"
    },
    {
      "type": "Extended Warranty",
      "duration": "3 Years",
      "description": "Extended warranty with additional coverage",
      "startDate": "2024-01-01T00:00:00Z",
      "endDate": "2027-01-01T00:00:00Z"
    }
  ]
}
```

## Response Example

```json
{
  "success": true,
  "message": "Product created successfully and is pending approval.",
  "data": {
    "id": "product-guid",
    "name": "Gaming Laptop",
    "price": 1299.99,
    "description": "High-performance gaming laptop with RTX graphics",
    "stock": 50,
    "discountPrice": 1199.99,
    "status": "Pending",
    "category": {
      "id": "category-guid",
      "name": "Electronics"
    },
    "subCategory": {
      "id": "subcategory-guid",
      "name": "Laptops"
    },
    "techCompany": {
      "id": "tech-company-guid",
      "name": "Your Tech Company"
    },
    "specifications": [
      {
        "id": "spec-guid-1",
        "key": "Processor",
        "value": "Intel Core i7-12700H"
      },
      {
        "id": "spec-guid-2",
        "key": "Graphics",
        "value": "NVIDIA RTX 3070 8GB"
      },
      {
        "id": "spec-guid-3",
        "key": "RAM",
        "value": "16GB DDR4"
      },
      {
        "id": "spec-guid-4",
        "key": "Storage",
        "value": "512GB NVMe SSD"
      },
      {
        "id": "spec-guid-5",
        "key": "Display",
        "value": "15.6-inch 144Hz Full HD"
      }
    ],
    "warranties": [
      {
        "id": "warranty-guid-1",
        "type": "Manufacturer Warranty",
        "duration": "2 Years",
        "description": "Standard manufacturer warranty covering hardware defects",
        "startDate": "2024-01-01T00:00:00Z",
        "endDate": "2026-01-01T00:00:00Z"
      },
      {
        "id": "warranty-guid-2",
        "type": "Extended Warranty",
        "duration": "3 Years",
        "description": "Extended warranty with additional coverage",
        "startDate": "2024-01-01T00:00:00Z",
        "endDate": "2027-01-01T00:00:00Z"
      }
    ],
    "imageUrls": [
      "https://example.com/laptop1.jpg",
      "https://example.com/laptop2.jpg",
      "https://example.com/laptop3.jpg",
      "https://example.com/laptop4.jpg"
    ]
  }
}
```

## Features Implemented

1. **Product Creation with Specifications**: The endpoint now supports creating multiple specifications for a product during creation
2. **Product Creation with Warranties**: The endpoint now supports creating multiple warranties for a product during creation
3. **Validation**: All specifications and warranties are validated according to their respective DTO requirements
4. **Transaction Safety**: All operations are performed within a single transaction to ensure data consistency
5. **Admin Notification**: When a product is created, an automatic notification is sent to admin users

## Notes

- The `specifications` and `warranties` arrays are optional - you can create a product without them
- **Important**: Do NOT include `id` fields in specifications and warranties when creating a product - these will be automatically generated
- All specifications and warranties are automatically associated with the created product
- The product status is set to "Pending" by default and requires admin approval
- Date formats should be in ISO 8601 format (YYYY-MM-DDTHH:mm:ssZ)
- Make sure the `techCompanyId` exists in the database before creating the product 