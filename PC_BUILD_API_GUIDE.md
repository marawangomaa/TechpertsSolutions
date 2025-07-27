# PC Build API Guide

This guide explains how to use the enhanced PCAssemblyController for PC Build functionality.

## Overview

The PC Build system allows users to:
- Browse components by category
- Add components to their PC build
- Remove components from their build
- View build status and total price
- Get compatible component recommendations

## API Endpoints

### 1. Get PC Component Categories
**GET** `/api/PCAssembly/build/categories`

Returns all available PC component categories for the build system.

**Response:**
```json
{
  "success": true,
  "message": "PC component categories retrieved successfully.",
  "data": [
    {
      "value": 0,
      "name": "Processor",
      "displayName": "Processor"
    },
    {
      "value": 1,
      "name": "Motherboard", 
      "displayName": "Motherboard"
    },
    // ... other categories
  ]
}
```

### 2. Get Components by Category
**GET** `/api/PCAssembly/build/components/{category}`

Loads products for a specific PC component category.

**Parameters:**
- `category`: ProductCategory enum value (e.g., "Processor", "Motherboard")
- `pageNumber`: Page number for pagination (default: 1)
- `pageSize`: Items per page (default: 20)
- `search`: Search term (optional)
- `sortBy`: Sort field (default: "name")
- `sortDesc`: Sort direction (default: false)

**Example:**
```
GET /api/PCAssembly/build/components/Processor?pageNumber=1&pageSize=10
```

### 3. Add Component to PC Build
**POST** `/api/PCAssembly/build/{assemblyId}/add-component`

Adds a specific product to the PC build.

**Request Body:**
```json
{
  "productId": "product-guid-here"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Component added to PC build successfully.",
  "data": {
    "id": "assembly-id",
    "customerId": "customer-id",
    "name": "My PC Build",
    "createdAt": "2025-01-20T10:00:00Z",
    "items": [
      {
        "id": "item-id",
        "productId": "product-id",
        "quantity": 1,
        "price": 299.99,
        "total": 299.99
      }
    ]
  }
}
```

### 4. Remove Component from PC Build
**DELETE** `/api/PCAssembly/build/{assemblyId}/remove-component/{itemId}`

Removes a component from the PC build.

**Response:**
```json
{
  "success": true,
  "message": "Component removed from PC build successfully.",
  "data": {
    // Updated assembly data
  }
}
```

### 5. Get PC Build Status
**GET** `/api/PCAssembly/build/{assemblyId}/status`

Returns the current status of the PC build with detailed component information.

**Response:**
```json
{
  "success": true,
  "message": "PC Build status retrieved successfully.",
  "data": {
    "assemblyId": "assembly-id",
    "customerId": "customer-id",
    "name": "My PC Build",
    "createdAt": "2025-01-20T10:00:00Z",
    "totalPrice": 1299.99,
    "components": [
      {
        "id": "item-id",
        "productId": "product-id",
        "productName": "Intel Core i7-12700K",
        "productImage": "image-url",
        "category": "Processor",
        "subCategory": "Desktop Processors",
        "price": 299.99,
        "discountPrice": null,
        "quantity": 1,
        "total": 299.99,
        "isSelected": true
      }
    ],
    "componentStatus": {
      "Processor": true,
      "Motherboard": false,
      "CPUCooler": false,
      "Case": false,
      "GraphicsCard": false,
      "RAM": false,
      "Storage": false,
      "CaseCooler": false,
      "PowerSupply": false,
      "Monitor": false,
      "Accessories": false
    }
  }
}
```

### 6. Calculate Build Total
**GET** `/api/PCAssembly/build/{assemblyId}/total`

Calculates the total price of the PC build.

**Response:**
```json
{
  "success": true,
  "message": "Build total calculated successfully.",
  "data": {
    "assemblyId": "assembly-id",
    "subtotal": 1299.99,
    "discount": 0.00,
    "total": 1299.99,
    "totalComponents": 11,
    "selectedComponents": 3
  }
}
```

### 7. Get Compatible Components
**GET** `/api/PCAssembly/build/compatible/{productId}`

Returns compatible components for a specific product (for recommendations).

**Response:**
```json
{
  "success": true,
  "message": "Compatible components retrieved successfully.",
  "data": [
    {
      "productId": "product-id",
      "productName": "AMD Ryzen 7 5800X",
      "productImage": "image-url",
      "category": "Processor",
      "price": 349.99,
      "discountPrice": null,
      "compatibilityReason": "Same category component",
      "compatibilityScore": 0.8
    }
  ]
}
```

## Frontend Integration

### PC Build Page Workflow

1. **Load Categories**: Call `/api/PCAssembly/build/categories` to get available component types
2. **Display Build Table**: Use `/api/PCAssembly/build/{assemblyId}/status` to show current build
3. **Add Component**: 
   - Call `/api/PCAssembly/build/components/{category}` to load products for a category
   - User selects a product
   - Call `/api/PCAssembly/build/{assemblyId}/add-component` to add it
4. **Remove Component**: Call `/api/PCAssembly/build/{assemblyId}/remove-component/{itemId}`
5. **Show Total**: Call `/api/PCAssembly/build/{assemblyId}/total` to display pricing

### Example Frontend Usage

```javascript
// Load PC build status
async function loadPCBuild(assemblyId) {
  const response = await fetch(`/api/PCAssembly/build/${assemblyId}/status`);
  const data = await response.json();
  
  if (data.success) {
    displayPCBuildTable(data.data);
  }
}

// Load components for a category
async function loadComponents(category) {
  const response = await fetch(`/api/PCAssembly/build/components/${category}`);
  const data = await response.json();
  
  if (data.success) {
    displayComponentSelection(data.data);
  }
}

// Add component to build
async function addComponent(assemblyId, productId) {
  const response = await fetch(`/api/PCAssembly/build/${assemblyId}/add-component`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({ productId })
  });
  
  const data = await response.json();
  if (data.success) {
    // Refresh the build display
    loadPCBuild(assemblyId);
  }
}
```

## Component Categories

The system supports these PC component categories:
- **Processor**: CPUs
- **Motherboard**: Motherboards
- **CPUCooler**: CPU cooling solutions
- **Case**: PC cases
- **GraphicsCard**: GPUs
- **RAM**: Memory modules
- **Storage**: SSDs, HDDs
- **CaseCooler**: Case fans
- **PowerSupply**: PSUs
- **Monitor**: Displays
- **Accessories**: Other components

## Error Handling

All endpoints return consistent error responses:

```json
{
  "success": false,
  "message": "Error description",
  "data": null
}
```

Common error scenarios:
- Invalid assembly ID
- Product not found
- Component already exists in build
- Database connection issues

## Notes

- Each component category can only have one product selected at a time
- Adding a new component of the same category replaces the existing one
- The system automatically calculates totals and discounts
- Component compatibility is based on category matching (can be enhanced with more sophisticated logic) 