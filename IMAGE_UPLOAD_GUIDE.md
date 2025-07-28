# Image Upload System Guide

This guide explains how to use the image upload functionality in the TechpertsSolutions API.

## Overview

The image upload system allows you to upload images for various entities (Products, Categories, etc.) and stores them in an organized folder structure within the `wwwroot/assets` directory.

## Features

- **File Validation**: Validates image file type and size
- **Organized Storage**: Images are stored in controller-specific folders
- **Unique Naming**: Generates unique filenames to prevent conflicts
- **URL Generation**: Automatically generates accessible URLs for uploaded images
- **File Management**: Supports upload, update, and delete operations

## Supported Image Formats

- JPEG (.jpg, .jpeg)
- PNG (.png)
- GIF (.gif)
- BMP (.bmp)
- WebP (.webp)

## File Size Limits

- Maximum file size: 5MB per image

## Folder Structure

```
wwwroot/
└── assets/
    ├── products/
    │   ├── product1_20241201120000_abc12345.jpg
    │   └── product2_20241201120001_def67890.png
    ├── categories/
    │   ├── category1_20241201120002_ghi11111.jpg
    │   └── category2_20241201120003_jkl22222.png
    └── [other-controllers]/
        └── [images]/
```

## API Endpoints

### Generic Image Upload Controller

#### Upload Image
```
POST /api/ImageUpload/upload/{controllerName}
```

**Parameters:**
- `controllerName` (string): The name of the controller (e.g., "products", "categories")
- `imageFile` (IFormFile): The image file to upload

**Response:**
```json
{
  "success": true,
  "message": "Image uploaded successfully",
  "data": {
    "success": true,
    "message": "Image uploaded successfully",
    "imagePath": "assets/products/product_20241201120000_abc12345.jpg",
    "imageUrl": "https://localhost:7001/assets/products/product_20241201120000_abc12345.jpg"
  }
}
```

#### Delete Image
```
DELETE /api/ImageUpload/delete?imagePath={imagePath}
```

**Parameters:**
- `imagePath` (string): The relative path of the image to delete

#### Validate Image
```
GET /api/ImageUpload/validate
```

**Parameters:**
- `imageFile` (IFormFile): The image file to validate

### Product-Specific Endpoints

#### Upload Product Image
```
POST /api/Product/{productId}/upload-image
```

**Parameters:**
- `productId` (string): The ID of the product
- `imageFile` (IFormFile): The image file to upload

#### Delete Product Image
```
DELETE /api/Product/{productId}/delete-image
```

**Parameters:**
- `productId` (string): The ID of the product

### Category-Specific Endpoints

#### Upload Category Image
```
POST /api/Category/{categoryId}/upload-image
```

**Parameters:**
- `categoryId` (string): The ID of the category
- `imageFile` (IFormFile): The image file to upload

#### Delete Category Image
```
DELETE /api/Category/{categoryId}/delete-image
```

**Parameters:**
- `categoryId` (string): The ID of the category

## Usage Examples

### Using cURL

#### Upload a Product Image
```bash
curl -X POST "https://localhost:7001/api/Product/123e4567-e89b-12d3-a456-426614174000/upload-image" \
  -H "Content-Type: multipart/form-data" \
  -F "imageFile=@/path/to/your/image.jpg"
```

#### Upload Using Generic Controller
```bash
curl -X POST "https://localhost:7001/api/ImageUpload/upload/products" \
  -H "Content-Type: multipart/form-data" \
  -F "imageFile=@/path/to/your/image.jpg"
```

### Using JavaScript/Fetch

#### Upload Product Image
```javascript
const formData = new FormData();
formData.append('imageFile', fileInput.files[0]);

fetch(`/api/Product/${productId}/upload-image`, {
  method: 'POST',
  body: formData
})
.then(response => response.json())
.then(data => {
  if (data.success) {
    console.log('Image uploaded:', data.data.imageUrl);
  } else {
    console.error('Upload failed:', data.message);
  }
});
```

### Using Postman

1. Set the request method to `POST`
2. Set the URL to the appropriate endpoint
3. In the request body, select "form-data"
4. Add a key named "imageFile" with type "File"
5. Select your image file
6. Send the request

## Error Handling

### Common Error Responses

#### Invalid File Type
```json
{
  "success": false,
  "message": "Invalid image file. Please upload a valid image (jpg, jpeg, png, gif, bmp, webp) with size less than 5MB"
}
```

#### File Too Large
```json
{
  "success": false,
  "message": "Invalid image file. Please upload a valid image (jpg, jpeg, png, gif, bmp, webp) with size less than 5MB"
}
```

#### No File Provided
```json
{
  "success": false,
  "message": "No image file provided"
}
```

#### Entity Not Found
```json
{
  "success": false,
  "message": "Product not found"
}
```

## Configuration

### Base URL Configuration

The base URL for image URLs is configured in `appsettings.json`:

```json
{
  "BaseUrl": "https://localhost:7001"
}
```

### File Service Configuration

The file service is registered in `Program.cs`:

```csharp
builder.Services.AddScoped<IFileService, FileService>();
```

## Security Considerations

1. **File Type Validation**: Only allows specific image file types
2. **File Size Limits**: Prevents large file uploads
3. **Unique Naming**: Prevents filename conflicts and potential security issues
4. **Organized Storage**: Keeps files organized by controller type

## Best Practices

1. **Always validate files** on the client side before uploading
2. **Handle errors gracefully** and provide user-friendly error messages
3. **Use appropriate file formats** (JPEG for photos, PNG for graphics with transparency)
4. **Optimize images** before uploading to reduce file size
5. **Implement proper authentication** for upload endpoints in production

## Troubleshooting

### Common Issues

1. **File not found errors**: Ensure the `wwwroot` directory exists and is writable
2. **Permission errors**: Check file system permissions for the application
3. **URL not accessible**: Verify the `BaseUrl` configuration in `appsettings.json`
4. **File size errors**: Ensure uploaded files are under 5MB

### Debugging

Enable detailed logging in `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Service.FileService": "Debug"
    }
  }
}
```

## Extending the System

To add image upload support for a new entity:

1. **Add the interface method** to the appropriate service interface
2. **Implement the method** in the service class
3. **Add controller endpoints** for upload/delete operations
4. **Update DTOs** if necessary to handle image paths
5. **Test the functionality** thoroughly

Example for a new entity:

```csharp
// In IYourEntityService
Task<GeneralResponse<ImageUploadResponseDTO>> UploadYourEntityImageAsync(IFormFile imageFile, string entityId);
Task<GeneralResponse<bool>> DeleteYourEntityImageAsync(string entityId);

// In YourEntityService
public async Task<GeneralResponse<ImageUploadResponseDTO>> UploadYourEntityImageAsync(IFormFile imageFile, string entityId)
{
    // Implementation similar to ProductService
}

// In YourEntityController
[HttpPost("{entityId}/upload-image")]
public async Task<IActionResult> UploadYourEntityImage(string entityId, IFormFile imageFile)
{
    // Implementation similar to ProductController
}
``` 